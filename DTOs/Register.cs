using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class Register
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }

        public string? UserEmail { get; set; }
    }
}