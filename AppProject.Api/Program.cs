using AppProject.Common.Core;
using AppProject.Common.Helpers;
using AppProject.Extensions.ServiceExtensions;
using AppProject.Extensions.ServiceExtensions.Authentications;
using AppProject.IService.Identities;
using AppProject.Model.Entities.Identities;
using AppProject.Repository;
using AppProject.Repository.Context;
using AppProject.Services.Identities;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

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
builder.Services.AddAllOptionRegister();

builder.Services.AddCacheSetup();   //缓存服务注入

builder.Host.AddSerilogSetup();

builder.Services.AddIdentityExtension();
builder.Services.AddAuthenticationJwtSetup();

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

app.UseHttpsRedirection();

app.MapControllers();

app.Run();