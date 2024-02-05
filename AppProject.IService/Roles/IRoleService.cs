using AppProject.Model.Entities.Identities;

namespace AppProject.IService.Roles;

public interface IRoleService
{
    Task<bool> CreateAsync(ApplicationRole role);
    Task<bool> DeleteAsync(string roleName);
}