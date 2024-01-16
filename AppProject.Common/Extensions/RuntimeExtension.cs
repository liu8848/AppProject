using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace AppProject.Common.Extensions;

public static class RuntimeExtension
{
    /// <summary>
    ///     获取项目程序集，排除所有系统程序集
    /// </summary>
    /// <returns></returns>
    public static IList<Assembly> GetAllAssemblies()
    {
        var list = new List<Assembly>();
        var deps = DependencyContext.Default;

        //仅加载项目中的程序集
        var libs = deps.CompileLibraries
            //排除所有系统程序集和Nuget下载包
            .Where(lib => !lib.Serviceable && lib.Type == "project");

        foreach (var lib in libs)
            try
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                list.Add(assembly);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        return list;
    }

    /// <summary>
    ///     根据程序集名获取程序集
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public static Assembly GetAssembly(string assemblyName)
    {
        return GetAllAssemblies().FirstOrDefault(assembly =>
            assembly.FullName.Contains(assemblyName));
    }


    /// <summary>
    ///     获取所有定义的类型
    /// </summary>
    /// <returns></returns>
    public static IList<Type> GetAllType()
    {
        var list = new List<Type>();
        foreach (var assembly in GetAllAssemblies())
        {
            var typeInfos = assembly.DefinedTypes;
            foreach (var typeInfo in typeInfos) list.Add(typeInfo.AsType());
        }

        return list;
    }


    /// <summary>
    ///     获取某个程序集中所定义的类型
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public static IList<Type> GetTypesByAssembly(string assemblyName)
    {
        var list = new List<Type>();
        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
        var typeInfos = assembly.DefinedTypes;
        foreach (var typeInfo in typeInfos) list.Add(typeInfo.AsType());

        return list;
    }

    /// <summary>
    ///     根据类型名和接口类型获取接口实现类
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="baseInterfaceType"></param>
    /// <returns></returns>
    public static Type? GetImplementTypes(string typeName, Type baseInterfaceType)
    {
        return GetAllType().FirstOrDefault(t =>
        {
            if (t.Name.Equals(typeName) &&
                t.GetTypeInfo().GetInterfaces().Any(b => b.Name.Equals(baseInterfaceType.Name)))
            {
                var typeInfo = t.GetTypeInfo();
                return typeInfo is { IsClass: true, IsAbstract: false, IsGenericType: false };
            }

            return false;
        });
    }
}