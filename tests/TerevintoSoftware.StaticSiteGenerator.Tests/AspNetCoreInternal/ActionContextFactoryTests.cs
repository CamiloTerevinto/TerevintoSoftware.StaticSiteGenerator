using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TerevintoSoftware.StaticSiteGenerator.AspNetCoreInternal;

namespace TerevintoSoftware.StaticSiteGenerator.Tests.AspNetCoreInternal;

internal class ActionContextFactoryTests
{
    private readonly Mock<IEndpointProvider> _endpointProviderMock = new();
    private readonly IServiceProvider _serviceProvider = new ServiceCollection().BuildServiceProvider();

    [Test]
    public void Create_ShouldCreateAnActionContext()
    {
        var factory = new ActionContextFactory(_serviceProvider.GetRequiredService<IServiceScopeFactory>(), _endpointProviderMock.Object);

        var actionContext = factory.Create("test", "test");

        Assert.That(actionContext, Is.InstanceOf<ActionContext>());
    }
}
