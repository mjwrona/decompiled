// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskRunsOnConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [GenerateAllConstants(null)]
  public class TaskRunsOnConstants
  {
    public const string RunsOnAgent = "Agent";
    public const string RunsOnMachineGroup = "MachineGroup";
    public const string RunsOnDeploymentGroup = "DeploymentGroup";
    public const string RunsOnServer = "Server";
    public const string RunsOnServerGate = "ServerGate";
    public static readonly List<string> DefaultValue = new List<string>()
    {
      "Agent",
      "DeploymentGroup"
    };
    public static readonly List<string> RunsOnAllTypes = new List<string>()
    {
      "Agent",
      "DeploymentGroup",
      "Server"
    };
  }
}
