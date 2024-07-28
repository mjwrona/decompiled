// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.QueryPreparationTimes
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class QueryPreparationTimes
  {
    public static readonly QueryPreparationTimes Zero = new QueryPreparationTimes(new TimeSpan(), new TimeSpan(), new TimeSpan(), new TimeSpan());

    public QueryPreparationTimes(
      TimeSpan queryCompilationTime,
      TimeSpan logicalPlanBuildTime,
      TimeSpan physicalPlanBuildTime,
      TimeSpan queryOptimizationTime)
    {
      this.QueryCompilationTime = queryCompilationTime;
      this.LogicalPlanBuildTime = logicalPlanBuildTime;
      this.PhysicalPlanBuildTime = physicalPlanBuildTime;
      this.QueryOptimizationTime = queryOptimizationTime;
    }

    public TimeSpan QueryCompilationTime { get; }

    public TimeSpan LogicalPlanBuildTime { get; }

    public TimeSpan PhysicalPlanBuildTime { get; }

    public TimeSpan QueryOptimizationTime { get; }

    public ref struct Accumulator
    {
      public Accumulator(
        TimeSpan queryCompliationTime,
        TimeSpan logicalPlanBuildTime,
        TimeSpan physicalPlanBuildTime,
        TimeSpan queryOptimizationTime)
      {
        this.QueryCompilationTime = queryCompliationTime;
        this.LogicalPlanBuildTime = logicalPlanBuildTime;
        this.PhysicalPlanBuildTime = physicalPlanBuildTime;
        this.QueryOptimizationTime = queryOptimizationTime;
      }

      public TimeSpan QueryCompilationTime { get; }

      public TimeSpan LogicalPlanBuildTime { get; }

      public TimeSpan PhysicalPlanBuildTime { get; }

      public TimeSpan QueryOptimizationTime { get; }

      public QueryPreparationTimes.Accumulator Accumulate(
        QueryPreparationTimes queryPreparationTimes)
      {
        if (queryPreparationTimes == null)
          throw new ArgumentNullException(nameof (queryPreparationTimes));
        return new QueryPreparationTimes.Accumulator(this.QueryCompilationTime + queryPreparationTimes.QueryCompilationTime, this.LogicalPlanBuildTime + queryPreparationTimes.LogicalPlanBuildTime, this.PhysicalPlanBuildTime + queryPreparationTimes.PhysicalPlanBuildTime, this.QueryOptimizationTime + queryPreparationTimes.QueryOptimizationTime);
      }

      public static QueryPreparationTimes ToQueryPreparationTimes(
        QueryPreparationTimes.Accumulator accumulator)
      {
        return new QueryPreparationTimes(accumulator.QueryCompilationTime, accumulator.LogicalPlanBuildTime, accumulator.PhysicalPlanBuildTime, accumulator.QueryOptimizationTime);
      }
    }
  }
}
