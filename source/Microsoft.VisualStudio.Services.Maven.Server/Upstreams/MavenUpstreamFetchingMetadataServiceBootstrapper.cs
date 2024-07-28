// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Upstreams.MavenUpstreamFetchingMetadataServiceBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.Upstreams
{
  public class MavenUpstreamFetchingMetadataServiceBootstrapper : 
    IBootstrapper<IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry> metadataService;
    private readonly IUpstreamVersionListService<MavenPackageName, MavenPackageVersion> upstreamVersionListService;

    public MavenUpstreamFetchingMetadataServiceBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry> metadataService,
      IUpstreamVersionListService<MavenPackageName, MavenPackageVersion> upstreamVersionListService)
    {
      this.requestContext = requestContext;
      this.metadataService = metadataService;
      this.upstreamVersionListService = upstreamVersionListService;
    }

    public IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry> Bootstrap() => (IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>) new UpstreamFetchingMetadataService<MavenPackageIdentity, IMavenMetadataEntry>(this.metadataService, (IValidator<IPackageNameRequest>) new ThrowNotFoundOnInvalidPackageNameValidator(), (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(this.requestContext.GetFeatureFlagFacade()), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), MavenUpstreamMetadataManagerExistingLocalMetadataBootstrapper.Pending, InlineRefreshAsyncInvokerWithRebootstrapping.Bootstrap(this.requestContext), UpstreamsConfigurationSha256Hasher.Bootstrap(this.requestContext), (ICancellationFacade) new CancellationFacade(this.requestContext), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), this.requestContext.GetTracerFacade(), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext), true);
  }
}
