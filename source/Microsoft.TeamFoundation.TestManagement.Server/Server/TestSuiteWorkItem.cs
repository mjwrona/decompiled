// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSuiteWorkItem
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestSuiteWorkItem : TCMWorkItemBase
  {
    private const int c_StaticSuiteTypeId = 1;
    private const int c_QueryBasedSuiteTypeId = 2;
    private const int c_RequirementBasedSuiteTypeId = 3;
    private readonly string[] suiteWitFields = new string[16]
    {
      "System.Id",
      "System.Rev",
      "System.Title",
      "System.WorkItemType",
      "System.CreatedBy",
      "System.ChangedBy",
      "System.ChangedDate",
      "System.AreaPath",
      "System.AssignedTo",
      "System.IterationPath",
      "System.State",
      "System.TeamProject",
      "System.Description",
      TCMWitFields.SuiteType,
      TCMWitFields.SuiteTypeId,
      TCMWitFields.QueryText
    };

    internal TestSuiteWorkItem()
    {
      this.CategoryRefName = WitCategoryRefName.TestSuite;
      this.CategoryName = WitCategoryName.TestSuite;
      this.Id = -1;
      this.TCMObjectType = ObjectTypes.TestSuite;
    }

    internal TestSuiteWorkItem(ServerTestSuite tcmSuite)
      : this()
    {
      this.TCMSuite = tcmSuite;
    }

    internal TestSuiteWorkItem(ServerTestSuite tcmSuite, int parentId, string parentCategory)
      : this()
    {
      this.TCMSuite = tcmSuite;
      this.ParentId = parentId;
      this.ParentCategory = parentCategory;
    }

    internal void Create(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      IList<TestExternalLink> externalLinks,
      IList<int> witIdsToLink,
      string audit,
      bool byPass = false)
    {
      context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.Create");
      this.Audit = audit;
      this.Create(context, teamProjectName, projectId, externalLinks, witIdsToLink, byPass);
      context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.Create");
    }

    internal void Update(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      IdAndRev witIdAndRev,
      CoreWorkItemUpdateFields existingWorkItemFieldValues,
      IList<TestExternalLink> externalLinks,
      IList<WorkItemLinkInfo> witLinks,
      WitOperationType witOperationType,
      string audit,
      bool byPass = false,
      bool suppressNotifications = false,
      bool isSuiteRenameScenario = false)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.SyncSuitesViaJob"))
        witIdAndRev.Revision = 0;
      this.Audit = audit;
      this.Update(context, teamProjectName, projectId, witIdAndRev, existingWorkItemFieldValues, externalLinks, witLinks, witOperationType, byPass, suppressNotifications, isSuiteRenameScenario);
    }

    internal void UpdateAudit(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      IdAndRev witIdAndRev,
      string audit,
      bool suppressNotifications,
      bool byPass = false)
    {
      context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.UpdateAudit");
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.SyncSuitesViaJob"))
        witIdAndRev.Revision = 0;
      this.Audit = audit;
      this.Update(context, teamProjectName, projectId, witIdAndRev, (CoreWorkItemUpdateFields) null, (IList<TestExternalLink>) null, (IList<WorkItemLinkInfo>) null, WitOperationType.UpdateAudit, byPass, suppressNotifications);
      context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.UpdateAudit");
    }

    internal WorkItemUpdateData GetUpdateDataForAuditUpdate(
      TestManagementRequestContext context,
      string teamProjectName,
      IdAndRev witIdAndRev,
      string audit,
      bool byPass = false)
    {
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.SyncSuitesViaJob"))
        witIdAndRev.Revision = 0;
      this.Audit = audit;
      return this.GetUpdateDataForSave(context, teamProjectName, witIdAndRev, (CoreWorkItemUpdateFields) null, (IList<TestExternalLink>) null, (IList<WorkItemLinkInfo>) null, WitOperationType.UpdateAudit, byPass);
    }

    protected override Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State[] GetMappedWitStates(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig)
    {
      context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.GetMappedWitStates");
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State[] mappedWitStates = Array.Empty<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>();
      if (projProcConfig.TestSuiteStates != null)
        mappedWitStates = projProcConfig.TestSuiteStates;
      context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.GetMappedWitStates");
      return mappedWitStates;
    }

    protected override void PopulateWitFields(
      TestManagementRequestContext context,
      WitOperationType witOperationType,
      bool byPass)
    {
      if (byPass && this.TCMSuite != null)
      {
        this.OwnerName = this.TCMSuite.CreatedByName;
        this.OwnerDistinctName = this.TCMSuite.CreatedByDistinctName;
        this.CreatedByName = this.TCMSuite.CreatedByName;
        this.CreatedByDistinctName = this.TCMSuite.CreatedByDistinctName;
        this.LastUpdatedBy = this.TCMSuite.LastUpdatedBy;
        this.LastUpdatedByName = this.TCMSuite.LastUpdatedByName;
        this.LastUpdatedByDistinctName = IdentityHelper.ResolveIdentityToName(context, this.TCMSuite.LastUpdatedBy, true);
      }
      else
        base.PopulateWitFields(context, witOperationType, byPass);
      if (witOperationType != WitOperationType.WitFieldUpdate)
        return;
      if (this.TCMSuite == null)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(this.ParentName, "ParentName", context.RequestContext.ServiceName);
        this.Title = this.ParentName;
        this.Description = string.Empty;
      }
      else
      {
        this.Title = this.TCMSuite.Title;
        this.Description = this.TCMSuite.Description;
        if (!string.IsNullOrEmpty(this.TCMSuite.ConvertedQueryString))
          this.QueryString = this.TCMSuite.ConvertedQueryString;
        else
          this.QueryString = this.TCMSuite.QueryString;
      }
    }

    protected override void PopulateFieldsBeforeCreate(
      TestManagementRequestContext context,
      string teamProjectName,
      bool byPass)
    {
      base.PopulateFieldsBeforeCreate(context, teamProjectName, byPass);
      if (string.IsNullOrEmpty(this.AreaPath) || string.IsNullOrEmpty(this.Iteration))
      {
        CoreWorkItemUpdateFields itemUpdateFields = TCMWorkItemBase.FetchCoreWorkItemUpdateFields(context, teamProjectName, this.ParentId, this.ParentCategory);
        this.AreaPath = TCMWorkItemBase.GetFirstNonEmptyValue(itemUpdateFields.AreaPath, this.State);
        this.Iteration = TCMWorkItemBase.GetFirstNonEmptyValue(itemUpdateFields.IterationPath, this.Iteration);
      }
      this.SuiteTypeId = this.GetSuiteTypeId();
      this.SuiteType = this.ReadSuiteTypeByIdFromWorkItemDefinition(context, teamProjectName);
    }

    protected override void PopulateFieldsAfterCreate()
    {
      if (this.TCMSuite == null)
        return;
      this.TCMSuite.ParentId = this.ParentId;
    }

    protected override void OnWitUpdateComplete(
      TestManagementRequestContext context,
      int id,
      int revision,
      DateTime lastUpdated)
    {
      base.OnWitUpdateComplete(context, id, revision, lastUpdated);
      if (this.TCMSuite == null)
        return;
      this.TCMSuite.Id = this.Id;
      this.TCMSuite.Revision = this.Revision;
      this.TCMSuite.LastUpdated = this.LastUpdated;
      this.TCMSuite.LastUpdatedBy = this.LastUpdatedBy;
      this.TCMSuite.LastUpdatedByName = this.LastUpdatedByName;
      this.TCMSuite.Status = this.State;
    }

    protected override Dictionary<string, object> CreateWitFields(
      TestManagementRequestContext context,
      string teamprojectName,
      GuidAndString targetProject,
      bool suiteStateChanged,
      bool isNew,
      WitOperationType witOperationType,
      bool isUpgrade = false,
      bool isSuiteRenameScenario = false)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.CreateWitFields");
        WITCreator witCreator = new WITCreator(context);
        Dictionary<string, object> witFields = base.CreateWitFields(context, teamprojectName, targetProject, suiteStateChanged, isNew, witOperationType, isUpgrade, isSuiteRenameScenario);
        if (witOperationType == WitOperationType.WitFieldUpdate)
        {
          if (isNew)
          {
            witFields[TCMWitFields.SuiteTypeId] = (object) this.SuiteTypeId;
            witFields[TCMWitFields.SuiteType] = (object) this.SuiteType;
            this.CheckValueAndAddWitField(witCreator, context, witFields, "System.AssignedTo", this.OwnerDistinctName, targetProject);
          }
          if (this.TCMSuite != null && !string.IsNullOrEmpty(this.TCMSuite.QueryString))
            witFields[TCMWitFields.QueryText] = (object) this.QueryString;
        }
        if (!string.IsNullOrEmpty(this.Audit))
          witFields[TCMWitFields.SuiteAudit] = (object) this.Audit;
        return witFields;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.CreateWitFields");
      }
    }

    protected override void PopulateWitId(
      TestManagementRequestContext context,
      int id,
      int revision)
    {
    }

    protected override StateTypeEnum ToMetaState(byte tcmState)
    {
      switch (tcmState)
      {
        case 1:
          return StateTypeEnum.Proposed;
        case 2:
          return StateTypeEnum.InProgress;
        case 3:
          return StateTypeEnum.Complete;
        default:
          return StateTypeEnum.Proposed;
      }
    }

    internal override byte FromMetaState(StateTypeEnum metaState)
    {
      switch (metaState)
      {
        case StateTypeEnum.Proposed:
          return 1;
        case StateTypeEnum.InProgress:
          return 2;
        case StateTypeEnum.Complete:
          return 3;
        default:
          return 0;
      }
    }

    protected override void ValidateStateTransition(
      TestManagementRequestContext context,
      string teamProjectName,
      string fromState,
      out bool witStateChanged)
    {
      context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.ValidateStateTransition");
      base.ValidateStateTransition(context, teamProjectName, fromState, out witStateChanged);
      if (!string.Equals(fromState, this.State, StringComparison.CurrentCultureIgnoreCase) && !witStateChanged)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.NoValidStateTransitionExistsForSuite, (object) fromState, (object) this.State));
      context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.ValidateStateTransition");
    }

    internal override IList<string> GetDefaultWorkItemStates(TestManagementRequestContext context) => (IList<string>) new List<string>()
    {
      ServerResources.TestSuiteCompletedWorkItemState,
      ServerResources.TestSuiteInPlanningWorkItemState,
      ServerResources.TestSuiteInProgressWorkItemState
    };

    internal override List<StateTypeEnumAndStateString> GetDefaultStatesMap() => new List<StateTypeEnumAndStateString>()
    {
      new StateTypeEnumAndStateString((byte) 2, ServerResources.TestSuiteInProgressWorkItemState),
      new StateTypeEnumAndStateString((byte) 1, ServerResources.TestSuiteInPlanningWorkItemState),
      new StateTypeEnumAndStateString((byte) 3, ServerResources.TestSuiteCompletedWorkItemState)
    };

    protected override StateTypeEnum GetMetaStatesFromDefaultMap(
      TestManagementRequestContext context,
      string workItemState)
    {
      if (string.Equals(workItemState, ServerResources.TestSuiteCompletedWorkItemState, StringComparison.CurrentCultureIgnoreCase))
        return StateTypeEnum.Complete;
      if (string.Equals(workItemState, ServerResources.TestSuiteInPlanningWorkItemState, StringComparison.CurrentCultureIgnoreCase))
        return StateTypeEnum.Proposed;
      if (string.Equals(workItemState, ServerResources.TestSuiteInProgressWorkItemState, StringComparison.CurrentCultureIgnoreCase))
        return StateTypeEnum.InProgress;
      throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CouldNotFindAValidSuiteStateMapping, (object) workItemState));
    }

    protected override StateTypeEnum GetMetaStatesFromProcessConfiguration(
      TestManagementRequestContext context,
      TestManagementProcessConfig projProcConfig,
      string workItemState)
    {
      int processConfiguration = (int) base.GetMetaStatesFromProcessConfiguration(context, projProcConfig, workItemState);
      return processConfiguration != 0 ? (StateTypeEnum) processConfiguration : throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CouldNotFindAValidSuiteStateMapping, (object) workItemState));
    }

    protected override void FromWorkItem(
      TestManagementRequestContext context,
      WorkItem workItemFieldData)
    {
      this.SuiteTypeId = this.ExtractFieldValueFromPayload<int>(context, TCMWitFields.SuiteTypeId, workItemFieldData);
      this.SuiteType = this.ExtractFieldValueFromPayload<string>(context, TCMWitFields.SuiteType, workItemFieldData);
      this.QueryString = this.ExtractFieldValueFromPayload<string>(context, TCMWitFields.QueryText, workItemFieldData);
      base.FromWorkItem(context, workItemFieldData);
    }

    internal static Dictionary<int, int> FetchSuitesRevision(
      TestManagementRequestContext context,
      string projectName,
      List<int> ids)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.FetchSuitesRevision");
        Dictionary<int, int> dictionary1 = new Dictionary<int, int>();
        IWitHelper service = context.RequestContext.GetService<IWitHelper>();
        IVssRequestContext requestContext = context.RequestContext;
        List<int> ids1 = ids;
        List<string> fields = new List<string>();
        fields.Add("System.Id");
        fields.Add("System.Rev");
        fields.Add("System.TeamProject");
        string projectName1 = projectName;
        IEnumerable<WorkItem> workItems = service.GetWorkItems(requestContext, ids1, fields, true, projectName1);
        if (workItems == null || workItems.Count<WorkItem>() <= 0)
          throw new TestObjectNotFoundException(string.Format("Not able to fetch workitems: {0}", (object) string.Join<int>(",", (IEnumerable<int>) ids)), ObjectTypes.TestSuite);
        foreach (WorkItem workItem in workItems)
        {
          Dictionary<int, int> dictionary2 = dictionary1;
          int? nullable = workItem.Id;
          int key = nullable.Value;
          nullable = workItem.Rev;
          int num = nullable.Value;
          dictionary2[key] = num;
        }
        return dictionary1;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.FetchSuitesRevision");
      }
    }

    protected override void PopulateFieldsBeforeUpdate(
      TestManagementRequestContext context,
      string teamProjectName,
      ref CoreWorkItemUpdateFields existingWorkItemFieldValues,
      WitOperationType witOperationType,
      bool byPass)
    {
      base.PopulateFieldsBeforeUpdate(context, teamProjectName, ref existingWorkItemFieldValues, witOperationType, byPass);
      if (witOperationType == WitOperationType.TcmFieldUpdate)
        return;
      this.AreaPath = TCMWorkItemBase.GetFirstNonEmptyValue(existingWorkItemFieldValues.AreaPath, this.AreaPath);
      this.Iteration = TCMWorkItemBase.GetFirstNonEmptyValue(existingWorkItemFieldValues.IterationPath, this.Iteration);
      this.WitTypeName = TCMWorkItemBase.GetFirstNonEmptyValue(existingWorkItemFieldValues.WokItemType, this.WitTypeName);
      this.State = TCMWorkItemBase.GetFirstNonEmptyValue(this.TCMSuite != null ? this.TCMSuite.Status : string.Empty, existingWorkItemFieldValues.State);
    }

    internal override string GetDefaultWorkItemState(
      TestManagementRequestContext context,
      byte tcmState)
    {
      string defaultWorkItemState = (string) null;
      switch (tcmState)
      {
        case 1:
          defaultWorkItemState = ServerResources.TestSuiteInPlanningWorkItemState;
          break;
        case 2:
          defaultWorkItemState = ServerResources.TestSuiteInProgressWorkItemState;
          break;
        case 3:
          defaultWorkItemState = ServerResources.TestSuiteCompletedWorkItemState;
          break;
      }
      return defaultWorkItemState;
    }

    internal override byte GetDefaultTcmState(
      TestManagementRequestContext context,
      string workItemState)
    {
      if (string.Equals(workItemState, ServerResources.TestSuiteCompletedWorkItemState, StringComparison.CurrentCultureIgnoreCase))
        return 3;
      if (string.Equals(workItemState, ServerResources.TestSuiteInPlanningWorkItemState, StringComparison.CurrentCultureIgnoreCase))
        return 1;
      return string.Equals(workItemState, ServerResources.TestSuiteInProgressWorkItemState, StringComparison.CurrentCultureIgnoreCase) ? (byte) 2 : (byte) 0;
    }

    internal static List<ServerTestSuite> FetchSuites(
      TestManagementRequestContext context,
      string projectName,
      List<int> ids,
      bool skipPermissionCheck = false)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.FetchSuites");
        List<ServerTestSuite> suites = new List<ServerTestSuite>();
        if (ids.Count == 0)
          return suites;
        List<TCMWorkItemBase> workItems = TCMWorkItemBase.GetWorkItems(context, projectName, ids, WitCategoryRefName.TestSuite, skipPermissionCheck);
        if (workItems != null && workItems.Count > 0)
        {
          Dictionary<string, Guid> identitiesMap = new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          TestSuiteWorkItem.PopulateSuiteProperties(context, projectName, suites, identitiesMap, workItems);
          TestSuiteWorkItem.ResolveAndPopulateIdentityFields(context, suites, identitiesMap);
        }
        return suites;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.FetchSuites");
      }
    }

    internal static KeyValuePair<string, List<ServerTestSuite>> FetchUpdatedSuites(
      TestManagementRequestContext context,
      DateTime? startTime,
      string continuationToken,
      bool skipPermissionCheck = false)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.FetchSuites");
        List<ServerTestSuite> suites = new List<ServerTestSuite>();
        KeyValuePair<string, List<TCMWorkItemBase>> updatedWorkItems = TCMWorkItemBase.GetUpdatedWorkItems(context, WitCategoryRefName.TestSuite, startTime, continuationToken, skipPermissionCheck);
        if (updatedWorkItems.Value != null && updatedWorkItems.Value.Count > 0)
        {
          Dictionary<string, Guid> identitiesMap = new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          TestSuiteWorkItem.PopulateSuiteProperties(context, suites, identitiesMap, updatedWorkItems.Value);
          TestSuiteWorkItem.ResolveAndPopulateIdentityFields(context, suites, identitiesMap);
        }
        return new KeyValuePair<string, List<ServerTestSuite>>(updatedWorkItems.Key, suites);
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.FetchSuites");
      }
    }

    internal TestSuiteType GetSuiteType(TfsTestManagementRequestContext context)
    {
      context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.GetSuiteType");
      TestSuiteType suiteType = TestSuiteType.StaticTestSuite;
      if (this.TCMSuite != null)
      {
        if (this.SuiteTypeId == 2)
          suiteType = TestSuiteType.DynamicTestSuite;
        else if (this.SuiteTypeId == 3)
        {
          suiteType = TestSuiteType.RequirementTestSuite;
        }
        else
        {
          if (this.SuiteTypeId != 1)
            throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidSuiteType, (object) this.SuiteType));
          suiteType = TestSuiteType.StaticTestSuite;
        }
      }
      context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.GetSuiteType");
      return suiteType;
    }

    internal override byte GetTCMState(
      IReadOnlyCollection<WorkItemStateDefinition> possibleStates,
      string workItemState)
    {
      if (possibleStates != null)
      {
        switch (possibleStates.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (possibleState => possibleState.Name == workItemState)).First<WorkItemStateDefinition>().StateCategory)
        {
          case WorkItemStateCategory.Proposed:
            return 1;
          case WorkItemStateCategory.InProgress:
            return 2;
          case WorkItemStateCategory.Completed:
            return 3;
        }
      }
      return 0;
    }

    private static void PopulateSuiteProperties(
      TestManagementRequestContext context,
      string projectName,
      List<ServerTestSuite> suites,
      Dictionary<string, Guid> identitiesMap,
      List<TCMWorkItemBase> tcmWits)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      TestSuiteWorkItem.PopulateSuiteProperties(context, suites, identitiesMap, tcmWits, projectFromName);
    }

    private static void PopulateSuiteProperties(
      TestManagementRequestContext context,
      List<ServerTestSuite> suites,
      Dictionary<string, Guid> identitiesMap,
      List<TCMWorkItemBase> tcmWits,
      GuidAndString projectId = default (GuidAndString))
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.PopulateSuiteProperties");
        foreach (TCMWorkItemBase tcmWit in tcmWits)
        {
          ServerTestSuite serverTestSuite = ServerTestSuite.FromWorkItem(context, projectId, (TestSuiteWorkItem) tcmWit);
          if (serverTestSuite != null)
          {
            if (!string.IsNullOrEmpty(serverTestSuite.LastUpdatedByName))
              identitiesMap[serverTestSuite.LastUpdatedByName] = Guid.Empty;
            suites.Add(serverTestSuite);
          }
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.PopulateSuiteProperties");
      }
    }

    private static void ResolveAndPopulateIdentityFields(
      TestManagementRequestContext context,
      List<ServerTestSuite> suites,
      Dictionary<string, Guid> identitiesMap)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "TestSuiteWorkItem.ResolveAndPopulateIdentityFields");
        TCMWorkItemBase.ResolveDisplayNames(context, identitiesMap);
        TestSuiteWorkItem.PopulateIdentityFields(context, suites, identitiesMap);
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "TestSuiteWorkItem.ResolveAndPopulateIdentityFields");
      }
    }

    private static void PopulateIdentityFields(
      TestManagementRequestContext context,
      List<ServerTestSuite> suites,
      Dictionary<string, Guid> identitiesMap)
    {
      foreach (ServerTestSuite suite in suites)
      {
        suite.LastUpdatedBy = !string.IsNullOrEmpty(suite.LastUpdatedByName) ? identitiesMap[suite.LastUpdatedByName] : Guid.Empty;
        context.TraceVerbose("BusinessLayer", "TestSuiteWorkItem.PopulateIdentityFields: SuiteId:{0}, LastUpdatedByName:{1} LastUpdatedBy:{2}", (object) suite.Id, (object) suite.LastUpdatedByName, (object) suite.LastUpdatedBy);
      }
    }

    private int GetSuiteTypeId()
    {
      if (this.TCMSuite != null)
      {
        switch (this.TCMSuite.SuiteType)
        {
          case 0:
            throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidSuiteType, (object) this.TCMSuite.SuiteType.ToString()));
          case 1:
            return 2;
          case 2:
            return 1;
          case 3:
            return 3;
        }
      }
      return 1;
    }

    private string ReadSuiteTypeByIdFromWorkItemDefinition(
      TestManagementRequestContext context,
      string teamProjectName)
    {
      IWitHelper service = context.RequestContext.GetService<IWitHelper>();
      IEnumerable<RuleRecord> rulesForWorkItemType = service.GetRulesForWorkItemType(context as TfsTestManagementRequestContext, teamProjectName, this.WitTypeName);
      if (rulesForWorkItemType != null && rulesForWorkItemType.Count<RuleRecord>() > 0)
      {
        int fieldId1 = service.GetFieldId(context, "System.State");
        int fieldId2 = service.GetFieldId(context, TCMWitFields.SuiteType);
        int fieldId3 = service.GetFieldId(context, TCMWitFields.SuiteTypeId);
        foreach (RuleRecord ruleRecord in rulesForWorkItemType)
        {
          int result;
          if (ruleRecord.Fld2ID == fieldId1 && string.IsNullOrEmpty(ruleRecord.Fld2Was) && ruleRecord.ThenFldID == fieldId2 && ruleRecord.IfFldID == fieldId3 && int.TryParse(ruleRecord.If, out result) && result == this.SuiteTypeId)
            return ruleRecord.Then;
        }
      }
      return string.Empty;
    }

    public int PlanId { get; protected set; }

    public int RootSuiteId { get; private set; }

    public int ParentId { get; set; }

    public string ParentName { get; set; }

    public string ParentCategory { get; set; }

    public int ParentRevision { get; set; }

    public string QueryString { get; private set; }

    public int SuiteTypeId { get; private set; }

    public string SuiteType { get; private set; }

    public string Audit { get; private set; }

    public override string ProcessConfigCategoryName => "TestSuiteWorkItems";

    public ServerTestSuite TCMSuite { get; private set; }

    protected override string[] WitFields => this.suiteWitFields;

    protected override byte DefaultTCMState => 2;
  }
}
