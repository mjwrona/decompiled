// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.UpstreamPyPiClientFactory
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.PyPi.Client.Internal;
using Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.InternalUpstreamClient;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class UpstreamPyPiClientFactory : IFactory<UpstreamSource, Task<IUpstreamPyPiClient>>
  {
    private readonly IHttpClient httpClient;
    private readonly FeedServiceFacade feedServiceFacade;
    private readonly IVssRequestContext requestContext;
    private readonly ICrossCollectionClientCreator clientCreator;
    private readonly IAsyncHandler<UpstreamSource> upstreamSourceValidatingHandler;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly GetAuthenticationTokenFromServiceEndpointHandler serviceEndpointAuthenticationTokenHandler;
    private readonly IPublicRepositoryProvider<IUpstreamPyPiClient> publicRepositoryProvider;
    private readonly IOrgLevelPackagingSetting<bool> usePublicRepositorySetting;

    public UpstreamPyPiClientFactory(
      IHttpClient httpClient,
      IVssRequestContext requestContext,
      FeedServiceFacade feedServiceFacade,
      ICrossCollectionClientCreator clientCreator,
      IAsyncHandler<UpstreamSource> upstreamSourceValidatingHandler,
      IExecutionEnvironment executionEnvironment,
      GetAuthenticationTokenFromServiceEndpointHandler serviceEndpointAuthenticationTokenHandler,
      IPublicRepositoryProvider<IUpstreamPyPiClient> publicRepositoryProvider,
      IOrgLevelPackagingSetting<bool> usePublicRepositorySetting)
    {
      this.httpClient = httpClient;
      this.requestContext = requestContext;
      this.feedServiceFacade = feedServiceFacade;
      this.clientCreator = clientCreator;
      this.upstreamSourceValidatingHandler = upstreamSourceValidatingHandler;
      this.executionEnvironment = executionEnvironment;
      this.serviceEndpointAuthenticationTokenHandler = serviceEndpointAuthenticationTokenHandler;
      this.publicRepositoryProvider = publicRepositoryProvider;
      this.usePublicRepositorySetting = usePublicRepositorySetting;
    }

    public async Task<IUpstreamPyPiClient> Get(UpstreamSource upstreamSource)
    {
      switch (upstreamSource.UpstreamSourceType)
      {
        case UpstreamSourceType.Public:
          WellKnownUpstreamSource knownSourceOrDefault = WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(upstreamSource);
          if ((object) knownSourceOrDefault == null || !this.usePublicRepositorySetting.Get())
            return (IUpstreamPyPiClient) new PublicUpstreamPyPiClient(new Uri(upstreamSource.Location), this.httpClient, (IConverter<PyPiUpstreamJsonMetadataPackageFileMetadataRequest, IReadOnlyDictionary<string, string[]>>) new PyPiUpstreamJsonMetadataToIngestionMetadataConverter(), (IConverter<string, PyPiPackageRegistrationState>) new PyPiDotOrgNameLevelJsonToLimitedMetadatasConverter());
          return (this.publicRepositoryProvider.GetRepositoryForSourceOrDefault(knownSourceOrDefault) ?? throw new InvalidOperationException("Could not find public repository for well-known source " + knownSourceOrDefault.DisplayName + " (" + knownSourceOrDefault.LocationUriString + ")")).GetProxyClient((CollectionId) this.executionEnvironment.HostId);
        case UpstreamSourceType.Internal:
          return await this.GetClientForInternalUpstream(upstreamSource);
        default:
          throw new NotSupportedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamSourceTypeNotSupported((object) upstreamSource.Id, (object) upstreamSource.UpstreamSourceType));
      }
    }

    private async Task<IUpstreamPyPiClient> GetClientForInternalUpstream(
      UpstreamSource upstreamSource)
    {
      ArgumentUtility.CheckForNull<UpstreamSource>(upstreamSource, nameof (upstreamSource));
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamCollectionId, "InternalUpstreamCollectionId");
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamFeedId, "InternalUpstreamFeedId");
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamViewId, "InternalUpstreamViewId");
      Guid? upstreamCollectionId = upstreamSource.InternalUpstreamCollectionId;
      Guid instanceId = this.requestContext.ServiceHost.InstanceId;
      return (upstreamCollectionId.HasValue ? (upstreamCollectionId.HasValue ? (upstreamCollectionId.GetValueOrDefault() == instanceId ? 1 : 0) : 1) : 0) != 0 ? (IUpstreamPyPiClient) new SameCollectionUpstreamPyPiClient(this.requestContext, this.feedServiceFacade, upstreamSource) : await this.GetClientForExternalCollection(upstreamSource);
    }

    private async Task<IUpstreamPyPiClient> GetClientForExternalCollection(
      UpstreamSource upstreamSource)
    {
      NullResult nullResult = await this.upstreamSourceValidatingHandler.Handle(upstreamSource);
      Guid? nullable = upstreamSource.ServiceEndpointId;
      InternalPyPiHttpClient clientAsync;
      if (nullable.HasValue)
      {
        VssCredentials authenticationTokenCredentials = this.serviceEndpointAuthenticationTokenHandler.Handle(upstreamSource);
        ICrossCollectionClientCreator clientCreator = this.clientCreator;
        nullable = upstreamSource.InternalUpstreamCollectionId;
        Guid hostId = nullable.Value;
        Guid serviceIdentifier = PackagingServerConstants.ServiceIdentifier;
        Func<Uri, InternalPyPiHttpClient> makeClientWithCredentials = (Func<Uri, InternalPyPiHttpClient>) (baseUri => new InternalPyPiHttpClient(baseUri, authenticationTokenCredentials));
        CancellationToken cancellationToken = new CancellationToken();
        clientAsync = await clientCreator.CreateClientAsync<InternalPyPiHttpClient>(hostId, serviceIdentifier, makeClientWithCredentials, cancellationToken);
      }
      else
      {
        ICrossCollectionClientCreator clientCreator = this.clientCreator;
        nullable = upstreamSource.InternalUpstreamCollectionId;
        Guid hostId = nullable.Value;
        Guid serviceIdentifier = PackagingServerConstants.ServiceIdentifier;
        CancellationToken cancellationToken = new CancellationToken();
        clientAsync = await clientCreator.CreateClientAsync<InternalPyPiHttpClient>(hostId, serviceIdentifier, true, cancellationToken);
      }
      Guid aadTenantId = this.executionEnvironment.IsOrganizationAadBacked() ? this.executionEnvironment.GetOrganizationAadTenantId() : Guid.Empty;
      return (IUpstreamPyPiClient) new CrossCollectionUpstreamPyPiClient(upstreamSource.GetProjectId(), upstreamSource.GetFullyQualifiedFeedId(), aadTenantId, clientAsync);
    }
  }
}
