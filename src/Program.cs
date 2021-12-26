using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Logging.AddConsole();

var app = builder.Build();
var logger = app.Logger;

// Configure the HTTP request pipeline.

app.UseStaticFiles();

app.Use(
    async (context, next) =>
        {
            if (context.Request.Path.Value == "/notifications-hub")
            {
                var response = context.Response;
                response.Headers.Add("Content-Type", "text/event-stream");

                var count = 0;
                StreamWriter streamWriter = new StreamWriter(response.Body);
                while (!context.RequestAborted.IsCancellationRequested)
                {
                    var data = $"Notification {++count} at {DateTime.Now}";

                    await streamWriter.WriteLineAsync($"data:{data}");
                    await streamWriter.WriteLineAsync();
                    await streamWriter.FlushAsync();

                    logger.LogInformation($"Sent: {data}");

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            await next.Invoke();
        });

await app.RunAsync();

