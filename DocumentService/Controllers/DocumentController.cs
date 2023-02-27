using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Docker.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid documentNumber, string customerNumber, string documentText)
    {
        return Ok();
    }
}
