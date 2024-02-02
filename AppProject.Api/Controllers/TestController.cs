using System.Linq.Expressions;
using AppProject.Common;
using AppProject.Common.Option;
using AppProject.IService.Test;
using AppProject.Model;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.Api.Controllers;
[ApiController]
[Route("test")]
public class TestController:ControllerBase
{
    private readonly ITestModelService _testModelService;
    private readonly TestOption _testOption;
    private readonly TestOption _testOptionMonitor;
    private readonly TestOption _testOptionSnap;

    public TestController(ITestModelService testModelService)
    {
        _testModelService = testModelService;
        _testOption = App.GetOptions<TestOption>() ?? throw new ArgumentException(nameof(TestOption));
        _testOptionMonitor = App.GetOptionsMonitor<TestOption>() ?? throw new ArgumentException(nameof(TestOption));
        _testOptionSnap = App.GetOptionsSnapshot<TestOption>() ?? throw new ArgumentException(nameof(TestOption));
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

    [HttpGet("option")]
    public string GetOptionStr()
    {
        var option1 = _testOption;
        var option2 = _testOptionMonitor;
        var option3 = _testOptionSnap;
        return $"option:{_testOption.Content}   /r/n    optionMonitor:{_testOptionMonitor.Content}";
    }
    
    
    
}