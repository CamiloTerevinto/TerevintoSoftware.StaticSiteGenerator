using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;

internal interface IActionContextFactory
{
    ActionContext Create();
}

internal class ActionContextFactory : IActionContextFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IEndpointProvider _endpointProvider;

    public ActionContextFactory(IServiceScopeFactory serviceScopeFactory, IEndpointProvider endpointProvider)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _endpointProvider = endpointProvider;
    }

    public ActionContext Create()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = scope.ServiceProvider
        };
        httpContext.SetEndpoint(_endpointProvider.Endpoint);

        var routeData = httpContext.GetRouteData();

        return new ActionContext(httpContext, routeData, new ActionDescriptor());
    }
}
