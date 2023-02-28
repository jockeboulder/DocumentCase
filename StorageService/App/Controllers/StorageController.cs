using Microsoft.AspNetCore.Mvc;

namespace StorageService.Docker.Controllers;

[ApiController]
[Route("[controller]")]
public class StorageController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<StorageController> _logger;

    public StorageController(ILogger<StorageController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "StoreFile")]
    public async Task<IActionResult> Store()
    {
        // TODO: Store document on Azure Storage Account? Maybe on disk for now...
        return Ok();
    }
}
