// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseResourceStats
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseResourceStats
  {
    public int DatabaseId { get; set; }

    public DateTime PerfTime { get; set; }

    public Decimal AvgCpuPercent { get; set; }

    public Decimal AvgDataIOPercent { get; set; }

    public Decimal AvgLogWritePercent { get; set; }

    public Decimal AvgMemoryUsagePercent { get; set; }

    public Decimal MaxWorkerPercent { get; set; }

    public DateTime CurrentTime { get; set; }

    public string ServiceObjective { get; set; }

    public int PageLatchAvgTimeMS { get; set; }
  }
}
