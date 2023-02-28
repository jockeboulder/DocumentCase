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
        
        using var client = new HttpClient();
        var result = await client.PostAsync($"http://localhost:5002/Pdf/Create/documentNumber={documentNumber}&customerNumber={customerNumber}", data);
        return result;
    }
}