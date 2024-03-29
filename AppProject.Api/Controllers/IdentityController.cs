using AppProject.IService.Identities;
using AppProject.Model.Entities.Identities;
using Microsoft.AspNetCore.Authorization;
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
}