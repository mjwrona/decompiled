// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Partitioning.Server.PartitioningCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Partitioning.Server
{
  internal class PartitioningCacheService : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext deploymentContext) => deploymentContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext deploymentContext)
    {
    }

    public bool TryGetValue(
      IVssRequestContext deploymentContext,
      string partitionKey,
      Guid containerType,
      out Partition partition)
    {
      PartitionCacheKey key = new PartitionCacheKey(partitionKey, containerType);
      return deploymentContext.GetService<PartitioningCacheService.NestedPartitioningCacheService>().TryGetValue(deploymentContext, key, out partition);
    }

    public void Set(
      IVssRequestContext deploymentContext,
      string partitionKey,
      Guid containerType,
      Partition partition)
    {
      deploymentContext.CheckDeploymentRequestContext();
      PartitionCacheKey key = new PartitionCacheKey(partitionKey, containerType);
      deploymentContext.GetService<PartitioningCacheService.NestedPartitioningCacheService>().Set(deploymentContext, key, partition);
    }

    public void Remove(
      IVssRequestContext deploymentContext,
      string partitionKey,
      Guid containerType)
    {
      deploymentContext.CheckDeploymentRequestContext();
      PartitionCacheKey key = new PartitionCacheKey(partitionKey, containerType);
      deploymentContext.GetService<PartitioningCacheService.NestedPartitioningCacheService>().Remove(deploymentContext, key);
    }

    public class NestedPartitioningCacheService : 
      LocalAndRedisCache<PartitionCacheKey, Partition, PartitioningCacheService.SecurityToken>
    {
      protected static readonly Guid s_namespace = new Guid("EADCC9C2-85F2-4B82-8B8B-D452621C8DD5");

      public NestedPartitioningCacheService()
        : base(new LocalAndRedisCacheConfiguration().WithRedisNamespace(PartitioningCacheService.NestedPartitioningCacheService.s_namespace).WithMemoryCacheMaxLength(10000))
      {
      }
    }

    public class SecurityToken
    {
    }
  }
}
