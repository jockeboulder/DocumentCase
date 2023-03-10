using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

public class StorageClient
{
    private readonly BlobCredentials _blobCredentials;
    private readonly HttpClient _httpClient;
    public StorageClient(IOptions<BlobCredentials> blobCredentials, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5146/Storage/");
        _blobCredentials = blobCredentials.Value;
    }

    public async Task<HttpResponseMessage> GetFromAzureAsync(Guid documentNumber, string customerNumber)
    {
        byte[] content;
        BlobClient blobClient = new(_blobCredentials.ConnectionString, _blobCredentials.ContainerName, documentNumber.ToString());
        try
        {
            MemoryStream stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);
            content = stream.ToArray();
        }
        catch (RequestFailedException ex)
        {
            // Let the user know that the directory does not exist
            Console.WriteLine($"Blob not found: {ex.Message}");
            return new HttpResponseMessage( HttpStatusCode.NotFound );
        }
        catch (DirectoryNotFoundException ex)
        {
            // Let the user know that the directory does not exist
            Console.WriteLine($"Directory not found: {ex.Message}");
            return new HttpResponseMessage( HttpStatusCode.NotFound );
        }
        HttpResponseMessage response = new (HttpStatusCode.OK);
        response.Content = new ByteArrayContent(content);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        return response;
    }

    public async Task<HttpResponseMessage> StoreToDiskAsync(byte[] file, string name, string extension)
    {
        using var client = new HttpClient();
        var json = Convert.ToBase64String(file);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var result = await client.PostAsync($"Store?fileName={name}&extension={extension}", data);
        return result;
    }

    public async Task<HttpResponseMessage> StoreToAzureAsync(byte[] file, string name, string extension)
    {
        HttpResponseMessage response = new();
        // TODO: Maybe inject client instead of creating new instance on every call?
        BlobContainerClient containerClient = new(_blobCredentials.ConnectionString, _blobCredentials.ContainerName);
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