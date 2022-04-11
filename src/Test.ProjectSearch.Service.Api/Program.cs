using Microsoft.EntityFrameworkCore;
using System.Reflection;
using static AppConstants;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging();
        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddHttpClient<IGitHubApiHttpService, GitHubApiHttpService>();

        builder.Host.ConfigureGitHubHttpService(option =>
        {
            var headerList = new List<HttpHeader>();
            builder.Configuration.GetSection(GitHubHttpHeaderOptions.Position).Bind(headerList);
            foreach (var header in headerList)
                option.AddForwardHeader(header.Title, header.Value);
        });

        var connectionString = builder.Configuration[ConnectionString];
        builder.Services.AddDbContext<RepositoryDbContext>(opt =>
            opt.UseNpgsql(connectionString, options => options.UseNodaTime()));

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Api"
                });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddControllers();
        builder.Services.AddRazorPages();

        var app = builder.Build();
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseSwagger();
        app.UseSwaggerUI(options =>
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Api"));
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(e => e.MapControllers());
        app.MapRazorPages();
        app.Run();
    }
}