// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.EquatableTaggedUnion`4
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [DebuggerNonUserCode]
  public class EquatableTaggedUnion<T1, T2, T3, T4> : 
    IEquatable<EquatableTaggedUnion<T1, T2, T3, T4>>,
    ITaggedUnion<T1, T2, T3, T4>
    where T1 : IEquatable<T1>
    where T2 : IEquatable<T2>
    where T3 : IEquatable<T3>
    where T4 : IEquatable<T4>
  {
    private readonly EquatableTaggedUnionValue<T1, T2, T3, T4> impl;

    public EquatableTaggedUnion(EquatableTaggedUnionValue<T1, T2, T3, T4> toCopy) => this.impl = toCopy;

    public EquatableTaggedUnion(EquatableTaggedUnion<T1, T2, T3, T4> toCopy) => this.impl = toCopy.impl;

    public EquatableTaggedUnion(T1 value) => this.impl = new EquatableTaggedUnionValue<T1, T2, T3, T4>(value);

    public EquatableTaggedUnion(T2 value) => this.impl = new EquatableTaggedUnionValue<T1, T2, T3, T4>(value);

    public EquatableTaggedUnion(T3 value) => this.impl = new EquatableTaggedUnionValue<T1, T2, T3, T4>(value);

    public EquatableTaggedUnion(T4 value) => this.impl = new EquatableTaggedUnionValue<T1, T2, T3, T4>(value);

    public void Match(Action<T1> onT1, Action<T2> onT2, Action<T3> onT3, Action<T4> onT4) => this.impl.Match(onT1, onT2, onT3, onT4);

    public T Match<T>(Func<T1, T> onT1, Func<T2, T> onT2, Func<T3, T> onT3, Func<T4, T> onT4) => this.impl.Match<T>(onT1, onT2, onT3, onT4);

    public bool Equals(EquatableTaggedUnion<T1, T2, T3, T4> other) => (object) other != null && this.impl.Equals(other.impl);

    public override bool Equals(object obj) => this.Equals(obj as EquatableTaggedUnion<T1, T2, T3, T4>);

    public static bool operator ==(
      EquatableTaggedUnion<T1, T2, T3, T4> r1,
      EquatableTaggedUnion<T1, T2, T3, T4> r2)
    {
      return (object) r1 != null ? r1.Equals(r2) : (object) r2 == null;
    }

    public static bool operator !=(
      EquatableTaggedUnion<T1, T2, T3, T4> r1,
      EquatableTaggedUnion<T1, T2, T3, T4> r2)
    {
      return !(r1 == r2);
    }

    public override int GetHashCode() => this.impl.GetHashCode();
  }
}
