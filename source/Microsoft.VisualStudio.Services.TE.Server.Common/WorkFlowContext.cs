// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.WorkFlowContext
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  public class WorkFlowContext
  {
    public DiscoveryData DiscoveryData { get; set; }

    public string EnvironmentUri { get; set; }

    public string InfraFailureExceptionMessage { get; set; }

    public bool IsInfraFailure { get; set; }

    public int NumberOfAgents { get; set; }

    public int NumberOfTestsDiscovered { get; set; }

    public int NumberOfTestsExecuted { get; set; }

    [JsonConverter(typeof (StringEnumConverter))]
    public DistributedTestRunState RunState { get; set; }

    public DateTime TestRunEndedTime { get; set; }

    public int TestRunId { get; set; }

    public DateTime TestRunStatedTime { get; set; }

    public bool HasClientAbortedSlices { get; set; }

    public bool HasServerAbortedSlices { get; set; }

    public string ClientAbortMessage { get; set; }

    public bool TimedOut { get; set; }
  }
}
