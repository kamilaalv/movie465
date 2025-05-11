using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace APP.Movies.Domain
{
    public class MoviesDb : DbContext
    {
        // DbContext constructor that accepts options for configuration
        public MoviesDb(DbContextOptions<MoviesDb> options) : base(options)
        {
        }

        // DbSets for our entities
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }

        // Model configuration through Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Movie entity
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Director)
                .WithMany(d => d.Movies)
                .HasForeignKey(m => m.DirectorId);

            // Configure MovieGenre entity
            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId);

            base.OnModelCreating(modelBuilder);
        }
    }
}