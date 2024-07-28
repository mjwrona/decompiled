// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.EquatableTaggedUnion`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [DebuggerNonUserCode]
  public class EquatableTaggedUnion<T1, T2> : 
    IEquatable<EquatableTaggedUnion<T1, T2>>,
    ITaggedUnion<T1, T2>
    where T1 : IEquatable<T1>
    where T2 : IEquatable<T2>
  {
    private readonly EquatableTaggedUnionValue<T1, T2> impl;

    public EquatableTaggedUnion(EquatableTaggedUnionValue<T1, T2> toCopy) => this.impl = toCopy;

    public EquatableTaggedUnion(EquatableTaggedUnion<T1, T2> toCopy) => this.impl = toCopy.impl;

    public EquatableTaggedUnion(T1 value) => this.impl = new EquatableTaggedUnionValue<T1, T2>(value);

    public EquatableTaggedUnion(T2 value) => this.impl = new EquatableTaggedUnionValue<T1, T2>(value);

    public void Match(Action<T1> onT1, Action<T2> onT2) => this.impl.Match(onT1, onT2);

    public T Match<T>(Func<T1, T> onT1, Func<T2, T> onT2) => this.impl.Match<T>(onT1, onT2);

    public override string ToString() => this.impl.ToString();

    public bool Equals(EquatableTaggedUnion<T1, T2> other) => (object) other != null && this.impl.Equals(other.impl);

    public override bool Equals(object obj) => this.Equals(obj as EquatableTaggedUnion<T1, T2>);

    public static bool operator ==(EquatableTaggedUnion<T1, T2> r1, EquatableTaggedUnion<T1, T2> r2) => (object) r1 != null ? r1.Equals(r2) : (object) r2 == null;

    public static bool operator !=(EquatableTaggedUnion<T1, T2> r1, EquatableTaggedUnion<T1, T2> r2) => !(r1 == r2);

    public override int GetHashCode() => this.impl.GetHashCode();
  }
}
