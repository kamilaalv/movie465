using System;
using System.Collections.Generic;
using System.Data;
using CORE.APP.Domain;

namespace APP.Users.Domain
{
    public class User : Entity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int RoleId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }

        // Navigation properties
        public Role Role { get; set; }
        public List<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }
}