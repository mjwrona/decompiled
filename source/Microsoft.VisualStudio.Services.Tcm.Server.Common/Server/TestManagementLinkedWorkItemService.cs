// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementLinkedWorkItemService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestManagementLinkedWorkItemService : 
    TeamFoundationTestManagementService,
    ITestManagementLinkedWorkItemService,
    IVssFrameworkService
  {
    private const int c_defaultTestCaseBatchSize = 100;
    private const int c_defaultTestSuiteBatchSize = 5;
    private const int c_defaultWorkItemCount = 200;

    public List<LinkedWorkItemsQueryResult> GetLinkedWorkItemsByQuery(
      TestManagementRequestContext requestContext,
      ProjectInfo projectInfo,
      LinkedWorkItemsQuery workItemQuery)
    {
      using (PerfManager.Measure(requestContext.RequestContext, "RestLayer", "TestManagementLinkedWorkItemService.GetLinkedWorkItemsByQuery", 50, true))
      {
        ArgumentUtility.CheckForNull<LinkedWorkItemsQuery>(workItemQuery, nameof (workItemQuery), "Test Results");
        if (string.IsNullOrEmpty(workItemQuery.WorkItemCategory))
          throw new InvalidPropertyException("workItemQuery.WorkItemCategory", ServerResources.InvalidPropertyMessage);
        if ((workItemQuery.AutomatedTestNames == null || !workItemQuery.AutomatedTestNames.Any<string>()) && (workItemQuery.TestCaseIds == null || !workItemQuery.TestCaseIds.Any<int>()) && workItemQuery.PlanId <= 0)
          throw new InvalidPropertyException(nameof (workItemQuery), ServerResources.RequiredLinkedWorkItemQueryParametersMissing);
        if (workItemQuery.PlanId > 0)
        {
          if ((workItemQuery.SuiteIds == null || !workItemQuery.SuiteIds.Any<int>()) && (workItemQuery.PointIds == null || !workItemQuery.PointIds.Any<int>()))
            throw new InvalidPropertyException(nameof (workItemQuery), ServerResources.PointOrSuiteIdMissingInLinkedWorkItemQueryParameters);
          if (workItemQuery.SuiteIds != null && workItemQuery.SuiteIds.Any<int>() && workItemQuery.PointIds != null && workItemQuery.PointIds.Any<int>())
            throw new InvalidPropertyException(nameof (workItemQuery), ServerResources.BothSuiteAndPointSpecifiedInQuery);
          if (workItemQuery.SuiteIds != null && workItemQuery.SuiteIds.Count > 5)
            throw new InvalidPropertyException(nameof (workItemQuery), string.Format(ServerResources.MaximumSuitesExceeded, (object) 5));
        }
        if (workItemQuery.TestCaseIds != null && workItemQuery.TestCaseIds.Count > 100 || workItemQuery.AutomatedTestNames != null && workItemQuery.AutomatedTestNames.Count > 100 || workItemQuery.PointIds != null && workItemQuery.PointIds.Count > 100)
          throw new InvalidPropertyException(nameof (workItemQuery), string.Format(ServerResources.MaximumTestCaseIdsExceeded, (object) 100));
        List<TestCaseReference> testCaseReferenceList = new List<TestCaseReference>();
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
          testCaseReferenceList = managementDatabase.QueryTestCaseReference(projectInfo.Id, workItemQuery.AutomatedTestNames, workItemQuery.TestCaseIds, workItemQuery.PlanId, workItemQuery.SuiteIds, workItemQuery.PointIds);
        Dictionary<string, TestCaseReference> dictionary = new Dictionary<string, TestCaseReference>();
        bool isInTcm = requestContext.RequestContext.ServiceInstanceType() == TestManagementServerConstants.TCMServiceInstanceType;
        foreach (TestCaseReference testCaseReference in testCaseReferenceList)
        {
          if (isInTcm)
          {
            dictionary.Add(TestManagementServiceUtility.GetArtiFactUriForTestCaseReference(testCaseReference.TestCaseReferenceId, isInTcm), testCaseReference);
            dictionary.Add(TestManagementServiceUtility.GetArtiFactUriForTestCaseReference(testCaseReference.TestCaseReferenceId, !isInTcm), testCaseReference);
          }
          else
            dictionary.Add(TestManagementServiceUtility.GetArtiFactUriForTestCaseReference(testCaseReference.TestCaseReferenceId, isInTcm), testCaseReference);
        }
        Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> recordsForArtifactUris = this.BatchCreateWorkItemsRecordsForArtifactUris(requestContext, projectInfo.Id, workItemQuery.WorkItemCategory, dictionary.Keys.ToList<string>(), 0);
        List<LinkedWorkItemsQueryResult> workItemsByQuery = new List<LinkedWorkItemsQueryResult>();
        foreach (string key in dictionary.Keys)
        {
          TestCaseReference testCaseReference = dictionary[key];
          LinkedWorkItemsQueryResult itemsQueryResult = new LinkedWorkItemsQueryResult()
          {
            AutomatedTestName = testCaseReference.AutomatedTestName,
            TestCaseId = testCaseReference.TestCaseId,
            PointId = testCaseReference.PointId,
            PlanId = testCaseReference.PlanId,
            SuiteId = testCaseReference.SuiteId,
            WorkItems = new List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>()
          };
          if (recordsForArtifactUris.ContainsKey(key))
            itemsQueryResult.WorkItems = recordsForArtifactUris[key];
          workItemsByQuery.Add(itemsQueryResult);
        }
        return workItemsByQuery;
      }
    }

    public Dictionary<TestCaseResultIdentifier, List<int>> GetWorkItemIdsAssociatedToTestResults(
      TestCaseResult[] results,
      TestManagementRequestContext tcmRequestContext,
      string projectName)
    {
      using (PerfManager.Measure(tcmRequestContext.RequestContext, "BusinessLayer", "TestManagementLinkedWorkItemService.GetWorkItemIdsAssociatedToTestResults"))
        return this.ExecuteAction<Dictionary<TestCaseResultIdentifier, List<int>>>(tcmRequestContext.RequestContext, "TestManagementLinkedWorkItemService.GetWorkItemIdsAssociatedToTestResults", (Func<Dictionary<TestCaseResultIdentifier, List<int>>>) (() =>
        {
          ProjectInfo projectFromName = tcmRequestContext.ProjectServiceHelper.GetProjectFromName(projectName);
          List<string> list = ((IEnumerable<TestCaseResult>) results).Select<TestCaseResult, string>((Func<TestCaseResult, string>) (r => TestManagementServiceUtility.GetArtiFactUriForTestResult(r))).ToList<string>();
          Dictionary<TestCaseResultIdentifier, List<int>> associatedToTestResults = new Dictionary<TestCaseResultIdentifier, List<int>>();
          ArtifactUriQueryResult artifactUriQueryResult = !tcmRequestContext.IsFeatureEnabled("TestManagement.Server.QueryWorkItemsForArtifactUris.SqlReadReplica") ? tcmRequestContext.WorkItemServiceHelper.GetLinkedWorkItemIds(projectFromName.Id, (IList<string>) list) : tcmRequestContext.WorkItemServiceHelper.GetLinkedWorkItemIdsReadReplica(projectFromName.Id, (IList<string>) list);
          if (artifactUriQueryResult != null)
          {
            IDictionary<string, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>> artifactUrisQueryResult = artifactUriQueryResult.ArtifactUrisQueryResult;
            for (int index = 0; index < results.Length; ++index)
            {
              if (artifactUrisQueryResult.ContainsKey(list.ElementAt<string>(index)))
                associatedToTestResults.Add(results[index].Id, artifactUrisQueryResult[list.ElementAt<string>(index)].OrderBy<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference, int>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference, int>) (wi => wi.Id)).Select<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference, int>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference, int>) (wi => wi.Id)).ToList<int>());
            }
          }
          return associatedToTestResults;
        }), 1015095, "TestResultsInsights");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> BatchCreateWorkItemsRecordsForResults(
      TestManagementRequestContext requestContext,
      Guid projectId,
      string workItemCategory,
      List<TestCaseResult> results,
      int workItemCount)
    {
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>(requestContext.RequestContext, "TestManagementLinkedWorkItemService.BatchCreateWorkItemsRecordsForResults", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>) (() => this.BatchCreateWorkItemsRecordsForArtifactUris(requestContext, projectId, workItemCategory, results.Select<TestCaseResult, string>((Func<TestCaseResult, string>) (r => TestManagementServiceUtility.GetArtiFactUriForTestResult(r))).ToList<string>(), workItemCount).SelectMany<KeyValuePair<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>((Func<KeyValuePair<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>, IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>) (r => (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>) r.Value)).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>()), 1015095, "TestResultsInsights");
    }

    public Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> BatchCreateWorkItemsRecordsForTestCaseReference(
      TestManagementRequestContext requestContext,
      Guid projectId,
      string workItemCategory,
      List<int> testCaseRefIds,
      int workItemCount)
    {
      return !testCaseRefIds.Any<int>() ? new Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>() : this.ExecuteAction<Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>>(requestContext.RequestContext, "TestManagementLinkedWorkItemService.BatchCreateWorkItemsRecordsForTestCaseReference", (Func<Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>>) (() =>
      {
        bool isRequestInTcm = requestContext.RequestContext.ServiceInstanceType() == TestManagementServerConstants.TCMServiceInstanceType;
        return this.BatchCreateWorkItemsRecordsForArtifactUris(requestContext, projectId, workItemCategory, testCaseRefIds.Select<int, string>((Func<int, string>) (t => TestManagementServiceUtility.GetArtiFactUriForTestCaseReference(t, isRequestInTcm))).ToList<string>(), workItemCount);
      }), 1015095, "TestResultsInsights");
    }

    public Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> BatchCreateWorkItemsRecordsForArtifactUris(
      TestManagementRequestContext requestContext,
      Guid projectId,
      string workItemCategory,
      List<string> artifactUris,
      int workItemCount)
    {
      int associatedWorkItems = TestManagementServiceUtility.GetBatchSizeToFetchAssociatedWorkItems(requestContext.RequestContext);
      int index1 = 0;
      ArtifactUriQueryResult artifactUriQueryResult1 = (ArtifactUriQueryResult) null;
      int count1;
      for (; index1 < artifactUris.Count; index1 += count1)
      {
        count1 = artifactUris.Count - index1 > associatedWorkItems ? associatedWorkItems : artifactUris.Count - index1;
        ArtifactUriQueryResult artifactUriQueryResult2 = !requestContext.IsFeatureEnabled("TestManagement.Server.QueryWorkItemsForArtifactUris.SqlReadReplica") ? requestContext.WorkItemServiceHelper.GetLinkedWorkItemIds(projectId, (IList<string>) artifactUris.GetRange(index1, count1)) : requestContext.WorkItemServiceHelper.GetLinkedWorkItemIdsReadReplica(projectId, (IList<string>) artifactUris.GetRange(index1, count1));
        if (artifactUriQueryResult1 == null)
          artifactUriQueryResult1 = artifactUriQueryResult2;
        else if (artifactUriQueryResult2 != null)
          artifactUriQueryResult1.ArtifactUrisQueryResult.AddRange<KeyValuePair<string, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>>, IDictionary<string, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>>>((IEnumerable<KeyValuePair<string, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>>>) artifactUriQueryResult2.ArtifactUrisQueryResult);
      }
      List<int> list1 = artifactUriQueryResult1 != null ? artifactUriQueryResult1.ArtifactUrisQueryResult.Select<KeyValuePair<string, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>>, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>>((Func<KeyValuePair<string, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>>, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>>) (ar => ar.Value)).SelectMany<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>((Func<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>>) (ar => ar)).Select<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference, int>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference, int>) (ar => ar.Id)).Distinct<int>().OrderBy<int, int>((Func<int, int>) (id => id)).ToList<int>() : (List<int>) null;
      Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> recordsForArtifactUris = new Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>();
      if (list1 != null && list1.Any<int>())
      {
        int index2 = 0;
        HashSet<string> stringSet = new HashSet<string>();
        if (workItemCategory.Equals(WitCategoryRefName.AllWorkItemsCategory, StringComparison.OrdinalIgnoreCase))
        {
          stringSet.Add(workItemCategory);
        }
        else
        {
          WorkItemTypeCategory itemTypeCategory = requestContext.WorkItemServiceHelper.GetWorkItemTypeCategory(projectId, workItemCategory);
          if (itemTypeCategory != null)
            stringSet.AddRange<string, HashSet<string>>(itemTypeCategory.WorkItemTypes.Select<WorkItemTypeReference, string>((Func<WorkItemTypeReference, string>) (wi => wi.Name)));
        }
        workItemCount = workItemCount == 0 ? 200 : workItemCount;
        List<int> list2 = list1.Take<int>(workItemCount).ToList<int>();
        Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> dictionary = new Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>();
        int count2;
        for (; index2 < list2.Count; index2 += count2)
        {
          count2 = list2.Count - index2 > associatedWorkItems ? associatedWorkItems : list2.Count - index2;
          Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> itemsInfoFromIds = this.GetLinkedWorkItemsInfoFromIds(requestContext, projectId, list2.GetRange(index2, count2), (IEnumerable<string>) stringSet);
          if (itemsInfoFromIds != null && itemsInfoFromIds.Any<KeyValuePair<int, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>())
          {
            foreach (int key in itemsInfoFromIds.Keys)
            {
              int result;
              if (int.TryParse(itemsInfoFromIds[key].Id, out result) && !string.IsNullOrEmpty(itemsInfoFromIds[key].Name))
                dictionary[result] = new Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference()
                {
                  Id = itemsInfoFromIds[key].Id,
                  Name = itemsInfoFromIds[key].Name
                };
            }
          }
        }
        foreach (KeyValuePair<string, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>> keyValuePair in (IEnumerable<KeyValuePair<string, IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference>>>) artifactUriQueryResult1.ArtifactUrisQueryResult)
        {
          recordsForArtifactUris[keyValuePair.Key] = new List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>();
          foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference workItemReference in keyValuePair.Value)
          {
            if (dictionary.ContainsKey(workItemReference.Id))
              recordsForArtifactUris[keyValuePair.Key].Add(dictionary[workItemReference.Id]);
          }
        }
      }
      return recordsForArtifactUris;
    }

    private Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> GetLinkedWorkItemsInfoFromIds(
      TestManagementRequestContext requestContext,
      Guid projectId,
      List<int> workItemIds,
      IEnumerable<string> workItemTypeNames)
    {
      using (PerfManager.Measure(requestContext.RequestContext, "RestLayer", "TeamFoundationTestManagementResultService.GetLinkedBugsInfoFromIds"))
      {
        Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> itemsInfoFromIds = new Dictionary<int, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>();
        if (workItemIds.Any<int>())
        {
          IEnumerable<int> source1 = workItemIds.Distinct<int>();
          string[] source2 = new string[3]
          {
            "System.Id",
            "System.Title",
            "System.WorkItemType"
          };
          IList<WorkItem> workItems = requestContext.WorkItemServiceHelper.GetWorkItems(projectId, (IList<int>) source1.ToList<int>(), (IList<string>) ((IEnumerable<string>) source2).ToList<string>(), WorkItemExpand.None, WorkItemErrorPolicy.Omit);
          if (workItems != null)
          {
            foreach (WorkItem workItem in (IEnumerable<WorkItem>) workItems)
            {
              string field1 = workItem.Fields[WorkItemFieldRefNames.Title] as string;
              int key = workItem.Id.Value;
              string field2 = workItem.Fields[WorkItemFieldRefNames.WorkItemType] as string;
              if (!string.IsNullOrEmpty(field2) && (workItemTypeNames.Contains<string>(field2, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) || workItemTypeNames.Contains<string>(WitCategoryRefName.AllWorkItemsCategory, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)))
              {
                Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference workItemReference = new Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference()
                {
                  Id = key.ToString(),
                  Name = field1,
                  Type = field2,
                  Url = UrlBuildHelper.GetResourceUrl(requestContext.RequestContext, TestManagementServerConstants.TFSServiceInstanceType, "wit", WitConstants.WorkItemTrackingLocationIds.WorkItems, (object) new
                  {
                    id = key
                  })
                };
                itemsInfoFromIds[key] = workItemReference;
              }
            }
          }
        }
        return itemsInfoFromIds;
      }
    }
  }
}
