// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.LocalFilesCannotDecreaseMetadataChangeValidator`1
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
  public class LocalFilesCannotDecreaseMetadataChangeValidator<TMetadataEntry> : 
    IMetadataChangeValidator<TMetadataEntry>
    where TMetadataEntry : IMetadataEntry, IPackageFiles
  {
    public void Validate(
      IFeedRequest feedRequest,
      IPackageName packageName,
      IList<TMetadataEntry> originalEntries,
      IList<TMetadataEntry> newEntries,
      IEnumerable<ICommitLogEntry> commits)
    {
      int num1 = originalEntries.Count<TMetadataEntry>((Func<TMetadataEntry, bool>) (e => e.PackageFiles.Any<IPackageFile>((Func<IPackageFile, bool>) (f => f.StorageId.IsLocal))));
      int num2 = newEntries.Count<TMetadataEntry>((Func<TMetadataEntry, bool>) (e => e.PackageFiles.Any<IPackageFile>((Func<IPackageFile, bool>) (f => f.StorageId.IsLocal))));
      if (num1 > num2)
      {
        string str1 = string.Join<TMetadataEntry>(",", newEntries.Where<TMetadataEntry>((Func<TMetadataEntry, bool>) (e => e.PackageFiles.Any<IPackageFile>((Func<IPackageFile, bool>) (f => f.StorageId.IsLocal)))));
        string str2 = string.Join(",", commits.Select<ICommitLogEntry, string>((Func<ICommitLogEntry, string>) (c => string.Format("{0}:{1}", (object) c.CommitId, (object) c.CommitOperationData.GetType().Name))));
        throw new InvalidDataException(string.Format("BUG: could not apply edit for feed {0} package name {1}, it reduces the number of package versions with local content. Proactively failing before data loss. Original local version count: {2}. Commits: {3}. New Package Versions With Local Content: {4}", (object) feedRequest.Feed.FullyQualifiedId, (object) packageName.NormalizedName, (object) num1, (object) str2, (object) str1));
      }
    }
  }
}
