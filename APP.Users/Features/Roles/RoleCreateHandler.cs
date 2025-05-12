using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Roles
{
    public class RoleCreateRequest : Request<CommandResponse<int>>
    {
        public string Name { get; set; }
    }

    public class RoleCreateHandler : Handler<RoleCreateRequest, CommandResponse<int>>
    {
        private readonly UsersDbHandler _dbHandler;

        public RoleCreateHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<CommandResponse<int>> Handle(RoleCreateRequest request, CancellationToken cancellationToken)
        {
            // Check if role with same name already exists
            var exists = await _dbHandler.Query<Role>(r => r.Name.ToLower() == request.Name.ToLower())
                .AnyAsync(cancellationToken);

            if (exists)
                return new CommandResponse<int>(false, $"Role with name '{request.Name}' already exists", 0);

            var role = new Role
            {
                Name = request.Name
            };

            await _dbHandler.Add(role);

            return new CommandResponse<int>(true, "Role created successfully", role.Id);
        }
    }
}