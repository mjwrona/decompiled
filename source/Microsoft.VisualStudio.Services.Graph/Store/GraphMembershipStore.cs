// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Store.GraphMembershipStore
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph.Store
{
  internal class GraphMembershipStore : IGraphMembershipStore, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private SubjectDescriptor m_everyoneGroupDescriptor;
    private const string c_area = "Graph";
    private const string c_layer = "GraphMembershipStore";
    private const int c_batchSize = 10000;
    private static readonly PerformanceTracer s_tracer = new PerformanceTracer(GraphMembershipPerfCounters.StandardSet, "Graph", nameof (GraphMembershipStore));

    public void ServiceStart(IVssRequestContext systemContext)
    {
      this.m_serviceHostId = systemContext.ServiceHost.InstanceId;
      this.m_everyoneGroupDescriptor = new WellKnownIdentifierMapper(systemContext.ServiceHost.InstanceId).MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup.ToSubjectDescriptor(systemContext));
    }

    public void ServiceEnd(IVssRequestContext systemContext)
    {
    }

    public IEnumerable<SubjectDescriptor> GetChildren(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor)
    {
      return GraphMembershipStore.s_tracer.TraceTimedAction<IEnumerable<SubjectDescriptor>>(context, (TimedActionTracePoints) GraphMembershipStore.TracePoints.GetChildren, (Func<IEnumerable<SubjectDescriptor>>) (() =>
      {
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetChildren.Input, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Received input parameters", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor
        }), nameof (GetChildren));
        context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        Guid storageKey = GraphMembershipStore.GetStorageKeyByDescriptor(context, subjectDescriptor);
        if (storageKey == new Guid())
        {
          context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetChildren.StorageKeyNotFound, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returning null because we failed to find storage key", (Func<object>) (() => (object) new
          {
            subjectDescriptor = subjectDescriptor
          }), nameof (GetChildren));
          return (IEnumerable<SubjectDescriptor>) null;
        }
        List<GroupMembership> descendantMemberships;
        ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData scopeVisiblity;
        GraphMembershipStore.ReadMemberships(context, storageKey, QueryMembership.Direct, QueryMembership.None, GraphMembershipStore.TracePoints.GetChildren, out descendantMemberships, out List<GroupMembership> _, out scopeVisiblity);
        if (!this.IsTargetVisible(context, subjectDescriptor, scopeVisiblity))
        {
          context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetChildren.TargetNotVisible, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returning null because target not visible in scope", (Func<object>) (() => (object) new
          {
            subjectDescriptor = subjectDescriptor,
            storageKey = storageKey
          }), nameof (GetChildren));
          return (IEnumerable<SubjectDescriptor>) null;
        }
        IEnumerable<SubjectDescriptor> childDescriptors = (IEnumerable<SubjectDescriptor>) this.GetDescriptorsFromValidMemberships(context, storageKey, (IEnumerable<GroupMembership>) descendantMemberships, GraphMembershipStore.TracePoints.GetChildren);
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetChildren.Output, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returned result", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor,
          storageKey = storageKey,
          childDescriptors = childDescriptors
        }), nameof (GetChildren));
        return childDescriptors;
      }), actionName: nameof (GetChildren));
    }

    public IEnumerable<SubjectDescriptor> GetParents(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor)
    {
      return GraphMembershipStore.s_tracer.TraceTimedAction<IEnumerable<SubjectDescriptor>>(context, (TimedActionTracePoints) GraphMembershipStore.TracePoints.GetParents, (Func<IEnumerable<SubjectDescriptor>>) (() =>
      {
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetParents.Input, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Received input parameters", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor
        }), nameof (GetParents));
        context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        Guid storageKey = GraphMembershipStore.GetStorageKeyByDescriptor(context, subjectDescriptor);
        if (storageKey == new Guid())
        {
          context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetParents.StorageKeyNotFound, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returning null because we failed to find storage key", (Func<object>) (() => (object) new
          {
            subjectDescriptor = subjectDescriptor
          }), nameof (GetParents));
          return (IEnumerable<SubjectDescriptor>) null;
        }
        List<GroupMembership> ancestorMemberships;
        ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData scopeVisiblity;
        GraphMembershipStore.ReadMemberships(context, storageKey, QueryMembership.None, QueryMembership.Direct, GraphMembershipStore.TracePoints.GetParents, out List<GroupMembership> _, out ancestorMemberships, out scopeVisiblity);
        if (!this.IsTargetVisible(context, subjectDescriptor, scopeVisiblity))
        {
          context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetParents.TargetNotVisible, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returning null because target not visible in scope", (Func<object>) (() => (object) new
          {
            subjectDescriptor = subjectDescriptor,
            storageKey = storageKey
          }), nameof (GetParents));
          return (IEnumerable<SubjectDescriptor>) null;
        }
        IEnumerable<SubjectDescriptor> parentDescriptors = (IEnumerable<SubjectDescriptor>) this.GetDescriptorsFromValidMemberships(context, storageKey, (IEnumerable<GroupMembership>) ancestorMemberships, GraphMembershipStore.TracePoints.GetParents);
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetParents.Output, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returned result", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor,
          storageKey = storageKey,
          parentDescriptors = parentDescriptors
        }), nameof (GetParents));
        return parentDescriptors;
      }), actionName: nameof (GetParents));
    }

    public IList<SubjectDescriptor> GetDescendants(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor)
    {
      return GraphMembershipStore.s_tracer.TraceTimedAction<IList<SubjectDescriptor>>(context, (TimedActionTracePoints) GraphMembershipStore.TracePoints.GetDescendants, (Func<IList<SubjectDescriptor>>) (() =>
      {
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetDescendants.Input, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Received input parameters", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor
        }), nameof (GetDescendants));
        context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        Guid storageKey = GraphMembershipStore.GetStorageKeyByDescriptor(context, subjectDescriptor);
        if (storageKey == new Guid())
        {
          context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetDescendants.StorageKeyNotFound, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returning null because we failed to find storage key", (Func<object>) (() => (object) new
          {
            subjectDescriptor = subjectDescriptor
          }), nameof (GetDescendants));
          return (IList<SubjectDescriptor>) null;
        }
        List<GroupMembership> descendantMemberships;
        ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData scopeVisiblity;
        GraphMembershipStore.ReadMemberships(context, storageKey, QueryMembership.Expanded, QueryMembership.None, GraphMembershipStore.TracePoints.GetDescendants, out descendantMemberships, out List<GroupMembership> _, out scopeVisiblity);
        if (!this.IsTargetVisible(context, subjectDescriptor, scopeVisiblity))
        {
          context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetDescendants.TargetNotVisible, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returning null because target not visible in scope", (Func<object>) (() => (object) new
          {
            subjectDescriptor = subjectDescriptor,
            storageKey = storageKey
          }), nameof (GetDescendants));
          return (IList<SubjectDescriptor>) null;
        }
        IList<SubjectDescriptor> descendantDescriptors = this.GetDescriptorsFromValidMemberships(context, storageKey, (IEnumerable<GroupMembership>) descendantMemberships, GraphMembershipStore.TracePoints.GetDescendants);
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetDescendants.Output, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returned result", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor,
          storageKey = storageKey,
          descendantDescriptors = descendantDescriptors
        }), nameof (GetDescendants));
        return descendantDescriptors;
      }), actionName: nameof (GetDescendants));
    }

    public IDictionary<SubjectDescriptor, List<SubjectDescriptor>> GetDescendantsByDescriptors(
      IVssRequestContext context,
      IEnumerable<SubjectDescriptor> subjectDescriptors)
    {
      return GraphMembershipStore.s_tracer.TraceTimedAction<IDictionary<SubjectDescriptor, List<SubjectDescriptor>>>(context, (TimedActionTracePoints) GraphMembershipStore.TracePoints.GetDescendantsBatch, (Func<IDictionary<SubjectDescriptor, List<SubjectDescriptor>>>) (() =>
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) subjectDescriptors, nameof (subjectDescriptors));
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetDescendantsBatch.Input, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Received input parameters", (Func<object>) (() => (object) new
        {
          subjectDescriptors = subjectDescriptors
        }), nameof (GetDescendantsByDescriptors));
        context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        IDictionary<SubjectDescriptor, List<SubjectDescriptor>> descriptorToDescendantsResultMap = (IDictionary<SubjectDescriptor, List<SubjectDescriptor>>) subjectDescriptors.ToDedupedDictionary<SubjectDescriptor, SubjectDescriptor, List<SubjectDescriptor>>((Func<SubjectDescriptor, SubjectDescriptor>) (x => x), (Func<SubjectDescriptor, List<SubjectDescriptor>>) (y => (List<SubjectDescriptor>) null));
        IReadOnlyDictionary<SubjectDescriptor, Guid> descriptorToStorageKeyMap = GraphMembershipStore.GetStorageKeysByDescriptors(context, (IEnumerable<SubjectDescriptor>) descriptorToDescendantsResultMap.Keys);
        int descriptorsOfInvalidStorageKeysCount;
        IDictionary<SubjectDescriptor, Guid> keysByDescriptors = this.GetValidStorageKeysByDescriptors(context, (IEnumerable<SubjectDescriptor>) descriptorToDescendantsResultMap.Keys, out descriptorsOfInvalidStorageKeysCount);
        if (descriptorsOfInvalidStorageKeysCount == descriptorToDescendantsResultMap.Count)
          return descriptorToDescendantsResultMap;
        Dictionary<SubjectDescriptor, List<GroupMembership>> descendantMemberships;
        Dictionary<SubjectDescriptor, ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData> scopeVisibilities;
        GraphMembershipStore.ReadMembershipsBatch(context, (IEnumerable<KeyValuePair<SubjectDescriptor, Guid>>) keysByDescriptors, QueryMembership.Expanded, QueryMembership.None, GraphMembershipStore.TracePoints.GetDescendantsBatch, out descendantMemberships, out Dictionary<SubjectDescriptor, List<GroupMembership>> _, out scopeVisibilities);
        this.AddDescendantMembershipsToResultMap(context, descriptorToDescendantsResultMap, keysByDescriptors, descendantMemberships, scopeVisibilities);
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetDescendantsBatch.Output, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returned result", (Func<object>) (() => (object) new
        {
          descriptorToStorageKeyMap = descriptorToStorageKeyMap,
          descriptorToDescendantsResultMap = descriptorToDescendantsResultMap
        }), nameof (GetDescendantsByDescriptors));
        return descriptorToDescendantsResultMap;
      }), actionName: nameof (GetDescendantsByDescriptors));
    }

    public IEnumerable<SubjectDescriptor> GetAncestors(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor)
    {
      return GraphMembershipStore.s_tracer.TraceTimedAction<IEnumerable<SubjectDescriptor>>(context, (TimedActionTracePoints) GraphMembershipStore.TracePoints.GetAncestors, (Func<IEnumerable<SubjectDescriptor>>) (() =>
      {
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetAncestors.Input, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Received input parameters", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor
        }), nameof (GetAncestors));
        context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        Guid storageKey = GraphMembershipStore.GetStorageKeyByDescriptor(context, subjectDescriptor);
        if (storageKey == new Guid())
        {
          context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetAncestors.StorageKeyNotFound, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returning null because we failed to find storage key", (Func<object>) (() => (object) new
          {
            subjectDescriptor = subjectDescriptor
          }), nameof (GetAncestors));
          return (IEnumerable<SubjectDescriptor>) null;
        }
        List<GroupMembership> ancestorMemberships;
        ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData scopeVisiblity;
        GraphMembershipStore.ReadMemberships(context, storageKey, QueryMembership.None, QueryMembership.Expanded, GraphMembershipStore.TracePoints.GetAncestors, out List<GroupMembership> _, out ancestorMemberships, out scopeVisiblity);
        if (!this.IsTargetVisible(context, subjectDescriptor, scopeVisiblity))
        {
          context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetAncestors.TargetNotVisible, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returning null because target not visible in scope", (Func<object>) (() => (object) new
          {
            subjectDescriptor = subjectDescriptor,
            storageKey = storageKey
          }), nameof (GetAncestors));
          return (IEnumerable<SubjectDescriptor>) null;
        }
        IEnumerable<SubjectDescriptor> ancestorDescriptors = (IEnumerable<SubjectDescriptor>) this.GetDescriptorsFromValidMemberships(context, storageKey, (IEnumerable<GroupMembership>) ancestorMemberships, GraphMembershipStore.TracePoints.GetAncestors);
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetAncestors.Output, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returned result", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor,
          storageKey = storageKey,
          ancestorDescriptors = ancestorDescriptors
        }), nameof (GetAncestors));
        return ancestorDescriptors;
      }), actionName: nameof (GetAncestors));
    }

    public bool? IsActive(IVssRequestContext context, SubjectDescriptor subjectDescriptor) => GraphMembershipStore.s_tracer.TraceTimedAction<bool?>(context, GraphMembershipStore.TracePoints.IsActive, (Func<bool?>) (() =>
    {
      context.TraceDataConditionally(15270501, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Received input parameter subjectDescriptor: ", (Func<object>) (() => (object) new
      {
        subjectDescriptor = subjectDescriptor
      }), nameof (IsActive));
      Guid storageKeyByDescriptor = GraphMembershipStore.GetStorageKeyByDescriptor(context, subjectDescriptor);
      if (storageKeyByDescriptor == new Guid())
      {
        context.TraceDataConditionally(15270502, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returning null because we failed to find storage key", (Func<object>) (() => (object) new
        {
          subjectDescriptor = subjectDescriptor
        }), nameof (IsActive));
        return new bool?();
      }
      IEnumerable<Guid> identityIds = (IEnumerable<Guid>) new Guid[1]
      {
        storageKeyByDescriptor
      };
      Guid scopeId = context.ServiceHost.InstanceId;
      context.TraceDataConditionally(15270503, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Reading from database", (Func<object>) (() => (object) new
      {
        identityIds = identityIds,
        scopeId = scopeId
      }), nameof (IsActive));
      ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData scopeVisibility;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(context))
        scopeVisibility = groupComponent.ReadScopeVisiblity(identityIds, scopeId).SingleOrDefault<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>();
      context.TraceDataConditionally(15270504, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Read from database", (Func<object>) (() => (object) new
      {
        scopeVisibility = scopeVisibility
      }), nameof (IsActive));
      return scopeVisibility?.Active;
    }), actionName: nameof (IsActive));

    public IDictionary<SubjectDescriptor, bool?> AreActive(
      IVssRequestContext context,
      IEnumerable<SubjectDescriptor> subjectDescriptors)
    {
      return (IDictionary<SubjectDescriptor, bool?>) GraphMembershipStore.s_tracer.TraceTimedAction<Dictionary<SubjectDescriptor, bool?>>(context, GraphMembershipStore.TracePoints.AreActive, (Func<Dictionary<SubjectDescriptor, bool?>>) (() =>
      {
        context.TraceDataConditionally(15270601, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Received input parameters subjectDescriptors: ", (Func<object>) (() => (object) new
        {
          subjectDescriptors = subjectDescriptors
        }), nameof (AreActive));
        Dictionary<SubjectDescriptor, bool?> descriptorToIsActiveMap = subjectDescriptors.ToDedupedDictionary<SubjectDescriptor, SubjectDescriptor, bool?>((Func<SubjectDescriptor, SubjectDescriptor>) (x => x), (Func<SubjectDescriptor, bool?>) (y => new bool?()));
        int descriptorsOfInvalidStorageKeysCount;
        IDictionary<SubjectDescriptor, Guid> keysByDescriptors = this.GetValidStorageKeysByDescriptors(context, (IEnumerable<SubjectDescriptor>) descriptorToIsActiveMap.Keys, out descriptorsOfInvalidStorageKeysCount);
        if (descriptorsOfInvalidStorageKeysCount == descriptorToIsActiveMap.Count)
          return descriptorToIsActiveMap;
        IList<Guid> storageKeys = (IList<Guid>) keysByDescriptors.Select<KeyValuePair<SubjectDescriptor, Guid>, Guid>((Func<KeyValuePair<SubjectDescriptor, Guid>, Guid>) (x => x.Value)).ToList<Guid>();
        Guid instanceId = context.ServiceHost.InstanceId;
        context.TraceDataConditionally(15270603, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), string.Format("Reading from database: scopeId: {0} and storageKeys:", (object) instanceId), (Func<object>) (() => (object) new
        {
          storageKeys = storageKeys
        }), nameof (AreActive));
        IList<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData> list;
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(context))
          list = (IList<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) groupComponent.ReadScopeVisiblity((IEnumerable<Guid>) storageKeys, instanceId).ToList<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>();
        Dictionary<Guid, SubjectDescriptor> dictionary = keysByDescriptors.ToDictionary<KeyValuePair<SubjectDescriptor, Guid>, Guid, SubjectDescriptor>((Func<KeyValuePair<SubjectDescriptor, Guid>, Guid>) (kvp => kvp.Value), (Func<KeyValuePair<SubjectDescriptor, Guid>, SubjectDescriptor>) (kvp => kvp.Key));
        foreach (ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData filteredIdentityData in list.Where<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((Func<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData, bool>) (x => x != null)))
        {
          SubjectDescriptor key = dictionary[filteredIdentityData.IdentityId];
          descriptorToIsActiveMap[key] = new bool?(filteredIdentityData.Active);
        }
        context.TraceDataConditionally(15270604, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Read from database", (Func<object>) (() => (object) new
        {
          descriptorToIsActiveMap = descriptorToIsActiveMap
        }), nameof (AreActive));
        return descriptorToIsActiveMap;
      }), actionName: nameof (AreActive));
    }

    public void AddMember(
      IVssRequestContext context,
      SubjectDescriptor groupDescriptor,
      SubjectDescriptor memberDescriptor)
    {
      GraphMembershipStore.s_tracer.TraceTimedAction(context, GraphMembershipStore.TracePoints.AddMember, (Action) (() =>
      {
        context.TraceDataConditionally(15270311, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Received input parameters", (Func<object>) (() => (object) new
        {
          groupDescriptor = groupDescriptor,
          memberDescriptor = memberDescriptor
        }), nameof (AddMember));
        context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        Guid storageKeyByDescriptor = GraphMembershipStore.GetStorageKeyByDescriptor(context, memberDescriptor);
        if (storageKeyByDescriptor == new Guid())
          throw new GraphSubjectNotFoundException(memberDescriptor);
        int sequenceId = GraphMembershipStore.UpdateMemberships(context, groupDescriptor, storageKeyByDescriptor, true, 15270312, 15270313);
        context.TraceDataConditionally(15270314, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Firing membership change", (Func<object>) (() => (object) new
        {
          sequenceId = sequenceId
        }), nameof (AddMember));
        context.GetService<IGraphMembershipChangeHandler>().FireMembershipChange(context, sequenceId);
      }), actionName: nameof (AddMember));
    }

    public void RemoveMember(
      IVssRequestContext context,
      SubjectDescriptor groupDescriptor,
      SubjectDescriptor memberDescriptor)
    {
      GraphMembershipStore.s_tracer.TraceTimedAction(context, GraphMembershipStore.TracePoints.RemoveMember, (Action) (() =>
      {
        context.TraceDataConditionally(15270321, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Received input parameters", (Func<object>) (() => (object) new
        {
          groupDescriptor = groupDescriptor,
          memberDescriptor = memberDescriptor
        }), nameof (RemoveMember));
        context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        Guid storageKeyByDescriptor = GraphMembershipStore.GetStorageKeyByDescriptor(context, memberDescriptor);
        if (storageKeyByDescriptor == new Guid())
          throw new GraphSubjectNotFoundException(memberDescriptor);
        int sequenceId = GraphMembershipStore.UpdateMemberships(context, groupDescriptor, storageKeyByDescriptor, false, 15270322, 15270323);
        context.TraceDataConditionally(15270324, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Firing membership change", (Func<object>) (() => (object) new
        {
          sequenceId = sequenceId
        }), nameof (RemoveMember));
        context.GetService<IGraphMembershipChangeHandler>().FireMembershipChange(context, sequenceId);
      }), actionName: nameof (RemoveMember));
    }

    public IEnumerable<Guid> GetOrganizationStorageKeysForMembersInScope(IVssRequestContext context)
    {
      if (!context.IsFeatureEnabled("VisualStudio.Services.Graph.Store.EnableGetStorageKeysInScope"))
        throw new FeatureFlagNotEnabledGraphException(Resources.GetStorageKeysInScopeFeatureIsUnavailable());
      return GraphMembershipStore.s_tracer.TraceTimedAction<IEnumerable<Guid>>(context, (TimedActionTracePoints) GraphMembershipStore.TracePoints.ReadStorageKeysInScope, (Func<IEnumerable<Guid>>) (() =>
      {
        context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
        IEnumerable<Guid> organizationStorageKeys = (IEnumerable<Guid>) new List<Guid>();
        organizationStorageKeys = !context.ExecutionEnvironment.IsHostedDeployment ? GraphMembershipStore.ReadOrganizationStorageKeysUsingExpandEveryone(context) : GraphMembershipStore.ReadOrganizationStorageKeysUsingScopeVisibility(context);
        if (organizationStorageKeys != null)
          this.UpdateMegaTenantState(context, organizationStorageKeys.Count<Guid>());
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.ReadStorageKeysInScope.Output, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Read organization-level storage keys in current scope", (Func<object>) (() => (object) new
        {
          organizationStorageKeys = organizationStorageKeys
        }), nameof (GetOrganizationStorageKeysForMembersInScope));
        return organizationStorageKeys;
      }), actionName: nameof (GetOrganizationStorageKeysForMembersInScope));
    }

    private static IEnumerable<Guid> ReadOrganizationStorageKeysUsingExpandEveryone(
      IVssRequestContext context)
    {
      Guid instanceId = context.ServiceHost.InstanceId;
      IdentityDescriptor validUsersIdentityDescriptor = IdentityMapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup, instanceId);
      SubjectDescriptor subjectDescriptor = validUsersIdentityDescriptor.ToSubjectDescriptor(context);
      Guid storageKeyByDescriptor = GraphMembershipStore.GetStorageKeyByDescriptor(context, subjectDescriptor);
      if (storageKeyByDescriptor == new Guid())
      {
        context.TraceDataConditionally(GraphMembershipStore.TracePoints.ReadStorageKeysInScope.StorageKeyNotFound, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Returning null because we failed to find storage key for valid users group", (Func<object>) (() => (object) new
        {
          validUsersIdentityDescriptor = validUsersIdentityDescriptor
        }), nameof (ReadOrganizationStorageKeysUsingExpandEveryone));
        return (IEnumerable<Guid>) null;
      }
      List<GroupMembership> descendantMemberships;
      GraphMembershipStore.ReadMemberships(context, storageKeyByDescriptor, QueryMembership.ExpandedDown, QueryMembership.None, GraphMembershipStore.TracePoints.GetDescendants, out descendantMemberships, out List<GroupMembership> _, out ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData _);
      return descendantMemberships == null ? (IEnumerable<Guid>) null : descendantMemberships.Select<GroupMembership, Guid>((Func<GroupMembership, Guid>) (descendantMembership => descendantMembership.Id));
    }

    private static IEnumerable<Guid> ReadOrganizationStorageKeysUsingScopeVisibility(
      IVssRequestContext context)
    {
      context.TraceEnter(15270700, "Graph", nameof (GraphMembershipStore), nameof (ReadOrganizationStorageKeysUsingScopeVisibility));
      try
      {
        Guid instanceId = context.ServiceHost.InstanceId;
        IList<Guid> identityIds;
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(context))
          identityIds = groupComponent.GetIdentityIdsVisibleInScope(instanceId);
        context.TraceDataConditionally(15270701, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), string.Format("Found identities in scope {0} using GroupScopeVisibility", (object) instanceId), (Func<object>) (() => (object) new
        {
          identityIds = identityIds
        }), nameof (ReadOrganizationStorageKeysUsingScopeVisibility));
        return (IEnumerable<Guid>) identityIds;
      }
      finally
      {
        context.TraceLeave(15270702, "Graph", nameof (GraphMembershipStore), nameof (ReadOrganizationStorageKeysUsingScopeVisibility));
      }
    }

    public bool IsMegaTenant(IVssRequestContext context) => context.IsFeatureEnabled("VisualStudio.Services.Graph.Store.EnableIsMegaTenant") && context.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && context.GetService<IVssRegistryService>().GetValue<bool>(context, (RegistryQuery) "/Service/Sps/Graph/MegaTenant", false);

    private void UpdateMegaTenantState(IVssRequestContext context, int count)
    {
      if (!context.IsFeatureEnabled("VisualStudio.Services.Graph.Store.EnableIsMegaTenant") || !context.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      int num = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/Sps/Graph/MegaTenantSize", 5000);
      bool flag = this.IsMegaTenant(context);
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      if (count > num)
      {
        if (flag)
          return;
        service.SetValue<bool>(context, "/Service/Sps/Graph/MegaTenant", true);
      }
      else
      {
        if (!flag)
          return;
        service.SetValue<bool>(context, "/Service/Sps/Graph/MegaTenant", false);
      }
    }

    private static void ReadMemberships(
      IVssRequestContext context,
      Guid storageKey,
      QueryMembership descendantQuery,
      QueryMembership ancestorQuery,
      GraphMembershipStore.ReadMembershipTracePoints tracePoints,
      out List<GroupMembership> descendantMemberships,
      out List<GroupMembership> ancestorMemberships,
      out ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData scopeVisiblity)
    {
      IEnumerable<Guid> identityIds = (IEnumerable<Guid>) new Guid[1]
      {
        storageKey
      };
      bool includeRestriced = false;
      int? minInactivatedTime = new int?();
      Guid scopeId = context.ServiceHost.InstanceId;
      bool returnVisibleIdentities = true;
      bool enableUseXtpProc = context.IsFeatureEnabled(PlatformIdentityStore.EnableUseReadGroupMembershipXtpProcFeatureFlag);
      context.TraceDataConditionally(tracePoints.ComponentInput, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Reading from database", (Func<object>) (() => (object) new
      {
        identityIds = identityIds,
        descendantQuery = descendantQuery,
        ancestorQuery = ancestorQuery,
        includeRestriced = includeRestriced,
        minInactivatedTime = minInactivatedTime,
        scopeId = scopeId,
        returnVisibleIdentities = returnVisibleIdentities
      }), nameof (ReadMemberships));
      List<GroupMembership> descendantMembershipsResult = (List<GroupMembership>) null;
      List<GroupMembership> ancestorMembershipsResult = (List<GroupMembership>) null;
      ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData scopeVisbilityResult = (ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData) null;
      bool inScopeMembershipsOnly = IdentityMembershipHelper.ShouldReturnInScopeMemberships(context);
      using (ReadGroupMembershipsComponentBase membershipsComponent = PlatformIdentityStore.CreateReadGroupMembershipsComponent(context))
      {
        using (ResultCollection resultCollection = membershipsComponent.ReadMemberships(identityIds, descendantQuery, ancestorQuery, includeRestriced, minInactivatedTime, scopeId, returnVisibleIdentities, enableUseXtpProc, inScopeMembershipsOnly: inScopeMembershipsOnly))
        {
          if (ancestorQuery != QueryMembership.None)
            ancestorMembershipsResult = resultCollection.GetCurrent<GroupMembership>().Items;
          resultCollection.NextResult();
          if (descendantQuery != QueryMembership.None)
            descendantMembershipsResult = resultCollection.GetCurrent<GroupMembership>().Items;
          resultCollection.NextResult();
          scopeVisbilityResult = resultCollection.GetCurrent<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>().Items.SingleOrDefault<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>();
        }
      }
      context.TraceDataConditionally(tracePoints.ComponentOutput, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Read from database", (Func<object>) (() => (object) new
      {
        storageKey = storageKey,
        descendantMembershipsResult = descendantMembershipsResult,
        ancestorMembershipsResult = ancestorMembershipsResult,
        scopeVisbilityResult = scopeVisbilityResult
      }), nameof (ReadMemberships));
      descendantMemberships = descendantMembershipsResult;
      ancestorMemberships = ancestorMembershipsResult;
      scopeVisiblity = scopeVisbilityResult;
    }

    private static void ReadMembershipsBatch(
      IVssRequestContext context,
      IEnumerable<KeyValuePair<SubjectDescriptor, Guid>> descriptorToStorageKeyMap,
      QueryMembership descendantQuery,
      QueryMembership ancestorQuery,
      GraphMembershipStore.ReadMembershipTracePoints tracePoints,
      out Dictionary<SubjectDescriptor, List<GroupMembership>> descendantMemberships,
      out Dictionary<SubjectDescriptor, List<GroupMembership>> ancestorMemberships,
      out Dictionary<SubjectDescriptor, ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData> scopeVisibilities)
    {
      Dictionary<Guid, SubjectDescriptor> storageKeyToDescriptorMap = descriptorToStorageKeyMap.ToDictionary<KeyValuePair<SubjectDescriptor, Guid>, Guid, SubjectDescriptor>((Func<KeyValuePair<SubjectDescriptor, Guid>, Guid>) (kp => kp.Value), (Func<KeyValuePair<SubjectDescriptor, Guid>, SubjectDescriptor>) (kp => kp.Key));
      IEnumerable<Guid> storageKeys = (IEnumerable<Guid>) new List<Guid>((IEnumerable<Guid>) storageKeyToDescriptorMap.Keys);
      bool includeRestriced = false;
      int? minInactivatedTime = new int?();
      Guid scopeId = context.ServiceHost.InstanceId;
      bool returnVisibleIdentities = true;
      bool enableUseXtpProc = context.IsFeatureEnabled(PlatformIdentityStore.EnableUseReadGroupMembershipXtpProcFeatureFlag);
      context.TraceDataConditionally(tracePoints.ComponentInput, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Reading from database", (Func<object>) (() => (object) new
      {
        storageKeys = storageKeys,
        descendantQuery = descendantQuery,
        ancestorQuery = ancestorQuery,
        includeRestriced = includeRestriced,
        minInactivatedTime = minInactivatedTime,
        scopeId = scopeId,
        returnVisibleIdentities = returnVisibleIdentities
      }), nameof (ReadMembershipsBatch));
      Dictionary<SubjectDescriptor, List<GroupMembership>> ancestorMembershipsResults = new Dictionary<SubjectDescriptor, List<GroupMembership>>();
      Dictionary<SubjectDescriptor, List<GroupMembership>> descendantMembershipsResults = new Dictionary<SubjectDescriptor, List<GroupMembership>>();
      Dictionary<SubjectDescriptor, ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData> scopeVisbilityResults = new Dictionary<SubjectDescriptor, ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>();
      bool inScopeMembershipsOnly = IdentityMembershipHelper.ShouldReturnInScopeMemberships(context);
      using (ReadGroupMembershipsComponentBase membershipsComponent = PlatformIdentityStore.CreateReadGroupMembershipsComponent(context))
      {
        using (ResultCollection resultCollection = membershipsComponent.ReadMemberships(storageKeys, descendantQuery, ancestorQuery, includeRestriced, minInactivatedTime, scopeId, returnVisibleIdentities, enableUseXtpProc, inScopeMembershipsOnly: inScopeMembershipsOnly))
        {
          if (ancestorQuery != QueryMembership.None)
          {
            foreach (GroupMembership groupMembership in resultCollection.GetCurrent<GroupMembership>())
            {
              List<GroupMembership> groupMembershipList;
              if (!ancestorMembershipsResults.TryGetValue(storageKeyToDescriptorMap[groupMembership.QueriedId], out groupMembershipList))
                ancestorMembershipsResults.Add(storageKeyToDescriptorMap[groupMembership.QueriedId], new List<GroupMembership>()
                {
                  groupMembership
                });
              else
                groupMembershipList.Add(groupMembership);
            }
          }
          resultCollection.NextResult();
          if (descendantQuery != QueryMembership.None)
          {
            foreach (GroupMembership groupMembership in resultCollection.GetCurrent<GroupMembership>())
            {
              List<GroupMembership> groupMembershipList;
              if (!descendantMembershipsResults.TryGetValue(storageKeyToDescriptorMap[groupMembership.QueriedId], out groupMembershipList))
                descendantMembershipsResults.Add(storageKeyToDescriptorMap[groupMembership.QueriedId], new List<GroupMembership>()
                {
                  groupMembership
                });
              else
                groupMembershipList.Add(groupMembership);
            }
          }
          resultCollection.NextResult();
          foreach (ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData filteredIdentityData in resultCollection.GetCurrent<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>())
            scopeVisbilityResults.Add(storageKeyToDescriptorMap[filteredIdentityData.IdentityId], filteredIdentityData);
        }
      }
      context.TraceDataConditionally(tracePoints.ComponentOutput, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Read from database", (Func<object>) (() => (object) new
      {
        storageKeyToDescriptorMap = storageKeyToDescriptorMap,
        descendantMembershipsResults = descendantMembershipsResults,
        ancestorMembershipsResults = ancestorMembershipsResults,
        scopeVisbilityResults = scopeVisbilityResults
      }), nameof (ReadMembershipsBatch));
      descendantMemberships = descendantMembershipsResults;
      ancestorMemberships = ancestorMembershipsResults;
      scopeVisibilities = scopeVisbilityResults;
    }

    private bool IsTargetVisible(
      IVssRequestContext context,
      SubjectDescriptor descriptor,
      ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData scopeVisbility)
    {
      return scopeVisbility != null || context.ServiceHost.Is(TeamFoundationHostType.Deployment) || descriptor == this.m_everyoneGroupDescriptor;
    }

    private IList<SubjectDescriptor> GetDescriptorsFromValidMemberships(
      IVssRequestContext requestContext,
      Guid storageKey,
      IEnumerable<GroupMembership> memberships,
      GraphMembershipStore.ReadMembershipTracePoints tracePoints)
    {
      if (memberships == null)
        return (IList<SubjectDescriptor>) null;
      List<Guid> list = memberships.Where<GroupMembership>((Func<GroupMembership, bool>) (membership => this.IsValidMembership(requestContext, storageKey, membership, tracePoints))).ToList<GroupMembership>().Select<GroupMembership, Guid>((Func<GroupMembership, Guid>) (membership => membership.Id)).ToList<Guid>();
      return this.GetDescriptorsByStorageKeys(requestContext, (IList<Guid>) list);
    }

    private IDictionary<SubjectDescriptor, Guid> GetValidStorageKeysByDescriptors(
      IVssRequestContext context,
      IEnumerable<SubjectDescriptor> descriptors,
      out int descriptorsOfInvalidStorageKeysCount)
    {
      PartitionResults<KeyValuePair<SubjectDescriptor, Guid>> partitionResults = GraphMembershipStore.GetStorageKeysByDescriptors(context, descriptors).Partition<KeyValuePair<SubjectDescriptor, Guid>>((Predicate<KeyValuePair<SubjectDescriptor, Guid>>) (kvp => kvp.Value == new Guid()));
      descriptorsOfInvalidStorageKeysCount = 0;
      if (partitionResults.MatchingPartition.Any<KeyValuePair<SubjectDescriptor, Guid>>())
      {
        IEnumerable<SubjectDescriptor> subjectDescriptors = partitionResults.MatchingPartition.Select<KeyValuePair<SubjectDescriptor, Guid>, SubjectDescriptor>((Func<KeyValuePair<SubjectDescriptor, Guid>, SubjectDescriptor>) (x => x.Key));
        descriptorsOfInvalidStorageKeysCount = subjectDescriptors.Count<SubjectDescriptor>();
        context.TraceAlways(GraphMembershipStore.TracePoints.GetDescendantsBatch.StorageKeyNotFound, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Failed to find storage keys for SubjectDescriptors. {0}", (object) string.Join<SubjectDescriptor>(", ", subjectDescriptors));
      }
      return (IDictionary<SubjectDescriptor, Guid>) partitionResults.NonMatchingPartition.ToDictionary<KeyValuePair<SubjectDescriptor, Guid>, SubjectDescriptor, Guid>((Func<KeyValuePair<SubjectDescriptor, Guid>, SubjectDescriptor>) (x => x.Key), (Func<KeyValuePair<SubjectDescriptor, Guid>, Guid>) (y => y.Value));
    }

    private void AddDescendantMembershipsToResultMap(
      IVssRequestContext context,
      IDictionary<SubjectDescriptor, List<SubjectDescriptor>> descriptorToDescendantsResultMap,
      IDictionary<SubjectDescriptor, Guid> descriptorToValidGuidStorageKeyMap,
      Dictionary<SubjectDescriptor, List<GroupMembership>> descendantMemberships,
      Dictionary<SubjectDescriptor, ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData> scopeVisibilities)
    {
      foreach (SubjectDescriptor subjectDescriptor in descriptorToValidGuidStorageKeyMap.Select<KeyValuePair<SubjectDescriptor, Guid>, SubjectDescriptor>((Func<KeyValuePair<SubjectDescriptor, Guid>, SubjectDescriptor>) (x => x.Key)))
      {
        ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData scopeVisbility;
        scopeVisibilities.TryGetValue(subjectDescriptor, out scopeVisbility);
        if (!this.IsTargetVisible(context, subjectDescriptor, scopeVisbility))
        {
          context.TraceDataConditionally(GraphMembershipStore.TracePoints.GetDescendantsBatch.TargetNotVisible, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), string.Format("Descendants result null because target not visible in scope Descriptor: {0} StorageKey: {1}", (object) subjectDescriptor, (object) descriptorToValidGuidStorageKeyMap[subjectDescriptor]), methodName: nameof (AddDescendantMembershipsToResultMap));
        }
        else
        {
          descriptorToDescendantsResultMap[subjectDescriptor] = new List<SubjectDescriptor>();
          List<GroupMembership> memberships;
          if (descendantMemberships.TryGetValue(subjectDescriptor, out memberships))
          {
            IList<SubjectDescriptor> validMemberships = this.GetDescriptorsFromValidMemberships(context, descriptorToValidGuidStorageKeyMap[subjectDescriptor], (IEnumerable<GroupMembership>) memberships, GraphMembershipStore.TracePoints.GetDescendantsBatch);
            descriptorToDescendantsResultMap[subjectDescriptor] = validMemberships != null ? validMemberships.ToList<SubjectDescriptor>() : (List<SubjectDescriptor>) null;
          }
        }
      }
    }

    private static Guid GetStorageKeyByDescriptor(
      IVssRequestContext context,
      SubjectDescriptor descriptor)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<IGraphIdentifierConversionService>().GetStorageKeyByDescriptor(vssRequestContext, descriptor);
    }

    private static IReadOnlyDictionary<SubjectDescriptor, Guid> GetStorageKeysByDescriptors(
      IVssRequestContext context,
      IEnumerable<SubjectDescriptor> descriptors)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<IGraphIdentifierConversionService>().GetStorageKeysByDescriptors(vssRequestContext, descriptors);
    }

    private IList<SubjectDescriptor> GetDescriptorsByStorageKeys(
      IVssRequestContext context,
      IList<Guid> storageKeys)
    {
      if (storageKeys == null)
        return (IList<SubjectDescriptor>) null;
      IVssRequestContext vssRequestContext = context.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (IList<Guid> identityIds in storageKeys.Batch<Guid>(10000))
        identityList.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(vssRequestContext, identityIds, QueryMembership.None, (IEnumerable<string>) null));
      GraphMembershipHelper.CheckForNullIdentities<Guid>(context, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityList, storageKeys);
      return (IList<SubjectDescriptor>) identityList.Select<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>) (x => x.SubjectDescriptor)).ToList<SubjectDescriptor>();
    }

    private bool IsValidMembership(
      IVssRequestContext context,
      Guid storageKey,
      GroupMembership membership,
      GraphMembershipStore.ReadMembershipTracePoints tracePoints)
    {
      if (membership == null)
      {
        context.TraceDataConditionally(tracePoints.NullMembership, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Found null membership", (Func<object>) (() => (object) new
        {
          storageKey = storageKey
        }), nameof (IsValidMembership));
        return false;
      }
      if (membership.QueriedId != storageKey)
      {
        context.TraceDataConditionally(tracePoints.QueriedIdMismatch, TraceLevel.Error, "Graph", nameof (GraphMembershipStore), "Found membership for different subject than expected", (Func<object>) (() => (object) new
        {
          expectedQueriedId = storageKey,
          actualQueriedId = membership.QueriedId
        }), nameof (IsValidMembership));
        return false;
      }
      if (!membership.Active)
      {
        context.TraceDataConditionally(tracePoints.InactiveMembership, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Found inactive membership", (Func<object>) (() => (object) new
        {
          storageKey = storageKey,
          membership = membership
        }), nameof (IsValidMembership));
        return false;
      }
      context.TraceDataConditionally(tracePoints.ActiveMembership, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Found active membership", (Func<object>) (() => (object) new
      {
        storageKey = storageKey,
        membership = membership
      }), nameof (IsValidMembership));
      return true;
    }

    private static int UpdateMemberships(
      IVssRequestContext requestContext,
      SubjectDescriptor groupDescriptor,
      Guid memberStorageKey,
      bool active,
      int inputTracepoint,
      int outputTracepoint)
    {
      Guid scopeId = requestContext.ServiceHost.InstanceId;
      bool idempotent = true;
      bool incremental = true;
      bool insertInactiveUpdates = false;
      IEnumerable<Tuple<IdentityDescriptor, Guid, bool>> updates = (IEnumerable<Tuple<IdentityDescriptor, Guid, bool>>) new Tuple<IdentityDescriptor, Guid, bool>[1]
      {
        Tuple.Create<IdentityDescriptor, Guid, bool>(groupDescriptor.ToIdentityDescriptor(requestContext), memberStorageKey, active)
      };
      requestContext.TraceDataConditionally(inputTracepoint, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Updating database", (Func<object>) (() => (object) new
      {
        scopeId = scopeId,
        idempotent = idempotent,
        incremental = incremental,
        insertInactiveUpdates = insertInactiveUpdates,
        updates = updates
      }), nameof (UpdateMemberships));
      long sequenceId;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(requestContext))
        sequenceId = groupComponent.UpdateGroupMembership(scopeId, idempotent, incremental, insertInactiveUpdates, updates);
      requestContext.TraceDataConditionally(outputTracepoint, TraceLevel.Verbose, "Graph", nameof (GraphMembershipStore), "Updated database", (Func<object>) (() => (object) new
      {
        sequenceId = sequenceId
      }), nameof (UpdateMemberships));
      return checked ((int) sequenceId);
    }

    private static class TracePoints
    {
      internal static readonly GraphMembershipStore.ReadMembershipTracePoints GetChildren = new GraphMembershipStore.ReadMembershipTracePoints(15270130, 15270131, 15270132, 15270133, 15270134, 15270135, 15270136, 15270137, 15270138, 15270139, 15270456, 15270457, 15270458, 15270459);
      internal static readonly GraphMembershipStore.ReadMembershipTracePoints GetParents = new GraphMembershipStore.ReadMembershipTracePoints(15270140, 15270141, 15270142, 15270143, 15270144, 15270145, 15270146, 15270147, 15270148, 15270149, 15270466, 15270467, 15270468, 15270469);
      internal static readonly GraphMembershipStore.ReadMembershipTracePoints ReadStorageKeysInScope = new GraphMembershipStore.ReadMembershipTracePoints(15270270, 15270271, 15270272, 15270273, 15270274, 15270275, 15270276, 15270277, 15270278, 15270279, 15270466, 15270467, 15270468, 15270469);
      internal static readonly GraphMembershipStore.ReadMembershipTracePoints GetDescendants = new GraphMembershipStore.ReadMembershipTracePoints(15270280, 15270281, 15270282, 15270283, 15270284, 15270285, 15270286, 15270287, 15270288, 15270289, 15270476, 15270477, 15270478, 15270479);
      internal static readonly GraphMembershipStore.ReadMembershipTracePoints GetDescendantsBatch = new GraphMembershipStore.ReadMembershipTracePoints(15271280, 15271281, 15271282, 15271283, 15271284, 15271285, 15271286, 15271287, 15271288, 15271289, 15271476, 15271477, 15271478, 15271479);
      internal static readonly GraphMembershipStore.ReadMembershipTracePoints GetAncestors = new GraphMembershipStore.ReadMembershipTracePoints(15270290, 15270291, 15270292, 15270293, 15270294, 15270295, 15270296, 15270297, 15270298, 15270299, 15270486, 15270487, 15270488, 15270489);
      internal static readonly TimedActionTracePoints IsMember = new TimedActionTracePoints(15270300, 15270307, 15270308, 15270309);
      internal static readonly TimedActionTracePoints AddMember = new TimedActionTracePoints(15270310, 15270317, 15270318, 15270319);
      internal static readonly TimedActionTracePoints RemoveMember = new TimedActionTracePoints(15270320, 15270327, 15270328, 15270329);
      internal static readonly TimedActionTracePoints IsActive = new TimedActionTracePoints(15270500, 15270507, 15270508, 15270509);
      internal static readonly TimedActionTracePoints AreActive = new TimedActionTracePoints(15270600, 15270607, 15270608, 15270609);
    }

    private class ReadMembershipTracePoints : TimedActionTracePoints
    {
      public ReadMembershipTracePoints(
        int enter,
        int input,
        int storageKeyNotFound,
        int componentInput,
        int componentOutput,
        int relativesNotVisible,
        int nullMembership,
        int inactiveMembership,
        int activeMembership,
        int queriedIdMismatch,
        int output,
        int slow,
        int exception,
        int exit)
        : base(enter, slow, exception, exit)
      {
        this.Input = input;
        this.StorageKeyNotFound = storageKeyNotFound;
        this.ComponentInput = componentInput;
        this.ComponentOutput = componentOutput;
        this.TargetNotVisible = relativesNotVisible;
        this.NullMembership = nullMembership;
        this.InactiveMembership = inactiveMembership;
        this.ActiveMembership = activeMembership;
        this.QueriedIdMismatch = queriedIdMismatch;
        this.Output = output;
      }

      public int Input { get; }

      public int StorageKeyNotFound { get; }

      public int ComponentInput { get; }

      public int ComponentOutput { get; }

      public int NullMembership { get; }

      public int InactiveMembership { get; }

      public int ActiveMembership { get; }

      public int QueriedIdMismatch { get; }

      public int TargetNotVisible { get; }

      public int Output { get; }
    }
  }
}
