// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IWitHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [DefaultServiceImplementation(typeof (WITHelper))]
  internal interface IWitHelper : IVssFrameworkService
  {
    bool ValidateStateTransition(
      TestManagementRequestContext context,
      string projectName,
      string fromState,
      string toState,
      string workItemType);

    IEnumerable<WorkItemDelete> DeleteWorkItem(IVssRequestContext context, List<int> ids);

    IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext context,
      List<int> ids,
      List<string> fields,
      bool skipPermissionCheck = false,
      string projectName = null);

    IDictionary<int, string> GetWorkItemAreaUris(
      TestManagementRequestContext context,
      IEnumerable<int> workItemIds,
      bool skipPermissionCheck);

    IDictionary<int, CachedWorkItemData> GetWorkItemCacheData(
      TestManagementRequestContext context,
      IEnumerable<int> workItemIds,
      bool skipPermissionCheck);

    void ValidateTreePath(
      TfsTestManagementRequestContext tcmRequestContext,
      TreeStructureType type,
      string approxValue,
      GuidAndString targetProject);

    List<WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext context,
      string teamProjectName,
      IEnumerable<string> witCategoryRefNames);

    string AreaPathToUri(TestManagementRequestContext context, string areaPath);

    string TranslateCSSPath(string path);

    List<TestExternalLink> QueryHyperLinks(TestManagementRequestContext context, int workItemId);

    List<int> QueryWorkItems(
      TestManagementRequestContext context,
      string teamProjectName,
      string queryString,
      bool skipWiqlTextLimitValidation = false,
      int top = 2147483647);

    void PopulateWitRelationsToAdd(
      IList<WorkItemRelation> sourcePlanLinks,
      List<WorkItemRelation> filteredRelations,
      List<TestExternalLink> alreadyExistingHyperLinks);

    void PopulateFieldsToUpdate(
      TestManagementRequestContext context,
      IDictionary<string, object> sourceTestPlanFields,
      Dictionary<string, object> fieldsToUpdate,
      List<string> fieldsNotToUpdate);

    void UpdateWorkItem(
      TestManagementRequestContext context,
      IdAndRev idRev,
      Dictionary<string, object> fieldsToUpdate,
      List<WorkItemRelation> relations,
      bool suppressNotifications);

    void LinkTestCaseToRequirement(
      IVssRequestContext requestContext,
      int requirementId,
      int testCaseId);

    void UnLinkTestCaseFromRequirement(
      IVssRequestContext requestContext,
      int requirementId,
      int[] testcaseIds);

    IEnumerable<string> GetWorkItemTypesForWorkItemCategory(
      IVssRequestContext context,
      string projectName,
      string witCategoryRefName);

    bool WorkItemExists(
      TestManagementRequestContext context,
      string teamProjectName,
      Dictionary<int, string> ids,
      out List<int> validIds);

    void SuiteWorkItemExists(
      TestManagementRequestContext context,
      string teamProjectName,
      List<SuiteIdAndType> allSuites,
      out List<SuiteIdAndType> validSuites);

    IEnumerable<RuleRecord> GetRulesForWorkItemType(
      TfsTestManagementRequestContext requestContext,
      string teamProjectName,
      string workItemType);

    int GetFieldId(TestManagementRequestContext context, string propertyName);

    string GetWorkItemStartState(
      TestManagementRequestContext context,
      string projectName,
      string workItemType);

    ReportingWorkItemRevisionsBatch GetUpdatedWorkitems(
      TestManagementRequestContext context,
      string[] workItemFields,
      string workItemCategory,
      DateTime? startTime,
      string continuationToken,
      bool skipPermissionCheck = false);

    void GetRequiredFieldsData(
      TfsTestManagementRequestContext requestContext,
      string teamProjectName,
      string workItemType,
      out List<string> requiredFieldIds,
      out Dictionary<string, string> fieldTdToDefaultValueMap,
      out Dictionary<string, ServerDefaultFieldValue> fieldTdToServerDefaultValueMap);

    string GetDefaultWorkItemTypeInCategory(
      TestManagementRequestContext context,
      string teamProjectName,
      string witCategoryRefName);

    void AddWitField(
      WITCreator witCreator,
      TestManagementRequestContext context,
      Dictionary<string, object> witFields,
      string fieldName,
      string fieldValue,
      GuidAndString targetProject);

    IList<string> GetStates(
      TestManagementRequestContext context,
      string projectName,
      string workItemType);

    string ConvertWiqlQueryFromNameToIds(
      IVssRequestContext requestContext,
      Guid projectId,
      string query);

    string ConvertWiqlQueryFromIdsToName(
      IVssRequestContext requestContext,
      Guid projectId,
      string wiqlQueryWithIds);
  }
}
