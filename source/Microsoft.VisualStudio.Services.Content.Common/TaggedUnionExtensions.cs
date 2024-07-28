// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.TaggedUnionExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [DebuggerNonUserCode]
  public static class TaggedUnionExtensions
  {
    public static void CallCommonBase<TBase, T1, T2>(
      this TaggedUnion<T1, T2> tagged,
      Action<TBase> useAction)
      where T1 : TBase
      where T2 : TBase
    {
      useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two)));
    }

    public static T CallCommonBase<T, TBase, T1, T2>(
      this TaggedUnion<T1, T2> tagged,
      Func<TBase, T> useAction)
      where T1 : TBase
      where T2 : TBase
    {
      return useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two)));
    }

    public static void CallCommonBase<TBase, T1, T2, T3>(
      this TaggedUnion<T1, T2, T3> tagged,
      Action<TBase> useAction)
      where T1 : TBase
      where T2 : TBase
      where T3 : TBase
    {
      useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two), (Func<T3, TBase>) (three => (TBase) three)));
    }

    public static T CallCommonBase<T, TBase, T1, T2, T3>(
      this TaggedUnion<T1, T2, T3> tagged,
      Func<TBase, T> useAction)
      where T1 : TBase
      where T2 : TBase
      where T3 : TBase
    {
      return useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two), (Func<T3, TBase>) (three => (TBase) three)));
    }

    public static void CallCommonBase<TBase, T1, T2, T3, T4>(
      this TaggedUnion<T1, T2, T3, T4> tagged,
      Action<TBase> useAction)
      where T1 : TBase
      where T2 : TBase
      where T3 : TBase
      where T4 : TBase
    {
      useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two), (Func<T3, TBase>) (three => (TBase) three), (Func<T4, TBase>) (four => (TBase) four)));
    }

    public static T CallCommonBase<T, TBase, T1, T2, T3, T4>(
      this TaggedUnion<T1, T2, T3, T4> tagged,
      Func<TBase, T> useAction)
      where T1 : TBase
      where T2 : TBase
      where T3 : TBase
      where T4 : TBase
    {
      return useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two), (Func<T3, TBase>) (three => (TBase) three), (Func<T4, TBase>) (four => (TBase) four)));
    }

    public static void CallCommonBase<TBase, T1, T2>(
      this TaggedUnionValue<T1, T2> tagged,
      Action<TBase> useAction)
      where T1 : TBase
      where T2 : TBase
    {
      useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two)));
    }

    public static T CallCommonBase<T, TBase, T1, T2>(
      this TaggedUnionValue<T1, T2> tagged,
      Func<TBase, T> useAction)
      where T1 : TBase
      where T2 : TBase
    {
      return useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two)));
    }

    public static void CallCommonBase<TBase, T1, T2, T3>(
      this TaggedUnionValue<T1, T2, T3> tagged,
      Action<TBase> useAction)
      where T1 : TBase
      where T2 : TBase
      where T3 : TBase
    {
      useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two), (Func<T3, TBase>) (three => (TBase) three)));
    }

    public static T CallCommonBase<T, TBase, T1, T2, T3>(
      this TaggedUnionValue<T1, T2, T3> tagged,
      Func<TBase, T> useAction)
      where T1 : TBase
      where T2 : TBase
      where T3 : TBase
    {
      return useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two), (Func<T3, TBase>) (three => (TBase) three)));
    }

    public static void CallCommonBase<TBase, T1, T2, T3, T4>(
      this TaggedUnionValue<T1, T2, T3, T4> tagged,
      Action<TBase> useAction)
      where T1 : TBase
      where T2 : TBase
      where T3 : TBase
      where T4 : TBase
    {
      useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two), (Func<T3, TBase>) (three => (TBase) three), (Func<T4, TBase>) (four => (TBase) four)));
    }

    public static T CallCommonBase<T, TBase, T1, T2, T3, T4>(
      this TaggedUnionValue<T1, T2, T3, T4> tagged,
      Func<TBase, T> useAction)
      where T1 : TBase
      where T2 : TBase
      where T3 : TBase
      where T4 : TBase
    {
      return useAction(tagged.Match<TBase>((Func<T1, TBase>) (one => (TBase) one), (Func<T2, TBase>) (two => (TBase) two), (Func<T3, TBase>) (three => (TBase) three), (Func<T4, TBase>) (four => (TBase) four)));
    }

    public static bool IsOne<T, T1, T2>(this T tagged) where T : ITaggedUnion<T1, T2>
    {
      ref T local = ref tagged;
      if ((object) default (T) == null)
      {
        T obj = local;
        local = ref obj;
      }
      return local.Match<bool>((Func<T1, bool>) (one => true), (Func<T2, bool>) (two => false));
    }

    public static bool IsOne<T, T1, T2, T3>(this T tagged) where T : ITaggedUnion<T1, T2, T3>
    {
      ref T local = ref tagged;
      if ((object) default (T) == null)
      {
        T obj = local;
        local = ref obj;
      }
      return local.Match<bool>((Func<T1, bool>) (one => true), (Func<T2, bool>) (two => false), (Func<T3, bool>) (three => false));
    }

    public static bool IsOne<T, T1, T2, T3, T4>(this T tagged) where T : ITaggedUnion<T1, T2, T3, T4>
    {
      ref T local = ref tagged;
      if ((object) default (T) == null)
      {
        T obj = local;
        local = ref obj;
      }
      return local.Match<bool>((Func<T1, bool>) (one => true), (Func<T2, bool>) (two => false), (Func<T3, bool>) (three => false), (Func<T4, bool>) (four => false));
    }

    public static bool IsTwo<T, T1, T2>(this T tagged) where T : ITaggedUnion<T1, T2>
    {
      ref T local = ref tagged;
      if ((object) default (T) == null)
      {
        T obj = local;
        local = ref obj;
      }
      return local.Match<bool>((Func<T1, bool>) (one => false), (Func<T2, bool>) (two => true));
    }

    public static bool IsTwo<T, T1, T2, T3>(this T tagged) where T : ITaggedUnion<T1, T2, T3>
    {
      ref T local = ref tagged;
      if ((object) default (T) == null)
      {
        T obj = local;
        local = ref obj;
      }
      return local.Match<bool>((Func<T1, bool>) (one => false), (Func<T2, bool>) (two => true), (Func<T3, bool>) (three => false));
    }

    public static bool IsTwo<T, T1, T2, T3, T4>(this T tagged) where T : ITaggedUnion<T1, T2, T3, T4>
    {
      ref T local = ref tagged;
      if ((object) default (T) == null)
      {
        T obj = local;
        local = ref obj;
      }
      return local.Match<bool>((Func<T1, bool>) (one => false), (Func<T2, bool>) (two => true), (Func<T3, bool>) (three => false), (Func<T4, bool>) (four => false));
    }

    public static bool IsThree<T, T1, T2, T3>(this T tagged) where T : ITaggedUnion<T1, T2, T3>
    {
      ref T local = ref tagged;
      if ((object) default (T) == null)
      {
        T obj = local;
        local = ref obj;
      }
      return local.Match<bool>((Func<T1, bool>) (one => false), (Func<T2, bool>) (two => false), (Func<T3, bool>) (three => true));
    }

    public static bool IsThree<T, T1, T2, T3, T4>(this T tagged) where T : ITaggedUnion<T1, T2, T3, T4>
    {
      ref T local = ref tagged;
      if ((object) default (T) == null)
      {
        T obj = local;
        local = ref obj;
      }
      return local.Match<bool>((Func<T1, bool>) (one => false), (Func<T2, bool>) (two => true), (Func<T3, bool>) (three => true), (Func<T4, bool>) (four => false));
    }

    public static bool IsFour<T, T1, T2, T3, T4>(this T tagged) where T : ITaggedUnion<T1, T2, T3, T4>
    {
      ref T local = ref tagged;
      if ((object) default (T) == null)
      {
        T obj = local;
        local = ref obj;
      }
      return local.Match<bool>((Func<T1, bool>) (one => false), (Func<T2, bool>) (two => true), (Func<T3, bool>) (three => false), (Func<T4, bool>) (four => true));
    }

    public static IEnumerable<T1> SelectOnes<T1, T2>(
      this IEnumerable<ITaggedUnion<T1, T2>> taggedEnumerable)
    {
      return taggedEnumerable.SelectOnes<T1, T2, T1>((Func<T1, T1>) (x => x));
    }

    public static IEnumerable<R> SelectOnes<T1, T2, R>(
      this IEnumerable<ITaggedUnion<T1, T2>> taggedEnumerable,
      Func<T1, R> selector)
    {
      return taggedEnumerable.SelectMany<ITaggedUnion<T1, T2>, R>((Func<ITaggedUnion<T1, T2>, IEnumerable<R>>) (x => (IEnumerable<R>) x.Match<R[]>((Func<T1, R[]>) (one => new R[1]
      {
        selector(one)
      }), (Func<T2, R[]>) (two => new R[0]))));
    }

    public static IEnumerable<T1> SelectOnes<T1, T2, T3>(
      this IEnumerable<ITaggedUnion<T1, T2, T3>> taggedEnumerable)
    {
      return taggedEnumerable.SelectOnes<T1, T2, T3, T1>((Func<T1, T1>) (x => x));
    }

    public static IEnumerable<R> SelectOnes<T1, T2, T3, R>(
      this IEnumerable<ITaggedUnion<T1, T2, T3>> taggedEnumerable,
      Func<T1, R> selector)
    {
      return taggedEnumerable.SelectMany<ITaggedUnion<T1, T2, T3>, R>((Func<ITaggedUnion<T1, T2, T3>, IEnumerable<R>>) (x => (IEnumerable<R>) x.Match<R[]>((Func<T1, R[]>) (one => new R[1]
      {
        selector(one)
      }), (Func<T2, R[]>) (two => new R[0]), (Func<T3, R[]>) (three => new R[0]))));
    }

    public static IEnumerable<T1> SelectOnes<T1, T2, T3, T4>(
      this IEnumerable<ITaggedUnion<T1, T2, T3, T4>> taggedEnumerable)
    {
      return taggedEnumerable.SelectOnes<T1, T2, T3, T4, T1>((Func<T1, T1>) (x => x));
    }

    public static IEnumerable<R> SelectOnes<T1, T2, T3, T4, R>(
      this IEnumerable<ITaggedUnion<T1, T2, T3, T4>> taggedEnumerable,
      Func<T1, R> selector)
    {
      return taggedEnumerable.SelectMany<ITaggedUnion<T1, T2, T3, T4>, R>((Func<ITaggedUnion<T1, T2, T3, T4>, IEnumerable<R>>) (x => (IEnumerable<R>) x.Match<R[]>((Func<T1, R[]>) (one => new R[1]
      {
        selector(one)
      }), (Func<T2, R[]>) (two => new R[0]), (Func<T3, R[]>) (three => new R[0]), (Func<T4, R[]>) (four => new R[0]))));
    }

    public static IEnumerable<T2> SelectTwos<T1, T2>(
      this IEnumerable<ITaggedUnion<T1, T2>> taggedEnumerable)
    {
      return taggedEnumerable.SelectTwos<T1, T2, T2>((Func<T2, T2>) (x => x));
    }

    public static IEnumerable<R> SelectTwos<T1, T2, R>(
      this IEnumerable<ITaggedUnion<T1, T2>> taggedEnumerable,
      Func<T2, R> selector)
    {
      return taggedEnumerable.SelectMany<ITaggedUnion<T1, T2>, R>((Func<ITaggedUnion<T1, T2>, IEnumerable<R>>) (x => (IEnumerable<R>) x.Match<R[]>((Func<T1, R[]>) (one => new R[0]), (Func<T2, R[]>) (two => new R[1]
      {
        selector(two)
      }))));
    }

    public static IEnumerable<T2> SelectTwos<T1, T2, T3>(
      this IEnumerable<ITaggedUnion<T1, T2, T3>> taggedEnumerable)
    {
      return taggedEnumerable.SelectTwos<T1, T2, T3, T2>((Func<T2, T2>) (x => x));
    }

    public static IEnumerable<R> SelectTwos<T1, T2, T3, R>(
      this IEnumerable<ITaggedUnion<T1, T2, T3>> taggedEnumerable,
      Func<T2, R> selector)
    {
      return taggedEnumerable.SelectMany<ITaggedUnion<T1, T2, T3>, R>((Func<ITaggedUnion<T1, T2, T3>, IEnumerable<R>>) (x => (IEnumerable<R>) x.Match<R[]>((Func<T1, R[]>) (one => new R[0]), (Func<T2, R[]>) (two => new R[1]
      {
        selector(two)
      }), (Func<T3, R[]>) (three => new R[0]))));
    }

    public static IEnumerable<T2> SelectTwos<T1, T2, T3, T4>(
      this IEnumerable<ITaggedUnion<T1, T2, T3, T4>> taggedEnumerable)
    {
      return taggedEnumerable.SelectTwos<T1, T2, T3, T4, T2>((Func<T2, T2>) (x => x));
    }

    public static IEnumerable<R> SelectTwos<T1, T2, T3, T4, R>(
      this IEnumerable<ITaggedUnion<T1, T2, T3, T4>> taggedEnumerable,
      Func<T2, R> selector)
    {
      return taggedEnumerable.SelectMany<ITaggedUnion<T1, T2, T3, T4>, R>((Func<ITaggedUnion<T1, T2, T3, T4>, IEnumerable<R>>) (x => (IEnumerable<R>) x.Match<R[]>((Func<T1, R[]>) (one => new R[0]), (Func<T2, R[]>) (two => new R[1]
      {
        selector(two)
      }), (Func<T3, R[]>) (three => new R[0]), (Func<T4, R[]>) (four => new R[0]))));
    }

    public static IEnumerable<T3> SelectThrees<T1, T2, T3>(
      this IEnumerable<ITaggedUnion<T1, T2, T3>> taggedEnumerable)
    {
      return taggedEnumerable.SelectThrees<T1, T2, T3, T3>((Func<T3, T3>) (x => x));
    }

    public static IEnumerable<R> SelectThrees<T1, T2, T3, R>(
      this IEnumerable<ITaggedUnion<T1, T2, T3>> taggedEnumerable,
      Func<T3, R> selector)
    {
      return taggedEnumerable.SelectMany<ITaggedUnion<T1, T2, T3>, R>((Func<ITaggedUnion<T1, T2, T3>, IEnumerable<R>>) (x => (IEnumerable<R>) x.Match<R[]>((Func<T1, R[]>) (one => new R[0]), (Func<T2, R[]>) (two => new R[0]), (Func<T3, R[]>) (three => new R[1]
      {
        selector(three)
      }))));
    }

    public static IEnumerable<T3> SelectThrees<T1, T2, T3, T4>(
      this IEnumerable<ITaggedUnion<T1, T2, T3, T4>> taggedEnumerable)
    {
      return taggedEnumerable.SelectThrees<T1, T2, T3, T4, T3>((Func<T3, T3>) (x => x));
    }

    public static IEnumerable<R> SelectThrees<T1, T2, T3, T4, R>(
      this IEnumerable<ITaggedUnion<T1, T2, T3, T4>> taggedEnumerable,
      Func<T3, R> selector)
    {
      return taggedEnumerable.SelectMany<ITaggedUnion<T1, T2, T3, T4>, R>((Func<ITaggedUnion<T1, T2, T3, T4>, IEnumerable<R>>) (x => (IEnumerable<R>) x.Match<R[]>((Func<T1, R[]>) (one => new R[0]), (Func<T2, R[]>) (two => new R[0]), (Func<T3, R[]>) (three => new R[1]
      {
        selector(three)
      }), (Func<T4, R[]>) (four => new R[0]))));
    }

    public static IEnumerable<T4> SelectFours<T1, T2, T3, T4>(
      this IEnumerable<ITaggedUnion<T1, T2, T3, T4>> taggedEnumerable)
    {
      return taggedEnumerable.SelectFours<T1, T2, T3, T4, T4>((Func<T4, T4>) (x => x));
    }

    public static IEnumerable<R> SelectFours<T1, T2, T3, T4, R>(
      this IEnumerable<ITaggedUnion<T1, T2, T3, T4>> taggedEnumerable,
      Func<T4, R> selector)
    {
      return taggedEnumerable.SelectMany<ITaggedUnion<T1, T2, T3, T4>, R>((Func<ITaggedUnion<T1, T2, T3, T4>, IEnumerable<R>>) (x => (IEnumerable<R>) x.Match<R[]>((Func<T1, R[]>) (one => new R[0]), (Func<T2, R[]>) (two => new R[0]), (Func<T3, R[]>) (three => new R[0]), (Func<T4, R[]>) (four => new R[1]
      {
        selector(four)
      }))));
    }
  }
}
