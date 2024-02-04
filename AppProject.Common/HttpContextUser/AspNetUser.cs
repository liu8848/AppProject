using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AppProject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AppProject.Common.HttpContextUser;

public class AspNetUser:IUser
{
    private readonly IHttpContextAccessor _accessor;

    public AspNetUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor??throw new ArgumentNullException(nameof(accessor));
    }
    

    public string Name { get; }
    public long ID { get; }
    public long TenantId { get; }


    private string GetName()
    {
        if (IsAuthenticated()&&string.IsNullOrEmpty(_accessor.HttpContext.User.Identity.Name))
        {
            return _accessor.HttpContext.User.Identity.Name;
        }
        else
        {
            return GetUserInfoFromToken("name").FirstOrDefault();
        }
    }

    public bool IsAuthenticated()
    {
        return _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
    

    public string GetToken()
    {
        var token = _accessor.HttpContext?.Request?.Headers["Authorization"].ToString()
            .Replace("Bearer", "");
        if (!token.IsNullOrEmpty())
        {
            return token;
        }

        return token;
    }

    public List<string> GetUserInfoFromToken(string ClaimType)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var token = string.Empty;
        token = GetToken();
        
        //token 校验
        if (token.IsNullOrEmpty() && jwtHandler.CanReadToken(token))
        {
            var jwtToken = jwtHandler.ReadJwtToken(token);

            return (from item in jwtToken.Claims
                where item.Type == ClaimType
                select item.Value).ToList();
        }

        return new List<string>();
    }

    public MessageModel<string> MessageModel { get; set; }
    
    public IEnumerable<Claim> GetClaimsIdentity()
    {
        if (_accessor.HttpContext == null) return ArraySegment<Claim>.Empty;

        if (!IsAuthenticated()) return GetClaimsIdentity(GetToken());

        var claims = _accessor.HttpContext.User.Claims.ToList();
        var headers = _accessor.HttpContext.Request.Headers;
        foreach (var header in headers)
        {
            claims.Add(new Claim(header.Key, header.Value));
        }

        return claims;
    }

    public IEnumerable<Claim> GetClaimsIdentity(string token)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        // token校验
        if (token.IsNullOrEmpty() && jwtHandler.CanReadToken(token))
        {
            var jwtToken = jwtHandler.ReadJwtToken(token);

            return jwtToken.Claims;
        }

        return new List<Claim>();
    }

    public List<string> GetClaimValueByType(string ClaimType)
    {
        return (from item in GetClaimsIdentity()
            where item.Type == ClaimType
            select item.Value).ToList();
    }
}