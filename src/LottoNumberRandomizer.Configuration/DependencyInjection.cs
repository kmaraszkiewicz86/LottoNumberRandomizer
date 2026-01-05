using LottoNumberRandomizer.Infrastructure.Services;
using LottoNumberRandomizer.Model.Queries;
using LottoNumberRandomizer.Presentation.ViewModels;
using LottoNumberRandomizer.Presentation.Views;
using SimpleCqrs;

namespace LottoNumberRandomizer.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddLottoNumberRandomizer(this IServiceCollection services)
    {
        // Register HttpClient
        services.AddHttpClient();
        
        // Register Infrastructure services
        services.AddScoped<ILottoNumberService, LottoNumberService>();
        
        // Register SimpleCqrs
        services.ConfigureSimpleCqrs(typeof(GetLottoNumbersQuery).Assembly);
        
        // Register ViewModels
        services.AddTransient<LottoNumbersViewModel>();
        
        // Register Views
        services.AddTransient<LottoNumbersPage>();
        
        return services;
    }
}
