using Ocelot.DependencyInjection; // Provides extension methods for adding Ocelot services
using Ocelot.Middleware;         // Provides middleware to enable Ocelot request routing

var builder = WebApplication.CreateBuilder(args);

// Load the Ocelot configuration file, which defines routes and downstream services
builder.Configuration.AddJsonFile("ocelot.json");

// Register Ocelot services into the DI container
builder.Services.AddOcelot();

var app = builder.Build();

// Enables Ocelot's middleware to intercept and forward incoming HTTP requests
await app.UseOcelot();

// Optional: Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Start the application
app.Run();