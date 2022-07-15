namespace MultipleApiRequester.BusinessLayer;

public interface IDeliveryResearcher
{
    Task<DeliveryResponse> GetResponseWithMinPriceAsync(DeliveryRequest deliveryRequest, CancellationToken cancellationToken);
}