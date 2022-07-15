namespace MultipleApiRequester.BusinessLayer;

public interface IDeliveryClient
{
    Task<DeliveryResponse> GetDeliveryEstimationAsync(DeliveryRequest deliveryRequest, CancellationToken cancellationToken);
}