using AppProject.IService.Modules;
using AppProject.Model;
using AppProject.Model.Entities.ApplicationModules;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.Api.Controllers;

[ApiController]
[Route("api/modules")]
public class ModuleController:ControllerBase
{
    private readonly IModuleService _moduleService;

    public ModuleController(IModuleService moduleService)
    {
        _moduleService = moduleService??throw new ArgumentNullException(nameof(moduleService));
    }

    [HttpGet]
    public async Task<MessageModel<List<ApplicationModule>>> Get()
    {
        var modules = await _moduleService.GetListAsync(null);
        return MessageModel<List<ApplicationModule>>.Success("成功",modules);
    }
}