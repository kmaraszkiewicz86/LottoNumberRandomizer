# LottoNumberRandomizer
LottoNumberRandomizer analyzes historical lottery draws using a Lotto API and generates random number sets for Lotto tickets, built with .NET MAUI, CQRS, and MVVM.

## Architecture

This project follows **Domain-Driven Design (DDD)** and **CQRS** patterns with the following structure:

### Projects

- **LottoNumberRandomizer.UI** - .NET MAUI Android application (entry point)
- **LottoNumberRandomizer.Presentation** - MAUI views and ViewModels (MVVM pattern)
- **LottoNumberRandomizer.ApplicationLayer** - CQRS Query handlers using SimpleCqrs
- **LottoNumberRandomizer.Domain** - Domain entities
- **LottoNumberRandomizer.Infrastructure** - Services with HttpClientFactory
- **LottoNumberRandomizer.Model** - Data Transfer Objects (DTOs)
- **LottoNumberRandomizer.Configuration** - Dependency Injection setup

### Technologies

- **.NET 10** - Latest .NET framework
- **.NET MAUI** - Cross-platform UI framework (Android-only in this implementation)
- **SimpleCqrs** (v0.1.3) - CQRS library by kmaraszkiewicz86
- **CommunityToolkit.Mvvm** - MVVM helpers
- **HttpClientFactory** - HTTP client management

### Key Features

- Generates 10 random lottery numbers
- Displays numbers in a table with count of occurrences
- Localized UI (Number, Count columns)
- Follows CQRS pattern with `IAsyncQueryHandler`
- Uses `ISimpleMediator` for query dispatching
- Clean architecture with separation of concerns

### Building

```bash
dotnet restore
dotnet build
```

### Running

The application targets Android. You can run it using:

```bash
dotnet build -t:Run -f net10.0-android
```

