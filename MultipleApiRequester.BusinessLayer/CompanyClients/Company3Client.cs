using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace MultipleApiRequester.BusinessLayer.CompanyClients;

// need for xml serialization
public class Company3Package
{
    [XmlAttribute("length")]
    public int Length { get; set; }
    [XmlAttribute("width")]
    public int Width { get; set; }
    [XmlAttribute("height")]
    public int Height { get; set; }
}

[XmlRoot("xml")]
public class Company3RequestData
{
    public Company3RequestData()
    {
        Source = string.Empty;
        Destination = string.Empty;
        Packages = new Company3Package[] {};
    }

    public Company3RequestData(DeliveryRequest deliveryRequest)
    {
        Source = deliveryRequest.SourceAddress;
        Destination = deliveryRequest.DestinationAddress;
        Packages = deliveryRequest.CartonsDimensions.Select( x => new Company3Package {
                Length = x.Length, Width = x.Width, Height = x.Height
            }).ToArray();
    }

    [XmlElement("source")]
    public string Source { get; set; }
    [XmlElement("destination")]
    public string Destination { get; set; }
    [XmlArray("packages")]
    [XmlArrayItem("package")]
    public Company3Package[] Packages { get; set; }
}

[XmlRoot("xml")]
public class Company3ResponseData
{
    public Company3ResponseData() {}

    [XmlElement("quote")]
    public decimal Quote { get; set; }
}

public class Company3Client : BaseCompanyClient
{
    public Company3Client(HttpClient httpClient) : base(httpClient) {}

    private const string CompanyNameInternal = "Company3";

    protected override string CompanyName => CompanyNameInternal;

    protected override HttpRequestMessage GetHttpRequestMessage(DeliveryRequest deliveryRequest)
    {
        // remove xml declaration and namespaces
        XmlWriterSettings settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true
        };
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
        
        string xmlContent;
        using (StringWriter stream = new StringWriter())
        using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Company3RequestData));
            xmlSerializer.Serialize(xmlWriter, new Company3RequestData(deliveryRequest), ns);
            xmlContent = stream.ToString();
        }

        return new HttpRequestMessage(HttpMethod.Post, "/")
        {
            Content = new StringContent(xmlContent, Encoding.UTF8, "application/xml")
        };
    }

    protected override async Task<decimal> ParseResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        Company3ResponseData? responseData;
        string xmlResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        using(TextReader reader = new StringReader(xmlResponse))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Company3ResponseData));
            responseData = xmlSerializer.Deserialize(reader) as Company3ResponseData;
        }

        if (responseData is null)
        {
            throw new FormatException("Unexpected response data");
        }

        return responseData.Quote;
    }
}