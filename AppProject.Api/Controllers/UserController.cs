using AppProject.Common.Constants;
using AppProject.Model;
using AppProject.Model.Entities.Identities;
using AppProject.Model.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.Api.Controllers;

[ApiController]
[Route("api/user")]
[Authorize(PolicyConstant.ADMIN)]
public class UserController:ControllerBase
{

    private UserManager<ApplicationUser> _userManager;
    private RoleManager<ApplicationRole> _roleManager;

    public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("create")]
    public async Task<MessageModel> CreateUser([FromBody] ApplicationUser user)
    {
        var u = await _userManager.FindByNameAsync(user.UserName);
        if (u != null)
        {
            return ResponseModel.Failed(msg: "存在相同用户名");
        }

        await _userManager.CreateAsync(user);
        return ResponseModel.Success();
    }

    [HttpPost("AddUserToRole")]
    public async Task AddUserToRole(string userName, string roleName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        await _userManager.AddToRoleAsync(user, roleName);
    }
}