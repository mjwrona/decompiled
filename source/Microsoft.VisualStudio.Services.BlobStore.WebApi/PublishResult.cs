// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.PublishResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public sealed class PublishResult
  {
    public readonly DedupIdentifier ManifestId;
    public readonly DedupIdentifier RootId;
    public readonly IEnumerable<string> ProofNodes;
    public readonly long FileCount;
    public readonly long ContentSize;

    public PublishResult(
      DedupIdentifier manifestId,
      DedupIdentifier root,
      IEnumerable<string> proofNodes,
      long fileCount,
      long contentSize)
    {
      this.ManifestId = manifestId;
      this.RootId = root;
      this.ProofNodes = proofNodes;
      this.FileCount = fileCount;
      this.ContentSize = contentSize;
    }
  }
}
