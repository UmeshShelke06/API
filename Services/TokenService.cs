using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using API.Interfaces;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            // var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));

            // var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.);

            // var tokeOptions = new JwtSecurityToken(
            //     issuer: "https://localhost:5001",
            //     audience: "https://localhost:5001",
            //     claims: claims,
            //     expires: DateTime.Now.AddMinutes(5),
            //     signingCredentials: signinCredentials
            // );
            
            // var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);


            
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);


           // return tokenString;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

         public bool PasswordMatchOrNot(byte[] userDBPassword, byte[]  hashpasword)
        {
            bool PasswordMatchOrNot = false;
           for(int i=0; i < hashpasword.Length; i++)
           {
               if(hashpasword[i] != userDBPassword[i])
               {
                PasswordMatchOrNot = false;
                return PasswordMatchOrNot;
               }
               else{
                PasswordMatchOrNot = true;
               }

           }

          return  PasswordMatchOrNot;
        }

    }
}