using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Genres
{
    public class GenreCreateHandler : IRequest<Response<int>>
    {
        public string Name { get; set; }
    }

    public class GenreCreateHandler : Handler<GenreCreateHandler, Response<int>>
    {
        private readonly MoviesDbHandler _dbHandler;

        public GenreCreateHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response<int>> Handle(GenreCreateHandler request, CancellationToken cancellationToken)
        {
            // Check if genre with same name already exists
            var exists = await _dbHandler.Query<Genre>(g => g.Name.ToLower() == request.Name.ToLower())
                .AnyAsync(cancellationToken);

            if (exists)
                return new Response<int>(false, $"Genre with name '{request.Name}' already exists", 0);

            var genre = new Genre
            {
                Name = request.Name
            };

            await _dbHandler.Add(genre);

            return new Response<int>(true, "Genre created successfully", genre.Id);
        }
    }
}