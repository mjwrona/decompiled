// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PlatformIdentitySearchHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class PlatformIdentitySearchHelper
  {
    private const string s_area = "PlatformIdentitySearchHelper";
    private const string s_searchCacheInvalidation = "SearchCacheInvalidation";
    internal const string PopulateGroupScopeVisibilityInvalidations = "VisualStudio.Services.Identity.Invalidations.PopulateGroupScopeVisibilityInvalidations";

    internal static bool ProcessChangesOnPlatformSearchCaches(
      IVssRequestContext context,
      Guid scopeId,
      IList<MembershipChangeInfo> membershipChanges)
    {
      if (membershipChanges.IsNullOrEmpty<MembershipChangeInfo>())
        return true;
      try
      {
        IVssRequestContext organizationContext = IdentitySearchHelper.GetOrganizationContext(context);
        IdentitySearchHelper.ProcessChangesOnSearchCaches(organizationContext, (ICollection<GroupScopeVisibiltyChangeInfo>) PlatformIdentitySearchHelper.PopulateGroupScopeVisibilityChangesForNewlyAddedGroupsAndUsers(organizationContext, membershipChanges));
        return true;
      }
      catch (Exception ex)
      {
        context.TraceException(1753800, nameof (PlatformIdentitySearchHelper), "SearchCacheInvalidation", ex);
        return false;
      }
    }

    internal static List<GroupScopeVisibiltyChangeInfo> PopulateGroupScopeVisibilityChangesForNewlyAddedGroupsAndUsers(
      IVssRequestContext requestContext,
      IList<MembershipChangeInfo> membershipChanges)
    {
      requestContext.TraceEnter(1753900, nameof (PlatformIdentitySearchHelper), "SearchCacheInvalidation", nameof (PopulateGroupScopeVisibilityChangesForNewlyAddedGroupsAndUsers));
      List<GroupScopeVisibiltyChangeInfo> visibiltyChangeInfoList = new List<GroupScopeVisibiltyChangeInfo>();
      if (membershipChanges.IsNullOrEmpty<MembershipChangeInfo>())
      {
        requestContext.Trace(1753902, TraceLevel.Verbose, nameof (PlatformIdentitySearchHelper), "SearchCacheInvalidation", "No membership changes found, returning empty list");
        return visibiltyChangeInfoList;
      }
      requestContext.TraceSerializedConditionally(1753904, TraceLevel.Verbose, nameof (PlatformIdentitySearchHelper), "SearchCacheInvalidation", "membershipChanges:{0}", (object) membershipChanges);
      IList<Tuple<Guid, Guid>> containerIds = PlatformIdentitySearchHelper.PopulateNewlyAddedMemberIdsToContainerIds(membershipChanges);
      requestContext.TraceSerializedConditionally(1753906, TraceLevel.Verbose, nameof (PlatformIdentitySearchHelper), "SearchCacheInvalidation", "newlyAddedMemberIdsToContainerIds:{0}", (object) containerIds);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IList<Tuple<Guid, Guid>> containerScopeIds = PlatformIdentitySearchHelper.PopulateMemberIdToContainerScopeIds(requestContext1, containerIds);
      requestContext.TraceSerializedConditionally(1753908, TraceLevel.Verbose, nameof (PlatformIdentitySearchHelper), "SearchCacheInvalidation", "newlyAddedMemberIdToContainerScopeId:{0}", (object) containerScopeIds);
      IDictionary<Guid, IdentityScope> scopeMap = (IDictionary<Guid, IdentityScope>) null;
      IDictionary<Guid, HashSet<Guid>> ancestorScopeIds = PlatformIdentitySearchHelper.PopulateMemberIdToAncestorScopeIds(requestContext1, containerScopeIds, out scopeMap);
      requestContext.TraceSerializedConditionally(1753910, TraceLevel.Verbose, nameof (PlatformIdentitySearchHelper), "SearchCacheInvalidation", "newlyAddedMemberIdToAncestorScopeIds:{0}, scopeMap:{1}", (object) ancestorScopeIds, (object) scopeMap);
      foreach (KeyValuePair<Guid, HashSet<Guid>> keyValuePair in (IEnumerable<KeyValuePair<Guid, HashSet<Guid>>>) ancestorScopeIds)
      {
        Guid key1 = keyValuePair.Key;
        foreach (Guid key2 in keyValuePair.Value)
        {
          IdentityScope identityScope = scopeMap[key2];
          visibiltyChangeInfoList.Add(new GroupScopeVisibiltyChangeInfo()
          {
            IdentityId = key1,
            ScopeId = identityScope.LocalScopeId,
            Active = true,
            GroupScopeType = identityScope.ScopeType
          });
        }
      }
      requestContext.TraceLeave(1753920, nameof (PlatformIdentitySearchHelper), "SearchCacheInvalidation", nameof (PopulateGroupScopeVisibilityChangesForNewlyAddedGroupsAndUsers));
      return visibiltyChangeInfoList;
    }

    private static IList<Tuple<Guid, Guid>> PopulateNewlyAddedMemberIdsToContainerIds(
      IList<MembershipChangeInfo> membershipChanges)
    {
      List<Tuple<Guid, Guid>> containerIds = new List<Tuple<Guid, Guid>>();
      foreach (MembershipChangeInfo membershipChangeInfo in membershipChanges.Where<MembershipChangeInfo>((Func<MembershipChangeInfo, bool>) (x => x != null && x.Active)))
      {
        if (membershipChangeInfo.IsMemberGroup)
        {
          if (IdentityHelper.IsWellKnownGroup(membershipChangeInfo.ContainerDescriptor, GroupWellKnownIdentityDescriptors.EveryoneGroup))
            containerIds.Add(new Tuple<Guid, Guid>(membershipChangeInfo.MemberId, membershipChangeInfo.ContainerId));
        }
        else if (IdentityHelper.IsWellKnownGroup(membershipChangeInfo.ContainerDescriptor, GroupWellKnownIdentityDescriptors.LicensedUsersGroup))
          containerIds.Add(new Tuple<Guid, Guid>(membershipChangeInfo.MemberId, membershipChangeInfo.ContainerId));
      }
      return (IList<Tuple<Guid, Guid>>) containerIds;
    }

    private static IDictionary<Guid, HashSet<Guid>> PopulateMemberIdToAncestorScopeIds(
      IVssRequestContext requestContext,
      IList<Tuple<Guid, Guid>> newlyAddedMemberIdToContainerScopeId,
      out IDictionary<Guid, IdentityScope> scopeMap)
    {
      scopeMap = (IDictionary<Guid, IdentityScope>) new Dictionary<Guid, IdentityScope>();
      HashSet<Guid> guidSet = new HashSet<Guid>(newlyAddedMemberIdToContainerScopeId.Select<Tuple<Guid, Guid>, Guid>((Func<Tuple<Guid, Guid>, Guid>) (x => x.Item2)));
      Dictionary<Guid, IList<Guid>> dictionary = new Dictionary<Guid, IList<Guid>>(guidSet.Count);
      foreach (Guid guid in guidSet)
        dictionary[guid] = PlatformIdentitySearchHelper.GetAncestorScopeIds(requestContext, guid, scopeMap);
      Dictionary<Guid, HashSet<Guid>> ancestorScopeIds = new Dictionary<Guid, HashSet<Guid>>();
      foreach (Tuple<Guid, Guid> tuple in (IEnumerable<Tuple<Guid, Guid>>) newlyAddedMemberIdToContainerScopeId)
      {
        Guid key = tuple.Item1;
        if (!ancestorScopeIds.ContainsKey(key))
          ancestorScopeIds[key] = new HashSet<Guid>();
      }
      foreach (Tuple<Guid, Guid> tuple in (IEnumerable<Tuple<Guid, Guid>>) newlyAddedMemberIdToContainerScopeId)
      {
        Guid key1 = tuple.Item1;
        Guid key2 = tuple.Item2;
        ancestorScopeIds[key1].UnionWith((IEnumerable<Guid>) dictionary[key2]);
      }
      return (IDictionary<Guid, HashSet<Guid>>) ancestorScopeIds;
    }

    private static IList<Guid> GetAncestorScopeIds(
      IVssRequestContext requestContext,
      Guid scopeId,
      IDictionary<Guid, IdentityScope> scopeMap)
    {
      HashSet<Guid> source = new HashSet<Guid>();
      IdentityService service = requestContext.GetService<IdentityService>();
      Guid guid = scopeId;
      Guid instanceId = IdentitySearchHelper.GetOrganizationContext(requestContext).ServiceHost.InstanceId;
      for (int index = 0; index < 5; ++index)
      {
        IdentityScope identityScope = (IdentityScope) null;
        if (!scopeMap.TryGetValue(guid, out identityScope))
          scopeMap[guid] = identityScope = service.GetScope(requestContext, guid);
        source.Add(guid);
        if (!(identityScope.ParentId == Guid.Empty))
        {
          if (instanceId.Equals(identityScope.ParentId) && identityScope.ScopeType == GroupScopeType.Generic)
            identityScope.ScopeType = GroupScopeType.ServiceHost;
          guid = identityScope.ParentId;
        }
        else
          break;
      }
      return (IList<Guid>) source.ToList<Guid>();
    }

    private static IList<Tuple<Guid, Guid>> PopulateMemberIdToContainerScopeIds(
      IVssRequestContext requestContext,
      IList<Tuple<Guid, Guid>> memberIdsToContainerIds)
    {
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityMap = PlatformIdentitySearchHelper.PopulateIdToIdentityMap(requestContext, (IList<Guid>) new HashSet<Guid>(memberIdsToContainerIds.Select<Tuple<Guid, Guid>, Guid>((Func<Tuple<Guid, Guid>, Guid>) (x => x.Item2))).ToList<Guid>());
      List<Tuple<Guid, Guid>> containerScopeIds = new List<Tuple<Guid, Guid>>();
      foreach (Tuple<Guid, Guid> idsToContainerId in (IEnumerable<Tuple<Guid, Guid>>) memberIdsToContainerIds)
      {
        Guid guid = idsToContainerId.Item1;
        Guid key = idsToContainerId.Item2;
        Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        if (!identityMap.TryGetValue(key, out identity) || identity == null || !identity.IsActive)
        {
          string message = string.Format("Ignoring the container with Id:{0}, because it is", (object) key) + identity?.ToString() == null ? "null" : "inactive";
          requestContext.Trace(1753950, TraceLevel.Warning, nameof (PlatformIdentitySearchHelper), "SearchCacheInvalidation", message);
        }
        else
        {
          Guid property = identity.GetProperty<Guid>("ScopeId", Guid.Empty);
          if (!property.Equals(Guid.Empty))
            containerScopeIds.Add(new Tuple<Guid, Guid>(guid, property));
        }
      }
      return (IList<Tuple<Guid, Guid>>) containerScopeIds;
    }

    private static IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> PopulateIdToIdentityMap(
      IVssRequestContext requestContext,
      IList<Guid> identityIds)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, identityIds, QueryMembership.None, (IEnumerable<string>) null);
      Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityMap = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>(identityIds.Count);
      for (int index = 0; index < identityIds.Count; ++index)
      {
        Guid identityId = identityIds[index];
        Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
        if (identity == null)
          requestContext.Trace(1753960, TraceLevel.Warning, nameof (PlatformIdentitySearchHelper), "SearchCacheInvalidation", string.Format("Found null identity for id:{0}", (object) identityId));
        identityMap[identityId] = identity;
      }
      return (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) identityMap;
    }
  }
}
