// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.AgentCloudType
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  internal class AgentCloudType
  {
    internal static readonly AgentCloudType OneES = new AgentCloudType("1ES Hosted Pool", nameof (OneES), (string) null);
    internal static readonly AgentCloudType MMS = new AgentCloudType("Azure Pipelines", nameof (MMS), "Azure Pipelines");
    internal static readonly AgentCloudType OneBranch = new AgentCloudType("Generic", nameof (OneBranch), "OneBranch Pipelines");
    internal static readonly AgentCloudType Default = new AgentCloudType((string) null, "Other", (string) null);
    private static readonly AgentCloudType[] agentCloudTypes = new AgentCloudType[4]
    {
      AgentCloudType.OneES,
      AgentCloudType.MMS,
      AgentCloudType.OneBranch,
      AgentCloudType.Default
    };
    private readonly string poolType;

    public string AgentCloudName { get; }

    public string DispatcherType { get; }

    private AgentCloudType(string poolType, string dispatcherType, string agentCloudName)
    {
      this.poolType = poolType;
      this.DispatcherType = dispatcherType;
      this.AgentCloudName = agentCloudName;
    }

    internal static AgentCloudType ByPoolType(string poolType)
    {
      foreach (AgentCloudType agentCloudType in AgentCloudType.agentCloudTypes)
      {
        if (agentCloudType.poolType == poolType)
          return agentCloudType;
      }
      return AgentCloudType.Default;
    }
  }
}
