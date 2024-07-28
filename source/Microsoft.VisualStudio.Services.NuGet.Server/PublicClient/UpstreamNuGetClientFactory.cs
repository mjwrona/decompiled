// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicClient.UpstreamNuGetClientFactory
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.UpstreamClient;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicClient
{
  public class UpstreamNuGetClientFactory : IFactory<UpstreamSource, Task<IUpstreamNuGetClient>>
  {
    private readonly IHttpClient httpClient;
    private readonly IHttpClient httpNonForwardingClient;
    private readonly IVssRequestContext requestContext;
    private readonly IFeedService feedService;
    private readonly ICrossCollectionClientCreator clientCreator;
    private readonly IAsyncHandler<UpstreamSource> upstreamSourceValidatingHandler;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly GetAuthenticationTokenFromServiceEndpointHandler serviceEndpointAuthenticationTokenHandler;
    private readonly IVersionCountsFromFileProvider versionCountsFromFileProvider;
    private readonly IPublicRepositoryProvider<IUpstreamNuGetClient> publicRepositoryProvider;
    private readonly IOrgLevelPackagingSetting<bool> usePublicRepositorySetting;

    public UpstreamNuGetClientFactory(
      IHttpClient httpClient,
      IHttpClient httpNonForwardingClient,
      IVssRequestContext requestContext,
      IFeedService feedService,
      ICrossCollectionClientCreator clientCreator,
      IAsyncHandler<UpstreamSource> upstreamSourceValidatingHandler,
      IExecutionEnvironment executionEnvironment,
      GetAuthenticationTokenFromServiceEndpointHandler serviceEndpointAuthenticationTokenHandler,
      IVersionCountsFromFileProvider versionCountsFromFileProvider,
      IPublicRepositoryProvider<IUpstreamNuGetClient> publicRepositoryProvider,
      IOrgLevelPackagingSetting<bool> usePublicRepositorySetting)
    {
      this.httpClient = httpClient;
      this.httpNonForwardingClient = httpNonForwardingClient;
      this.requestContext = requestContext;
      this.feedService = feedService;
      this.clientCreator = clientCreator;
      this.upstreamSourceValidatingHandler = upstreamSourceValidatingHandler;
      this.executionEnvironment = executionEnvironment;
      this.serviceEndpointAuthenticationTokenHandler = serviceEndpointAuthenticationTokenHandler;
      this.versionCountsFromFileProvider = versionCountsFromFileProvider;
      this.publicRepositoryProvider = publicRepositoryProvider;
      this.usePublicRepositorySetting = usePublicRepositorySetting;
    }

    public async Task<IUpstreamNuGetClient> Get(UpstreamSource upstreamSource)
    {
      switch (upstreamSource.UpstreamSourceType)
      {
        case UpstreamSourceType.Public:
          return this.GetClientForPublicUpstream(upstreamSource);
        case UpstreamSourceType.Internal:
          return await this.GetClientForInternalUpstream(upstreamSource);
        default:
          throw new NotSupportedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamSourceTypeNotSupported((object) upstreamSource.Id, (object) upstreamSource.UpstreamSourceType));
      }
    }

    private IUpstreamNuGetClient GetClientForPublicUpstream(UpstreamSource upstreamSource)
    {
      if (this.usePublicRepositorySetting.Get())
      {
        WellKnownUpstreamSource knownSourceOrDefault = WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(upstreamSource);
        if ((object) knownSourceOrDefault == null)
          throw new InvalidOperationException("NuGet supports only well-known external sources, but source with location " + upstreamSource.Location + " does not match.");
        if (knownSourceOrDefault == WellKnownSources.NugetOrg)
          return (this.publicRepositoryProvider.GetRepositoryForSourceOrDefault(knownSourceOrDefault) ?? throw new InvalidOperationException("Could not find public repository for well-known source " + knownSourceOrDefault.DisplayName + " (" + knownSourceOrDefault.LocationUriString + ")")).GetProxyClient((CollectionId) this.executionEnvironment.HostId);
      }
      return upstreamSource.Location.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? (IUpstreamNuGetClient) new PublicNuGetHttpClient(new Uri(upstreamSource.Location), this.httpClient, (IPublicServiceIndexService) new PublicServiceIndexFacade(this.requestContext), this.versionCountsFromFileProvider, this.requestContext.GetTracerFacade(), (ICancellationFacade) new CancellationFacade(this.requestContext)) : (IUpstreamNuGetClient) new PublicNuGetV2HttpClient(new Uri(upstreamSource.Location), this.httpClient, this.versionCountsFromFileProvider, this.httpNonForwardingClient);
    }

    private async Task<IUpstreamNuGetClient> GetClientForInternalUpstream(
      UpstreamSource upstreamSource)
    {
      ArgumentUtility.CheckForNull<UpstreamSource>(upstreamSource, nameof (upstreamSource));
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamCollectionId, "InternalUpstreamCollectionId");
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamFeedId, "InternalUpstreamFeedId");
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamViewId, "InternalUpstreamViewId");
      Guid? upstreamCollectionId = upstreamSource.InternalUpstreamCollectionId;
      Guid instanceId = this.requestContext.ServiceHost.InstanceId;
      return (upstreamCollectionId.HasValue ? (upstreamCollectionId.HasValue ? (upstreamCollectionId.GetValueOrDefault() == instanceId ? 1 : 0) : 1) : 0) != 0 ? (IUpstreamNuGetClient) new SameCollectionUpstreamClient(this.requestContext, this.feedService, upstreamSource) : await this.GetClientForExternalCollection(upstreamSource);
    }

    private async Task<IUpstreamNuGetClient> GetClientForExternalCollection(
      UpstreamSource upstreamSource)
    {
      NullResult nullResult = await this.upstreamSourceValidatingHandler.Handle(upstreamSource);
      InternalNuGetHttpClient elevatedInternalNuGetHttpClient = await this.clientCreator.CreateClientAsync<InternalNuGetHttpClient>(upstreamSource.InternalUpstreamCollectionId.Value, PackagingServerConstants.ServiceIdentifier, true);
      InternalNuGetHttpClient internalNuGetHttpClient = elevatedInternalNuGetHttpClient;
      Guid? nullable = upstreamSource.ServiceEndpointId;
      FeedHttpClient clientAsync;
      if (nullable.HasValue)
      {
        VssCredentials authenticationTokenCredentials = this.serviceEndpointAuthenticationTokenHandler.Handle(upstreamSource);
        ICrossCollectionClientCreator clientCreator1 = this.clientCreator;
        nullable = upstreamSource.InternalUpstreamCollectionId;
        Guid hostId1 = nullable.Value;
        Guid serviceIdentifier1 = PackagingServerConstants.ServiceIdentifier;
        Func<Uri, InternalNuGetHttpClient> makeClientWithCredentials1 = (Func<Uri, InternalNuGetHttpClient>) (baseUri => new InternalNuGetHttpClient(baseUri, authenticationTokenCredentials));
        CancellationToken cancellationToken1 = new CancellationToken();
        internalNuGetHttpClient = await clientCreator1.CreateClientAsync<InternalNuGetHttpClient>(hostId1, serviceIdentifier1, makeClientWithCredentials1, cancellationToken1);
        ICrossCollectionClientCreator clientCreator2 = this.clientCreator;
        nullable = upstreamSource.InternalUpstreamCollectionId;
        Guid hostId2 = nullable.Value;
        Guid serviceIdentifier2 = Guid.Parse("00000036-0000-8888-8000-000000000000");
        Func<Uri, FeedHttpClient> makeClientWithCredentials2 = (Func<Uri, FeedHttpClient>) (baseUri => new FeedHttpClient(baseUri, authenticationTokenCredentials));
        CancellationToken cancellationToken2 = new CancellationToken();
        clientAsync = await clientCreator2.CreateClientAsync<FeedHttpClient>(hostId2, serviceIdentifier2, makeClientWithCredentials2, cancellationToken2);
      }
      else
      {
        ICrossCollectionClientCreator clientCreator = this.clientCreator;
        nullable = upstreamSource.InternalUpstreamCollectionId;
        Guid hostId = nullable.Value;
        Guid serviceIdentifier = Guid.Parse("00000036-0000-8888-8000-000000000000");
        CancellationToken cancellationToken = new CancellationToken();
        clientAsync = await clientCreator.CreateClientAsync<FeedHttpClient>(hostId, serviceIdentifier, true, cancellationToken);
      }
      Guid guid = this.executionEnvironment.IsOrganizationAadBacked() ? this.executionEnvironment.GetOrganizationAadTenantId() : Guid.Empty;
      Guid? projectId = upstreamSource.GetProjectId();
      string fullyQualifiedFeedId = upstreamSource.GetFullyQualifiedFeedId();
      nullable = upstreamSource.InternalUpstreamViewId;
      Guid viewId = nullable.Value;
      Guid aadTenantId = guid;
      InternalNuGetHttpClient nugetHttpClient = internalNuGetHttpClient;
      InternalNuGetHttpClient elevatedNuGetHttpClient = elevatedInternalNuGetHttpClient;
      FeedHttpClient feedHttpClient = clientAsync;
      IVersionCountsFromFileProvider fromFileProvider = this.versionCountsFromFileProvider;
      IUpstreamNuGetClient externalCollection = (IUpstreamNuGetClient) new CrossCollectionUpstreamNuGetClient(projectId, fullyQualifiedFeedId, viewId, aadTenantId, nugetHttpClient, elevatedNuGetHttpClient, feedHttpClient, fromFileProvider);
      elevatedInternalNuGetHttpClient = (InternalNuGetHttpClient) null;
      internalNuGetHttpClient = (InternalNuGetHttpClient) null;
      return externalCollection;
    }
  }
}
