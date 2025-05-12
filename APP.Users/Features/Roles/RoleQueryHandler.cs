using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Roles
{
    public class RoleQueryRequest : Request<QueryResponse<List<RoleQueryResponse>>>
    {
        public string NameFilter { get; set; }
    }

    public class RoleQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserCount { get; set; }
    }

    public class RoleQueryHandler : Handler<RoleQueryRequest, QueryResponse<List<RoleQueryResponse>>>
    {
        private readonly UsersDbHandler _dbHandler;

        public RoleQueryHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<QueryResponse<List<RoleQueryResponse>>> Handle(RoleQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _dbHandler.Query<Role>()
                .Include(r => r.Users)
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrWhiteSpace(request.NameFilter))
                query = query.Where(r => r.Name.Contains(request.NameFilter));

            var roles = await query.ToListAsync(cancellationToken);

            var response = roles.Select(r => new RoleQueryResponse
            {
                Id = r.Id,
                Name = r.Name,
                UserCount = r.Users.Count
            }).ToList();

            return new QueryResponse<List<RoleQueryResponse>>(true, "Roles retrieved successfully", response);
        }
    }
}