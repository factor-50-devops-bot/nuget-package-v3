using HelpMyStreet.Utils.PollyPolicies;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelpMyStreet.UnitTests
{
    public class PollyHttpPoliciesTests
    {
        private Mock<IPollyHttpPoliciesConfig> _pollyHttpPoliciesConfig;

        private Mock<IServiceCaller> _serviceCaller;
        
        [SetUp]
        public void SetUp()
        {
            TimeSpan[] pausesOnError = new TimeSpan[]
            {
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1),
                TimeSpan.FromMilliseconds(1)
            };

            _pollyHttpPoliciesConfig = new Mock<IPollyHttpPoliciesConfig>();

            _pollyHttpPoliciesConfig.SetupGet(x => x.ServiceErrorPauses).Returns(pausesOnError);
            _pollyHttpPoliciesConfig.SetupGet(x => x.AzureFunctionNotStartedPauses).Returns(pausesOnError);

        }

        [Test]
        public async Task ExternalHttpRetryPolicy()
        {
            _serviceCaller = new Mock<IServiceCaller>();
            _serviceCaller.SetupSequence(x => x.GetAsync())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.RequestTimeout))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadGateway))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.GatewayTimeout))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InsufficientStorage))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            PollyHttpPolicies pollyHttpPolicies = new PollyHttpPolicies(_pollyHttpPoliciesConfig.Object);

            HttpResponseMessage result = await pollyHttpPolicies.ExternalHttpRetryPolicy.ExecuteAsync(async () => await _serviceCaller.Object.GetAsync());
            Assert.AreEqual(200, (int)result.StatusCode);
        }

        [Test]
        public async Task InternalHttpRetryPolicy()
        {
            _serviceCaller = new Mock<IServiceCaller>();
            _serviceCaller.SetupSequence(x => x.GetAsync())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.RequestTimeout))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadGateway))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.GatewayTimeout))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InsufficientStorage))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            PollyHttpPolicies pollyHttpPolicies = new PollyHttpPolicies(_pollyHttpPoliciesConfig.Object);

            HttpResponseMessage result = await pollyHttpPolicies.InternalHttpRetryPolicy.ExecuteAsync(async () => await _serviceCaller.Object.GetAsync());
            Assert.AreEqual(200, (int)result.StatusCode);

            _serviceCaller.Verify(x => x.GetAsync(), Times.Exactly(7));
        }

        [Test]
        public async Task InternalHttpRetryPolicy_CorrectWaitTimesAreUsedWhenServiceUnavailable()
        {
            _pollyHttpPoliciesConfig.SetupGet(x => x.AzureFunctionNotStartedPauses).Returns(new TimeSpan[0]);

            _serviceCaller = new Mock<IServiceCaller>();
            _serviceCaller.SetupSequence(x => x.GetAsync())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadGateway))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadGateway))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            PollyHttpPolicies pollyHttpPolicies = new PollyHttpPolicies(_pollyHttpPoliciesConfig.Object);

            HttpResponseMessage result = await pollyHttpPolicies.InternalHttpRetryPolicy.ExecuteAsync(async () => await _serviceCaller.Object.GetAsync());

            Assert.AreEqual(503, (int)result.StatusCode); // shows the correct wait times were used for ServiceUnavailable (since it was empty the ServiceUnavailable is returned)
        }

    }
}
