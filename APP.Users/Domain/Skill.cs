using System.Collections.Generic;
using CORE.APP.Domain;

namespace APP.Users.Domain
{
    public class Skill : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public List<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }
}