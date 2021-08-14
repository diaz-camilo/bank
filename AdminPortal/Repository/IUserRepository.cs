using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AdminPortal.ViewModels;

namespace AdminPortal.Repository
{
    public interface IUserRepository
    {
        
        Task<SignInResult> LoginUserAsync(LoginViewModel login);
        Task LogoutUserAsync();
        
    }
}