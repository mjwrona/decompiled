// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.UpstreamNpmClientFactory
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Client.Internal;
using Microsoft.VisualStudio.Services.Npm.Server.Registry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class UpstreamNpmClientFactory : IFactory<UpstreamSource, Task<IUpstreamNpmClient>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IHttpClient httpClient;
    private readonly ICrossCollectionClientCreator clientCreator;
    private readonly IAsyncHandler<UpstreamSource> upstreamSourceValidatingHandler;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly ITracerService tracerService;
    private readonly INpmUpstreamMetadataDocumentParser upstreamMetadataDocumentParser;
    private readonly GetAuthenticationTokenFromServiceEndpointHandler serviceEndpointAuthenticationTokenHandler;

    public UpstreamNpmClientFactory(
      IVssRequestContext requestContext,
      IHttpClient httpClient,
      ICrossCollectionClientCreator clientCreator,
      IAsyncHandler<UpstreamSource> upstreamSourceValidatingHandler,
      IExecutionEnvironment executionEnvironment,
      ITracerService tracerService,
      INpmUpstreamMetadataDocumentParser upstreamMetadataDocumentParser,
      GetAuthenticationTokenFromServiceEndpointHandler serviceEndpointAuthenticationTokenHandler)
    {
      this.requestContext = requestContext;
      this.httpClient = httpClient;
      this.clientCreator = clientCreator;
      this.upstreamSourceValidatingHandler = upstreamSourceValidatingHandler;
      this.executionEnvironment = executionEnvironment;
      this.tracerService = tracerService;
      this.upstreamMetadataDocumentParser = upstreamMetadataDocumentParser;
      this.serviceEndpointAuthenticationTokenHandler = serviceEndpointAuthenticationTokenHandler;
    }

    public async Task<IUpstreamNpmClient> Get(UpstreamSource source)
    {
      switch (source.UpstreamSourceType)
      {
        case UpstreamSourceType.Public:
          return (IUpstreamNpmClient) new PublicUpstreamNpmClient(this.httpClient, source, this.tracerService, this.upstreamMetadataDocumentParser);
        case UpstreamSourceType.Internal:
          Guid? upstreamCollectionId = source.InternalUpstreamCollectionId;
          Guid instanceId = this.requestContext.ServiceHost.InstanceId;
          if ((upstreamCollectionId.HasValue ? (upstreamCollectionId.HasValue ? (upstreamCollectionId.GetValueOrDefault() == instanceId ? 1 : 0) : 1) : 0) != 0)
            return (IUpstreamNpmClient) new SameCollectionUpstreamNpmClient(this.requestContext, source, this.tracerService, this.upstreamMetadataDocumentParser);
          NullResult nullResult = await this.upstreamSourceValidatingHandler.Handle(source);
          Guid currentAadTenantId = this.executionEnvironment.IsOrganizationAadBacked() ? this.executionEnvironment.GetOrganizationAadTenantId() : Guid.Empty;
          Guid? nullable = source.ServiceEndpointId;
          NpmApiClient clientAsync;
          if (nullable.HasValue)
          {
            VssCredentials authenticationTokenCredentials = this.serviceEndpointAuthenticationTokenHandler.Handle(source);
            ICrossCollectionClientCreator clientCreator = this.clientCreator;
            nullable = source.InternalUpstreamCollectionId;
            Guid hostId = nullable.Value;
            Guid serviceIdentifier = PackagingServerConstants.ServiceIdentifier;
            Func<Uri, NpmApiClient> makeClientWithCredentials = (Func<Uri, NpmApiClient>) (baseUri => new NpmApiClient(baseUri, authenticationTokenCredentials));
            CancellationToken cancellationToken = new CancellationToken();
            clientAsync = await clientCreator.CreateClientAsync<NpmApiClient>(hostId, serviceIdentifier, makeClientWithCredentials, cancellationToken);
          }
          else
          {
            ICrossCollectionClientCreator clientCreator = this.clientCreator;
            nullable = source.InternalUpstreamCollectionId;
            Guid hostId = nullable.Value;
            Guid serviceIdentifier = PackagingServerConstants.ServiceIdentifier;
            CancellationToken cancellationToken = new CancellationToken();
            clientAsync = await clientCreator.CreateClientAsync<NpmApiClient>(hostId, serviceIdentifier, true, cancellationToken);
          }
          return (IUpstreamNpmClient) new CrossCollectionUpstreamNpmClient(clientAsync, source, this.tracerService, currentAadTenantId, this.upstreamMetadataDocumentParser);
        default:
          throw new NotSupportedException();
      }
    }
  }
}
