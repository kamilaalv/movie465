using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Genres
{
    public class GenreQueryRequest : IRequest<Response<List<GenreQueryResponse>>>
    {
        public string? NameFilter { get; set; }
    }

    public class GenreQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MovieCount { get; set; }
    }

    public class GenreQueryHandler : Handler<GenreQueryRequest, Response<List<GenreQueryResponse>>>
    {
        private readonly MoviesDbHandler _dbHandler;

        public GenreQueryHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response<List<GenreQueryResponse>>> Handle(GenreQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _dbHandler.Query<Genre>()
                .Include(g => g.MovieGenres)
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrWhiteSpace(request.NameFilter))
                query = query.Where(g => g.Name.Contains(request.NameFilter));

            var genres = await query.ToListAsync(cancellationToken);

            var response = genres.Select(g => new GenreQueryResponse
            {
                Id = g.Id,
                Name = g.Name,
                MovieCount = g.MovieGenres.Count
            }).ToList();

            return new Response<List<GenreQueryResponse>>(true, "Genres retrieved successfully", response);
        }
    }
}