// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.EquatableTaggedUnionValue`4
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [DebuggerNonUserCode]
  public struct EquatableTaggedUnionValue<T1, T2, T3, T4> : 
    IEquatable<EquatableTaggedUnionValue<T1, T2, T3, T4>>,
    ITaggedUnion<T1, T2, T3, T4>
    where T1 : IEquatable<T1>
    where T2 : IEquatable<T2>
    where T3 : IEquatable<T3>
    where T4 : IEquatable<T4>
  {
    private readonly TaggedUnionValue<T1, T2, T3, T4> tagged;

    public EquatableTaggedUnionValue(T1 t1) => this.tagged = new TaggedUnionValue<T1, T2, T3, T4>(t1);

    public EquatableTaggedUnionValue(T2 t2) => this.tagged = new TaggedUnionValue<T1, T2, T3, T4>(t2);

    public EquatableTaggedUnionValue(T3 t3) => this.tagged = new TaggedUnionValue<T1, T2, T3, T4>(t3);

    public EquatableTaggedUnionValue(T4 t4) => this.tagged = new TaggedUnionValue<T1, T2, T3, T4>(t4);

    public bool Equals(EquatableTaggedUnionValue<T1, T2, T3, T4> other) => (int) this.tagged.which == (int) other.tagged.which && this.Match<bool>((Func<T1, bool>) (one => one.Equals(other.tagged.one)), (Func<T2, bool>) (two => two.Equals(other.tagged.two)), (Func<T3, bool>) (three => three.Equals(other.tagged.three)), (Func<T4, bool>) (four => four.Equals(other.tagged.four)));

    public static bool operator ==(
      EquatableTaggedUnionValue<T1, T2, T3, T4> op1,
      EquatableTaggedUnionValue<T1, T2, T3, T4> op2)
    {
      return op1.Equals(op2);
    }

    public static bool operator !=(
      EquatableTaggedUnionValue<T1, T2, T3, T4> op1,
      EquatableTaggedUnionValue<T1, T2, T3, T4> op2)
    {
      return !op1.Equals(op2);
    }

    public override int GetHashCode() => (13 * 397 ^ (int) this.tagged.which) * 397 ^ this.tagged.CallCommonBase<int, object, T1, T2, T3, T4>((Func<object, int>) (o => o.GetHashCode()));

    public override bool Equals(object obj) => obj is EquatableTaggedUnionValue<T1, T2, T3, T4> other && this.Equals(other);

    public void Match(Action<T1> onT1, Action<T2> onT2, Action<T3> onT3, Action<T4> onT4) => this.tagged.Match(onT1, onT2, onT3, onT4);

    public T Match<T>(Func<T1, T> onT1, Func<T2, T> onT2, Func<T3, T> onT3, Func<T4, T> onT4) => this.tagged.Match<T>(onT1, onT2, onT3, onT4);
  }
}
