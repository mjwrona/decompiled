// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.NodeBlobHasher
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class NodeBlobHasher : DedupBlobHasher
  {
    public const byte AlgorithmId = 2;
    public static readonly DedupBlobHasher Instance = (DedupBlobHasher) new NodeBlobHasher();

    public NodeBlobHasher()
      : base((byte) 2)
    {
    }

    public override BlobIdentifier OfNothing => throw new InvalidOperationException();
  }
}
