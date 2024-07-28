// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TrendDataPoint`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class TrendDataPoint<T>
  {
    private Dictionary<T, int> m_valueCounts;

    public TrendDataPoint(DateTime dateTime, IEqualityComparer<T> comparer)
      : this(dateTime, new Dictionary<T, int>(comparer))
    {
    }

    public TrendDataPoint(DateTime dateTime, Dictionary<T, int> valueCounts)
    {
      this.DateTime = dateTime;
      this.m_valueCounts = valueCounts;
    }

    public DateTime DateTime { get; internal set; }

    public IDictionary<T, int> ValueCounts => (IDictionary<T, int>) this.m_valueCounts;

    public IEqualityComparer<T> Comparer => this.m_valueCounts.Comparer;
  }
}
