using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Clients;

namespace DotNet.Docker.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid documentNumber, string customerNumber, string documentText)
    {
        var result = await PdfClient.CreateAsync(documentNumber, customerNumber, documentText);
        if (result.IsSuccessStatusCode)
        {
            return Ok(result.Content);
        }
        return NotFound();
    }
}
