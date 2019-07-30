using System.Threading.Tasks;
using dummy.api.Controllers.ApiModels.Request;
using dummy.api.Controllers.ApiModels.Result;

namespace dummy.api.Handlers
{
    /// <summary>
    /// Handler for a request.
    /// </summary>
    /// <typeparam name="TRequest">Request to handle.</typeparam>
    /// <typeparam name="TResponse">Response to return.</typeparam>
    public interface IHandleRequest<in TRequest, TResponse>
        where TRequest : IRequestModel
        where TResponse : IResultModel
    {
        /// <summary>
        /// Handle request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation holding.</returns>
        Task<TResponse> Handle(TRequest request);
    }

    public interface IHandleRequest<in TRequest>
        where TRequest : IRequestModel
    {
        Task HandleAsync(TRequest request);
    }
}