using System.IdentityModel.Tokens.Jwt;
using AppProject.Common.Helpers.JwtHelpers;
using AppProject.IService.Identities;
using AppProject.Model;
using AppProject.Model.Entities.Identities;
using AppProject.Model.Responses;
using AppProject.Model.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.Api.Controllers;

[ApiController]
[Route("api/Login")]
public class LoginController:ControllerBase
{
    private readonly IIdentityService _identityService;

    public LoginController(IIdentityService identityService)
    {
        _identityService = identityService;
    }


    [HttpPost]
    [Route("getToken")]
    public async Task<MessageModel<ApplicationToken>> GetJwtStr(UserForAuthentication userForAuthentication)
    {

        //验证用户名与密码
        var isSuccess = await _identityService.ValidateUserAsync(userForAuthentication);
        if (!isSuccess)
        {
            throw new Exception("账户名或密码错误");
        }
        // var userInfo = await _identityService.GetUserInfo(userForAuthentication.UserName);
        //
        // var tokenModel = new TokenModelJwt
        // {
        //     UserName = userInfo.UserName
        // };
        //
        // jwtStr = JwtHelper.IssueJwt(tokenModel);

        ApplicationToken token = await _identityService.CreateTokenAsync(true);
        

        return new MessageModel<ApplicationToken>
        {
            success = isSuccess,
            msg = isSuccess ? "获取成功" : "获取失败",
            response = token
        };
    }

    [HttpPost]
    [Route("refreshToken")]
    public async Task<MessageModel<ApplicationToken>> RefreshToken([FromBody] ApplicationToken token)
    {
        var refreshToken = await _identityService.RefreshTokenAsync(token);
        return ResponseModel.Success(refreshToken);
    }
}