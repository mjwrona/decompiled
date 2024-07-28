// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QueryPreparationTimes
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  public sealed class QueryPreparationTimes
  {
    internal static readonly QueryPreparationTimes Zero = new QueryPreparationTimes(new TimeSpan(), new TimeSpan(), new TimeSpan(), new TimeSpan());
    private readonly TimeSpan queryCompilationTime;
    private readonly TimeSpan logicalPlanBuildTime;
    private readonly TimeSpan physicalPlanBuildTime;
    private readonly TimeSpan queryOptimizationTime;

    [JsonConstructor]
    internal QueryPreparationTimes(
      TimeSpan queryCompilationTime,
      TimeSpan logicalPlanBuildTime,
      TimeSpan physicalPlanBuildTime,
      TimeSpan queryOptimizationTime)
    {
      this.queryCompilationTime = queryCompilationTime;
      this.logicalPlanBuildTime = logicalPlanBuildTime;
      this.physicalPlanBuildTime = physicalPlanBuildTime;
      this.queryOptimizationTime = queryOptimizationTime;
    }

    internal TimeSpan QueryCompilationTime => this.queryCompilationTime;

    public TimeSpan CompileTime => this.queryCompilationTime;

    public TimeSpan LogicalPlanBuildTime => this.logicalPlanBuildTime;

    public TimeSpan PhysicalPlanBuildTime => this.physicalPlanBuildTime;

    public TimeSpan QueryOptimizationTime => this.queryOptimizationTime;

    internal static QueryPreparationTimes CreateFromDelimitedString(string delimitedString)
    {
      Dictionary<string, double> delimitedString1 = QueryMetricsUtils.ParseDelimitedString(delimitedString);
      return new QueryPreparationTimes(QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "queryCompileTimeInMs"), QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "queryLogicalPlanBuildTimeInMs"), QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "queryPhysicalPlanBuildTimeInMs"), QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "queryOptimizationTimeInMs"));
    }

    internal static QueryPreparationTimes CreateFromIEnumerable(
      IEnumerable<QueryPreparationTimes> queryPreparationTimesList)
    {
      if (queryPreparationTimesList == null)
        throw new ArgumentNullException(nameof (queryPreparationTimesList));
      TimeSpan queryCompilationTime = new TimeSpan();
      TimeSpan logicalPlanBuildTime = new TimeSpan();
      TimeSpan physicalPlanBuildTime = new TimeSpan();
      TimeSpan queryOptimizationTime = new TimeSpan();
      foreach (QueryPreparationTimes preparationTimes in queryPreparationTimesList)
      {
        if (preparationTimes == null)
          throw new ArgumentException("queryPreparationTimesList can not have a null element");
        queryCompilationTime += preparationTimes.queryCompilationTime;
        logicalPlanBuildTime += preparationTimes.logicalPlanBuildTime;
        physicalPlanBuildTime += preparationTimes.physicalPlanBuildTime;
        queryOptimizationTime += preparationTimes.queryOptimizationTime;
      }
      return new QueryPreparationTimes(queryCompilationTime, logicalPlanBuildTime, physicalPlanBuildTime, queryOptimizationTime);
    }
  }
}
