using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APP.Projects.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace APP.Projects.Features.Projects
{
    /// <summary>
    /// Represents a request to update an existing project.
    /// </summary>
    public class ProjectUpdateRequest : Request, IRequest<CommandResponse>
    {
        /// <summary>
        /// The name of the project (required, must be between 5 and 200 characters).
        /// </summary>
        [Required, Length(5, 200)]
        public string Name { get; set; }

        /// <summary>
        /// The description of the project (optional, maximum length 1000 characters).
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// The URL associated with the project (optional, maximum length 400 characters).
        /// </summary>
        [StringLength(400)]
        public string Url { get; set; }

        /// <summary>
        /// The version of the project (optional, must be a positive number).
        /// </summary>
        [Range(0, double.MaxValue)]
        public double? Version { get; set; }

        /// <summary>
        /// List of tag IDs associated with the project.
        /// </summary>
        public List<int> TagIds { get; set; }
    }

    /// <summary>
    /// Handles the update of an existing project.
    /// </summary>
    public class ProjectUpdateHandler : ProjectsDbHandler, IRequestHandler<ProjectUpdateRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectUpdateHandler"/> class.
        /// </summary>
        /// <param name="projectsDb">Database context for projects.</param>
        public ProjectUpdateHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        /// <summary>
        /// Handles the request to update a project.
        /// </summary>
        /// <param name="request">The project update request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A command response indicating success or failure.</returns>
        public async Task<CommandResponse> Handle(ProjectUpdateRequest request, CancellationToken cancellationToken)
        {
            // Check if a project with the same name already exists (excluding the current project)
            if (await _projectsDb.Projects.AnyAsync(p => p.Id != request.Id && p.Name.ToUpper() == request.Name.ToUpper().Trim(), cancellationToken))
                return Error("Project with the same name exists!");

            // Retrieve the project along with its associated tags
            var entity = await _projectsDb.Projects
                .Include(p => p.ProjectTags)
                .SingleOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            // Return an error if the project does not exist
            if (entity is null)
                return Error("Project not found!");

            // Remove existing project tags before updating
            _projectsDb.ProjectTags.RemoveRange(entity.ProjectTags);

            // Update project properties
            entity.Name = request.Name.Trim();
            entity.Description = request.Description?.Trim();
            entity.Url = request.Url?.Trim();
            entity.Version = request.Version;
            entity.TagIds = request.TagIds;

            // Apply the updates to the database
            _projectsDb.Projects.Update(entity);
            await _projectsDb.SaveChangesAsync(cancellationToken);

            return Success("Project updated successfully.", entity.Id);
        }
    }
}