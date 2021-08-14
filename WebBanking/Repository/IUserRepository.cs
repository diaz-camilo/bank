using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebBanking.Enums;
using WebBanking.Models;
using WebBanking.ViewModels;

namespace WebBanking.Repository
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(SignupUser signupUser);
        Task<SignInResult> LoginUserAsync(LoginViewModel login);
        Task LogoutUserAsync();
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model, string CustomerID);
        Task<IdentityResult> AssignRoleAsync(string userName, RoleEnum role);
        Task CreateRolesAsync();
    }
}