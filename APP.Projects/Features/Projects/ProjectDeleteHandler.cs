using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APP.Projects.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Projects.Features.Projects
{
    /// <summary>
    /// Represents a request to delete a project.
    /// </summary>
    public class ProjectDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    /// <summary>
    /// Handles the deletion of a project.
    /// </summary>
    public class ProjectDeleteHandler : ProjectsDbHandler, IRequestHandler<ProjectDeleteRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDeleteHandler"/> class.
        /// </summary>
        /// <param name="projectsDb">Database context for projects.</param>
        public ProjectDeleteHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        /// <summary>
        /// Handles the request to delete a project.
        /// </summary>
        /// <param name="request">The project deletion request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A command response indicating success or failure.</returns>
        public async Task<CommandResponse> Handle(ProjectDeleteRequest request, CancellationToken cancellationToken)
        {
            // Retrieve the project entity along with its associated tags
            var entity = await _projectsDb.Projects
                .Include(p => p.ProjectTags)
                .SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            // Return an error if the project does not exist
            if (entity is null)
                return Error("Project not found!");

            // Remove all associated tags before deleting the project
            _projectsDb.ProjectTags.RemoveRange(entity.ProjectTags);

            // Remove the project from the database
            _projectsDb.Projects.Remove(entity);
            await _projectsDb.SaveChangesAsync(cancellationToken);

            return Success("Project deleted successfully.", entity.Id);
        }
    }
}