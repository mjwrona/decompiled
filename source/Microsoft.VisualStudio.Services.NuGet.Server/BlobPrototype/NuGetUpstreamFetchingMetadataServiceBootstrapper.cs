// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetUpstreamFetchingMetadataServiceBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Upstreams;
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

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetUpstreamFetchingMetadataServiceBootstrapper : 
    IBootstrapper<IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService;
    private readonly IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService;
    private readonly bool isFromExplicitUserAction;

    public NuGetUpstreamFetchingMetadataServiceBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService,
      IUpstreamVersionListService<VssNuGetPackageName, VssNuGetPackageVersion> upstreamVersionListService,
      bool isFromExplicitUserAction)
    {
      this.requestContext = requestContext;
      this.metadataService = metadataService;
      this.upstreamVersionListService = upstreamVersionListService;
      this.isFromExplicitUserAction = isFromExplicitUserAction;
    }

    public IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry> Bootstrap() => (IReadMetadataDocumentService<VssNuGetPackageIdentity, INuGetMetadataEntry>) new UpstreamFetchingMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry>(this.metadataService, (IValidator<IPackageNameRequest>) new ThrowNotFoundOnInvalidPackageNameValidator(), (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(this.requestContext.GetFeatureFlagFacade()), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), NuGetUpstreamMetadataManagerExistingLocalMetadataBootstrapper.Pending, InlineRefreshAsyncInvokerWithRebootstrapping.Bootstrap(this.requestContext), UpstreamsConfigurationSha256Hasher.Bootstrap(this.requestContext), (ICancellationFacade) new CancellationFacade(this.requestContext), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), this.requestContext.GetTracerFacade(), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext), this.isFromExplicitUserAction);
  }
}
