// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Operations.NpmAddMetadataOpApplier
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.Operations
{
  public class NpmAddMetadataOpApplier : 
    IgnoreDocumentMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>
  {
    public override INpmMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      INpmMetadataEntry currentState)
    {
      if (!(commitLogEntry.CommitOperationData is INpmAddOperationData commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      if (!this.ShouldUpdate(commitOperationData, currentState))
        return currentState;
      return (INpmMetadataEntry) new NpmMetadataEntry(commitLogEntry.CommitId, commitLogEntry.CreatedDate, commitLogEntry.ModifiedDate, commitLogEntry.UserId, commitLogEntry.UserId, commitOperationData.PackageStorageId, commitOperationData.PackageSize, commitOperationData.PackageJsonBytes, commitOperationData.PackageSha1Sum, commitOperationData.PackageJsonOptions, commitOperationData.PackageManifest, commitOperationData.SourceChain, commitOperationData.PackageViews)
      {
        Deprecated = commitOperationData.DeprecateMessage
      };
    }

    private bool ShouldUpdate(INpmAddOperationData addOperationData, INpmMetadataEntry currentState)
    {
      if (currentState == null || !string.Equals(UpstreamSourceInfoUtils.GetSourceChainKey(addOperationData.SourceChain ?? Enumerable.Empty<UpstreamSourceInfo>()), UpstreamSourceInfoUtils.GetSourceChainKey(currentState.SourceChain ?? Enumerable.Empty<UpstreamSourceInfo>())) || !new HashSet<Guid>(addOperationData.PackageViews ?? Enumerable.Empty<Guid>()).SetEquals(currentState.Views ?? Enumerable.Empty<Guid>()))
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
