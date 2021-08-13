using System;
using Microsoft.AspNetCore.Identity;

namespace WebBanking.Models
{
    public class AppUser : IdentityUser<int>
    {
        public int? CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
