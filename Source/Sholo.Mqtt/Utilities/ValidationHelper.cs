using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Sholo.Mqtt.Utilities;

[PublicAPI]
internal static class ValidationHelper
{
    private static ServiceProvider DefaultServiceProvider { get; }

    static ValidationHelper()
    {
        var serviceProvider = CreateServiceProvider(services => services.AddSingleton<IFileAbstraction, FileAbstraction>());
        DefaultServiceProvider = serviceProvider;
    }

    public static bool IsValid(object obj, [MaybeNullWhen(true)] out ValidationResult[] results)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var validationResults = new List<ValidationResult>();
        var success = IsValid(obj, validationResults);

        results = success ? null : validationResults.ToArray();
        return success;
    }

    public static bool IsValid(object obj, IList<ValidationResult> validationResults, IFileAbstraction? fileAbstraction = null)
    {
        using var serviceProvider = fileAbstraction != null
            ? CreateServiceProvider(services => services.AddSingleton(fileAbstraction))!
            : DefaultServiceProvider;

        var validationContext = new ValidationContext(obj, serviceProvider, null);

        var success = Validator.TryValidateObject(
            obj,
            validationContext,
            validationResults,
            validateAllProperties: true
        );

        return success;
    }

    private static ServiceProvider CreateServiceProvider(Action<IServiceCollection> config)
    {
        var serviceCollection = new ServiceCollection();
        config.Invoke(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        return serviceProvider!;
    }
}
