// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.TestInjections
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.Query.Core
{
  internal sealed class TestInjections
  {
    public TestInjections(
      bool simulate429s,
      bool simulateEmptyPages,
      bool enableOptimisticDirectExecution = false,
      TestInjections.ResponseStats responseStats = null)
    {
      this.SimulateThrottles = simulate429s;
      this.SimulateEmptyPages = simulateEmptyPages;
      this.Stats = responseStats;
      this.EnableOptimisticDirectExecution = enableOptimisticDirectExecution;
    }

    public bool EnableOptimisticDirectExecution { get; }

    public bool SimulateThrottles { get; }

    public bool SimulateEmptyPages { get; }

    public TestInjections.ResponseStats Stats { get; }

    public enum PipelineType
    {
      Passthrough,
      Specialized,
      OptimisticDirectExecution,
    }

    public sealed class ResponseStats
    {
      public TestInjections.PipelineType? PipelineType { get; set; }
    }
  }
}
