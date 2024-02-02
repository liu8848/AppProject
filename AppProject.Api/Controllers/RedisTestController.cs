using AppProject.Common.Extensions.Redis;
using Microsoft.AspNetCore.Mvc;


namespace AppProject.Api.Controllers;

[ApiController]
[Route("redis")]
public class RedisTestController : ControllerBase
{
    private readonly IRedisBasketRepository _redis;

    public RedisTestController(IRedisBasketRepository redis)
    {
        _redis = redis;
    }


    [HttpPost("add")]
    public async Task<IActionResult> AddRedisKey()
    {
        await _redis.Set("test", "hello", TimeSpan.FromMinutes(5));
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetRedisKey([FromQuery] string key)
    {
        var s = await _redis.GetValueAsync(key);
        return Ok(s);
    }
}