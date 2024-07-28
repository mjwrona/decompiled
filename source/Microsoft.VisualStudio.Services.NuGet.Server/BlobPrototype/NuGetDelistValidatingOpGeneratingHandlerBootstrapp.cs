// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetDelistValidatingOpGeneratingHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetDelistValidatingOpGeneratingHandlerBootstrapper : 
    RequireAggHandlerBootstrapper<PackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData, INuGetMetadataService>
  {
    protected override IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData> Bootstrap(
      INuGetMetadataService metadataAccessor)
    {
      return (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>) new ListValidatingHandler<ListingOperationRequestAdditionalData, VssNuGetPackageIdentity, INuGetMetadataEntry>(metadataAccessor.ToPointQueryHandler<VssNuGetPackageIdentity, INuGetMetadataEntry>(), (IConverter<IPackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>) new ListingOperationDataGeneratingConverter());
    }
  }
}
