// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.LocalEntriesCannotDecreaseMetadataChangeValidator`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class LocalEntriesCannotDecreaseMetadataChangeValidator<TMetadataEntry> : 
    IMetadataChangeValidator<TMetadataEntry>
    where TMetadataEntry : IMetadataEntry
  {
    public void Validate(
      IFeedRequest feedRequest,
      IPackageName packageName,
      IList<TMetadataEntry> originalEntries,
      IList<TMetadataEntry> newEntries,
      IEnumerable<ICommitLogEntry> commits)
    {
      int num = originalEntries.Count<TMetadataEntry>((Func<TMetadataEntry, bool>) (e => e.PackageStorageId == null || e.PackageStorageId.IsLocal));
      if (newEntries.Count<TMetadataEntry>((Func<TMetadataEntry, bool>) (e => e.PackageStorageId == null || e.PackageStorageId.IsLocal)) < num)
      {
        string str1 = string.Join(",", newEntries.Where<TMetadataEntry>((Func<TMetadataEntry, bool>) (e => e.PackageStorageId == null || e.PackageStorageId.IsLocal)).Select<TMetadataEntry, string>((Func<TMetadataEntry, string>) (e => e.PackageIdentity.Version.NormalizedVersion)));
        string str2 = string.Join(",", commits.Select<ICommitLogEntry, string>((Func<ICommitLogEntry, string>) (c => string.Format("{0}:{1}", (object) c.CommitId, (object) c.CommitOperationData.GetType().Name))));
        throw new InvalidDataException(string.Format("BUG: could not apply edit for feed {0} package name {1}, it reduces the number of local packages. Proactively failing before data loss. Original local version count: {2}. Commits: {3}. New Package Versions: {4}", (object) feedRequest.Feed.FullyQualifiedId, (object) packageName.NormalizedName, (object) num, (object) str2, (object) str1));
      }
    }
  }
}
