// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.TrieNode`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class TrieNode<V>
  {
    private readonly IDictionary<char, TrieNode<V>> m_childNodes;
    private readonly IList<V> m_values;

    internal TrieNode()
    {
      this.m_childNodes = (IDictionary<char, TrieNode<V>>) new Dictionary<char, TrieNode<V>>();
      this.m_values = (IList<V>) new List<V>();
    }

    internal void Add(string key, V value)
    {
      ArgumentUtility.CheckForNull<string>(key, nameof (key));
      ArgumentUtility.CheckGenericForNull((object) value, nameof (value));
      this.AddInternal(key, value, 0);
    }

    internal virtual IEnumerable<V> Search(string keyPrefix)
    {
      ArgumentUtility.CheckForNull<string>(keyPrefix, nameof (keyPrefix));
      return this.SearchInternal(keyPrefix, 0);
    }

    private void AddInternal(string key, V value, int position)
    {
      ArgumentUtility.CheckForOutOfRange(position, nameof (position), 0, key.Length);
      if (key.Length == position)
      {
        this.m_values.Add(value);
      }
      else
      {
        char key1 = key[position];
        TrieNode<V> trieNode;
        if (!this.m_childNodes.TryGetValue(key1, out trieNode))
        {
          trieNode = new TrieNode<V>();
          this.m_childNodes.Add(key1, trieNode);
        }
        trieNode.AddInternal(key, value, position + 1);
      }
    }

    private IEnumerable<V> SearchInternal(string keyPrefix, int position)
    {
      ArgumentUtility.CheckForOutOfRange(position, nameof (position), 0, keyPrefix.Length);
      if (keyPrefix.Length == position)
        return this.Subtree().SelectMany<TrieNode<V>, V>((Func<TrieNode<V>, IEnumerable<V>>) (node => (IEnumerable<V>) node.m_values));
      TrieNode<V> trieNode;
      return !this.m_childNodes.TryGetValue(keyPrefix[position], out trieNode) ? Enumerable.Empty<V>() : trieNode.SearchInternal(keyPrefix, position + 1);
    }

    private IEnumerable<TrieNode<V>> Subtree() => ((IEnumerable<TrieNode<V>>) new TrieNode<V>[1]
    {
      this
    }).Concat<TrieNode<V>>(this.m_childNodes.Values.SelectMany<TrieNode<V>, TrieNode<V>>((Func<TrieNode<V>, IEnumerable<TrieNode<V>>>) (child => child.Subtree())));
  }
}
