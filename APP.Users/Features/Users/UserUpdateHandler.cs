using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Users
{
    public class UserUpdateRequest : Request<CommandResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public List<int> SkillIds { get; set; } = new List<int>();
    }

    public class UserUpdateHandler : Handler<UserUpdateRequest, CommandResponse>
    {
        private readonly UsersDbHandler _dbHandler;

        public UserUpdateHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<CommandResponse> Handle(UserUpdateRequest request, CancellationToken cancellationToken)
        {
            // Find the user
            var user = await _dbHandler.Find<User>(request.Id);
            if (user == null)
                return new CommandResponse(false, $"User with ID {request.Id} not found");

            // Validate role exists
            var roleExists = await _dbHandler.Find<Role>(request.RoleId) != null;
            if (!roleExists)
                return new CommandResponse(false, $"Role with ID {request.RoleId} not found");

            // Validate skills exist
            if (request.SkillIds.Any())
            {
                var existingSkillIds = _dbHandler.Query<Skill>()
                    .Where(s => request.SkillIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToList();

                var missingSkillIds = request.SkillIds.Except(existingSkillIds).ToList();
                if (missingSkillIds.Any())
                    return new CommandResponse(false, $"Skill IDs not found: {string.Join(", ", missingSkillIds)}");
            }

            // Update user properties
            user.Name = request.Name;
            user.Surname = request.Surname;
            user.IsActive = request.IsActive;
            user.RoleId = request.RoleId;

            await _dbHandler.Update(user);

            // Get current user skills
            var currentUserSkills = await _dbHandler.Query<UserSkill>(us => us.UserId == user.Id).ToListAsync(cancellationToken);
            var currentSkillIds = currentUserSkills.Select(us => us.SkillId).ToList();

            // Add new skills
            var skillsToAdd = request.SkillIds.Except(currentSkillIds).ToList();
            foreach (var skillId in skillsToAdd)
            {
                var userSkill = new UserSkill
                {
                    UserId = user.Id,
                    SkillId = skillId
                };

                await _dbHandler.Add(userSkill);
            }

            // Remove skills that are no longer associated
            var skillsToRemove = currentSkillIds.Except(request.SkillIds).ToList();
            var userSkillsToRemove = currentUserSkills.Where(us => skillsToRemove.Contains(us.SkillId)).ToList();

            foreach (var userSkill in userSkillsToRemove)
            {
                await _dbHandler.Delete(userSkill);
            }

            return new CommandResponse(true, "User updated successfully");
        }
    }
}