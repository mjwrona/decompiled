// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackageFile
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class PackageFile : IPackageFile, IUnstoredPackageFile
  {
    public PackageFile(
      string path,
      IStorageId storageId,
      IReadOnlyCollection<HashAndType> hashes,
      long size,
      DateTime dateAdded)
      : this(path, storageId, hashes, size, dateAdded, (IReadOnlyCollection<InnerFileReference>) ImmutableArray<InnerFileReference>.Empty)
    {
    }

    public PackageFile(
      string path,
      IStorageId storageId,
      IReadOnlyCollection<HashAndType> hashes,
      long size,
      DateTime dateAdded,
      IReadOnlyCollection<InnerFileReference> innerFiles)
    {
      this.Path = path;
      this.StorageId = storageId;
      this.Hashes = this.ValidateAndReturn(hashes);
      this.SizeInBytes = size;
      this.DateAdded = dateAdded;
      this.InnerFiles = innerFiles;
    }

    private IReadOnlyCollection<HashAndType> ValidateAndReturn(
      IReadOnlyCollection<HashAndType> hashes)
    {
      foreach (IEnumerable<HashAndType> source in hashes.GroupBy<HashAndType, HashType>((Func<HashAndType, HashType>) (h => h.HashType)))
      {
        if (source.Count<HashAndType>() > 1)
          throw new InvalidDataException("duplicate hash types in same list is not allowed");
      }
      return hashes;
    }

    public string Path { get; }

    public IStorageId StorageId { get; }

    public IReadOnlyCollection<HashAndType> Hashes { get; }

    public long SizeInBytes { get; }

    public DateTime DateAdded { get; }

    public IReadOnlyCollection<InnerFileReference> InnerFiles { get; }
  }
}
