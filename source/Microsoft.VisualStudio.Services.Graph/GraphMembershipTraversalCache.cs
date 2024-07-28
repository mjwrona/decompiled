// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMembershipTraversalCache
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal class GraphMembershipTraversalCache : 
    VssMemoryCacheService<Guid, IDictionary<Guid, SubjectDescriptor>>,
    IGraphMembershipTraversalCache,
    IVssFrameworkService
  {
    private readonly List<IdentityMessageBusData> m_scopeVisibilityUpdates = new List<IdentityMessageBusData>();
    private GraphMembershipTraversalCache.TaskState m_processChangesTaskState;
    private static ILockName s_processChangesTaskLockName;
    private static ILockName s_updateCacheLockName;
    internal static readonly Guid s_IMS2PlatformGroupScopeVisibilityChanged = new Guid("F2C4DCC2-8F4B-46A7-9D94-FD81D1B10050");
    private const string c_area = "Graph";
    private const string c_layer = "GraphMembershipTraversalCache";

    private GraphMembershipTraversalCache() => this.ExpiryInterval.Value = TimeSpan.FromHours(48.0);

    protected override void ServiceStart(IVssRequestContext context)
    {
      context.CheckOrganizationRequestContext();
      base.ServiceStart(context);
      context.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      GraphMembershipTraversalCache.s_processChangesTaskLockName = context.ServiceHost.CreateUniqueLockName(string.Format("{0}.{1}.{2}", (object) nameof (GraphMembershipTraversalCache), (object) "s_processChangesTaskLockName", (object) context.ServiceHost.InstanceId));
      GraphMembershipTraversalCache.s_updateCacheLockName = context.ServiceHost.CreateUniqueLockName(string.Format("{0}.{1}.{2}", (object) nameof (GraphMembershipTraversalCache), (object) "s_updateCacheLockName", (object) context.ServiceHost.InstanceId));
      context.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(context, "Default", GraphMembershipTraversalCache.s_IMS2PlatformGroupScopeVisibilityChanged, new SqlNotificationCallback(this.OnGroupScopeVisibilityChanged), false);
    }

    protected override void ServiceEnd(IVssRequestContext context)
    {
      base.ServiceEnd(context);
      context.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(context, "Default", GraphMembershipTraversalCache.s_IMS2PlatformGroupScopeVisibilityChanged, new SqlNotificationCallback(this.OnGroupScopeVisibilityChanged), false);
    }

    private void OnGroupScopeVisibilityChanged(
      IVssRequestContext context,
      Guid eventClass,
      string eventData)
    {
      if (context.IsFeatureEnabled("VisualStudio.Services.Graph.DisableTraversalCacheInvalidations"))
        return;
      context.CheckOrganizationRequestContext();
      context.TraceDataConditionally(15380211, TraceLevel.Verbose, "Graph", nameof (GraphMembershipTraversalCache), "Received the following GroupScopeVisibility changes: ", (Func<object>) (() => (object) eventData), nameof (OnGroupScopeVisibilityChanged));
      IdentityMessageBusData identityMessageBusData = TeamFoundationSerializationUtility.Deserialize<IdentityMessageBusData>(eventData);
      if (identityMessageBusData == null)
        return;
      TeamFoundationTask teamFoundationTask = (TeamFoundationTask) null;
      using (context.Lock(GraphMembershipTraversalCache.s_processChangesTaskLockName))
      {
        this.m_scopeVisibilityUpdates.Add(identityMessageBusData);
        if (this.m_processChangesTaskState == GraphMembershipTraversalCache.TaskState.NotQueued)
        {
          teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.ProcessGroupScopeVisibilityChangesTask), (object) null, 0);
          this.m_processChangesTaskState = GraphMembershipTraversalCache.TaskState.Queueing;
        }
      }
      if (teamFoundationTask == null)
        return;
      try
      {
        TeamFoundationTaskService service = context.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>();
        context.TraceDataConditionally(15380212, TraceLevel.Verbose, "Graph", nameof (GraphMembershipTraversalCache), "Queueing ProcessGroupScopeVisibilityChangesTask...", methodName: nameof (OnGroupScopeVisibilityChanged));
        Guid instanceId = context.ServiceHost.InstanceId;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(instanceId, task);
      }
      catch (Exception ex)
      {
        this.m_processChangesTaskState = GraphMembershipTraversalCache.TaskState.NotQueued;
        context.TraceDataConditionally(15380213, TraceLevel.Verbose, "Graph", nameof (GraphMembershipTraversalCache), "Queueing ProcessGroupScopeVisibilityChangesTask failed because: " + ex.Message + "...", methodName: nameof (OnGroupScopeVisibilityChanged));
        throw;
      }
      using (context.Lock(GraphMembershipTraversalCache.s_processChangesTaskLockName))
      {
        if (this.m_processChangesTaskState != GraphMembershipTraversalCache.TaskState.Queueing)
          return;
        this.m_processChangesTaskState = GraphMembershipTraversalCache.TaskState.Queued;
      }
    }

    private void ProcessGroupScopeVisibilityChangesTask(IVssRequestContext context, object taskArgs)
    {
      List<IdentityMessageBusData> identityMessageBusDataList;
      using (context.Lock(GraphMembershipTraversalCache.s_processChangesTaskLockName))
      {
        identityMessageBusDataList = new List<IdentityMessageBusData>((IEnumerable<IdentityMessageBusData>) this.m_scopeVisibilityUpdates);
        this.m_scopeVisibilityUpdates.Clear();
        this.m_processChangesTaskState = GraphMembershipTraversalCache.TaskState.NotQueued;
      }
      foreach (IdentityMessageBusData identityMessageBusData in identityMessageBusDataList)
      {
        this.ProcessGroupScopeVisibilityMajorChanges(context, identityMessageBusData?.GroupScopeVisibilityMajorChanges);
        this.ProcessGroupScopeVisibilityChanges(context, (IList<GroupScopeVisibiltyChangeInfo>) identityMessageBusData?.GroupScopeVisibiltyChanges);
      }
    }

    private void ProcessGroupScopeVisibilityMajorChanges(
      IVssRequestContext context,
      Guid[] identityUpdateGroupScopeVisibiltyMajorChanges)
    {
      if (((IEnumerable<Guid>) identityUpdateGroupScopeVisibiltyMajorChanges).IsNullOrEmpty<Guid>())
        return;
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Application).Elevate();
      IGraphMembershipTraversalCache service = vssRequestContext.GetService<IGraphMembershipTraversalCache>();
      foreach (Guid visibiltyMajorChange in identityUpdateGroupScopeVisibiltyMajorChanges)
      {
        if (service.TryGetValue(vssRequestContext, visibiltyMajorChange, out IDictionary<Guid, SubjectDescriptor> _))
          service.Remove(vssRequestContext, visibiltyMajorChange);
      }
    }

    private void ProcessGroupScopeVisibilityChanges(
      IVssRequestContext context,
      IList<GroupScopeVisibiltyChangeInfo> groupScopeVisibilityChanges)
    {
      if (groupScopeVisibilityChanges.IsNullOrEmpty<GroupScopeVisibiltyChangeInfo>())
        return;
      IDictionary<Guid, IList<GroupScopeVisibiltyChangeInfo>> changesMap = GraphMembershipTraversalCache.BuildScopeIdToChangesMap(groupScopeVisibilityChanges);
      IList<Guid> list = (IList<Guid>) groupScopeVisibilityChanges.Select<GroupScopeVisibiltyChangeInfo, Guid>((Func<GroupScopeVisibiltyChangeInfo, Guid>) (x => x.IdentityId)).ToList<Guid>();
      IDictionary<Guid, Tuple<Guid, SubjectDescriptor>> subjectDescriptorsMap = this.BuildStorageKeysToRemoteIdsAndSubjectDescriptorsMap(context, list);
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Application).Elevate();
      IGraphMembershipTraversalCache service = vssRequestContext.GetService<IGraphMembershipTraversalCache>();
      foreach (KeyValuePair<Guid, IList<GroupScopeVisibiltyChangeInfo>> keyValuePair in (IEnumerable<KeyValuePair<Guid, IList<GroupScopeVisibiltyChangeInfo>>>) changesMap)
      {
        Guid scopeId = keyValuePair.Key;
        IList<GroupScopeVisibiltyChangeInfo> source = keyValuePair.Value;
        context.TraceDataConditionally(15380200, TraceLevel.Verbose, "Graph", nameof (GraphMembershipTraversalCache), "Updating cache for scope", (Func<object>) (() => (object) new
        {
          scopeId = scopeId
        }), nameof (ProcessGroupScopeVisibilityChanges));
        IDictionary<Guid, SubjectDescriptor> scopeCacheEntries;
        if (!service.TryGetValue(vssRequestContext, scopeId, out scopeCacheEntries) || scopeCacheEntries.IsNullOrEmpty<KeyValuePair<Guid, SubjectDescriptor>>())
        {
          context.TraceDataConditionally(15380201, TraceLevel.Verbose, "Graph", nameof (GraphMembershipTraversalCache), "Did not cache for scope because there were no cache entries", (Func<object>) (() => (object) new
          {
            scopeId = scopeId
          }), nameof (ProcessGroupScopeVisibilityChanges));
        }
        else
        {
          HashSet<Guid> hashSet = source.Select<GroupScopeVisibiltyChangeInfo, Guid>((Func<GroupScopeVisibiltyChangeInfo, Guid>) (x => x.IdentityId)).ToHashSet<Guid>();
          IDictionary<Guid, bool> scopeActivenessMap = GraphMembershipTraversalCache.BuildIdentitiesToScopeActivenessMap(context, scopeId, hashSet);
          using (context.Lock(GraphMembershipTraversalCache.s_updateCacheLockName))
          {
            foreach (GroupScopeVisibiltyChangeInfo visibiltyChangeInfo in (IEnumerable<GroupScopeVisibiltyChangeInfo>) source)
            {
              Guid storageKey = visibiltyChangeInfo.IdentityId;
              Tuple<Guid, SubjectDescriptor> tuple = subjectDescriptorsMap[storageKey];
              Guid remoteObjectId = tuple.Item1;
              SubjectDescriptor subjectDescriptor = tuple.Item2;
              if (scopeActivenessMap[storageKey])
              {
                scopeCacheEntries[remoteObjectId] = subjectDescriptor;
                context.TraceDataConditionally(15380202, TraceLevel.Verbose, "Graph", nameof (GraphMembershipTraversalCache), "Activated cache entry for scope", (Func<object>) (() => (object) new
                {
                  remoteObjectId = remoteObjectId,
                  storageKey = storageKey,
                  subjectDescriptor = subjectDescriptor,
                  scopeId = scopeId
                }), nameof (ProcessGroupScopeVisibilityChanges));
              }
              else
              {
                scopeCacheEntries.Remove(remoteObjectId);
                context.TraceDataConditionally(15380203, TraceLevel.Verbose, "Graph", nameof (GraphMembershipTraversalCache), "De-activated cache entry with remote Id for scope", (Func<object>) (() => (object) new
                {
                  remoteObjectId = remoteObjectId,
                  scopeId = scopeId
                }), nameof (ProcessGroupScopeVisibilityChanges));
              }
            }
          }
          context.TraceDataConditionally(15380204, TraceLevel.Verbose, "Graph", nameof (GraphMembershipTraversalCache), "Updated cache entry for scope with the following values", (Func<object>) (() => (object) new
          {
            scopeId = scopeId,
            scopeCacheEntries = scopeCacheEntries
          }), nameof (ProcessGroupScopeVisibilityChanges));
        }
      }
    }

    private static IDictionary<Guid, IList<GroupScopeVisibiltyChangeInfo>> BuildScopeIdToChangesMap(
      IList<GroupScopeVisibiltyChangeInfo> groupScopeVisibilityChanges)
    {
      Dictionary<Guid, IList<GroupScopeVisibiltyChangeInfo>> changesMap = new Dictionary<Guid, IList<GroupScopeVisibiltyChangeInfo>>();
      foreach (GroupScopeVisibiltyChangeInfo visibilityChange in (IEnumerable<GroupScopeVisibiltyChangeInfo>) groupScopeVisibilityChanges)
      {
        if (!changesMap.ContainsKey(visibilityChange.ScopeId))
          changesMap[visibilityChange.ScopeId] = (IList<GroupScopeVisibiltyChangeInfo>) new List<GroupScopeVisibiltyChangeInfo>();
        changesMap[visibilityChange.ScopeId].Add(visibilityChange);
      }
      return (IDictionary<Guid, IList<GroupScopeVisibiltyChangeInfo>>) changesMap;
    }

    private static IDictionary<Guid, bool> BuildIdentitiesToScopeActivenessMap(
      IVssRequestContext context,
      Guid scopeId,
      HashSet<Guid> identityStorageKeys)
    {
      Dictionary<Guid, bool> dictionary = identityStorageKeys.ToDictionary<Guid, Guid, bool>((Func<Guid, Guid>) (x => x), (Func<Guid, bool>) (y => false));
      IList<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData> list;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(context))
        list = (IList<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) groupComponent.ReadScopeVisiblity((IEnumerable<Guid>) identityStorageKeys, scopeId).ToList<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>();
      foreach (ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData filteredIdentityData in list.Where<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((Func<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData, bool>) (x => x != null)))
        dictionary[filteredIdentityData.IdentityId] = filteredIdentityData.Active;
      return (IDictionary<Guid, bool>) dictionary;
    }

    private IDictionary<Guid, Tuple<Guid, SubjectDescriptor>> BuildStorageKeysToRemoteIdsAndSubjectDescriptorsMap(
      IVssRequestContext context,
      IList<Guid> storageKeys)
    {
      Dictionary<Guid, Tuple<Guid, SubjectDescriptor>> subjectDescriptorsMap = new Dictionary<Guid, Tuple<Guid, SubjectDescriptor>>();
      IVssRequestContext vssRequestContext = context.Elevate();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<Guid>) storageKeys.ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
      {
        int num;
        if (identity == null)
        {
          num = 1;
        }
        else
        {
          SubjectDescriptor subjectDescriptor = identity.SubjectDescriptor;
          num = 0;
        }
        if (num != 0)
          throw new SubjectDescriptorNotFoundException(identity != null ? identity.Id : Guid.Empty);
      }
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
      {
        Guid key = identity.StorageKey(vssRequestContext);
        SubjectDescriptor subjectDescriptor = identity.SubjectDescriptor;
        Guid aadObjectId = identity.GetAadObjectId();
        if (aadObjectId != Guid.Empty)
          subjectDescriptorsMap[key] = new Tuple<Guid, SubjectDescriptor>(aadObjectId, subjectDescriptor);
      }
      return (IDictionary<Guid, Tuple<Guid, SubjectDescriptor>>) subjectDescriptorsMap;
    }

    private enum TaskState
    {
      NotQueued,
      Queueing,
      Queued,
    }
  }
}
