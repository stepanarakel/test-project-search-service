using Microsoft.EntityFrameworkCore;
using static AppConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
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
    opt.UseNpgsql(connectionString, options => options.UseNodaTime())
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

/////////
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "Api"
        });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        corsPolicyBuilder => corsPolicyBuilder
            .SetIsOriginAllowed(isOriginAllowed: _ => true)
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod());
});
/////////

builder.Services.AddControllers();
builder.Services.AddRazorPages();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Api"));

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
/////////////
app.UseCors("AllowAll");
/////////////
//app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(e => e.MapControllers());

app.Run();