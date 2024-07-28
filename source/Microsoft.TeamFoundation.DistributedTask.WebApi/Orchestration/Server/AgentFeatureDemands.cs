// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.AgentFeatureDemands
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class AgentFeatureDemands
  {
    public const string BaseAgentVersion = "2.163.1";
    public const string BaseAgentVersionRegistryKey = "/Service/DistributedTask/Pipelines/MinAgentVersion";

    public static DemandSource YamlPipelinesDemandSource() => new DemandSource()
    {
      SourceName = "YAML Pipelines",
      SourceType = DemandSourceType.Feature
    };

    public static DemandMinimumVersion AdvancedCheckoutDemand() => new DemandMinimumVersion(PipelineConstants.AgentVersionDemandName, "2.137.0", new DemandSource()
    {
      SourceName = "Advanced checkout",
      SourceType = DemandSourceType.Feature
    });

    public static DemandMinimumVersion StepTargetVersionDemand() => new DemandMinimumVersion(PipelineConstants.AgentVersionDemandName, "2.162.0", new DemandSource()
    {
      SourceName = "Step targets",
      SourceType = DemandSourceType.Feature
    });

    public static DemandMinimumVersion MultiRepoCheckoutDemand() => new DemandMinimumVersion(PipelineConstants.AgentVersionDemandName, "2.160.0", new DemandSource()
    {
      SourceName = "Multi-repo Checkout",
      SourceType = DemandSourceType.Feature
    });

    public static DemandMinimumVersion Node10TaskDemand() => new DemandMinimumVersion(PipelineConstants.AgentVersionDemandName, "2.144.0", new DemandSource()
    {
      SourceName = "Node 10 task",
      SourceType = DemandSourceType.Feature
    });

    public static DemandMinimumVersion Node14TaskDemand() => new DemandMinimumVersion(PipelineConstants.AgentVersionDemandName, "2.175.1", new DemandSource()
    {
      SourceName = "Node 14 task",
      SourceType = DemandSourceType.Feature
    });

    public static DemandMinimumVersion DecoratorPickupTargetTaskInputsDemand() => new DemandMinimumVersion(PipelineConstants.AgentVersionDemandName, "2.194.0", new DemandSource()
    {
      SourceName = "Picking up of target task inputs by decorators",
      SourceType = DemandSourceType.Feature
    });
  }
}
