public class TokenExtractionMiddleware
{
    private readonly RequestDelegate _next;

    public TokenExtractionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);

            context.Items["AccessToken"] = token; //TODO: validate token and find user related to this token
        }

        await _next(context);
    }
}