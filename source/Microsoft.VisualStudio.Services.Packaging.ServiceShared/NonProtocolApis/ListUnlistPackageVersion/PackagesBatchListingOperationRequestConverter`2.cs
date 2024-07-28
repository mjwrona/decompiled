// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion.PackagesBatchListingOperationRequestConverter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion
{
  public class PackagesBatchListingOperationRequestConverter<TPackageId, TListData> : 
    IConverter<PackagesBatchRequest<TPackageId>, PackagesBatchRequest<TPackageId, ListingOperationRequestAdditionalData>>,
    IHaveInputType<PackagesBatchRequest<TPackageId>>,
    IHaveOutputType<PackagesBatchRequest<TPackageId, ListingOperationRequestAdditionalData>>
    where TPackageId : IPackageIdentity
    where TListData : class
  {
    public PackagesBatchRequest<TPackageId, ListingOperationRequestAdditionalData> Convert(
      PackagesBatchRequest<TPackageId> input)
    {
      if (!(input.BatchOperationData is BatchListData batchOperationData))
        throw new InvalidOperationException("Invalid batch operation for List.");
      ListingDirection direction = batchOperationData.Listed ? ListingDirection.Relist : ListingDirection.Delist;
      return new PackagesBatchRequest<TPackageId, ListingOperationRequestAdditionalData>(input, new ListingOperationRequestAdditionalData(direction));
    }
  }
}
