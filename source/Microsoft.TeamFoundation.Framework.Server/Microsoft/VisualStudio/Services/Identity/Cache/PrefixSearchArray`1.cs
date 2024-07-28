// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.PrefixSearchArray`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  public class PrefixSearchArray<V> : ISearchable<V>
  {
    private readonly string[] m_keys;
    private readonly V[] m_values;
    private readonly StringComparer m_keyComparer;
    private readonly StringComparison m_keyComparison;

    public PrefixSearchArray(IEnumerable<KeyValuePair<string, V>> keyValuePairs)
      : this(keyValuePairs, StringComparer.OrdinalIgnoreCase, StringComparison.OrdinalIgnoreCase)
    {
    }

    public PrefixSearchArray(
      IEnumerable<KeyValuePair<string, V>> keyValuePairs,
      StringComparer keyComparer,
      StringComparison keyComparison)
    {
      this.m_keyComparer = keyComparer;
      this.m_keyComparison = keyComparison;
      ArgumentUtility.CheckForNull<IEnumerable<KeyValuePair<string, V>>>(keyValuePairs, nameof (keyValuePairs));
      foreach (KeyValuePair<string, V> keyValuePair in keyValuePairs)
      {
        ArgumentUtility.CheckForNull<string>(keyValuePair.Key, "Key");
        ArgumentUtility.CheckGenericForNull((object) keyValuePair.Value, "Value");
      }
      List<KeyValuePair<string, V>> list = keyValuePairs.ToList<KeyValuePair<string, V>>();
      list.Sort(new Comparison<KeyValuePair<string, V>>(this.Comparer));
      this.m_keys = list.Select<KeyValuePair<string, V>, string>((Func<KeyValuePair<string, V>, string>) (x => x.Key)).ToArray<string>();
      this.m_values = list.Select<KeyValuePair<string, V>, V>((Func<KeyValuePair<string, V>, V>) (x => x.Value)).ToArray<V>();
    }

    public IEnumerable<V> Search(string keyPrefix)
    {
      int index = Array.BinarySearch<string>(this.m_keys, keyPrefix, (IComparer<string>) this.m_keyComparer);
      if (index < 0)
        index = ~index;
      if (index < this.m_keys.Length && this.m_keys[index].StartsWith(keyPrefix, this.m_keyComparison))
      {
        int num = index;
        int endAt = index;
        while (num - 1 >= 0 && this.m_keys[num - 1].StartsWith(keyPrefix, this.m_keyComparison))
          --num;
        while (endAt + 1 < this.m_keys.Length && this.m_keys[endAt + 1].StartsWith(keyPrefix, this.m_keyComparison))
          ++endAt;
        for (int j = num; j <= endAt; ++j)
          yield return this.m_values[j];
      }
    }

    public int Count => this.m_keys.Length;

    private int Comparer(KeyValuePair<string, V> pair1, KeyValuePair<string, V> pair2) => this.m_keyComparer.Compare(pair1.Key, pair2.Key);
  }
}
