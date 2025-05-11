using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Genres
{
    public class GenreDeleteHandler : IRequest<CORE.APP.Features.Response>
    {
        public int Id { get; set; }
    }

    public class GenreDeleteHandler : Handler<GenreDeleteHandler, CORE.APP.Features.Response>
    {
        private readonly MoviesDbHandler _dbHandler;

        public GenreDeleteHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<CORE.APP.Features.Response> Handle(GenreDeleteHandler request, CancellationToken cancellationToken)
        {
            // Find the genre
            var genre = await _dbHandler.Find<Genre>(request.Id);
            if (genre == null)
                return new CORE.APP.Features.Response(false, $"Genre with ID {request.Id} not found");

            // Check if genre has associated movies
            var hasMovies = await _dbHandler.Query<MovieGenre>(mg => mg.GenreId == request.Id).AnyAsync(cancellationToken);
            if (hasMovies)
                return new CORE.APP.Features.Response(false, "Cannot delete genre with associated movies. Remove the genre from those movies first.");

            // Delete the genre
            await _dbHandler.Delete(genre);

            return new CORE.APP.Features.Response(true, "Genre deleted successfully");
        }
    }
}