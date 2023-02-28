using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WkHtmlToPdfDotNet;

namespace PdfService.Docker.Controllers;

[ApiController]
[Route("[controller]")]
public class PdfController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(Guid documentNumber, string customerNumber, string documentText)
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

        byte[] pdf = converter.Convert(doc);

        var result = await StorageClient.StoreAsync(pdf, documentNumber.ToString(), ".pdf");
        if (result.IsSuccessStatusCode)
        {
            return Ok(result.Content);
        }

        return NotFound();
    }
}
