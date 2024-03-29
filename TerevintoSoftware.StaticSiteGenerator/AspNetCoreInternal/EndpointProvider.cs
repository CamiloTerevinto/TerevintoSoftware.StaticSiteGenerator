﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;

internal interface IEndpointProvider
{
    Endpoint Endpoint { get; }
    IEndpointAddressScheme<RouteValuesAddress> EndpointAddressScheme { get; }
}

/// <summary>
/// This class is used to create an endpoint to be able to parse routes in the Razor code. 
/// <para>
/// This is needed because the app generated lacks the route information, so the DefaultLinkGenerator fails to generate links.
/// </para>
/// </summary>
[ExcludeFromCodeCoverage]
internal class EndpointProvider : IEndpointProvider, IAsyncDisposable
{
    private readonly StaticSiteGenerationOptions _siteGenerationOptions;
    private WebApplication _app = default!;

    public Endpoint Endpoint { get; }
    public IEndpointAddressScheme<RouteValuesAddress> EndpointAddressScheme { get; }

    public EndpointProvider(StaticSiteGenerationOptions siteGenerationOptions, Assembly baseAssembly)
    {
        _siteGenerationOptions = siteGenerationOptions;
        (Endpoint, EndpointAddressScheme) = Initialize(baseAssembly);
    }

    private (Endpoint, IEndpointAddressScheme<RouteValuesAddress>) Initialize(Assembly baseAssembly)
    {
        // NOTE: I create a new web application here as I could not think of a better way of replacing the EndpointAddressScheme
        // from the IServiceProvider, which means we need the built app and the app being built at the same time.
        // This is mainly because ASP.NET Core uses A LOT of internals for building endpoints, and the only easy way I could find to do this 
        // is by getting the IEndpointRouteBuilder configured by the MapControllerRoute call

        var builder = WebApplication.CreateBuilder();
        builder.Services
            .AddControllersWithViews()
            .AddApplicationPart(baseAssembly);

        _app = builder.Build();
        _app.MapControllerRoute("default", _siteGenerationOptions.DefaultRoutePattern);

        IEndpointRouteBuilder endpointRouteBuilder = _app;
        var endpointDataSource = endpointRouteBuilder.DataSources.First();

        // The last endpoint in the data source refers to the templated "default" route.
        var endpoint = endpointDataSource.Endpoints[endpointDataSource.Endpoints.Count - 1];

        // NOTE: This is a hack to get the endpoint address scheme using the endpoint built above.
        // The endpoint address scheme in the main app does not have any value for the EndpointDataSource, so without this links cannot be generated.
        var routeValueAddressSchemeType = Type.GetType("Microsoft.AspNetCore.Routing.RouteValuesAddressScheme, Microsoft.AspNetCore.Routing")!;
        var addressScheme = (IEndpointAddressScheme<RouteValuesAddress>)Activator.CreateInstance(routeValueAddressSchemeType, endpointDataSource)!;

        return (endpoint, addressScheme);
    }

    /// <summary>
    /// Replaces the default instance of RouteValuesAddressScheme on the main application 
    /// with <see cref="EndpointAddressScheme"/> built from the referenced application.
    /// </summary>
    /// <param name="services"></param>
    internal void Inject(IServiceCollection services)
    {
        services.RemoveAll<IEndpointAddressScheme<RouteValuesAddress>>();
        services.AddSingleton(EndpointAddressScheme);
    }

    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
    }
}
