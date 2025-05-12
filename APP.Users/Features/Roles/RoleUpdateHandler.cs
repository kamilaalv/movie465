using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Roles
{
    public class RoleUpdateRequest : Request<CommandResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RoleUpdateHandler : Handler<RoleUpdateRequest, CommandResponse>
    {
        private readonly UsersDbHandler _dbHandler;

        public RoleUpdateHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<CommandResponse> Handle(RoleUpdateRequest request, CancellationToken cancellationToken)
        {
            // Find the role
            var role = await _dbHandler.Find<Role>(request.Id);
            if (role == null)
                return new CommandResponse(false, $"Role with ID {request.Id} not found");

            // Check if another role with the same name already exists
            var exists = await _dbHandler.Query<Role>(r => r.Id != request.Id && r.Name.ToLower() == request.Name.ToLower())
                .AnyAsync(cancellationToken);

            if (exists)
                return new CommandResponse(false, $"Another role with name '{request.Name}' already exists");

            // Update role properties
            role.Name = request.Name;

            await _dbHandler.Update(role);

            return new CommandResponse(true, "Role updated successfully");
        }
    }
}