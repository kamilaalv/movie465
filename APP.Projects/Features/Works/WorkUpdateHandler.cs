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

namespace APP.Projects.Features.Works
{
    // Represents a request to update an existing work item.
    public class WorkUpdateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(300)]
        public string Name { get; set; } // The updated name of the work item (required, max length 300).

        public string Description { get; set; } // Updated description of the work item.

        public DateTime StartDate { get; set; } // Updated start date of the work item.

        public DateTime DueDate { get; set; } // Updated due date of the work item.

        public int? ProjectId { get; set; } // Updated project ID associated with the work item.
    }

    // Handles updating an existing work item in the database.
    public class WorkUpdateHandler : ProjectsDbHandler, IRequestHandler<WorkUpdateRequest, CommandResponse>
    {
        // Constructor to initialize the database context.
        public WorkUpdateHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        // Handles the work update request asynchronously.
        public async Task<CommandResponse> Handle(WorkUpdateRequest request, CancellationToken cancellationToken)
        {
            // Validate that the due date is not earlier than the start date.
            if (request.DueDate < request.StartDate)
                return Error("Due date must be later or equal to start date!");

            // Check if a work item with the same name already exists (excluding the current one).
            if (await _projectsDb.Works.AnyAsync(w => w.Id != request.Id && w.Name.ToUpper() == request.Name.ToUpper().Trim()))
                return Error("Work with the same name exists!");

            // Find the work item by ID.
            var entity = await _projectsDb.Works.SingleOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            // If the work item is not found, return an error response.
            if (entity is null)
                return Error("Work not found!");

            // Update work item properties with new values.
            entity.Description = request.Description?.Trim();
            entity.DueDate = request.DueDate;
            entity.Name = request.Name?.Trim();
            entity.ProjectId = request.ProjectId;
            entity.StartDate = request.StartDate;

            // Save the updated entity to the database.
            _projectsDb.Works.Update(entity);
            await _projectsDb.SaveChangesAsync(cancellationToken);

            return Success("Work updated successfully.", entity.Id);
        }
    }
}