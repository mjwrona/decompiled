// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.JsonMemoryReader
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Json
{
  internal abstract class JsonMemoryReader
  {
    protected readonly ReadOnlyMemory<byte> buffer;
    protected int position;

    protected JsonMemoryReader(ReadOnlyMemory<byte> buffer) => this.buffer = buffer;

    public bool IsEof => this.position >= this.buffer.Length;

    public int Position => this.position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Read()
    {
      int num = this.position < this.buffer.Length ? (int) this.buffer.Span[this.position] : 0;
      ++this.position;
      return (byte) num;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Peek() => this.position >= this.buffer.Length ? (byte) 0 : this.buffer.Span[this.position];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<byte> GetBufferedRawJsonToken() => this.buffer.Slice(this.position);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<byte> GetBufferedRawJsonToken(int startPosition) => this.buffer.Slice(startPosition);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<byte> GetBufferedRawJsonToken(int startPosition, int endPosition) => this.buffer.Slice(startPosition, endPosition - startPosition);
  }
}
