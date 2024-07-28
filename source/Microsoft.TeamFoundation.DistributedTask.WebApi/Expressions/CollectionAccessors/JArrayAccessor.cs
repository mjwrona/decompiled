// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors.JArrayAccessor
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions.CollectionAccessors
{
  internal sealed class JArrayAccessor : 
    IReadOnlyArray,
    IReadOnlyList<object>,
    IReadOnlyCollection<object>,
    IEnumerable<object>,
    IEnumerable
  {
    private readonly JArray m_jarray;

    public JArrayAccessor(JArray jarray) => this.m_jarray = jarray;

    public int Count => this.m_jarray.Count;

    public object this[int index] => (object) this.m_jarray[index];

    public IEnumerator<object> GetEnumerator() => (IEnumerator<object>) this.m_jarray.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_jarray.GetEnumerator();
  }
}
