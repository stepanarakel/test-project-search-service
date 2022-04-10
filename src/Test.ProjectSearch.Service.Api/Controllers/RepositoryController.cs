using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Контроллер проектов.
/// </summary>
[Route("[area]/projects")]
[ApiController]
public class RepositoryController : BaseServiceController
{

    [HttpGet]
    public ActionResult<IEnumerable<object>> GetAll()
    {
        return new[] { "value1", "value2" };
    }


    [HttpGet("{id}")]
    public ActionResult<object> GetById(int id)
    {
        return "value";
    }

    [HttpPost("filter")]
    public ActionResult<IEnumerable<object>> Filter([FromBody] object value)
    {
        return Ok(new[] { 1, 0 });
    }


    [HttpPost]
    public void Post([FromBody] string value)
    {
    }


    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }


    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}