// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.TypeArgument
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System;
using System.Diagnostics;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  [DebuggerDisplay("{this.type == null ? null : ToString()}")]
  public readonly struct TypeArgument : IEquatable<TypeArgument>
  {
    private readonly LayoutType type;
    private readonly TypeArgumentList typeArgs;

    public TypeArgument(LayoutType type)
    {
      Contract.Requires(type != null);
      this.type = type;
      this.typeArgs = TypeArgumentList.Empty;
    }

    public TypeArgument(LayoutType type, TypeArgumentList typeArgs)
    {
      Contract.Requires(type != null);
      this.type = type;
      this.typeArgs = typeArgs;
    }

    public LayoutType Type => this.type;

    public TypeArgumentList TypeArgs => this.typeArgs;

    public static bool operator ==(TypeArgument left, TypeArgument right) => left.Equals(right);

    public static bool operator !=(TypeArgument left, TypeArgument right) => !left.Equals(right);

    [DebuggerHidden]
    public T TypeAs<T>() where T : ILayoutType => this.type.TypeAs<T>();

    public override string ToString() => this.type == null ? string.Empty : this.type.Name + this.typeArgs.ToString();

    public override bool Equals(object obj) => obj is TypeArgument other && this.Equals(other);

    public override int GetHashCode() => this.type.GetHashCode() * 397 ^ this.typeArgs.GetHashCode();

    public bool Equals(TypeArgument other) => this.type.Equals((object) other.type) && this.typeArgs.Equals(other.typeArgs);
  }
}
