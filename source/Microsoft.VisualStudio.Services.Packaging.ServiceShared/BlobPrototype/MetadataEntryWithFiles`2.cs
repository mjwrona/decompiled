// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MetadataEntryWithFiles`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public abstract class MetadataEntryWithFiles<TPackageIdentity, TPackageFile> : 
    MetadataEntry<TPackageIdentity, TPackageFile>
    where TPackageIdentity : IPackageIdentity
    where TPackageFile : IPackageFile
  {
    protected MetadataEntryWithFiles(
      PackagingCommitId commitId,
      DateTime createdDate,
      DateTime modifiedDate,
      Guid createdBy,
      Guid modifiedBy,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      IEnumerable<TPackageFile> packageFiles)
      : base(commitId, createdDate, modifiedDate, createdBy, modifiedBy, (IStorageId) null, -1L, sourceChain)
    {
      this.PackageFilesInternal = (IReadOnlyCollection<TPackageFile>) ((packageFiles != null ? packageFiles.ToImmutableList<TPackageFile>() : (ImmutableList<TPackageFile>) null) ?? ImmutableList<TPackageFile>.Empty);
    }

    protected MetadataEntryWithFiles()
    {
      this.PackageSize = -1L;
      this.PackageFilesInternal = (IReadOnlyCollection<TPackageFile>) ImmutableList<TPackageFile>.Empty;
    }

    protected void CopyFrom(
      MetadataEntryWithFiles<TPackageIdentity, TPackageFile> source)
    {
      this.CopyFrom((MetadataEntry<TPackageIdentity, TPackageFile>) source);
      IReadOnlyCollection<TPackageFile> packageFiles = source.PackageFiles;
      this.PackageFilesInternal = packageFiles != null ? (IReadOnlyCollection<TPackageFile>) packageFiles.ToImmutableList<TPackageFile>() : (IReadOnlyCollection<TPackageFile>) null;
    }

    public bool AddPackageFile(TPackageFile packageFile)
    {
      ImmutableList<TPackageFile> immutableList = this.PackageFiles.ToImmutableList<TPackageFile>();
      string filePath = packageFile.Path;
      TPackageFile packageFileWithPath = this.GetPackageFileWithPath(filePath);
      if ((object) packageFileWithPath != null)
      {
        if (packageFileWithPath.StorageId.IsLocal || !packageFile.StorageId.IsLocal)
          return false;
        IEqualityComparer<string> pathEqualityComparer = this.PathEqualityComparer;
        immutableList = immutableList.RemoveAll((Predicate<TPackageFile>) (f => pathEqualityComparer.Equals(f.Path, filePath)));
      }
      this.PackageFilesInternal = (IReadOnlyCollection<TPackageFile>) immutableList.Add(packageFile);
      return true;
    }

    protected IReadOnlyCollection<TPackageFile> PackageFilesInternal { get; set; }

    public override IReadOnlyCollection<TPackageFile> PackageFiles => this.PackageFilesInternal;
  }
}
