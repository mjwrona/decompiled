// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.EncodingExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Text;

namespace Microsoft.Azure.Cosmos
{
  internal static class EncodingExtensions
  {
    public static unsafe string GetString(this Encoding encoding, ReadOnlySpan<byte> src)
    {
      if (src.IsEmpty)
        return string.Empty;
      fixed (byte* bytes = &src.GetPinnableReference())
        return encoding.GetString(bytes, src.Length);
    }

    public static unsafe int GetChars(
      this Encoding encoding,
      ReadOnlySpan<byte> src,
      Span<char> dest)
    {
      if (src.IsEmpty)
        return 0;
      fixed (byte* bytes = &src.GetPinnableReference())
        fixed (char* chars = &dest.GetPinnableReference())
          return encoding.GetChars(bytes, src.Length, chars, dest.Length);
    }

    public static int GetBytes(this Encoding encoding, string src, Span<byte> dest) => encoding.GetBytes(MemoryExtensions.AsSpan(src), dest);

    public static unsafe int GetBytes(
      this Encoding encoding,
      ReadOnlySpan<char> src,
      Span<byte> dest)
    {
      if (src.Length == 0 || dest.Length == 0)
        return 0;
      fixed (char* chars = &src.GetPinnableReference())
        fixed (byte* bytes = &dest.GetPinnableReference())
          return encoding.GetBytes(chars, src.Length, bytes, dest.Length);
    }

    public static unsafe int GetByteCount(this Encoding encoding, ReadOnlySpan<char> src)
    {
      if (src.IsEmpty)
        return 0;
      fixed (char* chars = &src.GetPinnableReference())
        return encoding.GetByteCount(chars, src.Length);
    }
  }
}
