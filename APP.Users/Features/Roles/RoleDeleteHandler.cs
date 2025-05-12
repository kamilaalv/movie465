using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Roles
{
    public class RoleDeleteRequest : Request<CommandResponse>
    {
        public int Id { get; set; }
    }

    public class RoleDeleteHandler : Handler<RoleDeleteRequest, CommandResponse>
    {
        private readonly UsersDbHandler _dbHandler;

        public RoleDeleteHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<CommandResponse> Handle(RoleDeleteRequest request, CancellationToken cancellationToken)
        {
            // Find the role
            var role = await _dbHandler.Find<Role>(request.Id);
            if (role == null)
                return new CommandResponse(false, $"Role with ID {request.Id} not found");

            // Check if role has associated users
            var hasUsers = await _dbHandler.Query<User>(u => u.RoleId == request.Id).AnyAsync(cancellationToken);
            if (hasUsers)
                return new CommandResponse(false, "Cannot delete role with associated users. Reassign the users to a different role first.");

            // Delete the role
            await _dbHandler.Delete(role);

            return new CommandResponse(true, "Role deleted successfully");
        }
    }
}