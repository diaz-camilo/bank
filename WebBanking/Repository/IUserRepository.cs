using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebBanking.Models;
using WebBanking.ViewModels;

namespace WebBanking.Repository
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(SignupUser signupUser);
        Task<SignInResult> LoginUserAsync(LoginViewModel login);
        Task LogoutUserAsync();
    }
}