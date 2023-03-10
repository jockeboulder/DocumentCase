using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace StorageService.Docker.Controllers;

[ApiController]
[Route("[controller]")]
public class StorageController : ControllerBase 
{
    private const string filePath = ".";

    [Route("Store")]
    [HttpPost]
    public async Task<IActionResult> Store([FromQuery] string fileName, [FromQuery] string extension, [FromBody] string file)
    {
        byte[] pdf = Convert.FromBase64String(file);
        // TODO: Store document on Azure Storage Account? Maybe on disk for now...
        using FileStream stream = System.IO.File.Create($"{filePath}/{fileName}.{extension}");
        stream.Write(pdf, 0, file.Length);
        return Ok();
    }
}
