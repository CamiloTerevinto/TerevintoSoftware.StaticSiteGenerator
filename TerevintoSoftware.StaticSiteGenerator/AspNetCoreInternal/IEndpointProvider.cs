using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;

internal interface IEndpointProvider
{
    Endpoint Endpoint { get; }
    IEndpointAddressScheme<RouteValuesAddress> EndpointAddressScheme { get; }
}
