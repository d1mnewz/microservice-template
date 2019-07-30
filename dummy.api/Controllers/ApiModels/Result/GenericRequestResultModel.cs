using System.Net;

namespace dummy.api.Controllers.ApiModels.Result
{
    /// <summary>
    /// Generic Request Result.
    /// </summary>
    /// <typeparam name="T">Entity to hold as Result.</typeparam>
    public class GenericRequestResultModel<T> : SimpleRequestResultModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRequestResultModel{T}"/> class.
        /// </summary>
        /// <param name="statusCode">HTTP Status Code.</param>
        public GenericRequestResultModel(HttpStatusCode statusCode)
            : base(statusCode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRequestResultModel{T}"/> class.
        /// </summary>
        /// <param name="statusCode">HTTP Status Code.</param>
        /// <param name="message">Message.</param>
        public GenericRequestResultModel(HttpStatusCode statusCode, string message)
            : base(statusCode, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRequestResultModel{T}"/> class.
        /// </summary>
        /// <param name="statusCode">HTTP Status Code.</param>
        /// <param name="result">Result.</param>
        public GenericRequestResultModel(HttpStatusCode statusCode, T result)
            : base(statusCode)
        {
            this.Result = result;
        }

        /// <summary>
        /// Result.
        /// </summary>
        public T Result { get; set; }
    }
}