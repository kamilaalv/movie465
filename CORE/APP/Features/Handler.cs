using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CORE.APP.Features
{
    public abstract class Handler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}

