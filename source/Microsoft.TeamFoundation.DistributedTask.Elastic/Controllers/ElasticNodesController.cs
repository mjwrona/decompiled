// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.Controllers.ElasticNodesController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "nodes")]
  public sealed class ElasticNodesController : ElasticApiController
  {
    [HttpGet]
    public async Task<IEnumerable<ElasticNode>> GetElasticNodes(int poolId, [FromUri(Name = "$state")] ElasticNodeState? state = null)
    {
      ElasticNodesController elasticNodesController = this;
      return (IEnumerable<ElasticNode>) await elasticNodesController.ElasticNodeService.GetElasticNodesAsync(elasticNodesController.TfsRequestContext, poolId, state);
    }

    [HttpPatch]
    public async Task<ElasticNode> UpdateElasticNode(
      int poolId,
      int elasticNodeId,
      ElasticNodeSettings elasticNodeSettings)
    {
      ElasticNodesController elasticNodesController = this;
      if (elasticNodeSettings.State != ElasticNodeState.PendingDelete && elasticNodeSettings.State != ElasticNodeState.FailedToStartPendingDelete && elasticNodeSettings.State != ElasticNodeState.FailedToRestartPendingDelete && elasticNodeSettings.State != ElasticNodeState.FailedVMPendingDelete)
        throw new ArgumentException(ElasticResources.UpdateElasticNodeStateNotPendingDelete(), "State");
      NullableElasticNode elasticNode = new NullableElasticNode(poolId, elasticNodeId, elasticNodeSettings);
      return await elasticNodesController.ElasticNodeService.UpdateElasticNodeAsync(elasticNodesController.TfsRequestContext, elasticNode);
    }
  }
}
