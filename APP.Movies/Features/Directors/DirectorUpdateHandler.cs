//using APP.Movies.Domain;
using CORE.APP.Features;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using APP.Movies.Domain;

namespace APP.Movies.Features.Directors
{
    public class DirectorUpdateRequest : IRequest<CORE.APP.Features.Response>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsRetired { get; set; }
    }

    public class DirectorUpdateHandler : Handler<DirectorUpdateRequest, Response>
    {
        private readonly MoviesDbHandler _dbHandler;

        public DirectorUpdateHandler(MoviesDbHandler dbHandler)
        {
            _dbHandler = dbHandler;
        }

        public override async Task<Response> Handle(DirectorUpdateRequest request, CancellationToken cancellationToken)
        {
            // Find the director
            var director = await _dbHandler.Find<Director>(request.Id);
            if (director == null)
                return new Response(false, $"Director with ID {request.Id} not found");

            // Update director properties
            director.Name = request.Name;
            director.Surname = request.Surname;
            director.IsRetired = request.IsRetired;

            await _dbHandler.Update(director);

            return new Response(true, "Director updated successfully");
        }
    }
}