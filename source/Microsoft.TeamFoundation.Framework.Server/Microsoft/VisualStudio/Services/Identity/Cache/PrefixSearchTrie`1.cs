// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.PrefixSearchTrie`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  public class PrefixSearchTrie<V> : ISearchable<V>
  {
    private readonly TrieNode<V> m_root = new TrieNode<V>();
    private readonly bool m_ignoreCase;
    private readonly StringComparer m_keyComparer;

    public PrefixSearchTrie(IEnumerable<KeyValuePair<string, V>> keyValuePairs)
      : this(keyValuePairs, StringComparer.OrdinalIgnoreCase, true)
    {
    }

    public PrefixSearchTrie(
      IEnumerable<KeyValuePair<string, V>> keyValuePairs,
      StringComparer keyComparer,
      bool ignoreCase)
    {
      ArgumentUtility.CheckForNull<IEnumerable<KeyValuePair<string, V>>>(keyValuePairs, nameof (keyValuePairs));
      foreach (KeyValuePair<string, V> keyValuePair in keyValuePairs)
      {
        ArgumentUtility.CheckForNull<string>(keyValuePair.Key, "Key");
        ArgumentUtility.CheckGenericForNull((object) keyValuePair.Value, "Value");
      }
      this.m_keyComparer = keyComparer;
      List<KeyValuePair<string, V>> list = keyValuePairs.ToList<KeyValuePair<string, V>>();
      list.Sort(new Comparison<KeyValuePair<string, V>>(this.Comparer));
      this.m_ignoreCase = ignoreCase;
      foreach (KeyValuePair<string, V> keyValuePair in list)
        this.Add(this.TransformKey(keyValuePair.Key), keyValuePair.Value);
    }

    public IEnumerable<V> Search(string keyPrefix) => this.m_root.Search(this.TransformKey(keyPrefix));

    public int Count { get; private set; }

    public void Add(string key, V value)
    {
      this.m_root.Add(this.TransformKey(key), value);
      ++this.Count;
    }

    private string TransformKey(string key) => !this.m_ignoreCase ? key : key.ToLowerInvariant();

    private int Comparer(KeyValuePair<string, V> pair1, KeyValuePair<string, V> pair2) => this.m_keyComparer.Compare(pair1.Key, pair2.Key);
  }
}
