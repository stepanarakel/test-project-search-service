using Microsoft.EntityFrameworkCore;

public class RepositoryDbContext : DbContext
{
    public RepositoryDbContext(DbContextOptions<RepositoryDbContext> options) : base(options) { }

    public DbSet<Request> Requests { get; set; }
}