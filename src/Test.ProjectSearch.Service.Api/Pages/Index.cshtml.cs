using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Test.ProjectSearch.Models.Api;

namespace Test.ProjectSearch.Service.Api.Pages
{
    public class IndexModel : PageModel
    {
        readonly IGitHubApiHttpService _gitHubApiHttpService;
        readonly RepositoryDbContext _context;
        readonly IMapper _mapper;

        [BindProperty(SupportsGet = true)]
        public string TextRequest { get; set; } = string.Empty;

        public IEnumerable<RepositoryViewModel> Repositories { get; set; } = Array.Empty<RepositoryViewModel>();

        public ErrorResponseViewModel Error { get; set; }

        /// <summary>
        /// »нициализировать новый экземпл€р <see cref="IndexModel"/>.
        /// </summary>
        /// <param name="gitHubApiHttpService"></param>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public IndexModel(
            IGitHubApiHttpService gitHubApiHttpService,
            RepositoryDbContext context,
            IMapper mapper)
        {
            _gitHubApiHttpService = gitHubApiHttpService;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ActionResult<IEnumerable<RepositoryViewModel>>> FilterRepository(string textRequest)
        {
            if (string.IsNullOrEmpty(textRequest))
                return BadRequest(new ErrorResponseViewModel("Repository name must contain at least one character."));

            IEnumerable<RepositoryViewModel> result;

            if (await _context.Requests.AnyAsync())
            {
                var allRequests = await _context.Requests
                    .AsNoTracking()
                    .ToListAsync();

                var queryResult = allRequests
                    .Where(x => x.RequestText.ToLower().Contains(textRequest.ToLower()))
                    .SelectMany(x => x.Result.Items)
                    .ToList();

                if (queryResult.Count > 0)
                {
                    result = _mapper.Map<IEnumerable<RepositoryViewModel>>(queryResult);
                    return new ActionResult<IEnumerable<RepositoryViewModel>>(result);
                }

                var repositoryResult = allRequests
                    .SelectMany(x => x.Result.Items)
                    .Where(x => x.Name.ToLower().Contains(textRequest.ToLower()))
                    .ToList();

                if (repositoryResult.Count > 0)
                {
                    result = _mapper.Map<IEnumerable<RepositoryViewModel>>(repositoryResult);
                    return new ActionResult<IEnumerable<RepositoryViewModel>>(result);
                }
            }

            var gitHubApiResult = await _gitHubApiHttpService.SearchProject(textRequest);
            if (gitHubApiResult.TotalCount == 0)
                return NotFound(new ErrorResponseViewModel("Nothing was found for your query."));

            var request = new Request
            {
                RequestText = textRequest,
                Result = _mapper.Map<SearchResult>(gitHubApiResult)
            };
            _context.Add(request);
            await _context.SaveChangesAsync();

            result = _mapper.Map<IEnumerable<RepositoryViewModel>>(gitHubApiResult.Items);
            return new ActionResult<IEnumerable<RepositoryViewModel>>(result);
        }

        public async Task Search(string request)
        {
            try
            {
                Repositories = (await FilterRepository(request)).Value!;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Error = new ErrorResponseViewModel(e.Message);
            }
        }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(TextRequest))
                await Search(TextRequest);
        }
    }
}
