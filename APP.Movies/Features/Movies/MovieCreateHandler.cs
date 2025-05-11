using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Movies
{
    public class MovieCreateRequest : IRequest<Response<int>>
    {
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int DirectorId { get; set; }
        public List<int> GenreIds { get; set; } = new List<int>();
    }

    public class MovieCreateHandler : Handler<MovieCreateRequest, Response<int>>
    {
        private readonly MoviesDbHandler _dbHandler;

        public MovieCreateHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response<int>> Handle(MovieCreateRequest request, CancellationToken cancellationToken)
        {
            // Validate director exists
            var directorExists = await _dbHandler.Find<Director>(request.DirectorId) != null;
            if (!directorExists)
                return new Response<int>(false, $"Director with ID {request.DirectorId} not found", 0);

            // Validate genres exist
            if (request.GenreIds.Any())
            {
                var existingGenreIds = _dbHandler.Query<Genre>()
                    .Where(g => request.GenreIds.Contains(g.Id))
                    .Select(g => g.Id)
                    .ToList();

                var missingGenreIds = request.GenreIds.Except(existingGenreIds).ToList();
                if (missingGenreIds.Any())
                    return new Response<int>(false, $"Genre IDs not found: {string.Join(", ", missingGenreIds)}", 0);
            }

            // Create new movie
            var movie = new Movie
            {
                Name = request.Name,
                ReleaseDate = request.ReleaseDate,
                TotalRevenue = request.TotalRevenue,
                DirectorId = request.DirectorId
            };

            await _dbHandler.Add(movie);

            // Add movie genres
            if (request.GenreIds.Any())
            {
                foreach (var genreId in request.GenreIds)
                {
                    var movieGenre = new MovieGenre
                    {
                        MovieId = movie.Id,
                        GenreId = genreId
                    };

                    await _dbHandler.Add(movieGenre);
                }
            }

            return new Response<int>(true, "Movie created successfully", movie.Id);
        }
    }
}