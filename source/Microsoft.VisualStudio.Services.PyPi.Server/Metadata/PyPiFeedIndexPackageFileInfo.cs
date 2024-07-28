// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiFeedIndexPackageFileInfo
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiFeedIndexPackageFileInfo
  {
    public string FileName { get; set; }

    public string PythonVersion { get; set; }

    public string FileType { get; set; }

    public string CommentText { get; set; }

    public string Md5Digest { get; set; }

    public string Sha256Digest { get; set; }

    public string Blake2Digest { get; set; }

    public IStorageId StorageId { get; set; }

    public long Size { get; set; }

    public DateTime DateAdded { get; set; }

    public PyPiFeedIndexPackageFileInfo(
      IReadOnlyDictionary<string, string[]> metadataFields,
      string fileName,
      IStorageId storageId,
      long size,
      DateTime dateAdded)
    {
      this.FileName = fileName;
      this.StorageId = storageId;
      this.Size = size;
      this.DateAdded = dateAdded;
      this.PythonVersion = PyPiMetadataUtils.GetOptionalMetadataField("pyversion", metadataFields);
      this.FileType = PyPiMetadataUtils.GetOptionalMetadataField("filetype", metadataFields);
      this.CommentText = PyPiMetadataUtils.GetOptionalMetadataField("comment", metadataFields);
      this.Md5Digest = PyPiMetadataUtils.GetOptionalMetadataField("md5_digest", metadataFields);
      this.Sha256Digest = PyPiMetadataUtils.GetOptionalMetadataField("sha256_digest", metadataFields);
      this.Blake2Digest = PyPiMetadataUtils.GetOptionalMetadataField("blake2_256_digest", metadataFields);
    }
  }
}
