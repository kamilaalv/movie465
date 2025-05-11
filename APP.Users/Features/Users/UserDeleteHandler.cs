using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Users
{
    public class UserDeleteRequest : Request<CommandResponse>
    {
        public int Id { get; set; }
    }

    public class UserDeleteHandler : Handler<UserDeleteRequest, CommandResponse>
    {
        private readonly UsersDbHandler _dbHandler;

        public UserDeleteHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<CommandResponse> Handle(UserDeleteRequest request, CancellationToken cancellationToken)
        {
            // Find the user
            var user = await _dbHandler.Find<User>(request.Id);
            if (user == null)
                return new CommandResponse(false, $"User with ID {request.Id} not found");

            // Delete related user skills first
            var userSkills = await _dbHandler.Query<UserSkill>(us => us.UserId == request.Id).ToListAsync(cancellationToken);
            foreach (var userSkill in userSkills)
            {
                await _dbHandler.Delete(userSkill);
            }

            // Delete the user
            await _dbHandler.Delete(user);

            return new CommandResponse(true, "User deleted successfully");
        }
    }
}