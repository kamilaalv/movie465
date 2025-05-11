using System.Collections.Generic;

namespace APP.Movies.Domain
{
    public class Director : CORE.APP.Domain.Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsRetired { get; set; }

        // Navigation properties
        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}