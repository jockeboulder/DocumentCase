using System.Net;
using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

public class StorageClient
{
    private readonly BlobCredentials _blobCredentials;

    public StorageClient(IOptions<BlobCredentials> blobCredentials)
    {
        _blobCredentials = blobCredentials.Value;
        ConfigurationHelper<BlobCredentials>.Initialize(blobCredentials);
    }

    public static async Task<HttpResponseMessage> StoreToDiskAsync(byte[] file, string name, string extension)
    {
        using var client = new HttpClient();
        var json = Convert.ToBase64String(file);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await client.PostAsync($"http://localhost:5146/Storage?fileName={name}&extension={extension}", data);
        return result;
    }

    public static async Task<HttpResponseMessage> StoreToAzureAsync(byte[] file, string name, string extension)
    {
        HttpResponseMessage response = new();
        // TODO: Maybe inject client instead of creating new instance on every call?
        BlobContainerClient containerClient = new(ConfigurationHelper<BlobCredentials>._options.Value.ConnectionString, ConfigurationHelper<BlobCredentials>._options.Value.ContainerName);
        try
        {
            // Get a reference to the blob just uploaded from the API in a container from configuration settings
            BlobClient client = containerClient.GetBlobClient(name);

            MemoryStream stream = new MemoryStream();
            stream.Write(file, 0, file.Length);

            // Upload the file async
            var res = await client.UploadAsync(stream);

            // Everything is OK and file got uploaded
            Console.WriteLine($"File {name} Uploaded Successfully");
            ConvertAzureResponse(res.GetRawResponse());
        }
        // If the file already exists, we catch the exception and do not upload it
        catch (RequestFailedException ex)
        when (ex.Status == (int)HttpStatusCode.Conflict)
        {
            // TODO: use SeriLog
            // Log.Error($"File with name {name} already exists in container. Set another name to store the file in the container: '{ConfigurationHelper<BlobCredentials>._options.Value.ContainerName}.'");
            return new HttpResponseMessage(HttpStatusCode.Conflict);
        } 
        // If we get an unexpected error, we catch it here and return the error message
        catch (RequestFailedException ex)
        {
            // TODO: use SeriLog
            // Log.Error($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }

        // Return the BlobUploadResponse object
        return response;
    }

    private static HttpResponseMessage ConvertAzureResponse(Response response)
    {
        return new HttpResponseMessage()
        {
            StatusCode = (HttpStatusCode)response.Status,
            Content = new ByteArrayContent(response.Content.ToArray()),
        };
    }
}