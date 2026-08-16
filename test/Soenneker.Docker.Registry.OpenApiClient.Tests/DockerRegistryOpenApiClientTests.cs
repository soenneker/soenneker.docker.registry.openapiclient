using Soenneker.Tests.HostedUnit;

namespace Soenneker.Docker.Registry.OpenApiClient.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class DockerRegistryOpenApiClientTests : HostedUnitTest
{
    public DockerRegistryOpenApiClientTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
