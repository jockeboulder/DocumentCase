using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace Clients;
public class PdfClient : IPdfClient
{
    private readonly HttpClient _httpClient;

    public PdfClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://pdfservice/Pdf/");
    }

    public async Task<HttpResponseMessage> CreateAsync(Guid documentNumber, string customerNumber, string documentText)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(documentText);
        StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
        
        HttpResponseMessage result = await _httpClient.PostAsync($"Create?documentNumber={documentNumber}&customerNumber={customerNumber}", data);
        return result;
    }

    public async Task<HttpResponseMessage> GetAsync(Guid documentNumber, string customerNumber)
    {
        HttpResponseMessage result = await _httpClient.GetAsync($"Get?documentNumber={documentNumber}&customerNumber={customerNumber}");
        return result;
    }
}