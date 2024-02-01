using System.Security.Claims;
using System.Text;
using AppProject.Common;
using AppProject.IService.Identities;
using AppProject.Model.Entities.Identities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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
        // if (!result) throw new Exception("账号密码错误");
        return result;
    }

    /// <summary>
    /// 生成Token
    /// </summary>
    /// <param name="populateExpiry"></param>
    /// <returns></returns>
    public async Task<ApplicationToken> CreateTokenAsync(bool populateExpiry)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        return null;
    }
    


    public async Task<ApplicationToken> RefreshTokenAsync(ApplicationToken token)
    {
        throw new NotImplementedException();
    }
    
    
    /// <summary>
    /// 获取签署秘钥
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings")["SecretKey"] ??
                                         "AppProjectApiSecretKey");
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }
    
    /// <summary>
    /// 生成Claims
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name,_applicationUser.UserName)
        };
        var roles = await _userManager.GetRolesAsync(_applicationUser);
        claims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role,role)));
        return claims;
    }

}