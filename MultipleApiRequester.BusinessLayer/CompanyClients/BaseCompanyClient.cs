namespace MultipleApiRequester.BusinessLayer.CompanyClients;

public abstract class BaseCompanyClient : IDeliveryClient
{
    private readonly HttpClient _httpClient;

    protected BaseCompanyClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected abstract string CompanyName { get; }

    protected abstract HttpRequestMessage GetHttpRequestMessage(DeliveryRequest deliveryRequest);

    protected abstract Task<decimal> ParseResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken);

    public async Task<DeliveryResponse> GetDeliveryEstimationAsync(DeliveryRequest deliveryRequest, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = GetHttpRequestMessage(deliveryRequest);
        HttpResponseMessage response;
        try
        {    
            response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred during sending request to {CompanyName}", ex);
        }

        decimal estimatedPrice;
        try
        {
            estimatedPrice = await ParseResponseAsync(response, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred during parsing response from {CompanyName}", ex);
        }
        return new DeliveryResponse(CompanyName, estimatedPrice);
    }
}