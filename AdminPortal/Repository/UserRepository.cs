
using System;
using System.Threading.Tasks;
using AdminPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using AdminPortal.ViewModels;

namespace AdminPortal.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserRepository(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        

        public async Task<SignInResult> LoginUserAsync(LoginViewModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.LoginID, login.Password, true, false);

            return result;
        }

        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }
        
    }
}
