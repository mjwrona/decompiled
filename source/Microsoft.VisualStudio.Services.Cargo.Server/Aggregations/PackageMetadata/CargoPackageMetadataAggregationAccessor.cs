// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata.CargoPackageMetadataAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata
{
  public class CargoPackageMetadataAggregationAccessor : 
    PackageMetadataAggregationAccessor<CargoPackageIdentity, ICargoMetadataEntry>,
    ICargoPackageMetadataAggregationAccessor,
    IAggregationAccessor,
    IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>,
    IMetadataService<CargoPackageIdentity, ICargoMetadataEntry>,
    IReadMetadataService<CargoPackageIdentity, ICargoMetadataEntry>,
    IReadSingleVersionMetadataService<CargoPackageIdentity, ICargoMetadataEntry>,
    IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>,
    IReadMetadataDocumentService
  {
    public CargoPackageMetadataAggregationAccessor(
      IAggregation aggregation,
      IConverter<IAggregation, Locator> aggVersionToLocatorConverter,
      IFactory<ContainerAddress, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>> metadataServiceFactory,
      IExecutionEnvironment executionEnvironment)
      : base(aggregation, aggVersionToLocatorConverter, metadataServiceFactory, executionEnvironment)
    {
    }
  }
}
