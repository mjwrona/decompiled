// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.NullValue
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public readonly struct NullValue : IEquatable<NullValue>
  {
    public static readonly NullValue Default;

    public static bool operator ==(NullValue left, NullValue right) => left.Equals(right);

    public static bool operator !=(NullValue left, NullValue right) => !left.Equals(right);

    public bool Equals(NullValue other) => true;

    public override bool Equals(object obj) => obj is NullValue other && this.Equals(other);

    public override int GetHashCode() => 42;
  }
}
