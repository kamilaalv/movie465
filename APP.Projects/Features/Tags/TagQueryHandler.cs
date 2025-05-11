using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APP.Projects.Domain;
using CORE.APP.Features;
using MediatR;

namespace APP.Projects.Features.Tags
{
    /// <summary>
    /// Represents a request to query tags, inheriting from <see cref="Request"/>.
    /// </summary>
    public class TagQueryRequest : Request, IRequest<IQueryable<TagQueryResponse>>
    {
    }

    /// <summary>
    /// Represents a response to a tag query, inheriting from <see cref="QueryResponse"/>.
    /// </summary>
    public class TagQueryResponse : QueryResponse
    {
        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Handles the tag query request, inheriting from <see cref="ProjectsDbHandler"/>.
    /// </summary>
    public class TagQueryHandler : ProjectsDbHandler, IRequestHandler<TagQueryRequest, IQueryable<TagQueryResponse>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagQueryHandler"/> class with the specified database context.
        /// </summary>
        /// <param name="projectsDb">The database context to use.</param>
        public TagQueryHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        /// <summary>
        /// Handles the tag query request and returns the queryable result.
        /// </summary>
        /// <param name="request">The tag query request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the queryable result of tag query responses.</returns>
        public Task<IQueryable<TagQueryResponse>> Handle(TagQueryRequest request, CancellationToken cancellationToken)
        {
            // SQL: select Id, Name from Tags order by Name
            return Task.FromResult(_projectsDb.Tags.OrderBy(tag => tag.Name).Select(tag => new TagQueryResponse()
            {
                Id = tag.Id,
                Name = tag.Name
            }));
        }
    }
}