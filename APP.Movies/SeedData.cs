using APP.Movies.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APP.Movies
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MoviesDb>();

            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed directors if none exist
            if (!await context.Directors.AnyAsync())
            {
                await context.Directors.AddRangeAsync(
                    new Director
                    {
                        Name = "Christopher",
                        Surname = "Nolan",
                        IsRetired = false
                    },
                    new Director
                    {
                        Name = "Steven",
                        Surname = "Spielberg",
                        IsRetired = false
                    },
                    new Director
                    {
                        Name = "Martin",
                        Surname = "Scorsese",
                        IsRetired = false
                    },
                    new Director
                    {
                        Name = "Quentin",
                        Surname = "Tarantino",
                        IsRetired = false
                    }
                );

                await context.SaveChangesAsync();
            }

            // Seed genres if none exist
            if (!await context.Genres.AnyAsync())
            {
                await context.Genres.AddRangeAsync(
                    new Genre { Name = "Action" },
                    new Genre { Name = "Comedy" },
                    new Genre { Name = "Drama" },
                    new Genre { Name = "Horror" },
                    new Genre { Name = "Science Fiction" },
                    new Genre { Name = "Thriller" },
                    new Genre { Name = "Adventure" },
                    new Genre { Name = "Crime" },
                    new Genre { Name = "Fantasy" },
                    new Genre { Name = "Mystery" }
                );

                await context.SaveChangesAsync();
            }

            // Seed movies if none exist
            if (!await context.Movies.AnyAsync())
            {
                var christopherNolan =
                    await context.Directors.FirstOrDefaultAsync(d => d.Name == "Christopher" && d.Surname == "Nolan");
                var stevenSpielberg =
                    await context.Directors.FirstOrDefaultAsync(d => d.Name == "Steven" && d.Surname == "Spielberg");
                var martinScorsese =
                    await context.Directors.FirstOrDefaultAsync(d => d.Name == "Martin" && d.Surname == "Scorsese");
                var quentinTarantino =
                    await context.Directors.FirstOrDefaultAsync(d => d.Name == "Quentin" && d.Surname == "Tarantino");

                var actionGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Action");
                var sciFiGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Science Fiction");
                var adventureGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Adventure");
                var dramaGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Drama");
                var crimeGenre = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Crime");

                if (christopherNolan != null && stevenSpielberg != null)
                {
                    // Nolan movies
                    var inception = new Movie
                    {
                        Name = "Inception",
                        ReleaseDate = new DateTime(2010, 7, 16),
                        TotalRevenue = 836800000,
                        DirectorId = christopherNolan.Id
                    };

                    await context.Movies.AddAsync(inception);
                    await context.SaveChangesAsync();

                    await context.MovieGenres.AddRangeAsync(
                        new MovieGenre { MovieId = inception.Id, GenreId = actionGenre.Id },
                        new MovieGenre { MovieId = inception.Id, GenreId = sciFiGenre.Id }
                    );

                    var interstellar = new Movie
                    {
                        Name = "Interstellar",
                        ReleaseDate = new DateTime(2014, 11, 7),
                        TotalRevenue = 677500000,
                        DirectorId = christopherNolan.Id
                    };

                    await context.Movies.AddAsync(interstellar);
                    await context.SaveChangesAsync();

                    await context.MovieGenres.AddRangeAsync(
                        new MovieGenre { MovieId = interstellar.Id, GenreId = sciFiGenre.Id },
                        new MovieGenre { MovieId = interstellar.Id, GenreId = adventureGenre.Id }
                    );

                    // Spielberg movies
                    var jurassicPark = new Movie
                    {
                        Name = "Jurassic Park",
                        ReleaseDate = new DateTime(1993, 6, 11),
                        TotalRevenue = 1029000000,
                        DirectorId = stevenSpielberg.Id
                    };

                    await context.Movies.AddAsync(jurassicPark);
                    await context.SaveChangesAsync();

                    await context.MovieGenres.AddRangeAsync(
                        new MovieGenre { MovieId = jurassicPark.Id, GenreId = actionGenre.Id },
                        new MovieGenre { MovieId = jurassicPark.Id, GenreId = adventureGenre.Id },
                        new MovieGenre { MovieId = jurassicPark.Id, GenreId = sciFiGenre.Id }
                    );

                    var savingPrivateRyan = new Movie
                    {
                        Name = "Saving Private Ryan",
                        ReleaseDate = new DateTime(1998, 7, 24),
                        TotalRevenue = 482300000,
                        DirectorId = stevenSpielberg.Id
                    };

                    await context.Movies.AddAsync(savingPrivateRyan);
                    await context.SaveChangesAsync();

                    await context.MovieGenres.AddRangeAsync(
                        new MovieGenre { MovieId = savingPrivateRyan.Id, GenreId = actionGenre.Id },
                        new MovieGenre { MovieId = savingPrivateRyan.Id, GenreId = dramaGenre.Id }
                    );
                }

                // Add Scorsese and Tarantino movies if they exist
                if (martinScorsese != null)
                {
                    var goodfellas = new Movie
                    {
                        Name = "Goodfellas",
                        ReleaseDate = new DateTime(1990, 9, 19),
                        TotalRevenue = 46800000,
                        DirectorId = martinScorsese.Id
                    };

                    await context.Movies.AddAsync(goodfellas);
                    await context.SaveChangesAsync();

                    if (crimeGenre != null && dramaGenre != null)
                    {
                        await context.MovieGenres.AddRangeAsync(
                            new MovieGenre { MovieId = goodfellas.Id, GenreId = crimeGenre.Id },
                            new MovieGenre { MovieId = goodfellas.Id, GenreId = dramaGenre.Id }
                        );
                    }
                }

                if (quentinTarantino != null)
                {
                    var pulpFiction = new Movie
                    {
                        Name = "Pulp Fiction",
                        ReleaseDate = new DateTime(1994, 10, 14),
                        TotalRevenue = 213900000,
                        DirectorId = quentinTarantino.Id
                    };

                    await context.Movies.AddAsync(pulpFiction);
                    await context.SaveChangesAsync();

                    if (crimeGenre != null && dramaGenre != null)
                    {
                        await context.MovieGenres.AddRangeAsync(
                            new MovieGenre { MovieId = pulpFiction.Id, GenreId = crimeGenre.Id },
                            new MovieGenre { MovieId = pulpFiction.Id, GenreId = dramaGenre.Id }
                        );
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}