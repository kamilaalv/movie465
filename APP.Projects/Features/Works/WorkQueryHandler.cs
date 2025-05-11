using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APP.Projects.Domain;
using APP.Projects.Features.Projects;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Projects.Features.Works
{
    /// <summary>
    /// Represents a request for querying work items.
    /// Inherits from Request and implements IRequest to return a queryable list of WorkQueryResponse.
    /// </summary>
    public class WorkQueryRequest : Request, IRequest<IQueryable<WorkQueryResponse>>
    {
        /// <summary>
        /// The name of the work item to filter by.
        /// Optional.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the project associated with the work item to filter by.
        /// Optional.
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// The beginning value of the start date of the work to filter the start date by using range.
        /// Optional.
        /// </summary>
        public DateTime? StartDateBegin { get; set; }

        /// <summary>
        /// The ending value of the start date of the work to filter the start date by using range.
        /// Optional.
        /// </summary>
        public DateTime? StartDateEnd { get; set; }

        /// <summary>
        /// The beginning value of the due date of the work to filter the due date by using range.
        /// Optional.
        /// </summary>
        public DateTime? DueDateBegin { get; set; }

        /// <summary>
        /// The ending value of the due date of the work to filter the due date by using range.
        /// Optional.
        /// </summary>
        public DateTime? DueDateEnd { get; set; }
    }

    /// <summary>
    /// Represents the response for a work query, containing details about a work item.
    /// </summary>
    public class WorkQueryResponse : QueryResponse
    {
        /// <summary>
        /// The name of the work item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the work item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The start date of the work item.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The formatted start date as a string.
        /// </summary>
        public string StartDateF { get; set; }

        /// <summary>
        /// The due date of the work item.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The formatted due date as a string.
        /// </summary>
        public string DueDateF { get; set; }

        /// <summary>
        /// The ID of the associated project.
        /// Optional.
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// The name of the associated project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// The project details related to the work item.
        /// </summary>
        public ProjectQueryResponse ProjectQueryResponse { get; set; }
    }

    /// <summary>
    /// Handles the WorkQueryRequest and returns a queryable list of WorkQueryResponse objects.
    /// </summary>
    public class WorkQueryHandler : ProjectsDbHandler, IRequestHandler<WorkQueryRequest, IQueryable<WorkQueryResponse>>
    {
        /// <summary>
        /// Initializes a new instance of the WorkQueryHandler class.
        /// </summary>
        /// <param name="projectsDb">The database context for projects.</param>
        public WorkQueryHandler(ProjectsDb projectsDb) : base(projectsDb)
        {
        }

        /// <summary>
        /// Handles the work query request by filtering and formatting the data.
        /// </summary>
        /// <param name="request">The work query request containing filters.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A queryable list of WorkQueryResponse objects.</returns>
        public Task<IQueryable<WorkQueryResponse>> Handle(WorkQueryRequest request, CancellationToken cancellationToken)
        {
            // Way 1: Fetch work items from the database and include related projects
            // then apply filters to the response based on request properties:
            //var query = _projectsDb.Works.Include(w => w.Project)
            //    .OrderByDescending(w => w.DueDate)
            //    .ThenByDescending(w => w.StartDate)
            //    .ThenBy(w => w.Name)
            //    .Select(w => new WorkQueryResponse()
            //    {
            //        Name = w.Name,
            //        Description = w.Description,
            //        DueDate = w.DueDate,
            //        DueDateF = w.DueDate.ToString("MM/dd/yyyy HH:mm:ss"),
            //        StartDate = w.StartDate,
            //        Id = w.Id,
            //        StartDateF = w.StartDate.ToShortDateString(),
            //        ProjectId = w.ProjectId,
            //        ProjectName = w.Project.Name,
            //        ProjectQueryResponse = w.Project != null ? new ProjectQueryResponse()
            //        {
            //            Description = w.Project.Description,
            //            Id = w.Project.Id,
            //            Name = w.Project.Name,
            //            TagIds = w.Project.TagIds,
            //            Url = w.Project.Url,
            //            Version = w.Project.Version,
            //            VersionF = w.Project.Version.HasValue ? w.Project.Version.Value.ToString("N1") : string.Empty
            //        } : null
            //    });
            //if (!string.IsNullOrWhiteSpace(request.Name))
            //    query = query.Where(w => w.Name == request.Name);
            //if (request.ProjectId.HasValue)
            //    query = query.Where(w => w.ProjectId == request.ProjectId);
            //if (request.StartDateBegin.HasValue)
            //    query = query.Where(w => w.StartDate >= request.StartDateBegin.Value); // or: query = query.Where(w => w.StartDate >= request.StartDateBegin)
            //if (request.StartDateEnd.HasValue)
            //    query = query.Where(w => w.StartDate <= request.StartDateEnd.Value);
            //if (request.DueDateBegin.HasValue)
            //    query = query.Where(w => w.DueDate >= request.DueDateBegin);
            //if (request.DueDateEnd.HasValue)
            //    query = query.Where(w => w.DueDate <= request.DueDateEnd);



            // Way 2: Fetch work items from the database and include related projects
            // then apply filters to the entities based on request properties:
            var entityQuery = _projectsDb.Works.Include(w => w.Project)
                .OrderByDescending(w => w.DueDate)
                .ThenByDescending(w => w.StartDate)
                .ThenBy(w => w.Name)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Name))
                entityQuery = entityQuery.Where(w => w.Name == request.Name);
            if (request.ProjectId.HasValue)
                entityQuery = entityQuery.Where(w => w.ProjectId == request.ProjectId);
            if (request.StartDateBegin.HasValue)
                entityQuery = entityQuery.Where(w => w.StartDate >= request.StartDateBegin);
            if (request.StartDateEnd.HasValue)
                entityQuery = entityQuery.Where(w => w.StartDate <= request.StartDateEnd);
            if (request.DueDateBegin.HasValue)
                entityQuery = entityQuery.Where(w => w.DueDate >= request.DueDateBegin);
            if (request.DueDateEnd.HasValue)
                entityQuery = entityQuery.Where(w => w.DueDate <= request.DueDateEnd);
            var query = entityQuery.Select(w => new WorkQueryResponse()
            {
                Name = w.Name,
                Description = w.Description,
                DueDate = w.DueDate,
                DueDateF = w.DueDate.ToString("MM/dd/yyyy HH:mm:ss"),
                StartDate = w.StartDate,
                Id = w.Id,
                StartDateF = w.StartDate.ToShortDateString(),
                ProjectId = w.ProjectId,
                ProjectName = w.Project.Name,
                ProjectQueryResponse = w.Project != null ? new ProjectQueryResponse()
                {
                    Description = w.Project.Description,
                    Id = w.Project.Id,
                    Name = w.Project.Name,
                    TagIds = w.Project.TagIds,
                    Url = w.Project.Url,
                    Version = w.Project.Version,
                    VersionF = w.Project.Version.HasValue ? w.Project.Version.Value.ToString("N1") : string.Empty
                } : null
            });



            return Task.FromResult(query);
        }
    }
}