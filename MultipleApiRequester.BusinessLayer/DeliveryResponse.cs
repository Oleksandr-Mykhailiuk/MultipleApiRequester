namespace MultipleApiRequester.BusinessLayer;

public class DeliveryResponse
{

    public DeliveryResponse(string companyName, decimal price)
    {
        CompanyName = companyName;
        Price = price;
    }

    public string CompanyName { get; }
    public decimal Price { get; }
}