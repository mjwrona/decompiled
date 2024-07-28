// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.StringToken
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  public readonly struct StringToken
  {
    public readonly ulong Id;
    public readonly byte[] Varint;
    public readonly Utf8String Path;

    public StringToken(ulong id, Utf8String path)
    {
      this.Id = id;
      this.Varint = new byte[StringToken.Count7BitEncodedUInt(id)];
      StringToken.Write7BitEncodedUInt(this.Varint.AsSpan<byte>(), id);
      this.Path = path;
    }

    public bool IsNull
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => this.Varint == null;
    }

    private static int Write7BitEncodedUInt(Span<byte> buffer, ulong value)
    {
      int index = 0;
      for (; value >= 128UL; value >>= 7)
      {
        buffer[index] = (byte) (value | 128UL);
        checked { ++index; }
      }
      buffer[index] = checked ((byte) value);
      return checked (index + 1);
    }

    private static int Count7BitEncodedUInt(ulong value)
    {
      int num = 0;
      for (; value >= 128UL; value >>= 7)
        checked { ++num; }
      return checked (num + 1);
    }
  }
}
