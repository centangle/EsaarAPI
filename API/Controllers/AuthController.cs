using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using BusinessLogic;
using Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace API.Controllers
{
    public class AuthController : Controller
    {
        public class RefreshRequest
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }

        public class LoginResponse
        {
            public string AccessToken { get; set; }
            public DateTimeOffset AccessTokenExpiration { get; set; }
            public string RefreshToken { get; set; }
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly Logic _logic;
        public AuthController(ApplicationDbContext context,
            Logic logic,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _logic = logic;
        }
        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string grant_type)
        {
            if (await IsValidUsernameAndPassword(username, password))
            {
                return new ObjectResult(await GenerateToken(username));
            }
            else
            {
                return BadRequest("Invalid username or password");
            }
        }

        [HttpPost]
        [Route("/refresh")]
        public async Task<IActionResult> Refresh(string token, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;
            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                string hashedTokenId = Encryption.GetHash(refreshToken);
                var savedRefreshToken = await _logic.FindRefreshToken(hashedTokenId);
                if (savedRefreshToken.Id != hashedTokenId || userId != savedRefreshToken.UserId)
                {
                    throw new SecurityTokenException("Invalid refresh token");
                }
                else if (savedRefreshToken.ExpiredTime < DateTime.Now)
                {
                    throw new SecurityTokenException("Refresh token has Expired");
                }
                else
                {
                    return new ObjectResult(await GenerateToken(username));
                }
            }
            throw new KnownException("Invalid Token");
        }
        #region[Token Helper Functions]
        private async Task<bool> IsValidUsernameAndPassword(string username, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(username);
                return await _userManager.CheckPasswordAsync(user, password);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private async Task<dynamic> GenerateToken(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            var roles = from ur in _context.UserRoles
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select new { ur.UserId, ur.RoleId, r.Name };
            var member = await _logic.GetMemberByAuthId(user.Id);
            int memberId = member == null ? 0 : member.Id;
            DateTime tokenExpiration = DateTime.Now.AddDays(1) ;
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,username),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim("memberId",memberId.ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf,new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp,new DateTimeOffset(tokenExpiration).ToUnixTimeSeconds().ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshToken(user.Id, refreshToken);
            var token = new JwtSecurityToken(
                 new JwtHeader(
                     new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Secrets:SecurityKey"))),
                     SecurityAlgorithms.HmacSha256)),
                     new JwtPayload(claims));
            var output = new
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                token_type = "bearer",
                expires_in = tokenExpiration,
                user_name = username,
                refresh_token = refreshToken,
                roles = string.Join(',', roles.Select(x => x.Name)),
            };
            return output;
        }
        #endregion

        #region[Refresh Token Helper Functions]
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Secrets:SecurityKey"))),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private async Task<bool> SaveRefreshToken(string userId, string refreshToken)
        {
            var refreshTokenLifeTime = "262800";//6 months
            //Creating the Refresh Token object
            var token = new RefreshTokenModel()
            {
                //storing the RefreshToken in hash format
                Id = Encryption.GetHash(refreshToken),// Hased Refresh Token 
                UserId = userId,
                IssuedTime = DateTime.UtcNow,
                ExpiredTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };
            return await _logic.UpdateRefreshToken(token);
        }
        #endregion
    }
}