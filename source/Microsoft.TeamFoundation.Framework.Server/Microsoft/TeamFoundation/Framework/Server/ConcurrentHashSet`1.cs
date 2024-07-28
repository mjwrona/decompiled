// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConcurrentHashSet`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ConcurrentHashSet<T> : IEnumerable<T>, IEnumerable
  {
    private readonly ConcurrentDictionary<T, T> m_dict;

    public ConcurrentHashSet(IEqualityComparer<T> comparer) => this.m_dict = new ConcurrentDictionary<T, T>(1, 31, comparer);

    public bool IsEmpty => this.m_dict.IsEmpty;

    public bool Add(T key) => this.m_dict.TryAdd(key, key);

    public bool Remove(T key) => this.m_dict.TryRemove(key, out T _);

    public void Clear() => this.m_dict.Clear();

    public IEnumerator<T> GetEnumerator()
    {
      foreach (KeyValuePair<T, T> keyValuePair in this.m_dict)
        yield return keyValuePair.Key;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
