using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using dummy.api.Configuration;
using dummy.api.Controllers.ApiModels.Request;
using dummy.api.Handlers;
using FluentAssertions;
using Flurl;
using Flurl.Http.Testing;
using NSubstitute;
using NUnit.Framework;
using OpenTracing.Mock;

namespace dummy.test
{
    [TestFixture]
    public class FillValuesRequestHandlerShould
    {
        private HttpTest httpTest;
        private IDummyConfiguration dummyConfiguration;
        private readonly FillValuesRequestHandler sut;

        public FillValuesRequestHandlerShould()
        {
            this.dummyConfiguration = Substitute.For<IDummyConfiguration>();
            this.dummyConfiguration.SomeHost.Returns(new Url("http://test:80"));

            this.sut = new FillValuesRequestHandler(this.dummyConfiguration, new MockTracer());
        }

        [SetUp]
        public void CreateHttpTest()
        {
            this.httpTest = new HttpTest();
        }

        [TearDown]
        public void DisposeHttpTest()
        {
            this.httpTest.Dispose();
        }

        [Test]
        public async Task HandleRequestWith200()
        {
            var request = new FillValuesRequest { Values = new List<int> { 4224, 2231 } };

            var result = await this.sut.Handle(request);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}