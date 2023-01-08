using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class UserDto
    {
        public string? UserName { get; set; }
        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public string? RoleName { get; set; }
    
    }
}