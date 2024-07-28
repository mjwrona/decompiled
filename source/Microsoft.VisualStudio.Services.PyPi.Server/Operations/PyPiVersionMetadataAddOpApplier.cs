// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Operations.PyPiVersionMetadataAddOpApplier
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Operations
{
  public class PyPiVersionMetadataAddOpApplier : 
    IgnoreDocumentMetadataEntryOpApplier<IPyPiMetadataEntryWithRawMetadata, MetadataDocument<IPyPiMetadataEntryWithRawMetadata>>
  {
    public override IPyPiMetadataEntryWithRawMetadata Apply(
      ICommitLogEntry commitLogEntry,
      IPyPiMetadataEntryWithRawMetadata currentState)
    {
      if (!(commitLogEntry.CommitOperationData is PyPiAddOperationData commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      if (!this.ShouldUpdate(commitOperationData, currentState))
        return currentState;
      PyPiResolvedMetadata from = PyPiResolvedMetadata.ParseFrom(commitOperationData.MetadataFields);
      string str1 = from.Sha256.IsNullOrEmpty<char>() ? commitOperationData.ComputedSha256 : from.Sha256;
      string str2 = from.Md5.IsNullOrEmpty<char>() ? commitOperationData.ComputedMd5 : from.Md5;
      List<HashAndType> hashes = new List<HashAndType>();
      if (!string.IsNullOrEmpty(str1))
        hashes.Add(new HashAndType()
        {
          HashType = HashType.SHA256,
          Value = str1
        });
      if (!string.IsNullOrEmpty(str2))
        hashes.Add(new HashAndType()
        {
          HashType = HashType.MD5,
          Value = str2
        });
      PyPiPackageFileWithRawMetadata packageFile = new PyPiPackageFileWithRawMetadata(commitOperationData.FileName, commitOperationData.PackageStorageId, (IReadOnlyCollection<HashAndType>) hashes, commitOperationData.PackageSize, commitLogEntry.CreatedDate, from.DistType, commitOperationData.MetadataFields, commitOperationData.GpgSignature);
      if (currentState == null)
        return (IPyPiMetadataEntryWithRawMetadata) new PyPiMetadataEntryWithRawMetadata(commitOperationData.Identity, packageFile, from.RequiresPython, commitLogEntry.CommitId, commitLogEntry.UserId, commitLogEntry.CreatedDate, commitLogEntry.UserId, commitLogEntry.ModifiedDate, commitOperationData.SourceChain);
      IPyPiMetadataEntryWithRawMetadataWritable writeable = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      int num = writeable.AddPackageFile(packageFile) ? 1 : 0;
      if (!commitOperationData.SourceChain.IsNullOrEmpty<UpstreamSourceInfo>() && ((IMetadataEntryWritable) writeable).SourceChain.IsNullOrEmpty<UpstreamSourceInfo>())
        writeable.SourceChain = commitOperationData.SourceChain;
      return num == 0 ? currentState : (IPyPiMetadataEntryWithRawMetadata) writeable;
    }

    private bool ShouldUpdate(
      PyPiAddOperationData addOperationData,
      IPyPiMetadataEntryWithRawMetadata currentState)
    {
      if (currentState == null)
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
      return true;
label_6:
      return false;
    }
  }
}
