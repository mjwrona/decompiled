// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.Controllers.ElasticPoolsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "elasticpools")]
  public sealed class ElasticPoolsController : ElasticApiController
  {
    [HttpPost]
    public async Task<ElasticPoolCreationResult> CreateElasticPool(
      ElasticPool elasticPool,
      [ClientQueryParameter] string poolName,
      [ClientQueryParameter] bool authorizeAllPipelines = false,
      [ClientQueryParameter] bool autoProvisionProjectPools = false,
      [ClientQueryParameter] Guid? projectId = null)
    {
      ElasticPoolsController elasticPoolsController = this;
      ArgumentUtility.CheckForNull<ElasticPool>(elasticPool, nameof (elasticPool));
      elasticPoolsController.TfsRequestContext.CheckHostedDeployment();
      return await elasticPoolsController.ElasticPoolService.CreateElasticPoolAsync(elasticPoolsController.TfsRequestContext, elasticPool, poolName, authorizeAllPipelines, autoProvisionProjectPools, projectId);
    }

    [HttpGet]
    public async Task<ElasticPool> GetElasticPool(int poolId)
    {
      ElasticPoolsController elasticPoolsController = this;
      return await elasticPoolsController.ElasticPoolService.GetElasticPoolAsync(elasticPoolsController.TfsRequestContext, poolId) ?? throw new ElasticPoolDoesNotExistException(ElasticResources.ElasticPoolDoesNotExist((object) poolId));
    }

    [HttpGet]
    public async Task<IEnumerable<ElasticPool>> GetElasticPools()
    {
      ElasticPoolsController elasticPoolsController = this;
      return (IEnumerable<ElasticPool>) await elasticPoolsController.ElasticPoolService.GetElasticPoolsAsync(elasticPoolsController.TfsRequestContext);
    }

    [HttpPatch]
    public async Task<ElasticPool> UpdateElasticPool(
      int poolId,
      ElasticPoolSettings elasticPoolSettings)
    {
      ElasticPoolsController elasticPoolsController = this;
      return await elasticPoolsController.ElasticPoolService.UpdateElasticPoolAsync(elasticPoolsController.TfsRequestContext, new NullableElasticPool(poolId, elasticPoolSettings)
      {
        State = new ElasticPoolState?(ElasticPoolState.Online),
        OfflineSince = new DateTime?()
      }, true);
    }
  }
}
