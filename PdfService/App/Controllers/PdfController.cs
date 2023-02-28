using Microsoft.AspNetCore.Mvc;

namespace PdfService.Docker.Controllers;

[ApiController]
[Route("[controller]")]
public class PdfController : ControllerBase
{
    [HttpPost(Name = "CreatePdf")]
    public async Task<IActionResult> Create()
    {
        // TODO: Create PDF and send call to Storage
        return Ok();
    }
}
