using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.ProjectSearch.Models.Api;

/// <summary>
/// Контроллер поиска проектов.
/// </summary>
[Route("[area]/search")]
[ApiController]
public class SearchController : BaseServiceController
{
    readonly IGitHubApiHttpService _gitHubApiHttpService;
    readonly RepositoryDbContext _context;
    readonly IMapper _mapper;

    /// <summary>
    /// Инициализировать новый экземпляр <see cref="SearchController"/>.
    /// </summary>
    /// <param name="gitHubApiHttpService"></param>
    /// <param name="context"></param>
    /// <param name="mapper"></param>
    public SearchController(
        IGitHubApiHttpService gitHubApiHttpService,
        RepositoryDbContext context,
        IMapper mapper)
    {
        _gitHubApiHttpService = gitHubApiHttpService;
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Получить репозитории по имени.
    /// </summary>
    /// <param name="name">Название репозитория/проекта.</param>
    /// <param name="page">Номер страницы.</param>
    /// <param name="perPage">Количество элементов на странице.</param>
    /// <returns></returns>
    [HttpGet("repositories/{name}")]
    public async Task<ActionResult<IEnumerable<RepositoryViewModel>>> FilterRepository(
        string name,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 40)
    {
        if (string.IsNullOrEmpty(name))
            return BadRequest(new ErrorResponseViewModel("Repository name must contain at least one character."));

        IEnumerable<RepositoryViewModel> result;

        if (await _context.Requests.AnyAsync())
        {
            var allRequests = await _context.Requests
                .AsNoTracking()
                .ToListAsync();

            var queryResult = allRequests
                .Where(x => x.RequestText.ToLower().Contains(name.ToLower()))
                .SelectMany(x => x.Result.Items)
                .ToList();

            if (queryResult.Count > 0)
            {
                result = _mapper.Map<IEnumerable<RepositoryViewModel>>(queryResult);
                return Ok(result);
            }

            var repositoryResult = allRequests
                .SelectMany(x => x.Result.Items)
                .Where(x => x.Name.ToLower().Contains(name.ToLower()))
                .ToList();

            if (repositoryResult.Count > 0)
            {
                result = _mapper.Map<IEnumerable<RepositoryViewModel>>(repositoryResult);
                return Ok(result);
            }
        }

        var gitHubApiResult = await _gitHubApiHttpService.SearchProject(name);
        if (gitHubApiResult.TotalCount == 0)
            return NotFound(new ErrorResponseViewModel("Nothing was found for your query."));

        var request = new Request
        {
            RequestText = name,
            Result = _mapper.Map<SearchResult>(gitHubApiResult)
        };
        _context.Add(request);
        await _context.SaveChangesAsync();

        result = _mapper.Map<IEnumerable<RepositoryViewModel>>(gitHubApiResult.Items);
        return Ok(result);
    }
}