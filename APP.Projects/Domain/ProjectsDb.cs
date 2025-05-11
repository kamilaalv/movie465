using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace APP.Projects.Domain
{
    /// <summary>
    /// Represents the database context for the projects, inheriting from <see cref="DbContext"/>.
    /// </summary>
    public class ProjectsDb : DbContext
    {
        /// <summary>
        /// Gets or sets the Tags DbSet, which represents the collection of all Tag entities in the context.
        /// </summary>
        public DbSet<Tag> Tags { get; set; } // Tags table in the database

        /// <summary>
        /// Gets or sets the Projects DbSet, which represents the collection of all Project entities in the context.
        /// </summary>
        public DbSet<Project> Projects { get; set; } // Projects table in the database

        /// <summary>
        /// Gets or sets the ProjectTags DbSet, which represents the collection of all ProjectTag entities in the context.
        /// </summary>
        public DbSet<ProjectTag> ProjectTags { get; set; } // ProjectTags table in the database

        public DbSet<Work> Works { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsDb"/> class with the specified options.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ProjectsDb(DbContextOptions<ProjectsDb> options) : base(options)
        {
        }

        // The connection string should be defined in appsettings.json of the API Project.
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //
        // SQL Server LocalDB:
        //   base.OnConfiguring(optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=PMSCTISProjectsDB;trusted_connection=true;"));
        //}
    }



    /// <summary>
    /// Factory class for creating instances of <see cref="ProjectsDb"/> at design time
    /// for solving problems about API scaffolding.
    /// </summary>
    public class ProjectsDbFactory : IDesignTimeDbContextFactory<ProjectsDb>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ProjectsDb"/> database context.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>A new instance of <see cref="ProjectsDb"/>.</returns>
        public ProjectsDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectsDb>();

            // Configure the database connection
            // Change the connection string based on your environment

            // SQL Server LocalDB:
            optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=PMSCTISProjectsDB;trusted_connection=true;");

            return new ProjectsDb(optionsBuilder.Options);
        }
    }
}