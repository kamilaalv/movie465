using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APP.Projects.Domain;
using CORE.APP.Features;
using System.Globalization;

namespace APP.Projects.Features
{
    /// <summary>
    /// Abstract base class for handling database operations in the ProjectsDb context, inheriting from <see cref="Handler"/>.
    /// </summary>
    public abstract class ProjectsDbHandler : Handler
    {
        /// <summary>
        /// The ProjectsDb context used for database operations.
        /// </summary>
        protected readonly ProjectsDb _projectsDb;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsDbHandler"/> class with the specified database context.
        /// Sets the current thread's culture to en-US. tr-TR can be used for Turkish culture.
        /// </summary>
        /// <param name="projectsDb">The ProjectsDb context to use for database operations.</param>
        public ProjectsDbHandler(ProjectsDb projectsDb) : base(new CultureInfo("en-US"))
        {
            _projectsDb = projectsDb;
        }
    }
}