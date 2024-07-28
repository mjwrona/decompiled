// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ImsCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Cache;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class ImsCacheService : IImsCacheService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private ImsOperation m_supportedLocalCacheOperations;
    private ImsOperation m_supportedRemoteCacheOperations;
    private static int s_tenPercentCacheMissRecheckCounter = 0;
    private static readonly string[] s_wellKnownAccountNamePrefixesNotToCache = new string[1]
    {
      ServerResources.GSS_PROJECT_ADMINISTRATORS()
    };
    private const string s_area = "Microsoft.VisualStudio.Services.Identity";
    private const string s_layer = "ImsCacheService";

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void Initialize(
      IVssRequestContext context,
      ImsOperation operationsToCacheLocally,
      ImsOperation operationsToCacheRemotely)
    {
      IEnumerable<Type> typesToCache1 = ImsCacheService.GenerateTypesToCache(operationsToCacheLocally);
      context.GetService<IImsLocalDataCache>().AddObjectTypes(context, typesToCache1);
      IEnumerable<Type> typesToCache2 = ImsCacheService.GenerateTypesToCache(operationsToCacheRemotely);
      context.GetService<IImsRemoteCache>().AddObjectTypes(context, typesToCache2);
      this.m_supportedLocalCacheOperations = operationsToCacheLocally;
      this.m_supportedRemoteCacheOperations = operationsToCacheRemotely;
    }

    public virtual void AddSupportForOperations(
      IVssRequestContext context,
      ImsOperation operationsToCacheLocally,
      ImsOperation operationsToCacheRemotely)
    {
      IEnumerable<Type> typesToCache1 = ImsCacheService.GenerateTypesToCache(operationsToCacheLocally);
      context.GetService<IImsLocalDataCache>().AddObjectTypes(context, typesToCache1);
      IEnumerable<Type> typesToCache2 = ImsCacheService.GenerateTypesToCache(operationsToCacheRemotely);
      context.GetService<IImsRemoteCache>().AddObjectTypes(context, typesToCache2);
      this.m_supportedLocalCacheOperations |= operationsToCacheLocally;
      this.m_supportedRemoteCacheOperations |= operationsToCacheRemotely;
    }

    private static IEnumerable<Type> GenerateTypesToCache(ImsOperation operations)
    {
      HashSet<Type> typesToCache = new HashSet<Type>();
      if (operations.HasFlag((Enum) ImsOperation.Children))
        typesToCache.Add(typeof (ImsCacheChildren));
      if (operations.HasFlag((Enum) ImsOperation.Descendants))
      {
        typesToCache.Add(typeof (ImsCacheChildren));
        typesToCache.Add(typeof (ImsCacheDescendants));
      }
      if (operations.HasFlag((Enum) ImsOperation.IdentitiesByDescriptor))
      {
        typesToCache.Add(typeof (ImsCacheIdentityId));
        typesToCache.Add(typeof (ImsCacheScopeMembership));
        typesToCache.Add(typeof (ImsCacheIdentity));
      }
      if (operations.HasFlag((Enum) ImsOperation.IdentitiesInScope))
        typesToCache.Add(typeof (ImsCacheIdentitiesInScope));
      if (operations.HasFlag((Enum) ImsOperation.IdentitiesByDisplayName))
      {
        typesToCache.Add(typeof (ImsCacheIdentity));
        typesToCache.Add(typeof (ImsCacheIdentitiesByDisplayName));
      }
      if (operations.HasFlag((Enum) ImsOperation.IdentityIdsByAppIdSearch))
        typesToCache.Add(typeof (ImsCacheAppIdSearchIndex));
      if (operations.HasFlag((Enum) ImsOperation.IdentitiesByAccountName))
      {
        typesToCache.Add(typeof (ImsCacheIdentity));
        typesToCache.Add(typeof (ImsCacheIdentitiesByAccountName));
      }
      if (operations.HasFlag((Enum) ImsOperation.IdentitiesById))
      {
        typesToCache.Add(typeof (ImsCacheScopeMembership));
        typesToCache.Add(typeof (ImsCacheIdentity));
      }
      if (operations.HasFlag((Enum) ImsOperation.IdentityIdsByDisplayNamePrefixSearch))
        typesToCache.Add(typeof (ImsCacheDisplayNameSearchIndex));
      if (operations.HasFlag((Enum) ImsOperation.IdentityIdsByEmailPrefixSearch))
        typesToCache.Add(typeof (ImsCacheEmailSearchIndex));
      if (operations.HasFlag((Enum) ImsOperation.IdentityIdsByAccountNamePrefixSearch))
        typesToCache.Add(typeof (ImsCacheAccountNameSearchIndex));
      if (operations.HasFlag((Enum) ImsOperation.IdentityIdsByDomainAccountNamePrefixSearch))
        typesToCache.Add(typeof (ImsCacheDomainAccountNameSearchIndex));
      if (operations.HasFlag((Enum) ImsOperation.MruIdentityIds))
        typesToCache.Add(typeof (ImsCacheMruIdentityIds));
      return (IEnumerable<Type>) typesToCache;
    }

    private IDictionary<ImsCacheIdKey, ImsCacheChildren> OnGetChildrenCacheMiss(
      IVssRequestContext context,
      IEnumerable<ImsCacheIdKey> keys,
      DateTimeOffset now)
    {
      return (IDictionary<ImsCacheIdKey, ImsCacheChildren>) context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) keys.Select<ImsCacheIdKey, Guid>((Func<ImsCacheIdKey, Guid>) (x => x.Id)).ToList<Guid>(), QueryMembership.Direct, (IEnumerable<string>) null).Zip<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdKey, KeyValuePair<ImsCacheIdKey, ImsCacheChildren>>(keys, (Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdKey, KeyValuePair<ImsCacheIdKey, ImsCacheChildren>>) ((value, key) => new KeyValuePair<ImsCacheIdKey, ImsCacheChildren>(key, new ImsCacheChildren(key, ImsCacheService.ExtractMembers(context, value), now)))).ToDedupedDictionary<KeyValuePair<ImsCacheIdKey, ImsCacheChildren>, ImsCacheIdKey, ImsCacheChildren>((Func<KeyValuePair<ImsCacheIdKey, ImsCacheChildren>, ImsCacheIdKey>) (kvp => kvp.Key), (Func<KeyValuePair<ImsCacheIdKey, ImsCacheChildren>, ImsCacheChildren>) (kvp => kvp.Value));
    }

    public Dictionary<Guid, ISet<IdentityId>> GetDescendants(
      IVssRequestContext context,
      IEnumerable<Guid> groupIds)
    {
      this.ValidateRequestContext(context);
      if (!ImsCacheService.IsFeatureEnabled(context) || !this.IsOperationSupported(ImsOperation.Descendants))
        return (Dictionary<Guid, ISet<IdentityId>>) null;
      List<ImsCacheService.GetDescendantsOperationState> list1 = groupIds.Select<Guid, ImsCacheService.GetDescendantsOperationState>((Func<Guid, ImsCacheService.GetDescendantsOperationState>) (x => new ImsCacheService.GetDescendantsOperationState()
      {
        Id = x,
        CacheKey = new ImsCacheIdKey(x),
        Results = (ISet<IdentityId>) new HashSet<IdentityId>(),
        RecursivelyComputedResults = new HashSet<IdentityId>(),
        GroupsToExpand = new HashSet<Guid>((IEnumerable<Guid>) new Guid[1]
        {
          x
        }),
        GroupsExpanded = new HashSet<Guid>(),
        LevelsExpanded = 0,
        LeftOverGroupsToExpand = new List<Guid>() { x },
        Completed = false,
        Failed = false
      })).ToList<ImsCacheService.GetDescendantsOperationState>().Zip<ImsCacheService.GetDescendantsOperationState, Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheService.GetDescendantsOperationState>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) groupIds.ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null), (Func<ImsCacheService.GetDescendantsOperationState, Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheService.GetDescendantsOperationState>) ((workingItem, group) =>
      {
        if (group == null || !group.IsContainer)
          workingItem.Completed = true;
        return workingItem;
      })).ToList<ImsCacheService.GetDescendantsOperationState>();
      if (list1.All<ImsCacheService.GetDescendantsOperationState>((Func<ImsCacheService.GetDescendantsOperationState, bool>) (x => x.Completed)))
        return list1.ToDedupedDictionary<ImsCacheService.GetDescendantsOperationState, Guid, ISet<IdentityId>>((Func<ImsCacheService.GetDescendantsOperationState, Guid>) (x => x.Id), (Func<ImsCacheService.GetDescendantsOperationState, ISet<IdentityId>>) (x => x.Results));
      List<ImsCacheService.GetDescendantsOperationState> list2 = list1.Where<ImsCacheService.GetDescendantsOperationState>((Func<ImsCacheService.GetDescendantsOperationState, bool>) (x => !x.Completed)).ToList<ImsCacheService.GetDescendantsOperationState>();
      try
      {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
        IDictionary<ImsCacheIdKey, ImsCacheDescendants> objects1 = context.GetService<IImsCacheOrchestrator>().GetObjects<ImsCacheIdKey, ImsCacheDescendants>(context, (ICollection<ImsCacheIdKey>) list2.Select<ImsCacheService.GetDescendantsOperationState, ImsCacheIdKey>((Func<ImsCacheService.GetDescendantsOperationState, ImsCacheIdKey>) (x => x.CacheKey)).ToList<ImsCacheIdKey>(), (Func<ImsCacheDescendants, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.DescendantsTimeToLive, now)));
        foreach (ImsCacheService.GetDescendantsOperationState descendantsOperationState in list2)
        {
          ImsCacheDescendants cacheDescendants = objects1[descendantsOperationState.CacheKey];
          if (!ImsCacheService.IsCacheMiss((ImsCacheObject) cacheDescendants, settings.DescendantsTimeToLive, now))
          {
            descendantsOperationState.Results = cacheDescendants.Value;
            descendantsOperationState.Completed = true;
          }
        }
        this.ExpandDescendantsRecursively(context, list1, settings);
        IEnumerable<ImsCacheService.GetDescendantsOperationState> descendantsOperationStates = list1.Where<ImsCacheService.GetDescendantsOperationState>((Func<ImsCacheService.GetDescendantsOperationState, bool>) (x =>
        {
          if (!x.Completed || !x.ResultsComputedRecursively)
            return false;
          return x.RecursivelyComputedResults.Count >= settings.WarningDescendantsCountThreshold || x.LevelsExpanded >= settings.WarningDescendantsLevelsThreshold;
        }));
        if (!descendantsOperationStates.IsNullOrEmpty<ImsCacheService.GetDescendantsOperationState>())
        {
          IEnumerable<ImsCacheDescendants> objects2 = descendantsOperationStates.Select<ImsCacheService.GetDescendantsOperationState, ImsCacheDescendants>((Func<ImsCacheService.GetDescendantsOperationState, ImsCacheDescendants>) (x => new ImsCacheDescendants(x.CacheKey, x.Results, DateTimeOffset.UtcNow)));
          try
          {
            context.GetService<IImsRemoteCache>().AddObjects<ImsCacheDescendants>(context, objects2);
          }
          catch (Exception ex)
          {
            context.TraceException(1754825, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), ex);
          }
        }
        return list1.ToDedupedDictionary<ImsCacheService.GetDescendantsOperationState, Guid, ISet<IdentityId>>((Func<ImsCacheService.GetDescendantsOperationState, Guid>) (x => x.Id), (Func<ImsCacheService.GetDescendantsOperationState, ISet<IdentityId>>) (x => !x.Failed ? x.Results : (ISet<IdentityId>) null));
      }
      catch (Exception ex)
      {
        context.TraceException(1754826, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), ex);
      }
      return (Dictionary<Guid, ISet<IdentityId>>) null;
    }

    private void ExpandDescendantsRecursively(
      IVssRequestContext context,
      List<ImsCacheService.GetDescendantsOperationState> workingSet,
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings)
    {
      for (int index = 0; workingSet.Any<ImsCacheService.GetDescendantsOperationState>((Func<ImsCacheService.GetDescendantsOperationState, bool>) (x => !x.Completed && !x.Failed)) && index < settings.MaxDescendantLevelsToIterate; ++index)
      {
        List<ImsCacheService.GetDescendantsOperationState> list = workingSet.Where<ImsCacheService.GetDescendantsOperationState>((Func<ImsCacheService.GetDescendantsOperationState, bool>) (x => !x.Completed && !x.Failed)).ToList<ImsCacheService.GetDescendantsOperationState>();
        HashSet<ImsCacheIdKey> keys1 = new HashSet<ImsCacheIdKey>();
        foreach (ImsCacheService.GetDescendantsOperationState descendantsOperationState in list)
        {
          descendantsOperationState.ResultsComputedRecursively = true;
          keys1.UnionWith(descendantsOperationState.LeftOverGroupsToExpand.Select<Guid, ImsCacheIdKey>((Func<Guid, ImsCacheIdKey>) (id => new ImsCacheIdKey(id))));
        }
        DateTimeOffset now = DateTimeOffset.UtcNow;
        IImsCacheOrchestrator service = context.GetService<IImsCacheOrchestrator>();
        IDictionary<ImsCacheIdKey, ImsCacheChildren> allCacheValues;
        try
        {
          allCacheValues = service.GetObjects<ImsCacheIdKey, ImsCacheChildren>(context, (ICollection<ImsCacheIdKey>) keys1, (Func<ImsCacheChildren, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.ChildrenTimeToLive, now)), (Func<IEnumerable<ImsCacheIdKey>, IDictionary<ImsCacheIdKey, ImsCacheChildren>>) (keys => this.OnGetChildrenCacheMiss(context, keys, now)));
        }
        catch (Exception ex)
        {
          context.TraceException(1754205, TraceLevel.Error, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), ex);
          using (List<ImsCacheService.GetDescendantsOperationState>.Enumerator enumerator = list.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.Failed = true;
            break;
          }
        }
        foreach (ImsCacheService.GetDescendantsOperationState descendantsOperationState in list)
        {
          HashSet<IdentityId> identityIdSet = new HashSet<IdentityId>();
          foreach (ImsCacheChildren imsCacheChildren in descendantsOperationState.LeftOverGroupsToExpand.Select<Guid, ImsCacheIdKey>((Func<Guid, ImsCacheIdKey>) (leftOverGroupId => new ImsCacheIdKey(leftOverGroupId))).Select<ImsCacheIdKey, ImsCacheChildren>((Func<ImsCacheIdKey, ImsCacheChildren>) (cacheKey => allCacheValues[cacheKey])).Where<ImsCacheChildren>((Func<ImsCacheChildren, bool>) (cacheValue => cacheValue?.Value != null)))
            identityIdSet.UnionWith((IEnumerable<IdentityId>) imsCacheChildren.Value);
          if (!identityIdSet.IsNullOrEmpty<IdentityId>())
          {
            descendantsOperationState.RecursivelyComputedResults.UnionWith((IEnumerable<IdentityId>) identityIdSet);
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            descendantsOperationState.GroupsToExpand.UnionWith(identityIdSet.Where<IdentityId>(ImsCacheService.\u003C\u003EO.\u003C0\u003E__IsVsoGroup ?? (ImsCacheService.\u003C\u003EO.\u003C0\u003E__IsVsoGroup = new Func<IdentityId, bool>(ImsCacheService.IsVsoGroup))).Select<IdentityId, Guid>((Func<IdentityId, Guid>) (x => x.Id)));
          }
          descendantsOperationState.GroupsExpanded.UnionWith((IEnumerable<Guid>) descendantsOperationState.LeftOverGroupsToExpand);
          descendantsOperationState.LeftOverGroupsToExpand = descendantsOperationState.GroupsToExpand.Except<Guid>((IEnumerable<Guid>) descendantsOperationState.GroupsExpanded).ToList<Guid>();
          ++descendantsOperationState.LevelsExpanded;
          descendantsOperationState.Completed = !descendantsOperationState.LeftOverGroupsToExpand.Any<Guid>();
        }
      }
      foreach (ImsCacheService.GetDescendantsOperationState descendantsOperationState in workingSet.Where<ImsCacheService.GetDescendantsOperationState>((Func<ImsCacheService.GetDescendantsOperationState, bool>) (x => x.ResultsComputedRecursively)))
        descendantsOperationState.Results = (ISet<IdentityId>) descendantsOperationState.RecursivelyComputedResults;
      this.TraceGetDescendantsResults(context, (IEnumerable<ImsCacheService.GetDescendantsOperationState>) workingSet, settings);
    }

    private void TraceGetDescendantsResults(
      IVssRequestContext context,
      IEnumerable<ImsCacheService.GetDescendantsOperationState> results,
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings)
    {
      foreach (ImsCacheService.GetDescendantsOperationState result in results)
      {
        if (result.LevelsExpanded >= settings.WarningDescendantsLevelsThreshold)
          context.Trace(1754105, TraceLevel.Warning, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), string.Format("Expanded group {0} and found {1} levels", (object) result.Id, (object) result.LevelsExpanded));
        if (result.LevelsExpanded >= settings.MaxDescendantLevelsToIterate)
          context.Trace(1754106, TraceLevel.Error, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), string.Format("Expanded group {0} and found {1} levels", (object) result.Id, (object) result.LevelsExpanded));
        if (result.Results.Count >= settings.WarningDescendantsCountThreshold)
          context.Trace(1754107, TraceLevel.Warning, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), string.Format("Expanded group {0} and found {1} users", (object) result.Id, (object) result.Results.Count));
      }
    }

    public Dictionary<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>> GetIdentitiesInScope(
      IVssRequestContext context,
      IEnumerable<Guid> scopeIds)
    {
      this.ValidateRequestContext(context);
      if (this.IsIdentitiesInScopeDisabled(context))
        return (Dictionary<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>) null;
      bool flag1 = this.IsLocalCacheEnabledForIdentitiesInScope(context);
      bool flag2 = this.IsRemoteCacheEnabledForIdentitiesInScope(context);
      if (!flag1 && !flag2)
        return (Dictionary<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>) null;
      DateTimeOffset now = DateTimeOffset.UtcNow;
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
      List<ImsCacheIdKey> list = scopeIds.Select<Guid, ImsCacheIdKey>((Func<Guid, ImsCacheIdKey>) (id => new ImsCacheIdKey(id))).ToList<ImsCacheIdKey>();
      IImsCacheOrchestrator service = context.GetService<IImsCacheOrchestrator>();
      IDictionary<ImsCacheIdKey, ImsCacheIdentitiesInScope> cacheValues;
      if (flag1 & flag2)
        cacheValues = service.GetObjectsAndRefreshLocal<ImsCacheIdKey, ImsCacheIdentitiesInScope>(context, (ICollection<ImsCacheIdKey>) list, (Func<ImsCacheIdentitiesInScope, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.IdentitiesInScopeTimeToLive, now)));
      else if (flag2)
        cacheValues = context.GetService<IImsRemoteCache>().GetObjects<ImsCacheIdKey, ImsCacheIdentitiesInScope>(context, (ICollection<ImsCacheIdKey>) list);
      else
        cacheValues = context.GetService<IImsLocalDataCache>().GetObjects<ImsCacheIdKey, ImsCacheIdentitiesInScope>(context, (ICollection<ImsCacheIdKey>) list);
      ImsCacheIdentitiesInScope identitiesInScope;
      return list.ToDedupedDictionary<ImsCacheIdKey, Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>((Func<ImsCacheIdKey, Guid>) (key => key.Id), (Func<ImsCacheIdKey, IList<Microsoft.VisualStudio.Services.Identity.Identity>>) (key => !cacheValues.TryGetValue(key, out identitiesInScope) || ImsCacheService.IsCacheMiss((ImsCacheObject) identitiesInScope, settings.IdentitiesInScopeTimeToLive, now) ? (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null : identitiesInScope.Value));
    }

    public void SetIdentitiesInScope(
      IVssRequestContext context,
      IEnumerable<KeyValuePair<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>> values)
    {
      this.ValidateRequestContext(context);
      if (this.IsIdentitiesInScopeDisabled(context))
        return;
      DateTimeOffset now = DateTimeOffset.UtcNow;
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
      List<ImsCacheIdentitiesInScope> list = values.Where<KeyValuePair<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>>((Func<KeyValuePair<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>, bool>) (x => x.Value != null && x.Value.Count > settings.IdentitesInScopeCountThresholdBeyondWhichCache)).Select<KeyValuePair<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>, ImsCacheIdentitiesInScope>((Func<KeyValuePair<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>, ImsCacheIdentitiesInScope>) (x => new ImsCacheIdentitiesInScope(new ImsCacheIdKey(x.Key), x.Value, now))).ToList<ImsCacheIdentitiesInScope>();
      if (!list.Any<ImsCacheIdentitiesInScope>())
        return;
      if (this.IsLocalCacheEnabledForIdentitiesInScope(context))
        context.GetService<IImsLocalDataCache>().AddObjects<ImsCacheIdentitiesInScope>(context, (IEnumerable<ImsCacheIdentitiesInScope>) list);
      if (!this.IsRemoteCacheEnabledForIdentitiesInScope(context))
        return;
      context.GetService<IImsRemoteCache>().AddObjects<ImsCacheIdentitiesInScope>(context, (IEnumerable<ImsCacheIdentitiesInScope>) list);
    }

    private bool IsIdentitiesInScopeDisabled(IVssRequestContext context) => !ImsCacheService.IsFeatureEnabled(context) || !this.IsOperationSupported(ImsOperation.IdentitiesInScope) || context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.ImsCacheService.IdentitiesByScope.Disable");

    private bool IsLocalCacheEnabledForIdentitiesInScope(IVssRequestContext context) => this.m_supportedLocalCacheOperations.HasFlag((Enum) ImsOperation.IdentitiesInScope) && !context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.LocalDataCache.IdentitiesByScope.Disable");

    private bool IsRemoteCacheEnabledForIdentitiesInScope(IVssRequestContext context) => this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.IdentitiesInScope) && context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.IdentitiesByScope");

    public bool ProcessChanges(
      IVssRequestContext context,
      Guid scopeId,
      ICollection<MembershipChangeInfo> membershipChanges)
    {
      this.ValidateRequestContext(context);
      if (!ImsCacheService.IsFeatureEnabled(context))
        return false;
      if (membershipChanges.IsNullOrEmpty<MembershipChangeInfo>())
        return true;
      List<ImsCacheIdKey> list1 = membershipChanges.Where<MembershipChangeInfo>((Func<MembershipChangeInfo, bool>) (x => x != null && x.ContainerId != Guid.Empty)).Select<MembershipChangeInfo, ImsCacheIdKey>((Func<MembershipChangeInfo, ImsCacheIdKey>) (x => new ImsCacheIdKey(x.ContainerId))).ToList<ImsCacheIdKey>();
      bool flag = true;
      try
      {
        if (!this.m_supportedLocalCacheOperations.HasFlag((Enum) ImsOperation.Children))
        {
          if (!this.m_supportedLocalCacheOperations.HasFlag((Enum) ImsOperation.Descendants))
            goto label_9;
        }
        context.GetService<IImsLocalDataCache>().RemoveObjects<ImsCacheIdKey, ImsCacheChildren>(context, (ICollection<ImsCacheIdKey>) list1);
      }
      catch (Exception ex)
      {
        context.TraceException(1754807, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), ex);
        flag = false;
      }
