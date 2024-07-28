// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RequirementCloner
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class RequirementCloner : WITCreator
  {
    private Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory m_requirementCategory;

    public RequirementCloner(TfsTestManagementRequestContext requestContext)
      : base((TestManagementRequestContext) requestContext)
    {
    }

    public int CloneRequirement(
      int requirementId,
      Dictionary<int, int> testCases,
      bool suppressNotifications)
    {
      int newRequirementId = -1;
      try
      {
        RetryHelper.RetryOnExceptions((Action) (() => newRequirementId = this.CloneRequirementWithoutRetry(this.options.DestinationProjectName, requirementId, testCases, suppressNotifications)), 1, typeof (WorkItemTrackingServiceException));
        if (newRequirementId == -1)
          return -1;
        using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(this.RequestContext))
          planningDatabase.UpdateCloneRelationship(this.opId, new Dictionary<int, int>()
          {
            {
              requirementId,
              newRequirementId
            }
          }, CloneItemType.Requirement);
      }
      catch (WorkItemTrackingServiceException ex)
      {
        this.RequestContext.TraceException(0, "TestManagement", "BusinessLayer", (Exception) ex);
        throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyFailedWithWITError, (object) requirementId, (object) ex.Message));
      }
      catch (TestManagementServiceException ex)
      {
        this.RequestContext.TraceException(0, "TestManagement", "BusinessLayer", (Exception) ex);
        throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DeepCopyFailedForRequirement, (object) requirementId, (object) ex.Message));
      }
      return newRequirementId;
    }

    public int CloneRequirementWithoutRetry(
      string projectName,
      int requirementId,
      Dictionary<int, int> testCases,
      bool suppressNotifications)
    {
      using (PerfManager.Measure(this.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (CloneRequirementWithoutRetry), "WorkItem")))
      {
        this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "Method CloneRequirementNewApi Old RequirementId : {0}", (object) requirementId);
        WITCreator witCreator = new WITCreator(this.TcmRequestContext);
        IWorkItemRemotableService service = this.RequestContext.GetService<IWorkItemRemotableService>();
        WorkItem workItem;
        try
        {
          workItem = service.GetWorkItem(this.RequestContext, requirementId, (IEnumerable<string>) new List<string>(), expand: WorkItemExpand.Relations);
        }
        catch (WorkItemNotFoundException ex)
        {
          return -1;
        }
        IEnumerable<WorkItemRelation> customizedRelations = this.GetCustomizedRelations(requirementId, workItem, testCases);
        Dictionary<string, object> customizedFieldUpdates = this.GetCustomizedFieldUpdates(requirementId, workItem);
        witCreator.AddRelatedLinkInfo(workItem.Url, (IList<WorkItemRelation>) customizedRelations.ToList<WorkItemRelation>());
        JsonPatchDocument jsonPatchDocument = WorkHelper.ConvertToJsonPatchDocument(this.RequestContext, (IDictionary<string, object>) customizedFieldUpdates, (IList<WorkItemRelation>) customizedRelations.ToList<WorkItemRelation>());
        string workItemtype = workItem.Fields[CoreFieldReferenceNames.WorkItemType].ToString();
        int? id = service.CreateWorkItem(this.RequestContext, projectName, workItemtype, jsonPatchDocument, bypassRules: true, suppressNotifications: suppressNotifications).Id;
        this.RequestContext.Trace(0, TraceLevel.Verbose, "TestManagement", "Database", "Method CloneRequirementNewApi New RequirementId : {0}", (object) id);
        return id.HasValue ? id.Value : -1;
      }
    }

    protected override void PostInitialize(CloneOperationInformation options)
    {
      IWitHelper service = this.RequestContext.GetService<IWitHelper>();
      this.RequestContext.GetService<WorkItemTypeCategoryService>();
      this.m_requirementCategory = service.GetWorkItemTypeCategories(this.RequestContext, this.options.DestinationProjectName, (IEnumerable<string>) new List<string>()
      {
        "Microsoft.RequirementCategory"
      }).FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>();
    }

    private Dictionary<string, object> GetCustomizedFieldUpdates(int requirementId, WorkItem wi)
    {
      Dictionary<string, object> cloneableFieldData = this.GetCloneableFieldData(wi);
      this.CustomizeFields(requirementId, cloneableFieldData);
      return cloneableFieldData.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key.ToString((IFormatProvider) CultureInfo.CurrentCulture)), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value));
    }

    private void CustomizeFields(int requirementId, Dictionary<string, object> requirementFields)
    {
      this.CustomizeExplicitlyOverriddenFields((IDictionary<string, object>) requirementFields, true);
      this.CustomizeWorkitemType(requirementFields);
      this.UpdateSpecialFields(requirementId, (IDictionary<string, object>) requirementFields);
      this.UpdateIdentityFields((IDictionary<string, object>) requirementFields);
    }

    private void CustomizeWorkitemType(Dictionary<string, object> requirementFields)
    {
      string oldtype = requirementFields[WorkItemFieldRefNames.WorkItemType] as string;
      IEnumerable<WorkItemTypeReference> workItemTypes = this.m_requirementCategory.WorkItemTypes;
      if ((workItemTypes != null ? (!workItemTypes.Any<WorkItemTypeReference>((Func<WorkItemTypeReference, bool>) (type => TFStringComparer.WorkItemTypeName.Equals(type.Name, oldtype))) ? 1 : 0) : 0) == 0)
        return;
      requirementFields[WorkItemFieldRefNames.WorkItemType] = (object) this.m_requirementCategory.DefaultWorkItemType?.Name;
    }

    private IEnumerable<WorkItemRelation> GetCustomizedRelations(
      int oldRequirementId,
      WorkItem workItem,
      Dictionary<int, int> testCases)
    {
      List<WorkItemRelation> customizedRelations = new List<WorkItemRelation>();
      if (workItem == null || workItem.Relations == null || testCases == null)
        return (IEnumerable<WorkItemRelation>) customizedRelations;
      IList<int> intList = (IList<int>) new List<int>();
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      IWorkItemRemotableService service = this.RequestContext.GetService<IWorkItemRemotableService>();
      IEnumerable<WorkItem> workItems = this.RequestContext.GetService<IWitHelper>().GetWorkItems(this.RequestContext, testCases.Select<KeyValuePair<int, int>, int>((Func<KeyValuePair<int, int>, int>) (kvp => kvp.Key)).ToList<int>(), new List<string>());
      foreach (WorkItemRelation relation1 in (IEnumerable<WorkItemRelation>) workItem.Relations)
      {
        WorkItemRelation relation = relation1;
        if (relation.Rel.Equals(TestCaseClonerConstants.TestedByForwardLinkName, StringComparison.OrdinalIgnoreCase))
        {
          IEnumerable<WorkItem> source = workItems.Where<WorkItem>((Func<WorkItem, bool>) (testcase => testcase.Url.Equals(relation.Url, StringComparison.OrdinalIgnoreCase)));
          if (source != null && source.Count<WorkItem>() >= 1)
          {
            int? id = source.FirstOrDefault<WorkItem>().Id;
            int num1;
            if (!id.HasValue)
            {
              num1 = -1;
            }
            else
            {
              id = source.FirstOrDefault<WorkItem>().Id;
              num1 = id.Value;
            }
            int key = num1;
            int num2;
            if (testCases.TryGetValue(key, out num2) && num2 > 0)
              intList.Add(num2);
          }
        }
      }
      foreach (int id in (IEnumerable<int>) intList)
      {
        WorkItem workItem1 = service.GetWorkItem(this.RequestContext, id, (IEnumerable<string>) new List<string>());
        WorkItemRelation workItemRelation1 = new WorkItemRelation();
        workItemRelation1.Rel = TestCaseClonerConstants.TestedByForwardLinkName;
        workItemRelation1.Url = workItem1.Url;
        WorkItemRelation workItemRelation2 = workItemRelation1;
        customizedRelations.Add(workItemRelation2);
      }
      return (IEnumerable<WorkItemRelation>) customizedRelations;
    }
  }
}
