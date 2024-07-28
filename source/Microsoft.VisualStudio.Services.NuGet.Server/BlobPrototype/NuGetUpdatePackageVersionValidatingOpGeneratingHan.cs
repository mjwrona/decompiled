// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetUpdatePackageVersionValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetUpdatePackageVersionValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData, INuGetMetadataService>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetUpdatePackageVersionValidatingOpGeneratingHandlerBootstrapper(
      IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
    }

    protected override IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData> Bootstrap(
      INuGetMetadataService metadataAccessor)
    {
      IAsyncHandler<IPackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry> pointQueryHandler = metadataAccessor.ToPointQueryHandler<VssNuGetPackageIdentity, INuGetMetadataEntry>();
      ViewsValidatingConverter<VssNuGetPackageIdentity, PackageVersionDetails> requestToOpConverter = new ViewsValidatingConverter<VssNuGetPackageIdentity, PackageVersionDetails>((IConverter<JsonPatchOperation, string>) new ViewPatchToViewIdOrNameValidatingConverter(), (IConverter<FeedViewRequest, FeedView>) new ViewIdResolver((IFeedService) new FeedServiceFacade(this.requestContext)));
      NuGetListingOperationFromUpdateRequestValidatingConverter listRequestToOpConverter = new NuGetListingOperationFromUpdateRequestValidatingConverter((IConverter<IPackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>) new ListingOperationDataGeneratingConverter());
      ListValidatingHandler<PackageVersionDetails, VssNuGetPackageIdentity, INuGetMetadataEntry> validatingHandler = new ListValidatingHandler<PackageVersionDetails, VssNuGetPackageIdentity, INuGetMetadataEntry>(pointQueryHandler, (IConverter<IPackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, IListingStateChangeOperationData>) listRequestToOpConverter);
      IConverter<IListingStateChangeOperationData, ICommitOperationData> handler = ConvertFrom.OutputTypeOf<IListingStateChangeOperationData>((IHaveOutputType<IListingStateChangeOperationData>) validatingHandler).By<IListingStateChangeOperationData, ICommitOperationData>((Func<IListingStateChangeOperationData, ICommitOperationData>) (op => (ICommitOperationData) op));
      return UntilNonNullHandler.Create<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData>((IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new PromoteValidatingHandler<VssNuGetPackageIdentity, PackageVersionDetails, INuGetMetadataEntry>((IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, INuGetMetadataEntry>) pointQueryHandler, (IConverter<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, IViewOperationData>) requestToOpConverter), (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData>) validatingHandler.ThenDelegateTo<IPackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, IListingStateChangeOperationData, ICommitOperationData>(handler), (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData>) new ThrowWithBadInputMessageHandler<PackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, ICommitOperationData>((IEnumerable<string>) new string[2]
      {
        "Listed",
        "Views"
      }));
    }
  }
}
