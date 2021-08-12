
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebBanking.Models;
using WebBanking.ViewModels;

namespace WebBanking.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<IdentityResult> CreateUserAsync(SignupUser signupUser)
        {
            

            var user = new AppUser()
            {
                //Id = signupUser.LoginID,
                UserName = signupUser.LoginID.ToString(),
                CustomerID = signupUser.CustomerID,
            };

            var result = await _userManager.CreateAsync(user,signupUser.password);
            return result;
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
