public interface IAuthService
{
    Task<string> GetTokenAsync();
    Task<bool> IsUserInRole(string roleName);
    Task Login(string token);
    Task Logout();
}