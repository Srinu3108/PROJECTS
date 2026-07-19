using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace EnergyPulse.UI.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly CustomAuthStateProvider _authProvider;
    private readonly NavigationManager _navigation;

    public AuthService(HttpClient http, AuthenticationStateProvider authProvider, NavigationManager navigation)
    {
        _http = http;
        _authProvider = (CustomAuthStateProvider)authProvider;
        _navigation = navigation;
    }

    public async Task<bool> Login(string username, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new { username, password });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<AuthResponse>(content, options);

                if (result != null)
                {
                    await _authProvider.Login(result.Token, result.Username, result.Role);
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> Register(string username, string email, string password, string role = "Technician")
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", new { username, email, password, role });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<AuthResponse>(content, options);

                if (result != null)
                {
                    await _authProvider.Login(result.Token, result.Username, result.Role);
                    return true;
                }
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task Logout()
    {
        await _authProvider.Logout();
        _navigation.NavigateTo("/login");
    }

    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
    }
}