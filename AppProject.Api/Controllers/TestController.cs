using System.Linq.Expressions;
using AppProject.IService.Test;
using AppProject.Model;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.Api.Controllers;
[ApiController]
[Route("test")]
public class TestController:ControllerBase
{
    private readonly ITestModelService _testModelService;

    public TestController(ITestModelService testModelService)
    {
        _testModelService = testModelService;
    }


    [HttpGet("{id}")]
    public async Task<TestModel?> GetById([FromRoute]int id)
    {
        return await _testModelService.GetAsync(t => t.id == id);
    }

    [HttpGet]
    public async Task<List<TestModel>> GetListByQuery([FromQuery] int id, [FromQuery] string desc)
    {
        return await _testModelService.GetListAsync(t =>
            (t.id == id || id == 0) &&
            (t.Desc.Contains(desc) || string.IsNullOrEmpty(desc)));
    }
    
    
    
}