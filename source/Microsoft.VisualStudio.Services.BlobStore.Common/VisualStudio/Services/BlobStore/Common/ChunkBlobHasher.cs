// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ChunkBlobHasher
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class ChunkBlobHasher : DedupBlobHasher
  {
    public const byte AlgorithmId = 1;
    private static readonly Lazy<BlobIdentifier> ofNothing = new Lazy<BlobIdentifier>((Func<BlobIdentifier>) (() => ChunkDedupIdentifier.CalculateIdentifier(new byte[0]).ToBlobIdentifier()));
    public static readonly DedupBlobHasher Instance = (DedupBlobHasher) new ChunkBlobHasher();

    public ChunkBlobHasher()
      : base((byte) 1)
    {
    }

    public override BlobIdentifier OfNothing => ChunkBlobHasher.ofNothing.Value;
  }
}
