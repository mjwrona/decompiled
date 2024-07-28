// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.IEnumerableWrapper`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  public class IEnumerableWrapper<T> : IEnumerable<T>, IEnumerable
  {
    private IEnumerable<T> m_baseEnumerable;

    public IEnumerableWrapper(IEnumerable<T> baseEnumerable) => this.m_baseEnumerable = baseEnumerable;

    public IEnumerator<T> GetEnumerator() => this.m_baseEnumerable.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_baseEnumerable.GetEnumerator();

    public void Add(object obj)
    {
    }
  }
}
