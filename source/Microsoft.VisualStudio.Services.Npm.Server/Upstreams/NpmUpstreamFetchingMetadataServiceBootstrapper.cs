// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmUpstreamFetchingMetadataServiceBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
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
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmUpstreamFetchingMetadataServiceBootstrapper : 
    IBootstrapper<IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> metadataService;
    private readonly IUpstreamVersionListService<NpmPackageName, SemanticVersion> upstreamVersionListService;

    public NpmUpstreamFetchingMetadataServiceBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> metadataService,
      IUpstreamVersionListService<NpmPackageName, SemanticVersion> upstreamVersionListService)
    {
      this.requestContext = requestContext;
      this.metadataService = metadataService;
      this.upstreamVersionListService = upstreamVersionListService;
    }

    public IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry> Bootstrap() => (IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>) new UpstreamFetchingMetadataService<NpmPackageIdentity, INpmMetadataEntry>(this.metadataService, (IValidator<IPackageNameRequest>) new ThrowNotFoundOnInvalidPackageNameValidator(), (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(this.requestContext.GetFeatureFlagFacade()), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), NpmUpstreamMetadataManagerExistingLocalMetadataBootstrapper.Pending, InlineRefreshAsyncInvokerWithRebootstrapping.Bootstrap(this.requestContext), UpstreamsConfigurationSha256Hasher.Bootstrap(this.requestContext), (ICancellationFacade) new CancellationFacade(this.requestContext), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), this.requestContext.GetTracerFacade(), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext), true);
  }
}
