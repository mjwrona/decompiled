// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonMemoryWriter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Json
{
  internal abstract class JsonMemoryWriter
  {
    protected byte[] buffer;

    protected JsonMemoryWriter(int initialCapacity = 256) => this.buffer = new byte[initialCapacity];

    public int Position { get; set; }

    public Span<byte> Cursor => this.buffer.AsSpan<byte>().Slice(this.Position);

    public ReadOnlyMemory<byte> BufferAsMemory => (ReadOnlyMemory<byte>) this.buffer.AsMemory<byte>();

    public Span<byte> BufferAsSpan => this.buffer.AsSpan<byte>();

    public Memory<byte> RawBuffer => (Memory<byte>) this.buffer;

    public void Write(ReadOnlySpan<byte> value)
    {
      this.EnsureRemainingBufferSpace(value.Length);
      value.CopyTo(this.Cursor);
      this.Position += value.Length;
    }

    public void EnsureRemainingBufferSpace(int size)
    {
      if (this.Position + size < this.buffer.Length)
        return;
      this.Resize(this.Position + size);
    }

    private void Resize(int minNewSize)
    {
      if (minNewSize < 0)
        throw new ArgumentOutOfRangeException();
      Array.Resize<byte>(ref this.buffer, (int) Math.Min((long) (minNewSize * 2), (long) int.MaxValue));
    }
  }
}
