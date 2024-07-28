// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WITHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Compatibility;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class WITHelper : TeamFoundationTestManagementService, IWitHelper, IVssFrameworkService
  {
    public bool ValidateStateTransition(
      TestManagementRequestContext context,
      string projectName,
      string fromState,
      string toState,
      string workItemType)
    {
      context.TraceEnter("BusinessLayer", "WITHelper.ValidateStateTransition");
      bool flag = false;
      if (!string.IsNullOrEmpty(workItemType))
      {
        IDictionary<string, HashSet<string>> stateTransitions = this.GetStateTransitions(context, projectName, workItemType);
        HashSet<string> stringSet = (HashSet<string>) null;
        string key = fromState;
        ref HashSet<string> local = ref stringSet;
        if (stateTransitions.TryGetValue(key, out local) && stringSet.Contains(toState))
        {
          context.TraceInfo("BusinessLayer", "State transition exists from " + fromState + " to " + toState);
          flag = true;
        }
      }
      else
        context.TraceError("BusinessLayer", "workItemType is null or empty");
      context.TraceLeave("BusinessLayer", "WITHelper.ValidateStateTransition");
      return flag;
    }

    public string ConvertWiqlQueryFromIdsToName(
      IVssRequestContext requestContext,
      Guid projectId,
      string wiqlQueryWithIds)
    {
      string name = requestContext.GetService<IWiqlConverterRemotableService>().WiqlConvertIdsToName(requestContext, projectId, wiqlQueryWithIds);
      string b = wiqlQueryWithIds;
      requestContext.TraceInfo("BusinessLayer", "Converted Wiql Query from {0} to {1}", (object) wiqlQueryWithIds, (object) wiqlQueryWithIds);
      if (!string.Equals(name, b, StringComparison.InvariantCultureIgnoreCase))
      {
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("originalQuery", b);
        intelligenceData.Add("convertedQuery", name);
        IVssRequestContext requestContext1 = requestContext;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, "TcmWithelper", nameof (ConvertWiqlQueryFromIdsToName), properties);
      }
      return name;
    }

    public IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext context,
      List<int> ids,
      List<string> fields,
      bool skipPermissionCheck = false,
      string projectName = null)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItems), "WorkItem")))
      {
        try
        {
          context.TraceEnter("BusinessLayer", string.Format("WITHelper.GetWorkItems"));
          using (PerfManager.Measure(context, "BusinessLayer", "WITHelper.GetWorkItems"))
          {
            IWorkItemRemotableService service = context.GetService<IWorkItemRemotableService>();
            return skipPermissionCheck ? service.GetWorkItems(context.Elevate(), (IEnumerable<int>) ids, projectName, (IEnumerable<string>) fields) : service.GetWorkItems(context, (IEnumerable<int>) ids, projectName, (IEnumerable<string>) fields);
          }
        }
        finally
        {
          context.TraceLeave("BusinessLayer", string.Format("WITHelper.GetWorkItems"));
        }
      }
    }

    public List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory> GetWorkItemTypeCategories(
      IVssRequestContext context,
      string teamProjectName,
      IEnumerable<string> witCategoryRefNames)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItemTypeCategories), "WorkItem")))
      {
        context.TraceEnter("BusinessLayer", string.Format("WITHelper.GetWorkItemTypeCategory , Parameters ProjectName = {0} , witCategoryRefNames = {1}", (object) teamProjectName, (object) string.Join(",", witCategoryRefNames)));
        List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory> itemTypeCategories = new List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>();
        IWorkItemTypeCategoryRemotableService service = context.GetService<IWorkItemTypeCategoryRemotableService>();
        foreach (string witCategoryRefName in witCategoryRefNames)
          itemTypeCategories.Add(service.GetWorkItemTypeCategory(context, teamProjectName, witCategoryRefName));
        context.TraceLeave("BusinessLayer", string.Format("WITHelper.GetWorkItemTypeCategory , Parameters ProjectName = {0} , witCategoryRefNames = {1}", (object) teamProjectName, (object) string.Join(",", witCategoryRefNames)));
        return itemTypeCategories;
      }
    }

    public IEnumerable<WorkItemDelete> DeleteWorkItem(IVssRequestContext context, List<int> ids)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (DeleteWorkItem), "WorkItem")))
      {
        try
        {
          context.TraceEnter("BusinessLayer", string.Format("WITHelper.DeleteWorkItem"));
          return context.GetService<IWorkItemRemotableService>().DeleteWorkItems(context, (IEnumerable<int>) ids);
        }
        finally
        {
          context.TraceLeave("BusinessLayer", string.Format("WITHelper.DeleteWorkItem"));
        }
      }
    }

    public IEnumerable<string> GetWorkItemTypesForWorkItemCategory(
      IVssRequestContext context,
      string projectName,
      string witCategoryRefName)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItemTypesForWorkItemCategory), "WorkItem")))
      {
        context.TraceEnter("BusinessLayer", string.Format("WITHelper.GetWorkItemTypesForWorkItemCategory , Parameters ProjectName = {0} , witCategoryRefName = {1}", (object) projectName, (object) witCategoryRefName));
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory itemTypeCategory = context.GetService<IWorkItemTypeCategoryRemotableService>().GetWorkItemTypeCategory(context, projectName, witCategoryRefName);
        context.TraceLeave("BusinessLayer", "WITHelper.GetWorkItemTypesForWorkItemCategory");
        IEnumerable<WorkItemTypeReference> workItemTypes = itemTypeCategory.WorkItemTypes;
        return workItemTypes != null ? (IEnumerable<string>) workItemTypes.Select<WorkItemTypeReference, string>((Func<WorkItemTypeReference, string>) (type => type.Name)).ToList<string>() : (IEnumerable<string>) null;
      }
    }

    public string GetDefaultWorkItemTypeInCategory(
      TestManagementRequestContext context,
      string teamProjectName,
      string witCategoryRefName)
    {
      IList<string> witCategoryRefNames = (IList<string>) new List<string>();
      witCategoryRefNames.Add(witCategoryRefName);
      List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory> itemTypeCategories = this.GetWorkItemTypeCategories(context.RequestContext, teamProjectName, (IEnumerable<string>) witCategoryRefNames);
      if (itemTypeCategories == null || itemTypeCategories.Count != 1)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidWitCategory, (object) witCategoryRefName));
      return itemTypeCategories[0].DefaultWorkItemType?.Name;
    }

    public void LinkTestCaseToRequirement(
      IVssRequestContext requestContext,
      int requirementId,
      int testCaseId)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (LinkTestCaseToRequirement), "WorkItem")))
      {
        IWorkItemRemotableService service = requestContext.GetService<IWorkItemRemotableService>();
        JsonPatchDocument linkFromRequirement = this.CreateJsonPatchForLinkFromRequirement(requestContext, requirementId, testCaseId);
        IVssRequestContext requestContext1 = requestContext;
        int workItemId = requirementId;
        JsonPatchDocument workItemPatchDocument = linkFromRequirement;
        service.UpdateWorkItem(requestContext1, workItemId, workItemPatchDocument);
      }
    }

    public void UnLinkTestCaseFromRequirement(
      IVssRequestContext requestContext,
      int requirementId,
      int[] testcaseIds)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (UnLinkTestCaseFromRequirement), "WorkItem")))
      {
        IWorkItemRemotableService service = requestContext.GetService<IWorkItemRemotableService>();
        JsonPatchDocument unlinkFromRequirement = this.CreateJsonPatchForUnlinkFromRequirement(requestContext, requirementId, testcaseIds);
        IVssRequestContext requestContext1 = requestContext;
        int workItemId = requirementId;
        JsonPatchDocument workItemPatchDocument = unlinkFromRequirement;
        service.UpdateWorkItem(requestContext1, workItemId, workItemPatchDocument);
      }
    }

    public string AreaPathToUri(TestManagementRequestContext context, string areaPath)
    {
      string path = this.TranslateCSSPath(areaPath);
      return context.AreaPathsCache.GetCssNodeAndThrow(context, path).Uri;
    }

    public int GetFieldId(TestManagementRequestContext context, string propertyName)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetFieldId), "WorkItem")))
      {
        FieldEntry field;
        return context.RequestContext.GetService<WorkItemTrackingFieldService>().TryGetField(context.RequestContext, propertyName, out field) && field != null ? field.FieldId : -1;
      }
    }

    public ReportingWorkItemRevisionsBatch GetUpdatedWorkitems(
      TestManagementRequestContext context,
      string[] workItemFields,
      string workItemCategory,
      DateTime? startTime,
      string continuationToken,
      bool skipPermissionCheck = false)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetUpdatedWorkitems), "WorkItem")))
      {
        WorkItemTrackingHttpClient trackingHttpClient1 = !skipPermissionCheck ? context.RequestContext.GetClient<WorkItemTrackingHttpClient>() : context.RequestContext.Elevate().GetClient<WorkItemTrackingHttpClient>();
        List<string> stringList = new List<string>()
        {
          workItemCategory
        };
        WorkItemTrackingHttpClient trackingHttpClient2 = trackingHttpClient1;
        string[] fields = workItemFields;
        List<string> types = stringList;
        DateTime? nullable1 = startTime;
        string continuationToken1 = continuationToken;
        DateTime? startDateTime = nullable1;
        bool? nullable2 = new bool?(true);
        bool? includeIdentityRef = new bool?();
        bool? includeDeleted = new bool?();
        bool? includeTagRef = new bool?();
        bool? includeLatestOnly = nullable2;
        ReportingRevisionsExpand? expand = new ReportingRevisionsExpand?();
        bool? includeDiscussionChangesOnly = new bool?();
        int? maxPageSize = new int?();
        CancellationToken cancellationToken = new CancellationToken();
        return trackingHttpClient2.ReadReportingRevisionsGetAsync((IEnumerable<string>) fields, (IEnumerable<string>) types, continuationToken1, startDateTime, includeIdentityRef, includeDeleted, includeTagRef, includeLatestOnly, expand, includeDiscussionChangesOnly, maxPageSize, (object) null, cancellationToken)?.Result;
      }
    }

    public string ConvertWiqlQueryFromNameToIds(
      IVssRequestContext requestContext,
      Guid projectId,
      string query)
    {
      string a = string.Empty;
      if (!string.IsNullOrEmpty(query))
      {
        a = requestContext.GetService<IWiqlConverterRemotableService>().WiqlConvertNameToIds(requestContext, projectId, query);
        if (!string.Equals(a, query, StringComparison.InvariantCultureIgnoreCase))
        {
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("originalQuery", query);
          intelligenceData.Add("convertedQuery", a);
          IVssRequestContext requestContext1 = requestContext;
          CustomerIntelligenceData properties = intelligenceData;
          service.Publish(requestContext1, "TcmWithelper", nameof (ConvertWiqlQueryFromNameToIds), properties);
        }
      }
      requestContext.TraceInfo("BusinessLayer", "Converted Wiql Query from {0} to {1}", (object) query, (object) a);
      return a;
    }

    public string TranslateCSSPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return (string) null;
      path = path.TrimStart('\\');
      return path;
    }

    public void SuiteWorkItemExists(
      TestManagementRequestContext context,
      string teamProjectName,
      List<SuiteIdAndType> allSuites,
      out List<SuiteIdAndType> validSuites)
    {
      Dictionary<int, string> ids = new Dictionary<int, string>();
      foreach (SuiteIdAndType allSuite in allSuites)
        ids.Add(allSuite.Id, "Microsoft.TestSuiteCategory");
      List<int> validIds;
      this.WorkItemExists(context, teamProjectName, ids, out validIds);
      validSuites = new List<SuiteIdAndType>();
      if (validIds == null)
        return;
      foreach (SuiteIdAndType allSuite in allSuites)
      {
        if (validIds.Contains(allSuite.Id))
          validSuites.Add(allSuite);
      }
    }

    public bool WorkItemExists(
      TestManagementRequestContext context,
      string teamProjectName,
      Dictionary<int, string> ids,
      out List<int> validIds)
    {
      context.TraceEnter("BusinessLayer", "WITHelper.WorkItemExists");
      bool flag = true;
      validIds = new List<int>();
      try
      {
        IEnumerable<WorkItem> workItems = this.GetWorkItems(context.RequestContext, ids.Keys.ToList<int>(), new List<string>()
        {
          "System.Id",
          "System.WorkItemType"
        }, true, (string) null);
        List<int> list = workItems.Select<WorkItem, int>((Func<WorkItem, int>) (w => w.Id.Value)).ToList<int>().Except<int>((IEnumerable<int>) ids.Keys.ToList<int>()).ToList<int>();
        if (workItems.ToList<WorkItem>().Count != ids.Count || list.Count > 0)
          flag = false;
        foreach (WorkItem workItem in workItems)
        {
          Dictionary<int, string> dictionary = ids;
          int? id = workItem.Id;
          int key = id.Value;
          string str;
          ref string local = ref str;
          if (!dictionary.TryGetValue(key, out local))
          {
            flag = false;
          }
          else
          {
            List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory> itemTypeCategories = this.GetWorkItemTypeCategories(context.RequestContext, teamProjectName, (IEnumerable<string>) new List<string>()
            {
              str
            });
            if (itemTypeCategories == null || itemTypeCategories.Count != 1)
            {
              flag = false;
            }
            else
            {
              string result;
              if (workItem.Fields.TryGetValue<string, string>("System.WorkItemType", out result))
              {
                IEnumerable<WorkItemTypeReference> workItemTypes = itemTypeCategories[0].WorkItemTypes;
                if ((workItemTypes != null ? (workItemTypes.Any<WorkItemTypeReference>((Func<WorkItemTypeReference, bool>) (type => TFStringComparer.WorkItemTypeName.Equals(type.Name, result))) ? 1 : 0) : 0) != 0)
                {
                  List<int> intList = validIds;
                  id = workItem.Id;
                  int num = id.Value;
                  intList.Add(num);
                }
                else
                  flag = false;
              }
            }
          }
        }
        return flag;
      }
      catch (Exception ex)
      {
        context.RequestContext.Trace(1015001, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.Message);
        return false;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "WITHelper.WorkItemExists");
      }
    }

    public void AddWitField(
      WITCreator witCreator,
      TestManagementRequestContext context,
      Dictionary<string, object> witFields,
      string fieldName,
      string fieldValue,
      GuidAndString targetProject)
    {
      context.TraceEnter("BusinessLayer", "WITHelper.AddWitField");
      if (string.IsNullOrWhiteSpace(fieldName))
        throw new TestManagementValidationException(ServerResources.FieldValueMissing);
      string resolvedFieldId;
      string resolvedFieldValue;
      witCreator.ResolveFieldNameAndValue(fieldName, fieldValue, targetProject, false, out resolvedFieldId, out resolvedFieldValue);
      witFields.Add(resolvedFieldId, (object) resolvedFieldValue);
      context.TraceLeave("BusinessLayer", "WITHelper.AddWitField");
    }

    public List<TestExternalLink> QueryHyperLinks(
      TestManagementRequestContext context,
      int workItemId)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (QueryHyperLinks), "WorkItem")))
      {
        context.TraceEnter("BusinessLayer", "WITHelper.QueryHyperLinks");
        List<TestExternalLink> testExternalLinkList = new List<TestExternalLink>();
        WorkItem workItem = context.RequestContext.GetService<IWorkItemRemotableService>().GetWorkItem(context.RequestContext, workItemId, (IEnumerable<string>) new List<string>(), expand: WorkItemExpand.Relations);
        if (workItem != null)
        {
          if (workItem.Relations != null)
          {
            foreach (WorkItemRelation relation in (IEnumerable<WorkItemRelation>) workItem.Relations)
            {
              if (string.CompareOrdinal(relation.Rel, "ArtifactLink") == 0 || string.CompareOrdinal(relation.Rel, "AttachedFile") == 0)
              {
                string str;
                relation.Attributes.TryGetValue<string, string>("comment", out str);
                int num;
                relation.Attributes.TryGetValue<string, int>("id", out num);
                testExternalLinkList.Add(new TestExternalLink()
                {
                  Uri = relation.Url,
                  Description = str,
                  LinkId = num
                });
              }
            }
          }
          context.TraceLeave("BusinessLayer", "WITHelper.QueryHyperLinks");
          return testExternalLinkList;
        }
        context.TraceError("BusinessLayer", string.Format("Not able to find Test Plan workitem with Id: {0}", (object) workItemId));
        throw new TestObjectNotFoundException(string.Format("Not able to find Test Plan workitem with Id: {0}", (object) workItemId), ObjectTypes.TestPlan);
      }
    }

    public void PopulateWitRelationsToAdd(
      IList<WorkItemRelation> sourcePlanLinks,
      List<WorkItemRelation> filteredRelations,
      List<TestExternalLink> alreadyExistingHyperLinks)
    {
      foreach (WorkItemRelation sourcePlanLink in (IEnumerable<WorkItemRelation>) sourcePlanLinks)
      {
        WorkItemRelation link = sourcePlanLink;
        if (link.Url != null && alreadyExistingHyperLinks.Find((Predicate<TestExternalLink>) (hl => link.Url.Contains(hl.Uri))) == null && this.IsClonelink(link))
        {
          WorkItemRelation workItemRelation1 = new WorkItemRelation();
          workItemRelation1.Rel = link.Rel;
          workItemRelation1.Url = link.Url;
          workItemRelation1.Title = link.Title;
          workItemRelation1.Attributes = (IDictionary<string, object>) new Dictionary<string, object>();
          WorkItemRelation workItemRelation2 = workItemRelation1;
          if (link.Attributes != null)
          {
            foreach (KeyValuePair<string, object> attribute in (IEnumerable<KeyValuePair<string, object>>) link.Attributes)
              workItemRelation2.Attributes.Add(attribute.Key, attribute.Value);
          }
          filteredRelations.Add(workItemRelation2);
        }
      }
    }

    public void PopulateFieldsToUpdate(
      TestManagementRequestContext context,
      IDictionary<string, object> sourceTestPlanFields,
      Dictionary<string, object> fieldsToUpdate,
      List<string> fieldsNotToUpdate)
    {
      foreach (KeyValuePair<string, object> sourceTestPlanField in (IEnumerable<KeyValuePair<string, object>>) sourceTestPlanFields)
      {
        if (!fieldsNotToUpdate.Contains<string>(sourceTestPlanField.Key, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
        {
          object obj = (object) null;
          if (sourceTestPlanFields.TryGetValue(sourceTestPlanField.Key, out obj))
            fieldsToUpdate[sourceTestPlanField.Key] = obj;
        }
      }
      fieldsToUpdate[WorkItemFieldRefNames.ChangedBy] = (object) context.UserTeamFoundationName;
    }

    public void UpdateWorkItem(
      TestManagementRequestContext context,
      IdAndRev idRev,
      Dictionary<string, object> fieldsToUpdate,
      List<WorkItemRelation> relations,
      bool suppressNotifications)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (UpdateWorkItem), "WorkItem")))
      {
        IWorkItemRemotableService service = context.RequestContext.GetService<IWorkItemRemotableService>();
        JsonPatchDocument jsonPatchDocument = WorkHelper.ConvertToJsonPatchDocument(context.RequestContext, (IDictionary<string, object>) fieldsToUpdate, (IList<WorkItemRelation>) relations);
        try
        {
          service.UpdateWorkItem(context.RequestContext, idRev.Id, jsonPatchDocument, suppressNotifications: suppressNotifications);
        }
        catch (Exception ex)
        {
          context.TraceError("BusinessLayer", ex.Message);
          throw WitToTcmExceptionConverter.Convert(ex);
        }
      }
    }

    public void ValidateTreePath(
      TfsTestManagementRequestContext tcmRequestContext,
      TreeStructureType type,
      string approxValue,
      GuidAndString targetProject)
    {
      string a = targetProject.String;
      string str = (string) null;
      switch (type)
      {
        case TreeStructureType.Area:
          TcmCommonStructureNodeInfo cssNodeAndThrow1 = tcmRequestContext.AreaPathsCache.GetCssNodeAndThrow((TestManagementRequestContext) tcmRequestContext, approxValue);
          string uri1 = cssNodeAndThrow1.Uri;
          str = cssNodeAndThrow1.ProjectUri;
          break;
        case TreeStructureType.Iteration:
          TcmCommonStructureNodeInfo cssNodeAndThrow2 = tcmRequestContext.IterationsCache.GetCssNodeAndThrow((TestManagementRequestContext) tcmRequestContext, approxValue);
          string uri2 = cssNodeAndThrow2.Uri;
          str = cssNodeAndThrow2.ProjectUri;
          break;
      }
      string b = str;
      if (!string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopy_TreePathNotInProject, (object) approxValue));
    }

    public IList<string> GetStates(
      TestManagementRequestContext context,
      string projectName,
      string workItemType)
    {
      context.TraceEnter("BusinessLayer", "WITHelper.GetStates");
      List<string> stringList = new List<string>();
      if (!string.IsNullOrEmpty(workItemType))
      {
        IDictionary<string, HashSet<string>> stateTransitions = this.GetStateTransitions(context, projectName, workItemType);
        if (stateTransitions == null)
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CouldNotFindStatesForWorkItemType, (object) workItemType));
        foreach (HashSet<string> second in (IEnumerable<HashSet<string>>) stateTransitions.Values)
        {
          if (second != null)
            stringList = stringList.Concat<string>((IEnumerable<string>) second).ToList<string>();
        }
      }
      else
        context.TraceError("BusinessLayer", "workItemType is null or empty");
      context.TraceLeave("BusinessLayer", "WITHelper.GetStates");
      return (IList<string>) stringList.Distinct<string>().ToList<string>();
    }

    public string GetWorkItemStartState(
      TestManagementRequestContext context,
      string projectName,
      string workItemType)
    {
      context.TraceEnter("BusinessLayer", "WITHelper.GetWorkItemStartState");
      HashSet<string> source = (HashSet<string>) null;
      IDictionary<string, HashSet<string>> stateTransitions = this.GetStateTransitions(context, projectName, workItemType);
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      ref HashSet<string> local = ref source;
      if (stateTransitions.TryGetValue(empty2, out local))
      {
        string workItemStartState = source.FirstOrDefault<string>();
        context.TraceLeave("BusinessLayer", "WITHelper.GetWorkItemStartState");
        return workItemStartState;
      }
      context.TraceError("BusinessLayer", "Could not find a valid start state for " + workItemType);
      throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CouldNotFindValidStartStateForWorkItemType, (object) workItemType));
    }

    public void GetRequiredFieldsData(
      TfsTestManagementRequestContext requestContext,
      string teamProjectName,
      string workItemType,
      out List<string> requiredFieldIds,
      out Dictionary<string, string> fieldTdToDefaultValueMap,
      out Dictionary<string, ServerDefaultFieldValue> fieldTdToServerDefaultValueMap)
    {
      using (PerfManager.Measure(requestContext.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetRequiredFieldsData), "WorkItem")))
      {
        requestContext.TraceEnter("BusinessLayer", "WITHelper.GetRequiredFieldsData");
        fieldTdToDefaultValueMap = new Dictionary<string, string>();
        fieldTdToServerDefaultValueMap = new Dictionary<string, ServerDefaultFieldValue>();
        requiredFieldIds = new List<string>();
        WorkItem workItemTemplate = requestContext.RequestContext.GetService<IWorkItemRemotableService>().GetWorkItemTemplate(requestContext.RequestContext, teamProjectName, workItemType);
        IEnumerable<RuleRecord> rulesForWorkItemType = this.GetRulesForWorkItemType(requestContext, teamProjectName, workItemType);
        if (rulesForWorkItemType != null && rulesForWorkItemType.Count<RuleRecord>() > 0)
        {
          foreach (RuleRecord ruleRecord in rulesForWorkItemType)
          {
            string fieldRefName1;
            if (this.IsRequiredRule(ruleRecord) && this.TryGetFieldRefNameById((TestManagementRequestContext) requestContext, ruleRecord.ThenFldID, out fieldRefName1) && !requiredFieldIds.Contains(fieldRefName1))
              requiredFieldIds.Add(fieldRefName1);
            string fieldRefName2;
            if (this.IsDefaultRule(ruleRecord) && this.TryGetFieldRefNameById((TestManagementRequestContext) requestContext, ruleRecord.ThenFldID, out fieldRefName2) && !fieldTdToDefaultValueMap.ContainsKey(fieldRefName2) && workItemTemplate != null && workItemTemplate.Fields != null && workItemTemplate.Fields.ContainsKey(fieldRefName2) && workItemTemplate.Fields[fieldRefName2] != null)
              fieldTdToDefaultValueMap.Add(fieldRefName2, Convert.ToString(workItemTemplate.Fields[fieldRefName2]));
            ServerDefaultType from;
            if (this.IsServerDefaultRule(ruleRecord, out from))
            {
              ServerDefaultFieldValue defaultFieldValue = (ServerDefaultFieldValue) null;
              switch (from)
              {
                case ServerDefaultType.ServerDateTime:
                  defaultFieldValue = new ServerDefaultFieldValue(ServerDefaultType.ServerDateTime);
                  break;
                case ServerDefaultType.CallerIdentity:
                  defaultFieldValue = new ServerDefaultFieldValue(ServerDefaultType.CallerIdentity);
                  break;
                case ServerDefaultType.RandomGuid:
                  defaultFieldValue = new ServerDefaultFieldValue(ServerDefaultType.RandomGuid);
                  break;
              }
              string fieldRefName3;
              if (this.TryGetFieldRefNameById((TestManagementRequestContext) requestContext, ruleRecord.ThenFldID, out fieldRefName3))
              {
                if (!requiredFieldIds.Contains(fieldRefName3))
                  requiredFieldIds.Add(fieldRefName3);
                if (!fieldTdToServerDefaultValueMap.ContainsKey(fieldRefName3))
                  fieldTdToServerDefaultValueMap.Add(fieldRefName3, defaultFieldValue);
              }
            }
          }
        }
        requestContext.TraceLeave("BusinessLayer", "WITHelper.GetRequiredFieldsData");
      }
    }

    public IEnumerable<RuleRecord> GetRulesForWorkItemType(
      TfsTestManagementRequestContext requestContext,
      string teamProjectName,
      string workItemType)
    {
      int treeNodeIdFromPath = this.Wrapper_LegacyGetTreeNodeIdFromPath(requestContext.RequestContext, teamProjectName, TreeStructureType.None);
      return treeNodeIdFromPath > 0 ? this.Wrapper_GetRules(requestContext.RequestContext, treeNodeIdFromPath, workItemType) : (IEnumerable<RuleRecord>) null;
    }

    public IDictionary<int, string> GetWorkItemAreaUris(
      TestManagementRequestContext context,
      IEnumerable<int> workItemIds,
      bool skipPermissionCheck)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "WITHelper.GetWorkItemAreaUris");
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "WITHelper.GetWorkItemAreaUris"))
        {
          IDictionary<int, string> workItemAreaUris = (IDictionary<int, string>) new Dictionary<int, string>();
          if (workItemIds != null && workItemIds.Count<int>() > 0)
          {
            IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
            IVssRequestContext requestContext = context.RequestContext;
            List<int> list = workItemIds.Distinct<int>().ToList<int>();
            List<string> fields = new List<string>();
            fields.Add("System.Id");
            fields.Add("System.AreaPath");
            int num = skipPermissionCheck ? 1 : 0;
            foreach (WorkItem workItem in this.GetWorkItems(requestContext, list, fields, num != 0, (string) null))
            {
              int key = workItem.Id.Value;
              string str;
              if (workItem.Fields.TryGetValue<string, string>("System.AreaPath", out str) && !string.IsNullOrEmpty(str))
              {
                string uri;
                if (!dictionary.TryGetValue(str, out uri))
                {
                  uri = context.AreaPathsCache.GetCssNodeAndThrow(context, this.TranslateCSSPath(str)).Uri;
                  dictionary[str] = uri;
                }
                workItemAreaUris[key] = uri;
              }
            }
          }
          return workItemAreaUris;
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "WITHelper.GetWorkItemAreaUris");
      }
    }

    public IDictionary<int, CachedWorkItemData> GetWorkItemCacheData(
      TestManagementRequestContext context,
      IEnumerable<int> workItemIds,
      bool skipPermissionCheck)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "WITHelper.GetWorkItemCacheData");
        using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "WITHelper.GetWorkItemCacheData"))
        {
          IDictionary<int, CachedWorkItemData> workItemCacheData = (IDictionary<int, CachedWorkItemData>) new Dictionary<int, CachedWorkItemData>();
          if (workItemIds != null && workItemIds.Count<int>() > 0)
          {
            IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
            IVssRequestContext requestContext = context.RequestContext;
            List<int> list = workItemIds.Distinct<int>().ToList<int>();
            List<string> fields = new List<string>();
            fields.Add("System.Id");
            fields.Add("System.AreaPath");
            fields.Add("System.Title");
            int num = skipPermissionCheck ? 1 : 0;
            foreach (WorkItem workItem in this.GetWorkItems(requestContext, list, fields, num != 0, (string) null))
            {
              string str1;
              if (workItem.Fields.TryGetValue<string, string>("System.Title", out str1))
              {
                CachedWorkItemData cachedWorkItemData = new CachedWorkItemData()
                {
                  Id = workItem.Id.Value,
                  Title = str1
                };
                string str2;
                if (workItem.Fields.TryGetValue<string, string>("System.AreaPath", out str2) && !string.IsNullOrEmpty(str2))
                {
                  string uri;
                  if (!dictionary.TryGetValue(str2, out uri))
                  {
                    uri = context.AreaPathsCache.GetCssNodeAndThrow(context, this.TranslateCSSPath(str2)).Uri;
                    dictionary[str2] = uri;
                  }
                  cachedWorkItemData.AreaUri = uri;
                }
                workItemCacheData[cachedWorkItemData.Id] = cachedWorkItemData;
              }
            }
          }
          return workItemCacheData;
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "WITHelper.GetWorkItemCacheData");
      }
    }

    public List<int> QueryWorkItems(
      TestManagementRequestContext context,
      string teamProjectName,
      string queryString,
      bool skipWiqlTextLimitValidation = false,
      int top = 2147483647)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (QueryWorkItems), "WorkItem")))
      {
        context.TraceEnter("BusinessLayer", "WITHelper.QueryWorkItems");
        context.IfEmptyThenTraceAndDebugFail(teamProjectName, "BusinessLayer", nameof (teamProjectName));
        context.IfEmptyThenTraceAndDebugFail(queryString, "BusinessLayer", nameof (queryString));
        context.TraceVerbose("BusinessLayer", "QueryWorkItems - query:{0}", (object) queryString);
        new Dictionary<string, object>()
        {
          {
            "project",
            (object) teamProjectName
          }
        };
        IWiqlRemotableService service = context.RequestContext.GetService<IWiqlRemotableService>();
        IVssRequestContext requestContext = context.RequestContext;
        string wiql = queryString;
        string projectName = teamProjectName;
        bool flag = skipWiqlTextLimitValidation;
        int? nullable = new int?(top);
        bool? timePrecision = new bool?();
        int? top1 = nullable;
        int num = flag ? 1 : 0;
        WorkItemQueryResult workItemQueryResult = service.QueryByWiql(requestContext, wiql, projectName, timePrecision, top1, num != 0);
        List<int> intList = new List<int>();
        if (workItemQueryResult.WorkItems != null)
          intList = workItemQueryResult.WorkItems.Select<WorkItemReference, int>((Func<WorkItemReference, int>) (wit => wit.Id)).ToList<int>();
        context.TraceLeave("BusinessLayer", "WITHelper.QueryWorkItems");
        return intList;
      }
    }

    internal IEnumerable<string> GetWorkItemTypesForWorkItemCategory(
      IVssRequestContext context,
      Guid projectId,
      string witCategoryRefName)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItemTypesForWorkItemCategory), "WorkItem")))
      {
        context.TraceEnter("BusinessLayer", string.Format("WITHelper.GetWorkItemTypesForWorkItemCategory , Parameters ProjectId = {0} , witCategoryRefName = {1}", (object) projectId, (object) witCategoryRefName));
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory itemTypeCategory = context.GetService<IWorkItemTypeCategoryRemotableService>().GetWorkItemTypeCategory(context, projectId, witCategoryRefName);
        context.TraceLeave("BusinessLayer", "WITHelper.GetWorkItemTypesForWorkItemCategory");
        IEnumerable<WorkItemTypeReference> workItemTypes = itemTypeCategory.WorkItemTypes;
        return workItemTypes != null ? (IEnumerable<string>) workItemTypes.Select<WorkItemTypeReference, string>((Func<WorkItemTypeReference, string>) (type => type.Name)).ToList<string>() : (IEnumerable<string>) null;
      }
    }

    internal void ValidateWiqlQuery(
      IVssRequestContext requestContext,
      string query,
      Guid projectId)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (ValidateWiqlQuery), "WorkItem")))
        requestContext.GetService<IWorkItemQueryRemotableService>().ValidateQuery(requestContext, projectId, query);
    }

    internal bool TryGetFieldRefNameById(
      TestManagementRequestContext context,
      int fieldId,
      out string fieldRefName)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (TryGetFieldRefNameById), "WorkItem")))
      {
        fieldRefName = string.Empty;
        FieldEntry fieldById = context.RequestContext.GetService<WorkItemTrackingFieldService>().GetFieldById(context.RequestContext, fieldId);
        if (fieldById == null)
          return false;
        fieldRefName = fieldById.ReferenceName;
        return true;
      }
    }

    internal bool ValidateCategoriesPresent(
      TestManagementRequestContext context,
      string teamProjectName,
      List<string> witCategories)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "WITHelper.ValidateCategoriesPresent");
        bool flag = false;
        try
        {
          List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory> itemTypeCategories = this.GetWorkItemTypeCategories(context.RequestContext, teamProjectName, (IEnumerable<string>) witCategories);
          if (itemTypeCategories != null)
          {
            if (itemTypeCategories.Count == 2)
              flag = true;
          }
        }
        catch (WorkItemTypeCategoryNotFoundException ex)
        {
          flag = false;
        }
        return flag;
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "WITHelper.ValidateCategoriesPresent");
      }
    }

    internal IDictionary<string, HashSet<string>> GetStateTransitions(
      TestManagementRequestContext context,
      string projectName,
      string workItemType)
    {
      using (PerfManager.Measure(context.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (GetStateTransitions), "WorkItem")))
      {
        context.TraceEnter("BusinessLayer", "WITHelper.GetStateTransitions");
        IDictionary<string, HashSet<string>> stateTransitions = (IDictionary<string, HashSet<string>>) new Dictionary<string, HashSet<string>>();
        IWorkItemTypeRemotableService service = context.RequestContext.GetService<IWorkItemTypeRemotableService>();
        Guid projectGuid = context.ProjectServiceHelper.GetProjectGuid(projectName);
        IVssRequestContext requestContext = context.RequestContext;
        Guid projectId = projectGuid;
        string witNameOrReferenceName = workItemType;
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType workItemType1 = service.GetWorkItemType(requestContext, projectId, witNameOrReferenceName);
        if (workItemType1 == null || workItemType1.Transitions == null)
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.CouldNotFindTransitionsForWorkItemType, (object) workItemType));
        foreach (KeyValuePair<string, WorkItemStateTransition[]> transition in (IEnumerable<KeyValuePair<string, WorkItemStateTransition[]>>) workItemType1.Transitions)
        {
          if (transition.Value != null)
          {
            stateTransitions.Add(transition.Key, new HashSet<string>());
            foreach (WorkItemStateTransition itemStateTransition in transition.Value)
              stateTransitions[transition.Key].Add(itemStateTransition.To);
          }
          else
            stateTransitions.Add(transition.Key, (HashSet<string>) null);
        }
        context.TraceLeave("BusinessLayer", "WITHelper.GetStateTransitions");
        return stateTransitions;
      }
    }

    internal virtual IEnumerable<RuleRecord> Wrapper_GetRules(
      IVssRequestContext requestContext,
      int projectId,
      string workItemType)
    {
      return CompatibilityRulesGenerator.GetRules(requestContext, projectId, workItemType);
    }

    internal virtual int Wrapper_LegacyGetTreeNodeIdFromPath(
      IVssRequestContext RequestContext,
      string approxValue,
      TreeStructureType type)
    {
      return RequestContext.WitContext().TreeService.LegacyGetTreeNodeIdFromPath(RequestContext, approxValue, type);
    }

    private JsonPatchDocument CreateJsonPatchForLinkFromRequirement(
      IVssRequestContext requestContext,
      int requirementId,
      int testcaseId)
    {
      JsonPatchDocument linkFromRequirement = new JsonPatchDocument();
      WorkItem workItem = requestContext.GetService<IWorkItemRemotableService>().GetWorkItem(requestContext, testcaseId, (IEnumerable<string>) new List<string>());
      JsonPatchOperation jsonPatchOperation = new JsonPatchOperation();
      jsonPatchOperation.Operation = Operation.Add;
      jsonPatchOperation.Path = WorkItemLinkPath.Relation;
      WorkItemRelation workItemRelation = new WorkItemRelation((ISecuredObject) workItem);
      workItemRelation.Rel = TestCaseClonerConstants.TestedByForwardLinkName;
      workItemRelation.Url = workItem.Url;
      jsonPatchOperation.Value = (object) workItemRelation;
      linkFromRequirement.Add(jsonPatchOperation);
      return linkFromRequirement;
    }

    private JsonPatchDocument CreateJsonPatchForUnlinkFromRequirement(
      IVssRequestContext requestContext,
      int requirementId,
      int[] testcaseIds)
    {
      JsonPatchDocument unlinkFromRequirement = new JsonPatchDocument();
      List<int> intList = new List<int>();
      IWorkItemRemotableService service = requestContext.GetService<IWorkItemRemotableService>();
      HashSet<string> stringSet = new HashSet<string>(service.GetWorkItems(requestContext, (IEnumerable<int>) testcaseIds, (IEnumerable<string>) new List<string>()).Select<WorkItem, string>((Func<WorkItem, string>) (workItem => workItem.Url.ToString())), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      WorkItem workItem1 = service.GetWorkItem(requestContext, requirementId, (IEnumerable<string>) new List<string>(), expand: WorkItemExpand.Relations);
      if (workItem1.Relations == null)
        return unlinkFromRequirement;
      for (int index = 0; index < workItem1.Relations.Count; ++index)
      {
        if (stringSet.Contains(workItem1.Relations[index].Url))
          intList.Add(index);
      }
      foreach (int num in intList)
        unlinkFromRequirement.Add(new JsonPatchOperation()
        {
          Operation = Operation.Remove,
          Path = string.Format(WorkItemLinkPath.RelationStringFormat, (object) num)
        });
      return unlinkFromRequirement;
    }

    private bool IsClonelink(WorkItemRelation workItemRelation)
    {
      string str;
      return string.CompareOrdinal(workItemRelation.Rel, "System.LinkTypes.Hierarchy-Forward") != 0 && (string.CompareOrdinal(workItemRelation.Rel, "System.LinkTypes.Related") != 0 || workItemRelation.Attributes == null || !workItemRelation.Attributes.TryGetValue<string, string>("comment", out str) || !str.Contains("TF237027"));
    }

    private bool IsServerDefaultRule(RuleRecord ruleRecord, out ServerDefaultType from)
    {
      from = ServerDefaultType.ServerDateTime;
      if (ruleRecord.If2FldID != 0 || !ruleRecord.Check(RuleFlags.Default, (RuleFlags) 265019392))
        return false;
      switch (ruleRecord.ThenConstID)
      {
        case -10031:
          from = ServerDefaultType.RandomGuid;
          break;
        case -10026:
          from = ServerDefaultType.CallerIdentity;
          break;
        case -10013:
          from = ServerDefaultType.ServerDateTime;
          break;
        default:
          return false;
      }
      return true;
    }

    private bool IsRequiredRule(RuleRecord ruleRecord) => ruleRecord.Fld2ID == 0 && ruleRecord.ThenConstID == -10000 && ruleRecord.IfFldID == 0 && ruleRecord.If2FldID == 0 && ruleRecord.Check(RuleFlags.DenyWrite | RuleFlags.Unless | RuleFlags.ThenNot, RuleFlags.ThenAllNodes | RuleFlags.ThenLike);

    private bool IsDefaultRule(RuleRecord ruleRecord) => ((IEnumerable<int>) new int[4]
    {
      ruleRecord.Fld1ID,
      ruleRecord.Fld2ID,
      ruleRecord.Fld3ID,
      ruleRecord.Fld4ID
    }).Contains<int>(ruleRecord.ThenFldID) && (ruleRecord.RuleFlags & RuleFlags.Default) == RuleFlags.Default;
  }
}
