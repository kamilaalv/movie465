using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Skills
{
    public class SkillQueryRequest : Request<QueryResponse<List<SkillQueryResponse>>>
    {
        public string NameFilter { get; set; }
    }

    public class SkillQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserCount { get; set; }
    }

    public class SkillQueryHandler : Handler<SkillQueryRequest, QueryResponse<List<SkillQueryResponse>>>
    {
        private readonly UsersDbHandler _dbHandler;

        public SkillQueryHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<QueryResponse<List<SkillQueryResponse>>> Handle(SkillQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _dbHandler.Query<Skill>()
                .Include(s => s.UserSkills)
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrWhiteSpace(request.NameFilter))
                query = query.Where(s => s.Name.Contains(request.NameFilter));

            var skills = await query.ToListAsync(cancellationToken);

            var response = skills.Select(s => new SkillQueryResponse
            {
                Id = s.Id,
                Name = s.Name,
                UserCount = s.UserSkills.Count
            }).ToList();

            return new QueryResponse<List<SkillQueryResponse>>(true, "Skills retrieved successfully", response);
        }
    }
}