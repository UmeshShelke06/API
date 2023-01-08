using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TokenController : BaseApiController
    {
        private readonly DataContext _userContext;
        private readonly ITokenService _tokenService;
        public TokenController(DataContext userContext, ITokenService TokenService)
        {
            this._userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            this._tokenService = TokenService ?? throw new ArgumentNullException(nameof(TokenService));
        }


        [HttpPost, Route("Refresh")]
        public IActionResult Refresh(TokenAPIModel tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default
            var user = _userContext.Users.SingleOrDefault(u => u.UserName == username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;

            _userContext.SaveChanges();


            return Ok(new UserDto()
            {
                UserName = username,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpGet, Route("Revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            var user = _userContext.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest();
            user.RefreshToken = null;
            _userContext.SaveChanges();
            return NoContent();
        }

      
    }
}