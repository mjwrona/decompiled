// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CachedRegistryService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.RegistryService.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class CachedRegistryService : 
    SqlRegistryService,
    ICachedRegistryService,
    IVssRegistryService,
    IVssFrameworkService,
    IInternalCachedRegistryService
  {
    private static readonly RegistryQuery[] s_registryCacheRoots;
    private const string c_area = "Registry";
    private const string c_layer = "CachedRegistryService";
    private bool m_hasParent;
    private ILockName m_cacheLockName;
    private long m_sequenceId;
    private List<KeyValuePair<string, PathTable<string>>> m_registryCache;
    private object m_notificationsLock;
    private List<IList<RegistryUpdateRecord>> m_enqueuedRegistryUpdates;
    private RegistryNotificationSet m_notificationSet = new RegistryNotificationSet();
    private INotificationRegistration m_registrySettingsRegistration;
    private static readonly VssPerformanceCounter s_registryCacheHitsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_RegistryCacheHitsPerSec");
    private static readonly VssPerformanceCounter s_deploymentRegistryCacheHitsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_DeploymentRegistryCacheHitsPerSec");
    private static readonly VssPerformanceCounter s_deploymentRegistryEntriesTotal = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_DeploymentRegistryCacheTotal");
    private static readonly VssPerformanceCounter s_deploymentRegistrySubscriptionsTotal = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_DeploymentRegistrySubscriptionsTotal");
    private static readonly RegistryEntryCollection s_dummyCollection = new RegistryEntryCollection("/", (IEnumerable<RegistryEntry>) new RegistryEntry[1]
    {
      new RegistryEntry(string.Empty, string.Empty)
    });

    static CachedRegistryService()
    {
      string[] strArray = new string[10]
      {
        "/Configuration",
        "/ConfigFramework",
        "/FeatureAvailability",
        "/Plugins",
        "/Service",
        "/Diagnostics",
        "/WebAccess",
        "/BusinessPolicy",
        "/OrgId",
        "/Testing"
      };
      SparseTree<string> sparseTree = new SparseTree<string>('/', StringComparison.OrdinalIgnoreCase);
      foreach (string str in strArray)
      {
        sparseTree.EnumAndEvaluateParents(str, EnumParentsOptions.None, (SparseTree<string>.EnumNodeCallback) ((token, root, noChildrenBelow, isExactMatch) =>
        {
          throw new InvalidOperationException();
        }));
        if (sparseTree.Remove(str, true))
          throw new InvalidOperationException();
        sparseTree.Add(str, str);
      }
      CachedRegistryService.s_registryCacheRoots = sparseTree.EnumSubTreeReferencedObjects((string) null, EnumSubTreeOptions.None, int.MaxValue).Select<string, RegistryQuery>((Func<string, RegistryQuery>) (s => new RegistryQuery(s, (string) null, int.MaxValue))).ToArray<RegistryQuery>();
    }

    public override void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1694849179, "Registry", nameof (CachedRegistryService), nameof (ServiceStart));
      try
      {
        base.ServiceStart(requestContext);
        this.m_notificationsLock = new object();
        this.m_cacheLockName = this.CreateLockName(requestContext, "cache");
        this.m_hasParent = this.GetParent(requestContext) != null;
        this.m_enqueuedRegistryUpdates = new List<IList<RegistryUpdateRecord>>();
        this.m_registrySettingsRegistration = requestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(requestContext, "Default", SqlNotificationEventClasses.RegistrySettingsChanged, new SqlNotificationCallback(this.OnRegistrySettingsChanged), false, false);
        requestContext.Trace(97053, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Initial load: starting");
        IEnumerable<PathTable<string>> second = this.Read(requestContext, (IEnumerable<RegistryQuery>) CachedRegistryService.s_registryCacheRoots, out this.m_sequenceId);
        this.m_registryCache = ((IEnumerable<RegistryQuery>) CachedRegistryService.s_registryCacheRoots).Zip<RegistryQuery, PathTable<string>, KeyValuePair<string, PathTable<string>>>(second, (Func<RegistryQuery, PathTable<string>, KeyValuePair<string, PathTable<string>>>) ((a, b) => new KeyValuePair<string, PathTable<string>>(a.Path, b))).ToList<KeyValuePair<string, PathTable<string>>>();
        int num = this.m_registryCache.Sum<KeyValuePair<string, PathTable<string>>>((Func<KeyValuePair<string, PathTable<string>>, int>) (s => s.Value == null ? 0 : s.Value.Count));
        requestContext.Trace(97054, num > 100000 ? TraceLevel.Error : TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Initial load: loaded {0} items", (object) num);
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          CachedRegistryService.s_deploymentRegistryEntriesTotal.SetValue((long) num);
        lock (this.m_notificationsLock)
        {
          if (this.m_sequenceId > 0L && this.m_enqueuedRegistryUpdates.Count > 0 && this.m_enqueuedRegistryUpdates.First<IList<RegistryUpdateRecord>>().Any<RegistryUpdateRecord>((Func<RegistryUpdateRecord, bool>) (s => s.SequenceId == 0L)))
            this.m_sequenceId = 0L;
          foreach (IList<RegistryUpdateRecord> enqueuedRegistryUpdate in this.m_enqueuedRegistryUpdates)
            this.UpdateRegistryCache(requestContext, enqueuedRegistryUpdate);
          this.m_enqueuedRegistryUpdates = (List<IList<RegistryUpdateRecord>>) null;
        }
      }
      finally
      {
        requestContext.TraceLeave(1613601356, "Registry", nameof (CachedRegistryService), nameof (ServiceStart));
      }
    }

    public override void ServiceEnd(IVssRequestContext requestContext)
    {
      this.m_registrySettingsRegistration.Unregister(requestContext);
      if (this.m_hasParent)
      {
        foreach (RegistryCallbackEntry notification in this.m_notificationSet)
        {
          if (notification.IsFallThru)
          {
            string message = "A fall-thru registry notification was not unregistered prior to service host shutdown: " + notification.Callback.ToString();
            requestContext.Trace(97060, TraceLevel.Error, "Registry", nameof (CachedRegistryService), message);
          }
        }
      }
      base.ServiceEnd(requestContext);
    }

    public override IVssRegistryService GetParent(IVssRequestContext requestContext) => (IVssRegistryService) requestContext.GetParentService<ICachedRegistryService>();

    public override void RegisterNotification(
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      bool fallThru,
      IEnumerable<RegistryQuery> filters,
      Guid serviceHostId = default (Guid))
    {
      if (!requestContext.ServiceHost.HasDatabaseAccess)
        return;
      ArgumentUtility.CheckForNull<RegistrySettingsChangedCallback>(callback, nameof (callback));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) filters, nameof (filters));
      if (Guid.Empty == serviceHostId)
        serviceHostId = requestContext.ServiceHost.InstanceId;
      int count;
      lock (this.m_notificationsLock)
      {
        this.m_notificationSet.AddNotification(callback, serviceHostId, filters, fallThru);
        count = this.m_notificationSet.Count;
      }
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        CachedRegistryService.s_deploymentRegistrySubscriptionsTotal.SetValue((long) count);
      if (!fallThru)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Parent);
      if (vssRequestContext == null)
        return;
      vssRequestContext.GetService<ICachedRegistryService>().RegisterNotification(vssRequestContext, callback, true, filters, serviceHostId);
    }

    public override void UnregisterNotification(
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback)
    {
      ArgumentUtility.CheckForNull<RegistrySettingsChangedCallback>(callback, nameof (callback));
      bool isFallThru;
      int count;
      lock (this.m_notificationsLock)
      {
        this.m_notificationSet.RemoveNotification(callback, out isFallThru);
        count = this.m_notificationSet.Count;
      }
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        CachedRegistryService.s_deploymentRegistrySubscriptionsTotal.SetValue((long) count);
      if (!isFallThru)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Parent);
      if (vssRequestContext == null)
        return;
      vssRequestContext.GetService<ICachedRegistryService>().UnregisterNotification(vssRequestContext, callback);
    }

    public override void Write(IVssRequestContext requestContext, IEnumerable<RegistryItem> items)
    {
      requestContext.TraceEnter(97070, "Registry", nameof (CachedRegistryService), nameof (Write));
      try
      {
        long sequenceId;
        using (requestContext.AcquireReaderLock(this.m_cacheLockName))
          sequenceId = this.m_sequenceId;
        IList<RegistryUpdateRecord> updateRecords;
        long newSequenceId = this.Write(requestContext, sequenceId == 0L ? long.MaxValue : sequenceId, items, out updateRecords);
        if (sequenceId == 0L)
          updateRecords = (IList<RegistryUpdateRecord>) items.Select<RegistryItem, RegistryUpdateRecord>((Func<RegistryItem, RegistryUpdateRecord>) (s => new RegistryUpdateRecord(newSequenceId, s))).ToList<RegistryUpdateRecord>();
        this.UpdateRegistryCache(requestContext, updateRecords);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(97072, "Registry", nameof (CachedRegistryService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(97071, "Registry", nameof (CachedRegistryService), nameof (Write));
      }
    }

    public override IEnumerable<RegistryItem> Read(
      IVssRequestContext requestContext,
      in RegistryQuery query)
    {
      bool flag = requestContext.IsTracing(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService));
      if (flag)
        requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Incoming registry query: {0}", (object) query.ToString());
      int registryCacheIndex = this.GetRegistryCacheIndex(query.Path);
      if (registryCacheIndex >= 0)
      {
        if (flag)
          requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Path {0} falls in cached space; will return a cached result", (object) query.Path);
        ICollection<RegistryItem> source;
        using (requestContext.AcquireReaderLock(this.m_cacheLockName))
          source = this.CacheRead(requestContext, registryCacheIndex, in query);
        if (flag)
        {
          requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Cached result for query {0} returning {1} records", (object) query.ToString(), (object) source.Count);
          if (source.Count > 0)
          {
            RegistryItem registryItem = source.First<RegistryItem>();
            requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "First result from query {0} is {1} => {2}", (object) query.ToString(), (object) registryItem.Path, (object) (registryItem.Value ?? string.Empty));
          }
        }
        CachedRegistryService.s_registryCacheHitsPerSec.Increment();
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          CachedRegistryService.s_deploymentRegistryCacheHitsPerSec.Increment();
        return (IEnumerable<RegistryItem>) source;
      }
      if (flag)
        requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Path {0} is not in cached space; will call the database", (object) query.Path);
      IEnumerable<RegistryItem> source1 = base.Read(requestContext, in query);
      if (flag)
      {
        RegistryItem registryItem = source1.FirstOrDefault<RegistryItem>();
        if (registryItem.Path != null)
        {
          requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Uncached result for query {0} returning at least 1 record", (object) query.ToString());
          requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "First result from query {0} is {1} => {2}", (object) query.ToString(), (object) registryItem.Path, (object) (registryItem.Value ?? string.Empty));
        }
        else
          requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Uncached result for query {0} returning 0 records", (object) query.ToString());
      }
      return source1;
    }

    public override IEnumerable<IEnumerable<RegistryItem>> Read(
      IVssRequestContext requestContext,
      IEnumerable<RegistryQuery> queries)
    {
      bool flag = requestContext.IsTracing(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService));
      if (flag)
      {
        int num = 0;
        requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Incoming registry multiquery: {0} queries", (object) queries.Count<RegistryQuery>().ToString());
        foreach (RegistryQuery query in queries)
          requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Query #{0}: {1}", (object) num++, (object) query.ToString());
      }
      IEnumerable<CachedRegistryService.QueryResultWithIndex> first = (IEnumerable<CachedRegistryService.QueryResultWithIndex>) CachedRegistryService.QueryResultWithIndex.EmptyArray;
      IEnumerable<CachedRegistryService.RegistryQueryWithIndexes> source = this.FilterQueriesByCacheState(queries, true);
      if (source.Any<CachedRegistryService.RegistryQueryWithIndexes>())
      {
        if (flag)
          requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "{0} queries will return cached results", (object) source.Count<CachedRegistryService.RegistryQueryWithIndexes>());
        List<CachedRegistryService.QueryResultWithIndex> queryResultWithIndexList = new List<CachedRegistryService.QueryResultWithIndex>();
        using (requestContext.AcquireReaderLock(this.m_cacheLockName))
        {
          foreach (CachedRegistryService.RegistryQueryWithIndexes queryWithIndexes in source)
          {
            RegistryQuery query = queryWithIndexes.Query;
            ICollection<RegistryItem> registryItems = this.CacheRead(requestContext, queryWithIndexes.CacheIndex, in query);
            queryResultWithIndexList.Add(new CachedRegistryService.QueryResultWithIndex(queryWithIndexes.QueryIndex, (IEnumerable<RegistryItem>) registryItems));
            if (flag)
            {
              requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Cached result for query {0} returning {1} records", (object) query.ToString(), (object) registryItems.Count);
              if (registryItems.Count > 0)
              {
                RegistryItem registryItem = registryItems.First<RegistryItem>();
                requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "First result from query {0} is {1} => {2}", (object) query.ToString(), (object) registryItem.Path, (object) (registryItem.Value ?? string.Empty));
              }
            }
          }
        }
        CachedRegistryService.s_registryCacheHitsPerSec.Increment();
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          CachedRegistryService.s_deploymentRegistryCacheHitsPerSec.Increment();
        first = (IEnumerable<CachedRegistryService.QueryResultWithIndex>) queryResultWithIndexList;
      }
      IEnumerable<CachedRegistryService.QueryResultWithIndex> queryResultWithIndexes = (IEnumerable<CachedRegistryService.QueryResultWithIndex>) CachedRegistryService.QueryResultWithIndex.EmptyArray;
      IEnumerable<CachedRegistryService.RegistryQueryWithIndexes> queryWithIndexeses = this.FilterQueriesByCacheState(queries, false);
      if (queryWithIndexeses.Any<CachedRegistryService.RegistryQueryWithIndexes>())
      {
        if (flag)
          requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "{0} queries will return uncached results", (object) queryResultWithIndexes.Count<CachedRegistryService.QueryResultWithIndex>());
        queryResultWithIndexes = (IEnumerable<CachedRegistryService.QueryResultWithIndex>) queryWithIndexeses.Zip<CachedRegistryService.RegistryQueryWithIndexes, IEnumerable<RegistryItem>, CachedRegistryService.QueryResultWithIndex>(base.Read(requestContext, queryWithIndexeses.Select<CachedRegistryService.RegistryQueryWithIndexes, RegistryQuery>((Func<CachedRegistryService.RegistryQueryWithIndexes, RegistryQuery>) (s => s.Query))), (Func<CachedRegistryService.RegistryQueryWithIndexes, IEnumerable<RegistryItem>, CachedRegistryService.QueryResultWithIndex>) ((a, b) => new CachedRegistryService.QueryResultWithIndex(a.QueryIndex, b))).ToList<CachedRegistryService.QueryResultWithIndex>();
        if (flag)
        {
          foreach (CachedRegistryService.QueryResultWithIndex queryResultWithIndex in queryResultWithIndexes)
          {
            RegistryQuery registryQuery = queries.ElementAt<RegistryQuery>(queryResultWithIndex.QueryIndex);
            RegistryItem registryItem = queryResultWithIndex.Results.FirstOrDefault<RegistryItem>();
            if (registryItem.Path != null)
            {
              requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Uncached result for query {0} returning at least 1 record", (object) registryQuery.ToString());
              requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "First result from query {0} is {1} => {2}", (object) registryQuery.ToString(), (object) registryItem.Path, (object) (registryItem.Value ?? string.Empty));
            }
            else
              requestContext.Trace(97200, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "Uncached result for query {0} returning 0 records", (object) registryQuery.ToString());
          }
        }
      }
      return first.Merge<CachedRegistryService.QueryResultWithIndex>(queryResultWithIndexes, (Func<CachedRegistryService.QueryResultWithIndex, CachedRegistryService.QueryResultWithIndex, int>) ((a, b) => a.QueryIndex - b.QueryIndex)).Select<CachedRegistryService.QueryResultWithIndex, IEnumerable<RegistryItem>>((Func<CachedRegistryService.QueryResultWithIndex, IEnumerable<RegistryItem>>) (s => s.Results));
    }

    private IEnumerable<CachedRegistryService.RegistryQueryWithIndexes> FilterQueriesByCacheState(
      IEnumerable<RegistryQuery> queries,
      bool cached)
    {
      int i = 0;
      foreach (RegistryQuery query in queries)
      {
        int registryCacheIndex = this.GetRegistryCacheIndex(query.Path);
        if (registryCacheIndex >= 0 == cached)
          yield return new CachedRegistryService.RegistryQueryWithIndexes(i, query, registryCacheIndex);
        ++i;
      }
    }

    private ICollection<RegistryItem> CacheRead(
      IVssRequestContext requestContext,
      int cacheIndex,
      in RegistryQuery query)
    {
      PathTable<string> pathTable = this.m_registryCache[cacheIndex].Value;
      ICollection<RegistryItem> registryItems;
      if (pathTable == null)
        registryItems = (ICollection<RegistryItem>) RegistryItem.EmptyArray;
      else if (query.Depth == 0)
      {
        string referencedObject;
        if (!pathTable.TryGetValue(query.Path, out referencedObject))
          registryItems = (ICollection<RegistryItem>) RegistryItem.EmptyArray;
        else
          registryItems = (ICollection<RegistryItem>) new RegistryItem[1]
          {
            new RegistryItem(query.Path, referencedObject)
          };
      }
      else
      {
        List<RegistryItem> registryItemList = new List<RegistryItem>();
        foreach (PathTableTokenAndValue<string> tableTokenAndValue in pathTable.EnumSubTree(query.Path, true, query.Depth == 1 ? PathTableRecursion.OneLevel : PathTableRecursion.Full))
        {
          if (query.Matches(tableTokenAndValue.Token))
            registryItemList.Add(new RegistryItem(tableTokenAndValue.Token, tableTokenAndValue.Value));
        }
        registryItems = (ICollection<RegistryItem>) registryItemList;
      }
      return registryItems;
    }

    void IInternalCachedRegistryService.OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.OnRegistrySettingsChanged(requestContext, eventClass, eventData);
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(97101, "Registry", nameof (CachedRegistryService), nameof (OnRegistrySettingsChanged));
      try
      {
        if (string.IsNullOrEmpty(eventData))
        {
          requestContext.Trace(97102, TraceLevel.Warning, "Registry", nameof (CachedRegistryService), "Empty event data for OnRegistrySettingsChanged; returning");
        }
        else
        {
          long sequenceId;
          List<RegistryUpdateRecord> list = SqlRegistryService.DeserializeSqlNotification(requestContext, eventData, out sequenceId).Select<RegistryItem, RegistryUpdateRecord>((Func<RegistryItem, RegistryUpdateRecord>) (s => new RegistryUpdateRecord(sequenceId, s))).ToList<RegistryUpdateRecord>();
          if (list.Count == 0)
          {
            requestContext.Trace(97102, TraceLevel.Warning, "Registry", nameof (CachedRegistryService), "No update records for OnRegistrySettingsChanged; returning");
          }
          else
          {
            requestContext.Trace(97103, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "OnRegistrySettingsChanged received {0} updates via SQL notification", (object) list.Count);
            Dictionary<RegistrySettingsChangedCallback, CachedRegistryService.RegistryCallbackInvocation> invocations = (Dictionary<RegistrySettingsChangedCallback, CachedRegistryService.RegistryCallbackInvocation>) null;
            lock (this.m_notificationsLock)
            {
              if (this.m_enqueuedRegistryUpdates != null)
              {
                this.m_enqueuedRegistryUpdates.Add((IList<RegistryUpdateRecord>) list);
                return;
              }
              foreach (RegistryUpdateRecord registryUpdateRecord in list)
              {
                RegistryUpdateRecord record = registryUpdateRecord;
                this.m_notificationSet.ProcessNotificationsForRegistryItem(record.Item, (RegistryNotificationSet.ProcessNotificationsEvaluator) ((in RegistryCallbackAndServiceHost match) =>
                {
                  if (invocations == null)
                    invocations = new Dictionary<RegistrySettingsChangedCallback, CachedRegistryService.RegistryCallbackInvocation>(RegistrySettingsChangedCallbackComparer.Instance);
                  CachedRegistryService.RegistryCallbackInvocation callbackInvocation;
                  if (!invocations.TryGetValue(match.Callback, out callbackInvocation))
                  {
                    callbackInvocation = new CachedRegistryService.RegistryCallbackInvocation(match.Callback, match.ServiceHostId);
                    invocations.Add(match.Callback, callbackInvocation);
                  }
                  callbackInvocation.FilteredEntries.Add(record.Item);
                }));
              }
            }
            requestContext.Trace(97105, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "OnRegistrySettingsChanged evaluated notifications and has {0} invocations to deliver", (object) (invocations != null ? invocations.Count : 0));
            this.UpdateRegistryCache(requestContext, (IList<RegistryUpdateRecord>) list);
            if (invocations == null || invocations.Count <= 0)
              return;
            this.DeliverNotifications(requestContext, invocations.Values);
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(97102, "Registry", nameof (CachedRegistryService), nameof (OnRegistrySettingsChanged));
      }
    }

    private void DeliverNotifications(
      IVssRequestContext requestContext,
      Dictionary<RegistrySettingsChangedCallback, CachedRegistryService.RegistryCallbackInvocation>.ValueCollection invocations)
    {
      ITeamFoundationHostManagementService service = requestContext.GetService<ITeamFoundationHostManagementService>();
      foreach (CachedRegistryService.RegistryCallbackInvocation invocation in invocations)
      {
        if (invocation.ServiceHostId != requestContext.ServiceHost.InstanceId)
        {
          requestContext.Trace(97106, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "DeliverNotifications invocation is for a child service host {0}", (object) invocation.ServiceHostId);
          using (IVssRequestContext requestContext1 = service.BeginRequest(requestContext, invocation.ServiceHostId, RequestContextType.SystemContext, false, false))
          {
            if (requestContext1 != null)
            {
              MethodInfo method = invocation.Callback.Method;
              Type reflectedType = method.ReflectedType;
              using (new TraceWatch(requestContext, 97006, TraceLevel.Error, TimeSpan.FromSeconds(1.0), "Registry", nameof (CachedRegistryService), "Callback {0}.{1}", new object[2]
              {
                reflectedType != (Type) null ? (object) reflectedType.FullName : (object) string.Empty,
                (object) method.Name
              }))
              {
                try
                {
                  requestContext.Trace(97109, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "DeliverNotifications is delivering a dummy collection with 1 item in it to {0}.{1} on service host {2}", reflectedType != (Type) null ? (object) reflectedType.FullName : (object) string.Empty, (object) method.Name, (object) invocation.ServiceHostId);
                  invocation.Callback(requestContext1, CachedRegistryService.s_dummyCollection);
                }
                catch (Exception ex)
                {
                  requestContext.TraceException(97008, "Registry", nameof (CachedRegistryService), ex);
                }
              }
            }
            else
              requestContext.Trace(97108, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "DeliverNotifications did not deliver to unloaded service host {0}", (object) invocation.ServiceHostId);
          }
        }
        else
        {
          requestContext.Trace(97107, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "DeliverNotifications invocation is local to service host {0}", (object) requestContext.ServiceHost.InstanceId);
          MethodInfo method = invocation.Callback.Method;
          Type reflectedType = method.ReflectedType;
          using (new TraceWatch(requestContext, 97006, TraceLevel.Error, TimeSpan.FromSeconds(1.0), "Registry", nameof (CachedRegistryService), "Callback {0}.{1}", new object[2]
          {
            reflectedType != (Type) null ? (object) reflectedType.FullName : (object) string.Empty,
            (object) method.Name
          }))
          {
            try
            {
              requestContext.Trace(97110, TraceLevel.Verbose, "Registry", nameof (CachedRegistryService), "DeliverNotifications is delivering a real set of filtered entries to {0}.{1} on service host {2}", reflectedType != (Type) null ? (object) reflectedType.FullName : (object) string.Empty, (object) method.Name, (object) invocation.ServiceHostId);
              invocation.Callback(requestContext, new RegistryEntryCollection("/", (IEnumerable<RegistryItem>) invocation.FilteredEntries));
            }
            catch (Exception ex)
            {
              requestContext.TraceException(97008, "Registry", nameof (CachedRegistryService), ex);
            }
          }
        }
      }
    }

    private void UpdateRegistryCache(
      IVssRequestContext requestContext,
      IList<RegistryUpdateRecord> updateRecords)
    {
      if (updateRecords.Count <= 0)
        return;
      long sequenceId;
      int num;
      using (requestContext.AcquireReaderLock(this.m_cacheLockName))
      {
        sequenceId = this.m_sequenceId;
        num = this.m_registryCache.Sum<KeyValuePair<string, PathTable<string>>>((Func<KeyValuePair<string, PathTable<string>>, int>) (s => s.Value == null ? 0 : s.Value.Count));
      }
      if (sequenceId == 0L)
      {
        using (requestContext.AcquireWriterLock(this.m_cacheLockName))
        {
          foreach (RegistryUpdateRecord updateRecord in (IEnumerable<RegistryUpdateRecord>) updateRecords)
            this.ApplyRegistryUpdateRecord(requestContext, in updateRecord);
          num = this.m_registryCache.Sum<KeyValuePair<string, PathTable<string>>>((Func<KeyValuePair<string, PathTable<string>>, int>) (s => s.Value == null ? 0 : s.Value.Count));
        }
      }
      else if (updateRecords[updateRecords.Count - 1].SequenceId > sequenceId)
      {
        using (requestContext.AcquireWriterLock(this.m_cacheLockName))
        {
          int index = 0;
          while (index < updateRecords.Count && updateRecords[index].SequenceId <= this.m_sequenceId)
            ++index;
          for (; index < updateRecords.Count; ++index)
          {
            this.ApplyRegistryUpdateRecord(requestContext, updateRecords[index]);
            this.m_sequenceId = updateRecords[index].SequenceId;
          }
          num = this.m_registryCache.Sum<KeyValuePair<string, PathTable<string>>>((Func<KeyValuePair<string, PathTable<string>>, int>) (s => s.Value == null ? 0 : s.Value.Count));
        }
      }
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      CachedRegistryService.s_deploymentRegistryEntriesTotal.SetValue((long) num);
    }

    private void ApplyRegistryUpdateRecord(
      IVssRequestContext requestContext,
      in RegistryUpdateRecord updateRecord)
    {
      int registryCacheIndex = this.GetRegistryCacheIndex(updateRecord.Item.Path);
      if (registryCacheIndex < 0)
        return;
      KeyValuePair<string, PathTable<string>> keyValuePair1 = this.m_registryCache[registryCacheIndex];
      PathTable<string> pathTable = keyValuePair1.Value;
      if (updateRecord.Item.Value == null)
      {
        pathTable?.Remove(updateRecord.Item.Path, false);
      }
      else
      {
        if (pathTable == null)
        {
          pathTable = new PathTable<string>('/', true);
          List<KeyValuePair<string, PathTable<string>>> registryCache = this.m_registryCache;
          int index = registryCacheIndex;
          keyValuePair1 = this.m_registryCache[registryCacheIndex];
          KeyValuePair<string, PathTable<string>> keyValuePair2 = new KeyValuePair<string, PathTable<string>>(keyValuePair1.Key, pathTable);
          registryCache[index] = keyValuePair2;
        }
        pathTable[updateRecord.Item.Path] = updateRecord.Item.Value;
      }
    }

    private int GetRegistryCacheIndex(string path)
    {
      for (int index = 0; index < this.m_registryCache.Count; ++index)
      {
        if (RegistryHelpers.IsSubItem(path, this.m_registryCache[index].Key))
          return index;
      }
      return -1;
    }

    private readonly struct RegistryCallbackInvocation
    {
      public readonly RegistrySettingsChangedCallback Callback;
      public readonly Guid ServiceHostId;
      public readonly List<RegistryItem> FilteredEntries;

      public RegistryCallbackInvocation(
        RegistrySettingsChangedCallback callback,
        Guid serviceHostId)
      {
        this.Callback = callback;
        this.ServiceHostId = serviceHostId;
        this.FilteredEntries = new List<RegistryItem>();
      }
    }

    private readonly struct RegistryQueryWithIndexes
    {
      public readonly int QueryIndex;
      public readonly RegistryQuery Query;
      public readonly int CacheIndex;

      public RegistryQueryWithIndexes(int queryIndex, RegistryQuery query, int cacheIndex)
      {
        this.QueryIndex = queryIndex;
        this.Query = query;
        this.CacheIndex = cacheIndex;
      }
    }

    private readonly struct QueryResultWithIndex
    {
      public readonly int QueryIndex;
      public readonly IEnumerable<RegistryItem> Results;
      public static readonly CachedRegistryService.QueryResultWithIndex[] EmptyArray = Array.Empty<CachedRegistryService.QueryResultWithIndex>();

      public QueryResultWithIndex(int queryIndex, IEnumerable<RegistryItem> results)
      {
        this.QueryIndex = queryIndex;
        this.Results = results;
      }
    }
  }
}
