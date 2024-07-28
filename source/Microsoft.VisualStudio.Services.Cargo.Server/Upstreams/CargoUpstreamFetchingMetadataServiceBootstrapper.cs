// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Upstreams.CargoUpstreamFetchingMetadataServiceBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
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
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Upstreams
{
  public class CargoUpstreamFetchingMetadataServiceBootstrapper : 
    IBootstrapper<IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry> metadataService;
    private readonly IUpstreamVersionListService<CargoPackageName, CargoPackageVersion> upstreamVersionListService;

    public CargoUpstreamFetchingMetadataServiceBootstrapper(
      IVssRequestContext requestContext,
      IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry> metadataService,
      IUpstreamVersionListService<CargoPackageName, CargoPackageVersion> upstreamVersionListService)
    {
      this.requestContext = requestContext;
      this.metadataService = metadataService;
      this.upstreamVersionListService = upstreamVersionListService;
    }

    public IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry> Bootstrap() => (IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>) new UpstreamFetchingMetadataService<CargoPackageIdentity, ICargoMetadataEntry>(this.metadataService, (IValidator<IPackageNameRequest>) new ThrowNotFoundOnInvalidPackageNameValidator(), (IHandler<IFeedRequest, IEnumerable<UpstreamSource>>) new UpstreamsFromFeedHandler(this.requestContext.GetFeatureFlagFacade()), UpstreamEntriesValidChecker.Bootstrap(this.requestContext), CargoUpstreamMetadataManagerExistingLocalMetadataBootstrapper.Pending, InlineRefreshAsyncInvokerWithRebootstrapping.Bootstrap(this.requestContext), UpstreamsConfigurationSha256Hasher.Bootstrap(this.requestContext), (ICancellationFacade) new CancellationFacade(this.requestContext), (IFeedPerms) new FeedPermsFacade(this.requestContext), (IExecutionEnvironment) new ExecutionEnvironmentFacade(this.requestContext), this.requestContext.GetTracerFacade(), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeeds.Bootstrap(this.requestContext), FeatureAvailabilityConstants.UpstreamsAllowedForPublicFeedsMSFT.Bootstrap(this.requestContext), true);
  }
}
