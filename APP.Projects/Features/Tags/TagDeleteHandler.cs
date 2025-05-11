using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APP.Projects.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Projects.Features.Tags
{
    /// <summary>
    /// Represents a request to delete a tag containing Id property from the base Request class.
    /// </summary>
    public class TagDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    /// <summary>
    /// Handles the deletion of tags.
    /// </summary>
    public class TagDeleteHandler : ProjectsDbHandler, IRequestHandler<TagDeleteRequest, CommandResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagDeleteHandler"/> class.
        /// </summary>
        /// <param name="projectsDb">The projects database context.</param>
        public TagDeleteHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        /// <summary>
        /// Handles the tag deletion request.
        /// </summary>
        /// <param name="request">The tag deletion request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the command response.</returns>
        public async Task<CommandResponse> Handle(TagDeleteRequest request, CancellationToken cancellationToken)
        {
            // Find the tag to delete.
            Tag tag = await _projectsDb.Tags.SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (tag is null)
                return Error("Tag not found!");

            // Delete the tag using one of the following ways:
            // Way 1
            //_projectsDb.Entry(tag).State = EntityState.Deleted; 
            // Way 2
            //_projectsDb.Remove(tag); 
            // Way 3
            _projectsDb.Tags.Remove(tag);

            // Save the changes to the database by unit of work.
            await _projectsDb.SaveChangesAsync(cancellationToken);

            return Success("Tag deleted successfully.");
        }
    }
}