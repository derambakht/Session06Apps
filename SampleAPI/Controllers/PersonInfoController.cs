using Microsoft.AspNetCore.Mvc;
using SampleAPI.Data;

[ApiController]
[Route("api/[controller]")]

public class PersonInfoController : ControllerBase
{
    public readonly SampleDB _sampleDB;
    public PersonInfoController(SampleDB sampleDB)
    {
        _sampleDB = sampleDB;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        //_sampleDB = new SampleDB();
        return Ok(_sampleDB.Persons);
    }

    [HttpGet("{code}")]
    public IActionResult Get(int code)
    {
        return Ok(new {Id = 1524, Code = code, FirstName= "Ali", LastName = "Rezaei"});
    }

    [HttpGet("GetMyProfile/{code}")]
    public IActionResult GetProfile(int code)
    {
        return Ok(new {Id = 9955, Code = code, FirstName= "Ali", LastName = "Rezaei"});
    }

    [HttpPost]
    public IActionResult Create(PersonInfo model)
    {
        // var sampleDB = new SampleDB();
        _sampleDB.Persons.Add(model);

        //SampleDB.Persons.Add(model);
        return Ok();
    }
    
}