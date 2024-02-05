using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AppProject.Common;
using AppProject.Common.Option;
using AppProject.Extensions.ServiceExtensions.Authorizations.Policys;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AppProject.Extensions.ServiceExtensions.Authentications;

public static class AuthenticationJwtSetup
{
    public static void AddAuthenticationJwtSetup(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        var jwtSettings = App.GetOptionsMonitor<JwtSettingsOptions>()??
                          throw new ArgumentNullException(nameof(JwtSettingsOptions));

        var secretKey = jwtSettings.SecretKey;
        var keyByteArray = Encoding.UTF8.GetBytes(secretKey);
        var signingKey = new SymmetricSecurityKey(keyByteArray);
        var issuer = jwtSettings.ValidIssuer;
        var audience = jwtSettings.ValidAudience;

        var signingCredentials = new SigningCredentials(signingKey,SecurityAlgorithms.HmacSha256);
        
        //令牌验证参数
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = true,
            ValidIssuer = issuer,//发行人
            ValidateAudience = true,
            ValidAudience = audience,//订阅人
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(60),
            RequireExpirationTime = true
        };
        
        //开启Bearer认证
        services.AddAuthentication(o =>
        {
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = nameof(ApiResponseHandler);
            o.DefaultForbidScheme = nameof(ApiResponseHandler);
        })//添加JwtBearer服务
        .AddJwtBearer(o =>
        {
            o.TokenValidationParameters = tokenValidationParameters;
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    context.Response.Headers["Token-Error"] = context.ErrorDescription;
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var jwtHandler = new JwtSecurityTokenHandler();
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    if (token.IsNullOrEmpty() && jwtHandler.CanReadToken(token))
                    {
                        var jwtToken = jwtHandler.ReadJwtToken(token);

                        if (jwtToken.Issuer != issuer)
                        {
                            context.Response.Headers["Token-Error-Iss"] = "issuer is wrong!";
                        }

                        if (jwtToken.Audiences.FirstOrDefault() != audience)
                        {
                            context.Response.Headers["Token-Error-Aud"] = "Audience is wrong!";
                        }
                    }

                    // 如果过期，则把<是否过期>添加到，返回头信息中
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers["Token-Expired"] = "true";
                    }

                    return Task.CompletedTask;
                }
            };
        })
        .AddScheme<AuthenticationSchemeOptions,ApiResponseHandler>(nameof(ApiResponseHandler), o => { });
    }
}