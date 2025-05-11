using APP.Users.Domain;
using CORE.APP.Features;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Users.Features.Users
{
    public class UserCreateRequest : Request<CommandResponse<int>>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int RoleId { get; set; }
        public List<int> SkillIds { get; set; } = new List<int>();
    }

    public class UserCreateHandler : Handler<UserCreateRequest, CommandResponse<int>>
    {
        private readonly UsersDbHandler _dbHandler;

        public UserCreateHandler(UsersDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<CommandResponse<int>> Handle(UserCreateRequest request, CancellationToken cancellationToken)
        {
            // Check if username already exists
            var usernameExists = await _dbHandler.Query<User>(u => u.UserName == request.UserName)
                .AnyAsync(cancellationToken);

            if (usernameExists)
                return new CommandResponse<int>(false, $"Username '{request.UserName}' is already taken", 0);

            // Validate role exists
            var roleExists = await _dbHandler.Find<Role>(request.RoleId) != null;
            if (!roleExists)
                return new CommandResponse<int>(false, $"Role with ID {request.RoleId} not found", 0);

            // Validate skills exist
            if (request.SkillIds.Any())
            {
                var existingSkillIds = _dbHandler.Query<Skill>()
                    .Where(s => request.SkillIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToList();

                var missingSkillIds = request.SkillIds.Except(existingSkillIds).ToList();
                if (missingSkillIds.Any())
                    return new CommandResponse<int>(false, $"Skill IDs not found: {string.Join(", ", missingSkillIds)}", 0);
            }

            // Create new user
            var user = new User
            {
                UserName = request.UserName,
                Password = request.Password, // In a real app, hash this password
                Name = request.Name,
                Surname = request.Surname,
                RoleId = request.RoleId,
                IsActive = true,
                RegistrationDate = DateTime.UtcNow
            };

            await _dbHandler.Add(user);

            // Add user skills
            if (request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                {
                    var userSkill = new UserSkill
                    {
                        UserId = user.Id,
                        SkillId = skillId
                    };

                    await _dbHandler.Add(userSkill);
                }
            }

            return new CommandResponse<int>(true, "User created successfully", user.Id);
        }
    }
}