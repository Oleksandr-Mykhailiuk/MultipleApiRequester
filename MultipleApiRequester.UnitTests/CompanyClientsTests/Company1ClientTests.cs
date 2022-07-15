using System.Text;
using MultipleApiRequester.BusinessLayer;
using MultipleApiRequester.BusinessLayer.CompanyClients;

namespace MultipleApiRequester.UnitTests.CompanyClientsTests;

public class Company1ClientTests : BaseCompanyClientTests
{
    protected override string ExpectedCompanyName => "Company1";

    protected override IDeliveryClient InitClient(HttpClient httpClient) => new Company1Client(httpClient);

    protected override StringContent MockedResponseContent(decimal expectedPrice) 
        => new StringContent($"{{\"total\":{expectedPrice}}}", Encoding.UTF8, "application/json");

    protected override string[] RequestFragmentsToCheck(DeliveryRequest deliveryRequest)
        => new[] { 
            "{",
            $"\"contactAddress\":\"{deliveryRequest.SourceAddress}\"",
            $"\"warehouseAddress\":\"{deliveryRequest.DestinationAddress}\"",
            "\"packageDimensions\":[",
            $"{{\"length\":{deliveryRequest.CartonsDimensions.First().Length}",
            $"\"width\":{deliveryRequest.CartonsDimensions.First().Width}",
            $"\"height\":{deliveryRequest.CartonsDimensions.First().Height}}}",
            "]}"
        };    
}