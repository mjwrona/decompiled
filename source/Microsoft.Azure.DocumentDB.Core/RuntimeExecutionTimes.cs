// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RuntimeExecutionTimes
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  public sealed class RuntimeExecutionTimes
  {
    internal static readonly RuntimeExecutionTimes Zero = new RuntimeExecutionTimes(new TimeSpan(), new TimeSpan(), new TimeSpan());
    private readonly TimeSpan queryEngineExecutionTime;
    private readonly TimeSpan systemFunctionExecutionTime;
    private readonly TimeSpan userDefinedFunctionExecutionTime;

    [JsonConstructor]
    internal RuntimeExecutionTimes(
      TimeSpan queryEngineExecutionTime,
      TimeSpan systemFunctionExecutionTime,
      TimeSpan userDefinedFunctionExecutionTime)
    {
      this.queryEngineExecutionTime = queryEngineExecutionTime;
      this.systemFunctionExecutionTime = systemFunctionExecutionTime;
      this.userDefinedFunctionExecutionTime = userDefinedFunctionExecutionTime;
    }

    internal TimeSpan QueryEngineExecutionTime => this.queryEngineExecutionTime;

    public TimeSpan SystemFunctionExecutionTime => this.systemFunctionExecutionTime;

    public TimeSpan UserDefinedFunctionExecutionTime => this.userDefinedFunctionExecutionTime;

    public TimeSpan TotalTime => this.queryEngineExecutionTime;

    internal static RuntimeExecutionTimes CreateFromDelimitedString(string delimitedString)
    {
      Dictionary<string, double> delimitedString1 = QueryMetricsUtils.ParseDelimitedString(delimitedString);
      TimeSpan timeSpan1 = QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "VMExecutionTimeInMs");
      TimeSpan timeSpan2 = QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "indexLookupTimeInMs");
      TimeSpan timeSpan3 = QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "documentLoadTimeInMs");
      TimeSpan timeSpan4 = QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "writeOutputTimeInMs");
      TimeSpan timeSpan5 = timeSpan2;
      return new RuntimeExecutionTimes(timeSpan1 - timeSpan5 - timeSpan3 - timeSpan4, QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "systemFunctionExecuteTimeInMs"), QueryMetricsUtils.TimeSpanFromMetrics(delimitedString1, "userFunctionExecuteTimeInMs"));
    }

    internal static RuntimeExecutionTimes CreateFromIEnumerable(
      IEnumerable<RuntimeExecutionTimes> runtimeExecutionTimesList)
    {
      if (runtimeExecutionTimesList == null)
        throw new ArgumentNullException(nameof (runtimeExecutionTimesList));
      TimeSpan queryEngineExecutionTime = new TimeSpan();
      TimeSpan systemFunctionExecutionTime = new TimeSpan();
      TimeSpan userDefinedFunctionExecutionTime = new TimeSpan();
      foreach (RuntimeExecutionTimes runtimeExecutionTimes in runtimeExecutionTimesList)
      {
        queryEngineExecutionTime += runtimeExecutionTimes.queryEngineExecutionTime;
        systemFunctionExecutionTime += runtimeExecutionTimes.systemFunctionExecutionTime;
        userDefinedFunctionExecutionTime += runtimeExecutionTimes.userDefinedFunctionExecutionTime;
      }
      return new RuntimeExecutionTimes(queryEngineExecutionTime, systemFunctionExecutionTime, userDefinedFunctionExecutionTime);
    }
  }
}
