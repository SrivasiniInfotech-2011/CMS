using System.IdentityModel.Tokens.Jwt;

namespace CMS.Api.Middlewares
{
    public class AuthMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            if (!IsValidToken(authorizationHeader))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            await next(context);
        }

        private static bool IsValidToken(string token)
        {
            var data = token.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (data.Length != 2 || !string.Equals(data[0], "Bearer", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(data[1]);
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return false;
                }

                return jwtToken.Audiences.Any(aud => string.Equals(aud, "your-audience", StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
    }
}
