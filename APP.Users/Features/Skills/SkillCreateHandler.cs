using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Skills
{
    public class SkillCreateRequest : Request<CommandResponse<int>>
    {
        public string Name { get; set; }
    }

    public class SkillCreateHandler : Handler<SkillCreateRequest, CommandResponse<int>>
    {
        private readonly UsersDbHandler _dbHandler;

        public SkillCreateHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<CommandResponse<int>> Handle(SkillCreateRequest request, CancellationToken cancellationToken)
        {
            // Check if skill with same name already exists
            var exists = await _dbHandler.Query<Skill>(s => s.Name.ToLower() == request.Name.ToLower())
                .AnyAsync(cancellationToken);

            if (exists)
                return new CommandResponse<int>(false, $"Skill with name '{request.Name}' already exists", 0);

            var skill = new Skill
            {
                Name = request.Name
            };

            await _dbHandler.Add(skill);

            return new CommandResponse<int>(true, "Skill created successfully", skill.Id);
        }
    }
}