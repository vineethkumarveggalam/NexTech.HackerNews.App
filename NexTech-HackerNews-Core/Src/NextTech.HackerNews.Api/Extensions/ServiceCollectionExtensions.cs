namespace NexTech.HackerNews.Api.Extensions
{
    using Microsoft.Extensions.Options;
    using Nextech.HackerNews.Application.Interfaces;
    using Nextech.HackerNews.Application.Services;
    using Nextech.HackerNews.Infrastructure.Integrations;
    using NexTech.HackerNews.Api.Mappings;
    using NexTech.HackerNews.Infrastructure.Integrations;
    using NexTech.HackerNews.Infrastructure.Models;
    using Polly;

    /// <summary>
    /// Defines the <see cref="ServiceCollectionExtensions" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// The AddHackerNewsServices
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/></param>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/></param>
        /// <returns>The <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddHackerNewsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<HackerNewsOptions>(configuration.GetSection("HackerNews"));

            //Adding Retry with policy incase of any failures to HackerNewsAPI           
            services.AddHttpClient<HttpClientWrapper>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<HackerNewsOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            }).AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))))
              .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromMinutes(1)));

            // API Client
            services.AddScoped<IHackerNewsApiClient, HackerNewsApiClient>();

            // Story Service
            services.AddScoped<INewsStoryService, NewsStoryService>();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<StoryMapper>();
            });

            return services;
        }
    }
}
