// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype.NpmDistTagRemoveOpApplier
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype
{
  public class NpmDistTagRemoveOpApplier : 
    IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>>
  {
    public MetadataDocument<INpmMetadataEntry> Apply(
      ICommitLogEntry commitLogEntry,
      MetadataDocument<INpmMetadataEntry> currentState)
    {
      if (!(commitLogEntry.CommitOperationData is NpmDistTagRemoveOperationData commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      if (!currentState.GetLocalDistTags().Remove(commitOperationData.Tag))
        return currentState;
      return currentState.UpdateSharedMetadata((object) new NpmLocalAndUpstreamSharedPackageMetadata()
      {
        LocalDistributionTags = currentState.GetLocalDistTags(),
        UpstreamDistributionTags = currentState.GetUpstreamDistTags(),
        Revision = Guid.NewGuid().ToString()
      });
    }
  }
}
