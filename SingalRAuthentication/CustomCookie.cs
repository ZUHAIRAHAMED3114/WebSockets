using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SingalRAuthentication
{
    public class CustomCookie : AuthenticationHandler<AuthenticationSchemeOptions>
    {
    

        private readonly IOptionsMonitor<AuthenticationSchemeOptions> options;
        private readonly ILoggerFactory logger;
        private readonly UrlEncoder encoder;
        private readonly ISystemClock clock;

        public CustomCookie(
              IOptionsMonitor<AuthenticationSchemeOptions> options,
              ILoggerFactory logger,
              UrlEncoder encoder,
              ISystemClock clock
          ) : base(options, logger, encoder, clock)
        {
            this.options = options;
            this.logger = logger;
            this.encoder = encoder;
            this.clock = clock;
        }



        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Context.Request.Cookies.TryGetValue("signalr-auth-cookie", out var cookie))
            {
                var claims = new Claim[]
                {
                        new("user_id", cookie),
                        new("cookie", "cookie_claim"),
                };
                var identity = new ClaimsIdentity(claims, Constant.CustomCookieScheme);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, new(), Constant.CustomCookieScheme);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            return Task.FromResult(AuthenticateResult.Fail("signalr-auth-cookie not found"));
        }
    }
}
