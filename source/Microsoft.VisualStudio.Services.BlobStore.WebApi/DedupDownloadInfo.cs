// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupDownloadInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class DedupDownloadInfo : DedupDownloadInfoBase
  {
    public DedupDownloadInfo()
    {
    }

    public DedupDownloadInfo(
      DedupDownloadInfoBase downloadInfoBase,
      ChunkDedupDownloadInfo[] chunks,
      long transitiveSize)
      : this(downloadInfoBase.Id, downloadInfoBase.Url, chunks, transitiveSize)
    {
    }

    public DedupDownloadInfo(
      DedupIdentifier id,
      Uri url,
      ChunkDedupDownloadInfo[] chunks,
      long transitiveSize)
      : base(id, url, transitiveSize)
    {
      this.Chunks = chunks ?? Array.Empty<ChunkDedupDownloadInfo>();
    }

    public ChunkDedupDownloadInfo[] Chunks { get; set; }
  }
}
