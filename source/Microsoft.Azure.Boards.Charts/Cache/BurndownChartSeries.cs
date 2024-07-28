// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.Cache.BurndownChartSeries
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.Charts.Cache
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BurndownChartSeries : ChartSeriesBase
  {
    public const int CurrentCacheVersion = 2;

    public BurndownChartSeries() => this.CacheVersion = 2;

    [DataMember(Name = "remainingWorkSeries", EmitDefaultValue = true)]
    public List<double?> RemainingWorkSeries { get; set; }

    [DataMember(Name = "queryText", EmitDefaultValue = false)]
    public string QueryText { get; set; }
  }
}
