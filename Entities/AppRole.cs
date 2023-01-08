using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class AppRole
    {

        [Key]
        public int RoleId { get; set; }

        public string RoleName { get; set; }
        public ICollection<AppUserRoles> UserRoles { get; set; }
    }
}