using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace Clients;
public class PdfClient
{
    public static async Task<HttpResponseMessage> CreateAsync(Guid documentNumber, string customerNumber, string documentText)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(documentText);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        
        // TODO: Use DI for this instead to not create a new client per call...
        using var client = new HttpClient();
        var result = await client.PostAsync($"http://localhost:5056/Pdf?documentNumber={documentNumber}&customerNumber={customerNumber}", data);
        return result;
    }

    public static Task<HttpResponseMessage> GetAsync(Guid documentNumber, string customerNumber)
    {
        throw new NotImplementedException();
    }
}