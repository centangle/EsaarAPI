using BusinessLogic;
using Microsoft.Owin.Security.Infrastructure;
using Models;
using System;
using System.Threading.Tasks;
using Helpers;

namespace DataManager.Providers
{
    public class AuthRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private Logic _logic;
        public AuthRefreshTokenProvider()
        {
            _logic = new Logic();
        }
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var refreshTokenLifeTime = "262800";//6 months
            var refreshTokenId = Guid.NewGuid().ToString();
            //Creating the Refresh Token object
            var token = new RefreshTokenModel()
            {
                //storing the RefreshTokenId in hash format
                Id = Encryption.GetHash(refreshTokenId),// Hased Refresh Token Id
                UserId = context.Ticket.Properties.Dictionary["userId"],
                IssuedTime = DateTime.UtcNow,
                ExpiredTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };

            //Setting the Issued and Expired time of the Refresh Token
            context.Ticket.Properties.IssuedUtc = token.IssuedTime;
            context.Ticket.Properties.ExpiresUtc = token.ExpiredTime;

            token.ProtectedTicket = context.SerializeTicket();// Contains all the claims of user
            var result = await _logic.UpdateRefreshToken(token);

            if (result)
            {
                context.SetToken(refreshTokenId);
            }
        }
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            string hashedTokenId = Encryption.GetHash(context.Token);
            var refreshToken = await _logic.FindRefreshToken(hashedTokenId);
            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                await _logic.RemoveRefreshToken(refreshToken);
            }
        }
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

    }
}