// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticNodeService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal class ElasticNodeService : IElasticNodeService, IVssFrameworkService
  {
    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public async Task<IReadOnlyList<ElasticNode>> AddElasticNodesAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<ElasticNode> nodes)
    {
      ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
      IReadOnlyList<ElasticNode> elasticNodeList;
      using (ElasticNodeComponent component = requestContext.CreateComponent<ElasticNodeComponent>())
        elasticNodeList = await component.AddElasticNodesAsync(poolId, (IEnumerable<ElasticNode>) nodes);
      return elasticNodeList;
    }

    public async Task DeleteElasticNodesAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<ElasticNode> nodes)
    {
      ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
      using (ElasticNodeComponent component = requestContext.CreateComponent<ElasticNodeComponent>())
        await component.DeleteElasticNodesAsync(poolId, (IEnumerable<ElasticNode>) nodes);
    }

    public async Task<IReadOnlyList<ElasticNode>> GetElasticNodesAsync(
      IVssRequestContext requestContext,
      int poolId,
      ElasticNodeState? state = null)
    {
      ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 1);
      IReadOnlyList<ElasticNode> elasticNodesAsync;
      using (ElasticNodeComponent component = requestContext.CreateComponent<ElasticNodeComponent>())
        elasticNodesAsync = await component.GetElasticNodesAsync(poolId, state);
      return elasticNodesAsync;
    }

    public async Task<IReadOnlyList<ElasticNode>> UpdateElasticNodesAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<ElasticNode> nodes)
    {
      ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
      IReadOnlyList<ElasticNode> elasticNodeList;
      using (ElasticNodeComponent component = requestContext.CreateComponent<ElasticNodeComponent>())
        elasticNodeList = await component.UpdateElasticNodesAsync(poolId, (IEnumerable<ElasticNode>) nodes);
      return elasticNodeList;
    }

    public async Task<ElasticNode> UpdateElasticNodeAsync(
      IVssRequestContext requestContext,
      NullableElasticNode elasticNode)
    {
      ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, elasticNode.PoolId, 2);
      ElasticNode elasticNode1;
      using (ElasticNodeComponent component = requestContext.CreateComponent<ElasticNodeComponent>())
        elasticNode1 = (await component.UpdateElasticNodesAsync(elasticNode.PoolId, (IEnumerable<NullableElasticNode>) new NullableElasticNode[1]
        {
          elasticNode
        })).FirstOrDefault<ElasticNode>();
      return elasticNode1;
    }
  }
}
