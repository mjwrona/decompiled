// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata.CargoMetadataDocumentOpApplierFactory
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Operations;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata
{
  public class CargoMetadataDocumentOpApplierFactory : 
    IMetadataDocumentOpApplierFactory<MetadataDocument<ICargoMetadataEntry>>
  {
    private readonly Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<ICargoMetadataEntry>>> applierDictionary;

    public CargoMetadataDocumentOpApplierFactory(ITracerService tracerService)
    {
      AddSourceChainOpApplier<CargoPackageIdentity, ICargoMetadataEntry> sourceChainOpApplier = new AddSourceChainOpApplier<CargoPackageIdentity, ICargoMetadataEntry>();
      ReverseVersionComparer<CargoPackageVersion> reverseVersionComparer = new ReverseVersionComparer<CargoPackageVersion>();
      this.applierDictionary = new Dictionary<Type, IMetadataDocumentOpApplier<MetadataDocument<ICargoMetadataEntry>>>()
      {
        {
          typeof (UpdateUpstreamMetadataOperationData<CargoPackageIdentity, CargoPackageName, CargoPackageVersion, ICargoMetadataEntry>),
          (IMetadataDocumentOpApplier<MetadataDocument<ICargoMetadataEntry>>) new UpdateUpstreamMetadataOpApplier<CargoPackageIdentity, CargoPackageName, CargoPackageVersion, ICargoMetadataEntry, ICargoMetadataEntryWriteable>((IComparer<IPackageVersion>) reverseVersionComparer, (IEqualityComparer<IPackageVersion>) reverseVersionComparer, tracerService)
        },
        {
          typeof (CargoAddOperationData),
          (IMetadataDocumentOpApplier<MetadataDocument<ICargoMetadataEntry>>) sourceChainOpApplier
        }
      };
    }

    public IMetadataDocumentOpApplier<MetadataDocument<ICargoMetadataEntry>>? Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataDocumentOpApplier<MetadataDocument<ICargoMetadataEntry>> documentOpApplier;
      if (this.applierDictionary.TryGetValue(operationData.GetType(), out documentOpApplier))
        return documentOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataDocumentOpApplier<MetadataDocument<ICargoMetadataEntry>>) null;
    }
  }
}
