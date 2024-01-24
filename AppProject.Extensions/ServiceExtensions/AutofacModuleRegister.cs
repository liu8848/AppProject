using System.Reflection;
using AppProject.Common;
using AppProject.IService.Base;
using AppProject.Repository.Base;
using AppProject.Repository.Context;
using AppProject.Services.Base;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Module = Autofac.Module;

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
        builder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>)).InstancePerDependency();

        // 获取 Service.dll 程序集服务，并注册
        var assemblysServices = Assembly.LoadFrom(servicesDllFile);
        builder.RegisterAssemblyTypes(assemblysServices)
            .AsImplementedInterfaces()
            .InstancePerDependency()
            .PropertiesAutowired();

        #endregion
    }
}