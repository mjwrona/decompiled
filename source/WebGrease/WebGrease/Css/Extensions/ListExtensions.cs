// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Extensions.ListExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WebGrease.Css.Extensions
{
  public static class ListExtensions
  {
    public static ReadOnlyCollection<T> AsSafeReadOnly<T>(this List<T> list) => list?.AsReadOnly();

    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
      if (list == null || action == null)
        return;
      foreach (T obj in list)
        action(obj);
    }

    public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> action)
    {
      if (list == null || action == null)
        return;
      int num = 0;
      foreach (T obj in list)
      {
        action(obj, num);
        ++num;
      }
    }

    public static void ForEach<T>(this IList<T> list, Action<T, bool> action)
    {
      if (list == null || action == null)
        return;
      int num = 0;
      foreach (T obj in (IEnumerable<T>) list)
      {
        action(obj, num >= list.Count - 1);
        ++num;
      }
    }

    public static ReadOnlyCollection<T> ToSafeReadOnlyCollection<T>(this IEnumerable<T> enumerable) where T : class => enumerable == null ? (ReadOnlyCollection<T>) null : new List<T>(enumerable.Where<T>((Func<T, bool>) (_ => (object) _ != null))).AsReadOnly();
  }
}
