using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Examples;

namespace dummy.api.Controllers.ApiModels.Request
{
    [Serializable]
    public class FillValuesRequest : IRequestModel
    {
        public List<int> Values { get; set; } = new List<int>();
    }

    public class FillValuesRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new FillValuesRequest
            {
                Values = new List<int> { 42, 23 },
            };
        }
    }
}