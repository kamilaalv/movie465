using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APP.Projects.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Projects.Features.Works
{
    // Represents a request to delete a work item.
    public class WorkDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    // Handles the deletion of a work item from the database.
    public class WorkDeleteHandler : ProjectsDbHandler, IRequestHandler<WorkDeleteRequest, CommandResponse>
    {
        // Constructor to initialize the database context.
        public WorkDeleteHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        // Handles the work deletion request asynchronously.
        public async Task<CommandResponse> Handle(WorkDeleteRequest request, CancellationToken cancellationToken)
        {
            // Find the work item in the database by its ID.
            var entity = await _projectsDb.Works.SingleOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            // If the work item is not found, return an error response.
            if (entity is null)
                return Error("Work not found!");

            // Remove the work item from the database.
            _projectsDb.Works.Remove(entity);
            await _projectsDb.SaveChangesAsync(cancellationToken);

            return Success("Work deleted successfully.", entity.Id);
        }
    }
}