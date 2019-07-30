using System.Net;
using System.Threading.Tasks;
using dummy.api.Configuration;
using dummy.api.Controllers.ApiModels.Request;
using dummy.api.Controllers.ApiModels.Result;
using OpenTracing;
using Serilog;

namespace dummy.api.Handlers
{
    public class FillValuesRequestHandler : IHandleRequest<FillValuesRequest, SimpleRequestResultModel>
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<FillValuesRequestHandler>();

        private readonly ITracer tracer;

        private readonly IDummyConfiguration dummyConfiguration;

        public FillValuesRequestHandler(IDummyConfiguration dummyConfiguration, ITracer tracer)
        {
            this.dummyConfiguration = dummyConfiguration;
            this.tracer = tracer;
        }

        public Task<SimpleRequestResultModel> Handle(FillValuesRequest request)
        {
            Log.Information("Filling some values {Values}", request.Values);

            return Task.FromResult(new SimpleRequestResultModel(HttpStatusCode.OK, "All good"));
        }
    }
}