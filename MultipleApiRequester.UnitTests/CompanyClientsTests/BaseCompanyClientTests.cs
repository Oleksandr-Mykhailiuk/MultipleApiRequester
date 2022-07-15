using System.Net;
using Moq;
using Moq.Protected;
using MultipleApiRequester.BusinessLayer;

namespace MultipleApiRequester.UnitTests.CompanyClientsTests;

public abstract class BaseCompanyClientTests
{
    protected abstract IDeliveryClient InitClient(HttpClient httpClient);

    protected abstract StringContent MockedResponseContent(decimal expectedPrice);

    protected abstract string ExpectedCompanyName { get; }

    protected abstract string[] RequestFragmentsToCheck(DeliveryRequest deliveryRequest);

    [Fact]
    public async Task GetDeliveryEstimationAsync_ShouldFail_WhenWorkWithWrongApi()
    {
        HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("http://localhost") };
        IDeliveryClient deliveryClient = InitClient(httpClient);

        await Assert.ThrowsAsync<Exception>(() => 
            deliveryClient.GetDeliveryEstimationAsync(new DeliveryRequest("TestSource", "TestDestination", new[] { new CartonDimensions(0, 0, 0) }), CancellationToken.None)); 
    }

    [Fact]
    public async Task GetDeliveryEstimationAsync_ShouldReturnProperData_ReceivedFromApi()
    {
        decimal expectedPrice = 100.23m;
        DeliveryRequest deliveryRequest = new DeliveryRequest("TestSource", "TestDestination", new[] { new CartonDimensions(1, 2, 3) });
        Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = MockedResponseContent(expectedPrice)
                });
        HttpClient httpClient = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost") };
        IDeliveryClient deliveryClient = InitClient(httpClient);

        DeliveryResponse deliveryResponse = await deliveryClient.GetDeliveryEstimationAsync(deliveryRequest, CancellationToken.None);
        
        Assert.Equal(ExpectedCompanyName, deliveryResponse.CompanyName);
        Assert.Equal(expectedPrice, deliveryResponse.Price);
        // some basic serialization checks (in general endpoint consumes properties in any order, so I don't want to compare with expected string)
        handlerMock
            .Protected()
            .Verify<Task<HttpResponseMessage>>(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>( message => message.Content != null 
                    && message.Content.ReadAsStringAsync().Result.ContainsAll(RequestFragmentsToCheck(deliveryRequest))),
                ItExpr.IsAny<CancellationToken>());
    }
}