using System.Net.Http.Json;

namespace MultipleApiRequester.BusinessLayer.CompanyClients;

class Company2RequestData
{
    public Company2RequestData(DeliveryRequest deliveryRequest)
    {
        Consignor = deliveryRequest.SourceAddress;
        Consignee = deliveryRequest.DestinationAddress;
        Cartons = deliveryRequest.CartonsDimensions; // Can define api-related dimensions and map them too
    }

    public string Consignor { get; }
    public string Consignee { get; }
    public IReadOnlyCollection<CartonDimensions> Cartons { get; }
}

class Company2ResponseData
{
    public decimal Amount { get; set; }
}

public class Company2Client : BaseCompanyClient
{
    public Company2Client(HttpClient httpClient) : base(httpClient) {}

    private const string CompanyNameInternal = "Company2";

    protected override string CompanyName => CompanyNameInternal;

    protected override HttpRequestMessage GetHttpRequestMessage(DeliveryRequest deliveryRequest) 
        => new HttpRequestMessage(HttpMethod.Post, "/") { Content = JsonContent.Create(new Company2RequestData(deliveryRequest)) }; 

    protected override async Task<decimal> ParseResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        Company2ResponseData? responseData = await response.Content.ReadFromJsonAsync<Company2ResponseData>(cancellationToken: cancellationToken);
        if (responseData is null)
        {
            throw new FormatException("Unexpected response data");
        }

        return responseData.Amount;
    }
}