// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.IElasticNodeService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  [DefaultServiceImplementation(typeof (ElasticNodeService))]
  public interface IElasticNodeService : IVssFrameworkService
  {
    Task<IReadOnlyList<ElasticNode>> AddElasticNodesAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<ElasticNode> nodes);

    Task DeleteElasticNodesAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<ElasticNode> nodes);

    Task<IReadOnlyList<ElasticNode>> GetElasticNodesAsync(
      IVssRequestContext requestContext,
      int poolId,
      ElasticNodeState? state = null);

    Task<IReadOnlyList<ElasticNode>> UpdateElasticNodesAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<ElasticNode> nodes);

    Task<ElasticNode> UpdateElasticNodeAsync(
      IVssRequestContext requestContext,
      NullableElasticNode elasticNode);
  }
}
