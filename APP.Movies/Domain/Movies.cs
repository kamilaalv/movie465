using System;
using System.Collections.Generic;
using System.IO;

namespace APP.Movies.Domain
{
    public class Movie : CORE.APP.Domain.Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int DirectorId { get; set; }

        // Navigation properties
        public Director Director { get; set; }
        public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    }
}