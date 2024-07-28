// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IDistributedTaskPoolServiceExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class IDistributedTaskPoolServiceExtensions
  {
    public static IList<DeploymentMachine> GetDeploymentMachines(
      this IDistributedTaskPoolService service,
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      IList<string> tagFilters = null,
      DeploymentTargetExpands expands = DeploymentTargetExpands.None,
      bool? enabled = null,
      IList<string> propertyFilters = null)
    {
      DeploymentGroupManager deploymentGroupManager = new DeploymentGroupManager();
      return requestContext.RunSynchronously<IList<DeploymentMachine>>((Func<Task<IList<DeploymentMachine>>>) (() => deploymentGroupManager.GetDeploymentTargetsAsync(requestContext, service, projectId, machineGroupId, tagFilters, expands, enabled, propertyFilters)));
    }

    public static TaskDefinition GetTaskDefinition(
      this IDistributedTaskPoolService service,
      IVssRequestContext requestContext,
      Guid taskId,
      string versionSpec)
    {
      TaskVersion version;
      if (string.IsNullOrEmpty(versionSpec))
      {
        versionSpec = "*";
        version = (TaskVersion) null;
      }
      else
        version = !versionSpec.Contains("*") ? new TaskVersion(versionSpec) : (TaskVersion) null;
      IList<TaskDefinition> taskDefinitions = service.GetTaskDefinitions(requestContext, new Guid?(taskId), version);
      return TaskVersionSpec.Parse(versionSpec).Match((IEnumerable<TaskDefinition>) taskDefinitions);
    }

    public static TaskDefinition GetLatestMajorVersion(
      this IDistributedTaskPoolService poolService,
      IVssRequestContext requestContext,
      Guid taskId)
    {
      TaskDefinition[] array = poolService.GetTaskDefinitions(requestContext, new Guid?(taskId)).OrderByDescending<TaskDefinition, TaskVersion>((Func<TaskDefinition, TaskVersion>) (t => t.Version)).ToArray<TaskDefinition>();
      return ((IEnumerable<TaskDefinition>) array).FirstOrDefault<TaskDefinition>((Func<TaskDefinition, bool>) (x => !x.Preview)) ?? ((IEnumerable<TaskDefinition>) array).FirstOrDefault<TaskDefinition>();
    }
  }
}
