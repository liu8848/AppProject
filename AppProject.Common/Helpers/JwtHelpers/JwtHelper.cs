using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AppProject.Common.Helpers.DateTimeHelpers;
using AppProject.Common.Option;
using Microsoft.IdentityModel.Tokens;

namespace AppProject.Common.Helpers.JwtHelpers;

public class JwtHelper
{
    /// <summary>
    /// 颁发JWT字符串
    /// </summary>
    /// <param name="tokenModel"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GenerateToken(TokenModelJwt tokenModel)
    {
        var jwtSettings = App.GetOptionsMonitor<JwtSettingsOptions>()??
                          throw new ArgumentNullException(nameof(JwtSettingsOptions));

        var iss = jwtSettings.ValidIssuer;
        var aud = jwtSettings.ValidAudience;
        var secret = jwtSettings.SecretKey;
        var expires = jwtSettings.Expires;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,tokenModel.UserName),
            new (JwtRegisteredClaimNames.Iat,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),  //签发时间
            new (JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),  //生效时间
            new (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(expires)).ToUnixTimeSeconds()}"),    //过期时间
            new (ClaimTypes.Expiration,DateTime.Now.AddMinutes(expires).ToString()),
            new (JwtRegisteredClaimNames.Iss,iss),
            new (JwtRegisteredClaimNames.Aud,aud)
        };

        claims.AddRange(tokenModel.Roles.Select(s=>new Claim(ClaimTypes.Role,s)));
        
        //秘钥
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer:iss,
            claims:claims,
            signingCredentials:credentials
        );
        var jwtHandler = new JwtSecurityTokenHandler();
        var encodedJwt = jwtHandler.WriteToken(jwt);

        return encodedJwt;
    }


    /// <summary>
    /// 生成RefreshToken
    /// </summary>
    /// <returns></returns>
    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    public static TokenModelJwt SerializeJwt(string jwtStr)
    {
        TokenModelJwt tokenModel=new TokenModelJwt();
        var tokenHandler = new JwtSecurityTokenHandler();

        if (string.IsNullOrEmpty(jwtStr) && tokenHandler.CanReadToken(jwtStr))
        {
            var jwtToken = tokenHandler.ReadJwtToken(jwtStr);
            object? userName;
            jwtToken.Payload.TryGetValue(ClaimTypes.Name, out userName);
            tokenModel = new TokenModelJwt
            {
                UserName = userName is not null ? userName.ToString() : ""
            };
        }

        return tokenModel;
    }

    public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var options = App.GetOptionsMonitor<JwtSettingsOptions>()??
                      throw new ArgumentNullException(nameof(JwtSettingsOptions));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
            ValidateLifetime = true,
            ValidIssuer = options.ValidIssuer,
            ValidAudience = options.ValidAudience
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

}


public class TokenModelJwt
{
    public string UserName { get; set; }
    
    public List<string> Roles { get; set; }
}