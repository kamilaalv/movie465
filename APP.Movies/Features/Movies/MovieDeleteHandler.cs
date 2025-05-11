using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Movies
{
    public class MovieDeleteRequest : IRequest<Response>
    {
        public int Id { get; set; }
    }

    public class MovieDeleteHandler : Handler<MovieDeleteRequest, Response>
    {
        private readonly MoviesDbHandler _dbHandler;

        public MovieDeleteHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response> Handle(MovieDeleteRequest request, CancellationToken cancellationToken)
        {
            // Find the movie
            var movie = await _dbHandler.Find<Movie>(request.Id);
            if (movie == null)
                return new Response(false, $"Movie with ID {request.Id} not found");

            // Delete related movie genres first
            var movieGenres = await _dbHandler.Query<MovieGenre>(mg => mg.MovieId == request.Id).ToListAsync(cancellationToken);
            foreach (var movieGenre in movieGenres)
            {
                await _dbHandler.Delete(movieGenre);
            }

            // Delete the movie
            await _dbHandler.Delete(movie);

            return new Response(true, "Movie deleted successfully");
        }
    }
}