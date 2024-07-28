// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.CatalogObjectCollection`1
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  internal class CatalogObjectCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : CatalogObject
  {
    public CatalogObjectCollection(ICollection<CatalogObject> items) => this.Items = items;

    public void Add(T item) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(T item) => this.Items.Contains((CatalogObject) item);

    public void CopyTo(T[] array, int arrayIndex) => this.Items.Where<CatalogObject>((Func<CatalogObject, bool>) (i => i.GetType() == typeof (T))).ToArray<CatalogObject>().CopyTo((Array) array, arrayIndex);

    public int Count => this.Items.Where<CatalogObject>((Func<CatalogObject, bool>) (c => typeof (T) == c.GetType())).Count<CatalogObject>();

    public bool Remove(T item) => throw new NotSupportedException();

    public IEnumerator<T> GetEnumerator() => this.Items.Where<CatalogObject>((Func<CatalogObject, bool>) (c => typeof (T) == c.GetType())).Cast<T>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public bool IsReadOnly => true;

    protected ICollection<CatalogObject> Items { get; set; }
  }
}
