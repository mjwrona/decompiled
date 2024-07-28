// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata.CargoMetadataEntryOpApplierFactory
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Operations;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata
{
  public class CargoMetadataEntryOpApplierFactory : 
    IMetadataEntryOpApplierFactory<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>
  {
    private readonly Dictionary<Type, IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>> applierDictionary;

    public CargoMetadataEntryOpApplierFactory() => this.applierDictionary = new Dictionary<Type, IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>>()
    {
      {
        typeof (CargoAddOperationData),
        (IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>) new CargoMetadataAddOpApplier()
      },
      {
        typeof (ViewOperationData),
        (IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>) new ViewMetadataOpApplier<ICargoMetadataEntry, ICargoMetadataEntryWriteable>()
      },
      {
        typeof (DeleteOperationData),
        (IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>) new DeleteMetadataOpApplier<ICargoMetadataEntry, ICargoMetadataEntryWriteable>()
      },
      {
        typeof (RestoreToFeedOperationData),
        (IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>) new RestoreToFeedMetadataOpApplier<ICargoMetadataEntry, ICargoMetadataEntryWriteable, IRestoreToFeedOperationData>()
      },
      {
        typeof (PermanentDeleteOperationData),
        (IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>) new PermanentDeleteMetadataOpApplier<ICargoMetadataEntry, ICargoMetadataEntryWriteable>()
      },
      {
        typeof (AddProblemPackageOperationData),
        (IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>) new NoChangeMetadataOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>()
      },
      {
        typeof (DelistOperationData),
        (IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>) new CargoYankChangeMetadataOpApplier<DelistOperationData>(true)
      },
      {
        typeof (RelistOperationData),
        (IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>) new CargoYankChangeMetadataOpApplier<RelistOperationData>(false)
      }
    };

    public IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>? Get(
      ICommitOperationData operationData,
      bool throwIfNotFound = true)
    {
      IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>> metadataEntryOpApplier;
      if (this.applierDictionary.TryGetValue(operationData.GetType(), out metadataEntryOpApplier))
        return metadataEntryOpApplier;
      if (throwIfNotFound)
        throw new InvalidOperationException(operationData.GetType().Name);
      return (IMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>) null;
    }
  }
}
