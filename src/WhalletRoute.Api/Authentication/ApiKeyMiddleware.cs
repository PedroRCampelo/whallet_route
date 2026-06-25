using WhalletRoute.Application.Tenancy;

namespace WhalletRoute.Api.Authentication;

public sealed class ApiKeyMiddleware
{
    private const string HeaderName = "X-Api-Key";
    private static readonly string[] OpenPaths = { "/health", "/swagger" };

    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ITenantResolver resolver, ITenantContext tenantContext)
    {
        if (HttpMethods.IsOptions(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path.Value ?? string.Empty;
        if (OpenPaths.Any(open => path.StartsWith(open, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var provided) || string.IsNullOrWhiteSpace(provided))
        {
            await Reject(context, "Missing API key.");
            return;
        }

        var tenant = await resolver.ResolveByApiKeyAsync(provided.ToString(), context.RequestAborted);
        if (tenant is null)
        {
            await Reject(context, "Invalid API key.");
            return;
        }

        tenantContext.Set(tenant);
        await _next(context);
    }

    private static async Task Reject(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { error = message });
    }
}