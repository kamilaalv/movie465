using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Users
{
    public class UserQueryRequest : Request<QueryResponse<List<UserQueryResponse>>>
    {
        public string NameFilter { get; set; }
        public int? RoleIdFilter { get; set; }
        public bool? IsActiveFilter { get; set; }
    }

    public class UserQueryResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName { get; set; }
        public string RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
    }

    public class UserQueryHandler : Handler<UserQueryRequest, QueryResponse<List<UserQueryResponse>>>
    {
        private readonly UsersDbHandler _dbHandler;

        public UserQueryHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<QueryResponse<List<UserQueryResponse>>> Handle(UserQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _dbHandler.Query<User>()
                .Include(u => u.Role)
                .Include(u => u.UserSkills)
                .ThenInclude(us => us.Skill)
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrWhiteSpace(request.NameFilter))
            {
                query = query.Where(u =>
                    u.Name.Contains(request.NameFilter) ||
                    u.Surname.Contains(request.NameFilter) ||
                    u.UserName.Contains(request.NameFilter));
            }

            if (request.RoleIdFilter.HasValue)
                query = query.Where(u => u.RoleId == request.RoleIdFilter.Value);

            if (request.IsActiveFilter.HasValue)
                query = query.Where(u => u.IsActive == request.IsActiveFilter.Value);

            var users = await query.ToListAsync(cancellationToken);

            var response = users.Select(u => new UserQueryResponse
            {
                Id = u.Id,
                UserName = u.UserName,
                Name = u.Name,
                Surname = u.Surname,
                FullName = $"{u.Name} {u.Surname}",
                RegistrationDate = u.RegistrationDate.ToShortDateString(),
                IsActive = u.IsActive,
                RoleName = u.Role.Name,
                Skills = u.UserSkills.Select(us => us.Skill.Name).ToList()
            }).ToList();

            return new QueryResponse<List<UserQueryResponse>>(true, "Users retrieved successfully", response);
        }
    }
}