// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupDownloadInfoBase
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public abstract class DedupDownloadInfoBase
  {
    public DedupDownloadInfoBase()
    {
    }

    public DedupDownloadInfoBase(DedupIdentifier id, Uri url, long transitiveSize)
    {
      this.Id = id;
      this.Url = url;
      this.Size = transitiveSize;
    }

    public DedupIdentifier Id { get; set; }

    public Uri Url { get; set; }

    public long Size { get; set; }
  }
}
