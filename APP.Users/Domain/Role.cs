using System.Collections.Generic;
using CORE.APP.Domain;

namespace APP.Users.Domain
{
    public class Role : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public List<User> Users { get; set; } = new List<User>();
    }
}