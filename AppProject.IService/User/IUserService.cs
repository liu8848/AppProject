using AppProject.Model.Entities.Identities;

namespace AppProject.IService.User;

public interface IUserService
{
    Task<ApplicationUser> SaveUserInfo(UserForAuthentication userForAuthentication);
    
    Task<string> GetUserRoleNameStr(UserForAuthentication userForAuthentication);
}