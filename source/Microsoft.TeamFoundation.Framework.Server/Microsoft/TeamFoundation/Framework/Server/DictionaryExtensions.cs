// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DictionaryExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class DictionaryExtensions
  {
    public static Dictionary<TKey, TElement> ToDedupedDictionary<TSource, TKey, TElement>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector,
      IEqualityComparer<TKey> comparer = null)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (keySelector == null)
        throw new ArgumentNullException(nameof (keySelector));
      if (elementSelector == null)
        throw new ArgumentNullException(nameof (elementSelector));
      Dictionary<TKey, TElement> dedupedDictionary = new Dictionary<TKey, TElement>(comparer);
      foreach (TSource source1 in source)
        dedupedDictionary[keySelector(source1)] = elementSelector(source1);
      return dedupedDictionary;
    }
  }
}
