using CORE.APP.Domain;
using MediatR;

namespace CORE.APP.Features
{
    /// <summary>
    /// Abstract base class for all requests, inheriting from <see cref="Entity"/>.
    /// </summary>
    public abstract class Request<TResponse> : Entity, IRequest<TResponse>
    {
    }
}
