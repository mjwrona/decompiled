// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiPackageFileWithRawMetadata
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiPackageFileWithRawMetadata : PyPiPackageFile
  {
    public PyPiPackageFileWithRawMetadata(
      string path,
      IStorageId storageId,
      IReadOnlyCollection<HashAndType> hashes,
      long size,
      DateTime dateAdded,
      PyPiDistType distType,
      IReadOnlyDictionary<string, string[]> rawMetadata,
      DeflateCompressibleBytes gpgSignature)
      : base(path, storageId, hashes, size, dateAdded, distType)
    {
      this.RawMetadata = rawMetadata;
      this.GpgSignature = gpgSignature;
    }

    public IReadOnlyDictionary<string, string[]> RawMetadata { get; }

    public DeflateCompressibleBytes GpgSignature { get; }
  }
}
