// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.RuntimeExecutionTimes
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal sealed class RuntimeExecutionTimes
  {
    public static readonly RuntimeExecutionTimes Empty = new RuntimeExecutionTimes(new TimeSpan(), new TimeSpan(), new TimeSpan());

    public RuntimeExecutionTimes(
      TimeSpan queryEngineExecutionTime,
      TimeSpan systemFunctionExecutionTime,
      TimeSpan userDefinedFunctionExecutionTime)
    {
      this.QueryEngineExecutionTime = queryEngineExecutionTime;
      this.SystemFunctionExecutionTime = systemFunctionExecutionTime;
      this.UserDefinedFunctionExecutionTime = userDefinedFunctionExecutionTime;
    }

    public TimeSpan QueryEngineExecutionTime { get; }

    public TimeSpan SystemFunctionExecutionTime { get; }

    public TimeSpan UserDefinedFunctionExecutionTime { get; }

    public ref struct Accumulator
    {
      public Accumulator(
        TimeSpan queryEngineExecutionTime,
        TimeSpan systemFunctionExecutionTime,
        TimeSpan userDefinedFunctionExecutionTimes)
      {
        this.QueryEngineExecutionTime = queryEngineExecutionTime;
        this.SystemFunctionExecutionTime = systemFunctionExecutionTime;
        this.UserDefinedFunctionExecutionTime = userDefinedFunctionExecutionTimes;
      }

      public TimeSpan QueryEngineExecutionTime { get; set; }

      public TimeSpan SystemFunctionExecutionTime { get; set; }

      public TimeSpan UserDefinedFunctionExecutionTime { get; set; }

      public RuntimeExecutionTimes.Accumulator Accumulate(
        RuntimeExecutionTimes runtimeExecutionTimes)
      {
        if (runtimeExecutionTimes == null)
          throw new ArgumentNullException(nameof (runtimeExecutionTimes));
        return new RuntimeExecutionTimes.Accumulator(this.QueryEngineExecutionTime + runtimeExecutionTimes.QueryEngineExecutionTime, this.SystemFunctionExecutionTime + runtimeExecutionTimes.SystemFunctionExecutionTime, this.UserDefinedFunctionExecutionTime + runtimeExecutionTimes.UserDefinedFunctionExecutionTime);
      }

      public static RuntimeExecutionTimes ToRuntimeExecutionTimes(
        RuntimeExecutionTimes.Accumulator accumulator)
      {
        return new RuntimeExecutionTimes(accumulator.QueryEngineExecutionTime, accumulator.SystemFunctionExecutionTime, accumulator.UserDefinedFunctionExecutionTime);
      }
    }
  }
}
