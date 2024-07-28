// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Rntbd.BytesDeserializer
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Rntbd
{
  internal struct BytesDeserializer
  {
    private readonly Memory<byte> metadata;

    public BytesDeserializer(byte[] metadata, int length)
      : this()
    {
      this.metadata = new Memory<byte>(metadata, 0, length);
      this.Position = 0;
      this.Length = length;
    }

    public int Position { get; private set; }

    public int Length { get; }

    public ushort ReadUInt16()
    {
      int num = (int) MemoryMarshal.Read<ushort>((ReadOnlySpan<byte>) this.metadata.Span.Slice(this.Position));
      this.Position += 2;
      return (ushort) num;
    }

    public byte ReadByte()
    {
      int num = (int) this.metadata.Span[this.Position];
      ++this.Position;
      return (byte) num;
    }

    public uint ReadUInt32()
    {
      int num = (int) MemoryMarshal.Read<uint>((ReadOnlySpan<byte>) this.metadata.Span.Slice(this.Position));
      this.Position += 4;
      return (uint) num;
    }

    public int ReadInt32()
    {
      int num = MemoryMarshal.Read<int>((ReadOnlySpan<byte>) this.metadata.Span.Slice(this.Position));
      this.Position += 4;
      return num;
    }

    public ulong ReadUInt64()
    {
      long num = (long) MemoryMarshal.Read<ulong>((ReadOnlySpan<byte>) this.metadata.Span.Slice(this.Position));
      this.Position += 8;
      return (ulong) num;
    }

    public long ReadInt64()
    {
      long num = MemoryMarshal.Read<long>((ReadOnlySpan<byte>) this.metadata.Span.Slice(this.Position));
      this.Position += 8;
      return num;
    }

    public float ReadSingle()
    {
      double num = (double) MemoryMarshal.Read<float>((ReadOnlySpan<byte>) this.metadata.Span.Slice(this.Position));
      this.Position += 4;
      return (float) num;
    }

    public double ReadDouble()
    {
      double num = MemoryMarshal.Read<double>((ReadOnlySpan<byte>) this.metadata.Span.Slice(this.Position));
      this.Position += 8;
      return num;
    }

    public Guid ReadGuid()
    {
      Guid guid = MemoryMarshal.Read<Guid>((ReadOnlySpan<byte>) this.metadata.Span.Slice(this.Position));
      this.Position += 16;
      return guid;
    }

    public ReadOnlyMemory<byte> ReadBytes(int length)
    {
      ReadOnlyMemory<byte> readOnlyMemory = (ReadOnlyMemory<byte>) this.metadata.Slice(this.Position, length);
      this.Position += length;
      return readOnlyMemory;
    }
  }
}
