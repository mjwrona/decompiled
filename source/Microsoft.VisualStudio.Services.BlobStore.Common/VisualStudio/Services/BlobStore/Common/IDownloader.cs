// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.IDownloader
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public interface IDownloader
  {
    Task<DownloadResult> DownloadAsync(
      string path,
      string uri,
      long? size,
      CancellationToken cancellationToken);

    Task<StreamWithRange> GetBlobAsync(
      Uri downloadUri,
      long? knownLength,
      TimeSpan? timeout,
      CancellationToken cancellationToken);
  }
}
