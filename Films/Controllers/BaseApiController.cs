using Microsoft.AspNetCore.Mvc;

namespace Films.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    protected string GetAbsoluteUri(string? path = null)
    {
        var absoluteUri = $"{Request.Scheme}://{Request.Host}{Request.Path}";
        return string.IsNullOrEmpty(path) ? absoluteUri : $"{absoluteUri}/{path}";
    }
}