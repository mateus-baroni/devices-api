using Microsoft.AspNetCore.Mvc;

namespace Devices.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Devices API is running");
    }
}