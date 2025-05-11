using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Movies
{
    public class MovieUpdateRequest : IRequest<Response>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int DirectorId { get; set; }
        public List<int> GenreIds { get; set; } = new List<int>();
    }

    public class MovieUpdateHandler : Handler<MovieUpdateRequest, Response>
    {
        private readonly MoviesDbHandler _dbHandler;

        public MovieUpdateHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response> Handle(MovieUpdateRequest request, CancellationToken cancellationToken)
        {
            // Find the movie
            var movie = await _dbHandler.Find<Movie>(request.Id);
            if (movie == null)
                return new Response(false, $"Movie with ID {request.Id} not found");

            // Validate director exists
            var directorExists = await _dbHandler.Find<Director>(request.DirectorId) != null;
            if (!directorExists)
                return new Response(false, $"Director with ID {request.DirectorId} not found");

            // Validate genres exist
            if (request.GenreIds.Any())
            {
                var existingGenreIds = _dbHandler.Query<Genre>()
                    .Where(g => request.GenreIds.Contains(g.Id))
                    .Select(g => g.Id)
                    .ToList();

                var missingGenreIds = request.GenreIds.Except(existingGenreIds).ToList();
                if (missingGenreIds.Any())
                    return new Response(false, $"Genre IDs not found: {string.Join(", ", missingGenreIds)}");
            }

            // Update movie properties
            movie.Name = request.Name;
            movie.ReleaseDate = request.ReleaseDate;
            movie.TotalRevenue = request.TotalRevenue;
            movie.DirectorId = request.DirectorId;

            await _dbHandler.Update(movie);

            // Get current movie genres
            var currentMovieGenres = await _dbHandler.Query<MovieGenre>(mg => mg.MovieId == movie.Id).ToListAsync(cancellationToken);
            var currentGenreIds = currentMovieGenres.Select(mg => mg.GenreId).ToList();

            // Add new genres
            var genresToAdd = request.GenreIds.Except(currentGenreIds).ToList();
            foreach (var genreId in genresToAdd)
            {
                var movieGenre = new MovieGenre
                {
                    MovieId = movie.Id,
                    GenreId = genreId
                };

                await _dbHandler.Add(movieGenre);
            }

            // Remove genres that are no longer associated
            var genresToRemove = currentGenreIds.Except(request.GenreIds).ToList();
            var movieGenresToRemove = currentMovieGenres.Where(mg => genresToRemove.Contains(mg.GenreId)).ToList();

            foreach (var movieGenre in movieGenresToRemove)
            {
                await _dbHandler.Delete(movieGenre);
            }

            return new Response(true, "Movie updated successfully");
        }
    }
}