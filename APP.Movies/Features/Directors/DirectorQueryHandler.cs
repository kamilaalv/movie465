using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Directors
{
    public class DirectorQueryRequest : IRequest<Response<List<DirectorQueryResponse>>>
    {
        public string? NameFilter { get; set; }
        public bool? IsRetiredFilter { get; set; }
    }

    public class DirectorQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName { get; set; }
        public bool IsRetired { get; set; }
        public int MovieCount { get; set; }
    }

    public class DirectorQueryHandler : Handler<DirectorQueryRequest, Response<List<DirectorQueryResponse>>>
    {
        private readonly MoviesDbHandler _dbHandler;

        public DirectorQueryHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response<List<DirectorQueryResponse>>> Handle(DirectorQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _dbHandler.Query<Director>()
                .Include(d => d.Movies)
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrWhiteSpace(request.NameFilter))
                query = query.Where(d => d.Name.Contains(request.NameFilter) || d.Surname.Contains(request.NameFilter));

            if (request.IsRetiredFilter.HasValue)
                query = query.Where(d => d.IsRetired == request.IsRetiredFilter.Value);

            var directors = await query.ToListAsync(cancellationToken);

            var response = directors.Select(d => new DirectorQueryResponse
            {
                Id = d.Id,
                Name = d.Name,
                Surname = d.Surname,
                FullName = $"{d.Name} {d.Surname}",
                IsRetired = d.IsRetired,
                MovieCount = d.Movies.Count
            }).ToList();

            return new Response<List<DirectorQueryResponse>>(true, "Directors retrieved successfully", response);
        }
    }
}