[![](https://img.shields.io/nuget/v/soenneker.docker.registry.openapiclient.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.docker.registry.openapiclient/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.docker.registry.openapiclient/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.docker.registry.openapiclient/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.docker.registry.openapiclient.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.docker.registry.openapiclient/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.docker.registry.openapiclient/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.docker.registry.openapiclient/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Docker.Registry.OpenApiClient

A Kiota-generated .NET client for the Docker Registry HTTP API V2.

## Installation

```bash
dotnet add package Soenneker.Docker.Registry.OpenApiClient
```

## Create a client

```csharp
using System.Net.Http.Headers;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Docker.Registry.OpenApiClient;

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", registryToken);

var adapter = new HttpClientRequestAdapter(
    new AnonymousAuthenticationProvider(),
    httpClient: httpClient)
{
    BaseUrl = "https://registry-1.docker.io"
};

var client = new DockerRegistryOpenApiClient(adapter);
```

The bearer token must already have the required repository scope. This client does not process `WWW-Authenticate` challenges or exchange Docker credentials for a registry token. Keep credentials outside source control and reuse the transport and generated client.

The companion `Soenneker.Docker.Registry.OpenApiClientUtil` package provides dependency-injection registration and cached construction.

## Read a manifest

```csharp
using Soenneker.Docker.Registry.OpenApiClient.Models;

GetImageManifest200DockerDistributionManifestV2JsonResponse? manifest =
    await client.V2["library/alpine"]
                .Manifests["latest"]
                .GetAsync(cancellationToken: cancellationToken);

string? mediaType = manifest?.MediaType;
int layerCount = manifest?.Layers?.Count ?? 0;
```

The first indexer is the repository name; the manifest indexer accepts a tag or digest. Manifest GET requests advertise Docker distribution manifest schema v2. Registries returning only OCI indexes, manifest lists, or another media type may not fit this generated response model.

## Important API behavior

- Manifest deletion should use a digest, not a mutable tag, and requires delete scope.
- Blob `HeadAsync` exposes the generated response model for existence checks.
- Blob `GetAsync` currently returns no body from the generated contract, so it is not suitable for downloading layer content. Use a raw `HttpClient` request when the response stream is required.
- The generated operations do not define typed error mappings. Non-success responses surface through Kiota’s general request/transport exceptions rather than Registry-specific error models.

The client and models are generated. Avoid editing them directly because regeneration replaces those changes. Review endpoint signatures, response media types, and model changes before upgrading production consumers.
