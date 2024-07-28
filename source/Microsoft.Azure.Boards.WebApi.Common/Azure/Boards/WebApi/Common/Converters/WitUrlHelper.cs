// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WitUrlHelper
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.RemoteWorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WitUrlHelper
  {
    private static int TracePointStart = 5951000;

    public static string GetFieldsUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid? project,
      bool generateProjectScopedUrl = true)
    {
      return WitUrlHelper.GetFieldUrl(witRequestContext.RequestContext, (string) null, project, generateProjectScopedUrl);
    }

    public static string GetWorkItemUrl(
      IVssRequestContext requestContext,
      int workItemId,
      bool isDeleted = false,
      Guid? project = null,
      bool generateProjectScopedUrl = true,
      Guid? remoteHostId = null,
      Guid? remoteProjectId = null)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        if (remoteHostId.HasValue)
        {
          requestContext.TraceEnter(WitUrlHelper.TracePointStart + 1, "Rest", nameof (WitUrlHelper), nameof (GetWorkItemUrl));
          remoteProjectId = WitUrlHelper.SanitizeProjectIdForResponseUrl(remoteProjectId, generateProjectScopedUrl);
          Uri relativeResourceUri = service.GetLocationData(requestContext, ServiceInstanceTypes.TFS).GetRelativeResourceUri(requestContext, "wit", isDeleted ? WorkItemTrackingLocationIds.RecycleBin : WorkItemTrackingLocationIds.WorkItems, (object) new
          {
            id = workItemId,
            project = remoteProjectId
          });
          string remoteHostUrl = RemoteWorkItemHostResolver.GetRemoteHostUrl(requestContext, remoteHostId.Value);
          requestContext.TraceLeave(WitUrlHelper.TracePointStart + 2, "Rest", nameof (WitUrlHelper), nameof (GetWorkItemUrl));
          if (!string.IsNullOrEmpty(remoteHostUrl))
            return remoteHostUrl + relativeResourceUri?.ToString();
          requestContext.Trace(WitUrlHelper.TracePointStart + 3, TraceLevel.Error, "Rest", nameof (WitUrlHelper), string.Format("Failed to resolve uri for host id: {0}", (object) remoteHostId.Value));
          return (string) null;
        }
        project = WitUrlHelper.SanitizeProjectIdForResponseUrl(project, generateProjectScopedUrl);
        return service.GetResourceUri(requestContext, "wit", isDeleted ? WorkItemTrackingLocationIds.RecycleBin : WorkItemTrackingLocationIds.WorkItems, (object) new Dictionary<string, object>()
        {
          {
            "id",
            (object) workItemId
          },
          {
            nameof (project),
            (object) project
          }
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetFieldUrl(
      IVssRequestContext requestContext,
      string fieldReferenceName,
      Guid? project = null,
      bool generateProjectScopedUrl = true)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        project = WitUrlHelper.SanitizeProjectIdForResponseUrl(project, generateProjectScopedUrl);
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.Fields, (object) new
        {
          fieldNameOrRefName = fieldReferenceName,
          project = project
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.FieldUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWorkItemTypeUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType)
    {
      return WitUrlHelper.GetWorkItemTypeUrl(witRequestContext, workItemType.ProjectId, workItemType.Name);
    }

    public static string GetWorkItemTypeUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid project,
      string workItemType)
    {
      return WitUrlHelper.GetWorkItemTypeUrl(witRequestContext.RequestContext, project, workItemType);
    }

    public static string GetWorkItemTypeUrl(
      IVssRequestContext requestContext,
      Guid project,
      string workItemType)
    {
      object routeValues = workItemType != null ? (object) new
      {
        type = workItemType
      } : (object) new{  };
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.WorkItemTypes, project, routeValues).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, ResourceStrings.LocationServiceException(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWorkItemTypeFieldUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid project,
      string workItemType,
      string fieldName)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      object routeValues = workItemType != null ? (object) new
      {
        type = workItemType,
        fields = "fields",
        field = fieldName
      } : (object) new{  };
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.WorkItemTypeField, project, routeValues).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, ResourceStrings.LocationServiceException(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWorkItemTemplateUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid projectId,
      string workItemTypeReferenceName)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.WorkItems, projectId, (object) new
      {
        type = workItemTypeReferenceName
      }).AbsoluteUri;
    }

    public static string GetWorkItemTypeCategoryUrl(
      IVssRequestContext requestContext,
      Guid project,
      string workItemTypeCategory)
    {
      object routeValues = workItemTypeCategory != null ? (object) new
      {
        category = workItemTypeCategory
      } : (object) new{  };
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.WorkItemTypeCategories, project, routeValues).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException("TODO: add to resourcestrings.resx", ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWorkItemHistoryUrl(
      WorkItemTrackingRequestContext witRequestContext,
      int workItemId,
      int? revisionNumber = null,
      Guid? project = null,
      bool generateProjectScopedUrl = true)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        project = WitUrlHelper.SanitizeProjectIdForResponseUrl(project, generateProjectScopedUrl);
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.History, (object) new
        {
          id = workItemId,
          revisionNumber = revisionNumber,
          project = project
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWorkItemTypeIconUrl(
      IVssRequestContext requestContext,
      string icon,
      string color = null)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ILocationService service = vssRequestContext.GetService<ILocationService>();
      try
      {
        Uri resourceUri = service.GetResourceUri(vssRequestContext, "wit", WorkItemTrackingLocationIds.WorkItemIcons, (object) new
        {
          icon = icon
        });
        string str;
        if (!string.IsNullOrWhiteSpace(color))
          str = string.Format("?{0}={1}&{2}={3}", (object) nameof (color), color.Length > 6 ? (object) color.Substring(color.Length - 6) : (object) color, (object) "v", (object) 2);
        else
          str = string.Format("?{0}={1}", (object) "v", (object) 2);
        return resourceUri.AbsoluteUri + str;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemIconUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWorkItemCommentUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid? projectId,
      int workItemId,
      int? revision,
      bool generateProjectScopedUrl = true)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        projectId = WitUrlHelper.SanitizeProjectIdForResponseUrl(projectId, generateProjectScopedUrl);
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.Comments, (object) new
        {
          id = workItemId,
          revision = revision,
          project = projectId
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemCommentUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWorkItemCommentResponseUrl(
      IVssRequestContext requestContext,
      Guid project,
      int workItemId,
      int commentId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.Comments2, (object) new
        {
          workItemId = workItemId,
          commentId = commentId,
          project = project
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemCommentResponseUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWorkItemCommentVersionUrl(
      IVssRequestContext requestContext,
      Guid project,
      int workItemId,
      int commentId,
      int version)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.CommentVersions, (object) new
        {
          workItemId = workItemId,
          commentId = commentId,
          version = version,
          project = project
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemCommentVersionUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWorkItemEditorUrl(IVssRequestContext requestContext, int workItemId) => requestContext.GetService<ITswaServerHyperlinkService>().GetWorkItemEditorUrl(requestContext, workItemId).ToString();

    public static string GetWorkItemEditorUrl(
      IVssRequestContext requestContext,
      Uri projectUri,
      int workItemId)
    {
      return requestContext.GetService<ITswaServerHyperlinkService>().GetWorkItemEditorUrl(requestContext, projectUri, workItemId).ToString();
    }

    public static string GetWorkItemEditorUrl(
      IVssRequestContext requestContext,
      Guid projectGuid,
      int workItemId)
    {
      return requestContext.GetService<ITswaServerHyperlinkService>().GetWorkItemEditorUrl(requestContext, projectGuid, workItemId).ToString();
    }

    public static string GetWorkItemUpdatesUrl(
      WorkItemTrackingRequestContext witRequestContext,
      int workItemId,
      ApiResourceVersion apiResourceVersion,
      long? updateNumber = null,
      Guid? project = null,
      bool generateProjectScopedUrl = true,
      bool asFirstParameter = true,
      string source = "")
    {
      return WitUrlHelper.GetWorkItemUpdatesUrl(witRequestContext, workItemId, updateNumber, project, generateProjectScopedUrl, apiResourceVersion);
    }

    public static string GetWorkItemUpdatesUrl(
      WorkItemTrackingRequestContext witRequestContext,
      int workItemId,
      long? updateNumber = null,
      Guid? project = null,
      bool generateProjectScopedUrl = true,
      ApiResourceVersion apiResourceVersion = null,
      bool asFirstParameter = true,
      string source = "")
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        project = WitUrlHelper.SanitizeProjectIdForResponseUrl(project, generateProjectScopedUrl);
        Uri resourceUri = service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.Updates, (object) new
        {
          id = workItemId,
          updateNumber = updateNumber,
          project = project
        });
        string stringAsParameter = apiResourceVersion != null ? WitUrlHelper.GetApiVersionStringAsParameter(apiResourceVersion, asFirstParameter) : (string) null;
        return string.IsNullOrEmpty(stringAsParameter) || !updateNumber.HasValue ? resourceUri.AbsoluteUri : resourceUri.AbsoluteUri + (!string.IsNullOrEmpty(source) ? "?source=" + source : "") + stringAsParameter;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemUpdatesUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWorkItemRevisionUrl(
      WorkItemTrackingRequestContext witRequestContext,
      int workItemId,
      int? revision = null,
      Guid? project = null,
      bool generateProjectScopedUrl = true)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        project = WitUrlHelper.SanitizeProjectIdForResponseUrl(project, generateProjectScopedUrl);
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.Revisions, (object) new
        {
          id = workItemId,
          revisionNumber = revision,
          project = project
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemRevisionUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetReportingRevisionsUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid projectId,
      string continuationToken,
      bool useContinuationToken,
      IEnumerable<string> fields,
      IEnumerable<string> types,
      bool? includeIdentityRef,
      bool? includeDeleted,
      bool? includeTagRef,
      bool? includeLatestOnly,
      bool? includeDiscussionChangesOnly,
      bool? expandFields,
      int? batchSize,
      ApiResourceVersion apiVersion)
    {
      IVssRequestContext requestContext1 = witRequestContext.RequestContext;
      ILocationService service = requestContext1.GetService<ILocationService>();
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      IVssRequestContext requestContext2 = requestContext1;
      Guid workItemRevisions = WorkItemTrackingLocationIds.ReportingWorkItemRevisions;
      Guid projectId1 = projectId;
      Dictionary<string, object> routeValues = dictionary;
      Guid serviceOwner = new Guid();
      Uri resourceUri = service.GetResourceUri(requestContext2, "wit", workItemRevisions, projectId1, (object) routeValues, serviceOwner);
      string str1 = !useContinuationToken ? "?watermark=" + continuationToken : "?continuationToken=" + continuationToken;
      if (fields != null && fields.Any<string>())
        str1 = str1 + "&fields=" + Uri.EscapeDataString(string.Join(",", fields));
      if (types != null && types.Any<string>())
        str1 = str1 + "&types=" + Uri.EscapeDataString(string.Join(",", types));
      if (includeIdentityRef.HasValue)
      {
        string str2 = includeIdentityRef.Value ? "true" : "false";
        str1 = str1 + "&includeIdentityRef=" + str2;
      }
      if (includeDeleted.HasValue)
      {
        string str3 = includeDeleted.Value ? "true" : "false";
        str1 = str1 + "&includeDeleted=" + str3;
      }
      if (includeTagRef.HasValue)
      {
        string str4 = includeTagRef.Value ? "true" : "false";
        str1 = str1 + "&includeTagRef=" + str4;
      }
      if (includeLatestOnly.HasValue)
      {
        string str5 = includeLatestOnly.Value ? "true" : "false";
        str1 = str1 + "&includeLatestOnly=" + str5;
      }
      if (includeDiscussionChangesOnly.HasValue)
      {
        string str6 = includeDiscussionChangesOnly.Value ? "true" : "false";
        str1 = str1 + "&includeDiscussionChangesOnly=" + str6;
      }
      if (expandFields.GetValueOrDefault())
      {
        string str7 = "Fields";
        str1 = str1 + "&$expand=" + str7;
      }
      if (batchSize.HasValue)
      {
        string str8 = batchSize.Value.ToString();
        str1 = str1 + "&$maxPageSize=" + str8;
      }
      string str9 = str1 + WitUrlHelper.GetApiVersionStringAsParameter(apiVersion);
      return resourceUri.AbsoluteUri + str9;
    }

    public static string GetReportingLinksUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid projectId,
      string continuationToken,
      string types,
      string linkTypes,
      bool useContinuationToken,
      ApiResourceVersion apiVersion)
    {
      IVssRequestContext requestContext1 = witRequestContext.RequestContext;
      ILocationService service = requestContext1.GetService<ILocationService>();
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      IVssRequestContext requestContext2 = requestContext1;
      Guid reportingWorkItemLinks = WorkItemTrackingLocationIds.ReportingWorkItemLinks;
      Guid projectId1 = projectId;
      Dictionary<string, object> routeValues = dictionary;
      Guid serviceOwner = new Guid();
      Uri resourceUri = service.GetResourceUri(requestContext2, "wit", reportingWorkItemLinks, projectId1, (object) routeValues, serviceOwner);
      string str1 = !useContinuationToken ? "?watermark=" + continuationToken : "?continuationToken=" + continuationToken;
      if (types != null && types.Any<char>())
        str1 = str1 + "&types=" + Uri.EscapeDataString(string.Join(",", new string[1]
        {
          types
        }));
      if (linkTypes != null && linkTypes.Any<char>())
        str1 = str1 + "&linkTypes=" + Uri.EscapeDataString(string.Join(",", new string[1]
        {
          linkTypes
        }));
      string str2 = str1 + WitUrlHelper.GetApiVersionStringAsParameter(apiVersion);
      return resourceUri.AbsoluteUri + str2;
    }

    public static string GetWorkItemRelationTypeUrl(
      WorkItemTrackingRequestContext witRequestContext,
      string relationReferenceName)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.RelationTypes, (object) new
        {
          relation = relationReferenceName
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemRevisionUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetResourceLinkLocation(
      IVssRequestContext requestContext,
      WorkItemResourceLinkInfo linkInfo,
      Guid? projectId,
      bool generateProjectScopedUrl = true)
    {
      return WitUrlHelper.GetResourceLinkLocation(requestContext, linkInfo.ResourceType, linkInfo.Location, linkInfo.Name, projectId, generateProjectScopedUrl);
    }

    public static string GetResourceLinkLocation(
      IVssRequestContext requestContext,
      ResourceLinkType linkType,
      string id,
      string fileName,
      Guid? projectId,
      bool generateProjectScopedUrl = true)
    {
      if (linkType != ResourceLinkType.Attachment)
        return id;
      ILocationService service = requestContext.GetService<ILocationService>();
      projectId = WitUrlHelper.SanitizeProjectIdForResponseUrl(projectId, generateProjectScopedUrl);
      try
      {
        Uri resourceUri;
        if (projectId.HasValue)
        {
          Guid? nullable = projectId;
          Guid empty = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          {
            resourceUri = service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.Attachments, projectId.Value, (object) new
            {
              id = id,
              fileName = fileName
            });
            goto label_7;
          }
        }
        resourceUri = service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.Attachments, (object) new
        {
          id = id,
          fileName = fileName
        });
label_7:
        return resourceUri.AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemAttachmentUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetQueryUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid projectId,
      Guid queryId)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.QueriesByProjectAndQueryReference, projectId, (object) new
        {
          query = queryId
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetWiqlUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid projectId,
      Guid queryId)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.WiqlWithId, projectId, (object) new
        {
          id = queryId
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetAttachmentUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid? projectId,
      Guid attachmentId,
      string fileName,
      bool generateProjectScopedUrl = true)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        projectId = WitUrlHelper.SanitizeProjectIdForResponseUrl(projectId, generateProjectScopedUrl);
        UriBuilder uriBuilder = new UriBuilder(service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.Attachments, (object) new
        {
          id = attachmentId,
          project = projectId
        }));
        string str = "fileName=" + fileName;
        if (!string.IsNullOrEmpty(fileName))
          uriBuilder.Query = !string.IsNullOrEmpty(uriBuilder.Query) ? uriBuilder.Query.Substring(1) + "&" + str : str;
        return uriBuilder.Uri.AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static string GetClassificationNodeUrl(
      IVssRequestContext requestContext,
      TreeNode treeNode)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      string str = treeNode.RelativePath.TrimStart('\\');
      Guid cssNodeId = treeNode.Project.CssNodeId;
      try
      {
        return service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.ClassificationNodes, cssNodeId, (object) new
        {
          path = str,
          structureGroup = WitUrlHelper.InternalToTreeStructureGroup(treeNode.Type)
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.ClassificationNodeUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static TreeStructureGroup InternalToTreeStructureGroup(TreeStructureType treeNodeType)
    {
      if (treeNodeType == TreeStructureType.Area)
        return TreeStructureGroup.Areas;
      if (treeNodeType == TreeStructureType.Iteration)
        return TreeStructureGroup.Iterations;
      throw new NotImplementedException();
    }

    public static string GetWorkItemsHubUrl(
      IVssRequestContext requestContext,
      string projectName,
      string teamName,
      string pivotName)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "WorkItemsHub", FrameworkServiceIdentifiers.WorkItemsHub, (object) new
        {
          teamName = teamName,
          projectName = projectName,
          pivotName = pivotName
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.WorkItemsHubUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    public static Uri GetWorkItemCommentNextPageUrl(
      WorkItemTrackingRequestContext witRequestContext,
      Guid projectId,
      int workItemId,
      string continuationToken,
      int? top = null,
      bool? includeDeleted = null,
      CommentExpandOptions expandOptions = CommentExpandOptions.None,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentSortOrder? order = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentSortOrder.Desc)
    {
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      ILocationService service = requestContext.GetService<ILocationService>();
      UriBuilder uriBuilder = new UriBuilder(service.FindServiceDefinition(requestContext, "wit", WorkItemTrackingLocationIds.Comments2) == null ? service.GetResourceUri(requestContext, "witcomments", WorkItemTrackingLocationIds.Comments2, (object) new
      {
        workItemId = workItemId,
        project = projectId
      }) : service.GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.Comments2, (object) new
      {
        workItemId = workItemId,
        project = projectId
      }));
      NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
      queryString[nameof (continuationToken)] = continuationToken;
      if (includeDeleted.HasValue)
        queryString[nameof (includeDeleted)] = includeDeleted.Value.ToString();
      int num;
      if (top.HasValue)
      {
        NameValueCollection nameValueCollection = queryString;
        num = top.Value;
        string str = num.ToString();
        nameValueCollection["$top"] = str;
      }
      if (expandOptions != CommentExpandOptions.None)
      {
        NameValueCollection nameValueCollection = queryString;
        num = (int) expandOptions;
        string str = num.ToString();
        nameValueCollection["$expand"] = str;
      }
      if (order.HasValue && order.Value != Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentSortOrder.Desc)
      {
        NameValueCollection nameValueCollection = queryString;
        num = (int) order.Value;
        string str = num.ToString();
        nameValueCollection[nameof (order)] = str;
      }
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.Uri;
    }

    public static string GetWorkItemTagsUrl(
      IVssRequestContext requestContext,
      Guid project,
      TagDefinition tagDefinition)
    {
      try
      {
        return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "wit", WorkItemTrackingLocationIds.WorkItemTagsGuid, project, (object) new
        {
          tagIdOrName = tagDefinition.TagId
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(requestContext, ResourceStrings.LocationServiceException(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    private static string GetApiVersionStringAsParameter(
      ApiResourceVersion apiVersion,
      bool asFirstParameter = false)
    {
      string str = (string) null;
      if (apiVersion != null)
        str = apiVersion.ToString();
      return !string.IsNullOrEmpty(str) ? (asFirstParameter ? "?" : "&") + "api-version=" + str : string.Empty;
    }

    private static Guid? SanitizeProjectIdForResponseUrl(
      Guid? projectId,
      bool generateProjectScopedUrl = true)
    {
      if (!generateProjectScopedUrl)
        projectId = new Guid?();
      else if (projectId.HasValue)
      {
        Guid? nullable = projectId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
          projectId = new Guid?();
      }
      return projectId;
    }
  }
}
