using AppProject.Common;
using AppProject.Common.Helpers.JwtHelpers;
using AppProject.Common.Option;
using AppProject.IService.Identities;
using AppProject.Model.Entities.Identities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AppProject.Services.Identities;

public class IdentityService:IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private ApplicationUser? _applicationUser;
    private readonly JwtSettingsOptions _jwtSettings;

    public IdentityService(UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _jwtSettings = App.GetOptionsMonitor<JwtSettingsOptions>() ??
                       throw new ArgumentNullException(nameof(JwtSettingsOptions));
    }

    public async Task<long> CreateUserAsync(string userName, string password)
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
        return result;
    }


    public async Task<ApplicationToken> CreateTokenAsync(bool populateExpiry=true)
    {
        var tokenModel = new TokenModelJwt
        {
            UserName = _applicationUser.UserName
        };

        var roles = await _userManager.GetRolesAsync(_applicationUser);
        tokenModel.Roles = roles.ToList();

        var jwtStr = JwtHelper.GenerateToken(tokenModel);
        var refreshToken = JwtHelper.GenerateRefreshToken();
        _applicationUser.RefreshToken = refreshToken;
        if (populateExpiry)
            _applicationUser.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(_jwtSettings.Expires);
        await _userManager.UpdateAsync(_applicationUser);
        return new ApplicationToken(jwtStr,refreshToken);
    }


    public async Task<ApplicationToken> RefreshTokenAsync(ApplicationToken token)
    {
        var principal = JwtHelper.GetPrincipalFromExpiredToken(token.AccessToken);
        var user = await _userManager.FindByNameAsync(principal.Identity.Name);
        if (user == null || !user.RefreshToken.Equals(token.RefreshToken)||user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new BadHttpRequestException("无效token，请重新登录");
        }
        

        _applicationUser = user;
        return await CreateTokenAsync(true);
    }
    
}