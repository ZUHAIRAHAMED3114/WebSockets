using Microsoft.AspNetCore.Authorization;

namespace SingalRAuthentication.Hub
{
    public class ProtectedHub: Microsoft.AspNetCore.SignalR.Hub
    {
        [Authorize("Cookie")]
        public object CookieProtected()
        {
            return CompileResult();
        }

        [Authorize("Token")]
        public object TokenProtected()
        {
            return CompileResult();
        }

        private object CompileResult() =>
            new
            {
                UserId = Context.UserIdentifier,
                Claims = Context.User.Claims.Select(x => new { x.Type, x.Value })
            };
    }
}
