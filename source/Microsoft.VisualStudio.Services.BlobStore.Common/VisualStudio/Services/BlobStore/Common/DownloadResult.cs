// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DownloadResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System.Net;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public struct DownloadResult
  {
    public readonly HttpStatusCode HttpStatusCode;
    public readonly int ErrorCode;
    public readonly long BytesDownloaded;

    public DownloadResult(HttpStatusCode statusCode, int errorCode, long bytesDownloaded)
    {
      this.HttpStatusCode = statusCode;
      this.ErrorCode = errorCode;
      this.BytesDownloaded = bytesDownloaded;
    }
  }
}
