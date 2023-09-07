using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.ProjectSearch.Models.Api;

/// <summary>
/// Контроллер проектов.
/// </summary>
[Route("api/find")]
//[Route("[area]/repository")]
[ApiController]
public class RepositoryController : BaseServiceController
{
    readonly RepositoryDbContext _context;
    readonly IMapper _mapper;

    /// <summary>
    /// Инициализировать новый экземпляр <see cref="RepositoryController"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="mapper"></param>
    public RepositoryController(
        RepositoryDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Получить историю запросов.
    /// </summary>
    /// <returns>Коллекция <see cref="RepositoryViewModel"/>.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RepositoryViewModel>>> GetAll()
    {
        var result = (await _context.Requests
                .AsNoTracking()
                .Select(x => x.Result)
                .ToListAsync())
            .SelectMany(x => x.Items);
        return Ok(_mapper.Map<IEnumerable<RepositoryViewModel>>(result));
    }

    /// <summary>
    /// Получить репозитории по фильтру.
    /// </summary>
    /// <param name="value">Фильтр.</param>
    /// <returns>Коллекция <see cref="RepositoryViewModel"/>.</returns>
    [HttpPost]
    //[HttpPost("filter")]
    public async Task<ActionResult<IEnumerable<RepositoryViewModel>>> Filter([FromBody] RepositoryFilterViewModel value)
    {
        if (!value.AllowEmpty && string.IsNullOrEmpty(value.TextRequest))
            return BadRequest(new ErrorResponseViewModel("Repository name must contain at least one character."));

        IEnumerable<RepositoryViewModel> result;

        if (!value.AllowEmpty && !await _context.Requests.AnyAsync())
            return NotFound(new ErrorResponseViewModel("No data."));

        var allRequests = await _context.Requests
            .AsNoTracking()
            .ToListAsync();

        var queryResult = allRequests
            .Where(x => x.RequestText.ToLower().Contains(value.TextRequest.ToLower()))
            .SelectMany(x => x.Result.Items)
            .ToList();

        if (queryResult.Count > 0)
        {
            result = _mapper.Map<IEnumerable<RepositoryViewModel>>(queryResult);
            return Ok(result);
        }

        var repositoryResult = allRequests
            .SelectMany(x => x.Result.Items)
            .Where(x => x.Name.ToLower().Contains(value.TextRequest.ToLower()))
            .ToList();

        if (repositoryResult.Count > 0)
        {
            result = _mapper.Map<IEnumerable<RepositoryViewModel>>(repositoryResult);
            return Ok(result);
        }

        if (value.AllowEmpty)
            return Array.Empty<RepositoryViewModel>();

        return NotFound(new ErrorResponseViewModel("Nothing was found for your query."));
    }

    /// <summary>
    /// Удалить запрос по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор запроса.</param>
    [HttpDelete("{id}")]
    //[HttpDelete("delete/{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var request = await _context.Requests.FirstOrDefaultAsync(x => x.Id == id);
        if (request == null)
            return NotFound(new ErrorResponseViewModel($"Request with ID {id} not found."));

        _context.Requests.Remove(request);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}