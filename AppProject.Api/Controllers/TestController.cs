using AppProject.Model;
using AppProject.Repository.Base;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.Api.Controllers;
[ApiController]
[Route("test")]
public class TestController:ControllerBase
{
    private readonly IBaseRepository<TestModel> _repository;

    public TestController(IBaseRepository<TestModel> repository)
    {
        _repository = repository??throw new ArgumentNullException($"{typeof(IBaseRepository<TestModel>).FullName}");
    }


    [HttpGet]
    public async Task<TestModel> GetAsync()
    {
        return await _repository.GetAsync(t => true);
    }

    [HttpPost("/insert")]
    public async Task<int> InsertAsync([FromBody] TestModel entity)
    {
        await _repository.InsertAsync(entity);
        return 1;
    }
}