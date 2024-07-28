// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.FrameworkNameResolutionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  internal class FrameworkNameResolutionService : 
    IInternalNameResolutionService,
    INameResolutionService,
    IVssFrameworkService
  {
    public virtual void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
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
      NameResolutionStore service = requestContext.GetService<NameResolutionStore>();
      bool flag = this.UseRemoteNameResolutionService(requestContext);
      QueryOptions queryOptions1 = flag ? queryOptions & ~(QueryOptions.IncludeMisses | QueryOptions.EnsureClientExpiration) : queryOptions;
      NameResolutionEntry[] source1 = service.QueryEntries(requestContext, queries, queryOptions1, filter);
      if (queryOptions.HasFlag((Enum) QueryOptions.ShortCircuitOnAnyResult) && ((IEnumerable<NameResolutionEntry>) source1).Any<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x != null && x.Value != Guid.Empty)))
        return source1;
      if (flag)
      {
        Dictionary<string, List<Tuple<NameResolutionQuery, int>>> dictionary = (Dictionary<string, List<Tuple<NameResolutionQuery, int>>>) null;
        for (int index = 0; index < source1.Length; ++index)
        {
          if (source1[index] == null)
          {
            dictionary = dictionary ?? new Dictionary<string, List<Tuple<NameResolutionQuery, int>>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            List<Tuple<NameResolutionQuery, int>> tupleList;
            if (!dictionary.TryGetValue(queries[index].Name, out tupleList))
              dictionary[queries[index].Name] = tupleList = new List<Tuple<NameResolutionQuery, int>>();
            tupleList.Add(Tuple.Create<NameResolutionQuery, int>(queries[index], index));
          }
        }
        if (dictionary != null)
        {
          Func<NameResolutionEntry, NameResolutionEntry> func = (Func<NameResolutionEntry, NameResolutionEntry>) (x => filter != null && x != null && !(x.Value == Guid.Empty) && !filter(x) ? (NameResolutionEntry) null : x);
          NameResolutionHttpClient client = this.GetClient(requestContext);
          foreach (KeyValuePair<string, List<Tuple<NameResolutionQuery, int>>> keyValuePair in dictionary)
          {
            string key = keyValuePair.Key;
            List<Tuple<NameResolutionQuery, int>> source2 = keyValuePair.Value;
            List<NameResolutionEntry> nameResolutionEntryList = client.QueryNameResolutionEntriesAsync(source2.Select<Tuple<NameResolutionQuery, int>, string>((Func<Tuple<NameResolutionQuery, int>, string>) (x => x.Item1.Namespace)), key, cancellationToken: requestContext.CancellationToken).SyncResult<List<NameResolutionEntry>>();
            for (int index = 0; index < nameResolutionEntryList.Count; ++index)
            {
              NameResolutionEntry nameResolutionEntry = func(nameResolutionEntryList[index]);
              source1[source2[index].Item2] = nameResolutionEntry;
              if (nameResolutionEntry != null)
              {
                NameResolutionEntryFaultInEvent notificationEvent = new NameResolutionEntryFaultInEvent()
                {
                  Entry = nameResolutionEntry
                };
                requestContext.GetService<TeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) notificationEvent);
                if (service.QueryEntries(requestContext, (IReadOnlyList<NameResolutionQuery>) new NameResolutionQuery[1]
                {
                  source2[index].Item1
                }).Single<NameResolutionEntry>() == null)
                {
                  using (requestContext.AllowAnonymousOrPublicUserWrites())
                    service.SetEntries(requestContext, (IEnumerable<NameResolutionEntry>) new NameResolutionEntry[1]
                    {
                      nameResolutionEntry
                    }, false);
                }
              }
            }
          }
        }
      }
      if (!queryOptions.HasFlag((Enum) QueryOptions.IncludeMisses))
      {
        for (int index = 0; index < source1.Length; ++index)
        {
          if (source1[index] != null && source1[index].Value == Guid.Empty)
            source1[index] = (NameResolutionEntry) null;
        }
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

    internal IList<NameResolutionEntry> QueryEntriesForValue(
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
      return !this.UseRemoteNameResolutionService(requestContext) ? requestContext.GetService<NameResolutionStore>().QueryEntriesForValue(requestContext, value, queryOptions) : (IList<NameResolutionEntry>) this.GetClient(requestContext).QueryNameResolutionEntriesForValueAsync(value, cancellationToken: requestContext.CancellationToken).SyncResult<List<NameResolutionEntry>>();
    }

    public NameResolutionEntry GetPrimaryEntryForValue(
      IVssRequestContext requestContext,
      Guid value)
    {
      NameResolutionStore localStore = requestContext.GetService<NameResolutionStore>();
      return localStore.GetPrimaryEntryForValue(requestContext, value, (Func<IVssRequestContext, Guid, NameResolutionEntry>) ((rq, v) =>
      {
        if (!this.UseRemoteNameResolutionService(rq))
          return (NameResolutionEntry) null;
        NameResolutionEntry primaryEntryForValue = this.QueryEntriesForValue(rq, v).Where<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x.IsPrimary)).SingleOrDefault<NameResolutionEntry>();
        if (primaryEntryForValue != null)
        {
          using (requestContext.AllowAnonymousOrPublicUserWrites())
            localStore.SetEntries(requestContext, (IEnumerable<NameResolutionEntry>) new NameResolutionEntry[1]
            {
              primaryEntryForValue
            }, true);
        }
        return primaryEntryForValue;
      }));
    }

    public void SetEntries(
      IVssRequestContext requestContext,
      IEnumerable<NameResolutionEntry> entries,
      bool overwriteExisting = false)
    {
      if (this.UseRemoteNameResolutionService(requestContext))
      {
        ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) entries, nameof (entries));
        foreach (NameResolutionEntry entry in entries)
        {
          ArgumentUtility.CheckForNull<NameResolutionEntry>(entry, "entry");
          this.GetClient(requestContext).SetNameResolutionEntryAsync(entry, entry.Namespace, entry.Name, new bool?(overwriteExisting), cancellationToken: requestContext.CancellationToken).SyncResult();
        }
      }
      requestContext.GetService<NameResolutionStore>().SetEntries(requestContext, entries, overwriteExisting);
    }

    public void DeleteEntries(
      IVssRequestContext requestContext,
      IEnumerable<NameResolutionEntry> entries)
    {
      if (this.UseRemoteNameResolutionService(requestContext))
      {
        ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) entries, nameof (entries));
        foreach (NameResolutionEntry entry in entries)
        {
          ArgumentUtility.CheckForNull<NameResolutionEntry>(entry, "entry");
          ArgumentUtility.CheckStringForNullOrWhiteSpace(entry.Name, "Name");
          this.GetClient(requestContext).DeleteNameResolutionEntryAsync(entry.Namespace, entry.Name, cancellationToken: requestContext.CancellationToken).SyncResult();
        }
      }
      requestContext.GetService<NameResolutionStore>().DeleteEntries(requestContext, entries);
    }

    protected virtual bool UseRemoteNameResolutionService(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment;

    private NameResolutionHttpClient GetClient(IVssRequestContext requestContext) => requestContext.Elevate().GetClient<NameResolutionHttpClient>(ServiceInstanceTypes.MPS);
  }
}
