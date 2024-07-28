// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Misc.MultiMap`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;

namespace Antlr4.Runtime.Misc
{
  [Serializable]
  internal class MultiMap<K, V> : Dictionary<K, IList<V>>
  {
    private const long serialVersionUID = -4956746660057462312;

    public virtual void Map(K key, V value)
    {
      IList<V> vList;
      if (!this.TryGetValue(key, out vList))
      {
        vList = (IList<V>) new ArrayList<V>();
        this[key] = vList;
      }
      vList.Add(value);
    }

    public virtual IList<Tuple<K, V>> GetPairs()
    {
      IList<Tuple<K, V>> pairs = (IList<Tuple<K, V>>) new ArrayList<Tuple<K, V>>();
      foreach (KeyValuePair<K, IList<V>> keyValuePair in (Dictionary<K, IList<V>>) this)
      {
        foreach (V v in (IEnumerable<V>) keyValuePair.Value)
          pairs.Add(Tuple.Create<K, V>(keyValuePair.Key, v));
      }
      return pairs;
    }
  }
}
