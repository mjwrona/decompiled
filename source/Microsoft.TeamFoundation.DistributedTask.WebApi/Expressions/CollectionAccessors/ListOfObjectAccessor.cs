// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors.ListOfObjectAccessor
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors
{
  internal sealed class ListOfObjectAccessor : 
    IReadOnlyArray,
    IReadOnlyList<object>,
    IReadOnlyCollection<object>,
    IEnumerable<object>,
    IEnumerable
  {
    private readonly IList<object> m_list;

    public ListOfObjectAccessor(IList<object> list) => this.m_list = list;

    public int Count => this.m_list.Count;

    public object this[int index] => this.m_list[index];

    public IEnumerator<object> GetEnumerator() => this.m_list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_list.GetEnumerator();
  }
}
