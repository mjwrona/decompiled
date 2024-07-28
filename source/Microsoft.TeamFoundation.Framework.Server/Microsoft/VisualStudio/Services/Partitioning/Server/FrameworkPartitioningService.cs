// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Partitioning.Server.FrameworkPartitioningService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Partitioning.Server
{
  internal class FrameworkPartitioningService : IPartitioningService, IVssFrameworkService
  {
    private PartitioningCacheService m_cache;
    private static readonly string s_Area = "Partitioning";
    private static readonly string s_Layer = nameof (FrameworkPartitioningService);

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      this.m_cache = requestContext.GetService<PartitioningCacheService>();
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void SavePartitionContainer(
      IVssRequestContext requestContext,
      PartitionContainer container)
    {
      requestContext.TraceEnter(598947, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (SavePartitionContainer));
      try
      {
        ArgumentUtility.CheckForNull<PartitionContainer>(container, nameof (container));
        ArgumentUtility.CheckForEmptyGuid(container.ContainerId, "ContainerId");
        ArgumentUtility.CheckForEmptyGuid(container.ContainerType, "ContainerType");
        ArgumentUtility.CheckStringForNullOrEmpty(container.Name, "Name");
        ArgumentUtility.CheckForOutOfRange(container.MaxPartitions, "MaxPartitions", 0);
        this.GetPartitioningClient(requestContext).CreatePartitionContainerAsync(container, container.ContainerId, cancellationToken: requestContext.CancellationToken).SyncResult();
      }
      finally
      {
        requestContext.TraceLeave(831705, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (SavePartitionContainer));
      }
    }

    public void DeletePartitionContainer(IVssRequestContext requestContext, Guid containerId)
    {
      requestContext.TraceEnter(842191, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (DeletePartitionContainer));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(containerId, nameof (containerId));
        this.GetPartitioningClient(requestContext).DeletePartitionContainerAsync(containerId, cancellationToken: requestContext.CancellationToken).SyncResult();
      }
      finally
      {
        requestContext.TraceLeave(842483, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (DeletePartitionContainer));
      }
    }

    public PartitionContainer GetPartitionContainer(
      IVssRequestContext requestContext,
      Guid containerId)
    {
      requestContext.TraceEnter(271216, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (GetPartitionContainer));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(containerId, nameof (containerId));
        return this.GetPartitioningClient(requestContext).GetPartitionContainerAsync(containerId, cancellationToken: requestContext.CancellationToken).SyncResult<PartitionContainer>();
      }
      finally
      {
        requestContext.TraceLeave(392359, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (GetPartitionContainer));
      }
    }

    public IList<PartitionContainer> QueryPartitionContainers(
      IVssRequestContext requestContext,
      Guid containerType = default (Guid))
    {
      requestContext.TraceEnter(787010, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (QueryPartitionContainers));
      try
      {
        PartitioningHttpClient partitioningClient = this.GetPartitioningClient(requestContext);
        Guid? containerType1 = new Guid?(containerType);
        CancellationToken cancellationToken1 = requestContext.CancellationToken;
        bool? isAcquirable = new bool?();
        CancellationToken cancellationToken2 = cancellationToken1;
        return (IList<PartitionContainer>) partitioningClient.QueryPartitionContainersAsync(containerType1, isAcquirable: isAcquirable, cancellationToken: cancellationToken2).SyncResult<List<PartitionContainer>>();
      }
      finally
      {
        requestContext.TraceLeave(367532, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (QueryPartitionContainers));
      }
    }

    public IList<PartitionContainer> QueryAcquirableContainers(
      IVssRequestContext requestContext,
      Guid containerType,
      string[] requiredTags = null)
    {
      requestContext.TraceEnter(519950, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (QueryAcquirableContainers));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(containerType, nameof (containerType));
        return (IList<PartitionContainer>) this.GetPartitioningClient(requestContext).QueryPartitionContainersAsync(new Guid?(containerType), (IEnumerable<string>) requiredTags, new bool?(true), cancellationToken: requestContext.CancellationToken).SyncResult<List<PartitionContainer>>();
      }
      finally
      {
        requestContext.TraceLeave(944409, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (QueryAcquirableContainers));
      }
    }

    public void CreatePartition<TKey>(
      IVssRequestContext requestContext,
      TKey partitionKey,
      PartitionContainer container)
    {
      requestContext.TraceEnter(72302, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (CreatePartition));
      try
      {
        ArgumentUtility.CheckGenericForNull((object) partitionKey, nameof (partitionKey));
        ArgumentUtility.CheckForNull<PartitionContainer>(container, nameof (container));
        ArgumentUtility.CheckForEmptyGuid(container.ContainerId, "ContainerId");
        ArgumentUtility.CheckForEmptyGuid(container.ContainerType, "ContainerType");
        Partition partition = new Partition()
        {
          PartitionKey = partitionKey.ToString(),
          Container = container
        };
        this.GetPartitioningClient(requestContext).CreatePartitionAsync(partition, partition.PartitionKey, container.ContainerType, cancellationToken: requestContext.CancellationToken).SyncResult();
      }
      finally
      {
        requestContext.TraceLeave(341404, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (CreatePartition));
      }
    }

    public void DeletePartition<TKey>(
      IVssRequestContext requestContext,
      TKey partitionKey,
      Guid containerType)
    {
      requestContext.TraceEnter(195704, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (DeletePartition));
      try
      {
        ArgumentUtility.CheckGenericForNull((object) partitionKey, nameof (partitionKey));
        ArgumentUtility.CheckForEmptyGuid(containerType, nameof (containerType));
        this.GetPartitioningClient(requestContext).DeletePartitionAsync(partitionKey.ToString(), containerType, cancellationToken: requestContext.CancellationToken).SyncResult();
      }
      finally
      {
        requestContext.TraceLeave(563944, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (DeletePartition));
      }
    }

    public Partition QueryPartition<TKey>(
      IVssRequestContext requestContext,
      TKey partitionKey,
      Guid containerType)
    {
      requestContext.TraceEnter(887326, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (QueryPartition));
      try
      {
        ArgumentUtility.CheckGenericForNull((object) partitionKey, nameof (partitionKey));
        ArgumentUtility.CheckForEmptyGuid(containerType, nameof (containerType));
        string partitionKey1 = partitionKey.ToString();
        bool flag = requestContext.RootContext.Items.ContainsKey(FrameworkServerConstants.UsePartitioningCache);
        Partition partition;
        if (flag && this.m_cache.TryGetValue(requestContext, partitionKey1, containerType, out partition))
          return partition;
        partition = this.GetPartitioningClient(requestContext).QueryPartitionAsync(partitionKey1, containerType, cancellationToken: requestContext.CancellationToken).SyncResult<Partition>();
        if (flag && partition != null)
          this.m_cache.Set(requestContext, partitionKey1, containerType, partition);
        return partition;
      }
      finally
      {
        requestContext.TraceLeave(721695, FrameworkPartitioningService.s_Area, FrameworkPartitioningService.s_Layer, nameof (QueryPartition));
      }
    }

    private PartitioningHttpClient GetPartitioningClient(IVssRequestContext requestContext) => requestContext.Elevate().GetClient<PartitioningHttpClient>(ServiceInstanceTypes.MPS);
  }
}
