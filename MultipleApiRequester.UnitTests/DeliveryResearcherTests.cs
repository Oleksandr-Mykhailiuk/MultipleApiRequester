using Moq;
using MultipleApiRequester.BusinessLayer;

namespace MultipleApiRequester.UnitTests;

public class DeliveryResearcherTests
{
    [Fact]
    public void DeliveryResearcher_CanNotBeCreated_WithoutClients() 
        => Assert.Throws<InvalidOperationException>(() => new DeliveryResearcher(new IDeliveryClient[] {}));

    private static Mock<IDeliveryClient> SetupDeliveryClientMock(string companyName, decimal price)
    {
        Mock<IDeliveryClient> deliveryClientMock = new Mock<IDeliveryClient>();
        deliveryClientMock
            .Setup(x => x.GetDeliveryEstimationAsync(It.IsAny<DeliveryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeliveryResponse(companyName, price));
        
        return deliveryClientMock;
    }

    [Fact]
    public async Task GetResponseWithMinPriceAsync_ReturnsResponseWithMinPrice_WhenAllClientsGiveResponseWithoutError()
    {
        Mock<IDeliveryClient> deliveryClientMock1 = SetupDeliveryClientMock("TestCompany1", 234m);
        Mock<IDeliveryClient> deliveryClientMock2 = SetupDeliveryClientMock("TestCompany2", 132m);
        Mock<IDeliveryClient> deliveryClientMock3 = SetupDeliveryClientMock("TestCompany3", 182m);
        DeliveryResearcher deliveryResearcher = new DeliveryResearcher(new IDeliveryClient[] {
            deliveryClientMock1.Object,
            deliveryClientMock2.Object,
            deliveryClientMock3.Object
        });

        DeliveryResponse optimalDeliveryResponse = await deliveryResearcher.GetResponseWithMinPriceAsync(
            new DeliveryRequest("TestSource", "TestDestination", new CartonDimensions[] {}), CancellationToken.None);

        Assert.Equal("TestCompany2", optimalDeliveryResponse.CompanyName);
        Assert.Equal(132m, optimalDeliveryResponse.Price);
    }

    [Fact]
    public async Task GetResponseWithMinPriceAsync_DidNotSwallowOrWrapException_WhenClientFail()
    {
        string expectedErrorMessage = "Some test error";
        Mock<IDeliveryClient> deliveryClientMock = new Mock<IDeliveryClient>();
        deliveryClientMock
            .Setup(x => x.GetDeliveryEstimationAsync(It.IsAny<DeliveryRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(expectedErrorMessage));
        DeliveryResearcher deliveryResearcher = new DeliveryResearcher(new IDeliveryClient[] {
            deliveryClientMock.Object
        });

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => deliveryResearcher.GetResponseWithMinPriceAsync(
                new DeliveryRequest("TestSource", "TestDestination", new CartonDimensions[] {}), CancellationToken.None));
        Assert.Equal(expectedErrorMessage, exception.Message);
    }
}