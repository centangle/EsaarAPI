using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.Models;
using API.Services;
using AutoMapper;
using BusinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Logic _logic;

        public AccountController(UserManager<IdentityUser> userManager,
            Logic logic
            )
        {
            _userManager = userManager;
            _logic = logic;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            MemberModel memberModel = new MemberModel();
            memberModel.AuthUserId = user.Id;
            memberModel.Name = model.Name;
            memberModel.NativeName = model.NativeName;
            memberModel.Address.MobileNo = model.MobileNo;
            memberModel.Address.Email = model.Email;
            memberModel.Address.Type = Catalogs.AddressTypeCatalog.Default;
            try
            {
                await _logic.AddMember(memberModel);
            }
            catch(Exception ex)
            {

            }

           

            return Ok();
        }
        //private IUserService _userService;
        //private IMapper _mapper;
        //private readonly AppSettings _appSettings;

        //public AccountController(
        //    IUserService userService,
        //    IMapper mapper,
        //    IOptions<AppSettings> appSettings)
        //{
        //    _userService = userService;
        //    _mapper = mapper;
        //    _appSettings = appSettings.Value;
        //}




        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    var users = _userService.GetAll();
        //    var model = _mapper.Map<IList<UserModel>>(users);
        //    return Ok(model);
        //}

        //[HttpGet("{id}")]
        //public IActionResult GetById(int id)
        //{
        //    var user = _userService.GetById(id);
        //    var model = _mapper.Map<UserModel>(user);
        //    return Ok(model);
        //}

        //[HttpPut("{id}")]
        //public IActionResult Update(int id, [FromBody]UpdateModel model)
        //{
        //    // map model to entity and set id
        //    var user = _mapper.Map<User>(model);
        //    user.Id = id;

        //    try
        //    {
        //        // update user 
        //        _userService.Update(user, model.Password);
        //        return Ok();
        //    }
        //    catch (AppException ex)
        //    {
        //        // return error message if there was an exception
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        //[HttpDelete("{id}")]
        //public IActionResult Delete(int id)
        //{
        //    _userService.Delete(id);
        //    return Ok();
        //}
    }
}