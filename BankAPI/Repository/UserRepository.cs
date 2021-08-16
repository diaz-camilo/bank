
//using System;
//using System.Threading.Tasks;
//using BankAPI.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;

//namespace BankAPI.Repository
//{
//    public class UserRepository : IUserRepository
//    {
//        private readonly UserManager<AppUser> _userManager;
//        private readonly RoleManager<AppRole> _roleManager;
//        private readonly SignInManager<AppUser> _signInManager;

//        public UserRepository(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _signInManager = signInManager;
//        }

        

//        public async Task LockUser(string LoginID)
//        {

//            return SetUserAccess(LoginID, true);
//        }

//        public async Task UnlockUser(string LoginID)
//        {
//            return SetUserAccess(LoginID, false);
//        }

//        private async Task SetUserAccess(string LoginID, bool isLocked)
//        {
//            var user = await _userManager.FindByNameAsync(LoginID);

//            user.IsLocked = isLocked;

//            _userManager.

//        }
        
//    }
//}
