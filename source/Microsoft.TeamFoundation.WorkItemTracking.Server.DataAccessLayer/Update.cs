// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Update
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class Update
  {
    private RequiredFields m_requiredFields;
    private IVssRequestContext m_requestContext;
    private WorkItemTrackingRequestContext m_witRequestContext;
    private SqlBatchBuilder m_sqlBatch;
    private XmlNode m_outputNode;
    private HashSet<int> m_reservedTempIds;
    private int m_lastActionId;
    private List<BaseUpdate> m_updateList;
    private List<int> m_workItemIds;
    private DalMetadataSelectElement m_getMetadataElement;
    private DalGetDbStampElement m_getDbStampElement;
    private bool m_metadataChanged;
    private bool m_destroyedWorkItemType;
    private IVssIdentity m_user;
    private bool m_bisNotification;
    private bool m_bypassRules;
    private bool m_isServiceAccount;
    private int m_linkChangeOrder;
    private bool m_overwrite;
    private IFieldTypeDictionary m_fieldsSnapshot;
    private Dictionary<string, object> m_serverComputedValues;
    private Update.LinkTypePermissions m_linkTypePermissions;
    private DalCheckChangesElement m_checkChanges;
    private DalApplyIssueChangesElement m_applyIssueChanges;
    private bool m_verboseOn;
    private bool m_isBulkUpdate;
    private bool m_isValidBatch = true;
    private ClientCapabilities m_clientCapabilities;
    private DateTime m_changedDate;
    private Dictionary<Guid, ServerQueryItem> m_activeQueryItems;
    private Dictionary<Guid, ServerQueryItem> m_deletedQueryItems;
    private DalPersonIdMapElement m_personIdMapElement;
    private DalGetTempIdMapElement m_getTempIdMapElement;
    private Dictionary<Type, DalSqlElement> m_singletonElements = new Dictionary<Type, DalSqlElement>();
    private ElementGroup m_getUtcGroup;
    private ElementGroup m_personGroup;
    private DalChangeConstantElement m_changeConstantElement;
    private DalSaveRuleElement m_saveRuleElement;
    private DalSaveIdentityRuleElement m_saveIdentityRuleElement;
    private DalSaveConstantSetElement m_saveConstantSetElement;
    private ElementGroup m_beginTransactionGroup;
    private ElementGroup m_changeIssueGroup;
    private ElementGroup m_changeQueryGroup;
    private ElementGroup m_changeFieldGroup;
    private ElementGroup m_deleteFieldGroup;
    private ElementGroup m_checkFieldGroup;
    private ElementGroup m_changeTreeGroup;
    private ElementGroup m_checkTreeGroup;
    private ElementGroup m_changeConstantSetGroup;
    private ElementGroup m_changeWorkItemTypeGroup;
    private ElementGroup m_changeWorkItemTypeUsageGroup;
    private ElementGroup m_checkWorkItemTypeGroup;
    private ElementGroup m_changeActionGroup;
    private ElementGroup m_checkActionGroup;
    private ElementGroup m_changeRuleGroup;
    private ElementGroup m_checkRuleGroup;
    private ElementGroup m_endTransactionGroup;
    private ElementGroup m_addLinkTypeGroup;
    private ElementGroup m_updateLinkTypeGroup;
    private ElementGroup m_deleteLinkTypeGroup;
    private ElementGroup m_addLinkGroup;
    private ElementGroup m_updateLinkGroup;
    private ElementGroup m_deleteLinkGroup;
    private ElementGroup m_changeCategoryGroup;
    private ElementGroup m_checkCategoryGroup;
    private ElementGroup m_changeCategoryMemberGroup;
    private ElementGroup m_destroyCategoryGroup;
    private List<int> m_destroyWorkItems;
    private ProvisionHelper m_provisionHelper;
    private PermissionCheckHelper m_permissionCheckHelper;
    private bool m_extensionMatcherInitialized;
    private IWorkItemTypeExtensionsMatcher m_extensionMatcher;
    private Microsoft.VisualStudio.Services.Identity.Identity m_requestContextIdentity;
    private const string s_authorizeProvisioningStmt = "\r\nIF ([dbo].[{0}]({1}, NULL) = 0)\r\nBEGIN\r\n    IF (@@TRANCOUNT <> 0) ROLLBACK TRAN\r\n    EXEC [dbo].[RaiseWITError] {2}, 16, 1\r\n    RETURN\r\nEND";

    public Update(
      WorkItemTrackingRequestContext witRequestContext,
      SqlBatchBuilder sqlBatch,
      XmlNode outputNode,
      bool bisNotification,
      bool bulkUpdate,
      IVssIdentity user,
      bool overwrite)
    {
      this.m_requestContext = witRequestContext.RequestContext;
      this.m_witRequestContext = witRequestContext;
      this.m_sqlBatch = sqlBatch;
      this.m_outputNode = outputNode;
      this.m_user = user;
      this.m_overwrite = overwrite;
      this.m_changedDate = DateTime.UtcNow;
      this.m_getUtcGroup = new ElementGroup();
      this.m_personGroup = new ElementGroup();
      this.m_beginTransactionGroup = new ElementGroup();
      this.m_changeIssueGroup = new ElementGroup();
      this.m_changeQueryGroup = new ElementGroup();
      this.m_changeFieldGroup = new ElementGroup();
      this.m_deleteFieldGroup = new ElementGroup();
      this.m_checkFieldGroup = new ElementGroup();
      this.m_changeTreeGroup = new ElementGroup();
      this.m_checkTreeGroup = new ElementGroup();
      this.m_changeConstantSetGroup = new ElementGroup();
      this.m_changeWorkItemTypeGroup = new ElementGroup();
      this.m_checkWorkItemTypeGroup = new ElementGroup();
      this.m_changeWorkItemTypeUsageGroup = new ElementGroup();
      this.m_changeActionGroup = new ElementGroup();
      this.m_checkActionGroup = new ElementGroup();
      this.m_changeRuleGroup = new ElementGroup();
      this.m_checkRuleGroup = new ElementGroup();
      this.m_endTransactionGroup = new ElementGroup();
      this.m_addLinkTypeGroup = new ElementGroup();
      this.m_updateLinkTypeGroup = new ElementGroup();
      this.m_deleteLinkTypeGroup = new ElementGroup();
      this.m_addLinkGroup = new ElementGroup();
      this.m_updateLinkGroup = new ElementGroup();
      this.m_deleteLinkGroup = new ElementGroup();
      this.m_changeCategoryGroup = new ElementGroup();
      this.m_checkCategoryGroup = new ElementGroup();
      this.m_changeCategoryMemberGroup = new ElementGroup();
      this.m_destroyCategoryGroup = new ElementGroup();
      this.m_bisNotification = bisNotification;
      this.m_isBulkUpdate = bulkUpdate;
      this.m_reservedTempIds = new HashSet<int>();
      this.m_updateList = new List<BaseUpdate>();
      this.m_activeQueryItems = new Dictionary<Guid, ServerQueryItem>();
      this.m_deletedQueryItems = new Dictionary<Guid, ServerQueryItem>();
      this.m_workItemIds = new List<int>();
      this.m_requiredFields = bisNotification ? RequiredFields.Notification : RequiredFields.Core;
    }

    public void BuildBatch(
      XmlElement updateElement,
      MetadataTable[] tablesRequested,
      long[] rowVersions,
      bool bypassRules = false,
      bool validationOnly = false,
      bool provisionRules = true)
    {
      this.m_requestContext.TraceEnter(900303, nameof (Update), nameof (Update), nameof (BuildBatch));
      if (validationOnly)
        this.m_bisNotification = false;
      this.Package = updateElement;
      this.m_requestContextIdentity = this.m_requestContext.GetUserIdentity();
      this.m_verboseOn = false;
      this.GetClientCapabilities(updateElement);
      this.GetTempIds(updateElement);
      this.GetBypassEnabled(updateElement, bypassRules);
      XmlNodeList childNodes = updateElement.ChildNodes;
      int count = childNodes.Count;
      this.m_requestContext.Trace(900321, TraceLevel.Verbose, nameof (Update), nameof (Update), "Number of actions in update Xml: {0}", (object) count.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (count == 0)
        throw new ArgumentException(DalResourceStrings.Get("UpdateNoActionsInUpdateXmlException"), nameof (updateElement));
      int bulkUpdateActionCount = 0;
      this.CheckBulkUpdatePackage(updateElement, out bulkUpdateActionCount);
      if (this.m_isBulkUpdate && bulkUpdateActionCount == 0)
        this.m_isBulkUpdate = false;
      if (this.m_isBulkUpdate)
      {
        if (bulkUpdateActionCount > 200)
          throw new ArgumentException(DalResourceStrings.Format("UpdateWorkItemTooManyWorkitems", (object) 200.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (updateElement));
        int multipleUpdateWorkitem = 0;
        if (this.MultipleUpdatesToSameWI(updateElement, out multipleUpdateWorkitem))
          throw new ArgumentException(DalResourceStrings.Format("UpdateWorkItemMultipleTimes", (object) multipleUpdateWorkitem.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (updateElement));
      }
      int num = 0;
      foreach (XmlNode actionNode in childNodes)
      {
        if (actionNode.NodeType == XmlNodeType.Element)
        {
          XmlNode outputNode = this.m_outputNode.AppendChild((XmlNode) this.m_outputNode.OwnerDocument.CreateElement(actionNode.Name));
          this.AddAction(actionNode, outputNode, this.m_verboseOn, provisionRules);
          if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertField"))
            ++num;
        }
      }
      this.m_metadataChanged = this.IsSingletonNeeded<DalChangeConstantElement>(ref this.m_changeConstantElement) || this.m_changeFieldGroup.ElementCount > 0 || this.m_deleteFieldGroup.ElementCount > 0 || this.IsSingletonNeeded<DalSaveConstantSetElement>(ref this.m_saveConstantSetElement) || this.m_changeConstantSetGroup.ElementCount > 0 || this.m_changeTreeGroup.ElementCount > 0 || this.m_changeWorkItemTypeGroup.ElementCount > 0 || this.m_changeWorkItemTypeUsageGroup.ElementCount > 0 || this.m_changeActionGroup.ElementCount > 0 || this.IsSingletonNeeded<DalSaveRuleElement>(ref this.m_saveRuleElement) || this.m_changeRuleGroup.ElementCount > 0 || this.m_changeCategoryGroup.ElementCount > 0 || this.m_changeCategoryMemberGroup.ElementCount > 0 || this.m_destroyCategoryGroup.ElementCount > 0 || this.IsLinkTypeBatch || this.WorkItemTypeTemplateUpdateType != 0;
      if (this.m_metadataChanged)
      {
        this.AuthorizeProvisioning(!this.IsLinkTypeBatch);
        List<MetadataTable> metadataTypes = new List<MetadataTable>();
        if (this.m_changeWorkItemTypeGroup.ElementCount > 0)
          metadataTypes.Add(MetadataTable.WorkItemTypes);
        if (this.m_changeWorkItemTypeUsageGroup.ElementCount > 0)
          metadataTypes.Add(MetadataTable.WorkItemTypeUsages);
        if (this.m_changeRuleGroup.ElementCount > 0)
          metadataTypes.Add(MetadataTable.Rules);
        if (this.m_changeTreeGroup.ElementCount > 0)
          metadataTypes.Add(MetadataTable.HierarchyProperties);
        if (this.m_changeCategoryMemberGroup.ElementCount > 0)
          metadataTypes.Add(MetadataTable.WorkItemTypeCategoryMembers);
        this.m_witRequestContext.RequestContext.GetService<WorkItemMetadataCompatibilityService>().IncreaseWorkItemMetadataBucketIds(this.m_witRequestContext.RequestContext, (IEnumerable<MetadataTable>) metadataTypes);
      }
      this.AddStaticElementsToBatch(this.m_verboseOn);
      DalSqlElement.GetElement<DalPersonIdElement>(this.m_sqlBatch).JoinBatch(this.PersonGroup, this.m_user);
      this.AddGroupToBatch(this.PersonGroup);
      if (this.IsQueryItemBatch)
        this.AddQueryUpdatesToBatch();
      this.m_getDbStampElement = DalSqlElement.GetElement<DalGetDbStampElement>(this.m_sqlBatch);
      this.m_getDbStampElement.JoinBatch();
      if (!this.m_metadataChanged)
      {
        this.m_getMetadataElement = DalSqlElement.GetElement<DalMetadataSelectElement>(this.m_sqlBatch);
        this.m_getMetadataElement.JoinBatch(tablesRequested, rowVersions);
      }
      if (this.IsSingletonNeeded<DalChangeConstantElement>(ref this.m_changeConstantElement))
        this.m_changeConstantElement.JoinBatch((ElementGroup) null, WorkItemTrackingFeatureFlags.IsConstantsCaseIgnoredForXmlUpdate(this.m_requestContext));
      if (this.m_applyIssueChanges != null && this.m_applyIssueChanges.IsNeeded)
        this.AddSqlForFetchingWatermark();
      this.AddGroupToBatch(this.m_beginTransactionGroup);
      this.AddGroupToBatch(this.m_deleteLinkTypeGroup);
      this.AddGroupToBatch(this.m_addLinkTypeGroup);
      this.AddGroupToBatch(this.m_updateLinkTypeGroup);
      if (this.IsSingletonNeeded<DalPersonIdMapElement>(ref this.m_personIdMapElement))
      {
        ElementGroup group = new ElementGroup();
        this.m_personIdMapElement.JoinBatch(group);
        this.AddGroupToBatch(group);
      }
      this.AddGroupToBatch(this.m_changeQueryGroup);
      this.AddGroupToBatch(this.m_changeFieldGroup);
      this.AddGroupToBatch(this.m_deleteFieldGroup);
      this.AddGroupToBatch(this.m_checkFieldGroup);
      this.AddGroupToBatch(this.m_changeTreeGroup);
      this.AddGroupToBatch(this.m_checkTreeGroup);
      this.AddGroupToBatch(this.m_changeWorkItemTypeGroup);
      this.AddGroupToBatch(this.m_changeWorkItemTypeUsageGroup);
      this.AddGroupToBatch(this.m_changeActionGroup);
      this.AddGroupToBatch(this.m_checkWorkItemTypeGroup);
      this.AddGroupToBatch(this.m_checkActionGroup);
      if (this.IsSingletonNeeded<DalSaveConstantSetElement>(ref this.m_saveConstantSetElement))
        this.m_saveConstantSetElement.JoinBatch((ElementGroup) null, this.Overwrite);
      this.AddGroupToBatch(this.m_changeConstantSetGroup);
      if (this.IsSingletonNeeded<DalSaveRuleElement>(ref this.m_saveRuleElement))
        this.m_saveRuleElement.JoinBatch((ElementGroup) null, this.Overwrite);
      this.AddGroupToBatch(this.m_changeRuleGroup);
      this.AddGroupToBatch(this.m_checkRuleGroup);
      if (this.m_changeCategoryGroup.ElementCount > 0 || this.m_changeCategoryMemberGroup.ElementCount > 0 || this.m_destroyCategoryGroup.ElementCount > 0)
      {
        if (this.m_requestContext.Items.ContainsKey("WorkItemTracking/Provisioning/MetadataSnapshot"))
          DalSqlElement.GetElement<DalMetadataChangeValidationElement>(this.m_sqlBatch).JoinBatch((MetadataDBStamps) this.m_requestContext.Items["WorkItemTracking/Provisioning/MetadataSnapshot"]);
        this.AddGroupToBatch(this.m_destroyCategoryGroup);
        this.AddGroupToBatch(this.m_changeCategoryGroup);
        this.AddGroupToBatch(this.m_checkCategoryGroup);
        this.AddGroupToBatch(this.m_changeCategoryMemberGroup);
      }
      if (this.IsSingletonNeeded<DalGetTempIdMapElement>(ref this.m_getTempIdMapElement))
        this.m_getTempIdMapElement.JoinBatch(this.m_changeIssueGroup);
      if (this.IsSingletonNeeded<DalSaveIdentityRuleElement>(ref this.m_saveIdentityRuleElement))
        this.m_saveIdentityRuleElement.JoinBatch((ElementGroup) null, this.Overwrite);
      if (this.m_checkChanges != null && this.m_checkChanges.IsNeeded)
        this.m_checkChanges.JoinBatch(this.m_changeIssueGroup, this.ObjectType, this.m_verboseOn, this.m_isBulkUpdate);
      if (this.m_applyIssueChanges != null && this.m_applyIssueChanges.IsNeeded)
        this.m_applyIssueChanges.JoinBatch(this.m_changeIssueGroup, this.ObjectType);
      this.AddGroupToBatch(this.m_changeIssueGroup);
      if (this.IsLinkTypeBatch)
      {
        Guid eventAuthor = Guid.Empty;
        if (this.m_requestContext != null)
          eventAuthor = this.m_requestContext.GetService<ITeamFoundationSqlNotificationService>().Author;
        DalSqlElement.GetElement<DalEventChangeElement>(this.m_sqlBatch).JoinBatch("EFD81C28-8A72-4DD7-94FF-16AA16543D81", DalSqlElement.DEFAULT_VALUE, eventAuthor);
      }
      if (this.IsFieldChangeBatch || this.m_destroyedWorkItemType)
      {
        Guid eventAuthor = Guid.Empty;
        if (this.m_requestContext != null)
          eventAuthor = this.m_requestContext.GetService<ITeamFoundationSqlNotificationService>().Author;
        DalSqlElement.GetElement<DalEventChangeElement>(this.m_sqlBatch).JoinBatch("92778466-E528-4569-8736-A7CBD9983DB7", DalSqlElement.DEFAULT_VALUE, eventAuthor);
      }
      if (this.IsQueryItemBatch)
      {
        Guid eventAuthor = Guid.Empty;
        if (this.m_requestContext != null)
          eventAuthor = this.m_requestContext.GetService<ITeamFoundationSqlNotificationService>().Author;
        DalSqlElement.GetElement<DalEventChangeElement>(this.m_sqlBatch).JoinBatch("8E616C67-502E-4BB5-8037-B6D49CAA6E73", QueryChangeNotificationPayload.Create(this.m_activeQueryItems, this.m_deletedQueryItems, (Func<int, Guid>) (id => this.WitRequestContext.TreeService.LegacyGetTreeNode(id).CssNodeId)).Serialize(), eventAuthor);
      }
      if (this.m_metadataChanged)
      {
        Guid eventAuthor = Guid.Empty;
        if (this.m_requestContext != null)
          eventAuthor = this.m_requestContext.GetService<ITeamFoundationSqlNotificationService>().Author;
        DalSqlElement.GetElement<DalEventChangeElement>(this.m_sqlBatch).JoinBatch(DBNotificationIds.WorkItemTrackingProvisionedMetadataChanged, DalSqlElement.DEFAULT_VALUE, eventAuthor);
      }
      DalSqlElement.GetElement<DalEndLocalTranElement>(this.m_sqlBatch).JoinBatch(this.m_endTransactionGroup, !validationOnly);
      this.AddGroupToBatch(this.m_endTransactionGroup);
      if (this.IsWorkItemTypeChangeBatch || this.IsFieldChangeBatch || this.m_destroyedWorkItemType || this.IsRuleChangeBatch)
        this.m_sqlBatch.PostExecutionActions += (SqlBatchBuilder.SqlBatchAction) ((requestContext, executionSucceeded) =>
        {
          if (!executionSucceeded)
            return;
          bool flag;
          requestContext.Items.TryGetValue<bool>("isContextFromPromotion", out flag);
          if (provisionRules && !flag)
            this.m_requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().SetIdentityFieldBit(this.m_requestContext);
          this.m_requestContext.ResetMetadataDbStamps();
          WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
          if (requestContext.IsFeatureEnabled("WorkItemTracking.Server.SkipFieldCacheResetOnImport"))
            return;
          service.InvalidateCache(this.m_requestContext);
        });
      if (this.m_metadataChanged)
      {
        this.m_getMetadataElement = DalSqlElement.GetElement<DalMetadataSelectElement>(this.m_sqlBatch);
        this.m_getMetadataElement.JoinBatch(tablesRequested, rowVersions);
      }
      if (this.NeedToReleaseWatermark)
        this.m_sqlBatch.AppendSqlLine("exec dbo.prc_ReleaseWorkItemWatermark @partitionId, @watermark");
      this.m_requestContext.TraceLeave(900577, nameof (Update), nameof (Update), nameof (BuildBatch));
    }

    private void AddSqlForFetchingWatermark()
    {
      if (this.m_sqlBatch.Version < 22 || this.m_sqlBatch.Version >= 24)
        return;
      this.m_sqlBatch.AppendSql("DECLARE @watermark INT \r\nEXEC @status = dbo.prc_AllocateNextWorkItemWatermark @partitionId, @newWatermark = @watermark OUTPUT\r\nIF @status <> 0\r\nBEGIN\r\n    RETURN \r\nEND\r\n");
      this.NeedToReleaseWatermark = true;
    }

    public bool NeedToReleaseWatermark { get; private set; }

    private void AuthorizeProvisioning(bool checkAdminRights)
    {
      this.ProvisionHelper.Authorize(checkAdminRights);
      if (!this.Overwrite)
        return;
      this.AddOverwriteElementsToBatch();
    }

    private void AddOverwriteElementsToBatch()
    {
      if (this.ChangeWorkItemTypeGroup.ElementCount > 3)
        return;
      DalSqlElement element = DalSqlElement.GetElement<DalSqlElement>(this.m_sqlBatch);
      if (this.WorkItemTypeTemplateUpdateType != WorkItemTypeTemplateUpdateType.None)
      {
        this.AddOverwriteWorkItemTypeToBatch(element);
      }
      else
      {
        if (this.ChangeCategoryGroup.ElementCount <= 0)
          return;
        this.AddOverwriteCategoryGroupsToBatch(element);
      }
    }

    private void AddOverwriteWorkItemTypeToBatch(DalSqlElement element)
    {
      int group = this.ChangeRuleGroup.AddElementToGroup(0);
      string str1 = "";
      string message = "Selecting rules for deletion; ";
      if (this.WorkItemTypeTemplateUpdateType == WorkItemTypeTemplateUpdateType.WorkItemType)
      {
        message += string.Format("RootTreeId = {0}, Fld1ID = {1}, Fld1IsContId = {2}; Form rules can be deleted", (object) "@projectId", (object) 25, (object) "@workItemTypeNameConstId");
        str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\ndeclare @ruleIds typ_Int32Table\r\ninsert into @ruleIds\r\nselect R.ID\r\nfrom Rules R\r\nwhere " + element.FormatPartitionClause("PartitionId = @partitionId and ") + "fAcl = 0\r\nand fDeleted = 0\r\nand RootTreeID = {0}\r\nand Fld1ID = {1}\r\nand Fld1IsConstID = {2}\r\nand VersionTo = 255\r\nand not exists (select * from {3} where id = R.ID)" + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))"), (object) "@projectId", (object) 25, (object) "@workItemTypeNameConstId", (object) "#ruleIds");
      }
      else if (this.WorkItemTypeTemplateUpdateType == WorkItemTypeTemplateUpdateType.ProjectGlobalWorkflow || this.WorkItemTypeTemplateUpdateType == WorkItemTypeTemplateUpdateType.CollectionGlobalWorkflow)
      {
        message += string.Format("RootTreeId = {0}, Fld1ID <> {1}, ThenFldID <> {1} and ThenFldID <> {2}; Form rules cannot be deleted", (object) "@projectId", (object) 25, (object) -14);
        str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\ndeclare @ruleIds typ_Int32Table\r\ninsert into @ruleIds\r\nselect R.ID\r\nfrom Rules R\r\nwhere " + element.FormatPartitionClause("PartitionId = @partitionId and ") + "fAcl = 0\r\nand fDeleted = 0\r\nand RootTreeID = {0}\r\nand Fld1ID <> {1}\r\nand ThenFldID <> {3}\r\nand ThenFldID <> {1}\r\nand fEditable = 1\r\nand not exists (select * from {2} where id = R.ID)" + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))"), (object) "@projectId", (object) 25, (object) "#ruleIds", (object) -14);
      }
      this.m_sqlBatch.RequestContext.Trace(908833, TraceLevel.Info, nameof (Update), nameof (Update), message);
      string str2;
      string str3;
      if (this.m_sqlBatch.Version < 42)
      {
        str2 = "typ_WitProcessedConstantsTable";
        str3 = "IsNull(IdentityDisplayName, DisplayPart), NULL";
      }
      else
      {
        str2 = "typ_WitProcessedConstantsTable2";
        str3 = "C.TeamFoundationId, C.ConstId, ISNULL(IdentityDisplayName, DisplayPart), NULL, NULL, NULL";
      }
      this.ChangeRuleGroup.AppendSql(group, string.Format((IFormatProvider) CultureInfo.InvariantCulture, str1 + "\r\n\r\n\r\n\r\ndeclare @setsToDelete dbo.typ_Int32Table\r\nDECLARE @constIds dbo.typ_Int32Table\r\nDECLARE @processedConstants " + str2 + "\r\n\r\ninsert into @setsToDelete\r\nselect S.RuleSetID\r\nfrom Rules R\r\njoin @ruleIds Ids\r\non " + element.FormatPartitionClause("R.PartitionId = @partitionId and ") + "R.ID = Ids.Val\r\njoin Sets S\r\non " + element.FormatPartitionClause("R.PartitionId = S.PartitionId and ") + "R.ThenConstID = S.ParentID\r\nand S.fDeleted = 0\r\njoin Constants C\r\non " + element.FormatPartitionClause("R.PartitionId = C.PartitionId and ") + "S.ParentID = C.ConstID\r\nand C.ConstID > 0\r\nand C.SID is null" + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))") + "\r\n\r\n\r\nupdate S\r\nset ChangeDate = @NowUtc, ChangerID = @PersonId, fDeletedHere = 1\r\noutput inserted.ConstID\r\n    into @constIds\r\nfrom @setsToDelete sd\r\njoin Sets S on sd.Val = s.RuleSetID " + element.FormatPartitionClause(" and S.PartitionId = @partitionId ") + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))") + "\r\n\r\nINSERT INTO @processedConstants\r\nSELECT " + str3 + "\r\nFROM Constants C\r\nJOIN @constIds S\r\nON S.Val = C.ConstID" + element.FormatPartitionClause(" AND C.PartitionId = @partitionId\r\n OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))") + "\r\n\r\nIF (EXISTS(SELECT TOP 1 1 FROM @processedConstants))\r\nEXEC prc_UpdateDuplicateIdentities @partitionId, @processedConstants\r\n\r\nupdate R\r\nset fSuggestion=0,fDefault=0,fHelpText=0,fDenyAdmin=0,fGrantAdmin=0,fDenyWrite=0,fGrantWrite=0,fDenyRead=0,fGrantRead=0,ChangeDate=@NowUtc, ChangerID=@PersonId\r\nfrom Rules R\r\njoin @ruleIds Ids\r\non " + element.FormatPartitionClause("R.PartitionId = @partitionId and ") + "R.ID = Ids.Val" + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n"), (object) "@projectId", (object) 25, (object) "@workItemTypeNameConstId", (object) "#ruleIds"));
      this.ChangeWorkItemTypeUsageGroup.AppendSql(this.ChangeWorkItemTypeUsageGroup.AddElementToGroup(0), string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.SetWorkItemTypeIdIfGlobalWorkflow() + "\r\nupdate W\r\nset fDeleted = 1, ChangeDate=@NowUtc, ChangerID=@PersonId\r\nfrom WorkItemTypeUsages W\r\nwhere " + element.FormatPartitionClause("PartitionId = @partitionId and ") + "WorkItemTypeID = {0}\r\nand fDeleted = 0\r\nand not exists (select * from {1} where id = W.WorkItemTypeUsageID)\r\nand VersionTo = 255" + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n"), (object) "@workItemTypeId", (object) "#workItemTypeUsageIds", (object) "@projectId"));
      if (this.WorkItemTypeTemplateUpdateType == WorkItemTypeTemplateUpdateType.ProjectGlobalWorkflow || this.WorkItemTypeTemplateUpdateType == WorkItemTypeTemplateUpdateType.CollectionGlobalWorkflow)
        this.ChangeWorkItemTypeUsageGroup.AppendSql(this.ChangeWorkItemTypeUsageGroup.AddElementToGroup(0), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\nUPDATE wuObsolete\r\nSET wuObsolete.fDeleted=1, wuObsolete.ChangeDate=@NowUtc, wuObsolete.ChangerID=@PersonId\r\nFROM WorkItemTypeUsages wuChanged\r\nJOIN WorkItemTypeUsages wuObsolete \r\n      ON wuObsolete.FieldID = wuChanged.FieldID \r\n      " + element.FormatPartitionClause(" AND wuObsolete.PartitionId = wuChanged.PartitionId ") + "\r\n      AND wuObsolete.VersionTo = 3\r\n      AND wuObsolete.VersionFrom = 1\r\n      AND wuObsolete.WorkItemTypeID > 0\r\n      AND wuObsolete.fDeleted = 0\r\nWHERE " + element.FormatPartitionClause(" wuChanged.PartitionId = @partitionId AND ") + "\r\n      wuChanged.WorkItemTypeID = {0}\r\n      AND wuChanged.fDeleted = 1\r\n      AND wuChanged.FieldId NOT IN (SELECT wuGlobal.FieldId\r\n                                    FROM WorkItemTypeUsages wuGlobal\r\n                                    WHERE " + element.FormatPartitionClause(" wuGlobal.PartitionId = @partitionId AND ") + "\r\n                                    wuGlobal.WorkItemTypeID <= 0\r\n                                    AND wuGlobal.fDeleted = 0)\r\n\r\n" + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n"), (object) "@workItemTypeId", (object) "#workItemTypeUsageIds", (object) "@projectId"));
      this.ChangeActionGroup.AppendSql(this.ChangeActionGroup.AddElementToGroup(0), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\nupdate A\r\nset fDeleted = 1\r\nfrom Actions A\r\nwhere " + element.FormatPartitionClause("PartitionId = @partitionId and ") + "WorkItemTypeID = {0}\r\nand fDeleted = 0\r\nand not exists (select * from {1} where id = A.ActionID)" + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n"), (object) "@workItemTypeId", (object) "#actionIds"));
      this.ChangeConstantSetGroup.AppendSql(this.ChangeConstantSetGroup.AddElementToGroup(0), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\ndeclare @validTypes int\r\nselect @validTypes = s.ParentID\r\nfrom Sets s\r\njoin Rules r\r\n    on " + element.FormatPartitionClause("s.PartitionId = r.PartitionId and ") + "s.ParentID = r.ThenConstID\r\n    and r.ThenFldID = {0}\r\n    and r.IfFldID = 0\r\n    and r.If2FldID = 0\r\n    and r.Fld1ID = 0\r\n    and r.Fld2ID = 0\r\n    and r.Fld3ID = 0\r\n    and r.Fld4ID = 0\r\n    and r.RootTreeID = {1}\r\n    and r.fFlowDownTree = 1\r\n    and r.fUnless = 1\r\n    and r.fThenLeaf = 1\r\n    and r.fThenOneLevel = 1\r\n    and r.fAcl = 0\r\n    and r.fDeleted = 0\r\n" + element.FormatPartitionClause("where r.PartitionId = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))") + "\r\n\r\n-- Preserve global lists.\r\nDECLARE @globalListConstId INT = -1\r\n\r\nSELECT @globalListConstId = ConstId\r\nFROM Constants\r\nWHERE DisplayPart = '{3}'\r\nAND PartitionId = @partitionId\r\n\r\ndelete from {2}\r\nwhere parentId = @validTypes\r\n   OR parentId = @globalListConstId\r\n\r\nupdate S\r\nset ChangeDate = @NowUtc, ChangerID = @PersonId, fDeletedHere = 1\r\nfrom Sets S\r\njoin {2} TS1\r\non S.ParentID = TS1.parentId\r\nand S.fDeleted = 0\r\nleft join {2} TS2\r\non S.ParentID = TS2.parentId\r\nand S.ConstID = TS2.constId\r\nwhere " + element.FormatPartitionClause("S.PartitionId = @partitionId and ") + "TS2.parentId is null" + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n"), (object) 25, (object) "@projectId", (object) "#sets", (object) "299f07ef-6201-41b3-90fc-03eeb3977587"));
    }

    private string SetWorkItemTypeIdIfGlobalWorkflow() => this.WorkItemTypeTemplateUpdateType == WorkItemTypeTemplateUpdateType.ProjectGlobalWorkflow || this.WorkItemTypeTemplateUpdateType == WorkItemTypeTemplateUpdateType.CollectionGlobalWorkflow ? "\r\nset {0} = -{2};" : "";

    private void AddOverwriteCategoryGroupsToBatch(DalSqlElement element)
    {
      this.ChangeCategoryGroup.AppendSql(this.ChangeCategoryGroup.AddElementToGroup(0), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\nupdate C\r\nset fDeleted = 1, ChangeDate=@NowUtc, ChangerID=@PersonId\r\nfrom WorkItemTypeCategories C\r\nwhere " + element.FormatPartitionClause("PartitionId = @partitionId and ") + "ProjectID = {0}\r\nand fDeleted = 0\r\nand not exists (select * from {1} where id = C.WorkItemTypeCategoryID)" + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n"), (object) "@projectId", (object) "#categoryIds"));
      this.ChangeCategoryMemberGroup.AppendSql(this.ChangeCategoryMemberGroup.AddElementToGroup(0), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\nupdate M\r\nset fDeleted = 1, ChangeDate=@NowUtc, ChangerID=@PersonId\r\nfrom WorkItemTypeCategories C\r\njoin WorkItemTypeCategoryMembers M\r\non C.WorkItemTypeCategoryID = M.WorkItemTypeCategoryID" + element.FormatPartitionClause(" and C.PartitionId = M.PartitionId") + "\r\nwhere " + element.FormatPartitionClause("C.PartitionId = @partitionId and ") + "C.ProjectID = {0}\r\nand M.fDeleted = 0\r\nand not exists (select * from {1} where id = M.WorkItemTypeCategoryMemberID)" + element.FormatPartitionClause("\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n"), (object) "@projectId", (object) "#categoryMembers"));
    }

    private void CheckBulkUpdatePackage(XmlElement updateElement, out int bulkUpdateActionCount)
    {
      this.m_requestContext.TraceEnter(900304, nameof (Update), nameof (Update), nameof (CheckBulkUpdatePackage));
      XmlNodeList xmlNodeList = updateElement.SelectNodes("//InsertWorkItem|//UpdateWorkItem");
      bulkUpdateActionCount = xmlNodeList.Count;
      this.m_requestContext.TraceLeave(900578, nameof (Update), nameof (Update), nameof (CheckBulkUpdatePackage));
    }

    private bool MultipleUpdatesToSameWI(XmlElement updateElement, out int multipleUpdateWorkitem)
    {
      this.m_requestContext.TraceEnter(900306, nameof (Update), nameof (Update), nameof (MultipleUpdatesToSameWI));
      bool sameWi = false;
      XmlNodeList xmlNodeList = updateElement.SelectNodes("//UpdateWorkItem[@WorkItemID!='']");
      string empty = string.Empty;
      int result = 0;
      Hashtable hashtable = new Hashtable(200);
      object obj = new object();
      multipleUpdateWorkitem = 0;
      foreach (XmlElement xmlElement in xmlNodeList)
      {
        if (!int.TryParse(xmlElement.GetAttribute("WorkItemID").Trim(), out result))
          throw new ArgumentException(DalResourceStrings.Get("InvalidWorkItemIdException"), nameof (updateElement));
        if (hashtable.ContainsKey((object) result))
        {
          multipleUpdateWorkitem = result;
          sameWi = true;
        }
        else
          hashtable.Add((object) result, obj);
      }
      this.m_requestContext.TraceLeave(900579, nameof (Update), nameof (Update), nameof (MultipleUpdatesToSameWI));
      return sameWi;
    }

    private void GetClientCapabilities(XmlElement updateElement)
    {
      int result = 0;
      if (updateElement.SelectSingleNode("//@ClientCapabilities") is XmlAttribute xmlAttribute && !int.TryParse(xmlAttribute.Value.Trim(), out result))
        throw new ArgumentException(DalResourceStrings.Get("UpdateInvalidAttributeIntegerException"), nameof (updateElement));
      this.m_clientCapabilities = (ClientCapabilities) result;
    }

    private void GetTempIds(XmlElement updateElement)
    {
      this.m_requestContext.TraceEnter(900307, nameof (Update), nameof (Update), nameof (GetTempIds));
      foreach (XmlElement selectNode in updateElement.SelectNodes(".//*[@TempID!='']"))
      {
        int result;
        if (!int.TryParse(selectNode.GetAttribute("TempID").Trim(), out result))
          throw new ArgumentException(DalResourceStrings.Get("QueryInvalidTempIdException"), nameof (updateElement));
        if (result < 0)
          throw new ArgumentException(DalResourceStrings.Get("QueryInvalidTempIdException"), nameof (updateElement));
        if (this.m_reservedTempIds.Contains(result))
          throw new ArgumentException(DalResourceStrings.Get("UpdateDuplicateTempIdsInUpdateXmlException"), nameof (updateElement));
        this.m_reservedTempIds.Add(result);
      }
      this.m_requestContext.TraceLeave(900580, nameof (Update), nameof (Update), nameof (GetTempIds));
    }

    private void GetBypassEnabled(XmlElement updatePackage, bool bypassRules)
    {
      if (bypassRules)
      {
        this.m_bypassRules = true;
        this.m_isServiceAccount = true;
      }
      else
      {
        this.m_bypassRules = false;
        foreach (XmlElement selectNode in updatePackage.SelectNodes(".//*[@BypassRules!='']"))
        {
          string str = selectNode.GetAttribute("BypassRules").Trim();
          switch (str)
          {
            case "0":
            case "-1":
              this.m_bypassRules = false;
              goto label_16;
            case "1":
              this.m_bypassRules = true;
              continue;
            default:
              if (!bool.TryParse(str, out this.m_bypassRules))
                throw new ArgumentException(DalResourceStrings.Get("UpdateInvalidBooleanAttributeException"), "queryXml");
              if (this.m_bypassRules)
                continue;
              goto label_16;
          }
        }
label_16:
        if (!this.m_bypassRules)
          return;
        this.m_isServiceAccount = this.m_requestContext.IsSystemContext;
        if (this.m_isServiceAccount)
          return;
        this.m_isServiceAccount = this.m_requestContext.GetService<IdentityService>().IsMemberOrSame(this.m_requestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup);
      }
    }

    private void AddStaticElementsToBatch(bool verbose)
    {
      this.m_requestContext.TraceEnter(900308, nameof (Update), nameof (Update), nameof (AddStaticElementsToBatch));
      DalSqlElement.GetElement<DalDeclareRollbackVariableElement>(this.m_sqlBatch).JoinBatch();
      DalSqlElement.GetElement<DalDeclareRollbackErrorElement>(this.m_sqlBatch).JoinBatch();
      DalSqlElement.GetElement<DalDeclareVerboseVariableElement>(this.m_sqlBatch).JoinBatch(verbose);
      DalSqlElement.GetElement<DalCurrentUtcElement>(this.m_sqlBatch).JoinBatch();
      this.m_sqlBatch.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "declare {0} int; set {0} = 0;", (object) "@status"));
      this.m_sqlBatch.AppendSql(Environment.NewLine);
      this.m_sqlBatch.AddParameterBit(false, "@fAdmin");
      this.m_sqlBatch.AddParameterBit(this.m_isBulkUpdate, "@isBulkUpdate");
      this.m_sqlBatch.AddParameterBit(this.m_bypassRules, "@bypassRules");
      this.m_sqlBatch.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "declare {0} typ_WitTempIdMapTable;", (object) "@tempIdMap"));
      this.m_sqlBatch.AppendSql(Environment.NewLine);
      if (this.Overwrite)
      {
        string format1 = "declare {0} int;";
        this.m_sqlBatch.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format1, (object) "@workItemTypeId"));
        this.m_sqlBatch.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format1, (object) "@workItemTypeNameConstId"));
        this.m_sqlBatch.AppendSql(Environment.NewLine);
        string format2 = "create table {0} (id int); create clustered index {1} on {0}(id);";
        this.m_sqlBatch.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) "#ruleIds", (object) "ix_temp_ruleIds_id"));
        this.m_sqlBatch.AppendSql(Environment.NewLine);
        this.m_sqlBatch.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) "#workItemTypeUsageIds", (object) "ix_temp_workItemTypeUsageIds_id"));
        this.m_sqlBatch.AppendSql(Environment.NewLine);
        this.m_sqlBatch.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) "#actionIds", (object) "ix_temp_actionIds_id"));
        this.m_sqlBatch.AppendSql(Environment.NewLine);
        this.m_sqlBatch.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) "#categoryIds", (object) "ix_temp_categoryIds_id"));
        this.m_sqlBatch.AppendSql(Environment.NewLine);
        this.m_sqlBatch.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format2, (object) "#categoryMembers", (object) "ix_temp_categoryMemberIds_id"));
        this.m_sqlBatch.AppendSql(Environment.NewLine);
        this.m_sqlBatch.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "create table {0} (parentId int, constId int); create clustered index ix_temp_sets on {0}(parentId, constId)", (object) "#sets"));
        this.m_sqlBatch.AppendSql(Environment.NewLine);
        if (this.ProvisionHelper.ProjectIds.Count != 1)
          this.m_overwrite = false;
        else
          this.m_sqlBatch.AddParameterInt(this.ProvisionHelper.ProjectIds.First<int>(), "@projectId");
      }
      DalSqlElement.GetElement<DalBeginLocalTranElement>(this.m_sqlBatch).JoinBatch(this.m_beginTransactionGroup);
      this.m_requestContext.TraceLeave(900581, nameof (Update), nameof (Update), nameof (AddStaticElementsToBatch));
    }

    private void AddQueryUpdatesToBatch()
    {
      LocalSecurityNamespace queryItemSecurity = this.m_requestContext.GetService<WorkItemTrackingService>().QueryItemSecurity;
      HashSet<IdentityDescriptor> descriptors = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      this.EnumerateQueryReferencedIdentities((Func<IdentityDescriptor, Guid>) (descriptor =>
      {
        descriptors.Add(descriptor);
        return Guid.Empty;
      }));
      if (descriptors.Count > 0)
      {
        Dictionary<IdentityDescriptor, Guid> idMap = this.m_requestContext.GetService<IdentityService>().ReadIdentities(this.m_requestContext.Elevate(), (IList<IdentityDescriptor>) descriptors.ToList<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (x => x.Descriptor), (Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id), (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        Guid guid;
        this.EnumerateQueryReferencedIdentities((Func<IdentityDescriptor, Guid>) (descriptor => idMap.TryGetValue(descriptor, out guid) ? guid : this.m_requestContext.GetService<LocalSecurityService>().EnsureIdentityIsKnown(this.m_requestContext.Elevate(), descriptor).Id));
      }
      Dictionary<Guid, ServerQueryItem> queryItems = new Dictionary<Guid, ServerQueryItem>();
      Guid parentId1;
      foreach (ServerQueryItem serverQueryItem1 in this.m_activeQueryItems.Values)
      {
        if (serverQueryItem1.Action == PersistenceAction.Insert)
        {
          ServerQueryItem serverQueryItem2;
          for (Guid guid = serverQueryItem1.New.ParentId; !guid.Equals(Guid.Empty); guid = serverQueryItem2 != null ? serverQueryItem2.New.ParentId : Guid.Empty)
          {
            if (queryItems.TryGetValue(guid, out serverQueryItem2))
            {
              if (serverQueryItem1.New.ParentId == guid)
                serverQueryItem1.New.Parent = serverQueryItem2;
              serverQueryItem1.New.NearestPersistedParent = serverQueryItem2;
              break;
            }
            if (!this.m_activeQueryItems.TryGetValue(guid, out serverQueryItem2) || serverQueryItem2.Action == PersistenceAction.Update)
            {
              if (serverQueryItem2 == null)
                serverQueryItem2 = new ServerQueryItem(guid);
              if (serverQueryItem1.New.ParentId == guid)
                serverQueryItem1.New.Parent = serverQueryItem2;
              serverQueryItem1.New.NearestPersistedParent = serverQueryItem2;
              queryItems.Add(guid, serverQueryItem2);
              break;
            }
            if (serverQueryItem1.New.ParentId == guid)
              serverQueryItem1.New.Parent = serverQueryItem2;
            if (serverQueryItem2 != null && serverQueryItem1.Id == serverQueryItem2.New.ParentId)
              throw new LegacyValidationException(DalResourceStrings.Get("UserInputLoop"), 600295);
          }
          parentId1 = serverQueryItem1.New.ParentId;
          if (parentId1.Equals(Guid.Empty))
          {
            bool? isPublic = serverQueryItem1.New.IsPublic;
            if (isPublic.HasValue)
            {
              isPublic = serverQueryItem1.New.IsPublic;
              if (isPublic.Value)
                queryItems.Add(serverQueryItem1.Id, serverQueryItem1);
            }
          }
        }
        else
        {
          if (!queryItems.ContainsKey(serverQueryItem1.Id))
            queryItems.Add(serverQueryItem1.Id, serverQueryItem1);
          if (serverQueryItem1.Action == PersistenceAction.Update)
          {
            parentId1 = serverQueryItem1.New.ParentId;
            if (!parentId1.Equals(Guid.Empty))
            {
              ServerQueryItem serverQueryItem3;
              if (!queryItems.TryGetValue(serverQueryItem1.New.ParentId, out serverQueryItem3))
              {
                if (!this.m_activeQueryItems.TryGetValue(serverQueryItem1.New.ParentId, out serverQueryItem3))
                  serverQueryItem3 = new ServerQueryItem(serverQueryItem1.New.ParentId);
                if (serverQueryItem3.Action != PersistenceAction.Insert)
                  queryItems.Add(serverQueryItem1.New.ParentId, serverQueryItem3);
              }
              serverQueryItem1.New.Parent = serverQueryItem3;
              serverQueryItem1.New.NearestPersistedParent = serverQueryItem3;
            }
          }
        }
      }
      foreach (ServerQueryItem serverQueryItem in this.m_deletedQueryItems.Values)
      {
        if (!queryItems.ContainsKey(serverQueryItem.Id))
          queryItems.Add(serverQueryItem.Id, serverQueryItem);
      }
      QueryItemMethods.PopulateSecurityInfo(this.m_requestContext, queryItems);
      Guid parentId2;
      bool? isPublic1;
      foreach (ServerQueryItem serverQueryItem in this.m_activeQueryItems.Values)
      {
        if (serverQueryItem.Action == PersistenceAction.Insert)
        {
          if (serverQueryItem.New.NearestPersistedParent == null || serverQueryItem.New.NearestPersistedParent.SecurityToken == null)
          {
            parentId2 = serverQueryItem.New.ParentId;
            if (parentId2.Equals(Guid.Empty))
            {
              isPublic1 = serverQueryItem.New.IsPublic;
              if (isPublic1.HasValue)
              {
                isPublic1 = serverQueryItem.New.IsPublic;
                if (!isPublic1.Value)
                {
                  QueryItemMethods.CheckPermission(this.m_requestContext, serverQueryItem, 2);
                  continue;
                }
              }
            }
            throw new LegacyValidationException(this.m_requestContext, InternalsResourceStrings.Manager, 600292, (string) null, "QueryHierarchyParentDoesNotExist", (object[]) null);
          }
          QueryItemMethods.CheckPermission(this.m_requestContext, serverQueryItem.New.NearestPersistedParent, 2);
        }
        else if (serverQueryItem.Action == PersistenceAction.Update)
        {
          parentId2 = serverQueryItem.New.ParentId;
          if (parentId2.Equals(Guid.Empty) && serverQueryItem.New.Parent == null)
          {
            isPublic1 = serverQueryItem.New.IsPublic;
            if (isPublic1.HasValue)
            {
              isPublic1 = serverQueryItem.New.IsPublic;
              int num1 = isPublic1.Value ? 1 : 0;
              isPublic1 = serverQueryItem.Existing.IsPublic;
              int num2 = isPublic1.Value ? 1 : 0;
              if (num1 != num2)
              {
                isPublic1 = serverQueryItem.New.IsPublic;
                if (isPublic1.Value)
                {
                  QueryItemMethods.PopulateSecurityInfo(this.m_requestContext, serverQueryItem);
                }
                else
                {
                  serverQueryItem.New.Parent = new ServerQueryItem();
                  serverQueryItem.New.Parent.Existing.IsFolder = true;
                  Details existing = serverQueryItem.New.Parent.Existing;
                  isPublic1 = serverQueryItem.New.IsPublic;
                  bool? nullable = new bool?(isPublic1.Value);
                  existing.IsPublic = nullable;
                  serverQueryItem.New.Parent.Existing.ProjectId = serverQueryItem.Existing.ProjectId;
                  serverQueryItem.New.Parent.Existing.QueryName = "PrivateQueries";
                  serverQueryItem.New.Parent.Existing.Owner = !(serverQueryItem.New.Owner != (IdentityDescriptor) null) || serverQueryItem.New.Owner.Identifier == null ? this.m_requestContext.UserContext : serverQueryItem.New.Owner;
                }
              }
            }
          }
          parentId2 = serverQueryItem.New.ParentId;
          if (!parentId2.Equals(Guid.Empty))
          {
            parentId2 = serverQueryItem.New.ParentId;
            if (!parentId2.Equals(serverQueryItem.Existing.ParentId))
            {
              QueryItemMethods.CheckPermission(this.m_requestContext, serverQueryItem, 4);
              if (serverQueryItem.New.Parent == null)
                throw new LegacyValidationException(this.m_requestContext, InternalsResourceStrings.Manager, 600292, (string) null, "QueryHierarchyParentDoesNotExist", (object[]) null);
              QueryItemMethods.CheckPermission(this.m_requestContext, serverQueryItem.New.Parent, 2);
              QueryItemMethods.CheckPermissionForAllChildren(this.m_requestContext, serverQueryItem, 1);
            }
          }
          if (serverQueryItem.New.Owner != (IdentityDescriptor) null && !string.IsNullOrEmpty(serverQueryItem.New.Owner.Identifier) && !IdentityDescriptorComparer.Instance.Equals(serverQueryItem.New.Owner, serverQueryItem.Existing.Owner))
            QueryItemMethods.CheckPermission(this.m_requestContext, serverQueryItem, 8);
          QueryItemMethods.CheckPermission(this.m_requestContext, serverQueryItem, 2);
        }
      }
      foreach (ServerQueryItem serverQueryItem in this.m_deletedQueryItems.Values)
        QueryItemMethods.CheckPermissionForAllChildren(this.m_requestContext, serverQueryItem, 4);
      foreach (ServerQueryItem serverQueryItem in this.m_activeQueryItems.Values)
      {
        if (serverQueryItem.AccessControlList != null)
        {
          bool? isPublic2 = serverQueryItem.Existing.IsPublic;
          if (isPublic2.HasValue)
          {
            isPublic2 = serverQueryItem.Existing.IsPublic;
            if (!isPublic2.Value)
            {
              isPublic2 = serverQueryItem.New.IsPublic;
              if (isPublic2.HasValue)
              {
                isPublic2 = serverQueryItem.New.IsPublic;
                if (isPublic2.Value)
                  goto label_84;
              }
              IVssRequestContext requestContext = this.m_requestContext;
              throw new LegacyValidationException(requestContext, DalResourceStrings.Manager, 600288, "AccessException", "QueryHierarchyItemAccessException", new object[3]
              {
                (object) requestContext.DomainUserName,
                (object) QueryItemMethods.GetPermissionDisplayStrings(requestContext, serverQueryItem, 8),
                (object) serverQueryItem.Existing.QueryName
              });
            }
          }
label_84:
          QueryItemMethods.CheckPermission(this.m_requestContext, serverQueryItem, 8);
          QueryItemMethods.ValidatePermissions(this.m_requestContext, serverQueryItem, serverQueryItem.AccessControlList);
        }
      }
      foreach (ServerQueryItem serverQueryItem in this.m_activeQueryItems.Values)
      {
        if (serverQueryItem.Action == PersistenceAction.Update)
        {
          bool? isPublic3 = serverQueryItem.Existing.IsPublic;
          if (!isPublic3.Value)
          {
            isPublic3 = serverQueryItem.New.IsPublic;
            if (isPublic3.HasValue)
            {
              isPublic3 = serverQueryItem.New.IsPublic;
              if (isPublic3.Value)
              {
                serverQueryItem.New.Owner = this.m_requestContext.UserContext;
                serverQueryItem.New.OwnerTeamFoundationId = this.RequestContextIdentity.Id;
              }
            }
          }
        }
      }
      bool backCompat = false;
      foreach (Guid key in this.m_activeQueryItems.Keys)
      {
        this.ExplodeQueryUpdates(key);
        backCompat |= this.m_activeQueryItems[key].IsBackCompat;
      }
      foreach (ServerQueryItem serverQueryItem in this.m_deletedQueryItems.Values)
      {
        DalSqlElement.GetElement<DalDeleteQueryItemElement>(this.m_sqlBatch).JoinBatch(this.ChangeQueryGroup, serverQueryItem);
        backCompat |= serverQueryItem.IsBackCompat;
      }
      DalSqlElement.GetElement<DalAuthorizeQueryItemsElement>(this.m_sqlBatch).JoinBatch(this.ChangeQueryGroup, this.m_changedDate, backCompat);
    }

    private void EnumerateQueryReferencedIdentities(Func<IdentityDescriptor, Guid> callback)
    {
      foreach (ServerQueryItem serverQueryItem in this.m_activeQueryItems.Values)
      {
        if ((serverQueryItem.Action == PersistenceAction.Insert || serverQueryItem.Action == PersistenceAction.Update) && serverQueryItem.New.OwnerTeamFoundationId == Guid.Empty && serverQueryItem.New.Owner != (IdentityDescriptor) null && !string.IsNullOrEmpty(serverQueryItem.New.Owner.Identifier))
        {
          Guid guid = callback(serverQueryItem.New.Owner);
          if (guid != Guid.Empty)
            serverQueryItem.New.OwnerTeamFoundationId = guid;
        }
        if (serverQueryItem.AccessControlList != null)
        {
          foreach (AccessControlEntryData permission in serverQueryItem.AccessControlList.Permissions)
          {
            Guid guid = callback(permission.Descriptor);
            if (guid != Guid.Empty)
              permission.TeamFoundationId = guid;
          }
        }
      }
    }

    private void ExplodeQueryUpdates(Guid id)
    {
      ServerQueryItem serverQueryItem;
      if (!this.m_activeQueryItems.TryGetValue(id, out serverQueryItem) || serverQueryItem.HasFinishedProcessing)
        return;
      serverQueryItem.HasStartedProcessing = !serverQueryItem.HasStartedProcessing ? true : throw new LegacyValidationException(DalResourceStrings.Get("UserInputLoop"), 600295);
      this.ExplodeQueryUpdates(serverQueryItem.New.ParentId);
      if (serverQueryItem.Action == PersistenceAction.Insert)
        DalSqlElement.GetElement<DalInsertQueryItemElement>(this.m_sqlBatch).JoinBatch(this.ChangeQueryGroup, serverQueryItem, this.m_requestContext);
      else if (serverQueryItem.Action == PersistenceAction.Update)
        DalSqlElement.GetElement<DalUpdateQueryItemElement>(this.m_sqlBatch).JoinBatch(this.ChangeQueryGroup, serverQueryItem, this.m_requestContext);
      if (serverQueryItem.AccessControlList != null)
        DalSqlElement.GetElement<DalSetQueryItemAclElement>(this.m_sqlBatch).JoinBatch(this.ChangeQueryGroup, serverQueryItem, serverQueryItem.AccessControlList);
      if (serverQueryItem.Action == PersistenceAction.Insert || serverQueryItem.Action == PersistenceAction.Update)
        DalSqlElement.GetElement<DalUpdateQueryItemHashElement>(this.m_sqlBatch).JoinBatch(this.ChangeQueryGroup, serverQueryItem, this.m_requestContext);
      serverQueryItem.HasFinishedProcessing = true;
    }

    private void DestroyIssue(XmlNode actionNode)
    {
      XmlAttribute namedItem = (XmlAttribute) actionNode.Attributes.GetNamedItem("WorkItemID");
      if (namedItem == null)
        throw new ArgumentException(DalResourceStrings.Format("MissingAttributeInXmlException", (object) "WorkItemID"));
      int result;
      if (!int.TryParse(namedItem.Value.Trim(), out result))
        throw new ArgumentException(DalResourceStrings.Get("UpdateInvalidAttributeIntegerException"), "updateElement");
      if (this.m_destroyWorkItems == null)
        this.m_destroyWorkItems = new List<int>();
      this.m_destroyWorkItems.Add(result);
    }

    private void AddAction(
      XmlNode actionNode,
      XmlNode outputNode,
      bool verbose,
      bool provisionRules)
    {
      this.m_requestContext.TraceEnter(900309, nameof (Update), nameof (Update), nameof (AddAction));
      BaseUpdate baseUpdate = (BaseUpdate) null;
      if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "DestroyWorkItem"))
        this.DestroyIssue(actionNode);
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertQuery") || TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateQuery") || TFStringComparer.UpdateAction.Equals(actionNode.Name, "DeleteQuery") || TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertQueryItem") || TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateQueryItem") || TFStringComparer.UpdateAction.Equals(actionNode.Name, "DeleteQueryItem"))
      {
        ServerQueryItem queryUpdateDetails = this.GetQueryUpdateDetails(actionNode);
        if (queryUpdateDetails.Action == PersistenceAction.Delete)
        {
          this.m_deletedQueryItems.Add(queryUpdateDetails.Id, queryUpdateDetails);
        }
        else
        {
          if (this.m_activeQueryItems.ContainsKey(queryUpdateDetails.Id))
            throw new ArgumentException("Duplicate Query Item ID used!");
          this.m_activeQueryItems.Add(queryUpdateDetails.Id, queryUpdateDetails);
        }
        XmlDocument ownerDocument = outputNode.OwnerDocument;
        XmlAttribute attribute1 = ownerDocument.CreateAttribute("QueryID");
        attribute1.Value = queryUpdateDetails.Id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
        outputNode.Attributes.Append(attribute1);
        XmlAttribute attribute2 = ownerDocument.CreateAttribute("UpdateTime");
        attribute2.Value = queryUpdateDetails.UpdateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
        outputNode.Attributes.Append(attribute2);
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "SetQueryItemAccessControlList"))
      {
        ServerQueryItem aclUpdateDetails = this.GetQueryAclUpdateDetails(actionNode, this.m_activeQueryItems, this.m_deletedQueryItems);
        XmlDocument ownerDocument = outputNode.OwnerDocument;
        XmlAttribute attribute3 = ownerDocument.CreateAttribute("QueryID");
        attribute3.Value = aclUpdateDetails.Id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
        outputNode.Attributes.Append(attribute3);
        XmlAttribute attribute4 = ownerDocument.CreateAttribute("UpdateTime");
        attribute4.Value = aclUpdateDetails.UpdateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
        outputNode.Attributes.Append(attribute4);
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertField"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertField();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateField"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.UpdateField();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "DeleteField"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.DeleteField();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertFieldUsage"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertFieldUsage();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateFieldUsage"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.UpdateFieldUsage();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertTreeProperty"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertTreeProperty();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateTreeProperty"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.UpdateTreeProperty();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertConstant"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertConstant();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertRule"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertRule();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateRule"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.UpdateRule();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertConstantSet"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertConstantSet(provisionRules);
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateConstantSet"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.UpdateConstantSet(provisionRules);
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "DestroyGlobalList"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.DestroyGlobalList();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertWorkItemType"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertWorkItemType();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateWorkItemType"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.UpdateWorkItemType();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "DestroyWorkItemType"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.DestroyWorkItemType();
        baseUpdate = (BaseUpdate) adminUpdate;
        this.m_destroyedWorkItemType = true;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertWorkItemTypeUsage"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertWorkItemTypeUsage();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateWorkItemTypeUsage"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.UpdateWorkItemTypeUsage();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertWorkItemTypeCategory"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertWorkItemTypeCategory();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateWorkItemTypeCategory"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.UpdateWorkItemTypeCategory();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "DestroyWorkItemTypeCategory"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.DestroyWorkItemTypeCategory();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertWorkItemTypeCategoryMember"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertWorkItemTypeCategoryMember();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "DeleteWorkItemTypeCategoryMember"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.DeleteWorkItemTypeCategoryMember();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertAction"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertAction();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateAction"))
      {
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.UpdateAction();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertWorkItemLinkType"))
      {
        this.CheckLinkTypePermissions(true);
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.InsertLinkType();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateWorkItemLinkType"))
      {
        this.CheckLinkTypePermissions(false);
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.UpdateLinkType();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "DeleteWorkItemLinkType"))
      {
        this.CheckLinkTypePermissions(false);
        AdminUpdate adminUpdate = new AdminUpdate(this.m_sqlBatch, actionNode, this, outputNode, this.m_user, verbose, this.m_requestContext);
        adminUpdate.DeleteLinkType();
        baseUpdate = (BaseUpdate) adminUpdate;
      }
      else
        throw new ArgumentException(DalResourceStrings.Format("UpdateActionNotSupportedException", (object) actionNode.Name));
      if (baseUpdate != null)
        this.m_updateList.Add(baseUpdate);
      this.m_requestContext.TraceLeave(900582, nameof (Update), nameof (Update), nameof (AddAction));
    }

    private void CheckLinkTypePermissions(bool forInsert)
    {
      if (this.m_linkTypePermissions == Update.LinkTypePermissions.Unchecked)
      {
        this.m_linkTypePermissions = Update.LinkTypePermissions.None;
        if (this.m_requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.m_requestContext, WitProvisionSecurity.NamespaceId).HasPermission(this.m_requestContext, "ManageLinkTypes", 2))
          this.m_linkTypePermissions = Update.LinkTypePermissions.FullControl;
        else if (this.m_requestContext.GetExtension<IAuthorizationProviderFactory>().IsPermitted(this.m_requestContext, PermissionNamespaces.Global, "CREATE_PROJECTS", this.RequestContextIdentity.Descriptor))
          this.m_linkTypePermissions = Update.LinkTypePermissions.InsertOnly;
      }
      if (this.m_linkTypePermissions != Update.LinkTypePermissions.FullControl && !(this.m_linkTypePermissions == Update.LinkTypePermissions.InsertOnly & forInsert))
        throw new LegacyProvisionPermissionsException(DalResourceStrings.Format("ManageLinkTypesDenied", (object) this.m_requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.m_requestContext, WitProvisionSecurity.NamespaceId).Description.GetLocalizedActions(2).FirstOrDefault<string>()));
    }

    public int GetNextActionId()
    {
      this.m_requestContext.TraceEnter(900310, nameof (Update), nameof (Update), nameof (GetNextActionId));
      try
      {
        ++this.m_lastActionId;
        bool flag = false;
        while (!flag)
        {
          if (this.m_reservedTempIds.Contains(this.m_lastActionId))
            ++this.m_lastActionId;
          else
            flag = true;
        }
        return this.m_lastActionId;
      }
      finally
      {
        this.m_requestContext.TraceLeave(900583, nameof (Update), nameof (Update), nameof (GetNextActionId));
      }
    }

    public int GetLinkChangeOrder() => ++this.m_linkChangeOrder;

    public void CheckTempId(int tempId)
    {
      if (!this.m_reservedTempIds.Contains(tempId))
        throw new ArgumentException(DalResourceStrings.Format("UpdateTempIdNotDefined", (object) tempId), "updateElement");
    }

    internal void AddWorkItemIdToBatch(int id) => this.m_workItemIds.Add(id);

    private void AddGroupToBatch(ElementGroup group)
    {
      this.m_requestContext.TraceEnter(900311, nameof (Update), nameof (Update), nameof (AddGroupToBatch));
      if (group == null)
        return;
      for (int index = 0; index < group.ElementCount; ++index)
      {
        this.m_sqlBatch.AppendSql(group.GetSql(index));
        int outputIndex = this.m_sqlBatch.AddExpectedReturnedDataTables(group.GetExpectedOutputs(index));
        group.PutOutputIndex(index, outputIndex);
      }
      this.m_requestContext.TraceLeave(900584, nameof (Update), nameof (Update), nameof (AddGroupToBatch));
    }

    public void GetResults(
      IVssRequestContext requestContext,
      Payload metadataPayload,
      out string dbStamp)
    {
      requestContext.TraceEnter(900312, nameof (Update), nameof (Update), nameof (GetResults));
      this.m_serverComputedValues = new Dictionary<string, object>();
      if (this.m_applyIssueChanges != null && this.m_applyIssueChanges.IsNeeded)
      {
        PayloadTable.PayloadRow row = this.m_applyIssueChanges.GetResultTable().Rows[0];
        PayloadColumnCollection columns = row.Table.Columns;
        for (int index = columns.Count - 1; index >= 0; --index)
        {
          PayloadColumn payloadColumn = columns[index];
          this.m_serverComputedValues[payloadColumn.Name] = row[payloadColumn.Index];
        }
      }
      else if (this.m_getUtcGroup.ElementCount > 0)
        this.m_serverComputedValues["System.AuthorizedDate"] = (object) (DateTime) this.m_sqlBatch.ResultPayload.Tables[this.m_getUtcGroup.GetOutputIndex(0)].Rows[0][0];
      for (int index = 0; index < this.m_updateList.Count; ++index)
      {
        BaseUpdate update = this.m_updateList[index];
        if (!update.IsDummy)
          update.GenerateOutput();
      }
      if (this.IsLinkTypeBatch)
      {
        this.m_requestContext.ResetMetadataDbStamps();
        this.m_requestContext.GetService<WorkItemTrackingLinkService>().InvalidateCache(this.m_requestContext);
      }
      if (this.m_changeWorkItemTypeGroup.ElementCount > 0)
      {
        this.m_requestContext.ResetMetadataDbStamps();
        requestContext.GetService<LegacyWorkItemTypeDictionary>().InvalidateCache(this.m_requestContext);
      }
      if (this.IsQueryItemBatch)
      {
        WorkItemTrackingService service = this.m_requestContext.GetService<WorkItemTrackingService>();
        if (this.m_deletedQueryItems.Any<KeyValuePair<Guid, ServerQueryItem>>())
          service.OnQueryItemsDeleted(requestContext, (IEnumerable<ServerQueryItem>) this.m_deletedQueryItems.Values);
        service.OnQueryItemsChanged(requestContext, Guid.Empty, string.Empty);
      }
      if (this.m_metadataChanged)
        this.m_requestContext.ResetMetadataDbStamps();
      this.TraceProjectWriteViewsRebuild();
      AdminUpdate[] adminUpdates = this.m_updateList.OfType<AdminUpdate>().ToArray<AdminUpdate>();
      if (((IEnumerable<AdminUpdate>) adminUpdates).Any<AdminUpdate>())
      {
        requestContext.TraceBlock(901340, 901349, 901348, nameof (Update), nameof (Update), "GetResults.ExtensionReconciliation", (Action) (() =>
        {
          WorkItemTypeCategoryUpdateEventData[] array1 = ((IEnumerable<AdminUpdate>) adminUpdates).SelectMany<AdminUpdate, WorkItemTypeCategoryUpdateEventData>((Func<AdminUpdate, IEnumerable<WorkItemTypeCategoryUpdateEventData>>) (update => update.Operations.OfType<WorkItemTypeCategoryUpdateEventData>())).ToArray<WorkItemTypeCategoryUpdateEventData>();
          if (((IEnumerable<WorkItemTypeCategoryUpdateEventData>) array1).Any<WorkItemTypeCategoryUpdateEventData>())
          {
            int num = (int) this.m_requestContext.GetService<WorkItemTypeExtensionService>().ReconcileExtensions(this.m_requestContext, (IEnumerable<WorkItemTypeCategoryUpdateEventData>) array1);
          }
          WorkItemTypeRenameEventData[] array2 = ((IEnumerable<AdminUpdate>) adminUpdates).SelectMany<AdminUpdate, WorkItemTypeRenameEventData>((Func<AdminUpdate, IEnumerable<WorkItemTypeRenameEventData>>) (update => update.Operations.OfType<WorkItemTypeRenameEventData>())).ToArray<WorkItemTypeRenameEventData>();
          if (!((IEnumerable<WorkItemTypeRenameEventData>) array2).Any<WorkItemTypeRenameEventData>())
            return;
          this.m_requestContext.GetService<WorkItemTypeExtensionService>().ReconcileExtensions(this.m_requestContext, (IEnumerable<WorkItemTypeRenameEventData>) array2);
        }));
        requestContext.TraceBlock(901920, 901929, 901928, "Queries", "Query", "GetResults.UpdateQueryPersonReferences_FieldChange", (Action) (() =>
        {
          FieldTypeUpdateEventData[] array = ((IEnumerable<AdminUpdate>) adminUpdates).SelectMany<AdminUpdate, FieldTypeUpdateEventData>((Func<AdminUpdate, IEnumerable<FieldTypeUpdateEventData>>) (update => update.Operations.OfType<FieldTypeUpdateEventData>())).ToArray<FieldTypeUpdateEventData>();
          if (!((IEnumerable<FieldTypeUpdateEventData>) array).Any<FieldTypeUpdateEventData>())
            return;
          WiqlTransformUtils.UpdateQueryTexts(this.m_requestContext, array);
        }));
      }
      this.m_fieldsSnapshot = (IFieldTypeDictionary) null;
      if (this.m_bisNotification)
      {
        Update.EventTaskInfo taskArgs = new Update.EventTaskInfo();
        taskArgs.MetadataChanged = this.m_metadataChanged;
        try
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          this.m_requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(this.m_requestContext, new TeamFoundationTask(Update.\u003C\u003EO.\u003C0\u003E__FireEventThreadProc ?? (Update.\u003C\u003EO.\u003C0\u003E__FireEventThreadProc = new TeamFoundationTaskCallback(Update.FireEventThreadProc)), (object) taskArgs, 0));
        }
        catch (Exception ex)
        {
          requestContext.Trace(900530, TraceLevel.Error, nameof (Update), nameof (Update), ex.ToString());
          TeamFoundationEventLog.Default.LogException(this.m_requestContext, DalResourceStrings.Format("FireWorkItemEventFailedToQueueEvent", (object) ex.Message), ex, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
        }
      }
      if (metadataPayload != null)
        this.m_getMetadataElement.GetResults(metadataPayload);
      dbStamp = this.m_getDbStampElement.GetDbStamp();
      requestContext.TraceLeave(900531, nameof (Update), nameof (Update), nameof (GetResults));
    }

    private void TraceProjectWriteViewsRebuild()
    {
      if (this.m_sqlBatch.Version < 21 || this.m_checkChanges == null)
        return;
      object obj = this.m_checkChanges.GetResultTable(this.m_checkChanges.GetOutputs() - 1).Rows[0][0];
      int num;
      if (obj == null || (num = (int) obj) <= 0)
        return;
      this.RequestContext.Trace(900748, TraceLevel.Info, nameof (Update), nameof (Update), "Write views of {0} project(s) have been rebuilt.", (object) num);
    }

    private static void FireEventThreadProc(IVssRequestContext requestContext, object stateInfo)
    {
      Update.EventTaskInfo eventTaskInfo = (Update.EventTaskInfo) stateInfo;
      try
      {
        if (!eventTaskInfo.MetadataChanged)
          return;
        TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
        if (!eventTaskInfo.MetadataChanged)
          return;
        service.PublishNotification(requestContext, (object) new WorkItemMetadataChangedNotification());
      }
      catch (Exception ex)
      {
        requestContext.Trace(900532, TraceLevel.Error, nameof (Update), nameof (Update), ex.ToString());
        TeamFoundationEventLog.Default.LogException(requestContext, DalResourceStrings.Format("FireWorkItemEventFailed", (object) ex.Message), ex, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
      }
    }

    public void GetBulkUpdateFailResults(out string dbStamp)
    {
      this.m_requestContext.TraceEnter(900313, nameof (Update), nameof (Update), nameof (GetBulkUpdateFailResults));
      int outputRowSetIndex = this.m_checkChanges.GetOutputRowSetIndex();
      if (outputRowSetIndex >= 0)
      {
        int index = this.m_verboseOn ? outputRowSetIndex + 1 : outputRowSetIndex;
        List<int> idList = new List<int>();
        foreach (int workItemId in this.m_workItemIds)
        {
          int outputId = this.GetOutputId(workItemId);
          idList.Add(outputId > 0 ? outputId : workItemId);
        }
        this.AddFailedIdsToOutput(idList, this.m_sqlBatch.ResultPayload.Tables[index].Rows.Select<PayloadTable.PayloadRow, int>((Func<PayloadTable.PayloadRow, int>) (r => (int) r[0])), 600139);
      }
      dbStamp = this.m_getDbStampElement.GetDbStamp();
      this.TraceProjectWriteViewsRebuild();
      this.m_requestContext.TraceLeave(900585, nameof (Update), nameof (Update), nameof (GetBulkUpdateFailResults));
    }

    private void AddFailedIdsToOutput(List<int> idList, IEnumerable<int> failedIds, int errorId)
    {
      foreach (int failedId in failedIds)
      {
        XmlNode xmlNode = this.m_outputNode.AppendChild((XmlNode) this.m_outputNode.OwnerDocument.CreateElement("FailedWorkItem"));
        XmlAttribute attribute1 = xmlNode.OwnerDocument.CreateAttribute("BatchIndex");
        attribute1.Value = idList.IndexOf(failedId).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xmlNode.Attributes.Append(attribute1);
        XmlAttribute attribute2 = xmlNode.OwnerDocument.CreateAttribute("ErrorCode");
        attribute2.Value = errorId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xmlNode.Attributes.Append(attribute2);
      }
    }

    internal int GetOutputId(int id) => this.GetTempIdMapElement.GetOutputId(id);

    private ServerQueryItem GetQueryUpdateDetails(XmlNode actionNode)
    {
      this.m_requestContext.TraceEnter(900314, nameof (Update), nameof (Update), nameof (GetQueryUpdateDetails));
      ServerQueryItem queryUpdateDetails = new ServerQueryItem();
      if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertQuery") || TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertQueryItem"))
        queryUpdateDetails.Action = PersistenceAction.Insert;
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateQuery") || TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateQueryItem"))
        queryUpdateDetails.Action = PersistenceAction.Update;
      else if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "DeleteQuery") || TFStringComparer.UpdateAction.Equals(actionNode.Name, "DeleteQueryItem"))
        queryUpdateDetails.Action = PersistenceAction.Delete;
      queryUpdateDetails.UpdateTime = this.m_changedDate;
      string identifier = (string) null;
      string identityType = "System.Security.Principal.WindowsIdentity";
      bool flag1 = false;
      foreach (XmlNode childNode in actionNode.ChildNodes)
      {
        if (childNode.NodeType == XmlNodeType.Element)
        {
          if (VssStringComparer.XmlNodeName.Equals(childNode.Name, "Name"))
          {
            queryUpdateDetails.New.QueryName = !string.IsNullOrEmpty(childNode.InnerText) ? childNode.InnerText : throw new LegacyValidationException(this.m_requestContext, DalResourceStrings.Manager, 600029, (string) null, "QueryHierarchyNameCannotBeEmpty", (object[]) null);
            flag1 = true;
          }
          else if (VssStringComparer.XmlNodeName.Equals(childNode.Name, "ProjectID"))
          {
            queryUpdateDetails.New.ProjectId = Convert.ToInt32(childNode.InnerText, (IFormatProvider) CultureInfo.InvariantCulture);
            flag1 = true;
          }
          else if (VssStringComparer.XmlNodeName.Equals(childNode.Name, "QueryText"))
          {
            queryUpdateDetails.New.QueryText = !string.IsNullOrEmpty(childNode.InnerText) ? childNode.InnerText : throw new LegacyValidationException(this.m_requestContext, DalResourceStrings.Manager, 600030, (string) null, "QueryHierarchyQueryTextCannotBeEmpty", (object[]) null);
            flag1 = true;
          }
          else if (VssStringComparer.XmlNodeName.Equals(childNode.Name, "Description"))
          {
            queryUpdateDetails.New.Description = childNode.InnerXml;
            flag1 = true;
          }
          else if (VssStringComparer.XmlNodeName.Equals(childNode.Name, "IsPublic"))
          {
            queryUpdateDetails.New.IsPublic = Convert.ToInt32(childNode.InnerText, (IFormatProvider) CultureInfo.InvariantCulture) != 0 ? new bool?(true) : new bool?(false);
            flag1 = true;
          }
          else if (VssStringComparer.XmlNodeName.Equals(childNode.Name, "OwnerIdentifier"))
            identifier = childNode.InnerXml;
          else if (VssStringComparer.XmlNodeName.Equals(childNode.Name, "OwnerType"))
            identityType = childNode.InnerXml;
        }
      }
      bool flag2 = false;
      bool flag3 = !string.IsNullOrEmpty(identifier);
      if (((queryUpdateDetails.Action != PersistenceAction.Update ? 0 : (!flag1 ? 1 : 0)) & (flag3 ? 1 : 0)) != 0)
        flag2 = true;
      else if (queryUpdateDetails.Action != PersistenceAction.Update & flag3)
        flag2 = true;
      if (flag2)
        queryUpdateDetails.New.Owner = new IdentityDescriptor(identityType, identifier);
      queryUpdateDetails.IsBackCompat = !TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertQueryItem") && !TFStringComparer.UpdateAction.Equals(actionNode.Name, "UpdateQueryItem") && !TFStringComparer.UpdateAction.Equals(actionNode.Name, "DeleteQueryItem");
      if (TFStringComparer.UpdateAction.Equals(actionNode.Name, "InsertQuery") && queryUpdateDetails.New.ProjectId > 0)
      {
        queryUpdateDetails.Id = Guid.NewGuid();
      }
      else
      {
        XmlAttribute namedItem1 = (XmlAttribute) actionNode.Attributes.GetNamedItem("QueryID");
        if (namedItem1 == null)
          throw new LegacyValidationException(this.m_requestContext, DalResourceStrings.Manager, 600032, (string) null, "ErrorInvalidQueryId", (object[]) null);
        try
        {
          queryUpdateDetails.Id = XmlConvert.ToGuid(namedItem1.Value);
        }
        catch
        {
          throw new LegacyValidationException(this.m_requestContext, DalResourceStrings.Manager, 600032, (string) null, "ErrorInvalidQueryId", (object[]) null);
        }
        XmlAttribute namedItem2 = (XmlAttribute) actionNode.Attributes.GetNamedItem("QueryParentID");
        if (queryUpdateDetails.Action == PersistenceAction.Insert && namedItem2 == null)
          throw new LegacyValidationException("TODO: Invalid Query Parent ID", 600032, (Exception) null);
        if (namedItem2 != null)
        {
          if (queryUpdateDetails.New.ProjectId > 0)
            throw new ArgumentException("TODO: Ambiguous scope");
          try
          {
            queryUpdateDetails.New.ParentId = XmlConvert.ToGuid(namedItem2.Value);
          }
          catch
          {
            throw new LegacyValidationException("TODO: Invalid Query Parent ID", 600032, (Exception) null);
          }
        }
      }
      if (string.IsNullOrEmpty(queryUpdateDetails.New.QueryName) && queryUpdateDetails.Action == PersistenceAction.Insert)
        throw new LegacyValidationException(this.m_requestContext, DalResourceStrings.Manager, 600029, (string) null, "QueryHierarchyNameCannotBeEmpty", (object[]) null);
      if (!string.IsNullOrEmpty(queryUpdateDetails.New.QueryName))
      {
        string str = queryUpdateDetails.New.QueryName;
        int length = str.Length;
        int num1 = str.IndexOf('«');
        int num2 = str.LastIndexOf('«');
        int num3 = str.IndexOf('»');
        int num4 = str.LastIndexOf('»');
        if (num1 < 0 && num3 >= 0 || num1 >= 0 && num3 < 0 || num1 != num2 || num3 != num4 || num3 < num1 || num1 > 0 || num3 == length - 1)
          throw new LegacyValidationException(this.m_requestContext, InternalsResourceStrings.Manager, 600029, (string) null, "ErrorInvalidQueryName", (object[]) null);
        if (num1 == 0 && num3 > 0)
          str = str.Substring(num3 + 1, length - num3 - 1).Trim();
        queryUpdateDetails.New.QueryName = str.Length <= (int) byte.MaxValue && str.IndexOf('⁄') < 0 && str.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 ? str : throw new LegacyValidationException(this.m_requestContext, InternalsResourceStrings.Manager, 600029, (string) null, "ErrorInvalidQueryName", (object[]) null);
      }
      this.m_requestContext.TraceLeave(900315, nameof (Update), nameof (Update), nameof (GetQueryUpdateDetails));
      return queryUpdateDetails;
    }

    private ServerQueryItem GetQueryAclUpdateDetails(
      XmlNode actionNode,
      Dictionary<Guid, ServerQueryItem> activeQueryItems,
      Dictionary<Guid, ServerQueryItem> deletedQueryItems)
    {
      this.m_requestContext.TraceEnter(900316, nameof (Update), nameof (Update), nameof (GetQueryAclUpdateDetails));
      Guid guidValue;
      BaseUpdate.GetAttributeGuid(actionNode, "QueryID", true, Guid.Empty, out guidValue);
      if (deletedQueryItems.ContainsKey(guidValue))
        return deletedQueryItems[guidValue];
      ServerQueryItem aclUpdateDetails;
      if (!activeQueryItems.TryGetValue(guidValue, out aclUpdateDetails))
      {
        aclUpdateDetails = new ServerQueryItem(guidValue);
        activeQueryItems.Add(aclUpdateDetails.Id, aclUpdateDetails);
      }
      aclUpdateDetails.UpdateTime = this.m_changedDate;
      if (aclUpdateDetails.AccessControlList != null)
        throw new ArgumentException("TODO: Multiple access control lists passed for same Query Item ID!");
      ExtendedAccessControlListData accessControlListData = new ExtendedAccessControlListData();
      aclUpdateDetails.AccessControlList = accessControlListData;
      accessControlListData.Token = guidValue.ToString();
      bool outValue;
      BaseUpdate.GetAttributeBool(actionNode, "InheritPermissions", false, accessControlListData.InheritPermissions, out outValue);
      accessControlListData.InheritPermissions = outValue;
      foreach (XmlElement selectNode in actionNode.SelectNodes("AccessControlEntry"))
        accessControlListData.Permissions.Add(this.GetQueryACEData((XmlNode) selectNode));
      this.m_requestContext.TraceLeave(900317, nameof (Update), nameof (Update), nameof (GetQueryAclUpdateDetails));
      return aclUpdateDetails;
    }

    private AccessControlEntryData GetQueryACEData(XmlNode actionNode)
    {
      this.m_requestContext.TraceEnter(900318, nameof (Update), nameof (Update), nameof (GetQueryACEData));
      AccessControlEntryData queryAceData = new AccessControlEntryData();
      int intValue;
      BaseUpdate.GetAttributeInt(actionNode, "Allow", false, queryAceData.Allow, out intValue);
      queryAceData.Allow = intValue;
      BaseUpdate.GetAttributeInt(actionNode, "Deny", false, queryAceData.Deny, out intValue);
      queryAceData.Deny = intValue;
      string stringValue1;
      BaseUpdate.GetAttributeString(actionNode, "IdentityType", true, "System.Security.Principal.WindowsIdentity", out stringValue1);
      string stringValue2;
      BaseUpdate.GetAttributeString(actionNode, "Identifier", true, (string) null, out stringValue2);
      queryAceData.Descriptor = new IdentityDescriptor(stringValue1, stringValue2);
      this.m_requestContext.TraceLeave(900319, nameof (Update), nameof (Update), nameof (GetQueryACEData));
      return queryAceData;
    }

    public DalSqlElement GetSingletonElement(Type elementType)
    {
      DalSqlElement singletonElement;
      if (this.m_singletonElements.TryGetValue(elementType, out singletonElement))
        return singletonElement;
      DalSqlElement element = DalSqlElement.GetElement(elementType, this.m_sqlBatch, this);
      this.m_singletonElements[elementType] = element;
      return element;
    }

    private T EnsureSingletonInitialized<T>(ref T sqlElement, SqlBatchBuilder sqlBatch) where T : DalSqlElement, new()
    {
      if ((object) sqlElement == null)
      {
        DalSqlElement dalSqlElement;
        if (this.m_singletonElements.TryGetValue(typeof (T), out dalSqlElement))
        {
          sqlElement = dalSqlElement as T;
        }
        else
        {
          sqlElement = DalSqlElement.GetElement<T>(sqlBatch, this);
          this.m_singletonElements[typeof (T)] = (DalSqlElement) sqlElement;
        }
      }
      return sqlElement;
    }

    private bool IsSingletonNeeded<T>(ref T sqlElement) where T : DalSqlElement
    {
      DalSqlElement dalSqlElement;
      if ((object) sqlElement == null && this.m_singletonElements.TryGetValue(typeof (T), out dalSqlElement))
        sqlElement = dalSqlElement as T;
      return (object) sqlElement != null && sqlElement.IsNeeded;
    }

    internal IFieldTypeDictionary GetFieldsSnapshot(IVssRequestContext requestContext)
    {
      if (this.m_fieldsSnapshot == null)
        this.m_fieldsSnapshot = (IFieldTypeDictionary) new LockFreeFieldDictionary(requestContext);
      return this.m_fieldsSnapshot;
    }

    public string ObjectType => "WorkItem";

    public ElementGroup PersonGroup => this.m_personGroup;

    public DalChangeConstantElement ChangeConstantElement => this.EnsureSingletonInitialized<DalChangeConstantElement>(ref this.m_changeConstantElement, this.m_sqlBatch);

    public DalSaveRuleElement SaveRuleElement => this.EnsureSingletonInitialized<DalSaveRuleElement>(ref this.m_saveRuleElement, this.m_sqlBatch);

    public DalSaveIdentityRuleElement SaveIdentityRuleElement => this.EnsureSingletonInitialized<DalSaveIdentityRuleElement>(ref this.m_saveIdentityRuleElement, this.m_sqlBatch);

    public DalSaveConstantSetElement SaveConstantSetElement => this.EnsureSingletonInitialized<DalSaveConstantSetElement>(ref this.m_saveConstantSetElement, this.m_sqlBatch);

    public ElementGroup ChangeIssueGroup => this.m_changeIssueGroup;

    public ElementGroup ChangeQueryGroup => this.m_changeQueryGroup;

    public ElementGroup ChangeFieldGroup => this.m_changeFieldGroup;

    public ElementGroup DeleteFieldGroup => this.m_deleteFieldGroup;

    public ElementGroup CheckFieldGroup => this.m_checkFieldGroup;

    public ElementGroup ChangeTreeGroup => this.m_changeTreeGroup;

    public ElementGroup ChangeRuleGroup => this.m_changeRuleGroup;

    public ElementGroup CheckRuleGroup => this.m_checkRuleGroup;

    public ElementGroup CheckTreeGroup => this.m_checkTreeGroup;

    public ElementGroup ChangeConstantSetGroup => this.m_changeConstantSetGroup;

    public ElementGroup ChangeWorkItemTypeGroup => this.m_changeWorkItemTypeGroup;

    public ElementGroup CheckWorkItemTypeGroup => this.m_checkWorkItemTypeGroup;

    public ElementGroup ChangeWorkItemTypeUsageGroup => this.m_changeWorkItemTypeUsageGroup;

    public ElementGroup ChangeActionGroup => this.m_changeActionGroup;

    public ElementGroup CheckActionGroup => this.m_checkActionGroup;

    public ElementGroup AddLinkTypeGroup => this.m_addLinkTypeGroup;

    public ElementGroup UpdateLinkTypeGroup => this.m_updateLinkTypeGroup;

    public ElementGroup DeleteLinkTypeGroup => this.m_deleteLinkTypeGroup;

    public ElementGroup AddLinkGroup => this.m_addLinkGroup;

    public ElementGroup UpdateLinkGroup => this.m_updateLinkGroup;

    public ElementGroup DeleteLinkGroup => this.m_deleteLinkGroup;

    public ElementGroup ChangeCategoryGroup => this.m_changeCategoryGroup;

    public ElementGroup CheckCategoryGroup => this.m_checkCategoryGroup;

    public ElementGroup ChangeCategoryMemberGroup => this.m_changeCategoryMemberGroup;

    public ElementGroup DestroyCategoryGroup => this.m_destroyCategoryGroup;

    public DalPersonIdMapElement PersonIdMapElement => this.EnsureSingletonInitialized<DalPersonIdMapElement>(ref this.m_personIdMapElement, this.m_sqlBatch);

    public DalGetTempIdMapElement GetTempIdMapElement => this.EnsureSingletonInitialized<DalGetTempIdMapElement>(ref this.m_getTempIdMapElement, this.m_sqlBatch);

    public DalCheckChangesElement CheckChangesElement => this.EnsureSingletonInitialized<DalCheckChangesElement>(ref this.m_checkChanges, this.m_sqlBatch);

    public DalApplyIssueChangesElement ApplyChangesElement => this.EnsureSingletonInitialized<DalApplyIssueChangesElement>(ref this.m_applyIssueChanges, this.m_sqlBatch);

    public bool IsLinkTypeBatch => this.m_addLinkTypeGroup.ElementCount + this.m_updateLinkTypeGroup.ElementCount + this.m_deleteLinkTypeGroup.ElementCount > 0;

    public bool IsFieldChangeBatch => this.m_changeFieldGroup.ElementCount + this.m_deleteFieldGroup.ElementCount > 0;

    public bool IsRuleChangeBatch => this.m_changeRuleGroup.ElementCount > 0;

    public bool IsQueryItemBatch => this.m_activeQueryItems.Count + this.m_deletedQueryItems.Count > 0;

    public bool IsWorkItemTypeChangeBatch => this.m_changeWorkItemTypeGroup.ElementCount > 0;

    public bool BypassRules => this.m_bypassRules;

    public ProvisionHelper ProvisionHelper
    {
      get
      {
        if (this.m_provisionHelper == null)
          this.m_provisionHelper = new ProvisionHelper(this.m_requestContext);
        return this.m_provisionHelper;
      }
    }

    public PermissionCheckHelper PermissionCheckHelper
    {
      get
      {
        if (this.m_permissionCheckHelper == null)
          this.m_permissionCheckHelper = new PermissionCheckHelper(this.m_requestContext);
        return this.m_permissionCheckHelper;
      }
    }

    public IWorkItemTypeExtensionsMatcher WorkItemTypeExtensionMatcher
    {
      get
      {
        if (!this.m_extensionMatcherInitialized)
        {
          this.m_extensionMatcher = this.m_requestContext.GetService<WorkItemTypeExtensionService>().GetExtensionMatcher(this.m_requestContext, new Guid?(), new Guid?());
          this.m_extensionMatcherInitialized = true;
        }
        return this.m_extensionMatcher;
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity RequestContextIdentity => this.m_requestContextIdentity;

    public IVssRequestContext RequestContext => this.m_requestContext;

    internal WorkItemTrackingRequestContext WitRequestContext => this.m_witRequestContext;

    public bool Overwrite => this.m_overwrite;

    public bool IsValidBatch => this.m_isValidBatch;

    public DateTime ChangedDate => this.m_changedDate;

    internal bool MetaDataChanged => this.m_metadataChanged;

    internal XmlElement Package { get; private set; }

    internal WorkItemTypeTemplateUpdateType WorkItemTypeTemplateUpdateType { get; set; }

    private struct EventTaskInfo
    {
      public bool MetadataChanged;
    }

    private enum LinkTypePermissions
    {
      Unchecked,
      None,
      FullControl,
      InsertOnly,
    }
  }
}
