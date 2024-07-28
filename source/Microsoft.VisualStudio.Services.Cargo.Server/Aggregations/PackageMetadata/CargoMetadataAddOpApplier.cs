// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata.CargoMetadataAddOpApplier
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.Operations;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata
{
  public class CargoMetadataAddOpApplier : 
    IgnoreDocumentMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>
  {
    public override ICargoMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      ICargoMetadataEntry? currentState)
    {
      if (!(commitLogEntry.CommitOperationData is CargoAddOperationData commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      return !this.ShouldUpdate(commitOperationData, currentState) ? currentState : (ICargoMetadataEntry) new CargoMetadataEntry(commitLogEntry.CommitId, commitLogEntry.CreatedDate, commitLogEntry.ModifiedDate, commitLogEntry.UserId, commitLogEntry.UserId, commitOperationData.PackageStorageId, commitOperationData.PackageSize, commitOperationData.Metadata, commitOperationData.SourceChain, (IEnumerable<HashAndType>) commitOperationData.Hashes, (IEnumerable<InnerFileReference>) commitOperationData.InnerFiles, commitOperationData.AddAsYanked);
    }

    private bool ShouldUpdate(
      CargoAddOperationData addOperationData,
      ICargoMetadataEntry? currentState)
    {
      return currentState == null || !currentState.IsDeleted() && !currentState.PackageStorageId.Equals(addOperationData.PackageStorageId);
    }
  }
}
