// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ArraySegmentExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class ArraySegmentExtensions
  {
    public static T[] CreateCopy<T>(this ArraySegment<T> segment)
    {
      T[] dst = new T[segment.Count];
      Buffer.BlockCopy((Array) segment.Array, segment.Offset, (Array) dst, 0, segment.Count);
      return dst;
    }

    public static MemoryStream AsMemoryStream(this ArraySegment<byte> bytes) => new MemoryStream(bytes.Array, bytes.Offset, bytes.Count);

    public static ArraySegment<byte> AsArraySegment(this byte[] bytes) => new ArraySegment<byte>(bytes);
  }
}
