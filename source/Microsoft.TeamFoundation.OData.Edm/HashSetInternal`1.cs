// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.HashSetInternal`1
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal class HashSetInternal<T> : IEnumerable<T>, IEnumerable
  {
    private readonly Dictionary<T, object> wrappedDictionary;

    public HashSetInternal() => this.wrappedDictionary = new Dictionary<T, object>();

    public bool Add(T thingToAdd)
    {
      if (this.wrappedDictionary.ContainsKey(thingToAdd))
        return false;
      this.wrappedDictionary[thingToAdd] = (object) null;
      return true;
    }

    public bool Contains(T item) => this.wrappedDictionary.ContainsKey(item);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.wrappedDictionary.Keys.GetEnumerator();

    public void Remove(T item) => this.wrappedDictionary.Remove(item);
  }
}
