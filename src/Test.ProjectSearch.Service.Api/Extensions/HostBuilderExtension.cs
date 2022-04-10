using Microsoft.Extensions.Options;

/// <summary>
/// Расширения для <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderExtension
{
    /// <summary>
    /// Сконфигурировать опции для работы GitHub Web Api.
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigureGitHubHttpService(this IHostBuilder hostBuilder, Action<GitHubHttpHeaderOptions> setupAction)
        => hostBuilder.ConfigureServices((builderContext, config) =>
        {
            config.Configure(setupAction);
            config.AddOptions();
            config.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GitHubHttpHeaderOptions>>().Value);
        });
}