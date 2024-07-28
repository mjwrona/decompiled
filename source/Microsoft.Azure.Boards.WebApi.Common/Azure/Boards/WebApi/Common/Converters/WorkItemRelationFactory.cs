// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemRelationFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WorkItemRelationFactory
  {
    private static int TracePointStart = 5950000;

    public static (int targetId, Guid? remoteHostId) GetTarget(this WorkItemRelation relation)
    {
      (int, Guid?) target;
      if (relation.TryGetTarget(out target))
        return target;
      throw new VssPropertyValidationException("url", ResourceStrings.InvalidRelationUrl());
    }

    public static bool TryGetTarget(
      this WorkItemRelation relation,
      out (int targetId, Guid? remoteHostId) target)
    {
      Uri result1;
      int result2;
      if (relation.Url != null && Uri.TryCreate(relation.Url, UriKind.Absolute, out result1) && int.TryParse(((IEnumerable<string>) result1.Segments).Last<string>(), out result2))
      {
        Guid guid;
        target = relation.Attributes == null || !relation.Attributes.TryGetGuid("remoteHostId", out guid) ? (result2, new Guid?()) : (result2, new Guid?(guid));
        return true;
      }
      target = ();
      return false;
    }

    public static WorkItemRelation Create(
      IVssRequestContext requestContext,
      WorkItemLinkInfo linkInfo,
      ISecuredObject workItem,
      Guid? projectId = null,
      bool returnProjectScopedUrl = true,
      bool excludeRemoteLinkProperties = false)
    {
      WorkItemTrackingRequestContext witContext = requestContext.WitContext();
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "isLocked",
          (object) linkInfo.IsLocked
        }
      };
      if (!string.IsNullOrEmpty(linkInfo.Comment))
        dictionary.Add("comment", (object) linkInfo.Comment);
      if (linkInfo.RemoteStatus.HasValue)
        dictionary.Add("remoteStatus", (object) linkInfo.RemoteStatus);
      if (!string.IsNullOrEmpty(linkInfo.RemoteStatusMessage))
        dictionary.Add("remoteStatusMessage", (object) WorkItemRelationFactory.GetRemoteStatusMessage(requestContext, linkInfo.RemoteStatusMessage));
      if (linkInfo.RemoteWatermark.HasValue && !excludeRemoteLinkProperties)
        dictionary.Add("remoteWatermark", (object) linkInfo.RemoteWatermark);
      string workItemLinkName = WorkItemRelationFactory.GetWorkItemLinkName(linkInfo.LinkType, witContext);
      if (workItemLinkName != null)
        dictionary.Add("name", (object) workItemLinkName);
      if (linkInfo.RemoteHostId.HasValue)
        dictionary.Add("remoteHostId", (object) linkInfo.RemoteHostId);
      WorkItemRelation workItemRelation = new WorkItemRelation(workItem);
      workItemRelation.Rel = WorkItemRelationFactory.GetWorkItemLinkType(linkInfo.LinkType, witContext);
      workItemRelation.Url = WitUrlHelper.GetWorkItemUrl(requestContext, linkInfo.TargetId, project: projectId, generateProjectScopedUrl: returnProjectScopedUrl, remoteHostId: linkInfo.RemoteHostId, remoteProjectId: linkInfo.RemoteProjectId);
      workItemRelation.Attributes = (IDictionary<string, object>) dictionary;
      return workItemRelation;
    }

    public static WorkItemRelation Create(
      IVssRequestContext requestContext,
      WorkItemResourceLinkInfo linkInfo,
      ISecuredObject workItem,
      Guid? projectId = null,
      bool returnProjectScopedUrl = true)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "authorizedDate",
          (object) linkInfo.AuthorizedDate
        },
        {
          "id",
          (object) linkInfo.ResourceId
        },
        {
          "resourceCreatedDate",
          (object) linkInfo.ResourceCreatedDate
        },
        {
          "resourceModifiedDate",
          (object) linkInfo.ResourceModifiedDate
        },
        {
          "revisedDate",
          (object) linkInfo.RevisedDate
        }
      };
      if (linkInfo.ResourceSize > 0)
        dictionary.Add("resourceSize", (object) linkInfo.ResourceSize);
      if (!string.IsNullOrEmpty(linkInfo.Comment))
        dictionary.Add("comment", (object) linkInfo.Comment);
      if (!string.IsNullOrEmpty(linkInfo.Name))
        dictionary.Add("name", (object) linkInfo.Name);
      WorkItemRelation workItemRelation = new WorkItemRelation(workItem);
      workItemRelation.Rel = WorkItemRelationFactory.GetRelationLinkType(linkInfo);
      workItemRelation.Url = WitUrlHelper.GetResourceLinkLocation(requestContext, linkInfo, projectId, returnProjectScopedUrl);
      workItemRelation.Attributes = (IDictionary<string, object>) dictionary;
      return workItemRelation;
    }

    public static WorkItemLinkUpdate ToWorkItemLinkUpdate(
      this WorkItemRelation relation,
      IVssRequestContext requestContext,
      LinkUpdateType updateType,
      int source,
      WorkItemLinkTypeEnd linkTypeEnd)
    {
      string str = (string) null;
      flag = false;
      Guid? nullable = new Guid?();
      try
      {
        if (linkTypeEnd.LinkType.IsRemote)
        {
          requestContext.TraceEnter(WorkItemRelationFactory.TracePointStart + 1, "Rest", nameof (WorkItemRelationFactory), nameof (ToWorkItemLinkUpdate));
          if (!Uri.IsWellFormedUriString(relation.Url, UriKind.Absolute))
            throw new ArgumentException(ResourceStrings.WorkItemUrlNotWellFormed((object) relation.Url));
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          nullable = new Guid?((vssRequestContext.GetService<IUrlHostResolutionService>().ResolveHost(vssRequestContext, new Uri(relation.Url)) ?? throw new ArgumentException(ResourceStrings.EnterpriseWithUrlNotFound((object) relation.Url))).HostId);
          requestContext.TraceLeave(WorkItemRelationFactory.TracePointStart + 2, "Rest", nameof (WorkItemRelationFactory), nameof (ToWorkItemLinkUpdate));
        }
        if (relation.Attributes != null)
        {
          str = (string) relation.Attributes.GetValueOrDefault<string, object>("comment");
          if (!(relation.Attributes.GetValueOrDefault<string, object>("isLocked", (object) false) is bool flag))
            throw new ArgumentException(ResourceStrings.InvalidParameterType((object) "isLocked", (object) "Boolean"));
        }
      }
      catch (InvalidCastException ex)
      {
        requestContext.Trace(WorkItemRelationFactory.TracePointStart + 3, TraceLevel.Error, "Rest", nameof (WorkItemRelationFactory), string.Format("InvalidCastException thrown while creating a WorkItemLinkUpdate from WorkItemRelation: comment : {0}; isLocked : {1}; Exception {2}", (object) str, (object) flag, (object) ex));
        throw;
      }
      WorkItemLinkUpdate workItemLinkUpdate = new WorkItemLinkUpdate();
      workItemLinkUpdate.UpdateType = updateType;
      workItemLinkUpdate.LinkType = linkTypeEnd.Id;
      workItemLinkUpdate.Locked = new bool?(flag);
      workItemLinkUpdate.Comment = str;
      workItemLinkUpdate.SourceWorkItemId = source;
      workItemLinkUpdate.TargetWorkItemId = relation.GetTarget().targetId;
      workItemLinkUpdate.RemoteUrl = relation.Url;
      workItemLinkUpdate.RemoteHostId = nullable;
      return workItemLinkUpdate;
    }

    public static WorkItemResourceLinkUpdate ToWorkItemResourceLinkUpdate(
      this WorkItemRelation relation,
      ResourceLinkType linkType,
      LinkUpdateType updateType,
      int? resourceId)
    {
      if (updateType == LinkUpdateType.Delete)
      {
        if (!resourceId.HasValue || resourceId.Value <= 0)
          throw new VssPropertyValidationException(nameof (resourceId), ResourceStrings.InvalidOrMissingResourceId());
        WorkItemResourceLinkUpdate resourceLinkUpdate = new WorkItemResourceLinkUpdate();
        resourceLinkUpdate.ResourceId = resourceId;
        resourceLinkUpdate.UpdateType = updateType;
        return resourceLinkUpdate;
      }
      string url = relation.Url;
      int? nullable = new int?();
      string str1 = (string) null;
      string str2 = (string) null;
      if (relation.Attributes != null)
      {
        try
        {
          nullable = new int?(Convert.ToInt32(relation.Attributes.GetValueOrDefault<string, object>("resourceSize", (object) 0)));
        }
        catch
        {
          throw new VssPropertyValidationException("resourceSize", ResourceStrings.InvalidRelationResourceSize());
        }
        try
        {
          str1 = (string) relation.Attributes.GetValueOrDefault<string, object>("comment");
        }
        catch
        {
          throw new VssPropertyValidationException("comment", ResourceStrings.InvalidRelationComment());
        }
        try
        {
          str2 = (string) relation.Attributes.GetValueOrDefault<string, object>("name");
        }
        catch
        {
          throw new VssPropertyValidationException("name", ResourceStrings.InvalidRelationName());
        }
      }
      if (linkType == ResourceLinkType.Attachment)
      {
        Uri result1;
        Guid result2;
        if (!Uri.TryCreate(relation.Url, UriKind.Absolute, out result1) || !Guid.TryParse(((IEnumerable<string>) result1.Segments).Last<string>(), out result2))
          throw new VssPropertyValidationException("url", ResourceStrings.InvalidRelationUrl());
        url = result2.ToString();
        if (string.IsNullOrEmpty(str2))
          str2 = HttpUtility.ParseQueryString(result1.Query)["fileName"];
      }
      WorkItemResourceLinkUpdate resourceLinkUpdate1 = new WorkItemResourceLinkUpdate();
      resourceLinkUpdate1.UpdateType = updateType;
      resourceLinkUpdate1.ResourceId = resourceId;
      resourceLinkUpdate1.Name = str2;
      resourceLinkUpdate1.Location = url;
      resourceLinkUpdate1.Type = new ResourceLinkType?(linkType);
      resourceLinkUpdate1.Comment = str1;
      resourceLinkUpdate1.Length = nullable;
      return resourceLinkUpdate1;
    }

    public static string GetWorkItemLinkType(int linkId, WorkItemTrackingRequestContext witContext)
    {
      WorkItemLinkTypeEnd linkType;
      return witContext.LinkService.TryGetLinkTypeEndById(witContext.RequestContext, linkId, out linkType) ? WorkItemRelationTypeFactory.GetRelationReferenceName(linkType) : (string) null;
    }

    public static string GetWorkItemLinkName(int linkId, WorkItemTrackingRequestContext witContext)
    {
      WorkItemLinkTypeEnd linkType;
      return witContext.LinkService.TryGetLinkTypeEndById(witContext.RequestContext, linkId, out linkType) ? linkType.Name : (string) null;
    }

    public static string GetRelationLinkType(WorkItemResourceLinkInfo linkInfo)
    {
      switch (linkInfo.ResourceType)
      {
        case ResourceLinkType.Attachment:
          return "AttachedFile";
        case ResourceLinkType.Hyperlink:
          return "Hyperlink";
        case ResourceLinkType.ArtifactLink:
          return "ArtifactLink";
        default:
          return (string) null;
      }
    }

    public static string GetRemoteStatusMessage(
      IVssRequestContext requestContext,
      string remoteStatusMessage)
    {
      try
      {
        return JsonConvert.DeserializeObject<RemoteStatusMessage>(remoteStatusMessage).StatusMessage;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1300576, nameof (WorkItemRelationFactory), nameof (GetRemoteStatusMessage), ex);
        return ResourceStrings.ErrorContactingRemoteServer();
      }
    }
  }
}
