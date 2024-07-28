// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WITCreator
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class WITCreator
  {
    private IVssRequestContext requestContext;
    private TestManagementRequestContext tcmRequestContext;
    protected int opId;
    internal CloneOperationInformation options;
    private IWorkItemFieldsRemotableService fieldsService;
    private ITreeDictionary treeDictionary;
    protected WorkItemTrackingLinkService linkDictionary;
    private static HashSet<string> DoNotCloneFields = new HashSet<string>()
    {
      "System.Tree",
      "System.WorkItem",
      "System.WorkItemLink",
      "System.Links.LinkType",
      "System.NodeType",
      "System.NotAField",
      "System.TeamProject",
      "System.AttachedFiles",
      "System.BISLinks",
      "System.ChangedSet",
      "System.InAdminOnlyTreeFlag",
      "System.InDeletedTreeFlag",
      "System.LinkedFiles",
      "System.PersonId",
      "System.ProjectId",
      "System.RelatedLinks",
      "System.TFServer",
      "System.WorkItemFormId"
    };
    private static HashSet<string> ReadOnlyFields = new HashSet<string>()
    {
      WorkItemFieldRefNames.CreatedDate,
      WorkItemFieldRefNames.ChangedDate
    };
    private static HashSet<string> CoreFields = new HashSet<string>()
    {
      WorkItemFieldRefNames.Rev,
      WorkItemFieldRefNames.AuthorizedDate,
      WorkItemFieldRefNames.RevisedDate,
      WorkItemFieldRefNames.AuthorizedAs,
      WorkItemFieldRefNames.WorkItemType,
      WorkItemFieldRefNames.AreaPath,
      WorkItemFieldRefNames.AreaId,
      WorkItemFieldRefNames.AuthorizedAs,
      WorkItemFieldRefNames.IterationPath,
      WorkItemFieldRefNames.IterationId,
      WorkItemFieldRefNames.CreatedBy,
      WorkItemFieldRefNames.CreatedDate,
      WorkItemFieldRefNames.State,
      WorkItemFieldRefNames.Reason,
      WorkItemFieldRefNames.AssignedTo,
      WorkItemFieldRefNames.IsDeleted,
      WorkItemFieldRefNames.CommentCount
    };

    public WITCreator(TestManagementRequestContext requestContext)
    {
      this.tcmRequestContext = requestContext;
      this.requestContext = requestContext.RequestContext;
      this.fieldsService = this.requestContext.RootContext.GetService<IWorkItemFieldsRemotableService>();
      this.treeDictionary = this.requestContext.WitContext().TreeService;
      this.linkDictionary = this.requestContext.RootContext.GetService<WorkItemTrackingLinkService>();
      this.opId = 0;
      this.options = (CloneOperationInformation) null;
    }

    public void VerifyInputs(
      CloneOptions options,
      GuidAndString project,
      GuidAndString targetProject,
      string targetProjectName)
    {
      if (options == null)
        return;
      if (options.RelatedLinkComment != null && options.RelatedLinkComment.Length > (int) byte.MaxValue)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyRelatedLinkTooLong, (object) (int) byte.MaxValue));
      this.VerifyOverriddenFieldDetails(options, targetProject);
      if (!project.GuidId.Equals(targetProject.GuidId) && (!options.ResolvedFieldDetails.ContainsKey("System.IterationId") || !options.ResolvedFieldDetails.ContainsKey("System.AreaId")))
        throw new TestManagementValidationException(ServerResources.DeepCopy_TreePathMustForCloneAcrossProjects);
      if (options.CloneRequirements)
        this.ValidateAndGetWorkItemTypeCategory("Microsoft.RequirementCategory", targetProjectName, ServerResources.DeepCopy_TargetProjectHasNoRequirementCategory);
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory itemTypeCategory = this.ValidateAndGetWorkItemTypeCategory("Microsoft.TestCaseCategory", targetProjectName, ServerResources.DeepCopy_TargetProjectHasNoTestCaseCategory);
      if (string.IsNullOrEmpty(options.DestinationWorkItemType))
        return;
      string str = (string) null;
      foreach (string workItemTypeName in itemTypeCategory.WorkItemTypeNames)
      {
        if (string.Equals(workItemTypeName, options.DestinationWorkItemType, StringComparison.OrdinalIgnoreCase))
        {
          str = workItemTypeName;
          break;
        }
      }
      options.DestinationWorkItemType = !string.IsNullOrEmpty(str) ? str : throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DestinationWorkItemTypeNotFound, (object) options.DestinationWorkItemType, (object) targetProjectName));
    }

    public void VerifyTestCaseInputs(
      CloneTestCaseOptions options,
      GuidAndString project,
      GuidAndString targetProject)
    {
      if (options == null)
        return;
      if (options.RelatedLinkComment != null && options.RelatedLinkComment.Length > (int) byte.MaxValue)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyRelatedLinkTooLong, (object) (int) byte.MaxValue));
      this.VerifyOverriddenFieldDetailsForTestCase(options, targetProject);
      if (!project.GuidId.Equals(targetProject.GuidId) && (!options.ResolvedFieldDetails.ContainsKey("System.IterationId") || !options.ResolvedFieldDetails.ContainsKey("System.AreaId")))
        throw new TestManagementValidationException(ServerResources.DeepCopy_TreePathMustForCloneAcrossProjects);
    }

    public Dictionary<string, string> GetOverriddenFieldDetails(
      GuidAndString targetProject,
      Dictionary<string, string> resolvedFieldDetails)
    {
      Dictionary<string, string> overriddenFieldDetails = new Dictionary<string, string>();
      if (resolvedFieldDetails.Count > 0)
      {
        foreach (KeyValuePair<string, string> resolvedFieldDetail in resolvedFieldDetails)
        {
          try
          {
            KeyValuePair<string, string> keyValuePair = this.UpdateOverrideFieldDetails(resolvedFieldDetail.Key, resolvedFieldDetail.Value, targetProject);
            if (keyValuePair.Key != null)
            {
              if (keyValuePair.Value != null)
                overriddenFieldDetails.Add(keyValuePair.Key, keyValuePair.Value);
            }
          }
          catch (Exception ex)
          {
            this.requestContext.TraceException("BusinessLayer", ex);
          }
        }
      }
      return overriddenFieldDetails;
    }

    internal virtual int Wrapper_LegacyGetTreeNodeIdFromPath(
      IVssRequestContext RequestContext,
      string approxValue,
      TreeStructureType type)
    {
      return this.treeDictionary.LegacyGetTreeNodeIdFromPath(RequestContext, approxValue, type);
    }

    private Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory ValidateAndGetWorkItemTypeCategory(
      string resourceKey,
      string targetProjectName,
      string exceptionString)
    {
      using (PerfManager.Measure(this.requestContext, "CrossService", TraceUtils.GetActionName(nameof (ValidateAndGetWorkItemTypeCategory), "WorkItem")))
        return this.requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(this.requestContext, targetProjectName).FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory, bool>) (c => c.ReferenceName.Equals(resourceKey, StringComparison.OrdinalIgnoreCase))) ?? throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, exceptionString, (object) targetProjectName));
    }

    private void VerifyOverriddenFieldDetails(CloneOptions options, GuidAndString targetProject)
    {
      if (options.OverrideFieldDetails.Count > 0)
      {
        foreach (NameValuePair overrideFieldDetail in options.OverrideFieldDetails)
          this.UpdateResolvedFieldDetails(options, overrideFieldDetail.Name, overrideFieldDetail.Value, targetProject);
      }
      else
      {
        if (string.IsNullOrWhiteSpace(options.OverrideFieldName))
          return;
        this.UpdateResolvedFieldDetails(options, options.OverrideFieldName, options.OverrideFieldValue, targetProject);
      }
    }

    private void VerifyOverriddenFieldDetailsForTestCase(
      CloneTestCaseOptions options,
      GuidAndString targetProject)
    {
      if (options.OverrideFieldDetails.Count <= 0)
        return;
      foreach (NameValuePair overrideFieldDetail in options.OverrideFieldDetails)
        this.UpdateResolvedFieldDetailsForTestCase(options, overrideFieldDetail.Name, overrideFieldDetail.Value, targetProject);
    }

    private void UpdateResolvedFieldDetails(
      CloneOptions options,
      string fieldName,
      string fieldValue,
      GuidAndString targetProject)
    {
      KeyValuePair<string, string> fieldDetail = this.ResolveFieldDetails(fieldName, fieldValue, targetProject);
      this.EnsureOverrideFieldUniqueness(options, fieldDetail, fieldName);
      options.ResolvedFieldDetails.Add(fieldDetail.Key, fieldDetail.Value);
    }

    private void UpdateResolvedFieldDetailsForTestCase(
      CloneTestCaseOptions options,
      string fieldName,
      string fieldValue,
      GuidAndString targetProject)
    {
      KeyValuePair<string, string> fieldDetails = this.ResolveFieldDetails(fieldName, fieldValue, targetProject);
      if (options.ResolvedFieldDetails.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (field => field.Key == fieldDetails.Key)))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyFieldOverriddenMultipleTimes, (object) fieldName));
      options.ResolvedFieldDetails.Add(fieldDetails.Key, fieldDetails.Value);
    }

    private KeyValuePair<string, string> UpdateOverrideFieldDetails(
      string resolvedFieldId,
      string resolvedFieldValue,
      GuidAndString targetProject)
    {
      string approxName = (string) null;
      string approxValue = (string) null;
      this.GetOverridseFieldNameAndValue(targetProject, true, resolvedFieldId, resolvedFieldValue, out approxName, out approxValue);
      return new KeyValuePair<string, string>(approxName, approxValue);
    }

    private void EnsureOverrideFieldUniqueness(
      CloneOptions options,
      KeyValuePair<string, string> fieldDetail,
      string fieldName)
    {
      if (options.ResolvedFieldDetails.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (field => field.Key == fieldDetail.Key)))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyFieldOverriddenMultipleTimes, (object) fieldName));
    }

    private KeyValuePair<string, string> ResolveFieldDetails(
      string fieldName,
      string fieldValue,
      GuidAndString targetProject,
      bool isDeepCopy = true)
    {
      if (string.IsNullOrWhiteSpace(fieldName))
        throw new TestManagementValidationException(ServerResources.DeepCopyFieldOverriddenFieldNameEmpty);
      if (string.IsNullOrWhiteSpace(fieldValue))
        throw new TestManagementValidationException(ServerResources.DeepCopyFieldValueMissing);
      string resolvedFieldId;
      string resolvedFieldValue;
      this.ResolveFieldNameAndValue(fieldName, fieldValue, targetProject, isDeepCopy, out resolvedFieldId, out resolvedFieldValue);
      if (resolvedFieldValue.Length > 1024)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyFieldValueTooLong, (object) 1024));
      this.requestContext.TraceInfo("BusinessLayer", "WITCreator: Field {0} with value {1} resolved to field {2} with value {3}", (object) fieldName, (object) fieldValue, (object) resolvedFieldId, (object) resolvedFieldValue);
      return new KeyValuePair<string, string>(resolvedFieldId, resolvedFieldValue);
    }

    public void Initialize(CloneOperationInformation options)
    {
      using (PerfManager.Measure(this.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (Initialize), "Identity")))
      {
        this.options = options;
        TeamFoundationIdentity readIdentity = this.RequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.RequestContext, new Guid[1]
        {
          options.TeamFoundationUserId
        })[0];
        if (readIdentity != null)
          this.options.TeamFoundationUserName = readIdentity.DisplayName;
        this.opId = options.OpId;
        this.PostInitialize(options);
      }
    }

    public void GetWorkItemFieldsAndLinks(
      int witId,
      GuidAndString sourceProject,
      GuidAndString targetProject,
      out IDictionary<string, object> witFields,
      out IList<WorkItemRelation> witRelations,
      out string sourceWorkItemUrl)
    {
      using (PerfManager.Measure(this.requestContext, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItemFieldsAndLinks), "WorkItem")))
      {
        this.requestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "WITCreator.GetWorkItemFieldsAndLinks {0}", (object) witId);
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem = this.requestContext.GetService<IWorkItemRemotableService>().GetWorkItem(this.requestContext, witId, (IEnumerable<string>) new List<string>(), expand: WorkItemExpand.All);
        if (workItem != null)
        {
          witFields = (IDictionary<string, object>) new Dictionary<string, object>();
          foreach (KeyValuePair<string, object> field in (IEnumerable<KeyValuePair<string, object>>) workItem.Fields)
          {
            if (this.IsValidFieldForDeepCopy(field.Key, field.Value, sourceProject, targetProject))
              witFields.Add(field.Key, field.Value);
          }
          witRelations = workItem.Relations == null ? (IList<WorkItemRelation>) new List<WorkItemRelation>() : (IList<WorkItemRelation>) workItem.Relations.ToList<WorkItemRelation>();
          sourceWorkItemUrl = workItem.Url;
        }
        else
        {
          this.requestContext.Trace(0, TraceLevel.Error, "TestManagement", "BusinessLayer", "Workitem {0} not found.", (object) witId);
          witFields = (IDictionary<string, object>) new Dictionary<string, object>();
          witRelations = (IList<WorkItemRelation>) new List<WorkItemRelation>();
          sourceWorkItemUrl = string.Empty;
        }
      }
    }

    public void ResolveFieldNameAndValue(
      string approxName,
      string approxValue,
      GuidAndString targetProject,
      bool isDeepCopy,
      out string resolvedFieldId,
      out string resolvedFieldValue)
    {
      WorkItemField workItemField = (WorkItemField) null;
      resolvedFieldId = (string) null;
      resolvedFieldValue = (string) null;
      foreach (WorkItemField2 field in this.fieldsService.GetFields(this.RequestContext, new GetFieldsExpand?(), (string) null))
      {
        if (string.Equals(field.ReferenceName, approxName, StringComparison.InvariantCulture) || string.Equals(field.Name, approxName, StringComparison.CurrentCulture))
        {
          workItemField = (WorkItemField) field;
          break;
        }
      }
      resolvedFieldId = workItemField != null ? workItemField.ReferenceName : throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyFieldNotFound, (object) approxName));
      resolvedFieldValue = approxValue;
      if ((WITCreator.CoreFields.Contains<string>(workItemField.ReferenceName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase) && !string.Equals(workItemField.ReferenceName, "System.AreaPath", StringComparison.Ordinal) && !string.Equals(workItemField.ReferenceName, "System.IterationPath", StringComparison.Ordinal) || workItemField.ReferenceName.StartsWith("Microsoft.")) && isDeepCopy)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyNonEditableField, (object) approxName));
      if (workItemField.Type == FieldType.TreePath)
      {
        TreeStructureType type = TreeStructureType.None;
        switch (workItemField.ReferenceName)
        {
          case "System.AreaPath":
          case "System.AreaId":
            type = TreeStructureType.Area;
            resolvedFieldId = "System.AreaId";
            break;
          case "System.IterationPath":
          case "System.IterationId":
            type = TreeStructureType.Iteration;
            resolvedFieldId = "System.IterationId";
            break;
        }
        int treeNodeIdFromPath = this.Wrapper_LegacyGetTreeNodeIdFromPath(this.RequestContext, approxValue, type);
        if (treeNodeIdFromPath <= 0)
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyInvalidPath, (object) approxValue, (object) approxName));
        this.ValidateTreePath(type, approxValue, targetProject);
        resolvedFieldValue = Convert.ToString(treeNodeIdFromPath);
      }
      else if (workItemField.ReadOnly && !this.fieldsService.IsUpdatable(this.RequestContext, workItemField.ReferenceName))
      {
        this.requestContext.Trace(1015008, TraceLevel.Info, "TestManagement", "BusinessLayer", "TCM:ResolveFieldNameAndValue : FieldName = {0} FieldName.ReadOnly = {1} FieldName.IsUpdatable = {2}", (object) workItemField.ReferenceName, (object) workItemField.ReadOnly, (object) this.fieldsService.IsUpdatable(this.RequestContext, workItemField.ReferenceName));
      }
      else
      {
        try
        {
          resolvedFieldValue = Convert.ToString(approxValue);
        }
        catch (Exception ex)
        {
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyFieldValueInvalidDataType, (object) approxValue, (object) approxName, (object) workItemField.Type));
        }
      }
    }

    public void GetOverridseFieldNameAndValue(
      GuidAndString targetProject,
      bool isDeepCopy,
      string resolvedFieldId,
      string resolvedFieldValue,
      out string approxName,
      out string approxValue)
    {
      approxName = (string) null;
      approxValue = (string) null;
      if (string.CompareOrdinal(resolvedFieldId, WorkItemFieldRefNames.IterationId) == 0)
        approxName = "System.IterationPath";
      else if (string.CompareOrdinal(resolvedFieldId, WorkItemFieldRefNames.AreaId) == 0)
        approxName = "System.AreaPath";
      int result;
      if (approxName != null && int.TryParse(resolvedFieldValue, out result))
      {
        TreeNode treeNode = (TreeNode) null;
        if (result != 0)
          treeNode = this.treeDictionary.LegacyGetTreeNode(result);
        if (treeNode == null)
          return;
        approxValue = treeNode.GetPath(this.RequestContext);
      }
      else
      {
        foreach (WorkItemField2 field in this.fieldsService.GetFields(this.RequestContext, new GetFieldsExpand?(), (string) null))
        {
          if (string.Compare(field.ReferenceName, resolvedFieldId) == 0)
          {
            approxName = field.Name;
            approxValue = resolvedFieldValue;
          }
        }
      }
    }

    protected virtual void PostInitialize(CloneOperationInformation options)
    {
    }

    internal void CustomizeExplicitlyOverriddenFields(
      IDictionary<string, object> oldTCFields,
      bool insertFieldIfAbsent)
    {
      if (this.options == null || this.options.EditFieldDetails == null)
        return;
      foreach (KeyValuePair<string, string> editFieldDetail in this.options.EditFieldDetails)
      {
        if (editFieldDetail.Key != null && (insertFieldIfAbsent || oldTCFields.ContainsKey(editFieldDetail.Key)))
        {
          oldTCFields[editFieldDetail.Key] = (object) editFieldDetail.Value;
          this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "BusinessLayer", "WITCreator: Overriding property {0} to value {1}", (object) editFieldDetail.Key, (object) editFieldDetail.Value);
        }
      }
    }

    protected void AddRelatedLink(int oldWorkItemId, List<WorkItemLinkUpdate> links) => this.AddRelatedLink(oldWorkItemId, links, WITConstants.NewClonedEntityTempId);

    internal void AddRelatedLink(
      int oldWorkItemId,
      List<WorkItemLinkUpdate> links,
      int temporaryId)
    {
      List<WorkItemLinkUpdate> workItemLinkUpdateList = links;
      WorkItemLinkUpdate workItemLinkUpdate = new WorkItemLinkUpdate();
      workItemLinkUpdate.LinkType = 1;
      workItemLinkUpdate.SourceWorkItemId = temporaryId;
      workItemLinkUpdate.TargetWorkItemId = oldWorkItemId;
      workItemLinkUpdate.Comment = ServerResources.DeepCopyLinkDefaultComment;
      workItemLinkUpdate.UpdateType = LinkUpdateType.Add;
      workItemLinkUpdateList.Add(workItemLinkUpdate);
    }

    internal void AddRelatedLinkInfo(string url, IList<WorkItemRelation> links)
    {
      WorkItemRelation workItemRelation1 = new WorkItemRelation();
      workItemRelation1.Url = url;
      workItemRelation1.Rel = "System.LinkTypes.Related";
      workItemRelation1.Attributes = (IDictionary<string, object>) new Dictionary<string, object>();
      WorkItemRelation workItemRelation2 = workItemRelation1;
      workItemRelation2.Attributes.Add("comment", (object) ServerResources.DeepCopyLinkDefaultComment);
      links.Add(workItemRelation2);
    }

    protected static WorkItemLinkUpdate CreateLinkUpdate(Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo linkInfo) => WITCreator.CreateLinkUpdate(linkInfo, WITConstants.NewClonedEntityTempId);

    protected static WorkItemResourceLinkUpdate CreateResourceLinkUpdate(
      WorkItemResourceLinkInfo resourceLinkInfo)
    {
      return WITCreator.CreateResourceLinkUpdate(resourceLinkInfo, WITConstants.NewClonedEntityTempId);
    }

    protected static WorkItemResourceLinkUpdate CreateResourceLinkUpdate(
      WorkItemResourceLinkInfo resourceLinkInfo,
      int temporaryId)
    {
      WorkItemResourceLinkUpdate resourceLinkUpdate = new WorkItemResourceLinkUpdate();
      resourceLinkUpdate.Comment = resourceLinkInfo.Comment;
      resourceLinkUpdate.Length = new int?(resourceLinkInfo.ResourceSize);
      resourceLinkUpdate.Location = resourceLinkInfo.Location;
      resourceLinkUpdate.Name = resourceLinkInfo.Name;
      resourceLinkUpdate.ResourceId = new int?(resourceLinkInfo.ResourceId);
      resourceLinkUpdate.SourceWorkItemId = temporaryId;
      resourceLinkUpdate.Type = new ResourceLinkType?(resourceLinkInfo.ResourceType);
      resourceLinkUpdate.UpdateType = LinkUpdateType.Add;
      return resourceLinkUpdate;
    }

    protected static WorkItemLinkUpdate CreateLinkUpdate(Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo linkInfo, int temporaryId)
    {
      WorkItemLinkUpdate linkUpdate = new WorkItemLinkUpdate();
      linkUpdate.Comment = linkInfo.Comment;
      linkUpdate.LinkType = linkInfo.LinkType;
      linkUpdate.SourceWorkItemId = temporaryId;
      linkUpdate.TargetWorkItemId = linkInfo.TargetId;
      linkUpdate.UpdateType = LinkUpdateType.Add;
      return linkUpdate;
    }

    private bool IsValidFieldForDeepCopy(
      string fieldRefName,
      object approxValueObject,
      GuidAndString sourceProject,
      GuidAndString targetProject)
    {
      WorkItemField field = (WorkItemField) this.fieldsService.GetField(this.RequestContext, fieldRefName, (string) null);
      if (field == null)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyFieldNotFound, (object) fieldRefName));
      if (WITCreator.CoreFields.Contains<string>(field.ReferenceName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase) && !string.Equals(field.ReferenceName, "System.AreaPath", StringComparison.Ordinal) && !string.Equals(field.ReferenceName, "System.IterationPath", StringComparison.Ordinal) || field.ReferenceName.StartsWith("Microsoft.") || WITCreator.CoreFields.Contains<string>(field.ReferenceName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase) && (string.Equals(field.ReferenceName, "System.AreaPath", StringComparison.Ordinal) || string.Equals(field.ReferenceName, "System.IterationPath", StringComparison.Ordinal)) && !object.Equals((object) sourceProject.GuidId, (object) targetProject.GuidId))
        return false;
      if (field.Type == FieldType.TreePath)
      {
        TreeStructureType type = TreeStructureType.None;
        switch (field.ReferenceName)
        {
          case "System.AreaPath":
          case "System.AreaId":
            type = TreeStructureType.Area;
            break;
          case "System.IterationPath":
          case "System.IterationId":
            type = TreeStructureType.Iteration;
            break;
        }
        string approxValue = Convert.ToString(Convert.ChangeType(approxValueObject, approxValueObject.GetType(), (IFormatProvider) CultureInfo.CurrentCulture));
        if (this.Wrapper_LegacyGetTreeNodeIdFromPath(this.RequestContext, approxValue, type) <= 0)
          return false;
        this.ValidateTreePath(type, approxValue, sourceProject);
      }
      else if (field.ReadOnly || WITCreator.DoNotCloneFields.Contains<string>(field.ReferenceName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
        return false;
      return true;
    }

    internal void UpdateSpecialFields(int workItemId, IDictionary<string, object> fields)
    {
      this.UpdateCreatedAndLastChangedFields(fields);
      this.UpdateHistoryField(workItemId, fields);
      if (!fields.TryGetValue(WorkItemFieldRefNames.AssignedTo, out object _))
        fields[WorkItemFieldRefNames.AssignedTo] = (object) string.Empty;
      this.RemoveFields(fields, WITCreator.ReadOnlyFields);
      HashSet<string> fieldsToRemove = new HashSet<string>((IEnumerable<string>) new string[4]
      {
        "Microsoft.Sync.ProjSrv.RequestedAssnGuid",
        "Microsoft.Sync.ProjSrv.RequestedProjGuid",
        "Microsoft.Sync.ProjSrv.ProjGuid",
        "Microsoft.Sync.ProjSrv.TaskGuid"
      });
      this.RemoveFields(fields, fieldsToRemove);
    }

    private void UpdateCreatedAndLastChangedFields(IDictionary<string, object> fields)
    {
      fields[WorkItemFieldRefNames.CreatedBy] = (object) this.options.TeamFoundationUserName;
      fields[WorkItemFieldRefNames.ChangedBy] = fields[WorkItemFieldRefNames.CreatedBy];
    }

    internal void UpdateIdentityFields(IDictionary<string, object> fields)
    {
      foreach (KeyValuePair<string, object> keyValuePair in fields.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (entry => entry.Key), (Func<KeyValuePair<string, object>, object>) (entry => entry.Value)))
      {
        if (keyValuePair.Value is IdentityRef)
        {
          string str = CommonUtils.DistinctDisplayName(keyValuePair.Value as IdentityRef);
          if (str != null)
            fields[keyValuePair.Key] = (object) str;
        }
      }
    }

    private void RemoveFields(IDictionary<string, object> fields, HashSet<string> fieldsToRemove)
    {
      foreach (string key in fieldsToRemove)
      {
        if (fields.ContainsKey(key))
          fields.Remove(key);
      }
    }

    private void UpdateHistoryField(int oldWorkItemId, IDictionary<string, object> fields) => fields[WorkItemFieldRefNames.History] = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.HistoryCommentForClonedObject, (object) "TF237027", (object) this.opId, (object) oldWorkItemId);

    private void ValidateTreePath(
      TreeStructureType type,
      string approxValue,
      GuidAndString targetProject)
    {
      string a = targetProject.String;
      string str = (string) null;
      switch (type)
      {
        case TreeStructureType.Area:
          TcmCommonStructureNodeInfo cssNodeAndThrow1 = this.tcmRequestContext.AreaPathsCache.GetCssNodeAndThrow(this.tcmRequestContext, approxValue);
          string uri1 = cssNodeAndThrow1.Uri;
          str = cssNodeAndThrow1.ProjectUri;
          break;
        case TreeStructureType.Iteration:
          TcmCommonStructureNodeInfo cssNodeAndThrow2 = this.tcmRequestContext.IterationsCache.GetCssNodeAndThrow(this.tcmRequestContext, approxValue);
          string uri2 = cssNodeAndThrow2.Uri;
          str = cssNodeAndThrow2.ProjectUri;
          break;
      }
      string b = str;
      if (!string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopy_TreePathNotInProject, (object) approxValue));
    }

    internal Dictionary<string, object> GetCloneableFieldData(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
      IWorkItemFieldsRemotableService fieldsService = this.requestContext.GetService<IWorkItemFieldsRemotableService>();
      return workItem.Fields.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => WITCreator.IsClonableField((WorkItemField) fieldsService.GetField(this.RequestContext, kvp.Key, new Guid?())))).ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value));
    }

    internal Dictionary<string, object> GetCloneableFieldData(Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem)
    {
      WorkItemTrackingFieldService fieldDictionary = this.requestContext.GetService<WorkItemTrackingFieldService>();
      return workItem.LatestData.Select<KeyValuePair<int, object>, KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<int, object>, KeyValuePair<FieldEntry, object>>) (kvp => new KeyValuePair<FieldEntry, object>(fieldDictionary.GetFieldById(this.requestContext, kvp.Key), kvp.Value))).Where<KeyValuePair<FieldEntry, object>>((Func<KeyValuePair<FieldEntry, object>, bool>) (kvp => WITCreator.IsClonableField(kvp.Key))).ToDictionary<KeyValuePair<FieldEntry, object>, string, object>((Func<KeyValuePair<FieldEntry, object>, string>) (kvp => kvp.Key.ReferenceName), (Func<KeyValuePair<FieldEntry, object>, object>) (kvp => kvp.Value));
    }

    private static bool IsClonableField(WorkItemField e) => !WITCreator.IsComputed(e) && !WITCreator.IsReadOnly(e) && !WITCreator.DoNotCloneFields.Contains(e.ReferenceName);

    private static bool IsClonableField(FieldEntry e) => !e.IsComputed && !e.IsReadOnly && !WITCreator.DoNotCloneFields.Contains(e.ReferenceName);

    private static bool IsComputed(WorkItemField workItemField) => workItemField.Type == FieldType.TreePath || workItemField.ReadOnly && !WITCreator.ReadOnlyFields.Contains(workItemField.ReferenceName);

    private static bool IsReadOnly(WorkItemField workItemField) => workItemField.ReadOnly && !WITCreator.ReadOnlyFields.Contains(workItemField.ReferenceName);

    protected IVssRequestContext RequestContext => this.requestContext;

    protected TestManagementRequestContext TcmRequestContext => this.tcmRequestContext;

    protected IWorkItemFieldsRemotableService FieldsService
    {
      get => this.fieldsService;
      set => this.fieldsService = value;
    }

    protected ITreeDictionary TreeDictionary
    {
      get => this.treeDictionary;
      set => this.treeDictionary = value;
    }
  }
}
