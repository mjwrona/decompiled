// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.Utf8StringHelpers
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Json
{
  internal static class Utf8StringHelpers
  {
    public static unsafe string ToString(ReadOnlyMemory<byte> buffer)
    {
      ArraySegment<byte> segment;
      string str;
      if (MemoryMarshal.TryGetArray<byte>(buffer, out segment))
      {
        str = Encoding.UTF8.GetString(segment.Array, segment.Offset, buffer.Length);
      }
      else
      {
        ReadOnlySpan<byte> span = buffer.Span;
        fixed (byte* bytes = &span.GetPinnableReference())
          str = Encoding.UTF8.GetString(bytes, span.Length);
      }
      return str;
    }
  }
}
