// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore.BasicMetaDataStoreReader`1
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore
{
  public abstract class BasicMetaDataStoreReader<T> : 
    IMetaDataStore<T>,
    IMetaDataStore,
    IEnumerable,
    IEnumerable<T>
    where T : IMetaDataStoreItem, new()
  {
    public abstract string this[string key] { get; set; }

    public abstract IList<string> ContentKeys { get; set; }

    public abstract void AddOrUpdate(T item);

    public abstract void Add(string key, T item, bool throwOnDuplicates);

    public abstract int CompleteAdding();

    public abstract IEnumerator<T> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
