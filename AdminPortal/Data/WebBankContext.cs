using AdminPortal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminPortal.Data
{

    // Context for authentication and authorization use only
    public class WebBankContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public WebBankContext(DbContextOptions<WebBankContext> options) : base(options)
        {

        }
    }
}