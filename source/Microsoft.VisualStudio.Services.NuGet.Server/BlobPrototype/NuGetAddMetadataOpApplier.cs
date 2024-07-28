// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetAddMetadataOpApplier
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetAddMetadataOpApplier : 
    IgnoreDocumentMetadataEntryOpApplier<INuGetMetadataEntry, MetadataDocument<INuGetMetadataEntry>>
  {
    public override INuGetMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      INuGetMetadataEntry currentState)
    {
      if (!(commitLogEntry.CommitOperationData is INuGetAddOperationData commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      return !this.ShouldUpdate(commitOperationData, currentState) ? currentState : (INuGetMetadataEntry) new NuGetMetadataEntry(commitLogEntry.CommitId, commitLogEntry.CreatedDate, commitLogEntry.ModifiedDate, commitLogEntry.UserId, commitLogEntry.UserId, commitOperationData.PackageStorageId, commitOperationData.PackageSize, new ContentBytes(commitOperationData.NuspecBytes), !commitOperationData.AddAsDelisted, commitOperationData.SourceChain);
    }

    private bool ShouldUpdate(
      INuGetAddOperationData addOperationData,
      INuGetMetadataEntry currentState)
    {
      if (currentState == null || !string.Equals(UpstreamSourceInfoUtils.GetSourceChainKey(addOperationData.SourceChain ?? Enumerable.Empty<UpstreamSourceInfo>()), UpstreamSourceInfoUtils.GetSourceChainKey(currentState.SourceChain ?? Enumerable.Empty<UpstreamSourceInfo>())))
        return true;
      DateTime? nullable;
      if (currentState != null)
      {
        nullable = currentState.DeletedDate;
        if (nullable.HasValue)
          goto label_6;
      }
      if (currentState != null)
      {
        nullable = currentState.PermanentDeletedDate;
        if (nullable.HasValue)
          goto label_6;
      }
      return currentState == null || !currentState.PackageStorageId.Equals(addOperationData.PackageStorageId);
label_6:
      return false;
    }
  }
}
