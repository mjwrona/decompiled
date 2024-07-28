// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Partitioning.Server.IPartitioningService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Partitioning.Server
{
  [DefaultServiceImplementation(typeof (FrameworkPartitioningService))]
  public interface IPartitioningService : IVssFrameworkService
  {
    void SavePartitionContainer(IVssRequestContext requestContext, PartitionContainer container);

    void DeletePartitionContainer(IVssRequestContext requestContext, Guid containerId);

    PartitionContainer GetPartitionContainer(IVssRequestContext requestContext, Guid containerId);

    IList<PartitionContainer> QueryPartitionContainers(
      IVssRequestContext requestContext,
      Guid containerType = default (Guid));

    IList<PartitionContainer> QueryAcquirableContainers(
      IVssRequestContext requestContext,
      Guid containerType,
      string[] requiredTags = null);

    void CreatePartition<TKey>(
      IVssRequestContext requestContext,
      TKey partitionKey,
      PartitionContainer container);

    void DeletePartition<TKey>(
      IVssRequestContext requestContext,
      TKey partitionKey,
      Guid containerType);

    Partition QueryPartition<TKey>(
      IVssRequestContext requestContext,
      TKey partitionKey,
      Guid containerType);
  }
}
