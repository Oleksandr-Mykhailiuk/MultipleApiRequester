using System.Text;
using MultipleApiRequester.BusinessLayer;
using MultipleApiRequester.BusinessLayer.CompanyClients;

namespace MultipleApiRequester.UnitTests.CompanyClientsTests;

public class Company3ClientTests : BaseCompanyClientTests
{
    protected override string ExpectedCompanyName => "Company3";

    protected override IDeliveryClient InitClient(HttpClient httpClient) => new Company3Client(httpClient);

    protected override StringContent MockedResponseContent(decimal expectedPrice) 
        => new StringContent($"<xml><quote>{expectedPrice}</quote></xml>", Encoding.UTF8, "application/xml");

    protected override string[] RequestFragmentsToCheck(DeliveryRequest deliveryRequest)
        => new[] { 
            "<xml>",
            $"<source>{deliveryRequest.SourceAddress}</source>",
            $"<destination>{deliveryRequest.DestinationAddress}</destination>",
            "<packages>",
            $"<package length=\"{deliveryRequest.CartonsDimensions.First().Length}\" width=\"{deliveryRequest.CartonsDimensions.First().Width}\" height=\"{deliveryRequest.CartonsDimensions.First().Height}\" />",
            "</packages></xml>" 
        };    
}