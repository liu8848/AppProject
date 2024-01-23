using AppProject.Common;
using AppProject.Repository.Base;
using AppProject.Repository.Context;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace AppProject.Extensions.ServiceExtensions;

public class AutofacModuleRegister : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var basePath = AppContext.BaseDirectory;

        #region 注册数据库连接

        var connectionString = App.Configuration.GetConnectionString("Default");
        builder.RegisterType<AppProjectDbContext>()
            .WithParameter("options",
                new DbContextOptionsBuilder()
                    .UseMySql(connectionString,
                        MySqlServerVersion.LatestSupportedServerVersion)
                    // .LogTo(Log.Logger.Information)
                    .Options)
            .SingleInstance();

        #endregion

        #region 带接口层的服务注入

        var servicesDllFile = Path.Combine(basePath, "AppProject.Services.dll");
        var repositoryDllFile = Path.Combine(basePath, "AppProject.Repository.dll");

        if (!(File.Exists(servicesDllFile) && File.Exists(repositoryDllFile)))
        {
            var msg = "Repository.dll和Service.dll丢失";
            throw new Exception(msg);
        }

        //注册仓储
        builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();

        #endregion
    }
}