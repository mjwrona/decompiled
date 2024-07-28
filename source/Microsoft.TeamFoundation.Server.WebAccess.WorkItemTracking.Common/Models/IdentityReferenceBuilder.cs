// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.IdentityReferenceBuilder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  public static class IdentityReferenceBuilder
  {
    private static Version MinimumVersionForNewIdentityRef = new Version(4, 0);

    public static bool ShouldUseProperIdentityRef(IVssRequestContext requestContext)
    {
      bool flag = true;
      ApiResourceVersion apiResourceVersion = (ApiResourceVersion) null;
      if (requestContext.TryGetItem<ApiResourceVersion>("WitApiResourceVersion", out apiResourceVersion))
        flag = apiResourceVersion.ApiVersion >= IdentityReferenceBuilder.MinimumVersionForNewIdentityRef;
      return flag;
    }

    internal static IdentityReference Create(
      IVssRequestContext requestContext,
      Guid vsid,
      bool includeUrls = false,
      bool includeInactive = false)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        vsid
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return IdentityReferenceBuilder.Create(requestContext, identity, includeUrls, includeInactive);
    }

    public static IDictionary<Guid, IdentityReference> Create(
      IVssRequestContext requestContext,
      IEnumerable<Guid> vsids,
      bool includeUrls = false,
      bool includeInactive = false)
    {
      List<Guid> list = vsids.Where<Guid>((Func<Guid, bool>) (x => x != Guid.Empty)).Distinct<Guid>().ToList<Guid>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) list, QueryMembership.None, (IEnumerable<string>) null);
      Dictionary<Guid, IdentityReference> dictionary = new Dictionary<Guid, IdentityReference>();
      for (int index = 0; index < list.Count; ++index)
      {
        if (identityList[index] != null)
          dictionary.Add(list[index], IdentityReferenceBuilder.Create(requestContext, identityList[index], includeUrls, includeInactive));
      }
      return (IDictionary<Guid, IdentityReference>) dictionary;
    }

    internal static IdentityReference Create(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool includeUrls = false,
      bool includeInactive = false)
    {
      return identity == null ? (IdentityReference) null : new IdentityReference(identity.ToIdentityRef(requestContext, includeUrls, includeInactive));
    }

    public static IdentityReference CreateFromWitIdentityName(
      WorkItemTrackingRequestContext witRequestContext,
      string identityName,
      bool includeUrls = false,
      bool includeInactive = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(identityName, nameof (identityName));
      return IdentityReferenceBuilder.CreateFromWitIdentityNames(witRequestContext, Enumerable.Repeat<string>(identityName, 1), includeUrls, includeInactive).Values.First<IdentityReference>();
    }

    public static IDictionary<string, IdentityReference> CreateFromWitIdentityNames(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<string> identityNames,
      bool includeUrls = false,
      bool includeInactive = false)
    {
      IDictionary<string, IdentityRef> witIdentityNames1 = IdentityReferenceBuilder.CreateIdentityRefFromWitIdentityNames(witRequestContext, identityNames, includeUrls, includeInactive);
      Dictionary<string, IdentityReference> witIdentityNames2 = new Dictionary<string, IdentityReference>();
      foreach (KeyValuePair<string, IdentityRef> keyValuePair in (IEnumerable<KeyValuePair<string, IdentityRef>>) witIdentityNames1)
        witIdentityNames2.Add(keyValuePair.Key, new IdentityReference(keyValuePair.Value, keyValuePair.Key));
      return (IDictionary<string, IdentityReference>) witIdentityNames2;
    }

    public static IDictionary<string, IdentityRef> CreateIdentityRefFromWitIdentityNames(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<string> identityNames,
      bool includeUrls = false,
      bool includeInactive = false,
      ISecuredObject secureInfoSourceObjectForConstantIdentity = null)
    {
      List<string> list1 = identityNames.Where<string>((Func<string, bool>) (name => !string.IsNullOrWhiteSpace(name))).Distinct<string>().ToList<string>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      List<string> list2 = list1.Where<string>(IdentityReferenceBuilder.\u003C\u003EO.\u003C0\u003E__IsDistinctNameIdentity ?? (IdentityReferenceBuilder.\u003C\u003EO.\u003C0\u003E__IsDistinctNameIdentity = new Func<string, bool>(IdentityUtilities.IsDistinctNameIdentity))).ToList<string>();
      List<string> list3 = list1.Except<string>((IEnumerable<string>) list2, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      Dictionary<string, IdentityRef> witIdentityNames = new Dictionary<string, IdentityRef>();
      List<string> stringList = new List<string>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> identities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (IdentityReferenceBuilder.ShouldUseProperIdentityRef(requestContext))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        foreach (string factorValue in list2)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(requestContext, IdentitySearchFilter.DisplayName, factorValue, QueryMembership.None, (IEnumerable<string>) null);
          if (identityList != null && identityList.Count == 1)
          {
            stringList.Add(factorValue);
            identities.Add(identityList[0]);
          }
          else
          {
            requestContext.Trace(5923000, TraceLevel.Warning, "WitIdentityReference", "CreateFromWitIdentityNames", "Could not resolve " + factorValue + " to an identity");
            list3.Add(factorValue);
          }
        }
        IdentityRef[] identityRefs = identities.ToIdentityRefs(requestContext, includeUrls);
        for (int index = 0; index < stringList.Count; ++index)
          witIdentityNames[stringList[index]] = identityRefs[index];
        foreach (string str in list3)
        {
          Dictionary<string, IdentityRef> dictionary = witIdentityNames;
          string key = str;
          ConstantIdentityRef constantIdentityRef = new ConstantIdentityRef(secureInfoSourceObjectForConstantIdentity);
          constantIdentityRef.DisplayName = str;
          dictionary[key] = (IdentityRef) constantIdentityRef;
        }
      }
      else
      {
        IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord> constantRecords = requestContext.GetService<TeamFoundationWorkItemTrackingMetadataService>().GetConstantRecords(requestContext, (IEnumerable<string>) list1, true, new WitReadReplicaContext?(), true);
        Dictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord> dictionary = new Dictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord>();
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord constantRecord1 in constantRecords)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord constantRecord2;
          if (constantRecord1 != null && !string.IsNullOrWhiteSpace(constantRecord1.DisplayText) && (!dictionary.TryGetValue(constantRecord1.DisplayText, out constantRecord2) || constantRecord1.TeamFoundationId != Guid.Empty && constantRecord2.TeamFoundationId == Guid.Empty))
            dictionary[constantRecord1.DisplayText] = constantRecord1;
        }
        foreach (string key in list1)
        {
          IdentityRef identityRef = new IdentityRef();
          Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord constantRecord;
          if (dictionary.TryGetValue(key, out constantRecord))
          {
            identityRef.Id = constantRecord.TeamFoundationId.ToString();
            identityRef.Url = IdentityHelper.GetIdentityResourceUriString(witRequestContext.RequestContext, constantRecord.TeamFoundationId);
          }
          witIdentityNames[key] = identityRef;
        }
      }
      return (IDictionary<string, IdentityRef>) witIdentityNames;
    }
  }
}
