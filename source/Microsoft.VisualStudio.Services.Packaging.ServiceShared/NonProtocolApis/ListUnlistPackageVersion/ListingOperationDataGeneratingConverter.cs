// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion.ListingOperationDataGeneratingConverter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion
{
  public class ListingOperationDataGeneratingConverter : 
    IConverter<IPackageRequest<IPackageIdentity, ListingOperationRequestAdditionalData>, IListingStateChangeOperationData>,
    IHaveInputType<IPackageRequest<IPackageIdentity, ListingOperationRequestAdditionalData>>,
    IHaveOutputType<IListingStateChangeOperationData>
  {
    public IListingStateChangeOperationData Convert(
      IPackageRequest<IPackageIdentity, ListingOperationRequestAdditionalData> input)
    {
      switch (input.AdditionalData.Direction)
      {
        case ListingDirection.Delist:
          return (IListingStateChangeOperationData) new DelistOperationData(input.PackageId);
        case ListingDirection.Relist:
          return (IListingStateChangeOperationData) new RelistOperationData(input.PackageId);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
