using Microsoft.AspNetCore.Mvc;
using Clients;

namespace DotNet.Docker.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IPdfClient _pdfClient;

    public DocumentController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _pdfClient = new PdfClient(_httpClient);
    }

    // TODO: Could add caching to this request if we're making similar requests
    [Route("Get")]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery]Guid documentNumber, [FromQuery]string customerNumber)
    {
        var result = await _pdfClient.GetAsync(documentNumber, customerNumber);
        if (result.IsSuccessStatusCode)
        {
            return Ok(result.Content);
        }
        return NotFound();
    }

    [Route("Create")]
    [HttpPost]
    public async Task<IActionResult> Create([FromQuery]Guid documentNumber, [FromQuery]string customerNumber, [FromBody]string documentText)
    {
        // TODO: Should call Storage instead...
        var result = await _pdfClient.CreateAsync(documentNumber, customerNumber, documentText);
        if (result.IsSuccessStatusCode)
        {
            return Ok(result.Content);
        }
        return NotFound();
    }
}
