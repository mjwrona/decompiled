// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.EventConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [GenerateAllConstants(null)]
  public class EventConstants
  {
    public const string AgentAdded = "MS.TF.DistributedTask.AgentAdded";
    public const string AgentDeleted = "MS.TF.DistributedTask.AgentDeleted";
    public const string AgentRequestAssigned = "MS.TF.DistributedTask.AgentRequestAssigned";
    public const string AgentRequestCompleted = "MS.TF.DistributedTask.AgentRequestCompleted";
    public const string AgentRequestQueued = "MS.TF.DistributedTask.AgentRequestQueued";
    public const string AgentUpdated = "MS.TF.DistributedTask.AgentUpdated";
    public const string AuthorizePipelines = "MS.TF.DistributedTask.AuthorizePipelines";
    public const string DeploymentFailed = "MS.TF.DistributedTask.DeploymentFailed";
    public const string DeploymentGatesChanged = "MS.TF.DistributedTask.DeploymentGatesChanged";
    public const string DeploymentMachinesChanged = "MS.TF.DistributedTask.DeploymentMachinesChanged";
    public const string ElasticAgentPoolResized = "MS.TF.DistributedTask.ElasticAgentPoolResized";
    public const string PoolCreated = "MS.TF.DistributedTask.AgentPoolCreated";
    public const string PoolDeleted = "MS.TF.DistributedTask.AgentPoolDeleted";
    public const string QueueCreated = "MS.TF.DistributedTask.AgentQueueCreated";
    public const string QueueDeleted = "MS.TF.DistributedTask.AgentQueueDeleted";
    public const string QueuesDeleted = "MS.TF.DistributedTask.AgentQueuesDeleted";
    public const string TasksChanged = "MS.TF.DistributedTask.TasksChanged";
    public const string Version = "2.0";
  }
}
