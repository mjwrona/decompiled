// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.MetadataEntryWithSingleFile`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public abstract class MetadataEntryWithSingleFile<TPackageIdentity> : 
    MetadataEntry<TPackageIdentity, IPackageFile>
    where TPackageIdentity : IPackageIdentity
  {
    private IReadOnlyCollection<IPackageFile>? packageFiles;

    protected MetadataEntryWithSingleFile(
      PackagingCommitId commitId,
      DateTime createdDate,
      DateTime modifiedDate,
      Guid createdBy,
      Guid modifiedBy,
      IStorageId packageStorageId,
      long packageSize,
      IEnumerable<UpstreamSourceInfo> sourceChain)
      : base(commitId, createdDate, modifiedDate, createdBy, modifiedBy, packageStorageId, packageSize, sourceChain)
    {
    }

    protected MetadataEntryWithSingleFile()
    {
    }

    public override sealed IReadOnlyCollection<IPackageFile> PackageFiles
    {
      get
      {
        IReadOnlyCollection<IPackageFile> packageFiles = this.packageFiles;
        if (packageFiles != null)
          return packageFiles;
        return this.packageFiles = (IReadOnlyCollection<IPackageFile>) new PackageFile[1]
        {
          new PackageFile(this.SingleFilePath, this.PackageStorageId, this.SingleFileHashes, this.PackageSize, this.CreatedDate, this.SingleFileInnerFiles)
        };
      }
    }

    protected virtual IReadOnlyCollection<HashAndType> SingleFileHashes => (IReadOnlyCollection<HashAndType>) Array.Empty<HashAndType>();

    protected virtual IReadOnlyCollection<InnerFileReference> SingleFileInnerFiles => (IReadOnlyCollection<InnerFileReference>) ImmutableArray<InnerFileReference>.Empty;

    public abstract string SingleFilePath { get; }
  }
}
