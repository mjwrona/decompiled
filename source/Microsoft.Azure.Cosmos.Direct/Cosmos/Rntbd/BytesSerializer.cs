// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Rntbd.BytesSerializer
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Rntbd
{
  internal ref struct BytesSerializer
  {
    private readonly Span<byte> targetByteArray;
    private int position;

    public BytesSerializer(byte[] targetByteArray, int length)
    {
      this.targetByteArray = new Span<byte>(targetByteArray, 0, length);
      this.position = 0;
    }

    public static Guid ReadGuidFromBytes(ArraySegment<byte> array) => MemoryMarshal.Read<Guid>((ReadOnlySpan<byte>) new Span<byte>(array.Array, array.Offset, array.Count));

    public static unsafe string GetStringFromBytes(ReadOnlyMemory<byte> memory)
    {
      if (memory.IsEmpty)
        return string.Empty;
      fixed (byte* bytes = &memory.Span.GetPinnableReference())
        return Encoding.UTF8.GetString(bytes, memory.Length);
    }

    public static ReadOnlyMemory<byte> GetBytesForString(
      string toConvert,
      RntbdConstants.Request request)
    {
      byte[] bytes1 = request.GetBytes(Encoding.UTF8.GetMaxByteCount(toConvert.Length));
      int bytes2 = Encoding.UTF8.GetBytes(toConvert, 0, toConvert.Length, bytes1, 0);
      return new ReadOnlyMemory<byte>(bytes1, 0, bytes2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetSizeOfGuid() => sizeof (Guid);

    public void Write(byte[] value) => this.Write(new ArraySegment<byte>(value, 0, value.Length));

    public void Write(uint value) => this.WriteValue<uint>(value, 4);

    public void Write(int value) => this.WriteValue<int>(value, 4);

    public void Write(long value) => this.WriteValue<long>(value, 8);

    public void Write(ulong value) => this.WriteValue<ulong>(value, 8);

    public void Write(float value) => this.WriteValue<float>(value, 4);

    public void Write(double value) => this.WriteValue<double>(value, 8);

    public void Write(ushort value) => this.WriteValue<ushort>(value, 2);

    public void Write(byte value) => this.WriteValue<byte>(value, 1);

    public int Write(Guid value)
    {
      this.WriteValue<Guid>(value, BytesSerializer.GetSizeOfGuid());
      return BytesSerializer.GetSizeOfGuid();
    }

    public void Write(ArraySegment<byte> value) => this.Write((ReadOnlySpan<byte>) new Span<byte>(value.Array, value.Offset, value.Count));

    public void Write(ReadOnlyMemory<byte> valueToWrite) => this.Write(valueToWrite.Span);

    public void Write(ReadOnlySpan<byte> valueToWrite)
    {
      Span<byte> destination = this.targetByteArray.Slice(this.position);
      valueToWrite.CopyTo(destination);
      this.position += valueToWrite.Length;
    }

    private void WriteValue<T>(T value, int sizeT) where T : struct
    {
      MemoryMarshal.Write<T>(this.targetByteArray.Slice(this.position), ref value);
      this.position += sizeT;
    }
  }
}
