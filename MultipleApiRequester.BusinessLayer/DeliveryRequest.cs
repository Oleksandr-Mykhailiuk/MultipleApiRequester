namespace MultipleApiRequester.BusinessLayer;

public class DeliveryRequest
{
    public DeliveryRequest(string sourceAddress, string destinationAddress, IReadOnlyCollection<CartonDimensions> cartonsDimensions)
    {
        SourceAddress = sourceAddress;
        DestinationAddress = destinationAddress;
        CartonsDimensions = cartonsDimensions;
    }

    public string SourceAddress { get; set; }
    public string DestinationAddress { get; set; }
    public IReadOnlyCollection<CartonDimensions> CartonsDimensions { get; set; }
}