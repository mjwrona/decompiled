// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetListingOperationFromUpdateRequestValidatingConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetListingOperationFromUpdateRequestValidatingConverter : 
    IConverter<IPackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>, IListingStateChangeOperationData>,
    IHaveInputType<IPackageRequest<VssNuGetPackageIdentity, PackageVersionDetails>>,
    IHaveOutputType<IListingStateChangeOperationData>
  {
    private readonly IConverter<IPackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData> operationDataGenerator;

    public NuGetListingOperationFromUpdateRequestValidatingConverter(
      IConverter<IPackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData> operationDataGenerator)
    {
      this.operationDataGenerator = operationDataGenerator;
    }

    public IListingStateChangeOperationData Convert(
      IPackageRequest<VssNuGetPackageIdentity, PackageVersionDetails> input)
    {
      PackageVersionDetails additionalData = input.AdditionalData;
      if ((additionalData != null ? (!additionalData.Listed.HasValue ? 1 : 0) : 1) != 0)
        return (IListingStateChangeOperationData) null;
      VssNuGetPackageIdentity packageId = input.PackageId;
      ListingDirection direction = additionalData.Listed.Value ? ListingDirection.Relist : ListingDirection.Delist;
      return this.operationDataGenerator.Convert((IPackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>) new PackageRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>((IPackageRequest<VssNuGetPackageIdentity>) new PackageRequest<VssNuGetPackageIdentity>(input.Feed, packageId), new ListingOperationRequestAdditionalData(direction)));
    }
  }
}
