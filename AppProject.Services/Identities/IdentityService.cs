using AppProject.Common;
using AppProject.IService.Identities;
using AppProject.Model.Entities.Identities;
using AppProject.Repository.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace AppProject.Services.Identities;

public class IdentityService:IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private ApplicationUser? _applicationUser;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _configuration = App.Configuration;
    }

    public async Task<string> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName
        };
        await _userManager.CreateAsync(user, password);
        return user.Id;
    }

    public async Task<bool> ValidateUserAsync(UserForAuthentication userForAuthentication)
    {
        _applicationUser = await _userManager.FindByNameAsync(userForAuthentication.UserName);

        var result = _applicationUser != null &&
                     await _userManager.CheckPasswordAsync(_applicationUser, userForAuthentication.Password);
        if (!result) throw new Exception("账号密码错误");
        return result;
    }

    public async Task<ApplicationToken> CreateTokenAsync(bool populateExpiry)
    {
        throw new NotImplementedException();
    }

    public async Task<ApplicationToken> RefreshTokenAsync(ApplicationToken token)
    {
        throw new NotImplementedException();
    }
}