using System.Net;

namespace dummy.api.Controllers.ApiModels.Result
{
    /// <summary>
    /// Simple Request Result.
    /// </summary>
    public class SimpleRequestResultModel : IResultModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleRequestResultModel"/> class.
        /// </summary>
        /// <param name="statusCode">HTTP Status Code.</param>
        public SimpleRequestResultModel(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleRequestResultModel"/> class.
        /// </summary>
        /// <param name="statusCode">HTTP Status Code.</param>
        /// <param name="message">Message.</param>
        public SimpleRequestResultModel(HttpStatusCode statusCode, string message)
        {
            this.StatusCode = statusCode;
            this.Message = message;
        }

        /// <summary>
        /// HTTP Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Message.
        /// </summary>
        public string Message { get; set; }
    }
}