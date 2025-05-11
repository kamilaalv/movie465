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
using System.Text.Json.Serialization;

namespace APP.Projects.Features.Projects
{
    /// <summary>
    /// Represents a request to create a new project.
    /// Implements validation using data annotations.
    /// </summary>
    public class ProjectCreateRequest : Request, IRequest<CommandResponse>
    {
        [JsonIgnore]
        public override int Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Name of the project (required, must be between 5 and 200 characters).
        /// </summary>
        [Required, Length(5, 200)]
        public string Name { get; set; }

        /// <summary>
        /// Description of the project (optional, maximum length 1000 characters).
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// URL associated with the project (optional, maximum length 400 characters).
        /// </summary>
        [StringLength(400)]
        public string Url { get; set; }

        /// <summary>
        /// Version of the project (optional, must be a positive number).
        /// </summary>
        [Range(0, double.MaxValue)]
        public double? Version { get; set; }

        /// <summary>
        /// List of tag IDs associated with the project.
        /// </summary>
        //[Required] // Uncomment to enforce that at least one tag ID must be present.
        public List<int> TagIds { get; set; }
    }

    /// <summary>
    /// Handles the creation of a new project.
    /// </summary>
    public class ProjectCreateHandler : ProjectsDbHandler, IRequestHandler<ProjectCreateRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCreateHandler"/> class.
        /// </summary>
        /// <param name="projectsDb">Database context for projects.</param>
        public ProjectCreateHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        /// <summary>
        /// Handles the request to create a new project.
        /// </summary>
        /// <param name="request">The project creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A command response indicating success or failure.</returns>
        public async Task<CommandResponse> Handle(ProjectCreateRequest request, CancellationToken cancellationToken)
        {
            // Check if a project with the same name already exists
            if (await _projectsDb.Projects.AnyAsync(p => p.Name.ToUpper() == request.Name.ToUpper().Trim(), cancellationToken))
                return Error("Project with the same name exists!");

            // Construct the new project entity
            var entity = new Project()
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                Url = request.Url?.Trim(),
                Version = request.Version,
                TagIds = request.TagIds
            };

            // Add the new project to the database and save changes
            _projectsDb.Projects.Add(entity);
            await _projectsDb.SaveChangesAsync(cancellationToken);

            return Success("Project created successfully.", entity.Id);
        }
    }
}