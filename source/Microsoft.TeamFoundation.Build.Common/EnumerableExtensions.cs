// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.EnumerableExtensions
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.Common
{
  public static class EnumerableExtensions
  {
    public static IEnumerable<IList<TSource>> Buffer<TSource>(
      this IEnumerable<TSource> source,
      int size,
      bool share = false)
    {
      ArgumentValidation.Check(nameof (source), (object) source, false);
      ArgumentValidation.CheckBound(nameof (size), size, 1);
      List<TSource> list = new List<TSource>(size);
      foreach (TSource source1 in source)
      {
        list.Add(source1);
        if (list.Count == size)
        {
          yield return (IList<TSource>) list;
          if (share)
            list.Clear();
          else
            list = new List<TSource>(size);
        }
      }
      if (list.Count > 0)
        yield return (IList<TSource>) list;
    }
  }
}
