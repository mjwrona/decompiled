// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public class WorkItemFactory
  {
    public static readonly string[] ExcludedFields = new string[11]
    {
      "System.Id",
      "System.IsDeleted",
      "System.Rev",
      "System.AreaId",
      "System.NodeName",
      "System.IterationId",
      "System.PersonId",
      "System.RevisedDate",
      "System.Watermark",
      "System.AuthorizedDate",
      "System.AuthorizedAs"
    };
    public static readonly string[] ExcludedStartsWithFields = new string[2]
    {
      "System.AreaLevel",
      "System.IterationLevel"
    };
    public static readonly string[] ExcludedEndsWithExtensionFields = new string[1]
    {
      "ExtensionMarker"
    };
    private static readonly Version MinimumVersionForCommentLink = new Version(5, 0);
    private const string c_RemovedSentinelValue = "$Removed";

    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem Create(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData workItemFieldData,
      bool expandFields = false,
      IEnumerable<string> fields = null,
      bool includeURL = true,
      bool includeIdentityRef = false,
      bool includeIsDeletedField = false,
      bool includeTagRef = false,
      IDictionary<Guid, IdentityReference> identityMap = null,
      bool returnIdentityRef = false,
      bool returnProjectScopedUrl = true,
      bool includeLinks = false,
      bool extendCommentVersionRef = false)
    {
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem((ISecuredObject) workItemFieldData);
      workItem.Id = new int?(workItemFieldData.Id);
      workItem.Rev = new int?(workItemFieldData.Revision);
      string str;
      if (!includeURL)
      {
        str = (string) null;
      }
      else
      {
        IVssRequestContext requestContext = witRequestContext.RequestContext;
        int id = workItemFieldData.Id;
        bool flag = returnProjectScopedUrl;
        Guid? project = new Guid?();
        int num = flag ? 1 : 0;
        Guid? remoteHostId = new Guid?();
        Guid? remoteProjectId = new Guid?();
        str = WitUrlHelper.GetWorkItemUrl(requestContext, id, project: project, generateProjectScopedUrl: num != 0, remoteHostId: remoteHostId, remoteProjectId: remoteProjectId);
      }
      workItem.Url = str;
      workItem.Fields = WorkItemFactory.GetWorkItemFields(witRequestContext, workItemFieldData, expandFields, fields, includeIdentityRef, includeIsDeletedField, includeTagRef, identityMap, returnIdentityRef);
      workItem.Links = includeLinks ? WorkItemFactory.GetWorkItemReferenceLinks(witRequestContext, workItemFieldData, false, false, returnProjectScopedUrl: returnProjectScopedUrl) : (ReferenceLinks) null;
      workItem.CommentVersionRef = WorkItemFactory.GetWorkItemCommentVersionRef(witRequestContext, workItemFieldData, extendCommentVersionRef);
      return workItem;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem Create(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision workItem,
      bool includeRelations = true,
      bool includeLinks = true,
      bool expandFields = false,
      bool isRevision = false,
      bool isUpdate = false,
      IEnumerable<string> fields = null,
      bool includeIdentityRef = false,
      bool isDeleted = false,
      IDictionary<Guid, IdentityReference> identityMap = null,
      bool returnIdentityRef = false,
      bool returnProjectScopedUrl = true,
      bool excludeRemoteLinkProperties = true)
    {
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem1 = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem((ISecuredObject) workItem);
      workItem1.Id = workItem.Id > 0 ? new int?(workItem.Id) : new int?();
      workItem1.Rev = workItem.Revision > 0 ? new int?(workItem.Revision) : new int?();
      workItem1.Url = WorkItemFactory.GetWorkItemUrl(witRequestContext, (WorkItemFieldData) workItem, isRevision, isDeleted, returnProjectScopedUrl);
      workItem1.Links = includeLinks ? WorkItemFactory.GetWorkItemReferenceLinks(witRequestContext, (WorkItemFieldData) workItem, isRevision, isUpdate, isDeleted, returnProjectScopedUrl) : (ReferenceLinks) null;
      workItem1.Fields = WorkItemFactory.GetWorkItemFields(witRequestContext, (WorkItemFieldData) workItem, expandFields, fields, includeIdentityRef, identityMap: identityMap, returnIdentityRef: returnIdentityRef);
      workItem1.Relations = includeRelations ? WorkItemFactory.GetWorkItemRelations(witRequestContext, workItem, returnProjectScopedUrl, excludeRemoteLinkProperties) : (IList<WorkItemRelation>) null;
      workItem1.CommentVersionRef = WorkItemFactory.GetWorkItemCommentVersionRef(witRequestContext, (WorkItemFieldData) workItem);
      return workItem1;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem CreateToWebApiModel(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision workItem,
      bool includeRelations = true,
      bool includeLinks = true,
      bool expandFields = false,
      bool isRevision = false,
      bool isUpdate = false,
      IEnumerable<string> fields = null,
      bool includeIdentityRef = false,
      bool isDeleted = false,
      IDictionary<Guid, IdentityReference> identityMap = null,
      bool returnIdentityRef = false,
      bool returnProjectScopedUrl = true)
    {
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem toWebApiModel = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem((ISecuredObject) workItem);
      toWebApiModel.Id = workItem.Id > 0 ? new int?(workItem.Id) : new int?();
      toWebApiModel.Rev = workItem.Revision > 0 ? new int?(workItem.Revision) : new int?();
      toWebApiModel.Url = WorkItemFactory.GetWorkItemUrl(witRequestContext, (WorkItemFieldData) workItem, isRevision, isDeleted, returnProjectScopedUrl);
      toWebApiModel.Links = includeLinks ? WorkItemFactory.GetWorkItemReferenceLinks(witRequestContext, (WorkItemFieldData) workItem, isRevision, isUpdate, isDeleted, returnProjectScopedUrl) : (ReferenceLinks) null;
      toWebApiModel.Fields = WorkItemFactory.GetWorkItemFields(witRequestContext, (WorkItemFieldData) workItem, expandFields, fields, includeIdentityRef, identityMap: identityMap, returnIdentityRef: returnIdentityRef);
      List<WorkItemRelation> workItemRelationList;
      if (!includeRelations)
      {
        workItemRelationList = (List<WorkItemRelation>) null;
      }
      else
      {
        IList<WorkItemRelation> workItemRelations = WorkItemFactory.GetWorkItemRelations(witRequestContext, workItem, returnProjectScopedUrl);
        workItemRelationList = workItemRelations != null ? workItemRelations.ToList<WorkItemRelation>() : (List<WorkItemRelation>) null;
      }
      toWebApiModel.Relations = (IList<WorkItemRelation>) workItemRelationList;
      return toWebApiModel;
    }

    public static ReferenceLinks GetWorkItemReferenceLinks(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData workItem,
      bool isRevision,
      bool isUpdate,
      bool isDeleted = false,
      bool returnProjectScopedUrl = true)
    {
      ReferenceLinks itemReferenceLinks = new ReferenceLinks();
      Guid projectGuid = workItem.GetProjectGuid(witRequestContext);
      bool flag = projectGuid == Guid.Empty;
      if (isDeleted)
      {
        itemReferenceLinks.AddLink("self", WorkItemFactory.GetWorkItemUrl(witRequestContext, workItem, isRevision, isDeleted, returnProjectScopedUrl), (ISecuredObject) workItem);
        if (!flag)
          itemReferenceLinks.AddLink("workItemType", WitUrlHelper.GetWorkItemTypeUrl(witRequestContext, projectGuid, workItem.WorkItemType));
        itemReferenceLinks.AddLink("fields", WitUrlHelper.GetFieldsUrl(witRequestContext, new Guid?(projectGuid), returnProjectScopedUrl));
        return itemReferenceLinks;
      }
      if (workItem.Id > 0)
      {
        itemReferenceLinks.AddLink("self", WorkItemFactory.GetWorkItemUrl(witRequestContext, workItem, isRevision, returnProjectScopedUrl: returnProjectScopedUrl), (ISecuredObject) workItem);
        if (!isRevision)
          itemReferenceLinks.AddLink("workItemUpdates", WitUrlHelper.GetWorkItemUpdatesUrl(witRequestContext, workItem.Id, project: new Guid?(projectGuid), generateProjectScopedUrl: returnProjectScopedUrl), (ISecuredObject) workItem);
        if (!isUpdate)
          itemReferenceLinks.AddLink("workItemRevisions", WitUrlHelper.GetWorkItemRevisionUrl(witRequestContext, workItem.Id, project: new Guid?(projectGuid), generateProjectScopedUrl: returnProjectScopedUrl), (ISecuredObject) workItem);
        if (isRevision | isUpdate)
          itemReferenceLinks.AddLink("parent", WitUrlHelper.GetWorkItemUrl(witRequestContext.RequestContext, workItem.Id, project: new Guid?(projectGuid), generateProjectScopedUrl: returnProjectScopedUrl), (ISecuredObject) workItem);
      }
      if (!isRevision && !isUpdate)
      {
        if (workItem.Id > 0)
        {
          IVssRequestContext requestContext = witRequestContext.RequestContext;
          if (WitApiVersionHelpers.SupportsVersion(requestContext, WorkItemFactory.MinimumVersionForCommentLink))
            itemReferenceLinks.AddLink("workItemComments", WitUrlHelper.GetWorkItemCommentUrl(witRequestContext, new Guid?(projectGuid), workItem.Id, new int?(), returnProjectScopedUrl), (ISecuredObject) workItem);
          else
            itemReferenceLinks.AddLink("workItemHistory", WitUrlHelper.GetWorkItemHistoryUrl(witRequestContext, workItem.Id, project: new Guid?(projectGuid), generateProjectScopedUrl: returnProjectScopedUrl), (ISecuredObject) workItem);
          if (Guid.Empty == projectGuid)
          {
            itemReferenceLinks.AddLink("html", WitUrlHelper.GetWorkItemEditorUrl(requestContext, workItem.Id), (ISecuredObject) workItem);
          }
          else
          {
            Uri projectUri = new Uri(requestContext.GetService<IProjectService>().GetProject(requestContext, projectGuid).Uri);
            itemReferenceLinks.AddLink("html", WitUrlHelper.GetWorkItemEditorUrl(requestContext, projectUri, workItem.Id), (ISecuredObject) workItem);
          }
        }
        if (!flag)
          itemReferenceLinks.AddLink("workItemType", WitUrlHelper.GetWorkItemTypeUrl(witRequestContext, projectGuid, workItem.WorkItemType), (ISecuredObject) workItem);
        itemReferenceLinks.AddLink("fields", WitUrlHelper.GetFieldsUrl(witRequestContext, new Guid?(projectGuid), returnProjectScopedUrl), (ISecuredObject) workItem);
      }
      return itemReferenceLinks;
    }

    private static IList<WorkItemRelation> GetWorkItemRelations(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemRevision workItem,
      bool returnProjectScopedUrl = true,
      bool excludeRemoteLinkProperties = false)
    {
      Guid projectGuid = workItem.GetProjectGuid(witRequestContext);
      IEnumerable<WorkItemRelation> workItemRelations1 = workItem.WorkItemLinks.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, WorkItemRelation>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, WorkItemRelation>) (wil => WorkItemRelationFactory.Create(witRequestContext.RequestContext, wil, (ISecuredObject) workItem, new Guid?(wil.TargetProjectId != Guid.Empty ? wil.TargetProjectId : projectGuid), returnProjectScopedUrl, excludeRemoteLinkProperties)));
      IEnumerable<WorkItemRelation> workItemRelations2 = workItem.ResourceLinks.Select<WorkItemResourceLinkInfo, WorkItemRelation>((Func<WorkItemResourceLinkInfo, WorkItemRelation>) (rl => WorkItemRelationFactory.Create(witRequestContext.RequestContext, rl, (ISecuredObject) workItem, new Guid?(projectGuid), returnProjectScopedUrl)));
      return !workItemRelations1.Any<WorkItemRelation>() && !workItemRelations2.Any<WorkItemRelation>() ? (IList<WorkItemRelation>) null : (IList<WorkItemRelation>) workItemRelations1.Union<WorkItemRelation>(workItemRelations2).ToList<WorkItemRelation>();
    }

    private static IDictionary<string, object> GetWorkItemFields(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData workItem,
      bool expandFields = false,
      IEnumerable<string> fields = null,
      bool includeIdentityRef = false,
      bool includeIsDeletedField = false,
      bool includeTagRef = false,
      IDictionary<Guid, IdentityReference> identityMap = null,
      bool returnIdentityRef = false)
    {
      try
      {
        IEnumerable<KeyValuePair<FieldEntry, object>> valuesByFieldEntry = workItem.GetAllFieldValuesByFieldEntry(witRequestContext, true);
        Dictionary<string, object> dictionary = valuesByFieldEntry.Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (field => WorkItemFactory.IncludeField(fields, expandFields, field.Key, includeIsDeletedField) && field.Value != null)).ToDictionary<KeyValuePair<FieldEntry, object>, string, object>((Func<KeyValuePair<FieldEntry, object>, string>) (kvp => kvp.Key.ReferenceName), (Func<KeyValuePair<FieldEntry, object>, object>) (kvp =>
        {
          if (returnIdentityRef && kvp.Key.IsIdentity)
            return (object) WorkItemIdentityHelper.GetIdentityRef(witRequestContext.RequestContext, kvp.Value, (ISecuredObject) workItem);
          if (includeIdentityRef && kvp.Key.IsIdentity)
            return (object) WorkItemFactory.GetWorkItemIdentityRef(witRequestContext.RequestContext, workItem, kvp.Key.FieldId, kvp.Value, identityMap);
          return includeTagRef && kvp.Key.ReferenceName == "System.Tags" ? (object) WorkItemFactory.GetWorkItemTagRef(workItem) : kvp.Value;
        }), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        object obj1;
        if (dictionary.TryGetValue("System.AreaId", out obj1))
        {
          int int32 = Convert.ToInt32(obj1);
          Guid projectGuid = workItem.GetProjectGuid(witRequestContext.RequestContext);
          TreeNode node;
          if (projectGuid != Guid.Empty && witRequestContext.TreeService.TryGetTreeNode(projectGuid, int32, out node) && node.IsProject)
            dictionary["System.AreaId"] = (object) node.Children.First<KeyValuePair<string, TreeNode>>((Func<KeyValuePair<string, TreeNode>, bool>) (cn => cn.Value.Type == TreeStructureType.Area)).Value.Id;
        }
        object obj2;
        if (dictionary.TryGetValue("System.IterationId", out obj2))
        {
          int int32 = Convert.ToInt32(obj2);
          Guid projectGuid = workItem.GetProjectGuid(witRequestContext.RequestContext);
          TreeNode node;
          if (projectGuid != Guid.Empty && witRequestContext.TreeService.TryGetTreeNode(projectGuid, int32, out node) && node.IsProject)
            dictionary["System.IterationId"] = (object) node.Children.First<KeyValuePair<string, TreeNode>>((Func<KeyValuePair<string, TreeNode>, bool>) (cn => cn.Value.Type == TreeStructureType.Iteration)).Value.Id;
        }
        if (!dictionary.TryGetValue("System.IterationPath", out object _) && valuesByFieldEntry.Any<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (f => f.Key.FieldId == -105)) && WorkItemFactory.IncludeField(fields, expandFields, valuesByFieldEntry.Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (f => f.Key.FieldId == -105)).First<KeyValuePair<FieldEntry, object>>().Key))
          dictionary["System.IterationPath"] = (object) "$Removed";
        if (!dictionary.TryGetValue("System.AreaPath", out object _) && valuesByFieldEntry.Any<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (f => f.Key.FieldId == -7)) && WorkItemFactory.IncludeField(fields, expandFields, valuesByFieldEntry.Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (f => f.Key.FieldId == -7)).First<KeyValuePair<FieldEntry, object>>().Key))
          dictionary["System.AreaPath"] = (object) "$Removed";
        if (!dictionary.TryGetValue("System.TeamProject", out object _) && valuesByFieldEntry.Any<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (f => f.Key.ReferenceName.Equals("System.TeamProject", StringComparison.OrdinalIgnoreCase))) && WorkItemFactory.IncludeField(fields, expandFields, valuesByFieldEntry.Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (f => f.Key.ReferenceName.Equals("System.TeamProject", StringComparison.OrdinalIgnoreCase))).First<KeyValuePair<FieldEntry, object>>().Key))
          dictionary["System.TeamProject"] = (object) "$Removed";
        return (IDictionary<string, object>) dictionary;
      }
      catch (Exception ex)
      {
        witRequestContext.RequestContext.TraceException(0, "Rest", nameof (GetWorkItemFields), ex);
        throw;
      }
    }

    private static IdentityReference GetWorkItemIdentityRef(
      IVssRequestContext requestContext,
      WorkItemFieldData workItem,
      int fieldId,
      object value,
      IDictionary<Guid, IdentityReference> identityMap)
    {
      IdentityReference workItemIdentityRef = (IdentityReference) null;
      Guid key;
      bool flag = workItem.IdentitityFields.TryGetValue(fieldId, out key) && key != Guid.Empty && identityMap != null && identityMap.TryGetValue(key, out workItemIdentityRef) && workItemIdentityRef != null;
      if (IdentityReferenceBuilder.ShouldUseProperIdentityRef(requestContext))
      {
        if (flag)
          return workItemIdentityRef;
        string witIdentityName = value.ToString();
        ConstantIdentityRef constantIdentityRef = new ConstantIdentityRef((ISecuredObject) workItem);
        constantIdentityRef.DisplayName = witIdentityName;
        constantIdentityRef.UniqueName = witIdentityName;
        return new IdentityReference((IdentityRef) constantIdentityRef, witIdentityName);
      }
      ConstantIdentityRef constantIdentityRef1 = new ConstantIdentityRef((ISecuredObject) workItem);
      constantIdentityRef1.Id = key.ToString();
      constantIdentityRef1.UniqueName = value.ToString();
      return new IdentityReference((IdentityRef) constantIdentityRef1);
    }

    private static WebApiTagDefinition[] GetWorkItemTagRef(WorkItemFieldData workItem) => workItem.TagDefinitions.Select<TagDefinition, WebApiTagDefinition>((Func<TagDefinition, WebApiTagDefinition>) (tagDefinition => new WebApiTagDefinition()
    {
      Id = tagDefinition.TagId,
      Name = tagDefinition.Name,
      Active = new bool?(!tagDefinition.IsDeleted)
    })).ToArray<WebApiTagDefinition>();

    internal static bool IncludeField(
      IEnumerable<string> fields,
      bool expandFields,
      FieldEntry field,
      bool includeIsDeletedField = false)
    {
      if (field.FieldId == -404 && !includeIsDeletedField)
        return false;
      if (fields != null)
        return fields.Contains<string>(field.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      if (!expandFields)
      {
        if (((IEnumerable<string>) WorkItemFactory.ExcludedFields).Contains<string>(field.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName))
          return false;
        foreach (string excludedStartsWithField in WorkItemFactory.ExcludedStartsWithFields)
        {
          if (TFStringComparer.WorkItemFieldReferenceName.StartsWith(field.ReferenceName, excludedStartsWithField))
            return false;
        }
        foreach (string withExtensionField in WorkItemFactory.ExcludedEndsWithExtensionFields)
        {
          if (field.Usage == InternalFieldUsages.WorkItemTypeExtension && TFStringComparer.WorkItemFieldReferenceName.EndsWith(field.ReferenceName, withExtensionField))
            return false;
        }
      }
      return true;
    }

    private static string GetWorkItemUrl(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData workItem,
      bool isRevision,
      bool isDeleted = false,
      bool returnProjectScopedUrl = true)
    {
      Guid projectGuid = workItem.GetProjectGuid(witRequestContext);
      if (workItem.Id > 0)
        return isRevision && !isDeleted ? WitUrlHelper.GetWorkItemRevisionUrl(witRequestContext, workItem.Id, new int?(workItem.Revision), new Guid?(projectGuid), returnProjectScopedUrl) : WitUrlHelper.GetWorkItemUrl(witRequestContext.RequestContext, workItem.Id, isDeleted, new Guid?(projectGuid), returnProjectScopedUrl);
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType typeByReferenceName = witRequestContext.RequestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(witRequestContext.RequestContext, projectGuid, workItem.WorkItemType);
      return WitUrlHelper.GetWorkItemTemplateUrl(witRequestContext, projectGuid, typeByReferenceName.ReferenceName);
    }

    private static WorkItemCommentVersionRef GetWorkItemCommentVersionRef(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData workItem,
      bool extendCommentVersionRef = false)
    {
      if (workItem?.CommentVersion == null || !WorkItemTrackingFeatureFlags.IsCommentServiceReadsFromNewStorageEnabled(witRequestContext.RequestContext))
        return (WorkItemCommentVersionRef) null;
      Guid projectGuid = workItem.GetProjectGuid(witRequestContext);
      return WorkItemCommentVersionRefFactory.Create(witRequestContext.RequestContext, (ISecuredObject) workItem, projectGuid, workItem.CommentVersion, extendCommentVersionRef);
    }
  }
}
