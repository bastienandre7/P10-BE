using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

public class AuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
  

    public AuthService(ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider, IHttpContextAccessor httpContextAccessor)
    {
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
        _httpContextAccessor = httpContextAccessor;

    }

    public async Task Login(string token)
    {
        await _localStorage.SetItemAsync("authToken", token);
        
        await _authStateProvider.GetAuthenticationStateAsync();
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _authStateProvider.GetAuthenticationStateAsync();
    }

    public async Task<bool> IsUserInRole(string roleName)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity.IsAuthenticated && authState.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == roleName);
    }

    public async Task<string> GetTokenAsync()
    {
        string token = await _localStorage.GetItemAsStringAsync("authToken");
        return token?.Trim('"');
    }

}
