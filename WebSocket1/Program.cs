using System.Net.WebSockets;
using System.Text;

List<WebSocket> _connections = new();

var builder = WebApplication.CreateBuilder(args);
 // Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(30) });

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{

    endpoints.Map("/ws", async ctx =>
    {
        var buffer = new byte[1024 * 4];
        var webSocket = await ctx.WebSockets.AcceptWebSocketAsync();
        _connections.Add(webSocket);

        try
        {
            var result = await webSocket.ReceiveAsync(new(buffer), CancellationToken.None);
            int i = 0;
            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetBytes($"message index {i++}");
                foreach (var c in _connections.ToList()) // To avoid concurrent modifications
                {
                    if (c.State == WebSocketState.Open)
                    {
                        await c.SendAsync(new(message, 0, message.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    }
                }

                result = await webSocket.ReceiveAsync(new(buffer), CancellationToken.None);

                Console.WriteLine($"Received: {Encoding.UTF8.GetString(buffer[..result.Count])}");
            }
        }
        finally
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing the WebSocket", CancellationToken.None);
            _connections.Remove(webSocket);
        }
    });
});
app.Run();
