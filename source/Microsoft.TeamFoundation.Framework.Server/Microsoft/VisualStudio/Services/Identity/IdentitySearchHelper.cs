// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySearchHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentitySearchHelper
  {
    private const string s_area = "IdentitySearchHelper";
    private const string s_searchCacheInvalidation = "SearchCacheInvalidation";

    internal static bool IsSearchIdentitiesContextValid(IVssRequestContext context) => !context.ServiceHost.Is(TeamFoundationHostType.Deployment) || !context.ExecutionEnvironment.IsHostedDeployment;

    internal static void ValidateSearchIdentitiesContext(IVssRequestContext context)
    {
      if (!IdentitySearchHelper.IsSearchIdentitiesContextValid(context))
        throw new InvalidRequestContextHostException(FrameworkResources.SearchIdentitiesRequestContextHostMessage());
    }

    internal static IVssRequestContext GetOrganizationContext(IVssRequestContext context)
    {
      if (!context.ServiceHost.IsOnly(TeamFoundationHostType.ProjectCollection))
        return context;
      IVssRequestContext organizationContext = context.To(TeamFoundationHostType.Application);
      object obj;
      if (context.Items != null && context.Items.TryGetValue("ImsCacheConstants.Token.BypassCache", out obj) && organizationContext.Items != null && !organizationContext.Items.ContainsKey("ImsCacheConstants.Token.BypassCache"))
        organizationContext.Items["ImsCacheConstants.Token.BypassCache"] = obj;
      return organizationContext;
    }

    internal static IdentityDomain GetOrganizationDomain(IdentityDomain hostDomain) => !hostDomain.IsMaster ? hostDomain.Parent : hostDomain;

    internal static void PublishImsSearchCacheExpiryEvent(
      IVssRequestContext context,
      Guid scopeId,
      bool shouldPublishForEnterpriseDomain)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Application);
      if (!shouldPublishForEnterpriseDomain && vssRequestContext.ServiceHost.InstanceId == scopeId)
      {
        context.TraceConditionally(1755700, TraceLevel.Info, nameof (IdentitySearchHelper), "SearchCacheInvalidation", (Func<string>) (() => string.Format("PublishImsSearchCacheExpiryEvent for scopeId:{0} is disabled because it is targetted the enterprise domain", (object) scopeId)));
      }
      else
      {
        ImsSearchCacheExpiryEvent notificationEvent = new ImsSearchCacheExpiryEvent()
        {
          HostId = vssRequestContext.ServiceHost.InstanceId,
          ScopeId = scopeId
        };
        vssRequestContext.GetService<ITeamFoundationEventService>().PublishNotification(vssRequestContext, (object) notificationEvent);
      }
    }

    internal static IList<Microsoft.VisualStudio.Services.Identity.Identity> FilterOutSystemIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x =>
      {
        if (x == null || x.IsContainer && x.GetProperty<string>("RestrictedVisible", (string) null) != null || ServicePrincipals.IsServicePrincipal(requestContext, x.Descriptor) || IdentityHelper.IsAnonymousPrincipal(x.Descriptor))
          return false;
        return !(x.Descriptor != (IdentityDescriptor) null) || !x.Descriptor.IsImportedIdentityType();
      })).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    internal static bool ProcessChangesOnSearchCaches(
      IVssRequestContext context,
      ICollection<GroupScopeVisibiltyChangeInfo> groupScopeVisibiltyChanges)
    {
      context.TraceEnter(1755600, nameof (IdentitySearchHelper), "SearchCacheInvalidation", nameof (ProcessChangesOnSearchCaches));
      try
      {
        if (groupScopeVisibiltyChanges.IsNullOrEmpty<GroupScopeVisibiltyChangeInfo>())
          return true;
        context.TraceConditionally(1755602, TraceLevel.Info, nameof (IdentitySearchHelper), "SearchCacheInvalidation", (Func<string>) (() => "ProcessChangesOnSearchCaches with groupScopeVisibiltyChanges:" + groupScopeVisibiltyChanges.Serialize<ICollection<GroupScopeVisibiltyChangeInfo>>()));
        IVssRequestContext organizationContext = IdentitySearchHelper.GetOrganizationContext(context);
        organizationContext.TraceEnter(1754900, nameof (IdentitySearchHelper), "SearchCacheInvalidation", nameof (ProcessChangesOnSearchCaches));
        Guid organizationScopeId = organizationContext.ServiceHost.InstanceId;
        HashSet<Guid> allIdentityIds;
        Dictionary<Guid, HashSet<Guid>> identityIdsMap = IdentitySearchHelper.PopulateScopeIdToIdentityIdsMap(groupScopeVisibiltyChanges, out allIdentityIds);
        organizationContext.TraceSerializedConditionally(1754903, TraceLevel.Verbose, nameof (IdentitySearchHelper), "SearchCacheInvalidation", "scopeIdToIdentityIdsMap:{0},allIdentityIds:{1}", (object) identityIdsMap, (object) allIdentityIds);
        if (allIdentityIds.IsNullOrEmpty<Guid>())
        {
          organizationContext.Trace(1754904, TraceLevel.Verbose, nameof (IdentitySearchHelper), "SearchCacheInvalidation", "Found no new identities to update, returning");
          return true;
        }
        IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identityMap = IdentitySearchHelper.PopulateIdToIdentityMap(organizationContext, (IList<Guid>) allIdentityIds.ToList<Guid>());
        organizationContext.TraceSerializedConditionally(1754905, TraceLevel.Verbose, nameof (IdentitySearchHelper), "SearchCacheInvalidation", "identityIdToIdentityMap:{0}", (object) identityMap);
        Dictionary<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>> identitiesMap = IdentitySearchHelper.PopulateScopeIdToIdentitiesMap((IDictionary<Guid, HashSet<Guid>>) identityIdsMap, identityMap);
        organizationContext.TraceSerializedConditionally(1754907, TraceLevel.Verbose, nameof (IdentitySearchHelper), "SearchCacheInvalidation", "scopeIdToIdentitiesMap:{0}", (object) identitiesMap);
        IImsCacheService service = organizationContext.GetService<IImsCacheService>();
        foreach (KeyValuePair<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>> keyValuePair in identitiesMap)
        {
          Guid scopeId = keyValuePair.Key;
          IList<Microsoft.VisualStudio.Services.Identity.Identity> newlyAddedIdentities = IdentitySearchHelper.FilterOutSystemIdentities(organizationContext, keyValuePair.Value);
          if (scopeId == organizationScopeId)
          {
            IList<Microsoft.VisualStudio.Services.Identity.Identity> source = newlyAddedIdentities;
            newlyAddedIdentities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) (source != null ? source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x.LocalScopeId.Equals(organizationScopeId))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>() : (List<Microsoft.VisualStudio.Services.Identity.Identity>) null);
          }
          if (!newlyAddedIdentities.IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>())
          {
            organizationContext.TraceConditionally(1754909, TraceLevel.Info, nameof (IdentitySearchHelper), "SearchCacheInvalidation", (Func<string>) (() => string.Format("ProcessChangesOnSearchCaches for scopeId:{0} and newlyAddedIdentities: {1}", (object) scopeId, (object) newlyAddedIdentities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.GetProperty<Guid>("CUID", x.Id))).Serialize<IEnumerable<Guid>>())));
            service.ProcessChangesOnSearchCaches(organizationContext, scopeId, newlyAddedIdentities);
          }
        }
        organizationContext.TraceLeave(1754920, nameof (IdentitySearchHelper), "SearchCacheInvalidation", nameof (ProcessChangesOnSearchCaches));
        return true;
      }
      catch (Exception ex)
      {
        context.TraceException(1754800, nameof (IdentitySearchHelper), "SearchCacheInvalidation", ex);
        return false;
      }
    }

    private static Dictionary<Guid, HashSet<Guid>> PopulateScopeIdToIdentityIdsMap(
      ICollection<GroupScopeVisibiltyChangeInfo> groupScopeVisibiltyChanges,
      out HashSet<Guid> allIdentityIds)
    {
      Dictionary<Guid, HashSet<Guid>> identityIdsMap = new Dictionary<Guid, HashSet<Guid>>();
      allIdentityIds = new HashSet<Guid>();
      foreach (GroupScopeVisibiltyChangeInfo visibiltyChangeInfo in groupScopeVisibiltyChanges.Where<GroupScopeVisibiltyChangeInfo>((Func<GroupScopeVisibiltyChangeInfo, bool>) (x => x != null && x.Active && !x.ScopeId.Equals(Guid.Empty) && x.GroupScopeType == GroupScopeType.ServiceHost)))
      {
        Guid scopeId = visibiltyChangeInfo.ScopeId;
        HashSet<Guid> guidSet;
        if (!identityIdsMap.TryGetValue(visibiltyChangeInfo.ScopeId, out guidSet))
          identityIdsMap[scopeId] = guidSet = new HashSet<Guid>();
        guidSet.Add(visibiltyChangeInfo.IdentityId);
        allIdentityIds.Add(visibiltyChangeInfo.IdentityId);
      }
      return identityIdsMap;
    }

    private static Dictionary<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>> PopulateScopeIdToIdentitiesMap(
      IDictionary<Guid, HashSet<Guid>> scopeIdToIdentityIdsMap,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> idToIdentityMap)
    {
      Dictionary<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>> identitiesMap = new Dictionary<Guid, IList<Microsoft.VisualStudio.Services.Identity.Identity>>();
      foreach (KeyValuePair<Guid, HashSet<Guid>> scopeIdToIdentityIds in (IEnumerable<KeyValuePair<Guid, HashSet<Guid>>>) scopeIdToIdentityIdsMap)
      {
        Guid key = scopeIdToIdentityIds.Key;
        HashSet<Guid> source = scopeIdToIdentityIds.Value;
        Microsoft.VisualStudio.Services.Identity.Identity identity;
        identitiesMap[key] = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) source.Where<Guid>((Func<Guid, bool>) (x => idToIdentityMap.TryGetValue(x, out identity) && identity != null)).Select<Guid, Microsoft.VisualStudio.Services.Identity.Identity>((Func<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) (x => idToIdentityMap[x])).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      return identitiesMap;
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
          requestContext.Trace(1754930, TraceLevel.Warning, nameof (IdentitySearchHelper), "SearchCacheInvalidation", string.Format("Found null identity for id:{0}", (object) identityId));
        identityMap[identityId] = identity;
      }
      return (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) identityMap;
    }
  }
}
