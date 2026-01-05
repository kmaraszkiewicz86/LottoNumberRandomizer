using LottoNumberRandomizer.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
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
        services.ConfigureSimpleCqrs(typeof(ApplicationLayer.Queries.GetLottoNumbersQuery).Assembly);
        
        // Register ViewModels
        services.AddTransient<Presentation.ViewModels.LottoNumbersViewModel>();
        
        // Register Views
        services.AddTransient<Presentation.Views.LottoNumbersPage>();
        
        return services;
    }
}
