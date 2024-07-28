// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.Utf8Memory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Json
{
  internal readonly struct Utf8Memory : IEquatable<Utf8Memory>
  {
    public static readonly Utf8Memory Empty = new Utf8Memory(ReadOnlyMemory<byte>.Empty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf8Memory(ReadOnlyMemory<byte> utf8Bytes) => this.Memory = utf8Bytes;

    public ReadOnlyMemory<byte> Memory { get; }

    public Utf8Span Span => Utf8Span.UnsafeFromUtf8BytesNoValidation(this.Memory.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8Memory Slice(int start) => new Utf8Memory(this.Memory.Slice(start));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8Memory Slice(int start, int length) => new Utf8Memory(this.Memory.Slice(start, length));

    public bool IsEmpty => this.Memory.IsEmpty;

    public int Length => this.Memory.Length;

    public override bool Equals(object obj) => base.Equals(obj);

    public bool Equals(Utf8Memory utf8Memory) => this.Memory.Equals((object) utf8Memory);

    public override int GetHashCode() => this.Memory.GetHashCode();

    public override string ToString() => this.Span.ToString();

    public static Utf8Memory Create(ReadOnlyMemory<byte> utf8Bytes)
    {
      Utf8Memory utf8Memory;
      if (!Utf8Memory.TryCreate(utf8Bytes, out utf8Memory))
        throw new ArgumentException("utf8Bytes did not contain a valid UTF-8 byte sequence.");
      return utf8Memory;
    }

    public static Utf8Memory Create(string value) => Utf8Memory.UnsafeCreateNoValidation((ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(value));

    public static Utf8Memory UnsafeCreateNoValidation(ReadOnlyMemory<byte> utf8Bytes) => new Utf8Memory(utf8Bytes);

    public static bool TryCreate(ReadOnlyMemory<byte> utf8Bytes, out Utf8Memory utf8Memory)
    {
      Utf8Span utf8Span;
      if (!Utf8Span.TryParseUtf8Bytes(utf8Bytes.Span, ref utf8Span))
      {
        utf8Memory = new Utf8Memory();
        return false;
      }
      utf8Memory = new Utf8Memory(utf8Bytes);
      return true;
    }
  }
}
