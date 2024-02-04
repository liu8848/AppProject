using AppProject.IService.Identities;
using AppProject.Model.Entities.Identities;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.Api.Controllers;

[ApiController]
[Route("identity")]
public class IdentityController:ControllerBase
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("user/create")]
    public async Task<long> CreateUser([FromBody] UserForAuthentication userForAuthentication)
    {
        return await _identityService.CreateUserAsync(userForAuthentication.UserName, userForAuthentication.Password);
    }

    // [HttpPost("user/login")]
    // public async Task<IActionResult> Login([FromBody] UserForAuthentication userForAuthentication)
    // {
    //     var result = await _identityService.ValidateUserAsync(userForAuthentication);
    //     if (!result) return Unauthorized();
    //     var token = await _identityService.CreateTokenAsync(true);
    //     return Ok();
    // }
    
}