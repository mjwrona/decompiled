// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Common.ListWrapperCollection`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.AspNet.OData.Common
{
  internal sealed class ListWrapperCollection<T> : Collection<T>
  {
    private readonly List<T> _items;

    internal ListWrapperCollection()
      : this(new List<T>())
    {
    }

    internal ListWrapperCollection(List<T> list)
      : base((IList<T>) list)
    {
      this._items = list;
    }

    internal List<T> ItemsList => this._items;
  }
}
