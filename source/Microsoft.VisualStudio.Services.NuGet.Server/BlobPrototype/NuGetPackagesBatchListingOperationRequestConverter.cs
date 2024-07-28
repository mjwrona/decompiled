// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetPackagesBatchListingOperationRequestConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetPackagesBatchListingOperationRequestConverter : 
    IConverter<PackagesBatchRequest<VssNuGetPackageIdentity>, PackagesBatchRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>>,
    IHaveInputType<PackagesBatchRequest<VssNuGetPackageIdentity>>,
    IHaveOutputType<PackagesBatchRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>>
  {
    public PackagesBatchRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData> Convert(
      PackagesBatchRequest<VssNuGetPackageIdentity> input)
    {
      if (!(input.BatchOperationData is BatchListData batchOperationData))
        return (PackagesBatchRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>) null;
      ListingDirection direction = batchOperationData.Listed ? ListingDirection.Relist : ListingDirection.Delist;
      return new PackagesBatchRequest<VssNuGetPackageIdentity, ListingOperationRequestAdditionalData>(input, new ListingOperationRequestAdditionalData(direction));
    }
  }
}
