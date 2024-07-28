// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Metadata.MavenPackageFileNew
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Maven.Server.Metadata
{
  public class MavenPackageFileNew : PackageFile
  {
    public MavenPackageFileNew(string path, IStorageId storageId, long size, DateTime dateAdded)
      : this(path, storageId, (IEnumerable<HashAndType>) null, size, dateAdded)
    {
    }

    public MavenPackageFileNew(
      string path,
      IStorageId storageId,
      IEnumerable<HashAndType> hashes,
      long size,
      DateTime dateAdded)
      : base(path, storageId, (IReadOnlyCollection<HashAndType>) (hashes != null ? hashes.ToImmutableArray<HashAndType>() : ImmutableArray<HashAndType>.Empty), size, dateAdded)
    {
    }
  }
}
