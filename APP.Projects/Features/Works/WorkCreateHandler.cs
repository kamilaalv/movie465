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

namespace APP.Projects.Features.Works
{
    // Represents a request to create a new work item.
    public class WorkCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(300)]
        public string Name { get; set; } // The name of the work item (required, max length 300).

        public string Description { get; set; } // Optional description of the work item.

        public DateTime StartDate { get; set; } // The start date of the work item.

        public DateTime DueDate { get; set; } // The due date of the work item.

        public int? ProjectId { get; set; } // Optional project ID associated with the work item.

        [JsonIgnore]
        public override int Id { get => base.Id; set => base.Id = value; } // Inherited ID property, ignored in JSON serialization.
    }

    // Handles the creation of a new work item in the database.
    public class WorkCreateHandler : ProjectsDbHandler, IRequestHandler<WorkCreateRequest, CommandResponse>
    {
        // Constructor to initialize the database context.
        public WorkCreateHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        // Handles the work creation request asynchronously.
        public async Task<CommandResponse> Handle(WorkCreateRequest request, CancellationToken cancellationToken)
        {
            // Validate that the due date is not earlier than the start date.
            if (request.DueDate < request.StartDate)
                return Error("Due date must be later or equal to start date!");

            // Check if a work item with the same name already exists in the database.
            if (await _projectsDb.Works.AnyAsync(w => w.Name.ToUpper() == request.Name.ToUpper().Trim()))
                return Error("Work with the same name exists!");

            // Create a new work entity.
            var entity = new Work()
            {
                Description = request.Description?.Trim(),
                DueDate = request.DueDate,
                Name = request.Name?.Trim(),
                ProjectId = request.ProjectId,
                StartDate = request.StartDate
            };

            // Add the new work item to the database and save changes.
            _projectsDb.Works.Add(entity);
            await _projectsDb.SaveChangesAsync(cancellationToken);

            return Success("Work created successfully.", entity.Id);
        }
    }
}