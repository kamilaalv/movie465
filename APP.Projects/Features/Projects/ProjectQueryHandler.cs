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
    /// Represents a request to query projects.
    /// Implements IRequest to support MediatR pattern.
    /// </summary>
    public class ProjectQueryRequest : Request, IRequest<IQueryable<ProjectQueryResponse>>
    {
    }

    /// <summary>
    /// Represents the response for a project query.
    /// </summary>
    public class ProjectQueryResponse : QueryResponse
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL of the project.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the version of the project as a nullable double.
        /// </summary>
        public double? Version { get; set; }

        /// <summary>
        /// Gets or sets the formatted version of the project as a string.
        /// </summary>
        public string VersionF { get; set; }

        public List<int> TagIds { get; set; }
    }

    /// <summary>
    /// Handles queries related to projects by fetching data from the database.
    /// Implements IRequestHandler to process ProjectQueryRequest and return a list of ProjectQueryResponse.
    /// </summary>
    public class ProjectQueryHandler : ProjectsDbHandler, IRequestHandler<ProjectQueryRequest, IQueryable<ProjectQueryResponse>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectQueryHandler"/> class.
        /// </summary>
        /// <param name="projectsDb">The projects database instance.</param>
        public ProjectQueryHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        /// <summary>
        /// Handles the request to query projects.
        /// </summary>
        /// <param name="request">The project query request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="IQueryable{T}"/> of <see cref="ProjectQueryResponse"/>.</returns>
        public Task<IQueryable<ProjectQueryResponse>> Handle(ProjectQueryRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_projectsDb.Projects
                .Include(p => p.ProjectTags).ThenInclude(pt => pt.Tag)
                .OrderBy(p => p.Name)
                .ThenByDescending(p => p.Version)
                .Select(p => new ProjectQueryResponse()
                {
                    // Map properties from the Project entity to the response DTO.
                    // Optionally AutoMapper third party library (https://automapper.org/) can be used for mapping operations.
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Url = p.Url,
                    Version = p.Version,
                    VersionF = p.Version.HasValue ? p.Version.Value.ToString("N1") : string.Empty,
                    TagIds = p.TagIds
                }));
        }
    }
}