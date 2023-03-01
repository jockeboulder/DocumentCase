using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class StorageClient
{
    public static async Task<HttpResponseMessage> StoreAsync(byte[] file, string name, string extension)
    {
        using var client = new HttpClient();
        var json = Convert.ToBase64String(file);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await client.PostAsync($"http://localhost:5146/Storage?fileName={name}&extension={extension}", data);
        return result;
    }
}