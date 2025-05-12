using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Skills
{
    public class SkillDeleteRequest : Request<CommandResponse>
    {
        public int Id { get; set; }
    }

    public class SkillDeleteHandler : Handler<SkillDeleteRequest, CommandResponse>
    {
        private readonly UsersDbHandler _dbHandler;

        public SkillDeleteHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<CommandResponse> Handle(SkillDeleteRequest request, CancellationToken cancellationToken)
        {
            // Find the skill
            var skill = await _dbHandler.Find<Skill>(request.Id);
            if (skill == null)
                return new CommandResponse(false, $"Skill with ID {request.Id} not found");

            // Check if skill has associated users
            var hasUsers = await _dbHandler.Query<UserSkill>(us => us.SkillId == request.Id).AnyAsync(cancellationToken);
            if (hasUsers)
                return new CommandResponse(false, "Cannot delete skill with associated users. Remove the skill from those users first.");

            // Delete the skill
            await _dbHandler.Delete(skill);

            return new CommandResponse(true, "Skill deleted successfully");
        }
    }
}