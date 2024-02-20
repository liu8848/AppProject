using System.Security.Claims;
using AppProject.Common.HttpContextUser;
using AppProject.Model;
using AppProject.Model.Entities.Identities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AppProject.Extensions.ServiceExtensions.Authorizations.Policys;

public class PermissionHandler:AuthorizationHandler<PermissionRequirement>
{
    public IAuthenticationSchemeProvider Scheme { get; set; }

    private readonly IHttpContextAccessor _accessor;
    private readonly IUser _user;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionHandler(IHttpContextAccessor accessor, IUser user, 
        RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, 
        IAuthenticationSchemeProvider scheme)
    {
        _accessor = accessor;
        _user = user;
        _roleManager = roleManager;
        _userManager = userManager;
        Scheme = scheme;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var httpContext = _accessor.HttpContext;

        // if (!requirement.Permissions.Any())
        // {
        //     var list = new List<PermissionItem>();
        //
        //     if (Permissions.IsUseIds4)
        //     {
        //         
        //     }//使用Jwt
        //     else
        //     {
        //         list=(from VAR in date )
        //     }
        // }

        if (httpContext != null)
        {
            var requestUrl = httpContext.Request.Path.Value.ToLower();
            
            httpContext.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
            {
                OriginalPath = httpContext.Request.Path,
                OriginalPathBase = httpContext.Request.PathBase
            });

            //判断是否需要远程调用，并进行调用
            var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (var scheme in await Scheme.GetRequestHandlerSchemesAsync())
            {
                if (await handlers.GetHandlerAsync(httpContext,scheme.Name) 
                    is IAuthenticationRequestHandler handler &&await handler.HandleRequestAsync())
                {
                    context.Fail();
                    return;
                }
            }

            var user = new ApplicationUser();
            var defaultAuthenticate = await Scheme.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);

                if (result?.Principal != null)
                {
                    user = await _userManager.FindByNameAsync(_user.Name);
                    if (user == null)
                    {
                        _user.MessageModel = new ApiResponse(StatusCode.CODE401, "用户不存在或已被删除").MessageModel;
                        context.Fail(new AuthorizationFailureReason(this, _user.MessageModel.msg));
                        return;
                    }
                }


                //验证过期时间
                bool isExp = false;
                isExp = (httpContext.User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) != null
                        && DateTime.Parse(httpContext.User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.Expiration)
                            ?.Value)
                        >= DateTime.Now;
                if (!isExp)
                {
                    context.Fail(new AuthorizationFailureReason(this, "授权已过期,请重新授权"));
                    return;
                }

                //验证签发时间
                // var value = httpContext.User.Claims
                //     .FirstOrDefault(s => s.Type == JwtRegisteredClaimNames.Iat)?.Value;
                // if (value != null)
                // {
                //     if (user. > value.ObjToDate())
                //     {
                //         _user.MessageModel = new ApiResponse(StatusCode.CODE401, "很抱歉,授权已失效,请重新授权")
                //             .MessageModel;
                //         context.Fail(new AuthorizationFailureReason(this, _user.MessageModel.msg));
                //         return;
                //     }
                // }
                
                
                context.Succeed(requirement);
                return;
            }
            
            //判断没有登录时，是否访问登录的url,并且是Post请求，并且是form表单提交类型，否则为失败
            if (!(requestUrl.Equals(requirement.LoginPath.ToLower(), StringComparison.Ordinal) &&
                  (!httpContext.Request.Method.Equals("POST") || !httpContext.Request.HasFormContentType)))
            {
                context.Fail();
                return;
            }

        }
    }
}