// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WellKnownPerformanceTimings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public struct WellKnownPerformanceTimings
  {
    public int SqlExecutionTime { get; set; }

    public int SqlExecutionCount { get; set; }

    public int FinalSqlCommandExecutionTime { get; set; }

    public int SqlRetryExecutionTime { get; set; }

    public int SqlRetryExecutionCount { get; set; }

    public int SqlReadOnlyExecutionTime { get; set; }

    public int SqlReadOnlyExecutionCount { get; set; }

    public int RedisExecutionTime { get; set; }

    public int RedisExecutionCount { get; set; }

    public int AadTokenExecutionTime { get; set; }

    public int AadTokenExecutionCount { get; set; }

    public int AadGraphExecutionTime { get; set; }

    public int AadGraphExecutionCount { get; set; }

    public int BlobStorageExecutionTime { get; set; }

    public int BlobStorageExecutionCount { get; set; }

    public int TableStorageExecutionTime { get; set; }

    public int TableStorageExecutionCount { get; set; }

    public int ServiceBusExecutionTime { get; set; }

    public int ServiceBusExecutionCount { get; set; }

    public int VssClientExecutionTime { get; set; }

    public int VssClientExecutionCount { get; set; }

    public int DocDBExecutionTime { get; set; }

    public int DocDBExecutionCount { get; set; }

    public int DocDBRUsConsumed { get; set; }
  }
}
