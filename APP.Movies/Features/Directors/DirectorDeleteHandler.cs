using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Directors
{
    public class DirectorDeleteRequest : Request<Response>
    {
        public int Id { get; set; }
    }

    public class DirectorDeleteHandler : Handler<DirectorDeleteRequest, Response>
    {
        private readonly MoviesDbHandler _dbHandler;

        public DirectorDeleteHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response> Handle(DirectorDeleteRequest request, CancellationToken cancellationToken)
        {
            // Find the director
            var director = await _dbHandler.Find<Director>(request.Id);
            if (director == null)
                return new Response(false, $"Director with ID {request.Id} not found");

            // Check if director has movies
            var hasMovies = await _dbHandler.Query<Movie>(m => m.DirectorId == request.Id).AnyAsync(cancellationToken);
            if (hasMovies)
                return new Response(false, "Cannot delete director with associated movies. Remove or reassign the movies first.");

            // Delete the director
            await _dbHandler.Delete(director);

            return new Response(true, "Director deleted successfully");
        }
    }
}