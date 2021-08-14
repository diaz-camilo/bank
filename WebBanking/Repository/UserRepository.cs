
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebBanking.Enums;
using WebBanking.Models;
using WebBanking.ViewModels;

namespace WebBanking.Repository
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


        public async Task<IdentityResult> CreateUserAsync(SignupUser signupUser)
        {
            var user = new AppUser()
            {
                UserName = signupUser.LoginID,
                CustomerID = signupUser.CustomerID,
            };

            var result = await _userManager.CreateAsync(user, signupUser.password);
            await _userManager.AddToRoleAsync(user, RoleEnum.Customer.ToString());

            return result;
        }

        public async Task<IdentityResult> AssignRoleAsync(string userName, RoleEnum role)
        {
            var result = await _userManager.AddToRoleAsync(new AppUser { UserName = userName }, role.ToString());

            return result;
        }

        public async Task CreateRolesAsync()
        {
            var rolesArray = Enum.GetNames<RoleEnum>();

            for (int i = 0; i < rolesArray.Length; i ++)
            {
               await _roleManager.CreateAsync(new AppRole { Name = rolesArray[i], NormalizedName = rolesArray[i], ConcurrencyStamp = rolesArray[i] });
            }
            
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

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model, string CustomerID)
        {

            var user = await _userManager.FindByNameAsync(CustomerID);
            //FindByIdAsync(CustomerID);
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            return result;
        }
    }
}
