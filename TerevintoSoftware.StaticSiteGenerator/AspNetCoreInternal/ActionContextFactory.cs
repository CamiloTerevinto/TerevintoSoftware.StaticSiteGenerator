using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;

internal interface IActionContextFactory
{
    ActionContext Create(string controllerName, string viewName);
}

internal class ActionContextFactory(IServiceScopeFactory serviceScopeFactory, IEndpointProvider endpointProvider) : IActionContextFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly IEndpointProvider _endpointProvider = endpointProvider;

    public ActionContext Create(string controllerName, string viewName)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = scope.ServiceProvider
        };
        httpContext.SetEndpoint(_endpointProvider.Endpoint);

        var routeData = httpContext.GetRouteData();
        routeData.Values.Add("controller", controllerName);
        routeData.Values.Add("action", viewName);

        return new ActionContext(httpContext, routeData, new ActionDescriptor());
    }
}
