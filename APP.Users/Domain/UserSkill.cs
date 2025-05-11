using CORE.APP.Domain;

namespace APP.Users.Domain
{
    public class UserSkill : Entity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SkillId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Skill Skill { get; set; }
    }
}