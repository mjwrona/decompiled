// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspacePartitionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspacePartitionService : IDataspacePartitionService, IVssFrameworkService
  {
    private ConcurrentDictionary<string, DataspacePartitionMap> m_categoryDataspacePartitionMap = new ConcurrentDictionary<string, DataspacePartitionMap>((IEqualityComparer<string>) StringComparer.Ordinal);
    private INotificationRegistration m_dataspacePartitionRegistration;
    private static readonly string s_Area = nameof (DataspacePartitionService);
    private static readonly string s_Layer = "IVssFrameworkService";

    public void ServiceStart(IVssRequestContext requestContext) => this.m_dataspacePartitionRegistration = requestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(requestContext, "Default", SqlNotificationEventClasses.DataspacePartitionMapChanged, new SqlNotificationHandler(this.OnPartitionMapChanged), true, false);

    public void ServiceEnd(IVssRequestContext requestContext) => this.m_dataspacePartitionRegistration.Unregister(requestContext);

    public TComponent CreateComponent<TComponent>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      string partitionKey)
      where TComponent : class, ISqlResourceComponent, new()
    {
      Guid identifierForKey = this.GetDataspaceIdentifierForKey(requestContext, dataspaceCategory, (IStableHashCode) (StringStableHashCode) partitionKey);
      return this.CreateDatabaseComponent<TComponent>(requestContext, dataspaceCategory, identifierForKey);
    }

    public TComponent CreateComponent<TComponent>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid partitionKey)
      where TComponent : class, ISqlResourceComponent, new()
    {
      Guid identifierForKey = this.GetDataspaceIdentifierForKey(requestContext, dataspaceCategory, (IStableHashCode) (GuidStableHashCode) partitionKey);
      return this.CreateDatabaseComponent<TComponent>(requestContext, dataspaceCategory, identifierForKey);
    }

    public TComponent CreateReadReplicaAwareComponent<TComponent>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid partitionKey,
      string serviceName)
      where TComponent : class, ISqlResourceComponent, new()
    {
      Guid identifierForKey = this.GetDataspaceIdentifierForKey(requestContext, dataspaceCategory, (IStableHashCode) (GuidStableHashCode) partitionKey);
      return this.CreateReadReplicaAwareDatabaseComponent<TComponent>(requestContext, dataspaceCategory, identifierForKey, serviceName);
    }

    public bool RegisterNotificationAllPartitions(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool filterByAuthor)
    {
      DataspacePartitionMap dataspacePartitionMap = this.GetDataspacePartitionMap(requestContext, dataspaceCategory);
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      bool flag = true;
      foreach (Guid dataspaceIdentifier in dataspacePartitionMap.DataspaceIdentifiers)
        flag &= service.RegisterNotification(requestContext, dataspaceCategory, dataspaceIdentifier, eventClass, handler, filterByAuthor);
      return flag;
    }

    public void UnregisterNotificationAllPartitions(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid eventClass,
      SqlNotificationHandler handler,
      bool waitForInFlightNotifications)
    {
      DataspacePartitionMap dataspacePartitionMap = this.GetDataspacePartitionMap(requestContext, dataspaceCategory);
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      foreach (Guid dataspaceIdentifier in dataspacePartitionMap.DataspaceIdentifiers)
        service.UnregisterNotification(requestContext, dataspaceCategory, dataspaceIdentifier, eventClass, handler, waitForInFlightNotifications);
    }

    internal void ExecuteForAllPartitions<TComponent, TElement>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      IEnumerable<TElement> elements,
      Func<TElement, string> keySelector,
      Action<TComponent, List<TElement>> action)
      where TComponent : class, ISqlResourceComponent, new()
    {
      Func<TElement, Guid> getDataspace = (Func<TElement, Guid>) (element => this.GetDataspaceIdentifierForKey(requestContext, dataspaceCategory, (IStableHashCode) (StringStableHashCode) keySelector(element)));
      this.ExecuteForAllPartitionsInternal<TComponent, TElement>(requestContext, dataspaceCategory, elements, getDataspace, action);
    }

    internal void ExecuteForAllPartitions<TComponent, TElement>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      IEnumerable<TElement> elements,
      Func<TElement, Guid> keySelector,
      Action<TComponent, List<TElement>> action)
      where TComponent : class, ISqlResourceComponent, new()
    {
      Func<TElement, Guid> getDataspace = (Func<TElement, Guid>) (element => this.GetDataspaceIdentifierForKey(requestContext, dataspaceCategory, (IStableHashCode) (GuidStableHashCode) keySelector(element)));
      this.ExecuteForAllPartitionsInternal<TComponent, TElement>(requestContext, dataspaceCategory, elements, getDataspace, action);
    }

    private void ExecuteForAllPartitionsInternal<TComponent, TElement>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      IEnumerable<TElement> elements,
      Func<TElement, Guid> getDataspace,
      Action<TComponent, List<TElement>> action)
      where TComponent : class, ISqlResourceComponent, new()
    {
      Dictionary<Guid, List<TElement>> dictionary = new Dictionary<Guid, List<TElement>>();
      foreach (TElement element in elements)
      {
        Guid key = getDataspace(element);
        List<TElement> elementList;
        if (!dictionary.TryGetValue(key, out elementList))
        {
          elementList = new List<TElement>();
          dictionary.Add(key, elementList);
        }
        elementList.Add(element);
      }
      if (dictionary.Count <= 0)
        return;
      foreach (KeyValuePair<Guid, List<TElement>> keyValuePair in dictionary)
      {
        TComponent databaseComponent = this.CreateDatabaseComponent<TComponent>(requestContext, dataspaceCategory, keyValuePair.Key);
        try
        {
          action(databaseComponent, keyValuePair.Value);
        }
        finally
        {
          if ((object) databaseComponent != null)
            databaseComponent.Dispose();
        }
      }
    }

    internal void ExecuteForAllPartitions<TComponent>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Action<IVssRequestContext, TComponent> action)
      where TComponent : TeamFoundationSqlResourceComponent, new()
    {
      foreach (Guid dataspaceIdentifier1 in this.GetDataspacePartitionMap(requestContext, dataspaceCategory, true).DataspaceIdentifiers)
      {
        Guid dataspaceIdentifier = dataspaceIdentifier1;
        GeoReplicationHelper.PerformWrite<TComponent>(requestContext, (Func<TComponent>) (() => this.CreateDatabaseComponent<TComponent>(requestContext, dataspaceCategory, dataspaceIdentifier)), closure_0 ?? (closure_0 = (Action<TComponent>) (component => action(requestContext, component))), nameof (ExecuteForAllPartitions));
      }
    }

    private Guid GetDataspaceIdentifierForKey(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      IStableHashCode partitionKey)
    {
      DataspacePartitionMap dataspacePartitionMap = this.GetDataspacePartitionMap(requestContext, dataspaceCategory, true);
      if (dataspacePartitionMap.Ranges.Length == 1)
        return dataspacePartitionMap.Ranges[0].DataspaceIdentifier;
      int stableHashCode = partitionKey.GetStableHashCode();
      Guid identifierForHash = dataspacePartitionMap.GetDataspaceIdentifierForHash(stableHashCode);
      requestContext.Trace(493338, TraceLevel.Info, DataspacePartitionService.s_Area, DataspacePartitionService.s_Layer, "Received dataspace {0}. PartitionKey: {1}. HashCode: {2}.", (object) identifierForHash, (object) partitionKey, (object) stableHashCode);
      return identifierForHash;
    }

    private TComponent CreateDatabaseComponent<TComponent>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid databaseIdentifier)
      where TComponent : class, ISqlResourceComponent, new()
    {
      return requestContext.GetService<TeamFoundationResourceManagementService>().CreateComponent<TComponent>(requestContext, dataspaceCategory, new Guid?(databaseIdentifier));
    }

    private TComponent CreateReadReplicaAwareDatabaseComponent<TComponent>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid databaseIdentifier,
      string serviceName)
      where TComponent : class, ISqlResourceComponent, new()
    {
      DatabaseConnectionType databaseConnectionType = requestContext.GetService<IVssDatabaseReadReplicaSettingsService>().IsReadReplicaEnabled(requestContext, serviceName) ? DatabaseConnectionType.IntentReadOnly : DatabaseConnectionType.Default;
      return requestContext.GetService<TeamFoundationResourceManagementService>().CreateComponent<TComponent>(requestContext, dataspaceCategory, new Guid?(databaseIdentifier), new DatabaseConnectionType?(databaseConnectionType));
    }

    public DataspacePartitionMap GetDataspacePartitionMap(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      bool throwOnMissing = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(dataspaceCategory, nameof (dataspaceCategory));
      DataspacePartitionMap dataspacePartitionMap;
      if (!this.m_categoryDataspacePartitionMap.TryGetValue(dataspaceCategory, out dataspacePartitionMap))
      {
        using (DataspacePartitionComponent component = requestContext.CreateComponent<DataspacePartitionComponent>())
          dataspacePartitionMap = component.GetDataspacePartitionMap(dataspaceCategory);
        if (dataspacePartitionMap == null)
        {
          if (throwOnMissing)
            throw new TeamFoundationServiceException("Could not find dataspace partition map for category '" + dataspaceCategory + "'.");
          return (DataspacePartitionMap) null;
        }
        dataspacePartitionMap.ValidateAndInitialize();
        dataspacePartitionMap = this.m_categoryDataspacePartitionMap.GetOrAdd(dataspaceCategory, dataspacePartitionMap);
      }
      return dataspacePartitionMap;
    }

    public void SaveDataspacePartitionMap(
      IVssRequestContext requestContext,
      DataspacePartitionMap partitionMap)
    {
      ArgumentUtility.CheckForNull<DataspacePartitionMap>(partitionMap, nameof (partitionMap));
      partitionMap.ValidateAndInitialize();
      this.SaveDataspacePartitionMapAndInvalidateCache(requestContext, partitionMap);
    }

    public void DeleteDataspacePartitionMap(
      IVssRequestContext requestContext,
      string dataspaceCategory)
    {
      DataspacePartitionMap dataspacePartitionMap = this.GetDataspacePartitionMap(requestContext, dataspaceCategory);
      if (dataspacePartitionMap == null)
        return;
      dataspacePartitionMap.Ranges = (DataspaceHashRange[]) null;
      dataspacePartitionMap.Overrides = (DataspacePartitionMapOverride[]) null;
      this.SaveDataspacePartitionMapAndInvalidateCache(requestContext, dataspacePartitionMap);
    }

    private void OnPartitionMapChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      if (string.IsNullOrEmpty(args.Data))
        return;
      this.m_categoryDataspacePartitionMap.TryRemove(args.Data, out DataspacePartitionMap _);
    }

    private void SaveDataspacePartitionMapAndInvalidateCache(
      IVssRequestContext requestContext,
      DataspacePartitionMap partitionMap)
    {
      using (DataspacePartitionComponent component = requestContext.CreateComponent<DataspacePartitionComponent>())
        component.SaveDataspacePartitionMap(partitionMap);
      this.m_categoryDataspacePartitionMap.TryRemove(partitionMap.Category, out DataspacePartitionMap _);
    }
  }
}
