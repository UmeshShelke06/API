using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace API.Controllers
{

    public class AccountController : BaseApiController
    {

        private readonly DataContext _userContext;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext userContext, ITokenService TokenService)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _tokenService = TokenService ?? throw new ArgumentNullException(nameof(TokenService));
        }


        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] Login loginModel)
        {

            if (loginModel is null)
            {
                return BadRequest("Invalid client request");
            }


            var user = _userContext.Users.FirstOrDefault(u =>
                (u.UserName == loginModel.UserName));

            if (user is null) return Unauthorized();

            var mac = new HMACSHA512(user.PasswordSalt);

            var UserHashPassWord = mac.ComputeHash(Encoding.UTF8.GetBytes(loginModel.Password));

            if (_tokenService.PasswordMatchOrNot(user.PasswordHash, UserHashPassWord) == true)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginModel.UserName),
                    new Claim(ClaimTypes.Role, "User")
                };
                var accessToken = _tokenService.GenerateAccessToken(claims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

                _userContext.SaveChanges();

                return Ok(new UserDto
                {
                    UserName = user.UserName,
                    Token = accessToken,
                    RefreshToken = refreshToken,
                });
            }


            return Unauthorized();
        }


        [HttpPost, Route("Register")]
        public IActionResult Register([FromBody] Register register)
        {
            if (register is null)
            {
                return BadRequest("Invalid client request");
            }

            if (UserExists(register)) return BadRequest("User Name is Taken ");

            var mac = new HMACSHA512();

            var userHashPassWord = mac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
            var userPasswordSalt = mac.Key;



            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, register.UserName),
            new Claim(ClaimTypes.Role, "User")
        };
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.Now.AddDays(7);

            var user = new LoginModel
            {

                UserEmail = register.UserEmail,
                UserName = register.UserName,
                PasswordHash = userHashPassWord,
                PasswordSalt = userPasswordSalt,
                RefreshToken = refreshToken,
                AccessToken = accessToken,
                RefreshTokenExpiryTime = refreshTokenExpiryTime
            };

            _userContext.Users.Add(user);
            _userContext.SaveChanges();



            return Ok(new UserDto
            {
                UserName = user.UserName,
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }


        [Authorize]
        [HttpGet, Route("GetUsers")]
        public IActionResult GetUsers()
        {
            var userInfo = _userContext.Users;
            
            var userDetails = userInfo.OrderBy(u => u.UserName)
            .Select( u =>  
            new UserDetails() { UserName = u.UserName , UserEmail = u.UserEmail}).ToList();
          
            return Ok(userDetails);
        }
        public bool UserExists(Register register)
        {
            bool UserExists = false;
            try
            {

                UserExists = _userContext.Users.Any(u =>
                   (u.UserName == register.UserName));
            }
            catch (Exception e)
            {

            }

            return UserExists;
        }


    }
}
