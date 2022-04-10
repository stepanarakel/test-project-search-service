using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Контроллер поиска проектов.
/// </summary>
[Route("[area]/search")]
[ApiController]
public class SearchController : BaseServiceController
{
    readonly IGitHubApiHttpService _gitHubApiHttpService;
    readonly RepositoryDbContext _context;

    /// <summary>
    /// Инициализировать новый экземпляр <see cref="SearchController"/>.
    /// </summary>
    /// <param name="gitHubApiHttpService"></param>
    /// <param name="context"></param>
    public SearchController(
        IGitHubApiHttpService gitHubApiHttpService,
        RepositoryDbContext context)
    {
        _gitHubApiHttpService = gitHubApiHttpService;
        _context = context;
    }

    /// <summary>
    /// Получить репозитории по имени.
    /// </summary>
    /// <param name="name">Название репозитория/проекта.</param>
    /// <returns></returns>
    [HttpGet("repositories/{name}")]
    public async Task<ActionResult<object>> FilterRepository(string name)
    {
        if (string.IsNullOrEmpty(name))
            return BadRequest(new ErrorResponseViewModel("Repository name must contain at least one character."));

        var dbResult = await _context.Requests
            .AsNoTracking()
            .SelectMany(x => x.Result.Items)
            .Where(x => x.Name.ToLower().Contains(name.ToLower()))
            .Select(x => x).ToListAsync();

        if (dbResult.Count > 0)
            return Ok(dbResult);

        var gitHubApiResult = await _gitHubApiHttpService.SearchProject(name);
        return Ok();
    }
}