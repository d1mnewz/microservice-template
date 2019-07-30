using System.Threading.Tasks;
using dummy.api.Controllers.ApiModels.Request;
using dummy.api.Controllers.ApiModels.Result;
using dummy.api.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;

namespace dummy.api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{v:apiVersion}/values")]
    public class ValuesV1Controller : ControllerBase
    {
        private readonly IHandleRequest<FillValuesRequest, SimpleRequestResultModel>
            fillValuesRequestHandler;

        private readonly IHandleRequest<GetValuesRequest, GenericRequestResultModel<GetValuesResult>>
            getValuesHandler;

        public ValuesV1Controller(
            IHandleRequest<FillValuesRequest, SimpleRequestResultModel> fillValuesRequestHandler,
            IHandleRequest<GetValuesRequest, GenericRequestResultModel<GetValuesResult>> getValuesHandler)
        {
            this.fillValuesRequestHandler = fillValuesRequestHandler;
            this.getValuesHandler = getValuesHandler;
        }

        /// <summary>
        /// Fill in some values.
        /// </summary>
        /// <param name="request">Request model.</param>
        /// <response code="204">Values got registered successfully.</response>
        /// <response code="400">The model supplied is invalid.</response>
        /// <response code="401">User is unauthorized to perform this action. Use Authorization Bearer header.</response>
        /// <response code="500">System failed to process this request.</response>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation holding a <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [SwaggerRequestExample(typeof(FillValuesRequest), typeof(FillValuesRequestExample))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("")]
        public async Task<IActionResult> FillValues([FromBody] FillValuesRequest request)
        {
            var result =
                await this.fillValuesRequestHandler.Handle(request);

            return this.StatusCode((int)result.StatusCode, result.Message);
        }

        /// <summary>
        /// Get some values.
        /// </summary>
        /// <response code="200">Some values are returned.</response>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation holding a <see cref="ActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("")]
        public async Task<IActionResult> GetValues()
        {
            var result =
                await this.getValuesHandler.Handle(new GetValuesRequest());

            return this.StatusCode((int)result.StatusCode, result.Message);
        }
    }
}