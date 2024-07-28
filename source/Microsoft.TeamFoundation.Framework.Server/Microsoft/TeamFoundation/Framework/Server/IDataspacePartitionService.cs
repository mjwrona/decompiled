// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IDataspacePartitionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (DataspacePartitionService))]
  public interface IDataspacePartitionService : IVssFrameworkService
  {
    TComponent CreateComponent<TComponent>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      string partitionKey)
      where TComponent : class, ISqlResourceComponent, new();

    TComponent CreateComponent<TComponent>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid partitionKey)
      where TComponent : class, ISqlResourceComponent, new();

    TComponent CreateReadReplicaAwareComponent<TComponent>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid partitionKey,
      string serviceName)
      where TComponent : class, ISqlResourceComponent, new();

    bool RegisterNotificationAllPartitions(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool filterByAuthor);

    void UnregisterNotificationAllPartitions(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool waitForInFlightNotifications);

    void SaveDataspacePartitionMap(
      IVssRequestContext requestContext,
      DataspacePartitionMap partitionMap);

    void DeleteDataspacePartitionMap(IVssRequestContext requestContext, string dataspaceCategory);

    DataspacePartitionMap GetDataspacePartitionMap(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      bool throwOnMissing);
  }
}
