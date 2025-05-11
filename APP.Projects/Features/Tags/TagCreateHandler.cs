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

namespace APP.Projects.Features.Tags
{
    // Represents the request to create a new tag
    public class TagCreateRequest : Request, IRequest<CommandResponse>
    {
        [JsonIgnore]
        public override int Id { get => base.Id; set => base.Id = value; }

        // Tag name property - required and with a maximum length of 150 characters,
        // minimum length of 3 characters
        [Required, MaxLength(150), MinLength(3)]
        public string Name { get; set; }
    }

    // Handler for processing the TagCreateRequest and performing the creation of a tag
    public class TagCreateHandler : ProjectsDbHandler, IRequestHandler<TagCreateRequest, CommandResponse>
    {
        // Constructor that initializes the base class with the database context (ProjectsDb)
        public TagCreateHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        // Handles the request to create a new tag
        public async Task<CommandResponse> Handle(TagCreateRequest request, CancellationToken cancellationToken)
        {
            // Check to ensure the tag name doesn't already exist in the database:
            // Way 1:
            //var existingTag = await _projectsDb.Tags.SingleOrDefaultAsync(t => t.Name.ToUpper() == request.Name.ToUpper().Trim(), cancellationToken);
            //if (existingTag is not null)
            //    return Error("Tag with the same name exists!");
            // Way 2:
            if (await _projectsDb.Tags.AnyAsync(t => t.Name.ToUpper() == request.Name.ToUpper().Trim(), cancellationToken))
                return Error("Tag with the same name exists!");

            // Create a new Tag entity and set its name from the request (trimmed of leading/trailing whitespace)
            Tag tag = new Tag()
            {
                Name = request.Name.Trim()
            };

            // Add the tag to the database context
            // Way 1: does not insert relational data
            //_projectsDb.Entry(tag).State = EntityState.Added;
            // Way 2: inserts relational data
            //_projectsDb.Add(tag);
            // Way 3: inserts relational data
            _projectsDb.Tags.Add(tag);

            // Save changes asynchronously (unit of work pattern)
            await _projectsDb.SaveChangesAsync(cancellationToken);

            // Return a successful command response with the ID of the newly created tag
            // Note: This should ideally use the Success method from the base class instead of manually constructing the response
            return Success("Tag created successfully.", tag.Id);
        }
    }
}