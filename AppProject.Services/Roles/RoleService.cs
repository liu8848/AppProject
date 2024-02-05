using AppProject.IService.Roles;
using AppProject.Model.Entities.Identities;
using Microsoft.AspNetCore.Identity;

namespace AppProject.Services.Roles;

public class RoleService:IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager??throw new ArgumentNullException(nameof(roleManager));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<bool> CreateAsync(ApplicationRole role)
    {
        var isExist = await _roleManager.RoleExistsAsync(role.Name);
        if (isExist)
        {
            throw new Exception($"角色名：{role.Name}  已存在！");
        }

        role.ConcurrencyStamp = Guid.NewGuid().ToString();
        
        var result = await _roleManager.CreateAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName)??
                   throw new ArgumentNullException($"角色：{roleName} 不存在");
        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }
}