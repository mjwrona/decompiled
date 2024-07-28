// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ByteArrayPool
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public sealed class ByteArrayPool : Pool<byte[]>
  {
    private static Action<byte[]> Reset = (Action<byte[]>) (b => { });

    private static byte[] CreateNew(int bufferSize)
    {
      byte[] numArray = new byte[bufferSize];
      ByteArrayPool.Reset(numArray);
      return numArray;
    }

    public ByteArrayPool(int bufferSize, int maxToKeep)
      : base((Func<byte[]>) (() => ByteArrayPool.CreateNew(bufferSize)), ByteArrayPool.Reset, maxToKeep)
    {
    }

    public override Pool<byte[]>.PoolHandle Get() => base.Get();
  }
}
