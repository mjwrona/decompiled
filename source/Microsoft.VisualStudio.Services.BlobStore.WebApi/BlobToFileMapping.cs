// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.BlobToFileMapping
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class BlobToFileMapping
  {
    public string ItemPath { get; set; }

    public string FilePath { get; set; }

    public BlobIdentifier BlobId { get; set; }

    public FileBlobType BlobType { get; set; }

    public Uri DownloadUri { get; set; }

    public long? FileLength { get; set; }

    public GetDedupAsyncFunc DedupGetter { get; set; }

    public string SymbolicLinkTargetPath { get; set; }

    [CLSCompliant(false)]
    public uint? PermissionValue { get; set; }
  }
}
