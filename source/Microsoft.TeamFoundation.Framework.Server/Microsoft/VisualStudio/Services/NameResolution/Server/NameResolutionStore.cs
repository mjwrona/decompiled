// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.NameResolutionStore
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NameResolution.Server.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NameResolutionStore : 
    IInternalNameResolutionService,
    INameResolutionService,
    IVssFrameworkService
  {
    private const string c_area = "NameResolution";
    private const string c_layer = "NameResolutionStore";
    private INameResolutionCache m_entryCache;
    private PrimaryNameResolutionEntryCacheService m_primaryEntryCache;
    private int m_hitTTL;
    private int m_missTTL;
    private bool m_failSilentlyEnabled;
    private Guid m_serviceHostId;
    private bool m_isMps;
    private INotificationRegistration m_nameResolutionRegistration;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;
      this.ValidateRequestContext(systemRequestContext);
      this.InitializeSettings(systemRequestContext);
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), NameResolutionConstants.RegistryRoot + "**");
      this.m_isMps = systemRequestContext.ServiceInstanceType() == ServiceInstanceTypes.MPS;
      if (systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        this.m_nameResolutionRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.NameResolutionEntriesChanged, new SqlNotificationHandler(this.OnEntriesChanged), false, false);
        this.GetEntryCache(systemRequestContext);
      }
      else
        this.m_entryCache = (INameResolutionCache) systemRequestContext.GetService<INameResolutionCacheService>();
      this.m_primaryEntryCache = systemRequestContext.GetService<PrimaryNameResolutionEntryCacheService>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      if (!systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      this.m_nameResolutionRegistration.Unregister(systemRequestContext);
    }

    public IReadOnlyList<NameResolutionEntry> QueryEntries(
      IVssRequestContext requestContext,
      IReadOnlyList<NameResolutionQuery> queries)
    {
      return (IReadOnlyList<NameResolutionEntry>) this.QueryEntries(requestContext, queries, QueryOptions.None, (Predicate<NameResolutionEntry>) (_ => true));
    }

    internal NameResolutionEntry[] QueryEntries(
      IVssRequestContext requestContext,
      IReadOnlyList<NameResolutionQuery> queries,
      QueryOptions queryOptions,
      Predicate<NameResolutionEntry> filter)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) queries, nameof (queries));
      Func<NameResolutionEntry, NameResolutionEntry> func = (Func<NameResolutionEntry, NameResolutionEntry>) (x => filter != null && x != null && !(x.Value == Guid.Empty) && !filter(x) ? (NameResolutionEntry) null : x);
      NameResolutionEntry[] source1 = new NameResolutionEntry[queries.Count];
      INameResolutionCache entryCache = this.GetEntryCache(requestContext);
      Dictionary<string, List<Tuple<NameResolutionQuery, int>>> dictionary = (Dictionary<string, List<Tuple<NameResolutionQuery, int>>>) null;
      try
      {
        for (int index = 0; index < queries.Count; ++index)
        {
          NameResolutionQuery query = queries[index];
          ArgumentUtility.CheckForNull<NameResolutionQuery>(query, "query");
          NameResolutionValidation.ValidateNamespace(query.Namespace);
          NameResolutionValidation.ValidateName(query.Name);
          source1[index] = func(entryCache.Get(requestContext, query.Namespace, query.Name));
          if (source1[index] == null && entryCache.IncrementalUpdatesAllowed)
          {
            dictionary = dictionary ?? new Dictionary<string, List<Tuple<NameResolutionQuery, int>>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            List<Tuple<NameResolutionQuery, int>> tupleList;
            if (!dictionary.TryGetValue(query.Name, out tupleList))
              dictionary[query.Name] = tupleList = new List<Tuple<NameResolutionQuery, int>>();
            tupleList.Add(Tuple.Create<NameResolutionQuery, int>(query, index));
          }
        }
        if (queryOptions.HasFlag((Enum) QueryOptions.ShortCircuitOnAnyResult) && ((IEnumerable<NameResolutionEntry>) source1).Any<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x != null && x.Value != Guid.Empty)))
          return source1;
        if (dictionary != null)
        {
          foreach (KeyValuePair<string, List<Tuple<NameResolutionQuery, int>>> keyValuePair in dictionary)
          {
            string key = keyValuePair.Key;
            List<Tuple<NameResolutionQuery, int>> source2 = keyValuePair.Value;
            IList<NameResolutionEntry> resolutionEntries;
            using (NameResolutionComponent component = this.CreateComponent(requestContext, key))
              resolutionEntries = component.GetNameResolutionEntries((IList<NameResolutionQuery>) source2.Select<Tuple<NameResolutionQuery, int>, NameResolutionQuery>((Func<Tuple<NameResolutionQuery, int>, NameResolutionQuery>) (x => x.Item1)).ToArray<NameResolutionQuery>());
            for (int index = 0; index < resolutionEntries.Count; ++index)
            {
              NameResolutionEntry entry = func(resolutionEntries[index]);
              source1[source2[index].Item2] = entry;
              if (entry != null)
                entryCache.Set(requestContext, entry);
            }
          }
        }
        for (int index = 0; index < source1.Length; ++index)
        {
          NameResolutionEntry nameResolutionEntry = source1[index]?.Clone();
          if (queryOptions.HasFlag((Enum) QueryOptions.IncludeMisses) && nameResolutionEntry == null)
            nameResolutionEntry = new NameResolutionEntry()
            {
              Namespace = queries[index].Namespace,
              Name = queries[index].Name,
              Value = Guid.Empty,
              TTL = new int?(this.m_missTTL)
            };
          if (queryOptions.HasFlag((Enum) QueryOptions.EnsureClientExpiration) && nameResolutionEntry != null && !nameResolutionEntry.HasExpiration)
            nameResolutionEntry.TTL = new int?(this.m_hitTTL);
          source1[index] = nameResolutionEntry;
        }
      }
      catch (Exception ex) when (this.IsFailingSilently(requestContext))
      {
        source1 = new NameResolutionEntry[queries.Count];
        requestContext.TraceException(15010787, "NameResolution", nameof (NameResolutionStore), ex);
      }
      return source1;
    }

    public NameResolutionEntry QueryFirstEntry(
      IVssRequestContext requestContext,
      IReadOnlyList<string> namespaces,
      string name,
      Predicate<NameResolutionEntry> filter)
    {
      ArgumentUtility.CheckForNull<IReadOnlyList<string>>(namespaces, nameof (namespaces));
      NameResolutionQuery[] array = namespaces.Select<string, NameResolutionQuery>((Func<string, NameResolutionQuery>) (ns => new NameResolutionQuery(ns, name))).ToArray<NameResolutionQuery>();
      return ((IEnumerable<NameResolutionEntry>) this.QueryEntries(requestContext, (IReadOnlyList<NameResolutionQuery>) array, QueryOptions.ShortCircuitOnAnyResult, filter)).FirstOrDefault<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x != null && x.Value != Guid.Empty));
    }

    public virtual IList<NameResolutionEntry> QueryEntriesForValue(
      IVssRequestContext requestContext,
      Guid value)
    {
      return this.QueryEntriesForValue(requestContext, value, QueryOptions.None);
    }

    IList<NameResolutionEntry> IInternalNameResolutionService.QueryEntriesForValue(
      IVssRequestContext requestContext,
      Guid value,
      QueryOptions queryOptions)
    {
      return this.QueryEntriesForValue(requestContext, value, queryOptions);
    }

    internal IList<NameResolutionEntry> QueryEntriesForValue(
      IVssRequestContext requestContext,
      Guid value,
      QueryOptions queryOptions)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(value, nameof (value));
      List<NameResolutionEntry> results;
      if (this.m_isMps)
      {
        results = new List<NameResolutionEntry>();
        this.ExecuteForAllDatabases(requestContext, (Action<IVssRequestContext, NameResolutionComponent>) ((rq, component) => results.AddRange((IEnumerable<NameResolutionEntry>) component.GetNameResolutionEntriesByValue(value))));
      }
      else
      {
        using (NameResolutionComponent component = requestContext.CreateComponent<NameResolutionComponent>())
          results = component.GetNameResolutionEntriesByValue(value);
      }
      if (queryOptions.HasFlag((Enum) QueryOptions.PermanentEntriesOnly))
        results = results.Where<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => !x.HasExpiration)).ToList<NameResolutionEntry>();
      if (queryOptions.HasFlag((Enum) QueryOptions.EnsureClientExpiration))
      {
        for (int index = 0; index < results.Count; ++index)
        {
          if (!results[index].HasExpiration)
            results[index].TTL = new int?(this.m_hitTTL);
        }
      }
      return (IList<NameResolutionEntry>) results;
    }

    public NameResolutionEntry GetPrimaryEntryForValue(
      IVssRequestContext requestContext,
      Guid value)
    {
      return this.GetPrimaryEntryForValue(requestContext, value, (Func<IVssRequestContext, Guid, NameResolutionEntry>) null);
    }

    internal NameResolutionEntry GetPrimaryEntryForValue(
      IVssRequestContext requestContext,
      Guid value,
      Func<IVssRequestContext, Guid, NameResolutionEntry> fallbackFunc)
    {
      bool flag = NameResolutionStore.TreatCachedNullAsCacheMiss(requestContext);
      NameResolutionEntry primaryEntryForValue;
      if (!this.m_primaryEntryCache.TryGetValue(requestContext, value, out primaryEntryForValue) || primaryEntryForValue == null & flag)
      {
        primaryEntryForValue = this.QueryEntriesForValue(requestContext, value).Where<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x.IsPrimary)).SingleOrDefault<NameResolutionEntry>();
        if (primaryEntryForValue == null && fallbackFunc != null)
          primaryEntryForValue = fallbackFunc(requestContext, value);
        if (primaryEntryForValue != null)
          this.m_primaryEntryCache.TryAdd(requestContext, value, primaryEntryForValue);
        else
          this.m_primaryEntryCache.TryAdd(requestContext, value, (NameResolutionEntry) null);
      }
      return primaryEntryForValue;
    }

    private static bool TreatCachedNullAsCacheMiss(IVssRequestContext requestContext)
    {
      object obj;
      return requestContext.Items.TryGetValue(nameof (TreatCachedNullAsCacheMiss), out obj) && obj is bool flag && flag;
    }

    public virtual void SetEntries(
      IVssRequestContext requestContext,
      IEnumerable<NameResolutionEntry> entries,
      bool overwriteExisting = false)
    {
      this.ValidateRequestContext(requestContext);
      NameResolutionValidation.ValidateEntries(entries);
      try
      {
        requestContext.GetService<ITeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new NameResolutionSetEvent(entries));
      }
      catch (ActionDeniedBySubscriberException ex)
      {
        throw new NameResolutionEntryAlreadyExistsException(ex.Message);
      }
      try
      {
        foreach (NameResolutionEntry entry1 in entries)
        {
          NameResolutionEntry entry = entry1;
          requestContext.Trace(5001570, TraceLevel.Info, "NameResolution", nameof (NameResolutionStore), "Writing name resolution entry {0}, overwriteExisting={1}", (object) entry, (object) overwriteExisting);
          GeoReplicationHelper.PerformWrite<NameResolutionComponent>(requestContext, (Func<NameResolutionComponent>) (() => this.CreateComponent(requestContext, entry.Name)), (Action<NameResolutionComponent>) (component => component.SetResolutionEntries((IEnumerable<NameResolutionEntry>) new NameResolutionEntry[1]
          {
            entry
          }, (overwriteExisting ? 1 : 0) != 0)), nameof (SetEntries));
        }
        this.UpdateCache(requestContext, entries);
      }
      catch (Exception ex) when (this.IsFailingSilently(requestContext))
      {
        requestContext.TraceException(15010788, "NameResolution", nameof (NameResolutionStore), ex);
      }
    }

    public virtual void DeleteEntries(
      IVssRequestContext requestContext,
      IEnumerable<NameResolutionEntry> entries)
    {
      this.ValidateRequestContext(requestContext);
      NameResolutionValidation.ValidateEntries(entries);
      List<NameResolutionEntry> deletedEntries = new List<NameResolutionEntry>();
      foreach (NameResolutionEntry entry1 in entries)
      {
        NameResolutionEntry entry = entry1;
        requestContext.Trace(5001571, TraceLevel.Info, "NameResolution", nameof (NameResolutionStore), "Deleting name resolution entry {0}", (object) entry);
        deletedEntries.Add(entry);
        GeoReplicationHelper.PerformWrite<NameResolutionComponent>(requestContext, (Func<NameResolutionComponent>) (() => this.CreateComponent(requestContext, entry.Name)), (Action<NameResolutionComponent>) (component => deletedEntries.AddRange((IEnumerable<NameResolutionEntry>) component.RemoveResolutionEntries((IEnumerable<NameResolutionEntry>) new NameResolutionEntry[1]
        {
          entry
        }))), nameof (DeleteEntries));
      }
      this.UpdateCache(requestContext, (IEnumerable<NameResolutionEntry>) deletedEntries);
    }

    public virtual void DeleteEntriesForValue(
      IVssRequestContext requestContext,
      Guid value,
      string @namespace = null)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(value, nameof (value));
      if (@namespace != null)
        NameResolutionValidation.ValidateNamespace(@namespace);
      if (this.m_isMps)
        throw new InvalidOperationException("This operation is not supported on MPS.");
      requestContext.Trace(5001572, TraceLevel.Info, "NameResolution", nameof (NameResolutionStore), "Deleting name resolution entry for value {0} in namespace {1}", (object) value, (object) @namespace);
      List<NameResolutionEntry> deletedEntries = new List<NameResolutionEntry>();
      GeoReplicationHelper.PerformWrite<NameResolutionComponent>(requestContext, (Func<NameResolutionComponent>) (() => requestContext.CreateComponent<NameResolutionComponent>()), (Action<NameResolutionComponent>) (component => deletedEntries.AddRange((IEnumerable<NameResolutionEntry>) component.RemoveResolutionEntries(value, @namespace))), nameof (DeleteEntriesForValue));
      this.UpdateCache(requestContext, (IEnumerable<NameResolutionEntry>) deletedEntries);
    }

    public virtual IList<NameResolutionEntry> GetAllEntries(IVssRequestContext requestContext)
    {
      if (this.m_isMps)
        throw new InvalidOperationException("This operation is not supported on MPS.");
      using (NameResolutionComponent component = requestContext.CreateComponent<NameResolutionComponent>())
        return component.GetAllNameResolutionEntries();
    }

    private INameResolutionCache GetEntryCache(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        return this.m_entryCache;
      INameResolutionCache entryCache = this.m_entryCache;
      if (entryCache == null)
      {
        entryCache = (INameResolutionCache) new OnPremNameResolutionCache((IEnumerable<NameResolutionEntry>) this.GetAllEntries(requestContext));
        INameResolutionCache nameResolutionCache = Interlocked.CompareExchange<INameResolutionCache>(ref this.m_entryCache, entryCache, (INameResolutionCache) null);
        if (nameResolutionCache != null)
          entryCache = nameResolutionCache;
      }
      return entryCache;
    }

    private void UpdateCache(
      IVssRequestContext requestContext,
      IEnumerable<NameResolutionEntry> entries)
    {
      foreach (NameResolutionEntry entry in entries)
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          this.m_entryCache = (INameResolutionCache) null;
        else
          this.m_entryCache.Remove(requestContext, entry);
        if (entry.IsPrimary)
          this.m_primaryEntryCache.Remove(requestContext, entry.Value);
      }
    }

    private void ExecuteForAllDatabases(
      IVssRequestContext requestContext,
      Action<IVssRequestContext, NameResolutionComponent> action)
    {
      requestContext.GetService<DataspacePartitionService>().ExecuteForAllPartitions<NameResolutionComponent>(requestContext, NameResolutionConstants.MpsCategory, action);
    }

    private NameResolutionComponent CreateComponent(IVssRequestContext requestContext, string name) => this.m_isMps ? requestContext.GetService<IDataspacePartitionService>().CreateComponent<NameResolutionComponent>(requestContext, NameResolutionConstants.MpsCategory, name) : requestContext.CreateComponent<NameResolutionComponent>();

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.InitializeSettings(requestContext);
    }

    private void InitializeSettings(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.m_hitTTL = service.GetValue<int>(requestContext, (RegistryQuery) NameResolutionConstants.HitTTLKey, 3600);
      this.m_missTTL = service.GetValue<int>(requestContext, (RegistryQuery) NameResolutionConstants.MissTTLKey, 300);
      this.m_failSilentlyEnabled = service.GetValue<bool>(requestContext, (RegistryQuery) NameResolutionConstants.NameResolutionStoreSilentFailure, false);
    }

    private void OnEntriesChanged(IVssRequestContext requestContext, NotificationEventArgs args) => this.m_entryCache = (INameResolutionCache) null;

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private bool IsFailingSilently(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment && !this.m_isMps && this.m_failSilentlyEnabled;

    internal INameResolutionCache EntryCache
    {
      get => this.m_entryCache;
      set => this.m_entryCache = value;
    }
  }
}
