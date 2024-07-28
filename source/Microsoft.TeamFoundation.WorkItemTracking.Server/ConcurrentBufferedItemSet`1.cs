// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ConcurrentBufferedItemSet`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class ConcurrentBufferedItemSet<T> where T : BufferableItem
  {
    private ConcurrentDictionary<T, T> m_set;
    private int m_count;

    public ConcurrentBufferedItemSet(int maxRecords, IEqualityComparer<T> comparer)
    {
      this.m_set = new ConcurrentDictionary<T, T>(Environment.ProcessorCount * 3, maxRecords, comparer);
      this.m_count = 0;
    }

    public int Count => this.m_count;

    public int Add(T item) => (object) this.m_set.AddOrUpdate(item, item, (Func<T, T, T>) ((k, v) => (T) v.Merge((BufferableItem) item))) == (object) item ? Interlocked.Increment(ref this.m_count) : 0;

    public ICollection<T> Values => this.m_set.Values;
  }
}
