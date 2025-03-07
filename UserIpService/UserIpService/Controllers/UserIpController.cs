using Microsoft.AspNetCore.Mvc;
using UserIpService.Services;

namespace UserIpService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserIpController : ControllerBase
{
    private readonly IUserIpMainService _userIpService;

    public UserIpController(IUserIpMainService userIpService)
    {
        _userIpService = userIpService;
    }

    [HttpPost("{userId}/ips")]
    public async Task<IActionResult> AddUserIp(long userId, [FromQuery] string ipAddress)
    {
        await _userIpService.AddUserIp(userId, ipAddress);
        return Ok();
    }

    [HttpGet("byip/{ipPrefix}")]
    public async Task<IActionResult> FindUsersByIpPrefix(string ipPrefix)
    {
        var users = await _userIpService.FindUsersByIpPrefix(ipPrefix);
        return Ok(users);
    }

    [HttpGet("{userId}/ips")]
    public async Task<IActionResult> GetUserIpAddresses(long userId)
    {
        var ips = await _userIpService.GetUserIpAddresses(userId);
        return Ok(ips);
    }

    [HttpGet("{userId}/lastconnection")]
    public async Task<IActionResult> GetLastUserConnection(long userId)
    {
        var lastConnection = await _userIpService.GetLastUserConnection(userId);
        return Ok(lastConnection);
    }
}