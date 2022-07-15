namespace MultipleApiRequester.BusinessLayer;

public class DeliveryResearcher : IDeliveryResearcher
{
    private readonly IReadOnlyCollection<IDeliveryClient> _clients;

    public DeliveryResearcher(IReadOnlyCollection<IDeliveryClient> clients)
    {
        if (clients is null || clients.Count < 1) throw new InvalidOperationException($"{nameof(clients)} should contain at least on element");
        _clients = clients;
    }

    public async Task<DeliveryResponse> GetResponseWithMinPriceAsync(DeliveryRequest deliveryRequest, CancellationToken cancellationToken)
    {
        Task<DeliveryResponse>[] tasks = _clients.Select(x => x.GetDeliveryEstimationAsync(deliveryRequest, cancellationToken)).ToArray();
        // Do in parallel to minimize waiting time
        await Task.WhenAll(tasks);

        // get result safely via await (also can use Result property and MinBy LINQ method)
        DeliveryResponse result = await tasks.First();
        for (int i = 1; i < tasks.Length; i++)
        {
            DeliveryResponse nextDeliveryResponse = await tasks[i];
            if (result.Price > nextDeliveryResponse.Price)
            {
                result = nextDeliveryResponse;
            }
        }
        return result;
    }
}