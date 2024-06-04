using PatientWebApi.Models;

namespace PatientWebApi.Services
{
    public interface IAuthService
    {
        Task<bool> Register(LoginUser user);
        Task<bool> Login(LoginUser user);
        string GenerateTokenString(LoginUser user);
    }
}