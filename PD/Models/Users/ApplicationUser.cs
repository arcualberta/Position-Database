using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Users
{
    public class ApplicationUser : IdentityUser
    {
        [Key]
        [MaxLength(128)]
        public override string Id { get; set; }
    }
}
