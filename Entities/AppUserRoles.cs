using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppUserRoles
    {
        
        [Key]
        public int UserRoleId { get; set; }
        public LoginModel User { get; set; }
        
        public AppRole Role { get; set; }

        [ForeignKey("LoginModel")]
        public int UserId { get; set; }

        [ForeignKey("AppRole")]
        public int RoleId { get; set; }
    }
}