using AppProject.IService.Roles;
using AppProject.Model;
using AppProject.Model.Entities.Identities;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.Api.Controllers;

[ApiController]
[Route("api/role")]
public class RoleController:ControllerBase
{
    private readonly IRoleService _roleService;


    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("create")]
    public async Task<MessageModel<bool>> Create([FromBody] ApplicationRole role)
    {
        var success = await _roleService.CreateAsync(role);
        return MessageModel<bool>.Success(success.ToString());
    }

    [HttpPost("delete")]
    public async Task<MessageModel<bool>> Delete([FromBody] string roleName)
    {
        var success = await _roleService.DeleteAsync(roleName);
        return MessageModel<bool>.Success(success.ToString());
    }
    
}