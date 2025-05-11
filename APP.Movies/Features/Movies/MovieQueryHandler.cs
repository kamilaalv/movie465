using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Movies
{
    public class MovieQueryRequest : IRequest<Response<List<MovieQueryResponse>>>
    {
        public string? NameFilter { get; set; }
        public int? DirectorIdFilter { get; set; }
        public int? GenreIdFilter { get; set; }
    }

    public class MovieQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ReleaseDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public string DirectorFullName { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
    }

    public class MovieQueryHandler : Handler<MovieQueryRequest, Response<List<MovieQueryResponse>>>
    {
        private readonly MoviesDbHandler _dbHandler;

        public MovieQueryHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response<List<MovieQueryResponse>>> Handle(MovieQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _dbHandler.Query<Movie>()
                .Include(m => m.Director)
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrWhiteSpace(request.NameFilter))
                query = query.Where(m => m.Name.Contains(request.NameFilter));

            if (request.DirectorIdFilter.HasValue)
                query = query.Where(m => m.DirectorId == request.DirectorIdFilter.Value);

            if (request.GenreIdFilter.HasValue)
                query = query.Where(m => m.MovieGenres.Any(mg => mg.GenreId == request.GenreIdFilter.Value));

            var movies = await query.ToListAsync(cancellationToken);

            var response = movies.Select(m => new MovieQueryResponse
            {
                Id = m.Id,
                Name = m.Name,
                ReleaseDate = m.ReleaseDate?.ToShortDateString() ?? "Unknown",
                TotalRevenue = m.TotalRevenue,
                DirectorFullName = $"{m.Director.Name} {m.Director.Surname}",
                Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
            }).ToList();

            return new Response<List<MovieQueryResponse>>(true, "Movies retrieved successfully", response);
        }
    }
}