label_9:
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
      try
      {
        if (!this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.Children))
        {
          if (!this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.Descendants))
            goto label_14;
        }
        DateTimeOffset now = DateTimeOffset.UtcNow;
        IImsRemoteCache service = context.GetService<IImsRemoteCache>();
        IDictionary<ImsCacheIdKey, ImsCacheChildren> cacheValues = service.GetObjects<ImsCacheIdKey, ImsCacheChildren>(context, (ICollection<ImsCacheIdKey>) list1);
        List<ImsCacheIdKey> list2 = list1.Where<ImsCacheIdKey>((Func<ImsCacheIdKey, bool>) (key => !ImsCacheService.IsCacheMiss((ImsCacheObject) cacheValues.TryGetOrDefault<ImsCacheIdKey, ImsCacheChildren>(key), settings.ChildrenTimeToLive, now))).ToList<ImsCacheIdKey>();
        service.RemoveObjects<ImsCacheIdKey, ImsCacheChildren>(context, (ICollection<ImsCacheIdKey>) list2);
      }
      catch (Exception ex)
      {
        context.TraceException(1754808, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), ex);
        flag = false;
      }
label_14:
      try
      {
        if (!this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.IdentitiesByDescriptor))
        {
          if (!this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.IdentitiesById))
            goto label_23;
        }
        DateTimeOffset now = DateTimeOffset.UtcNow;
        IVssRequestContext masterDomainContext = ImsCacheService.GetMasterDomainContext(context);
        IImsRemoteCache service = masterDomainContext.GetService<IImsRemoteCache>();
        List<ImsCacheScopedIdKey> list3 = membershipChanges.Where<MembershipChangeInfo>((Func<MembershipChangeInfo, bool>) (x => x != null && !x.IsMemberGroup && x.MemberId != Guid.Empty)).Select<MembershipChangeInfo, ImsCacheScopedIdKey>((Func<MembershipChangeInfo, ImsCacheScopedIdKey>) (x => new ImsCacheScopedIdKey(new ScopedId(scopeId, x.MemberId)))).ToList<ImsCacheScopedIdKey>();
        if (context.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.ScopeMembershipAtOrgLevel"))
        {
          List<ImsCacheScopedIdKey> list4 = membershipChanges.Where<MembershipChangeInfo>((Func<MembershipChangeInfo, bool>) (x => x != null && !x.IsMemberGroup && x.MemberId != Guid.Empty && x.ContainerScopeId != Guid.Empty && x.ContainerScopeId != scopeId)).Select<MembershipChangeInfo, ImsCacheScopedIdKey>((Func<MembershipChangeInfo, ImsCacheScopedIdKey>) (x => new ImsCacheScopedIdKey(new ScopedId(x.ContainerScopeId, x.MemberId)))).ToList<ImsCacheScopedIdKey>();
          list3.AddRange((IEnumerable<ImsCacheScopedIdKey>) list4);
        }
        if (list3.Count > 1)
        {
          IDictionary<ImsCacheScopedIdKey, ImsCacheScopeMembership> cachedScopeMemberships = service.GetObjects<ImsCacheScopedIdKey, ImsCacheScopeMembership>(masterDomainContext, (ICollection<ImsCacheScopedIdKey>) list3);
          list3 = list3.Where<ImsCacheScopedIdKey>((Func<ImsCacheScopedIdKey, bool>) (key => !ImsCacheService.IsCacheMiss((ImsCacheObject) cachedScopeMemberships.TryGetOrDefault<ImsCacheScopedIdKey, ImsCacheScopeMembership>(key), settings.ScopeMembershipTimeToLive, now))).ToList<ImsCacheScopedIdKey>();
        }
        service.RemoveObjects<ImsCacheScopedIdKey, ImsCacheScopeMembership>(masterDomainContext, (ICollection<ImsCacheScopedIdKey>) list3);
      }
      catch (Exception ex)
      {
        context.TraceException(1754809, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), ex);
        flag = false;
      }
