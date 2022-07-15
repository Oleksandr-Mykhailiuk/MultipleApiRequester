using System.Text;
using MultipleApiRequester.BusinessLayer;
using MultipleApiRequester.BusinessLayer.CompanyClients;

namespace MultipleApiRequester.UnitTests.CompanyClientsTests;

public class Company2ClientTests : BaseCompanyClientTests
{
    protected override string ExpectedCompanyName => "Company2";

    protected override IDeliveryClient InitClient(HttpClient httpClient) => new Company2Client(httpClient);

    protected override StringContent MockedResponseContent(decimal expectedPrice) 
        => new StringContent($"{{\"amount\":{expectedPrice}}}", Encoding.UTF8, "application/json");

    protected override string[] RequestFragmentsToCheck(DeliveryRequest deliveryRequest)
        => new[] { 
            "{",
            $"\"consignor\":\"{deliveryRequest.SourceAddress}\"",
            $"\"consignee\":\"{deliveryRequest.DestinationAddress}\"",
            "\"cartons\":[",
            $"{{\"length\":{deliveryRequest.CartonsDimensions.First().Length}",
            $"\"width\":{deliveryRequest.CartonsDimensions.First().Width}",
            $"\"height\":{deliveryRequest.CartonsDimensions.First().Height}}}",
            "]}"
        };    
}