using System.Collections.Generic;

namespace APP.Movies.Domain
{
    public class Genre : CORE.APP.Domain.Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    }
}