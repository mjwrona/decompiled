// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.WorkItemIdentityHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Identity
{
  public static class WorkItemIdentityHelper
  {
    internal const string VsidLookupCacheKey = "IdentityLookupByVsid";
    internal const string DistinctDisplayNameLookupCacheKey = "IdentityLookupByDistinctDisplayName";

    internal static IDictionary<Guid, WorkItemIdentity> EnsureVsidToWorkItemIdentityMap(
      IVssRequestContext requestContext,
      IEnumerable<Guid> vsids)
    {
      Dictionary<Guid, WorkItemIdentity> workItemIdentityMap = WorkItemIdentityHelper.GetVsIdToWorkItemIdentityMap(requestContext);
      if (vsids != null)
      {
        List<Guid> list = vsids.Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).Distinct<Guid>().Except<Guid>((IEnumerable<Guid>) workItemIdentityMap.Keys).ToList<Guid>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
        IdentityRef[] identityRefs = identities.ToIdentityRefs(requestContext);
        for (int index = 0; index < list.Count; ++index)
        {
          if (identities[index] != null)
            workItemIdentityMap.Add(list[index], new WorkItemIdentity()
            {
              IdentityRef = identityRefs[index]
            });
        }
      }
      return (IDictionary<Guid, WorkItemIdentity>) workItemIdentityMap;
    }

    internal static IDictionary<Guid, List<WorkItemIdentity>> EnsureVsidToWorkItemIdentityMap(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdentity> workItemIdentities,
      out IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      Dictionary<Guid, WorkItemIdentity> workItemIdentityMap1 = WorkItemIdentityHelper.GetVsIdToWorkItemIdentityMap(requestContext);
      Dictionary<Guid, List<WorkItemIdentity>> workItemIdentityMap2 = new Dictionary<Guid, List<WorkItemIdentity>>();
      identities = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (workItemIdentities != null)
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        WorkItemIdentity[] array1 = workItemIdentities.Where<WorkItemIdentity>((Func<WorkItemIdentity, bool>) (i => i.IdentityRef?.Descriptor.ToString() != null)).ToArray<WorkItemIdentity>();
        SubjectDescriptor[] array2 = ((IEnumerable<WorkItemIdentity>) array1).Select<WorkItemIdentity, SubjectDescriptor>((Func<WorkItemIdentity, SubjectDescriptor>) (i => i.IdentityRef.Descriptor)).ToArray<SubjectDescriptor>();
        identities = service.ReadIdentities(requestContext, (IList<SubjectDescriptor>) array2, QueryMembership.None, (IEnumerable<string>) null);
        IdentityRef[] identityRefs = identities.ToIdentityRefs(requestContext);
        for (int index = 0; index < array2.Length; ++index)
        {
          if (identities[index] != null)
          {
            Guid id = identities[index].Id;
            workItemIdentityMap1[id] = array1[index];
            workItemIdentityMap1[id].IdentityRef = identityRefs[index];
            List<WorkItemIdentity> workItemIdentityList = (List<WorkItemIdentity>) null;
            if (!workItemIdentityMap2.TryGetValue(id, out workItemIdentityList))
            {
              workItemIdentityList = new List<WorkItemIdentity>();
              workItemIdentityMap2[id] = workItemIdentityList;
            }
            workItemIdentityList.Add(array1[index]);
          }
        }
      }
      return (IDictionary<Guid, List<WorkItemIdentity>>) workItemIdentityMap2;
    }

    internal static IDictionary<Guid, WorkItemIdentity> AddIdentityToWorkItemIdentityMap(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      Dictionary<Guid, WorkItemIdentity> workItemIdentityMap = WorkItemIdentityHelper.GetVsIdToWorkItemIdentityMap(requestContext);
      if (identity != null)
        workItemIdentityMap.TryAdd<Guid, WorkItemIdentity>(identity.Id, new WorkItemIdentity()
        {
          IdentityRef = identity.ToIdentityRef(requestContext)
        });
      return (IDictionary<Guid, WorkItemIdentity>) workItemIdentityMap;
    }

    public static WorkItemIdentity GetResolvedIdentityFromDistinctDisplayName(
      IVssRequestContext requestContext,
      string distinctDisplayName)
    {
      WorkItemIdentity distinctDisplayName1;
      WorkItemIdentityHelper.GetDistinctDisplayNameMap(requestContext).TryGetValue(distinctDisplayName, out distinctDisplayName1);
      return distinctDisplayName1;
    }

    public static void AddVsidToDistinctDisplayNameMap(IVssRequestContext requestContext, Guid id)
    {
      WorkItemIdentity workItemIdentity;
      if (!WorkItemIdentityHelper.GetVsIdToWorkItemIdentityMap(requestContext).TryGetValue(id, out workItemIdentity))
        return;
      WorkItemIdentityHelper.AddIdentitiesToDistinctDisplayNameMap(requestContext, (IEnumerable<WorkItemIdentity>) new List<WorkItemIdentity>()
      {
        workItemIdentity
      });
    }

    public static void AddIdentityToDistinctDisplayNameMap(
      IVssRequestContext requestContext,
      WorkItemIdentity identity)
    {
      if (identity == null)
        return;
      WorkItemIdentityHelper.AddIdentitiesToDistinctDisplayNameMap(requestContext, (IEnumerable<WorkItemIdentity>) new WorkItemIdentity[1]
      {
        identity
      });
    }

    public static void AddIdentitiesToDistinctDisplayNameMap(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemIdentity> identities)
    {
      if (identities == null)
        return;
      Dictionary<string, WorkItemIdentity> dedupedDictionary = identities.Where<WorkItemIdentity>((Func<WorkItemIdentity, bool>) (identity => !string.IsNullOrEmpty(identity.DistinctDisplayName))).ToDedupedDictionary<WorkItemIdentity, string, WorkItemIdentity>((Func<WorkItemIdentity, string>) (identity => identity.DistinctDisplayName), (Func<WorkItemIdentity, WorkItemIdentity>) (identity => identity));
      WorkItemIdentityHelper.GetDistinctDisplayNameMap(requestContext).TryAddRange<string, WorkItemIdentity, Dictionary<string, WorkItemIdentity>>(dedupedDictionary.Select<KeyValuePair<string, WorkItemIdentity>, KeyValuePair<string, WorkItemIdentity>>((Func<KeyValuePair<string, WorkItemIdentity>, KeyValuePair<string, WorkItemIdentity>>) (kvp => kvp)));
    }

    private static Dictionary<string, WorkItemIdentity> GetDistinctDisplayNameMap(
      IVssRequestContext requestContext)
    {
      Dictionary<string, WorkItemIdentity> distinctDisplayNameMap;
      if (!requestContext.TryGetItem<Dictionary<string, WorkItemIdentity>>("IdentityLookupByDistinctDisplayName", out distinctDisplayNameMap))
      {
        distinctDisplayNameMap = new Dictionary<string, WorkItemIdentity>();
        requestContext.Items.Add("IdentityLookupByDistinctDisplayName", (object) distinctDisplayNameMap);
      }
      return distinctDisplayNameMap;
    }

    private static Dictionary<Guid, WorkItemIdentity> GetVsIdToWorkItemIdentityMap(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, WorkItemIdentity> workItemIdentityMap;
      if (!requestContext.TryGetItem<Dictionary<Guid, WorkItemIdentity>>("IdentityLookupByVsid", out workItemIdentityMap))
      {
        workItemIdentityMap = new Dictionary<Guid, WorkItemIdentity>();
        requestContext.Items.Add("IdentityLookupByVsid", (object) workItemIdentityMap);
      }
      return workItemIdentityMap;
    }

    public static IdentityRef GetIdentityRef(
      IVssRequestContext requestContext,
      object value,
      ISecuredObject secureInfoSourceForConstantIdentity = null)
    {
      if (value == null)
        return (IdentityRef) null;
      if (value is WorkItemIdentity workItemIdentity)
        return WorkItemIdentityHelper.GetIdentityRef(workItemIdentity);
      string str = value.ToString();
      ConstantIdentityRef identityRef = new ConstantIdentityRef(secureInfoSourceForConstantIdentity);
      identityRef.Descriptor = new SubjectDescriptor();
      identityRef.DisplayName = str;
      return (IdentityRef) identityRef;
    }

    public static IdentityRef GetIdentityRef(
      WorkItemIdentity workItemIdentity,
      ISecuredObject secureInfoSourceForConstantIdentity = null)
    {
      if (workItemIdentity == null)
        return (IdentityRef) null;
      if (workItemIdentity.IdentityRef != null)
        return workItemIdentity.IdentityRef;
      ConstantIdentityRef identityRef = new ConstantIdentityRef(secureInfoSourceForConstantIdentity);
      identityRef.Descriptor = new SubjectDescriptor();
      identityRef.DisplayName = workItemIdentity.DistinctDisplayName;
      return (IdentityRef) identityRef;
    }
  }
}
