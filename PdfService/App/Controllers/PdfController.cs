using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WkHtmlToPdfDotNet;

namespace PdfService.Docker.Controllers;

[ApiController]
[Route("[controller]")]
public class PdfController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly StorageClient _storageClient;

    public PdfController(IOptions<BlobCredentials> blobCredentials, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _storageClient = new StorageClient(blobCredentials, _httpClient);
    }
    
    [Route("Create")]
    [HttpPost]
    public async Task<IActionResult> Create([FromQuery]Guid documentNumber, [FromQuery]string customerNumber, [FromBody]string documentText)
    {
        DocumentModel document = new()
        {
            DocumentNumber = documentNumber,
            CustomerNumber = customerNumber,
            DocumentText = documentText
        };

        string html = @"<h2>Document number: {{documentNumber}}</h2>
            </br>
            <h2>Personal number: {{customerNumber}}</h2>
            </br>
            </br>
            {{documentText}}
            ";
        var converter = new BasicConverter(new PdfTools());
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings() { Top = 10 },
            },
            Objects = {
            new ObjectSettings()
            {
                HtmlContent = html,
                Encoding = Encoding.UTF8,
                WebSettings = { DefaultEncoding = "utf-8" },
            },
            }
        };
        byte[] pdf = converter.Convert(doc); // should work but macOS doesn't support this package atm...

        // Preferably we would use something like Azure Client to send this to an Azure Storage Account
        var result = await _storageClient.StoreToAzureAsync(pdf, documentNumber.ToString(), "pdf");
        if (result.IsSuccessStatusCode)
        {
            return Ok(result.Content);
        }

        return NotFound();
    }

    [Route("Get")]
    [HttpGet]
    public async Task<HttpResponseMessage> GetAsync([FromQuery]Guid documentNumber, [FromQuery]string customerNumber)
    {
        return await _storageClient.GetFromAzureAsync(documentNumber, customerNumber);
    }
}
