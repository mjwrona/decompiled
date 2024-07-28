// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata.CargoYankChangeMetadataOpApplier`1
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.PackageMetadata
{
  public class CargoYankChangeMetadataOpApplier<TOpData> : 
    IgnoreDocumentMetadataEntryOpApplier<ICargoMetadataEntry, MetadataDocument<ICargoMetadataEntry>>
    where TOpData : IListingStateChangeOperationData
  {
    private readonly bool changeYankedStateTo;

    public CargoYankChangeMetadataOpApplier(bool changeYankedStateTo) => this.changeYankedStateTo = changeYankedStateTo;

    public override ICargoMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      ICargoMetadataEntry? currentState)
    {
      if (!(commitLogEntry.CommitOperationData is TOpData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      if (currentState == null)
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_NoCurrentStateForNonAddOperation());
      if (currentState.IsDeleted())
        return currentState;
      ICargoMetadataEntryWriteable writeable = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      writeable.Yanked = this.changeYankedStateTo;
      return (ICargoMetadataEntry) writeable;
    }
  }
}
