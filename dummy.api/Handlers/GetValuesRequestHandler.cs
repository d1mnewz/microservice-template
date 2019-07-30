using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using dummy.api.Configuration;
using dummy.api.Controllers.ApiModels.Request;
using dummy.api.Controllers.ApiModels.Result;
using OpenTracing;
using Serilog;

namespace dummy.api.Handlers
{
    public class GetValuesRequestHandler : IHandleRequest<GetValuesRequest, GenericRequestResultModel<GetValuesResult>>
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<GetValuesRequestHandler>();

        private readonly ITracer tracer;

        private readonly IDummyConfiguration dummyConfiguration;

        public GetValuesRequestHandler(IDummyConfiguration dummyConfiguration, ITracer tracer)
        {
            this.dummyConfiguration = dummyConfiguration;
            this.tracer = tracer;
        }

        public Task<GenericRequestResultModel<GetValuesResult>> Handle(GetValuesRequest request)
        {
            Log.Information("Getting some values");

            return Task.FromResult(new GenericRequestResultModel<GetValuesResult>(
                HttpStatusCode.OK,
                new GetValuesResult { Values = new List<int> { 42 } }));
        }
    }
}