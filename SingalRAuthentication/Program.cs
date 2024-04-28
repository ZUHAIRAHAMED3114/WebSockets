using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using SingalRAuthentication;
using System.Security.Claims;
using SingalRAuthentication.Hub;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(Constant.CustomTokenScheme)
                .AddScheme<AuthenticationSchemeOptions, CustomCookie>(Constant.CustomCookieScheme, options =>
                {

                })
                .AddJwtBearer(Constant.CustomTokenScheme, (o) =>
                {
                    o.Events=new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents() {
                        OnMessageReceived = (context) => {

                            var path = context.HttpContext.Request.Path;
                            if (path.StartsWithSegments("/protected")
                                || path.StartsWithSegments("/token"))
                            {
                                var accessToken = context.Request.Query["access_token"];

                                if (!string.IsNullOrWhiteSpace(accessToken))
                                {
                                    // context.Token = accessToken;

                                    var claims = new Claim[]
                                    {
                                        new("user_id", accessToken),
                                        new("token", "token_claim"),
                                    };
                                    var identity = new ClaimsIdentity(claims, Constant.CustomTokenScheme);
                                    context.Principal = new(identity);
                                    context.Success();
                                }
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

builder.Services.AddAuthorization(AuthOption =>
{
    AuthOption.AddPolicy("Cookie", (pb) => {
        pb.AddAuthenticationSchemes(Constant.CustomCookieScheme)
                                            .RequireAuthenticatedUser();    
      });

    AuthOption.AddPolicy("Token", Pb =>
    {
        Pb.AddAuthenticationSchemes(Constant.CustomTokenScheme)
          .RequireAuthenticatedUser()
          .RequireClaim("token");
    });
});

builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints?.MapHub<ProtectedHub>("/protected", o =>
    {
        // o.transports = httptransporttype.longpolling;
    });

    endpoints?.Map("/get-cookie", ctx =>
    {
        ctx.Response.StatusCode = 200;
        ctx.Response.Cookies.Append("signalr-auth-cookie", Guid.NewGuid().ToString(), new()
        {
            Expires = DateTimeOffset.UtcNow.AddSeconds(3000)
        });
        return ctx.Response.WriteAsync("");
    });

    endpoints?.Map("/token", ctx =>
    {
        ctx.Response.StatusCode = 200;
        return ctx.Response.WriteAsync(ctx?.User?.Claims?.FirstOrDefault(x => x.Type == "user_id")?.Value);
    }).RequireAuthorization("Token");

    endpoints?.Map("/cookie", ctx =>
    {
        ctx.Response.StatusCode = 200;
        return ctx.Response.WriteAsync(ctx.User?.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value);
    }).RequireAuthorization("Cookie");
});


app.Run();
