var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


// Simple health check for the server
app.MapGet("/healthz", () => Results.Ok(new { ok = true, ts = DateTime.UtcNow }));

// Simple get method for testing
app.MapGet("/ping", () => "pong");

app.Run();
