using System;
using System.Threading.Tasks;
using APP.Movies;
using APP.Movies.Domain;
using APP.Movies.Features;
using APP.Movies.Features.Genres;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Movies
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // 1) Build the Host
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, cfg) =>
                {
                    // if you have appsettings.json in your console project
                    cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // Logging
                    services.AddLogging(cfg => cfg.AddConsole());

                    // EF Core DbContext
                    services.AddDbContext<MoviesDb>(opts =>
                        opts.UseSqlServer(
                            context.Configuration.GetConnectionString("MoviesConnection")
                        )
                    );

                    // MediatR
                    services.AddMediatR(cfg =>
                        cfg.RegisterServicesFromAssembly(typeof(MoviesDb).Assembly)
                    );

                    // Your handler abstraction
                    services.AddScoped<MoviesDbHandler>();

                    // A small “driver” for console work:
                    services.AddTransient<ConsoleApp>();
                })
                .Build();

            // 2) Run migrations + seeding
            using (var scope = host.Services.CreateScope())
            {
                var svc = scope.ServiceProvider;
                var db = svc.GetRequiredService<MoviesDb>();
                db.Database.Migrate();
                await SeedData.Initialize(svc);
            }

            // 3) Kick off whatever console logic you like:
            var consoleApp = host.Services.GetRequiredService<ConsoleApp>();
            await consoleApp.RunAsync();

            // (we don’t call host.Run() because we want to exit when RunAsync completes)
        }
    }

    public class ConsoleApp
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ConsoleApp> _logger;

        public ConsoleApp(IMediator mediator, ILogger<ConsoleApp> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Starting console application…");

            // example: query all genres
            var response = await _mediator.Send(new GenreQueryRequest());
            if (!response.IsSuccessful)
            {
                _logger.LogError("GenreQuery failed: {msg}", response.Message);
                return;
            }

            foreach (var g in response.Data)
                Console.WriteLine($"#{g.Id}: {g.Name}");

            // example: delete a genre
            Console.Write("Enter a genre ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out var id))
            {
                var del = await _mediator.Send(new GenreDeleteRequest { Id = id });
                var verb = del.IsSuccessful
                    ? "deleted"
                    : $"failed ({del.Message})";

                Console.WriteLine($"Genre {verb}.");
            }

            _logger.LogInformation("Console application finished.");
        }
    }
}
