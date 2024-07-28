// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QDSExpensiveQuery
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class QDSExpensiveQuery
  {
    public string QueryText { get; set; }

    public long QueryId { get; set; }

    public string ObjectName { get; set; }

    public long QueryTextId { get; set; }

    public long PlanId { get; set; }

    public long TotalPhysicalReads { get; set; }

    public long TotalCpuTime { get; set; }

    public long AverageRowCount { get; set; }

    public long TotalExecutions { get; set; }

    public long TotalLogicalReads { get; set; }

    public long AverageCpuTime { get; set; }

    public long TotalAborted { get; set; }

    public long TotalExceptions { get; set; }

    public long TotalLogicalWrites { get; set; }

    public float AverageDop { get; set; }

    public long AverageQueryMaxUsedMemory { get; set; }

    public long QueryHash { get; set; }

    public long QueryPlanHash { get; set; }
  }
}
