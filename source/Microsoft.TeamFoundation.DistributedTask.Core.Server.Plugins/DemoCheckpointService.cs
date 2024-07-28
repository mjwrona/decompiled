// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.DemoCheckpointService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6486B3F7-B3D2-46E4-8024-05D53FB42B10
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Checkpoints;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins
{
  public class DemoCheckpointService : ICheckpointService, IVssFrameworkService
  {
    public Task<CheckpointDecision> AddCheckpointAsync(
      IVssRequestContext requestContext,
      CheckpointContext context)
    {
      return Task.FromResult<CheckpointDecision>(new CheckpointDecision()
      {
        Id = context.Id,
        Result = "Approved",
        Message = "Approved by default implementation of ICheckpointService"
      });
    }

    public Task<CheckpointDecision> AddCheckpointAsyncImplementation(
      IVssRequestContext requestContext,
      CheckpointContext context)
    {
      return Task.FromResult<CheckpointDecision>(new CheckpointDecision()
      {
        Id = context.Id,
        Result = "Approved",
        Message = "Approved by default implementation of ICheckpointService"
      });
    }

    public Task<bool> CancelCheckpointAsync(
      IVssRequestContext requestContext,
      CheckpointContext context)
    {
      return Task.FromResult<bool>(true);
    }

    public Task<CheckpointDecision> GetCheckpointDecisionAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid checkpointId,
      List<ResourceInfo> jobResources)
    {
      return Task.FromResult<CheckpointDecision>(new CheckpointDecision()
      {
        Id = checkpointId,
        Result = "Approved",
        Message = "Approved by default implementation of ICheckpointService"
      });
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
