// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.ITaskAgentExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public interface ITaskAgentExtension
  {
    Task<(int MaxParallelism, int RequestTimeout)> CheckBillingResourcesAsync(
      IVssRequestContext requestContext,
      int poolId,
      Guid scopeId,
      Guid planId,
      string parallelismTag,
      bool throwException = true);

    Task FilterCapabilitiesAsync(IVssRequestContext requestContext, int poolId, TaskAgent agent);

    Task JobAssignedAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest jobRequest,
      bool isAgentCloudBacked);

    void JobCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest jobRequest,
      bool isAgentCloudBacked);

    Task<Stream> GetAgentPoolMetadataAsync(
      IVssRequestContext requestContext,
      string poolName,
      int? poolMetadataFileId);
  }
}
