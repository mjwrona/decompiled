// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.PyPiUpstreamFetchingMetadataServiceBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
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
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class PyPiUpstreamFetchingMetadataServiceBootstrapper : 
    IBootstrapper<IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry> metadataService;
    private readonly IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion> upstreamVersionListService;

    public PyPiUpstreamFetchingMetadataServiceBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry> metadataService,
      IUpstreamVersionListService<PyPiPackageName, PyPiPackageVersion> upstreamVersionListService)
    {
      this.requestContext = requestContext;
      this.metadataService = metadataService;
      this.upstreamVersionListService = upstreamVersionListService;
    }

    public IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry> Bootstrap() => (IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>) new UpstreamFetchingMetadataService<PyPiPackageIdentity, IPyPiMetadataEntry>(this.metadataService, (IValidator<IPackageNameRequest>) new ThrowNotFoundOnInvalidPackageNameValidator(), (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(this.requestContext.GetFeatureFlagFacade()), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), PyPiUpstreamMetadataManagerExistingLocalMetadataBootstrapper.Pending, InlineRefreshAsyncInvokerWithRebootstrapping.Bootstrap(this.requestContext), UpstreamsConfigurationSha256Hasher.Bootstrap(this.requestContext), (ICancellationFacade) new CancellationFacade(this.requestContext), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), this.requestContext.GetTracerFacade(), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext), true);
  }
}
