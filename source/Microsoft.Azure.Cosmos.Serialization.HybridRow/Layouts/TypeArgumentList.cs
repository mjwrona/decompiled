// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.TypeArgumentList
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  [DebuggerDisplay("{this.args == null ? null : ToString()}")]
  public readonly struct TypeArgumentList : IEquatable<TypeArgumentList>
  {
    public static readonly TypeArgumentList Empty = new TypeArgumentList(Array.Empty<TypeArgument>());
    private readonly TypeArgument[] args;
    private readonly SchemaId schemaId;

    public TypeArgumentList(TypeArgument[] args)
    {
      Contract.Requires(args != null);
      this.args = args;
      this.schemaId = SchemaId.Invalid;
    }

    public TypeArgumentList(SchemaId schemaId)
    {
      this.args = Array.Empty<TypeArgument>();
      this.schemaId = schemaId;
    }

    public int Count => this.args.Length;

    public SchemaId SchemaId => this.schemaId;

    public TypeArgument this[int i] => this.args[i];

    [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Constructor")]
    public static implicit operator TypeArgumentList(SchemaId schemaId) => new TypeArgumentList(schemaId);

    public static bool operator ==(TypeArgumentList left, TypeArgumentList right) => left.Equals(right);

    public static bool operator !=(TypeArgumentList left, TypeArgumentList right) => !left.Equals(right);

    public TypeArgumentList.Enumerator GetEnumerator() => new TypeArgumentList.Enumerator(this.args);

    public override string ToString()
    {
      if (this.schemaId != SchemaId.Invalid)
        return string.Format("<{0}>", (object) this.schemaId);
      if (this.args != null)
      {
        TypeArgument[] args = this.args;
        if ((args != null ? (args.Length == 0 ? 1 : 0) : 0) == 0)
          return "<" + string.Join<TypeArgument>(", ", (IEnumerable<TypeArgument>) this.args) + ">";
      }
      return string.Empty;
    }

    public override bool Equals(object obj) => obj is TypeArgumentList other && this.Equals(other);

    public override int GetHashCode()
    {
      int hashCode = 19 * 397 ^ this.schemaId.GetHashCode();
      foreach (TypeArgument typeArgument in this.args)
        hashCode = hashCode * 397 ^ typeArgument.GetHashCode();
      return hashCode;
    }

    public bool Equals(TypeArgumentList other) => this.schemaId == other.schemaId && ((IEnumerable<TypeArgument>) this.args).SequenceEqual<TypeArgument>((IEnumerable<TypeArgument>) other.args);

    public struct Enumerator
    {
      private readonly TypeArgument[] list;
      private int index;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      internal Enumerator(TypeArgument[] list)
      {
        this.list = list;
        this.index = -1;
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool MoveNext()
      {
        int num = checked (this.index + 1);
        if (num >= this.list.Length)
          return false;
        this.index = num;
        return true;
      }

      public readonly ref readonly TypeArgument Current
      {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref this.list[this.index];
      }
    }
  }
}
