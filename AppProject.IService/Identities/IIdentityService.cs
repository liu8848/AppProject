using AppProject.Model.Entities.Identities;

namespace AppProject.IService.Identities;

public interface IIdentityService
{
    Task<long> CreateUserAsync(string UserName, string password);
    Task<bool> ValidateUserAsync(UserForAuthentication userForAuthentication);
    Task<ApplicationToken> CreateTokenAsync(bool populateExpiry);
    Task<ApplicationToken> RefreshTokenAsync(ApplicationToken token);
}