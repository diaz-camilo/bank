using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AdminPortal.Models
{
    public class AppUserClaimsPrincipalFactory :
        UserClaimsPrincipalFactory<AppUser, AppRole>
    {
        public AppUserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IOptions<IdentityOptions> options)
            : base (userManager,roleManager,options)
        {

        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            //if (user.CustomerID.HasValue)
            //{
            //    identity.AddClaim(new Claim(nameof(Customer) + nameof(Customer.Name), user.Customer.Name ?? ""));
            //    identity.AddClaim(new Claim(nameof(Customer.CustomerID), user.CustomerID.ToString() ?? "0"));
            //}
            

            return identity;
        }
    }
}
