var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseStaticFiles();

app.Use(
    async (context, next) =>
        {
            if (context.Request.Path.ToString().Equals("/notifications-hub"))
            {
                var response = context.Response;
                response.Headers.Add("Content-Type", "text/event-stream");

                var count = 0;
                while (true)
                {
                    await response.WriteAsync("data:");
                    await response.WriteAsync($"Notification {++count} at {DateTime.Now}");
                    await response.WriteAsync("\r\r");

                    await response.Body.FlushAsync();

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            await next.Invoke();
        });

await app.RunAsync();

