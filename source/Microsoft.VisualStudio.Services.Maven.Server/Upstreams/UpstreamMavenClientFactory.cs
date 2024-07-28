// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.UpstreamMavenClientFactory
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Client.Internal;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class UpstreamMavenClientFactory : IFactory<UpstreamSource, Task<IUpstreamMavenClient>>
  {
    private static readonly IConverter<Stream, IList<string>> metadataConverter = (IConverter<Stream, IList<string>>) new MavenXmlMetadataToVersionListConverter();
    private readonly IVssRequestContext requestContext;
    private readonly IFeedService feedServiceFacade;
    private readonly IMavenPackageVersionServiceFacade packageVersionService;
    private readonly IHttpClient httpClient;
    private readonly ICrossCollectionClientCreator clientCreator;
    private readonly IAsyncHandler<UpstreamSource> upstreamSourceValidatingHandler;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IAsyncHandler<IPackageNameRequest<MavenPackageName>, IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>> versionsToProvideDownstreamsHandler;
    private readonly GetAuthenticationTokenFromServiceEndpointHandler serviceEndpointAuthenticationTokenHandler;

    public UpstreamMavenClientFactory(
      IVssRequestContext requestContext,
      IHttpClient httpClient,
      IFeedService feedServiceFacade,
      ICrossCollectionClientCreator clientCreator,
      IMavenPackageVersionServiceFacade packageVersionService,
      IAsyncHandler<UpstreamSource> upstreamSourceValidatingHandler,
      IExecutionEnvironment executionEnvironment,
      IAsyncHandler<IPackageNameRequest<MavenPackageName>, IReadOnlyList<VersionWithSourceChain<MavenPackageVersion>>> versionsToProvideDownstreamsHandler,
      GetAuthenticationTokenFromServiceEndpointHandler serviceEndpointAuthenticationTokenHandler)
    {
      this.requestContext = requestContext;
      this.feedServiceFacade = feedServiceFacade;
      this.packageVersionService = packageVersionService;
      this.httpClient = httpClient;
      this.clientCreator = clientCreator;
      this.upstreamSourceValidatingHandler = upstreamSourceValidatingHandler;
      this.executionEnvironment = executionEnvironment;
      this.versionsToProvideDownstreamsHandler = versionsToProvideDownstreamsHandler;
      this.serviceEndpointAuthenticationTokenHandler = serviceEndpointAuthenticationTokenHandler;
    }

    public async Task<IUpstreamMavenClient> Get(UpstreamSource upstreamSource)
    {
      ArgumentUtility.CheckForNull<UpstreamSource>(upstreamSource, nameof (upstreamSource));
      switch (upstreamSource.UpstreamSourceType)
      {
        case UpstreamSourceType.Public:
          return (IUpstreamMavenClient) new PublicUpstreamMavenClient(new Uri(upstreamSource.Location), this.httpClient, UpstreamMavenClientFactory.metadataConverter, this.IsJitPackClient(upstreamSource));
        case UpstreamSourceType.Internal:
          return await this.GetClientForInternalUpstream(upstreamSource);
        default:
          throw new NotSupportedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamSourceTypeNotSupported((object) upstreamSource.Id, (object) upstreamSource.UpstreamSourceType));
      }
    }

    private async Task<IUpstreamMavenClient> GetClientForInternalUpstream(
      UpstreamSource upstreamSource)
    {
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamCollectionId, "InternalUpstreamCollectionId");
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamFeedId, "InternalUpstreamFeedId");
      ArgumentUtility.CheckForNull<Guid>(upstreamSource.InternalUpstreamViewId, "InternalUpstreamViewId");
      Guid upstreamCollectionId = upstreamSource.InternalUpstreamCollectionId.Value;
      if (upstreamCollectionId == this.requestContext.ServiceHost.InstanceId)
        return (IUpstreamMavenClient) new SameCollectionUpstreamMavenClient(this.feedServiceFacade, this.packageVersionService, upstreamSource, UpstreamMavenClientFactory.metadataConverter, this.versionsToProvideDownstreamsHandler);
      NullResult nullResult = await this.upstreamSourceValidatingHandler.Handle(upstreamSource);
      Guid? nullable = upstreamSource.ServiceEndpointId;
      InternalMavenHttpClient clientAsync;
      if (nullable.HasValue)
      {
        VssCredentials authenticationTokenCredentials = this.serviceEndpointAuthenticationTokenHandler.Handle(upstreamSource);
        ICrossCollectionClientCreator clientCreator = this.clientCreator;
        nullable = upstreamSource.InternalUpstreamCollectionId;
        Guid hostId = nullable.Value;
        Guid serviceIdentifier = PackagingServerConstants.ServiceIdentifier;
        Func<Uri, InternalMavenHttpClient> makeClientWithCredentials = (Func<Uri, InternalMavenHttpClient>) (baseUri => new InternalMavenHttpClient(baseUri, authenticationTokenCredentials));
        CancellationToken cancellationToken = new CancellationToken();
        clientAsync = await clientCreator.CreateClientAsync<InternalMavenHttpClient>(hostId, serviceIdentifier, makeClientWithCredentials, cancellationToken);
      }
      else
        clientAsync = await this.clientCreator.CreateClientAsync<InternalMavenHttpClient>(upstreamCollectionId, PackagingServerConstants.ServiceIdentifier, true);
      Guid aadTenantId = this.executionEnvironment.IsOrganizationAadBacked() ? this.executionEnvironment.GetOrganizationAadTenantId() : Guid.Empty;
      return (IUpstreamMavenClient) new CrossCollectionUpstreamMavenClient(upstreamSource.GetProjectId(), upstreamSource.GetFullyQualifiedFeedId(), aadTenantId, clientAsync);
    }

    private bool IsJitPackClient(UpstreamSource upstreamSource) => WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(upstreamSource.Location) == WellKnownSources.MavenJitPackRepo;
  }
}
