using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace APP.Movies.Features.Directors
{
    public class DirectorCreateRequest : IRequest<Response<int>>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsRetired { get; set; }
    }

    public class DirectorCreateHandler : Handler<DirectorCreateRequest, Response<int>>
    {
        private readonly MoviesDbHandler _dbHandler;

        public DirectorCreateHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response<int>> Handle(DirectorCreateRequest request, CancellationToken cancellationToken)
        {
            var director = new Director
            {
                Name = request.Name,
                Surname = request.Surname,
                IsRetired = request.IsRetired
            };

            await _dbHandler.Add(director);

            return new Response<int>(true, "Director created successfully", director.Id);
        }
    }
}