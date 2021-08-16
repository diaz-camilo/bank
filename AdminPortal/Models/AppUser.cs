using System;
using Microsoft.AspNetCore.Identity;

namespace AdminPortal.Models
{
    public class AppUser : IdentityUser<int>
    {
        public int? CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        public bool IsLocked { get; set; }
    }
}
