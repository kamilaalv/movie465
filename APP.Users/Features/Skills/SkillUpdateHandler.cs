using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Skills
{
    public class SkillUpdateRequest : Request<CommandResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SkillUpdateHandler : Handler<SkillUpdateRequest, CommandResponse>
    {
        private readonly UsersDbHandler _dbHandler;

        public SkillUpdateHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<CommandResponse> Handle(SkillUpdateRequest request, CancellationToken cancellationToken)
        {
            // Find the skill
            var skill = await _dbHandler.Find<Skill>(request.Id);
            if (skill == null)
                return new CommandResponse(false, $"Skill with ID {request.Id} not found");

            // Check if another skill with the same name already exists
            var exists = await _dbHandler.Query<Skill>(s => s.Id != request.Id && s.Name.ToLower() == request.Name.ToLower())
                .AnyAsync(cancellationToken);

            if (exists)
                return new CommandResponse(false, $"Another skill with name '{request.Name}' already exists");

            // Update skill properties
            skill.Name = request.Name;

            await _dbHandler.Update(skill);

            return new CommandResponse(true, "Skill updated successfully");
        }
    }
}