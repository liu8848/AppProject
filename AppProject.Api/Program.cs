using AppProject.Common.Core;
using AppProject.Common.Helpers;
using AppProject.Extensions.ServiceExtensions;
using AppProject.Extensions.ServiceExtensions.Authentications;
using AppProject.Extensions.ServiceExtensions.Authorizations;
using AppProject.Repository;
using Autofac;
using Autofac.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

#region Host与容器配置

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        hostingContext.Configuration.ConfigureApplication();
        config.Sources.Clear();
        config.AddJsonFile("appsettings.json", optional:true, reloadOnChange:true);
    })
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule(new AutofacModuleRegister());
    });
builder.ConfigureApplication();

#endregion

#region 服务配置

builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddAllOptionRegister();    //配置文件注入

builder.Services.AddCacheSetup();   //缓存服务注入

builder.Host.AddSerilogSetup();     //日志配置

builder.Services.AddIdentityExtension();    //权限管理注入
builder.Services.AddHttpContextSetUp();     //请求处理相关类注入

builder.Services.AddAuthenticationJwtSetup();       //JWT注入
builder.Services.AddAuthorizationSetup();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.MigrateDatabase();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();