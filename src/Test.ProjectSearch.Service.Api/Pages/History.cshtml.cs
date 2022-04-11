#nullable disable
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Test.ProjectSearch.Service.Api.Pages
{
    public class HistoryModel : PageModel
    {
        private readonly RepositoryDbContext _context;

        public HistoryModel(RepositoryDbContext context)
        {
            _context = context;
        }

        public IList<Request> Requests { get; set; }

        public async Task OnGetAsync()
        {
            Requests = await _context.Requests.ToListAsync();
        }
    }
}
