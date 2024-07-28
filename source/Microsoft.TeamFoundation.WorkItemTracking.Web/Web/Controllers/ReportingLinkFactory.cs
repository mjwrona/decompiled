// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ReportingLinkFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  internal static class ReportingLinkFactory
  {
    internal static WorkItemRelation CreateReportingLink(
      IVssRequestContext requestContext,
      WorkItemLinkChange linkChange,
      IDictionary<Guid, IdentityReference> identityMap = null,
      bool includeRemoteUrl = false)
    {
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      WorkItemRelation workItemRelation = new WorkItemRelation();
      workItemRelation.Rel = linkChange.LinkType;
      workItemRelation.Attributes = (IDictionary<string, object>) new Dictionary<string, object>()
      {
        ["linkType"] = (object) linkChange.LinkTypeString,
        ["sourceId"] = (object) linkChange.SourceID,
        ["targetId"] = (object) linkChange.TargetID,
        ["isActive"] = (object) linkChange.IsActive,
        ["changedDate"] = (object) linkChange.ChangedDate,
        ["changedBy"] = (linkChange.ChangedBy_Name != null ? (object) ReportingLinkFactory.GetIdentityRef(requestContext, linkChange.ChangedBy_TfId, linkChange.ChangedBy_Name, identityMap) : (object) (IdentityRef) null),
        ["comment"] = (string.IsNullOrEmpty(linkChange.Comment) ? (object) (string) null : (object) linkChange.Comment),
        ["changedOperation"] = (object) (LinkChangeType) (linkChange.IsActive ? 0 : 1),
        ["sourceProjectId"] = (object) linkChange.SourceProjectId,
        ["targetProjectId"] = (object) linkChange.TargetProjectId
      };
      WorkItemRelation reportingLink = workItemRelation;
      if (ReportingLinkFactory.IsRemoteLinkTypes(requestContext, linkChange.LinkTypeId))
        reportingLink.Attributes.Add("remoteUrl", (object) ReportingLinkFactory.GetRemoteWorkItemsUrl(witRequestContext, linkChange));
      return reportingLink;
    }

    public static bool IsRemoteLinkTypes(IVssRequestContext requestContext, int linkTypeId)
    {
      WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
      return CommonWITUtils.IsRemoteLinkingEnabled(requestContext) && trackingRequestContext.LinkService.GetLinkTypeById(trackingRequestContext.RequestContext, linkTypeId).IsRemote;
    }

    private static string GetRemoteWorkItemsUrl(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemLinkChange linkChange)
    {
      int num = linkChange.SourceDataspaceId == 0 ? linkChange.SourceID : linkChange.TargetID;
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      int workItemId = num;
      Guid? remoteHostId1 = linkChange.RemoteHostId;
      Guid? remoteProjectId1 = linkChange.RemoteProjectId;
      Guid? project = new Guid?();
      Guid? remoteHostId2 = remoteHostId1;
      Guid? remoteProjectId2 = remoteProjectId1;
      return WitUrlHelper.GetWorkItemUrl(requestContext, workItemId, project: project, remoteHostId: remoteHostId2, remoteProjectId: remoteProjectId2);
    }

    private static IdentityRef GetIdentityRef(
      IVssRequestContext requestContext,
      Guid vsid,
      string witName,
      IDictionary<Guid, IdentityReference> identityMap)
    {
      IdentityReference identityRef;
      if (identityMap != null && identityMap.TryGetValue(vsid, out identityRef))
        return (IdentityRef) identityRef;
      return new IdentityRef()
      {
        Id = vsid.ToString(),
        UniqueName = witName
      };
    }
  }
}
