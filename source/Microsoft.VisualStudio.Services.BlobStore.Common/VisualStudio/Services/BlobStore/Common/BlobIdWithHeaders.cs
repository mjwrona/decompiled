// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdWithHeaders
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public struct BlobIdWithHeaders
  {
    public readonly BlobIdentifier BlobId;
    public readonly string FileName;
    public readonly string ContentType;
    public readonly EdgeCache EdgeCache;
    public readonly DateTimeOffset? ExpiryTime;

    public BlobIdWithHeaders(
      BlobIdentifier blobId,
      EdgeCache edgeCache,
      string filename = null,
      string contentType = null,
      DateTimeOffset? expiryTime = null)
    {
      this.BlobId = blobId;
      this.FileName = filename;
      this.ContentType = contentType;
      this.EdgeCache = edgeCache;
      this.ExpiryTime = expiryTime;
    }
  }
}
