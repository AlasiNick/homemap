using Homemap.ApplicationCore.Interfaces.Services;

public class TokenExtractionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAuthService _authService;

    public TokenExtractionMiddleware(RequestDelegate next, IAuthService authService)
    {
        _next = next;
        _authService = authService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

            var user = await _authService.ValidateAccessTokenAsync(token);
            if (user != null)
            {
                context.Items["User"] = user;
            }
        }

        await _next(context);
    }
}