label_23:
      return flag;
    }

    public bool ProcessChanges(
      IVssRequestContext context,
      Guid scopeId,
      IList<Guid> identityChanges)
    {
      context.TraceEnter(1754811, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), nameof (ProcessChanges));
      this.ValidateRequestContext(context);
      if (!ImsCacheService.IsFeatureEnabled(context))
        return false;
      if (identityChanges.IsNullOrEmpty<Guid>())
        return true;
      List<ImsCacheIdKey> list1 = identityChanges.Select<Guid, ImsCacheIdKey>((Func<Guid, ImsCacheIdKey>) (x => new ImsCacheIdKey(x))).ToList<ImsCacheIdKey>();
      bool flag1 = true;
      try
      {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
        if (this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.IdentitiesByDescriptor) || this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.IdentitiesByDisplayName) || this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.IdentitiesByAccountName))
        {
          int num = context.ServiceHost.Is(TeamFoundationHostType.Deployment) ? 0 : (this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.IdentitiesByAccountName) ? 1 : 0);
          bool flag2 = !context.ServiceHost.Is(TeamFoundationHostType.Deployment) && this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.IdentitiesByDisplayName);
          IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
          IImsRemoteCache service1 = context1.GetService<IImsRemoteCache>();
          IImsRemoteCache service2 = context.GetService<IImsRemoteCache>();
          IDictionary<ImsCacheIdKey, ImsCacheIdentity> cachedIdentities = service1.GetObjects<ImsCacheIdKey, ImsCacheIdentity>(context1, (ICollection<ImsCacheIdKey>) list1);
          List<ImsCacheIdKey> list2 = list1.Where<ImsCacheIdKey>((Func<ImsCacheIdKey, bool>) (key => !ImsCacheService.IsCacheMiss((ImsCacheObject) cachedIdentities.TryGetOrDefault<ImsCacheIdKey, ImsCacheIdentity>(key), settings.IdentityTimeToLive, now))).ToList<ImsCacheIdKey>();
          List<ImsCacheScopedNameKey> keys1 = new List<ImsCacheScopedNameKey>();
          if (num != 0)
          {
            keys1 = list1.SelectMany<ImsCacheIdKey, ImsCacheScopedNameKey>((Func<ImsCacheIdKey, IEnumerable<ImsCacheScopedNameKey>>) (key =>
            {
              ImsCacheIdentity imsCacheIdentity;
              if (!cachedIdentities.TryGetValue(key, out imsCacheIdentity) || string.IsNullOrEmpty(imsCacheIdentity?.Value?.GetProperty<string>("Account", string.Empty)))
                return (IEnumerable<ImsCacheScopedNameKey>) Array.Empty<ImsCacheScopedNameKey>();
              Microsoft.VisualStudio.Services.Identity.Identity identity = imsCacheIdentity.Value;
              return (IEnumerable<ImsCacheScopedNameKey>) new ImsCacheScopedNameKey[2]
              {
                new ImsCacheScopedNameKey(new ScopedKey(scopeId, identity.GetProperty<string>("Account", string.Empty))),
                new ImsCacheScopedNameKey(new ScopedKey(scopeId, identity.GetProperty<string>("Domain", string.Empty) + "\\" + identity.GetProperty<string>("Account", string.Empty)))
              };
            })).Where<ImsCacheScopedNameKey>((Func<ImsCacheScopedNameKey, bool>) (x => x != null)).ToList<ImsCacheScopedNameKey>();
            if (keys1.Count > 1)
              keys1 = service2.GetObjects<ImsCacheScopedNameKey, ImsCacheIdentitiesByAccountName>(context, (ICollection<ImsCacheScopedNameKey>) keys1).Where<KeyValuePair<ImsCacheScopedNameKey, ImsCacheIdentitiesByAccountName>>((Func<KeyValuePair<ImsCacheScopedNameKey, ImsCacheIdentitiesByAccountName>, bool>) (kvp => !ImsCacheService.IsCacheMiss((ImsCacheObject) kvp.Value, settings.AccountNameQueryResultsTimeToLive, now))).Select<KeyValuePair<ImsCacheScopedNameKey, ImsCacheIdentitiesByAccountName>, ImsCacheScopedNameKey>((Func<KeyValuePair<ImsCacheScopedNameKey, ImsCacheIdentitiesByAccountName>, ImsCacheScopedNameKey>) (kvp => kvp.Key)).ToList<ImsCacheScopedNameKey>();
          }
          List<ImsCacheScopedNameKey> keys2 = new List<ImsCacheScopedNameKey>();
          if (flag2)
          {
            ImsCacheIdentity imsCacheIdentity;
            keys2 = list1.Select<ImsCacheIdKey, ImsCacheScopedNameKey>((Func<ImsCacheIdKey, ImsCacheScopedNameKey>) (key => !cachedIdentities.TryGetValue(key, out imsCacheIdentity) || string.IsNullOrEmpty(imsCacheIdentity?.Value?.DisplayName) ? (ImsCacheScopedNameKey) null : new ImsCacheScopedNameKey(new ScopedKey(scopeId, imsCacheIdentity.Value.DisplayName)))).Where<ImsCacheScopedNameKey>((Func<ImsCacheScopedNameKey, bool>) (x => x != null)).ToList<ImsCacheScopedNameKey>();
            if (keys2.Count > 1)
              keys2 = service2.GetObjects<ImsCacheScopedNameKey, ImsCacheIdentitiesByDisplayName>(context, (ICollection<ImsCacheScopedNameKey>) keys2).Where<KeyValuePair<ImsCacheScopedNameKey, ImsCacheIdentitiesByDisplayName>>((Func<KeyValuePair<ImsCacheScopedNameKey, ImsCacheIdentitiesByDisplayName>, bool>) (kvp => !ImsCacheService.IsCacheMiss((ImsCacheObject) kvp.Value, settings.DisplayNameQueryResultsTimeToLive, now))).Select<KeyValuePair<ImsCacheScopedNameKey, ImsCacheIdentitiesByDisplayName>, ImsCacheScopedNameKey>((Func<KeyValuePair<ImsCacheScopedNameKey, ImsCacheIdentitiesByDisplayName>, ImsCacheScopedNameKey>) (kvp => kvp.Key)).ToList<ImsCacheScopedNameKey>();
          }
          service1.RemoveObjects<ImsCacheIdKey, ImsCacheIdentity>(context1, (ICollection<ImsCacheIdKey>) list2);
          service2.RemoveObjects<ImsCacheScopedNameKey, ImsCacheIdentitiesByAccountName>(context, (ICollection<ImsCacheScopedNameKey>) keys1);
          service2.RemoveObjects<ImsCacheScopedNameKey, ImsCacheIdentitiesByDisplayName>(context, (ICollection<ImsCacheScopedNameKey>) keys2);
        }
        if (!context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          if (this.m_supportedRemoteCacheOperations.HasFlag((Enum) ImsOperation.IdentitiesByDescriptor))
          {
            IVssRequestContext masterDomainContext = ImsCacheService.GetMasterDomainContext(context);
            IImsRemoteCache service = masterDomainContext.GetService<IImsRemoteCache>();
            IList<ImsCacheScopedIdKey> keys = identityChanges.Count <= 1 ? (IList<ImsCacheScopedIdKey>) list1.Select<ImsCacheIdKey, ImsCacheScopedIdKey>((Func<ImsCacheIdKey, ImsCacheScopedIdKey>) (x => new ImsCacheScopedIdKey(new ScopedId(scopeId, x.Id)))).ToList<ImsCacheScopedIdKey>() : (IList<ImsCacheScopedIdKey>) service.GetObjects<ImsCacheScopedIdKey, ImsCacheScopeMembership>(masterDomainContext, (ICollection<ImsCacheScopedIdKey>) list1.Select<ImsCacheIdKey, ImsCacheScopedIdKey>((Func<ImsCacheIdKey, ImsCacheScopedIdKey>) (x => new ImsCacheScopedIdKey(new ScopedId(scopeId, x.Id)))).ToList<ImsCacheScopedIdKey>()).Where<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>>((Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, bool>) (kvp => !ImsCacheService.IsCacheMiss((ImsCacheObject) kvp.Value, settings.ScopeMembershipTimeToLive, now))).Select<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheScopedIdKey>((Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheScopedIdKey>) (kvp => kvp.Key)).ToList<ImsCacheScopedIdKey>();
            service.RemoveObjects<ImsCacheScopedIdKey, ImsCacheScopeMembership>(masterDomainContext, (ICollection<ImsCacheScopedIdKey>) keys);
          }
        }
      }
      catch (Exception ex)
      {
        context.TraceException(1754818, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), ex);
        flag1 = false;
      }
      context.TraceLeave(1754819, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), nameof (ProcessChanges));
      return flag1;
    }

    public void ProcessChangesOnSearchCaches(
      IVssRequestContext context,
      Guid scopeId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> newlyAddedIdentities)
    {
      if (!IdentitySearchHelper.IsSearchIdentitiesContextValid(context) || !this.IsOperationSupported(ImsOperation.IdentityIdsByDisplayNamePrefixSearch) && !this.IsOperationSupported(ImsOperation.IdentityIdsByEmailPrefixSearch) && !this.IsOperationSupported(ImsOperation.IdentityIdsByAccountNamePrefixSearch) && !this.IsOperationSupported(ImsOperation.IdentityIdsByDomainAccountNamePrefixSearch))
        return;
      IImsRemoteCache service1 = context.GetService<IImsRemoteCache>();
      IImsLocalSearchCache service2 = context.GetService<IImsLocalSearchCache>();
      ImsCacheIdKey imsCacheIdKey = new ImsCacheIdKey(scopeId);
      if (this.IsOperationSupported(ImsOperation.IdentityIdsByDisplayNamePrefixSearch))
      {
        service1.RemoveObjects<ImsCacheIdKey, ImsCacheDisplayNameSearchIndex>(context, (ICollection<ImsCacheIdKey>) new ImsCacheIdKey[1]
        {
          imsCacheIdKey
        });
        service2.AddToIndex<ImsCacheIdentityIdByDisplayName>(context, scopeId, (ICollection<ImsCacheIdentityIdByDisplayName>) newlyAddedIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByDisplayName>((Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByDisplayName>) (x => ImsCacheService.Functions.SearchByDisplayName.CreateLocalCacheEntryFromIdentity()(x))).ToNullFilteredList<ImsCacheIdentityIdByDisplayName>());
      }
      if (this.IsOperationSupported(ImsOperation.IdentityIdsByEmailPrefixSearch))
      {
        service1.RemoveObjects<ImsCacheIdKey, ImsCacheEmailSearchIndex>(context, (ICollection<ImsCacheIdKey>) new ImsCacheIdKey[1]
        {
          imsCacheIdKey
        });
        service2.AddToIndex<ImsCacheIdentityIdByEmail>(context, scopeId, (ICollection<ImsCacheIdentityIdByEmail>) newlyAddedIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByEmail>((Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByEmail>) (x => ImsCacheService.Functions.SearchByEmailPrefix.CreateLocalCacheEntryFromIdentity()(x))).ToNullFilteredList<ImsCacheIdentityIdByEmail>());
      }
      if (this.IsOperationSupported(ImsOperation.IdentityIdsByAccountNamePrefixSearch))
      {
        service1.RemoveObjects<ImsCacheIdKey, ImsCacheAccountNameSearchIndex>(context, (ICollection<ImsCacheIdKey>) new ImsCacheIdKey[1]
        {
          imsCacheIdKey
        });
        service2.AddToIndex<ImsCacheIdentityIdByAccountName>(context, scopeId, (ICollection<ImsCacheIdentityIdByAccountName>) newlyAddedIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByAccountName>((Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByAccountName>) (x => ImsCacheService.Functions.SearchByAccountName.CreateLocalCacheEntryFromIdentity()(x))).ToNullFilteredList<ImsCacheIdentityIdByAccountName>());
      }
      if (!this.IsOperationSupported(ImsOperation.IdentityIdsByDomainAccountNamePrefixSearch))
        return;
      service1.RemoveObjects<ImsCacheIdKey, ImsCacheDomainAccountNameSearchIndex>(context, (ICollection<ImsCacheIdKey>) new ImsCacheIdKey[1]
      {
        imsCacheIdKey
      });
      service2.AddToIndex<ImsCacheIdentityIdByDomainAccountName>(context, scopeId, (ICollection<ImsCacheIdentityIdByDomainAccountName>) newlyAddedIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByDomainAccountName>((Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByDomainAccountName>) (x => ImsCacheService.Functions.SearchByDomainAccountName.CreateLocalCacheEntryFromIdentity()(x))).ToNullFilteredList<ImsCacheIdentityIdByDomainAccountName>());
    }

    public virtual IList<Guid> GetMruIdentityIds(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId)
    {
      this.ValidateRequestContext(context);
      if (context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new InvalidRequestContextHostException(FrameworkResources.ApplicationHostRequired());
      if (!ImsCacheService.IsFeatureEnabled(context))
        return (IList<Guid>) null;
      IImsRemoteCache service = context.GetService<IImsRemoteCache>();
      ImsCacheScopedIdKey key = new ImsCacheScopedIdKey(new ScopedId(containerId, identityId));
      IVssRequestContext context1 = context;
      ImsCacheScopedIdKey[] keys = new ImsCacheScopedIdKey[1]
      {
        key
      };
      ImsCacheMruIdentityIds cacheMruIdentityIds = service.GetObjects<ImsCacheScopedIdKey, ImsCacheMruIdentityIds>(context1, (ICollection<ImsCacheScopedIdKey>) keys)[key];
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
      return ImsCacheService.IsCacheMiss((ImsCacheObject) cacheMruIdentityIds, settings.MruIdentityIdsTimeToLive, DateTimeOffset.UtcNow) ? (IList<Guid>) null : cacheMruIdentityIds.Value;
    }

    public virtual void SetMruIdentityIds(
      IVssRequestContext context,
      Guid identityId,
      Guid containerId,
      List<Guid> values)
    {
      this.ValidateRequestContext(context);
      if (context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new InvalidRequestContextHostException(FrameworkResources.ApplicationHostRequired());
      if (!ImsCacheService.IsFeatureEnabled(context))
        return;
      ArgumentUtility.CheckForNull<List<Guid>>(values, nameof (values));
      IImsRemoteCache service = context.GetService<IImsRemoteCache>();
      ImsCacheMruIdentityIds cacheMruIdentityIds = new ImsCacheMruIdentityIds(new ImsCacheScopedIdKey(new ScopedId(containerId, identityId)), (IList<Guid>) values, DateTimeOffset.UtcNow);
      IVssRequestContext context1 = context;
      ImsCacheMruIdentityIds[] objects = new ImsCacheMruIdentityIds[1]
      {
        cacheMruIdentityIds
      };
      service.AddObjects<ImsCacheMruIdentityIds>(context1, (IEnumerable<ImsCacheMruIdentityIds>) objects);
    }

    public void RemoveMruIdentityIds(IVssRequestContext context, Guid identityId, Guid containerId)
    {
      this.ValidateRequestContext(context);
      if (context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new InvalidRequestContextHostException(FrameworkResources.ApplicationHostRequired());
      if (!ImsCacheService.IsFeatureEnabled(context))
        return;
      IImsRemoteCache service = context.GetService<IImsRemoteCache>();
      ImsCacheScopedIdKey cacheScopedIdKey = new ImsCacheScopedIdKey(new ScopedId(containerId, identityId));
      IVssRequestContext context1 = context;
      ImsCacheScopedIdKey[] keys = new ImsCacheScopedIdKey[1]
      {
        cacheScopedIdKey
      };
      service.RemoveObjects<ImsCacheScopedIdKey, ImsCacheMruIdentityIds>(context1, (ICollection<ImsCacheScopedIdKey>) keys);
    }

    public virtual void CreateSearchIndexByDisplayName(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap)
    {
      this.CreateSearchIndex<ImsCacheIdentityIdByDisplayName, ImsCacheDisplayNameSearchIndex>(context, scopeId, valuesMap, ImsOperation.IdentityIdsByDisplayNamePrefixSearch, ImsCacheService.Functions.SearchByDisplayName.CreateKey(), ImsCacheService.Functions.SearchByDisplayName.CreateLocalCacheEntry(), (Func<ImsCacheIdKey, IList<ImsCacheIdentityIdByDisplayName>, DateTimeOffset, ImsCacheDisplayNameSearchIndex>) ((x, y, z) => new ImsCacheDisplayNameSearchIndex(x, y, z)), ImsCacheService.ImsCacheSearchOperationPerformanceCounters.SearchByDisplayName);
    }

    public virtual void CreateSearchIndexByAppId(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap)
    {
      this.CreateSearchIndex<ImsCacheIdentityIdByAppId, ImsCacheAppIdSearchIndex>(context, scopeId, valuesMap, ImsOperation.IdentityIdsByAppIdSearch, ImsCacheService.Functions.SearchByAppId.CreateKey(), ImsCacheService.Functions.SearchByAppId.CreateLocalCacheEntry(), (Func<ImsCacheIdKey, IList<ImsCacheIdentityIdByAppId>, DateTimeOffset, ImsCacheAppIdSearchIndex>) ((x, y, z) => new ImsCacheAppIdSearchIndex(x, y, z)), ImsCacheService.ImsCacheSearchOperationPerformanceCounters.SearchByAppId);
    }

    public virtual void CreateSearchIndexByEmail(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap)
    {
      this.CreateSearchIndex<ImsCacheIdentityIdByEmail, ImsCacheEmailSearchIndex>(context, scopeId, valuesMap, ImsOperation.IdentityIdsByEmailPrefixSearch, ImsCacheService.Functions.SearchByEmailPrefix.CreateKey(), ImsCacheService.Functions.SearchByEmailPrefix.CreateLocalCacheEntry(), (Func<ImsCacheIdKey, IList<ImsCacheIdentityIdByEmail>, DateTimeOffset, ImsCacheEmailSearchIndex>) ((x, y, z) => new ImsCacheEmailSearchIndex(x, y, z)), ImsCacheService.ImsCacheSearchOperationPerformanceCounters.SearchByEmail);
    }

    public virtual void CreateSearchIndexByAccountName(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap)
    {
      this.CreateSearchIndex<ImsCacheIdentityIdByAccountName, ImsCacheAccountNameSearchIndex>(context, scopeId, valuesMap, ImsOperation.IdentityIdsByAccountNamePrefixSearch, ImsCacheService.Functions.SearchByAccountName.CreateKey(), ImsCacheService.Functions.SearchByAccountName.CreateLocalCacheEntry(), (Func<ImsCacheIdKey, IList<ImsCacheIdentityIdByAccountName>, DateTimeOffset, ImsCacheAccountNameSearchIndex>) ((x, y, z) => new ImsCacheAccountNameSearchIndex(x, y, z)), ImsCacheService.ImsCacheSearchOperationPerformanceCounters.SearchByAccountName);
    }

    public virtual void CreateSearchIndexByDomainAccountName(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap)
    {
      this.CreateSearchIndex<ImsCacheIdentityIdByDomainAccountName, ImsCacheDomainAccountNameSearchIndex>(context, scopeId, valuesMap, ImsOperation.IdentityIdsByDomainAccountNamePrefixSearch, ImsCacheService.Functions.SearchByDomainAccountName.CreateKey(), ImsCacheService.Functions.SearchByDomainAccountName.CreateLocalCacheEntry(), (Func<ImsCacheIdKey, IList<ImsCacheIdentityIdByDomainAccountName>, DateTimeOffset, ImsCacheDomainAccountNameSearchIndex>) ((x, y, z) => new ImsCacheDomainAccountNameSearchIndex(x, y, z)), ImsCacheService.ImsCacheSearchOperationPerformanceCounters.SearchByDomainAccountName);
    }

    private void CreateSearchIndex<L, R>(
      IVssRequestContext context,
      Guid scopeId,
      IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId> valuesMap,
      ImsOperation operation,
      Func<Microsoft.VisualStudio.Services.Identity.Identity, string> createSearchKey,
      Func<ImsCacheStringKey, IdentityId, L> createLocalCacheEntry,
      Func<ImsCacheIdKey, IList<L>, DateTimeOffset, R> createRemoteCacheEntry,
      ImsCacheService.ImsCacheSearchOperationPerformanceCounters perfCounters)
      where L : ImsCacheObject<string, IdentityId>
      where R : ImsCacheObject
    {
      context.TraceEnter(1754500, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), nameof (CreateSearchIndex));
      this.ValidateRequestContext(context);
      IdentitySearchHelper.ValidateSearchIdentitiesContext(context);
      bool flag1 = ImsCacheService.IsFeatureEnabled(context);
      bool flag2 = this.IsOperationSupported(operation);
      if (!flag1 || !flag2)
      {
        context.Trace(1754502, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), "skipping creation search index for scopeId {0}, operation {1}. IsFeatureEnabled: {2}, IsOperationSupported: {3}", (object) scopeId, (object) operation, (object) flag1, (object) flag2);
      }
      else
      {
        ArgumentUtility.CheckForNull<IDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>>(valuesMap, nameof (valuesMap));
        if (valuesMap.Keys.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (obj => obj == null)) || valuesMap.Values.Any<IdentityId>((Func<IdentityId, bool>) (obj => obj == null)))
          throw new ArgumentNullException(nameof (valuesMap), "Attempt to add null object, or an object with null key or value.");
        IList<L> nullFilteredList = valuesMap.Where<KeyValuePair<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>>((Func<KeyValuePair<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>, bool>) (x => !string.IsNullOrWhiteSpace(createSearchKey(x.Key)))).Select<KeyValuePair<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>, L>((Func<KeyValuePair<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>, L>) (x => createLocalCacheEntry(new ImsCacheStringKey(createSearchKey(x.Key)), x.Value))).ToNullFilteredList<L>();
        if (this.m_supportedRemoteCacheOperations.HasFlag((Enum) operation))
        {
          ImsCacheIdKey imsCacheIdKey = new ImsCacheIdKey(scopeId);
          IImsRemoteCache service = context.GetService<IImsRemoteCache>();
          R r = createRemoteCacheEntry(imsCacheIdKey, nullFilteredList, DateTimeOffset.UtcNow);
          IVssRequestContext context1 = context;
          R[] objects = new R[1]{ r };
          service.AddObjects<R>(context1, (IEnumerable<R>) objects);
          context.Trace(1754504, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), "Creating search index for ScopeId {0} for remote cache finished.", (object) scopeId);
        }
        if (this.m_supportedLocalCacheOperations.HasFlag((Enum) operation))
        {
          context.GetService<IImsLocalSearchCache>().CreateIndex<L>(context, scopeId, (ICollection<L>) nullFilteredList, DateTimeOffset.UtcNow);
          context.Trace(1754506, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), "Creating search index for ScopeId {0} for local cache finished.", (object) scopeId);
        }
        perfCounters.CreateIndexRequests.Increment();
        perfCounters.CreateIndexRequestsPerSecond.Increment();
        context.TraceLeave(1754508, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), nameof (CreateSearchIndex));
      }
    }

    public virtual IEnumerable<IdentityId> SearchIdentityIdsByDisplayName(
      IVssRequestContext context,
      Guid scopeId,
      string displayNamePrefix,
      out bool isStale)
    {
      return this.SearchIdentityIds<ImsCacheIdentityIdByDisplayName, ImsCacheDisplayNameSearchIndex>(context, scopeId, displayNamePrefix, ImsOperation.IdentityIdsByDisplayNamePrefixSearch, ImsCacheService.ImsCacheSearchOperationPerformanceCounters.SearchByDisplayName, out isStale);
    }

    public virtual IEnumerable<IdentityId> SearchIdentityIdsByEmail(
      IVssRequestContext context,
      Guid scopeId,
      string emailPrefix,
      out bool isStale)
    {
      return this.SearchIdentityIds<ImsCacheIdentityIdByEmail, ImsCacheEmailSearchIndex>(context, scopeId, emailPrefix, ImsOperation.IdentityIdsByEmailPrefixSearch, ImsCacheService.ImsCacheSearchOperationPerformanceCounters.SearchByEmail, out isStale);
    }

    public virtual IEnumerable<IdentityId> SearchIdentityIdsByAccountName(
      IVssRequestContext context,
      Guid scopeId,
      string accountNamePrefix,
      out bool isStale)
    {
      return this.SearchIdentityIds<ImsCacheIdentityIdByAccountName, ImsCacheAccountNameSearchIndex>(context, scopeId, accountNamePrefix, ImsOperation.IdentityIdsByAccountNamePrefixSearch, ImsCacheService.ImsCacheSearchOperationPerformanceCounters.SearchByAccountName, out isStale);
    }

    public virtual IEnumerable<IdentityId> SearchIdentityIdsByDomainAccountName(
      IVssRequestContext context,
      Guid scopeId,
      string domainAccountNamePrefix,
      out bool isStale)
    {
      return this.SearchIdentityIds<ImsCacheIdentityIdByDomainAccountName, ImsCacheDomainAccountNameSearchIndex>(context, scopeId, domainAccountNamePrefix, ImsOperation.IdentityIdsByDomainAccountNamePrefixSearch, ImsCacheService.ImsCacheSearchOperationPerformanceCounters.SearchByDomainAccountName, out isStale);
    }

    public virtual IEnumerable<IdentityId> SearchIdentityIdsByAppId(
      IVssRequestContext context,
      Guid scopeId,
      string appId,
      out bool isStale,
      out bool inputParameterError)
    {
      inputParameterError = false;
      if (Guid.TryParse(appId, out Guid _))
        return this.SearchIdentityIds<ImsCacheIdentityIdByAppId, ImsCacheAppIdSearchIndex>(context, scopeId, appId, ImsOperation.IdentityIdsByAppIdSearch, ImsCacheService.ImsCacheSearchOperationPerformanceCounters.SearchByAppId, out isStale);
      isStale = false;
      inputParameterError = true;
      return (IEnumerable<IdentityId>) new List<IdentityId>();
    }

    private IEnumerable<IdentityId> SearchIdentityIds<L, R>(
      IVssRequestContext context,
      Guid scopeId,
      string prefix,
      ImsOperation operation,
      ImsCacheService.ImsCacheSearchOperationPerformanceCounters counters,
      out bool isStale)
      where L : ImsCacheObject<string, IdentityId>
      where R : ImsCacheObject<Guid, IList<L>>
    {
      isStale = false;
      this.ValidateRequestContext(context);
      IdentitySearchHelper.ValidateSearchIdentitiesContext(context);
      context.Trace(1754820, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), "scopeId={0}, prefix={1}", (object) scopeId, (object) prefix);
      if (!ImsCacheService.IsFeatureEnabled(context) || !this.IsOperationSupported(operation))
        return (IEnumerable<IdentityId>) null;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(prefix, nameof (prefix));
      VssPerformanceCounter performanceCounter = counters.SearchRequests;
      performanceCounter.Increment();
      performanceCounter = counters.SearchRequestsPerSecond;
      performanceCounter.Increment();
      bool flag = true;
      try
      {
        IImsLocalSearchCache service1 = context.GetService<IImsLocalSearchCache>();
        if (service1.HasIndex<L>(context, scopeId))
        {
          IDictionary<string, IEnumerable<IdentityId>> dictionary = service1.SearchIndex<L>(context, scopeId, (ICollection<string>) new string[1]
          {
            prefix
          });
          isStale = flag = service1.IsIndexStale<L>(context, scopeId);
          context.Trace(1754821, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), "cachedValueMap Count={0} isStale={1}", (object) dictionary.Count, (object) isStale);
          return dictionary[prefix];
        }
        if (!this.m_supportedRemoteCacheOperations.HasFlag((Enum) operation))
          return (IEnumerable<IdentityId>) null;
        IImsRemoteCache service2 = context.GetService<IImsRemoteCache>();
        ImsCacheIdKey key = new ImsCacheIdKey(scopeId);
        IVssRequestContext context1 = context;
        ImsCacheIdKey[] keys = new ImsCacheIdKey[1]{ key };
        R r = service2.GetObjects<ImsCacheIdKey, R>(context1, (ICollection<ImsCacheIdKey>) keys)[key];
        context.Trace(1754822, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), "fetched search index from remote cache");
        if ((object) r == null || r.Value == null)
          return (IEnumerable<IdentityId>) null;
        TimeSpan searchCacheTimeToLive = this.GetSettings(context).IndexSearchCacheSettings.GetSearchCacheTimeToLive(r.Value.Count);
        isStale = flag = r.IsExpired(searchCacheTimeToLive, DateTimeOffset.UtcNow);
        service1.CreateIndex<L>(context, scopeId, (ICollection<L>) r.Value.Where<L>((Func<L, bool>) (x => (object) x != null)).ToList<L>(), r.Time);
        context.Trace(1754823, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), "localCache count={0} isMiss={1}", (object) r.Value.Count, (object) flag);
        return r.Value.Where<L>((Func<L, bool>) (x => (object) x != null && x.Key.Id != null && x.Key.Id.ToString().StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))).Select<L, IdentityId>((Func<L, IdentityId>) (x => x.Value));
      }
      finally
      {
        context.Trace(1754824, TraceLevel.Info, "Microsoft.VisualStudio.Services.Identity", nameof (ImsCacheService), "isMiss={0}", (object) flag);
        if (flag)
        {
          counters.SearchMisses.Increment();
          counters.SearchMissesPerSecond.IncrementBy(1L);
        }
        else
        {
          counters.SearchHits.Increment();
          counters.SearchHitsPerSecond.IncrementBy(1L);
        }
      }
    }

    public IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesByDisplayName(
      IVssRequestContext context,
      Guid scopeId,
      string displayName)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(displayName, nameof (displayName));
      if (!ImsCacheService.IsFeatureEnabled(context) || context.ServiceHost.Is(TeamFoundationHostType.Deployment) || !this.IsOperationSupported(ImsOperation.IdentitiesByDisplayName))
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      if (ImsCacheService.EmptyCacheItemRandomMiss())
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Requests");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Objects");
      performanceCounter.IncrementBy(1L);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.RequestsPerSecond");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.ObjectsPerSecond");
      performanceCounter.IncrementBy(1L);
      DateTimeOffset now = DateTimeOffset.UtcNow;
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
      ImsCacheIdentitiesByDisplayName identitiesByDisplayName = context.GetService<IImsCacheOrchestrator>().GetObjects<ImsCacheScopedNameKey, ImsCacheIdentitiesByDisplayName>(context, (ICollection<ImsCacheScopedNameKey>) new ImsCacheScopedNameKey[1]
      {
        new ImsCacheScopedNameKey(new ScopedKey(scopeId, displayName))
      }, (Func<ImsCacheIdentitiesByDisplayName, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.DisplayNameQueryResultsTimeToLive, now))).First<KeyValuePair<ImsCacheScopedNameKey, ImsCacheIdentitiesByDisplayName>>().Value;
      if (ImsCacheService.IsCacheMiss((ImsCacheObject) identitiesByDisplayName, settings.DisplayNameQueryResultsTimeToLive, now))
      {
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Misses");
        performanceCounter.Increment();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.MissesPerSecond");
        performanceCounter.IncrementBy(1L);
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      }
      if (!identitiesByDisplayName.Value.Any<Guid>())
      {
        if (context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.DisplayName.DontCacheNullResults"))
        {
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Misses");
          performanceCounter.Increment();
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.MissesPerSecond");
          performanceCounter.IncrementBy(1L);
          return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
        }
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Hits");
        performanceCounter.Increment();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.HitsPerSecond");
        performanceCounter.IncrementBy(1L);
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[0];
      }
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      IDictionary<ImsCacheIdKey, ImsCacheIdentity> objects = context1.GetService<IImsCacheOrchestrator>().GetObjects<ImsCacheIdKey, ImsCacheIdentity>(context1, (ICollection<ImsCacheIdKey>) identitiesByDisplayName.Value.Select<Guid, ImsCacheIdKey>((Func<Guid, ImsCacheIdKey>) (x => new ImsCacheIdKey(x))).ToList<ImsCacheIdKey>(), (Func<ImsCacheIdentity, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.IdentityTimeToLive, now)));
      IDictionary<ImsCacheIdKey, ImsCacheIdentity> source = this.FilterOutFrameworkIdentities<ImsCacheIdKey>(context, objects);
      if (source.Any<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>>((Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, bool>) (x => ImsCacheService.IsCacheMiss((ImsCacheObject) x.Value, settings.IdentityTimeToLive, now))))
      {
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Misses");
        performanceCounter.Increment();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.MissesPerSecond");
        performanceCounter.IncrementBy(1L);
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      }
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.Hits");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDisplayName.HitsPerSecond");
      performanceCounter.IncrementBy(1L);
      return source.Select<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x.Value.Value));
    }

    public void SetIdentitiesByDisplayName(
      IVssRequestContext context,
      Guid scopeId,
      string displayName,
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> values)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(displayName, nameof (displayName));
      if (!ImsCacheService.IsFeatureEnabled(context) || context.ServiceHost.Is(TeamFoundationHostType.Deployment) || !this.IsOperationSupported(ImsOperation.IdentitiesByDisplayName, false))
        return;
      values = this.FilterOutFrameworkIdentities(context, values);
      values = this.FilterOutInvalidCacheData(context, values);
      if (values == null || values.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null || x.MasterId == IdentityConstants.LinkedId || x.IsContainer)) || values.IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>() && context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.DisplayName.DontCacheNullResults"))
        return;
      DateTimeOffset now = DateTimeOffset.UtcNow;
      context.GetService<IImsRemoteCache>().AddObjects<ImsCacheIdentitiesByDisplayName>(context, (IEnumerable<ImsCacheIdentitiesByDisplayName>) new ImsCacheIdentitiesByDisplayName[1]
      {
        new ImsCacheIdentitiesByDisplayName(new ImsCacheScopedNameKey(new ScopedKey(scopeId, displayName)), (ISet<Guid>) new HashSet<Guid>(values.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id))), now)
      });
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      context1.GetService<IImsRemoteCache>().AddObjects<ImsCacheIdentity>(context1, values.Select<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentity>) (x => new ImsCacheIdentity(new ImsCacheIdKey(x.Id), x, now))));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDisplayName.Requests").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDisplayName.Objects").IncrementBy(1L);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDisplayName.RequestsPerSecond").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDisplayName.ObjectsPerSecond").IncrementBy(1L);
    }

    public IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesByAccountName(
      IVssRequestContext context,
      Guid scopeId,
      string accountName)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(accountName, nameof (accountName));
      if (!ImsCacheService.IsFeatureEnabled(context) || context.ServiceHost.Is(TeamFoundationHostType.Deployment) || !this.IsOperationSupported(ImsOperation.IdentitiesByAccountName))
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Requests");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Objects");
      performanceCounter.IncrementBy(1L);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.RequestsPerSecond");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.ObjectsPerSecond");
      performanceCounter.IncrementBy(1L);
      DateTimeOffset now = DateTimeOffset.UtcNow;
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
      ImsCacheIdentitiesByAccountName identitiesByAccountName = context.GetService<IImsCacheOrchestrator>().GetObjects<ImsCacheScopedNameKey, ImsCacheIdentitiesByAccountName>(context, (ICollection<ImsCacheScopedNameKey>) new ImsCacheScopedNameKey[1]
      {
        new ImsCacheScopedNameKey(new ScopedKey(scopeId, accountName))
      }, (Func<ImsCacheIdentitiesByAccountName, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.AccountNameQueryResultsTimeToLive, now))).First<KeyValuePair<ImsCacheScopedNameKey, ImsCacheIdentitiesByAccountName>>().Value;
      if (ImsCacheService.IsCacheMiss((ImsCacheObject) identitiesByAccountName, settings.AccountNameQueryResultsTimeToLive, now))
      {
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Misses");
        performanceCounter.Increment();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.MissesPerSecond");
        performanceCounter.IncrementBy(1L);
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      }
      if (!identitiesByAccountName.Value.Any<Guid>())
      {
        if (!context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.AccountName.CacheNullResults") || ImsCacheService.EmptyCacheItemRandomMiss() || ((IEnumerable<string>) ImsCacheService.s_wellKnownAccountNamePrefixesNotToCache).Any<string>((Func<string, bool>) (x => accountName.EndsWith(x))))
        {
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Misses");
          performanceCounter.Increment();
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.MissesPerSecond");
          performanceCounter.IncrementBy(1L);
          return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
        }
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Hits");
        performanceCounter.Increment();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.HitsPerSecond");
        performanceCounter.IncrementBy(1L);
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[0];
      }
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      IDictionary<ImsCacheIdKey, ImsCacheIdentity> objects = context1.GetService<IImsCacheOrchestrator>().GetObjects<ImsCacheIdKey, ImsCacheIdentity>(context1, (ICollection<ImsCacheIdKey>) identitiesByAccountName.Value.Select<Guid, ImsCacheIdKey>((Func<Guid, ImsCacheIdKey>) (x => new ImsCacheIdKey(x))).ToList<ImsCacheIdKey>(), (Func<ImsCacheIdentity, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.IdentityTimeToLive, now)));
      if (objects.Any<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>>((Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, bool>) (x => ImsCacheService.IsCacheMiss((ImsCacheObject) x.Value, settings.IdentityTimeToLive, now))))
      {
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Misses");
        performanceCounter.Increment();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.MissesPerSecond");
        performanceCounter.IncrementBy(1L);
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      }
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.Hits");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByAccountName.HitsPerSecond");
      performanceCounter.IncrementBy(1L);
      return this.FilterOutFrameworkIdentities<ImsCacheIdKey>(context, objects).Select<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x.Value.Value));
    }

    public void SetIdentitiesByAccountName(
      IVssRequestContext context,
      Guid scopeId,
      string accountName,
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> values)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(accountName, nameof (accountName));
      if (!ImsCacheService.IsFeatureEnabled(context) || context.ServiceHost.Is(TeamFoundationHostType.Deployment) || !this.IsOperationSupported(ImsOperation.IdentitiesByAccountName, false))
        return;
      values = this.FilterOutFrameworkIdentities(context, values);
      values = this.FilterOutInvalidCacheData(context, values);
      if (values == null || values.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null || x.MasterId == IdentityConstants.LinkedId || x.IsContainer)) || values.IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>() && !context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.AccountName.CacheNullResults"))
        return;
      DateTimeOffset now = DateTimeOffset.UtcNow;
      context.GetService<IImsRemoteCache>().AddObjects<ImsCacheIdentitiesByAccountName>(context, (IEnumerable<ImsCacheIdentitiesByAccountName>) new ImsCacheIdentitiesByAccountName[1]
      {
        new ImsCacheIdentitiesByAccountName(new ImsCacheScopedNameKey(new ScopedKey(scopeId, accountName)), (ISet<Guid>) new HashSet<Guid>(values.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id))), now)
      });
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      context1.GetService<IImsRemoteCache>().AddObjects<ImsCacheIdentity>(context1, values.Select<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentity>) (x => new ImsCacheIdentity(new ImsCacheIdKey(x.Id), x, now))));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByAccountName.Requests").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByAccountName.Objects").IncrementBy(1L);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByAccountName.RequestsPerSecond").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByAccountName.ObjectsPerSecond").IncrementBy(1L);
    }

    public virtual Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      IVssRequestContext context,
      Guid scopeId,
      ICollection<IdentityDescriptor> descriptorIds)
    {
      this.ValidateRequestContext(context);
      if (!ImsCacheService.IsFeatureEnabled(context) || context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.IdentitiesByDescriptor.Disable") || !this.IsOperationSupported(ImsOperation.IdentitiesByDescriptor))
        return (Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) null;
      descriptorIds = (ICollection<IdentityDescriptor>) descriptorIds.Distinct<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance).ToList<IdentityDescriptor>();
      int count = descriptorIds.Count;
      if (count == 0)
        return descriptorIds.ToDedupedDictionary<IdentityDescriptor, IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<IdentityDescriptor, IdentityDescriptor>) (x => x), (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) (x => (Microsoft.VisualStudio.Services.Identity.Identity) null), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.Requests");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.Objects");
      performanceCounter.IncrementBy((long) count);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.RequestsPerSecond");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.ObjectsPerSecond");
      performanceCounter.IncrementBy((long) count);
      DateTimeOffset now = DateTimeOffset.UtcNow;
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
      Dictionary<IdentityDescriptor, IdentityId> descriptorToIdentityIdMap = this.GetIdentityIds(context, (IEnumerable<IdentityDescriptor>) descriptorIds, (Func<ImsCacheIdentityId, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.IdentityIdTimeToLive, now)));
      if (descriptorToIdentityIdMap.IsNullOrEmpty<KeyValuePair<IdentityDescriptor, IdentityId>>())
        return descriptorIds.ToDedupedDictionary<IdentityDescriptor, IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<IdentityDescriptor, IdentityDescriptor>) (x => x), (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) (x => (Microsoft.VisualStudio.Services.Identity.Identity) null), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      List<ImsCacheScopedIdKey> list1 = descriptorIds.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => descriptorToIdentityIdMap.TryGetOrDefault<IdentityDescriptor, IdentityId>(x) != null)).Select<IdentityDescriptor, ImsCacheScopedIdKey>((Func<IdentityDescriptor, ImsCacheScopedIdKey>) (x => new ImsCacheScopedIdKey(new ScopedId(scopeId, descriptorToIdentityIdMap[x].Id)))).ToList<ImsCacheScopedIdKey>();
      IVssRequestContext masterDomainContext = ImsCacheService.GetMasterDomainContext(context);
      IDictionary<ImsCacheScopedIdKey, ImsCacheScopeMembership> scopeMembershipCachedValues = masterDomainContext.GetService<IImsCacheOrchestrator>().GetObjects<ImsCacheScopedIdKey, ImsCacheScopeMembership>(masterDomainContext, (ICollection<ImsCacheScopedIdKey>) list1, (Func<ImsCacheScopeMembership, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.ScopeMembershipTimeToLive, now)));
      List<ImsCacheIdKey> list2 = scopeMembershipCachedValues.Where<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>>((Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, bool>) (x => !ImsCacheService.IsCacheMiss((ImsCacheObject) x.Value, settings.ScopeMembershipTimeToLive, now))).Select<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheIdKey>((Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheIdKey>) (x => new ImsCacheIdKey(x.Key.Id.Id))).ToList<ImsCacheIdKey>();
      if (list2.IsNullOrEmpty<ImsCacheIdKey>())
        return descriptorIds.ToDedupedDictionary<IdentityDescriptor, IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<IdentityDescriptor, IdentityDescriptor>) (x => x), (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) (x => (Microsoft.VisualStudio.Services.Identity.Identity) null), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      IDictionary<ImsCacheIdKey, ImsCacheIdentity> identitiesWithValidScopeMembershipMap = context1.GetService<IImsCacheOrchestrator>().GetObjects<ImsCacheIdKey, ImsCacheIdentity>(context1, (ICollection<ImsCacheIdKey>) list2, (Func<ImsCacheIdentity, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.IdentityTimeToLive, now)));
      if (identitiesWithValidScopeMembershipMap.IsNullOrEmpty<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>>())
        return descriptorIds.ToDedupedDictionary<IdentityDescriptor, IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<IdentityDescriptor, IdentityDescriptor>) (x => x), (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) (x => (Microsoft.VisualStudio.Services.Identity.Identity) null), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      int cacheHits = 0;
      int cacheMisses = 0;
      Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> dedupedDictionary = descriptorIds.ToDedupedDictionary<IdentityDescriptor, IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<IdentityDescriptor, IdentityDescriptor>) (x => x), (Func<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) (x =>
      {
        IdentityId identityId;
        ImsCacheIdentity imsCacheIdentity;
        ImsCacheScopeMembership cacheScopeMembership;
        if (descriptorToIdentityIdMap.TryGetValue(x, out identityId) && identityId != null && identitiesWithValidScopeMembershipMap.TryGetValue(new ImsCacheIdKey(identityId.Id), out imsCacheIdentity) && !ImsCacheService.IsCacheMiss((ImsCacheObject) imsCacheIdentity, settings.IdentityTimeToLive, now) && scopeMembershipCachedValues.TryGetValue(new ImsCacheScopedIdKey(new ScopedId(scopeId, identityId.Id)), out cacheScopeMembership) && !ImsCacheService.IsCacheMiss((ImsCacheObject) cacheScopeMembership, settings.ScopeMembershipTimeToLive, now))
        {
          ++cacheHits;
          imsCacheIdentity.Value.IsActive = cacheScopeMembership.Value;
          return imsCacheIdentity.Value;
        }
        ++cacheMisses;
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      IEnumerable<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>> source = this.FilterOutFrameworkIdentities<IdentityDescriptor>(context, (IEnumerable<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>) dedupedDictionary);
      if (!(source is Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> dictionary))
        dictionary = source != null ? source.ToDictionary<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, IdentityDescriptor>) (x => x.Key), (Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x.Value)) : (Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) null;
      Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> identities = dictionary;
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.Hits");
      performanceCounter.IncrementBy((long) cacheHits);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.HitsPerSecond");
      performanceCounter.IncrementBy((long) cacheHits);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.Misses");
      performanceCounter.IncrementBy((long) cacheMisses);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesByDescriptor.MissesPerSecond");
      performanceCounter.IncrementBy((long) cacheMisses);
      return identities;
    }

    public void SetIdentities(
      IVssRequestContext context,
      Guid scopeId,
      IEnumerable<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>> values)
    {
      this.ValidateRequestContext(context);
      if (!ImsCacheService.IsFeatureEnabled(context) || context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.IdentitiesByDescriptor.Disable") || !this.IsOperationSupported(ImsOperation.IdentitiesByDescriptor))
        return;
      values = (IEnumerable<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>) values.Where<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (x => x.Value != null && x.Value.MasterId != IdentityConstants.LinkedId)).ToList<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>();
      values = this.FilterOutFrameworkIdentities<IdentityDescriptor>(context, values);
      values = this.FilterOutInvalidCacheData<IdentityDescriptor>(context, values);
      if (values.IsNullOrEmpty<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>())
        return;
      DateTimeOffset now = DateTimeOffset.UtcNow;
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
      Dictionary<IdentityDescriptor, IdentityId> cachedIdentityIds = this.GetIdentityIds(context, values.Select<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, IdentityDescriptor>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, IdentityDescriptor>) (x => x.Key)), (Func<ImsCacheIdentityId, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.FreshEntryTimeSpan, now)));
      IEnumerable<KeyValuePair<IdentityDescriptor, IdentityId>> values1 = values.Where<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (x =>
      {
        if (x.Value == null || !IdentityDescriptorComparer.Instance.Equals(x.Value.Descriptor, x.Key))
          return false;
        return !cachedIdentityIds.ContainsKey(x.Key) || !ImsCacheUtils.ExtractIdentityId((IReadOnlyVssIdentity) x.Value).Equals(cachedIdentityIds[x.Key]);
      })).Select<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, KeyValuePair<IdentityDescriptor, IdentityId>>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, KeyValuePair<IdentityDescriptor, IdentityId>>) (x => new KeyValuePair<IdentityDescriptor, IdentityId>(x.Key, ImsCacheUtils.ExtractIdentityId((IReadOnlyVssIdentity) x.Value))));
      this.SetIdentityIds(context, values1);
      List<ImsCacheScopedIdKey> list = values.Where<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (x => cachedIdentityIds.TryGetOrDefault<IdentityDescriptor, IdentityId>(x.Key) != null)).Select<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, ImsCacheScopedIdKey>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, ImsCacheScopedIdKey>) (x => new ImsCacheScopedIdKey(new ScopedId(scopeId, cachedIdentityIds[x.Key].Id)))).ToList<ImsCacheScopedIdKey>();
      IVssRequestContext masterDomainContext = ImsCacheService.GetMasterDomainContext(context);
      IImsRemoteCache service1 = masterDomainContext.GetService<IImsRemoteCache>();
      Dictionary<ImsCacheScopedIdKey, ImsCacheScopeMembership> dictionary1 = service1.GetObjects<ImsCacheScopedIdKey, ImsCacheScopeMembership>(masterDomainContext, (ICollection<ImsCacheScopedIdKey>) list).Where<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>>((Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, bool>) (x => !ImsCacheService.IsCacheMiss((ImsCacheObject) x.Value, settings.FreshEntryTimeSpan, now))).ToDictionary<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheScopedIdKey, ImsCacheScopeMembership>((Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheScopedIdKey>) (x => x.Key), (Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheScopeMembership>) (x => x.Value));
      List<ImsCacheScopeMembership> membershipsToSet = ImsCacheService.ComputeScopeMembershipsToSet(scopeId, values, dictionary1);
      service1.AddObjects<ImsCacheScopeMembership>(masterDomainContext, (IEnumerable<ImsCacheScopeMembership>) membershipsToSet);
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      IImsRemoteCache service2 = context1.GetService<IImsRemoteCache>();
      Dictionary<ImsCacheIdKey, ImsCacheIdentity> dictionary2 = service2.GetObjects<ImsCacheIdKey, ImsCacheIdentity>(context1, (ICollection<ImsCacheIdKey>) values.Where<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (x => x.Value != null)).Select<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, ImsCacheIdKey>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, ImsCacheIdKey>) (x => new ImsCacheIdKey(x.Value.Id))).ToList<ImsCacheIdKey>()).Where<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>>((Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, bool>) (x => !ImsCacheService.IsCacheMiss((ImsCacheObject) x.Value, settings.FreshEntryTimeSpan, now))).ToDictionary<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, ImsCacheIdKey, ImsCacheIdentity>((Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, ImsCacheIdKey>) (x => x.Key), (Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, ImsCacheIdentity>) (x => x.Value));
      List<ImsCacheIdentity> identitiesToSet = ImsCacheService.ComputeIdentitiesToSet(values.Select<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x.Value)), dictionary2);
      service2.AddObjects<ImsCacheIdentity>(context1, (IEnumerable<ImsCacheIdentity>) identitiesToSet);
      int count = identitiesToSet.Count;
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDescriptor.Requests");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDescriptor.Objects");
      performanceCounter.IncrementBy((long) count);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDescriptor.RequestsPerSecond");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesByDescriptor.ObjectsPerSecond");
      performanceCounter.IncrementBy((long) count);
    }

    public Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> GetIdentities(
      IVssRequestContext context,
      Guid scopeId,
      ICollection<Guid> ids)
    {
      this.ValidateRequestContext(context);
      if (!ImsCacheService.IsFeatureEnabled(context) || context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.IdentitiesById.Disable") || !this.IsOperationSupported(ImsOperation.IdentitiesById))
        return (Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) null;
      int count = ids.Count;
      if (count == 0)
        return ids.ToDictionary<Guid, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Guid, Guid>) (x => x), (Func<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) (x => (Microsoft.VisualStudio.Services.Identity.Identity) null));
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.Requests");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.Objects");
      performanceCounter.IncrementBy((long) count);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.RequestsPerSecond");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.ObjectsPerSecond");
      performanceCounter.IncrementBy((long) count);
      ids = (ICollection<Guid>) ids.Distinct<Guid>().ToList<Guid>();
      DateTimeOffset now = DateTimeOffset.UtcNow;
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
      List<ImsCacheScopedIdKey> list1 = ids.Select<Guid, ImsCacheScopedIdKey>((Func<Guid, ImsCacheScopedIdKey>) (x => new ImsCacheScopedIdKey(new ScopedId(scopeId, x)))).ToList<ImsCacheScopedIdKey>();
      IVssRequestContext masterDomainContext = ImsCacheService.GetMasterDomainContext(context);
      IDictionary<ImsCacheScopedIdKey, ImsCacheScopeMembership> scopeMembershipCachedValues = masterDomainContext.GetService<IImsCacheOrchestrator>().GetObjects<ImsCacheScopedIdKey, ImsCacheScopeMembership>(masterDomainContext, (ICollection<ImsCacheScopedIdKey>) list1, (Func<ImsCacheScopeMembership, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.ScopeMembershipTimeToLive, now)));
      List<ImsCacheIdKey> list2 = scopeMembershipCachedValues.Where<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>>((Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, bool>) (x => !ImsCacheService.IsCacheMiss((ImsCacheObject) x.Value, settings.ScopeMembershipTimeToLive, now))).Select<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheIdKey>((Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheIdKey>) (x => new ImsCacheIdKey(x.Key.Id.Id))).ToList<ImsCacheIdKey>();
      if (list2.IsNullOrEmpty<ImsCacheIdKey>())
        return ids.ToDictionary<Guid, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Guid, Guid>) (x => x), (Func<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) (x => (Microsoft.VisualStudio.Services.Identity.Identity) null));
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      Dictionary<ImsCacheIdKey, ImsCacheIdentity> identitiesWithValidScopeMembershipMap = context1.GetService<IImsCacheOrchestrator>().GetObjects<ImsCacheIdKey, ImsCacheIdentity>(context1, (ICollection<ImsCacheIdKey>) list2, (Func<ImsCacheIdentity, bool>) (obj => ImsCacheService.IsCacheMiss((ImsCacheObject) obj, settings.IdentityTimeToLive, now))).ToDictionary<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, ImsCacheIdKey, ImsCacheIdentity>((Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, ImsCacheIdKey>) (x => x.Key), (Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, ImsCacheIdentity>) (x => x.Value));
      if (identitiesWithValidScopeMembershipMap.IsNullOrEmpty<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>>())
        return ids.ToDictionary<Guid, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Guid, Guid>) (x => x), (Func<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) (x => (Microsoft.VisualStudio.Services.Identity.Identity) null));
      int cacheHits = 0;
      int cacheMisses = 0;
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary1 = ids.ToDictionary<Guid, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Guid, Guid>) (x => x), (Func<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) (x =>
      {
        ImsCacheIdentity imsCacheIdentity;
        ImsCacheScopeMembership cacheScopeMembership;
        if (identitiesWithValidScopeMembershipMap.TryGetValue(new ImsCacheIdKey(x), out imsCacheIdentity) && !ImsCacheService.IsCacheMiss((ImsCacheObject) imsCacheIdentity, settings.IdentityTimeToLive, now) && scopeMembershipCachedValues.TryGetValue(new ImsCacheScopedIdKey(new ScopedId(scopeId, x)), out cacheScopeMembership) && !ImsCacheService.IsCacheMiss((ImsCacheObject) cacheScopeMembership, settings.ScopeMembershipTimeToLive, now))
        {
          ++cacheHits;
          imsCacheIdentity.Value.IsActive = cacheScopeMembership.Value;
          return imsCacheIdentity.Value;
        }
        ++cacheMisses;
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }));
      IEnumerable<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>> source = this.FilterOutFrameworkIdentities<Guid>(context, (IEnumerable<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>) dictionary1);
      if (!(source is Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> dictionary2))
        dictionary2 = source != null ? source.ToDictionary<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x.Value)) : (Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) null;
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identities = dictionary2;
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.Hits");
      performanceCounter.IncrementBy((long) cacheHits);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.HitsPerSecond");
      performanceCounter.IncrementBy((long) cacheHits);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.Misses");
      performanceCounter.IncrementBy((long) cacheMisses);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.GetIdentitiesById.MissesPerSecond");
      performanceCounter.IncrementBy((long) cacheMisses);
      return identities;
    }

    public void SetIdentities(
      IVssRequestContext context,
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>> values)
    {
      this.ValidateRequestContext(context);
      if (!ImsCacheService.IsFeatureEnabled(context) || context.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.IdentitiesById.Disable") || !this.IsOperationSupported(ImsOperation.IdentitiesById))
        return;
      values = (IEnumerable<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>) values.Where<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (x => x.Value != null && x.Value.MasterId != IdentityConstants.LinkedId)).ToList<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>();
      values = this.FilterOutFrameworkIdentities<Guid>(context, values);
      values = this.FilterOutInvalidCacheData<Guid>(context, values);
      if (values.IsNullOrEmpty<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>())
        return;
      DateTimeOffset now = DateTimeOffset.UtcNow;
      ImsCacheSettings.ImsCacheServiceRegistrySettings settings = this.GetSettings(context);
      List<ImsCacheScopedIdKey> list = values.Select<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, ImsCacheScopedIdKey>((Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, ImsCacheScopedIdKey>) (x => new ImsCacheScopedIdKey(new ScopedId(scopeId, x.Key)))).ToList<ImsCacheScopedIdKey>();
      IVssRequestContext masterDomainContext = ImsCacheService.GetMasterDomainContext(context);
      IImsRemoteCache service1 = masterDomainContext.GetService<IImsRemoteCache>();
      Dictionary<ImsCacheScopedIdKey, ImsCacheScopeMembership> dictionary1 = service1.GetObjects<ImsCacheScopedIdKey, ImsCacheScopeMembership>(masterDomainContext, (ICollection<ImsCacheScopedIdKey>) list).Where<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>>((Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, bool>) (x => !ImsCacheService.IsCacheMiss((ImsCacheObject) x.Value, settings.FreshEntryTimeSpan, now))).ToDictionary<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheScopedIdKey, ImsCacheScopeMembership>((Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheScopedIdKey>) (x => x.Key), (Func<KeyValuePair<ImsCacheScopedIdKey, ImsCacheScopeMembership>, ImsCacheScopeMembership>) (x => x.Value));
      List<ImsCacheScopeMembership> membershipsToSet = ImsCacheService.ComputeScopeMembershipsToSet(scopeId, values, dictionary1);
      service1.AddObjects<ImsCacheScopeMembership>(masterDomainContext, (IEnumerable<ImsCacheScopeMembership>) membershipsToSet);
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      IImsRemoteCache service2 = context1.GetService<IImsRemoteCache>();
      Dictionary<ImsCacheIdKey, ImsCacheIdentity> dictionary2 = service2.GetObjects<ImsCacheIdKey, ImsCacheIdentity>(context1, (ICollection<ImsCacheIdKey>) values.Where<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (x => x.Value != null)).Select<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, ImsCacheIdKey>((Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, ImsCacheIdKey>) (x => new ImsCacheIdKey(x.Value.Id))).ToList<ImsCacheIdKey>()).Where<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>>((Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, bool>) (x => !ImsCacheService.IsCacheMiss((ImsCacheObject) x.Value, settings.FreshEntryTimeSpan, now))).ToDictionary<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, ImsCacheIdKey, ImsCacheIdentity>((Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, ImsCacheIdKey>) (x => x.Key), (Func<KeyValuePair<ImsCacheIdKey, ImsCacheIdentity>, ImsCacheIdentity>) (x => x.Value));
      List<ImsCacheIdentity> identitiesToSet = ImsCacheService.ComputeIdentitiesToSet(values.Select<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x.Value)), dictionary2);
      service2.AddObjects<ImsCacheIdentity>(context1, (IEnumerable<ImsCacheIdentity>) identitiesToSet);
      int count = identitiesToSet.Count;
      VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesById.Requests");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesById.Objects");
      performanceCounter.IncrementBy((long) count);
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesById.RequestsPerSecond");
      performanceCounter.Increment();
      performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SetIdentitiesById.ObjectsPerSecond");
      performanceCounter.IncrementBy((long) count);
    }

    private Dictionary<IdentityDescriptor, IdentityId> GetIdentityIds(
      IVssRequestContext context,
      IEnumerable<IdentityDescriptor> ids,
      Func<ImsCacheIdentityId, bool> isCacheMiss)
    {
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      List<ImsCacheDescriptorKey> list = ids.Select<IdentityDescriptor, ImsCacheDescriptorKey>((Func<IdentityDescriptor, ImsCacheDescriptorKey>) (id => new ImsCacheDescriptorKey(id))).ToList<ImsCacheDescriptorKey>();
      IDictionary<ImsCacheDescriptorKey, ImsCacheIdentityId> cacheValues = context1.GetService<IImsCacheOrchestrator>().GetObjects<ImsCacheDescriptorKey, ImsCacheIdentityId>(context1, (ICollection<ImsCacheDescriptorKey>) list, isCacheMiss);
      ImsCacheIdentityId imsCacheIdentityId;
      return list.ToDedupedDictionary<ImsCacheDescriptorKey, IdentityDescriptor, IdentityId>((Func<ImsCacheDescriptorKey, IdentityDescriptor>) (key => key.Id), (Func<ImsCacheDescriptorKey, IdentityId>) (key => !cacheValues.TryGetValue(key, out imsCacheIdentityId) || isCacheMiss(imsCacheIdentityId) ? (IdentityId) null : imsCacheIdentityId.Value), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
    }

    private void SetIdentityIds(
      IVssRequestContext context,
      IEnumerable<KeyValuePair<IdentityDescriptor, IdentityId>> values)
    {
      if (values.IsNullOrEmpty<KeyValuePair<IdentityDescriptor, IdentityId>>())
        return;
      DateTimeOffset now = DateTimeOffset.UtcNow;
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      context1.GetService<IImsRemoteCache>().AddObjects<ImsCacheIdentityId>(context1, values.Select<KeyValuePair<IdentityDescriptor, IdentityId>, ImsCacheIdentityId>((Func<KeyValuePair<IdentityDescriptor, IdentityId>, ImsCacheIdentityId>) (x => new ImsCacheIdentityId(new ImsCacheDescriptorKey(x.Key), x.Value, now))));
    }

    private static List<ImsCacheScopeMembership> ComputeScopeMembershipsToSet(
      Guid scopeId,
      IEnumerable<KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>> values,
      Dictionary<ImsCacheScopedIdKey, ImsCacheScopeMembership> cachedScopeMemberships)
    {
      DateTimeOffset utcNow = DateTimeOffset.UtcNow;
      List<ImsCacheScopeMembership> membershipsToSet = new List<ImsCacheScopeMembership>();
      foreach (KeyValuePair<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> keyValuePair in values)
      {
        if (keyValuePair.Value != null)
        {
          Guid id = keyValuePair.Value.Id;
          ImsCacheScopedIdKey key = new ImsCacheScopedIdKey(new ScopedId(scopeId, id));
          bool isActive = keyValuePair.Value.IsActive;
          if (!cachedScopeMemberships.ContainsKey(key) || isActive != cachedScopeMemberships[key].Value)
          {
            ImsCacheScopeMembership cacheScopeMembership = new ImsCacheScopeMembership(key, isActive, utcNow);
            membershipsToSet.Add(cacheScopeMembership);
          }
        }
      }
      return membershipsToSet;
    }

    private static List<ImsCacheScopeMembership> ComputeScopeMembershipsToSet(
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity>> values,
      Dictionary<ImsCacheScopedIdKey, ImsCacheScopeMembership> cachedScopeMemberships)
    {
      DateTimeOffset utcNow = DateTimeOffset.UtcNow;
      List<ImsCacheScopeMembership> membershipsToSet = new List<ImsCacheScopeMembership>();
      foreach (KeyValuePair<Guid, Microsoft.VisualStudio.Services.Identity.Identity> keyValuePair in values)
      {
        if (keyValuePair.Value != null)
        {
          ImsCacheScopedIdKey key = new ImsCacheScopedIdKey(new ScopedId(scopeId, keyValuePair.Value.Id));
          bool isActive = keyValuePair.Value.IsActive;
          if (!cachedScopeMemberships.ContainsKey(key) || isActive != cachedScopeMemberships[key].Value)
          {
            ImsCacheScopeMembership cacheScopeMembership = new ImsCacheScopeMembership(key, isActive, utcNow);
            membershipsToSet.Add(cacheScopeMembership);
          }
        }
      }
      return membershipsToSet;
    }

    private static List<ImsCacheIdentity> ComputeIdentitiesToSet(
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> values,
      Dictionary<ImsCacheIdKey, ImsCacheIdentity> cachedIdentityIds)
    {
      DateTimeOffset utcNow = DateTimeOffset.UtcNow;
      List<ImsCacheIdentity> identitiesToSet = new List<ImsCacheIdentity>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in values)
      {
        if (identity != null)
        {
          ImsCacheIdKey key = new ImsCacheIdKey(identity.Id);
          ImsCacheIdentity imsCacheIdentity = new ImsCacheIdentity(new ImsCacheIdKey(identity.Id), identity, utcNow);
          if (!cachedIdentityIds.ContainsKey(key) || !identity.Equals((object) cachedIdentityIds[key].Value))
            identitiesToSet.Add(imsCacheIdentity);
        }
      }
      return identitiesToSet;
    }

    private static bool IsCacheMiss(ImsCacheObject obj, TimeSpan timeToLive, DateTimeOffset now) => ImsCacheObject.IsNullOrExpired(obj, timeToLive, now);

    private static ISet<IdentityId> ExtractMembers(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        return (ISet<IdentityId>) null;
      return identity.MemberIds.IsNullOrEmpty<Guid>() ? (ISet<IdentityId>) new HashSet<IdentityId>() : (ISet<IdentityId>) new HashSet<IdentityId>(requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) identity.MemberIds.ToArray<Guid>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityId>) (x => new IdentityId(x.Id, ImsCacheUtils.ExtractIdentityType((IReadOnlyVssIdentity) x), x.Descriptor))));
    }

    private static ISet<Guid> ExtractMemberOfIds(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        return (ISet<Guid>) null;
      HashSet<Guid> memberOfIds = new HashSet<Guid>();
      if (identity.MemberOfIds != null)
        memberOfIds.UnionWith((IEnumerable<Guid>) identity.MemberOfIds);
      return (ISet<Guid>) memberOfIds;
    }

    private static IVssRequestContext GetMasterDomainContext(IVssRequestContext context) => context.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && context.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2.RemoteCache.ScopeMembershipAtOrgLevel") ? context.To(TeamFoundationHostType.Application) : context;

    private static bool IsVsoGroup(IdentityId identityId) => identityId != null && (identityId.Type & (Microsoft.VisualStudio.Services.Identity.Cache.IdentityType.Group | Microsoft.VisualStudio.Services.Identity.Cache.IdentityType.Internal)) != 0;

    private IEnumerable<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>> FilterOutFrameworkIdentities<T>(
      IVssRequestContext context,
      IEnumerable<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>> values)
    {
      return context.ServiceHost.Is(TeamFoundationHostType.Deployment) || values == null || !values.Any<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>() ? values : (IEnumerable<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>) values.Where<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (x => !this.IsFrameworkIdentity(context, x.Value))).ToDedupedDictionary<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, T, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, T>) (x => x.Key), (Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x.Value));
    }

    internal virtual ICollection<Microsoft.VisualStudio.Services.Identity.Identity> FilterOutInvalidCacheData(
      IVssRequestContext context,
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> values)
    {
      if (values == null || !values.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
        return values;
      if (!context.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return values;
      try
      {
        if (context.IsFeatureEnabled("VisualStudio.Services.IdentityCache.FilterOutInactiveIdentities"))
          return (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) values.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null || x.IsActive)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        return context.IsOrganizationAadBacked() ? (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) values.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null || !x.IsMsaIdentity() || x.IsActive)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>() : (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) values.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null || !x.IsExternalUser || x.IsActive)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      catch (OrganizationNotFoundException ex)
      {
        return values;
      }
    }

    internal virtual IEnumerable<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>> FilterOutInvalidCacheData<T>(
      IVssRequestContext context,
      IEnumerable<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>> values)
    {
      if (values == null || !values.Any<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>())
        return values;
      return context.IsOrganizationAadBacked() ? (IEnumerable<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>) values.Where<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (x => x.Value == null || !x.Value.IsMsaIdentity() || x.Value.IsActive)).ToDedupedDictionary<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, T, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, T>) (x => x.Key), (Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x.Value)) : (IEnumerable<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>) values.Where<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (x => x.Value == null || !x.Value.IsExternalUser || x.Value.IsActive)).ToDedupedDictionary<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, T, Microsoft.VisualStudio.Services.Identity.Identity>((Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, T>) (x => x.Key), (Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, Microsoft.VisualStudio.Services.Identity.Identity>) (x => x.Value));
    }

    private bool IsFrameworkIdentity(IVssRequestContext context, Microsoft.VisualStudio.Services.Identity.Identity identity) => identity != null && IdentityHelper.IsShardedFrameworkIdentity(context, identity.Descriptor);

    protected virtual IDictionary<T, ImsCacheIdentity> FilterOutFrameworkIdentities<T>(
      IVssRequestContext context,
      IDictionary<T, ImsCacheIdentity> values)
    {
      return context.ServiceHost.Is(TeamFoundationHostType.Deployment) || values == null || !values.Any<KeyValuePair<T, ImsCacheIdentity>>() ? values : (IDictionary<T, ImsCacheIdentity>) values.Where<KeyValuePair<T, ImsCacheIdentity>>((Func<KeyValuePair<T, ImsCacheIdentity>, bool>) (x => !this.IsFrameworkIdentity(context, x.Value?.Value))).ToDedupedDictionary<KeyValuePair<T, ImsCacheIdentity>, T, ImsCacheIdentity>((Func<KeyValuePair<T, ImsCacheIdentity>, T>) (x => x.Key), (Func<KeyValuePair<T, ImsCacheIdentity>, ImsCacheIdentity>) (x => x.Value));
    }

    protected virtual ICollection<Microsoft.VisualStudio.Services.Identity.Identity> FilterOutFrameworkIdentities(
      IVssRequestContext context,
      ICollection<Microsoft.VisualStudio.Services.Identity.Identity> values)
    {
      return context.ServiceHost.Is(TeamFoundationHostType.Deployment) || values == null || !values.Any<Microsoft.VisualStudio.Services.Identity.Identity>() ? values : (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) values.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => !this.IsFrameworkIdentity(context, x))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private static bool IsFeatureEnabled(IVssRequestContext requestContext) => requestContext.IsImsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Cache2");

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.CacheServiceRequestContextHostMessage((object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private ImsCacheSettings.ImsCacheServiceRegistrySettings GetSettings(IVssRequestContext context) => context.To(TeamFoundationHostType.Deployment).GetService<ImsCacheSettings>().ImsCacheServiceSettings;

    private bool IsOperationSupported(ImsOperation operation, bool checkLocal = true, bool checkRemote = true)
    {
      if (!checkLocal && !checkRemote)
        throw new ArgumentException("Atleast parameter checkLocal or checkRemote must be true.");
      return checkLocal && this.m_supportedLocalCacheOperations.HasFlag((Enum) operation) || checkRemote && this.m_supportedRemoteCacheOperations.HasFlag((Enum) operation);
    }

    private static bool EmptyCacheItemRandomMiss() => Interlocked.Increment(ref ImsCacheService.s_tenPercentCacheMissRecheckCounter) % 10 == 0;

    private static class Functions
    {
      internal static class SearchByDisplayName
      {
        internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, string> CreateKey() => (Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => x.DisplayName);

        internal static Func<ImsCacheStringKey, IdentityId, ImsCacheIdentityIdByDisplayName> CreateLocalCacheEntry() => (Func<ImsCacheStringKey, IdentityId, ImsCacheIdentityIdByDisplayName>) ((x, y) => new ImsCacheIdentityIdByDisplayName(x, y, DateTimeOffset.UtcNow));

        internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByDisplayName> CreateLocalCacheEntryFromIdentity() => (Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByDisplayName>) (x =>
        {
          if (x == null)
            return (ImsCacheIdentityIdByDisplayName) null;
          string id = ImsCacheService.Functions.SearchByDisplayName.CreateKey()(x);
          return string.IsNullOrEmpty(id) ? (ImsCacheIdentityIdByDisplayName) null : ImsCacheService.Functions.SearchByDisplayName.CreateLocalCacheEntry()(new ImsCacheStringKey(id), ImsCacheUtils.ExtractIdentityId((IReadOnlyVssIdentity) x));
        });
      }

      internal static class SearchByAppId
      {
        internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, string> CreateKey() => (Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x =>
        {
          Guid property = x.GetProperty<Guid>("ApplicationId", Guid.Empty);
          return Guid.Empty == property ? (string) null : property.ToString();
        });

        internal static Func<ImsCacheStringKey, IdentityId, ImsCacheIdentityIdByAppId> CreateLocalCacheEntry() => (Func<ImsCacheStringKey, IdentityId, ImsCacheIdentityIdByAppId>) ((x, y) => new ImsCacheIdentityIdByAppId(x, y, DateTimeOffset.UtcNow));

        internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByAppId> CreateLocalCacheEntryFromIdentity() => (Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByAppId>) (x =>
        {
          if (x == null)
            return (ImsCacheIdentityIdByAppId) null;
          string id = ImsCacheService.Functions.SearchByAppId.CreateKey()(x);
          return string.IsNullOrEmpty(id) ? (ImsCacheIdentityIdByAppId) null : ImsCacheService.Functions.SearchByAppId.CreateLocalCacheEntry()(new ImsCacheStringKey(id), ImsCacheUtils.ExtractIdentityId((IReadOnlyVssIdentity) x));
        });
      }

      internal static class SearchByEmailPrefix
      {
        internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, string> CreateKey() => (Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => x.Properties.GetValue<string>("Mail", (string) null));

        internal static Func<ImsCacheStringKey, IdentityId, ImsCacheIdentityIdByEmail> CreateLocalCacheEntry() => (Func<ImsCacheStringKey, IdentityId, ImsCacheIdentityIdByEmail>) ((x, y) => new ImsCacheIdentityIdByEmail(x, y, DateTimeOffset.UtcNow));

        internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByEmail> CreateLocalCacheEntryFromIdentity() => (Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByEmail>) (x =>
        {
          if (x == null)
            return (ImsCacheIdentityIdByEmail) null;
          string id = ImsCacheService.Functions.SearchByEmailPrefix.CreateKey()(x);
          return string.IsNullOrEmpty(id) ? (ImsCacheIdentityIdByEmail) null : ImsCacheService.Functions.SearchByEmailPrefix.CreateLocalCacheEntry()(new ImsCacheStringKey(id), ImsCacheUtils.ExtractIdentityId((IReadOnlyVssIdentity) x));
        });
      }

      internal static class SearchByAccountName
      {
        internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, string> CreateKey() => (Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => x.Properties.GetValue<string>("Account", (string) null));

        internal static Func<ImsCacheStringKey, IdentityId, ImsCacheIdentityIdByAccountName> CreateLocalCacheEntry() => (Func<ImsCacheStringKey, IdentityId, ImsCacheIdentityIdByAccountName>) ((x, y) => new ImsCacheIdentityIdByAccountName(x, y, DateTimeOffset.UtcNow));

        internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByAccountName> CreateLocalCacheEntryFromIdentity() => (Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByAccountName>) (x =>
        {
          if (x == null)
            return (ImsCacheIdentityIdByAccountName) null;
          string id = ImsCacheService.Functions.SearchByAccountName.CreateKey()(x);
          return string.IsNullOrEmpty(id) ? (ImsCacheIdentityIdByAccountName) null : ImsCacheService.Functions.SearchByAccountName.CreateLocalCacheEntry()(new ImsCacheStringKey(id), ImsCacheUtils.ExtractIdentityId((IReadOnlyVssIdentity) x));
        });
      }

      internal static class SearchByDomainAccountName
      {
        internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, string> CreateKey() => (Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x =>
        {
          string str1 = x.Properties.GetValue<string>("Domain", (string) null);
          if (string.IsNullOrWhiteSpace(str1))
            return (string) null;
          string str2 = x.Properties.GetValue<string>("Account", (string) null);
          return string.IsNullOrWhiteSpace(str2) ? (string) null : str1 + (object) '\\' + str2;
        });

        internal static Func<ImsCacheStringKey, IdentityId, ImsCacheIdentityIdByDomainAccountName> CreateLocalCacheEntry() => (Func<ImsCacheStringKey, IdentityId, ImsCacheIdentityIdByDomainAccountName>) ((imsCacheStringKey, identityId) => !string.Equals(identityId.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase) ? (ImsCacheIdentityIdByDomainAccountName) null : new ImsCacheIdentityIdByDomainAccountName(imsCacheStringKey, identityId, DateTimeOffset.UtcNow));

        internal static Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByDomainAccountName> CreateLocalCacheEntryFromIdentity() => (Func<Microsoft.VisualStudio.Services.Identity.Identity, ImsCacheIdentityIdByDomainAccountName>) (x =>
        {
          if (x == null)
            return (ImsCacheIdentityIdByDomainAccountName) null;
          string id = ImsCacheService.Functions.SearchByDomainAccountName.CreateKey()(x);
          return string.IsNullOrEmpty(id) ? (ImsCacheIdentityIdByDomainAccountName) null : ImsCacheService.Functions.SearchByDomainAccountName.CreateLocalCacheEntry()(new ImsCacheStringKey(id), ImsCacheUtils.ExtractIdentityId((IReadOnlyVssIdentity) x));
        });
      }
    }

    private class GetDescendantsOperationState
    {
      internal Guid Id { get; set; }

      internal ImsCacheIdKey CacheKey { get; set; }

      internal ISet<IdentityId> Results { get; set; }

      internal bool ResultsComputedRecursively { get; set; }

      internal HashSet<IdentityId> RecursivelyComputedResults { get; set; }

      internal HashSet<Guid> GroupsToExpand { get; set; }

      internal HashSet<Guid> GroupsExpanded { get; set; }

      internal int LevelsExpanded { get; set; }

      internal List<Guid> LeftOverGroupsToExpand { get; set; }

      internal bool Completed { get; set; }

      internal bool Failed { get; set; }
    }

    private class ImsCacheSearchOperationPerformanceCounters
    {
      internal readonly VssPerformanceCounter CreateIndexRequests;
      internal readonly VssPerformanceCounter CreateIndexRequestsPerSecond;
      internal readonly VssPerformanceCounter SearchRequests;
      internal readonly VssPerformanceCounter SearchRequestsPerSecond;
      internal readonly VssPerformanceCounter SearchHits;
      internal readonly VssPerformanceCounter SearchHitsPerSecond;
      internal readonly VssPerformanceCounter SearchMisses;
      internal readonly VssPerformanceCounter SearchMissesPerSecond;
      internal static readonly ImsCacheService.ImsCacheSearchOperationPerformanceCounters SearchByDisplayName = new ImsCacheService.ImsCacheSearchOperationPerformanceCounters(VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByDisplayName.Requests"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByDisplayName.RequestsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.Requests"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.RequestsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.Hits"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.HitsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.Misses"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByDisplayName.MissesPerSecond"));
      internal static readonly ImsCacheService.ImsCacheSearchOperationPerformanceCounters SearchByAppId = new ImsCacheService.ImsCacheSearchOperationPerformanceCounters(VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAppId.Requests"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAppId.RequestsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.Requests"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.RequestsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.Hits"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.HitsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.Misses"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAppId.MissesPerSecond"));
      internal static readonly ImsCacheService.ImsCacheSearchOperationPerformanceCounters SearchByEmail = new ImsCacheService.ImsCacheSearchOperationPerformanceCounters(VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByEmail.Requests"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByEmail.RequestsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.Requests"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.RequestsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.Hits"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.HitsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.Misses"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByEmail.MissesPerSecond"));
      internal static readonly ImsCacheService.ImsCacheSearchOperationPerformanceCounters SearchByAccountName = new ImsCacheService.ImsCacheSearchOperationPerformanceCounters(VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAlias.Requests"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAlias.RequestsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Requests"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.RequestsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Hits"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.HitsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Misses"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.MissesPerSecond"));
      internal static readonly ImsCacheService.ImsCacheSearchOperationPerformanceCounters SearchByDomainAccountName = new ImsCacheService.ImsCacheSearchOperationPerformanceCounters(VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAlias.Requests"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.CreateSearchIndexByAlias.RequestsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Requests"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.RequestsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Hits"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.HitsPerSecond"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.Misses"), VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.ImsCacheService.SearchIdentityIdsByAlias.MissesPerSecond"));

      private ImsCacheSearchOperationPerformanceCounters(
        VssPerformanceCounter createIndexRequests,
        VssPerformanceCounter createIndexRequestsPerSecond,
        VssPerformanceCounter searchRequests,
        VssPerformanceCounter searchRequestsPerSecond,
        VssPerformanceCounter searchHits,
        VssPerformanceCounter searchHitsPerSecond,
        VssPerformanceCounter searchMisses,
        VssPerformanceCounter searchMissesPerSecond)
      {
        this.CreateIndexRequests = createIndexRequests;
        this.CreateIndexRequestsPerSecond = createIndexRequestsPerSecond;
        this.SearchRequests = searchRequests;
        this.SearchRequestsPerSecond = searchRequestsPerSecond;
        this.SearchHits = searchHits;
        this.SearchHitsPerSecond = searchHitsPerSecond;
        this.SearchMisses = searchMisses;
        this.SearchMissesPerSecond = searchMissesPerSecond;
      }
    }
  }
}
