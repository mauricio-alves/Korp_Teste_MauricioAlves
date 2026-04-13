namespace ApiGateway.Configuration;

public static class SecurityHeaders
{
    public static void UseSecurityHeaders(this IApplicationBuilder app)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

        app.Use(async (context, next) =>
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
            if (context.Request.IsHttps)
            {
                context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            }
            
            context.Response.Headers["Content-Security-Policy"] = BuildCsp(configuration);

            await next();
        });
    }

    private static string BuildCsp(IConfiguration configuration)
    {
        var connectSources = new List<string> { "'self'" };
        var extraOrigins = configuration["SecurityHeaders:ConnectSrcOrigins"];
        
        if (!string.IsNullOrWhiteSpace(extraOrigins))
        {
            var origins = extraOrigins.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            connectSources.AddRange(origins);
        }

        return "default-src 'self'; " +
               "script-src 'self'; " +
               "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
               "font-src 'self' https://fonts.gstatic.com; " +
               "img-src 'self' data:; " +
               $"connect-src {string.Join(" ", connectSources.Distinct())};";
    }
}
