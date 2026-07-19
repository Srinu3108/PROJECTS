using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace EnergyPulse.UI.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    private const string TOKEN_KEY = "auth_token";
    private const string USERNAME_KEY = "auth_username";
    private const string ROLE_KEY = "auth_role";

    public CustomAuthStateProvider(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", TOKEN_KEY);

            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var username = await _js.InvokeAsync<string>("localStorage.getItem", USERNAME_KEY);
            var role = await _js.InvokeAsync<string>("localStorage.getItem", ROLE_KEY);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username ?? ""),
                new Claim(ClaimTypes.Role, role ?? "Technician")
            };

            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task Login(string token, string username, string role)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, token);
        await _js.InvokeVoidAsync("localStorage.setItem", USERNAME_KEY, username);
        await _js.InvokeVoidAsync("localStorage.setItem", ROLE_KEY, role);

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task Logout()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
        await _js.InvokeVoidAsync("localStorage.removeItem", USERNAME_KEY);
        await _js.InvokeVoidAsync("localStorage.removeItem", ROLE_KEY);

        _http.DefaultRequestHeaders.Authorization = null;

        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}