// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Operations.NpmDeprecateMetadataOpApplier
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.Operations
{
  public class NpmDeprecateMetadataOpApplier : 
    IgnoreDocumentMetadataEntryOpApplier<INpmMetadataEntry, MetadataDocument<INpmMetadataEntry>>
  {
    public override INpmMetadataEntry Apply(
      ICommitLogEntry commitLogEntry,
      INpmMetadataEntry currentState)
    {
      if (currentState == null)
        throw new ArgumentException(nameof (currentState));
      if (currentState.IsDeleted())
        return currentState;
      if (!(commitLogEntry.CommitOperationData is NpmDeprecateOperationData commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      INpmMetadataEntryWriteable writeable = currentState.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
      writeable.Deprecated = commitOperationData.DeprecateMessage;
      return (INpmMetadataEntry) writeable;
    }
  }
}
