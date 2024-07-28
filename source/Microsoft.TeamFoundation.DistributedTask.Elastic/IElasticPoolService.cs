// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.IElasticPoolService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  [DefaultServiceImplementation(typeof (ElasticPoolService))]
  public interface IElasticPoolService : IVssFrameworkService
  {
    Task<ElasticPool> AddElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string poolName);

    Task<ElasticPoolCreationResult> CreateElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string poolName,
      bool authorizeAllPipelines = false,
      bool autoProvisionProjectPools = false,
      Guid? projectId = null);

    Task<ElasticPool> UpdateElasticPoolAsync(
      IVssRequestContext requestContext,
      NullableElasticPool elasticPool,
      bool validateReimageOption = false);

    Task DeleteElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      bool deleteVMs);

    Task<ElasticPool> GetElasticPoolAsync(IVssRequestContext requestContext, int poolId);

    Task<IReadOnlyList<ElasticPool>> GetElasticPoolsAsync(IVssRequestContext requestContext);

    Task<IReadOnlyList<ElasticPool>> GetElasticPoolsByTypeAsync(
      IVssRequestContext requestContext,
      OrchestrationType type);

    Task UpdateExtensionAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string poolName);

    Task UpdateElasticPoolTimeStampTagAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      VirtualMachineScaleSet scaleSet);

    Task<VirtualMachineScaleSet> TagElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string poolName = null,
      VirtualMachineScaleSet scaleSet = null);
  }
}
