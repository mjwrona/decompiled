// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueryStoreOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class QueryStoreOptions : QueryStoreOptionsBase
  {
    public QueryStoreState DesiredState { get; set; }

    public QueryStoreState ActualState { get; set; }

    public int ReadOnlyReason { get; set; }

    public long CurrentStorageSizeMB { get; set; }

    public override string ToString() => string.Format("{0}, DesiredState: {{DesiredState}}, ActualState: {1}, ReadOnlyReason: {2}, CurrentStorageSizeMB: {3}", (object) base.ToString(), (object) this.ActualState, (object) this.ReadOnlyReason, (object) this.CurrentStorageSizeMB);

    public List<KpiMetric> GetQueryStoreMetrics()
    {
      bool flag = this.DesiredState != this.ActualState;
      if (flag && this.ActualState == QueryStoreState.ReadOnly && this.DesiredState == QueryStoreState.ReadWrite && this.ReadOnlyReason == 8)
        flag = false;
      return new List<KpiMetric>()
      {
        new KpiMetric("QueryStoreNotInDesiredState", Convert.ToDouble(flag)),
        new KpiMetric("QueryStoreDesiredState", (double) this.DesiredState),
        new KpiMetric("QueryStoreActualState", (double) this.ActualState),
        new KpiMetric("QueryStoreReadOnlyReason", (double) this.ReadOnlyReason),
        new KpiMetric("QueryStoreCurrentStorageSizeMB", (double) this.CurrentStorageSizeMB),
        new KpiMetric("QueryStoreMaxStorageSizeMB", (double) this.MaxStorageSizeMB),
        new KpiMetric("QueryStoreFlushIntervalSeconds", (double) this.FlushIntervalSeconds),
        new KpiMetric("QueryStoreIntervalLengthMinutes", (double) this.IntervalLengthMinutes),
        new KpiMetric("QueryStoreMaxPlansPerQuery", (double) this.MaxPlansPerQuery),
        new KpiMetric("QueryStoreStaleQueryThresholdDays", (double) this.StaleQueryThresholdDays),
        new KpiMetric("QueryStoreQueryCaptureMode", (double) this.QueryCaptureMode),
        new KpiMetric("QueryStoreSizeBasedCleanupMode", (double) this.SizeBasedCleanupMode)
      };
    }
  }
}
