// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.EquatableTaggedUnionValue`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [DebuggerNonUserCode]
  public struct EquatableTaggedUnionValue<T1, T2> : 
    IEquatable<EquatableTaggedUnionValue<T1, T2>>,
    ITaggedUnion<T1, T2>
    where T1 : IEquatable<T1>
    where T2 : IEquatable<T2>
  {
    private readonly TaggedUnionValue<T1, T2> impl;

    public EquatableTaggedUnionValue(T1 t1) => this.impl = new TaggedUnionValue<T1, T2>(t1);

    public EquatableTaggedUnionValue(T2 t2) => this.impl = new TaggedUnionValue<T1, T2>(t2);

    public bool Equals(EquatableTaggedUnionValue<T1, T2> other) => (int) this.impl.which == (int) other.impl.which && this.Match<bool>((Func<T1, bool>) (one => one.Equals(other.impl.one)), (Func<T2, bool>) (two => two.Equals(other.impl.two)));

    public override int GetHashCode() => (13 * 397 ^ (int) this.impl.which) * 397 ^ this.impl.CallCommonBase<int, object, T1, T2>((Func<object, int>) (o => o.GetHashCode()));

    public override bool Equals(object obj) => obj is EquatableTaggedUnionValue<T1, T2> other && this.Equals(other);

    public static bool operator ==(
      EquatableTaggedUnionValue<T1, T2> op1,
      EquatableTaggedUnionValue<T1, T2> op2)
    {
      return op1.Equals(op2);
    }

    public static bool operator !=(
      EquatableTaggedUnionValue<T1, T2> op1,
      EquatableTaggedUnionValue<T1, T2> op2)
    {
      return !op1.Equals(op2);
    }

    public void Match(Action<T1> onT1, Action<T2> onT2) => this.impl.Match(onT1, onT2);

    public T Match<T>(Func<T1, T> onT1, Func<T2, T> onT2) => this.impl.Match<T>(onT1, onT2);

    public override string ToString() => this.impl.ToString();
  }
}
