using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using MultipleApiRequester.BusinessLayer;
using MultipleApiRequester.BusinessLayer.CompanyClients;
using MultipleApiRequester.ConsoleSampleApp;

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("config.json", optional: false);
IConfiguration config = configurationBuilder.Build();

EndpointData company1EndpointData = config.GetSection("Company1Endpoint").Get<EndpointData>();
EndpointData company2EndpointData = config.GetSection("Company2Endpoint").Get<EndpointData>();
EndpointData company3EndpointData = config.GetSection("Company3Endpoint").Get<EndpointData>();

HttpClient InitHttpClient(EndpointData endpointData)
{
    if (endpointData.BaseUrl is null) throw new ArgumentNullException(nameof(endpointData.BaseUrl));
    if (endpointData.AccessToken is null) throw new ArgumentNullException(nameof(endpointData.AccessToken));

    HttpClient httpClient = new HttpClient() { BaseAddress = new Uri(endpointData.BaseUrl) };
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", endpointData.AccessToken);
    return httpClient;
}

IDeliveryResearcher deliveryResearcher = new DeliveryResearcher(new IDeliveryClient[] {
    new Company1Client(InitHttpClient(company1EndpointData)),
    new Company2Client(InitHttpClient(company2EndpointData)),
    new Company3Client(InitHttpClient(company3EndpointData))
});

// UI isn't required, so I simply hardcode delivery request with random US addresses into this sample app
DeliveryRequest newDeliveryRequest = new DeliveryRequest(
    "Po Box 567 Cairo, Georgia(GA), 39828",
    "Po Box 3016 Telluride, Colorado(CO), 81435",
    new[] { new CartonDimensions(123, 32, 42), new CartonDimensions(325, 143, 54) }
);

// I just want use somehow introduced cancellationToken, so let's use it to set 30 seconds timeout
using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(30000))
{
    DeliveryResponse optimalDeliveryResponse = await deliveryResearcher.GetResponseWithMinPriceAsync(newDeliveryRequest, cancellationTokenSource.Token);
    Console.WriteLine($"Best delivery option is {optimalDeliveryResponse.CompanyName} with price {optimalDeliveryResponse.Price.ToString("C")}");
}