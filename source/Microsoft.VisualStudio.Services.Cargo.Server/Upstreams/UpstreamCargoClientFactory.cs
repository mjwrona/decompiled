// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.UpstreamCargoClientFactory
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Client.Internal;
using Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.InternalUpstreamClient;
using Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.PublicUpstreamClient;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class UpstreamCargoClientFactory : IFactory<UpstreamSource, Task<IUpstreamCargoClient>>
  {
    private readonly IHttpClient httpClient;
    private readonly FeedServiceFacade feedServiceFacade;
    private readonly IVssRequestContext requestContext;
    private readonly ICrossCollectionClientCreator clientCreator;
    private readonly IAsyncHandler<UpstreamSource> upstreamSourceValidatingHandler;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly GetAuthenticationTokenFromServiceEndpointHandler serviceEndpointAuthenticationTokenHandler;
    private readonly ICargoDashFixer cargoDashFixer;

    public UpstreamCargoClientFactory(
      IHttpClient httpClient,
      IVssRequestContext requestContext,
      FeedServiceFacade feedServiceFacade,
      ICrossCollectionClientCreator clientCreator,
      IAsyncHandler<UpstreamSource> upstreamSourceValidatingHandler,
      IExecutionEnvironment executionEnvironment,
      GetAuthenticationTokenFromServiceEndpointHandler serviceEndpointAuthenticationTokenHandler,
      ICargoDashFixer cargoDashFixer)
    {
      this.httpClient = httpClient;
      this.requestContext = requestContext;
      this.feedServiceFacade = feedServiceFacade;
      this.clientCreator = clientCreator;
      this.upstreamSourceValidatingHandler = upstreamSourceValidatingHandler;
      this.executionEnvironment = executionEnvironment;
      this.serviceEndpointAuthenticationTokenHandler = serviceEndpointAuthenticationTokenHandler;
      this.cargoDashFixer = cargoDashFixer;
    }

    public async Task<IUpstreamCargoClient> Get(UpstreamSource upstreamSource)
    {
      switch (upstreamSource.UpstreamSourceType)
      {
        case UpstreamSourceType.Public:
          return (IUpstreamCargoClient) new PublicUpstreamCargoClient(new Uri(upstreamSource.Location), this.httpClient, this.cargoDashFixer);
        case UpstreamSourceType.Internal:
          return await this.GetClientForInternalUpstream(upstreamSource);
        default:
          throw new NotSupportedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamSourceTypeNotSupported((object) upstreamSource.Id, (object) upstreamSource.UpstreamSourceType));
      }
    }

    private async Task<IUpstreamCargoClient> GetClientForInternalUpstream(
      UpstreamSource upstreamSource)
    {
      ArgumentUtility.CheckForNull<UpstreamSource>(upstreamSource, nameof (upstreamSource));
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamCollectionId, "InternalUpstreamCollectionId");
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamFeedId, "InternalUpstreamFeedId");
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamViewId, "InternalUpstreamViewId");
      Guid? upstreamCollectionId = upstreamSource.InternalUpstreamCollectionId;
      Guid instanceId = this.requestContext.ServiceHost.InstanceId;
      return (upstreamCollectionId.HasValue ? (upstreamCollectionId.HasValue ? (upstreamCollectionId.GetValueOrDefault() == instanceId ? 1 : 0) : 1) : 0) != 0 ? (IUpstreamCargoClient) new SameCollectionUpstreamCargoClient(this.requestContext, this.feedServiceFacade, upstreamSource, this.httpClient) : await this.GetClientForExternalCollection(upstreamSource);
    }

    private async Task<IUpstreamCargoClient> GetClientForExternalCollection(
      UpstreamSource upstreamSource)
    {
      NullResult nullResult = await this.upstreamSourceValidatingHandler.Handle(upstreamSource);
      Guid? nullable = upstreamSource.ServiceEndpointId;
      InternalCargoHttpClient clientAsync;
      if (nullable.HasValue)
      {
        VssCredentials authenticationTokenCredentials = this.serviceEndpointAuthenticationTokenHandler.Handle(upstreamSource);
        ICrossCollectionClientCreator clientCreator = this.clientCreator;
        nullable = upstreamSource.InternalUpstreamCollectionId;
        Guid hostId = nullable.Value;
        Guid serviceIdentifier = PackagingServerConstants.ServiceIdentifier;
        Func<Uri, InternalCargoHttpClient> makeClientWithCredentials = (Func<Uri, InternalCargoHttpClient>) (baseUri => new InternalCargoHttpClient(baseUri, authenticationTokenCredentials));
        CancellationToken cancellationToken = new CancellationToken();
        clientAsync = await clientCreator.CreateClientAsync<InternalCargoHttpClient>(hostId, serviceIdentifier, makeClientWithCredentials, cancellationToken);
      }
      else
      {
        ICrossCollectionClientCreator clientCreator = this.clientCreator;
        nullable = upstreamSource.InternalUpstreamCollectionId;
        Guid hostId = nullable.Value;
        Guid serviceIdentifier = PackagingServerConstants.ServiceIdentifier;
        CancellationToken cancellationToken = new CancellationToken();
        clientAsync = await clientCreator.CreateClientAsync<InternalCargoHttpClient>(hostId, serviceIdentifier, true, cancellationToken);
      }
      Guid aadTenantId = this.executionEnvironment.IsOrganizationAadBacked() ? this.executionEnvironment.GetOrganizationAadTenantId() : Guid.Empty;
      return (IUpstreamCargoClient) new CrossCollectionUpstreamCargoClient(upstreamSource.GetProjectId(), upstreamSource.GetFullyQualifiedFeedId(), aadTenantId, clientAsync);
    }
  }
}
