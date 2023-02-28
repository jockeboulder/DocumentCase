using System;
using System.Net.Http;
using System.Threading.Tasks;

public class StorageClient
{
    public static async Task<HttpResponseMessage> StoreAsync(byte[] file, string name, string extension)
    {
        using var client = new HttpClient();
        ByteArrayContent byteContent = new ByteArrayContent(file);
        var result = await client.PostAsync($"http://localhost:5003/Storage/name={name}&extension={extension}", byteContent);
        return result;
    }
}