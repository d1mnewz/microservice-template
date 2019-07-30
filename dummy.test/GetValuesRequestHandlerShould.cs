using System.Net;
using System.Threading.Tasks;
using dummy.api.Configuration;
using dummy.api.Controllers.ApiModels.Request;
using dummy.api.Controllers.ApiModels.Result;
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
    public class GetValuesRequestHandlerShould
    {
        private HttpTest httpTest;
        private IDummyConfiguration dummyConfiguration;
        private readonly GetValuesRequestHandler sut;

        public GetValuesRequestHandlerShould()
        {
            this.dummyConfiguration = Substitute.For<IDummyConfiguration>();
            this.dummyConfiguration.SomeHost.Returns(new Url("http://test:80"));

            this.sut = new GetValuesRequestHandler(this.dummyConfiguration, new MockTracer());
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
            var request = new GetValuesRequest();

            var result = await this.sut.Handle(request);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Result.Should().NotBeNull().And.BeOfType<GetValuesResult>();
            result.Result.Values.Should().NotBeEmpty();
        }
    }
}