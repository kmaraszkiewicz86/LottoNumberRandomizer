using LottoNumberRandomizer.ApplicationLayer.Queries;
using LottoNumberRandomizer.Infrastructure.Services;
using LottoNumberRandomizer.Model.Configuration;
using LottoNumberRandomizer.Model.Queries;
using LottoNumberRandomizer.Presentation.ViewModels;
using LottoNumberRandomizer.Presentation.Views;
using Microsoft.Extensions.Configuration;
using SimpleCqrs;

namespace LottoNumberRandomizer.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddLottoNumberRandomizer(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind LottoApiSettings
        var lottoApiSettings = configuration.GetSection(LottoApiSettings.SectionName).Get<LottoApiSettings>() 
            ?? throw new InvalidOperationException("LottoApi configuration section is missing");
        
        services.Configure<LottoApiSettings>(configuration.GetSection(LottoApiSettings.SectionName));
        
        // Register HttpClient with LottoNumberService
        services.AddHttpClient<ILottoNumberService, LottoNumberService>(client =>
        {
            client.BaseAddress = new Uri(lottoApiSettings.BaseUrl);
            client.DefaultRequestHeaders.Add("secret", lottoApiSettings.ApiSecret);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        
        // Register SimpleCqrs
        services.ConfigureSimpleCqrs(typeof(GetLottoNumbersQueryHandler).Assembly);
        
        // Register ViewModels
        services.AddTransient<LottoNumbersViewModel>();
        
        // Register Views
        services.AddTransient<LottoNumbersPage>();
        
        return services;
    }
}
