using System.Net.Http.Json;

namespace MultipleApiRequester.BusinessLayer.CompanyClients;

class Company1RequestData
{
    public Company1RequestData(DeliveryRequest deliveryRequest)
    {
        ContactAddress = deliveryRequest.SourceAddress;
        WarehouseAddress = deliveryRequest.DestinationAddress;
        PackageDimensions = deliveryRequest.CartonsDimensions; // Can define api-related dimensions and map them too
    }

    public string ContactAddress { get; }
    public string WarehouseAddress { get; }
    public IReadOnlyCollection<CartonDimensions> PackageDimensions { get; }
}

class Company1ResponseData
{
    public decimal Total { get; set; }
}

public class Company1Client : BaseCompanyClient
{
    public Company1Client(HttpClient httpClient) : base(httpClient) {}

    private const string CompanyNameInternal = "Company1";

    protected override string CompanyName => CompanyNameInternal;

    protected override HttpRequestMessage GetHttpRequestMessage(DeliveryRequest deliveryRequest) 
        => new HttpRequestMessage(HttpMethod.Post, "/") { Content = JsonContent.Create(new Company1RequestData(deliveryRequest)) }; 

    protected override async Task<decimal> ParseResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        Company1ResponseData? responseData = await response.Content.ReadFromJsonAsync<Company1ResponseData>(cancellationToken: cancellationToken);
        if (responseData is null)
        {
            throw new FormatException("Unexpected response data");
        }

        return responseData.Total;
    }
}