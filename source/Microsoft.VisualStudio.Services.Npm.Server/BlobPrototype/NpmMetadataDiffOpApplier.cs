// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype.NpmMetadataDiffOpApplier
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.BlobPrototype
{
  public class NpmMetadataDiffOpApplier : 
    IMetadataDocumentOpApplier<MetadataDocument<INpmMetadataEntry>>
  {
    public MetadataDocument<INpmMetadataEntry> Apply(
      ICommitLogEntry commitLogEntry,
      MetadataDocument<INpmMetadataEntry> currentState)
    {
      if (!(commitLogEntry.CommitOperationData is NpmMetadataDiffOperationData commitOperationData))
        throw new ArgumentException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_InvalidCommitEntryType());
      bool flag = false;
      List<INpmMetadataEntry> entries = new List<INpmMetadataEntry>();
      foreach (INpmMetadataEntry entry in currentState.Entries)
      {
        if (entry.IsDeleted() || !entry.IsLocal)
        {
          entries.Add(entry);
        }
        else
        {
          VersionMetadata versionMetadata1 = commitOperationData.NewVersionMetadata[entry.PackageIdentity.Version.ToString()];
          VersionMetadata versionMetadata2 = commitOperationData.OldVersionMetadata[entry.PackageIdentity.Version.ToString()];
          if (versionMetadata1.Deprecated != versionMetadata2.Deprecated)
          {
            INpmMetadataEntryWriteable writeable = entry.CreateWriteable((ICommitLogEntryHeader) commitLogEntry);
            writeable.Deprecated = versionMetadata1.Deprecated;
            entries.Add((INpmMetadataEntry) writeable);
            flag = true;
          }
          else
            entries.Add(entry);
        }
      }
      if (!flag)
        return currentState;
      return new MetadataDocument<INpmMetadataEntry>(entries, currentState.Properties).UpdateSharedMetadata((object) new NpmLocalAndUpstreamSharedPackageMetadata()
      {
        LocalDistributionTags = currentState.GetLocalDistTags(),
        UpstreamDistributionTags = currentState.GetUpstreamDistTags(),
        Revision = Guid.NewGuid().ToString()
      });
    }
  }
}
