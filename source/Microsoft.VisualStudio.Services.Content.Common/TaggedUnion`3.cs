// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.TaggedUnion`3
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [DebuggerNonUserCode]
  public class TaggedUnion<T1, T2, T3> : ITaggedUnion<T1, T2, T3>
  {
    private readonly TaggedUnionValue<T1, T2, T3> impl;

    public TaggedUnion(T1 value) => this.impl = new TaggedUnionValue<T1, T2, T3>(value);

    public TaggedUnion(T2 value) => this.impl = new TaggedUnionValue<T1, T2, T3>(value);

    public TaggedUnion(T3 value) => this.impl = new TaggedUnionValue<T1, T2, T3>(value);

    public void Match(Action<T1> onT1, Action<T2> onT2, Action<T3> onT3) => this.impl.Match(onT1, onT2, onT3);

    public T Match<T>(Func<T1, T> onT1, Func<T2, T> onT2, Func<T3, T> onT3) => this.impl.Match<T>(onT1, onT2, onT3);

    public override string ToString() => this.impl.ToString();
  }
}
