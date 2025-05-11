using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Genres
{
    public class GenreUpdateRequest : IRequest<Response>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GenreUpdateHandler : Handler<GenreUpdateRequest, Response>
    {
        private readonly MoviesDbHandler _dbHandler;

        public GenreUpdateHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response> Handle(GenreUpdateRequest request, CancellationToken cancellationToken)
        {
            // Find the genre
            var genre = await _dbHandler.Find<Genre>(request.Id);
            if (genre == null)
                return new Response(false, $"Genre with ID {request.Id} not found");

            // Check if another genre with same name already exists
            var exists = await _dbHandler.Query<Genre>(g => g.Id != request.Id && g.Name.ToLower() == request.Name.ToLower())
                .AnyAsync(cancellationToken);

            if (exists)
                return new Response(false, $"Another genre with name '{request.Name}' already exists");

            // Update genre properties
            genre.Name = request.Name;

            await _dbHandler.Update(genre);

            return new Response(true, "Genre updated successfully");
        }
    }
}