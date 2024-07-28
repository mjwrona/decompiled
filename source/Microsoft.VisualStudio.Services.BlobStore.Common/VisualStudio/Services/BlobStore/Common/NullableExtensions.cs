// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.NullableExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class NullableExtensions
  {
    public static void SingleOrDefault<T>(this IEnumerable<T?> e) where T : struct => throw new Exception("SingleOrDefault is non-intuitive for nullables. Use NullableSingleOrNull or SingleOrNull.");

    public static T? NullableSingleOrNull<T>(this IEnumerable<T> e) where T : struct => e.GetEnumerator().NullableSingleOrNull<T>();

    public static T? NullableSingleOrNull<T>(this IEnumerator<T> e) where T : struct => e.MoveNext() ? new T?(e.Current) : new T?();

    public static T SingleOrNull<T>(this IEnumerable<T> e) where T : class => e.GetEnumerator().SingleOrNull<T>();

    public static T SingleOrNull<T>(this IEnumerator<T> e) where T : class => e.MoveNext() ? e.Current : default (T);

    public static T? NullableMaxOrNull<T>(this IEnumerable<T> e) where T : struct => !e.Any<T>() ? new T?() : new T?(e.Max<T>());

    public static T MaxOrDefault<T>(this IEnumerable<T> e) => !e.Any<T>() ? default (T) : e.Max<T>();

    public static TResult? NullableMaxOrNull<T, TResult>(
      this IEnumerable<T> e,
      Func<T, TResult> selector)
      where TResult : struct
    {
      return !e.Any<T>() ? new TResult?() : new TResult?(e.Max<T, TResult>(selector));
    }

    public static TResult MaxOrDefault<T, TResult>(this IEnumerable<T> e, Func<T, TResult> selector) => !e.Any<T>() ? default (TResult) : e.Max<T, TResult>(selector);

    public static T? NullableMinOrNull<T>(this IEnumerable<T> e) where T : struct => !e.Any<T>() ? new T?() : new T?(e.Min<T>());

    public static T MinOrDefault<T>(this IEnumerable<T> e) => !e.Any<T>() ? default (T) : e.Min<T>();

    public static TResult? NullableMinOrNull<T, TResult>(
      this IEnumerable<T> e,
      Func<T, TResult> selector)
      where TResult : struct
    {
      return !e.Any<T>() ? new TResult?() : new TResult?(e.Min<T, TResult>(selector));
    }

    public static TResult MinOrDefault<T, TResult>(this IEnumerable<T> e, Func<T, TResult> selector) => !e.Any<T>() ? default (TResult) : e.Min<T, TResult>(selector);

    public static void AssertValue<T>(this T? nullable, Action<T> action) where T : struct
    {
      if (!nullable.HasValue)
        throw new NullReferenceException("Caller asserted that Nullable<" + typeof (T).FullName + "> has a value, but it does not.");
      action(nullable.Value);
    }

    public static TRet AssertValue<T, TRet>(this T? nullable, Func<T, TRet> action) where T : struct
    {
      if (!nullable.HasValue)
        throw new NullReferenceException("Caller asserted that Nullable<" + typeof (T).FullName + "> has a value, but it does not.");
      return action(nullable.Value);
    }

    public static void IfHasValue<T>(this T? nullable, Action<T> action) where T : struct
    {
      if (!nullable.HasValue)
        return;
      action(nullable.Value);
    }
  }
}
