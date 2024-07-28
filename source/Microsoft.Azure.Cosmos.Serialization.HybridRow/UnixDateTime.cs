// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.UnixDateTime
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  [DebuggerDisplay("{Milliseconds}")]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public readonly struct UnixDateTime : IEquatable<UnixDateTime>
  {
    public const int Size = 8;
    public static readonly UnixDateTime Epoch;

    public UnixDateTime(long milliseconds) => this.Milliseconds = milliseconds;

    public long Milliseconds { get; }

    public static bool operator ==(UnixDateTime left, UnixDateTime right) => left.Equals(right);

    public static bool operator !=(UnixDateTime left, UnixDateTime right) => !left.Equals(right);

    public bool Equals(UnixDateTime other) => this.Milliseconds == other.Milliseconds;

    public override bool Equals(object obj) => obj != null && obj is UnixDateTime other && this.Equals(other);

    public override int GetHashCode() => this.Milliseconds.GetHashCode();
  }
}
