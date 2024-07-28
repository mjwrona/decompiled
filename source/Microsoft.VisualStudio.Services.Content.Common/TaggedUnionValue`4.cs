// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.TaggedUnionValue`4
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [DebuggerNonUserCode]
  public struct TaggedUnionValue<T1, T2, T3, T4> : ITaggedUnion<T1, T2, T3, T4>
  {
    internal readonly byte which;
    internal readonly T1 one;
    internal readonly T2 two;
    internal readonly T3 three;
    internal readonly T4 four;

    public TaggedUnionValue(TaggedUnionValue<T1, T2, T3, T4> toCopy)
    {
      this.which = toCopy.which;
      this.one = toCopy.one;
      this.two = toCopy.two;
      this.three = toCopy.three;
      this.four = toCopy.four;
    }

    public TaggedUnionValue(T1 value)
    {
      if ((object) value == null)
        throw new ArgumentNullException(nameof (value), "Tagged unions should not hold a null value.");
      this.which = (byte) 0;
      this.one = value;
      this.two = default (T2);
      this.three = default (T3);
      this.four = default (T4);
    }

    public TaggedUnionValue(T2 value)
    {
      if ((object) value == null)
        throw new ArgumentNullException(nameof (value), "Tagged unions should not hold a null value.");
      this.which = (byte) 1;
      this.one = default (T1);
      this.two = value;
      this.three = default (T3);
      this.four = default (T4);
    }

    public TaggedUnionValue(T3 value)
    {
      if ((object) value == null)
        throw new ArgumentNullException(nameof (value), "Tagged unions should not hold a null value.");
      this.which = (byte) 2;
      this.one = default (T1);
      this.two = default (T2);
      this.three = value;
      this.four = default (T4);
    }

    public TaggedUnionValue(T4 value)
    {
      if ((object) value == null)
        throw new ArgumentNullException(nameof (value), "Tagged unions should not hold a null value.");
      this.which = (byte) 3;
      this.one = default (T1);
      this.two = default (T2);
      this.three = default (T3);
      this.four = value;
    }

    public void Match(Action<T1> onT1, Action<T2> onT2, Action<T3> onT3, Action<T4> onT4)
    {
      switch (this.which)
      {
        case 0:
          onT1(this.one);
          break;
        case 1:
          onT2(this.two);
          break;
        case 2:
          onT3(this.three);
          break;
        case 3:
          onT4(this.four);
          break;
        default:
          throw new InvalidOperationException();
      }
    }

    public T Match<T>(Func<T1, T> onT1, Func<T2, T> onT2, Func<T3, T> onT3, Func<T4, T> onT4)
    {
      switch (this.which)
      {
        case 0:
          return onT1(this.one);
        case 1:
          return onT2(this.two);
        case 2:
          return onT3(this.three);
        case 3:
          return onT4(this.four);
        default:
          throw new InvalidOperationException();
      }
    }

    public override string ToString() => this.CallCommonBase<string, object, T1, T2, T3, T4>((Func<object, string>) (x => x.ToString()));
  }
}
