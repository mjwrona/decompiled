// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementTraceabilityService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementTraceabilityService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementTraceabilityService,
    IVssFrameworkService
  {
    private IWorkItemHelper _workItemHelper;

    public WorkItemToTestLinks AddRequirementToTestLinks(
      TestManagementRequestContext context,
      GuidAndString projectId,
      int workItemId,
      List<TestMethod> testMethods)
    {
      if (!this.WorkItemHelper.BelongsToRequirementCategory(context.RequestContext, projectId, workItemId, true))
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.WorkItemNotBelongToRequirementCategory, (object) workItemId));
      Guid teamFoundationId = context.UserTeamFoundationId;
      if (context.IsFeatureEnabled("TestManagement.Server.AddTestCaseRefLinksToRequirement"))
      {
        List<TestCaseReference> unplannedTestCaseRefs = this.GetUnplannedTestCaseRefs(context, projectId.GuidId, testMethods);
        List<int> caseRefsIfNotExists = this.GetAndCreateTestCaseRefsIfNotExists(context, projectId, workItemId, unplannedTestCaseRefs, testMethods);
        bool isRequestInTcm = context.RequestContext.ServiceInstanceType() == TestManagementServerConstants.TCMServiceInstanceType;
        IWorkItemServiceHelper itemServiceHelper = context.WorkItemServiceHelper;
        List<string> list = caseRefsIfNotExists.Select<int, string>((Func<int, string>) (tcRefId => TestManagementServiceUtility.GetArtiFactUriForTestCaseReference(tcRefId, isRequestInTcm))).ToList<string>();
        Dictionary<string, object> attributes = new Dictionary<string, object>();
        attributes.Add(WorkItemLinkAttributes.Name, (object) WorkItemLinkedArtifactName.Test);
        int workItemId1 = workItemId;
        itemServiceHelper.LinkArtifactsToWorkItem(list, attributes, workItemId1);
      }
      if (!context.IsFeatureEnabled("TestManagement.Server.DisableOlderTestRequirementLink"))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          managementDatabase.AddRequirementToTestLinks(projectId, workItemId, testMethods, teamFoundationId);
      }
      return new WorkItemToTestLinks()
      {
        WorkItem = new Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference()
        {
          Id = workItemId.ToString()
        },
        Tests = testMethods
      };
    }

    public TestToWorkItemLinks QueryLinkedRequirementsForTest(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      TestMethod test)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TeamFoundationTestManagementTraceabilityService.QueryLinkedRequirementsForTest"))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> workItemReferenceList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>();
        TestToWorkItemLinks testToWorkItemLinks = new TestToWorkItemLinks()
        {
          Test = test,
          WorkItems = workItemReferenceList
        };
        if (context.IsFeatureEnabled("TestManagement.Server.AddTestCaseRefLinksToRequirement"))
        {
          List<int> filteredTestCaseRefs = this.GetFilteredTestCaseRefs(context, projectRef.Id, test);
          IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> source = context.RequestContext.GetService<ITestManagementLinkedWorkItemService>().BatchCreateWorkItemsRecordsForTestCaseReference(context, projectRef.Id, "Microsoft.RequirementCategory", filteredTestCaseRefs, 0).Values.SelectMany<List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>, IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>) (w => (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>) w));
          workItemReferenceList.AddRange(source.GroupBy<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, string>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, string>) (w => w?.Id ?? string.Empty)).Select<IGrouping<string, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>((Func<IGrouping<string, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>, Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>) (g => g.First<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>())));
        }
        if (!context.IsFeatureEnabled("TestManagement.Server.DisableOlderTestRequirementLink"))
        {
          HashSet<int> requirementIds;
          using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
            requirementIds = new HashSet<int>((IEnumerable<int>) managementDatabase.QueryLinkedRequirementsForTest(projectRef.Id, test));
          workItemReferenceList.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>) (wref =>
          {
            int int32 = Convert.ToInt32(wref.Id);
            if (!requirementIds.Contains(int32))
              return;
            requirementIds.Remove(int32);
          }));
          if (requirementIds.Any<int>())
            workItemReferenceList.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>) this.WorkItemHelper.GetWorkItemReference(context.RequestContext, projectRef.Id, requirementIds.ToList<int>(), true));
        }
        return testToWorkItemLinks;
      }
    }

    public void DeleteRequirementToTestLink(
      TestManagementRequestContext context,
      GuidAndString projectId,
      int workItemId,
      TestMethod testMethod)
    {
      Guid teamFoundationId = context.UserTeamFoundationId;
      if (testMethod != null && context.RequestContext.IsFeatureEnabled("TestManagement.Server.AddTestCaseRefLinksToRequirement"))
      {
        List<int> filteredTestCaseRefs = this.GetFilteredTestCaseRefs(context, projectId.GuidId, testMethod);
        if (filteredTestCaseRefs.Any<int>())
        {
          IEnumerable<WorkItem> workItems = this.WorkItemHelper.GetWorkItems(context.RequestContext, new List<int>()
          {
            workItemId
          }, true);
          if (workItems != null && workItems.Any<WorkItem>() && workItems.First<WorkItem>().Relations != null)
          {
            bool isRequestInTcm = context.RequestContext.ServiceInstanceType() == TestManagementServerConstants.TCMServiceInstanceType;
            HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) filteredTestCaseRefs.Select<int, string>((Func<int, string>) (refId => TestManagementServiceUtility.GetArtiFactUriForTestCaseReference(refId, isRequestInTcm))).ToList<string>());
            WorkItem workItem = workItems.First<WorkItem>();
            List<int> deletedIndices = new List<int>();
            for (int index = 0; index < workItem.Relations.Count; ++index)
            {
              if (stringSet.Contains(workItem.Relations[index].Url))
                deletedIndices.Add(index);
            }
            context.WorkItemServiceHelper.UnLinkArtifactsFromWorkItem(deletedIndices, workItemId);
          }
        }
      }
      if (context.IsFeatureEnabled("TestManagement.Server.DisableOlderTestRequirementLink"))
        return;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.DeleteRequirementToTestLink(projectId, workItemId, testMethod, teamFoundationId);
    }

    public void RestoreRequirementToTestLink(
      TestManagementRequestContext context,
      GuidAndString projectId,
      int workItemId)
    {
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.RestoreRequirementToTestLink(projectId, workItemId, teamFoundationId);
    }

    public void DestroyRequirementToTestLink(
      TestManagementRequestContext context,
      IEnumerable<int> workItemIds)
    {
      int batchSize = 50;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.DestroyRequirementToTestLink(workItemIds, batchSize);
    }

    public void SyncRequirementTestLinks(
      TestManagementRequestContext context,
      Guid projectId,
      int workItemId,
      IEnumerable<int> testCaseRefIds)
    {
      Guid teamFoundationId = context.UserTeamFoundationId;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.SyncRequirementTestLinks(projectId, workItemId, testCaseRefIds, teamFoundationId);
    }

    public List<TestSummaryForWorkItem> QueryAggregatedDataByRequirementForBuild(
      TestManagementRequestContext context,
      GuidAndString projectId,
      List<int> workItemIds,
      BuildConfiguration buildConfiguration)
    {
      Dictionary<int, TestSummaryForWorkItem> witToSummaryMap = new Dictionary<int, TestSummaryForWorkItem>();
      if (context.IsFeatureEnabled("TestManagement.Server.AddTestCaseRefLinksToRequirement2"))
        witToSummaryMap = this.QueryAggregatedDataByRequirementForBuild2(context, projectId, workItemIds, buildConfiguration);
      if (!context.IsFeatureEnabled("TestManagement.Server.DisableOlderTestRequirementLink"))
      {
        Dictionary<int, TestSummaryForWorkItem> summaryByWorkItem = new Dictionary<int, TestSummaryForWorkItem>();
        int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          summaryByWorkItem = managementDatabase.QueryAggregatedDataByRequirementForBuild(projectId, workItemIds, buildConfiguration, SourceWorkflow.ContinuousIntegration, progressOrFailed);
        witToSummaryMap = this.ReadWorkItemDetails(context, witToSummaryMap, summaryByWorkItem);
      }
      return witToSummaryMap.Values.ToList<TestSummaryForWorkItem>();
    }

    public List<TestSummaryForWorkItem> QueryAggregatedDataByRequirementForRelease(
      TestManagementRequestContext context,
      GuidAndString projectId,
      List<int> workItemIds,
      ReleaseReference releaseReference)
    {
      Dictionary<int, TestSummaryForWorkItem> witToSummaryMap = new Dictionary<int, TestSummaryForWorkItem>();
      if (context.IsFeatureEnabled("TestManagement.Server.AddTestCaseRefLinksToRequirement2"))
        witToSummaryMap = this.QueryAggregatedDataByRequirementForRelease2(context, projectId, workItemIds, releaseReference);
      if (!context.IsFeatureEnabled("TestManagement.Server.DisableOlderTestRequirementLink"))
      {
        Dictionary<int, TestSummaryForWorkItem> summaryByWorkItem = new Dictionary<int, TestSummaryForWorkItem>();
        int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          summaryByWorkItem = managementDatabase.QueryAggregatedDataByRequirementForRelease(projectId, workItemIds, releaseReference.ReleaseDefId, releaseReference.ReleaseEnvDefId, SourceWorkflow.ContinuousDelivery, progressOrFailed);
        witToSummaryMap = this.ReadWorkItemDetails(context, witToSummaryMap, summaryByWorkItem);
      }
      return witToSummaryMap.Values.ToList<TestSummaryForWorkItem>();
    }

    private Dictionary<int, TestSummaryForWorkItem> QueryAggregatedDataByRequirementForBuild2(
      TestManagementRequestContext context,
      GuidAndString projectId,
      List<int> workItemIds,
      BuildConfiguration buildConfiguration)
    {
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, List<int>> workItemToRefIdMap = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, List<int>>();
      if (workItemIds != null && workItemIds.Any<int>())
        workItemToRefIdMap = this.GetTestCaseRefsLinkedToWorkItems(context, workItemIds);
      Dictionary<int, AggregatedDataForResultTrend> testCaseRefToAggregateOutcomeMap = new Dictionary<int, AggregatedDataForResultTrend>();
      int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        testCaseRefToAggregateOutcomeMap = managementDatabase.QueryAggregatedDataByRequirementForBuild2(projectId, workItemToRefIdMap.Values.SelectMany<List<int>, int>((Func<List<int>, IEnumerable<int>>) (t => (IEnumerable<int>) t)).Distinct<int>().ToList<int>(), buildConfiguration, SourceWorkflow.ContinuousIntegration, 15, progressOrFailed);
      TestResultsContext testResultsContext = new TestResultsContext()
      {
        ContextType = TestResultsContextType.Build,
        Build = new BuildReference()
        {
          Id = buildConfiguration.BuildId,
          Number = buildConfiguration.BuildNumber,
          DefinitionId = buildConfiguration.BuildDefinitionId,
          BranchName = buildConfiguration.BranchName
        }
      };
      return this.ReadWorkItemDetails(context, projectId.GuidId, workItemToRefIdMap, testCaseRefToAggregateOutcomeMap, testResultsContext);
    }

    private Dictionary<int, TestSummaryForWorkItem> QueryAggregatedDataByRequirementForRelease2(
      TestManagementRequestContext context,
      GuidAndString projectId,
      List<int> workItemIds,
      ReleaseReference releaseReference)
    {
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, List<int>> workItemToRefIdMap = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, List<int>>();
      if (workItemIds != null && workItemIds.Any<int>())
        workItemToRefIdMap = this.GetTestCaseRefsLinkedToWorkItems(context, workItemIds);
      Dictionary<int, AggregatedDataForResultTrend> testCaseRefToAggregateOutcomeMap = new Dictionary<int, AggregatedDataForResultTrend>();
      int progressOrFailed = TCMServiceDataMigrationRestHelper.GetRunIdThresholdIfDataMigrationInProgressOrFailed(context.RequestContext);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        testCaseRefToAggregateOutcomeMap = managementDatabase.QueryAggregatedDataByRequirementForRelease2(projectId, workItemToRefIdMap.Values.SelectMany<List<int>, int>((Func<List<int>, IEnumerable<int>>) (t => (IEnumerable<int>) t)).Distinct<int>().ToList<int>(), releaseReference.ReleaseDefId, releaseReference.ReleaseEnvDefId, SourceWorkflow.ContinuousDelivery, 15, progressOrFailed);
      TestResultsContext testResultsContext = new TestResultsContext()
      {
        ContextType = TestResultsContextType.Release,
        Release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
        {
          Id = releaseReference.ReleaseId,
          EnvironmentId = releaseReference.ReleaseEnvId,
          Name = releaseReference.ReleaseName
        }
      };
      return this.ReadWorkItemDetails(context, projectId.GuidId, workItemToRefIdMap, testCaseRefToAggregateOutcomeMap, testResultsContext);
    }

    private Dictionary<int, TestSummaryForWorkItem> ReadWorkItemDetails(
      TestManagementRequestContext context,
      Dictionary<int, TestSummaryForWorkItem> witToSummaryMap,
      Dictionary<int, TestSummaryForWorkItem> summaryByWorkItem)
    {
      witToSummaryMap.Keys.ToList<int>().ForEach((Action<int>) (witId =>
      {
        if (!summaryByWorkItem.ContainsKey(witId))
          return;
        summaryByWorkItem.Remove(witId);
      }));
      if (summaryByWorkItem.Any<KeyValuePair<int, TestSummaryForWorkItem>>())
      {
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference workItemReference in this.WorkItemHelper.GetWorkItemReference(context.RequestContext, summaryByWorkItem.Keys.ToList<int>(), true))
        {
          int result = 0;
          if (int.TryParse(workItemReference.Id, out result))
          {
            summaryByWorkItem[result].WorkItem = workItemReference;
          }
          else
          {
            summaryByWorkItem.Remove(result);
            context.RequestContext.Trace(1015070, TraceLevel.Warning, "TestResultsInsights", "BusinessLayer", "Invalid workitem Id");
          }
        }
      }
      return witToSummaryMap.Concat<KeyValuePair<int, TestSummaryForWorkItem>>((IEnumerable<KeyValuePair<int, TestSummaryForWorkItem>>) summaryByWorkItem).ToDictionary<KeyValuePair<int, TestSummaryForWorkItem>, int, TestSummaryForWorkItem>((Func<KeyValuePair<int, TestSummaryForWorkItem>, int>) (w => w.Key), (Func<KeyValuePair<int, TestSummaryForWorkItem>, TestSummaryForWorkItem>) (w => w.Value));
    }

    private Dictionary<int, TestSummaryForWorkItem> ReadWorkItemDetails(
      TestManagementRequestContext context,
      Guid projectId,
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, List<int>> workItemToRefIdMap,
      Dictionary<int, AggregatedDataForResultTrend> testCaseRefToAggregateOutcomeMap,
      TestResultsContext testResultsContext)
    {
      Dictionary<int, TestSummaryForWorkItem> workItemToSummaryMap = new Dictionary<int, TestSummaryForWorkItem>();
      if (workItemToRefIdMap != null && workItemToRefIdMap.Any<KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, List<int>>>())
      {
        foreach (KeyValuePair<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, List<int>> workItemToRefId in workItemToRefIdMap)
        {
          AggregatedDataForResultTrend aggregateData = new AggregatedDataForResultTrend()
          {
            ResultsByOutcome = (IDictionary<TestOutcome, AggregatedResultsByOutcome>) new Dictionary<TestOutcome, AggregatedResultsByOutcome>()
          };
          TestSummaryForWorkItem summaryForWorkItem = new TestSummaryForWorkItem()
          {
            WorkItem = workItemToRefId.Key,
            Summary = aggregateData
          };
          workItemToRefId.Value.ForEach((Action<int>) (refId =>
          {
            aggregateData.TestResultsContext = testResultsContext;
            if (!testCaseRefToAggregateOutcomeMap.ContainsKey(refId) || testCaseRefToAggregateOutcomeMap[refId].ResultsByOutcome == null)
              return;
            aggregateData.TestResultsContext = testCaseRefToAggregateOutcomeMap[refId].TestResultsContext;
            testCaseRefToAggregateOutcomeMap[refId].ResultsByOutcome.ToList<KeyValuePair<TestOutcome, AggregatedResultsByOutcome>>().ForEach((Action<KeyValuePair<TestOutcome, AggregatedResultsByOutcome>>) (aggr =>
            {
              IDictionary<TestOutcome, AggregatedResultsByOutcome> resultsByOutcome3 = aggregateData.ResultsByOutcome;
              int key = (int) aggr.Key;
              AggregatedResultsByOutcome resultsByOutcome4;
              if (aggregateData.ResultsByOutcome.ContainsKey(aggr.Key))
              {
                resultsByOutcome4 = aggregateData.ResultsByOutcome[aggr.Key];
              }
              else
              {
                resultsByOutcome4 = new AggregatedResultsByOutcome();
                resultsByOutcome4.Outcome = aggr.Key;
              }
              resultsByOutcome3[(TestOutcome) key] = resultsByOutcome4;
              aggregateData.ResultsByOutcome[aggr.Key].Count += aggr.Value.Count;
            }));
          }));
          workItemToSummaryMap[Convert.ToInt32(workItemToRefId.Key.Id)] = summaryForWorkItem;
        }
      }
      else
      {
        Dictionary<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> testCaseReference = context.RequestContext.GetService<ITestManagementLinkedWorkItemService>().BatchCreateWorkItemsRecordsForTestCaseReference(context, projectId, "Microsoft.RequirementCategory", testCaseRefToAggregateOutcomeMap.Keys.ToList<int>(), 0);
        if (testCaseReference != null && testCaseReference.Any<KeyValuePair<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>>>())
        {
          foreach (KeyValuePair<string, List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>> keyValuePair in testCaseReference)
          {
            int tcRefId = TestManagementServiceUtility.GetArtifactId(keyValuePair.Key);
            if (testCaseRefToAggregateOutcomeMap.ContainsKey(tcRefId) && testCaseRefToAggregateOutcomeMap[tcRefId].ResultsByOutcome != null)
              keyValuePair.Value.ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>) (wit =>
              {
                int witId = Convert.ToInt32(wit.Id);
                Dictionary<int, TestSummaryForWorkItem> dictionary = workItemToSummaryMap;
                int key1 = witId;
                TestSummaryForWorkItem summaryForWorkItem;
                if (workItemToSummaryMap.ContainsKey(witId))
                {
                  summaryForWorkItem = workItemToSummaryMap[witId];
                }
                else
                {
                  summaryForWorkItem = new TestSummaryForWorkItem();
                  summaryForWorkItem.WorkItem = wit;
                  summaryForWorkItem.Summary = new AggregatedDataForResultTrend()
                  {
                    ResultsByOutcome = (IDictionary<TestOutcome, AggregatedResultsByOutcome>) new Dictionary<TestOutcome, AggregatedResultsByOutcome>()
                  };
                }
                dictionary[key1] = summaryForWorkItem;
                workItemToSummaryMap[witId].Summary.TestResultsContext = testCaseRefToAggregateOutcomeMap[tcRefId].TestResultsContext;
                testCaseRefToAggregateOutcomeMap[tcRefId].ResultsByOutcome.ToList<KeyValuePair<TestOutcome, AggregatedResultsByOutcome>>().ForEach((Action<KeyValuePair<TestOutcome, AggregatedResultsByOutcome>>) (aggr =>
                {
                  IDictionary<TestOutcome, AggregatedResultsByOutcome> resultsByOutcome7 = workItemToSummaryMap[witId].Summary.ResultsByOutcome;
                  int key2 = (int) aggr.Key;
                  AggregatedResultsByOutcome resultsByOutcome8;
                  if (workItemToSummaryMap[witId].Summary.ResultsByOutcome.ContainsKey(aggr.Key))
                  {
                    resultsByOutcome8 = workItemToSummaryMap[witId].Summary.ResultsByOutcome[aggr.Key];
                  }
                  else
                  {
                    resultsByOutcome8 = new AggregatedResultsByOutcome();
                    resultsByOutcome8.Outcome = aggr.Key;
                  }
                  resultsByOutcome7[(TestOutcome) key2] = resultsByOutcome8;
                  workItemToSummaryMap[witId].Summary.ResultsByOutcome[aggr.Key].Count += aggr.Value.Count;
                }));
              }));
          }
        }
      }
      return workItemToSummaryMap;
    }

    private Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, List<int>> GetTestCaseRefsLinkedToWorkItems(
      TestManagementRequestContext context,
      List<int> workItemIds)
    {
      Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, List<int>> workItemToRefIdMap = new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference, List<int>>();
      IEnumerable<WorkItem> workItems = this.WorkItemHelper.GetWorkItems(context.RequestContext, workItemIds, true);
      if (workItems != null && workItems.Any<WorkItem>())
      {
        foreach (WorkItem workItem in workItems)
        {
          if (workItem.Relations != null)
          {
            IEnumerable<WorkItemRelation> source = workItem.Relations.Where<WorkItemRelation>((Func<WorkItemRelation, bool>) (link => string.Equals(link.Rel, WorkItemLinkTypes.ArtifactLink, StringComparison.OrdinalIgnoreCase) && string.Equals(link.Attributes[WorkItemLinkAttributes.Name] as string, WorkItemLinkedArtifactName.Test, StringComparison.OrdinalIgnoreCase)));
            if (source.Any<WorkItemRelation>())
            {
              Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference workItemRef = new Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference()
              {
                Id = workItem.Id.ToString(),
                Name = workItem.Fields[WorkItemFieldRefNames.Title] as string,
                Type = workItem.Fields[WorkItemFieldRefNames.WorkItemType] as string
              };
              workItemToRefIdMap[workItemRef] = new List<int>();
              source.ToList<WorkItemRelation>().ForEach((Action<WorkItemRelation>) (rl => workItemToRefIdMap[workItemRef].Add(TestManagementServiceUtility.GetArtifactId(rl.Url))));
            }
          }
        }
      }
      return workItemToRefIdMap;
    }

    private List<TestCaseReference> GetUnplannedTestCaseRefs(
      TestManagementRequestContext requestContext,
      Guid projectId,
      List<TestMethod> testMethods)
    {
      List<TestCaseReference> source = new List<TestCaseReference>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
        source = managementDatabase.QueryTestCaseReference(projectId, testMethods.Select<TestMethod, string>((Func<TestMethod, string>) (testMethod => testMethod.Name)).ToList<string>(), new List<int>(), 0, new List<int>(), new List<int>());
      return source.Where<TestCaseReference>((Func<TestCaseReference, bool>) (r => r.TestCaseId == 0)).ToList<TestCaseReference>();
    }

    private List<int> GetFilteredTestCaseRefs(
      TestManagementRequestContext requextContext,
      Guid projectId,
      TestMethod testMethod)
    {
      return this.GetUnplannedTestCaseRefs(requextContext, projectId, new List<TestMethod>()
      {
        testMethod
      }).Where<TestCaseReference>((Func<TestCaseReference, bool>) (tcref => string.IsNullOrEmpty(testMethod.Container) || string.Equals(tcref.AutomatedTestStorage, testMethod.Container, StringComparison.OrdinalIgnoreCase))).Select<TestCaseReference, int>((Func<TestCaseReference, int>) (tcref => tcref.TestCaseReferenceId)).ToList<int>();
    }

    private List<int> GetAndCreateTestCaseRefsIfNotExists(
      TestManagementRequestContext context,
      GuidAndString projectId,
      int workItemId,
      List<TestCaseReference> testCaseRefs,
      List<TestMethod> testMethods)
    {
      List<int> referenceIds = new List<int>();
      List<TestCaseResult> testCaseResultOfMissingRefs = new List<TestCaseResult>();
      Dictionary<string, Dictionary<string, List<int>>> mappedTestCaseReferences = new Dictionary<string, Dictionary<string, List<int>>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      TcmCommonStructureNodeInfo areaNodeInfo = context.CSSHelper.GetRootNodes(projectId.String).FirstOrDefault<TcmCommonStructureNodeInfo>((Func<TcmCommonStructureNodeInfo, bool>) (node => node.StructureType == "ProjectModelHierarchy"));
      testCaseRefs.ForEach((Action<TestCaseReference>) (r =>
      {
        if (mappedTestCaseReferences.ContainsKey(r.AutomatedTestName))
        {
          if (mappedTestCaseReferences[r.AutomatedTestName].ContainsKey(r.AutomatedTestStorage))
            mappedTestCaseReferences[r.AutomatedTestName][r.AutomatedTestStorage].Add(r.TestCaseReferenceId);
          else
            mappedTestCaseReferences[r.AutomatedTestName][r.AutomatedTestStorage] = new List<int>()
            {
              r.TestCaseReferenceId
            };
        }
        else
          mappedTestCaseReferences[r.AutomatedTestName] = new Dictionary<string, List<int>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
          {
            {
              r.AutomatedTestStorage,
              new List<int>() { r.TestCaseReferenceId }
            }
          };
      }));
      testMethods.ForEach((Action<TestMethod>) (method =>
      {
        if (mappedTestCaseReferences.ContainsKey(method.Name) && mappedTestCaseReferences[method.Name].ContainsKey(method.Container))
          referenceIds.AddRange((IEnumerable<int>) mappedTestCaseReferences[method.Name][method.Container]);
        else
          testCaseResultOfMissingRefs.Add(new TestCaseResult()
          {
            AutomatedTestName = method.Name,
            AutomatedTestStorage = method.Container,
            TestCaseAreaUri = areaNodeInfo?.Uri ?? string.Empty
          });
      }));
      int[] collection = Array.Empty<int>();
      if (testCaseResultOfMissingRefs.Any<TestCaseResult>())
      {
        Guid teamFoundationId = context.UserTeamFoundationId;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          collection = managementDatabase.CreateTestCaseReference(projectId.GuidId, (IEnumerable<TestCaseResult>) testCaseResultOfMissingRefs, teamFoundationId, out int _, out List<int> _);
      }
      referenceIds.AddRange((IEnumerable<int>) collection);
      return this.FilterOutRefsAlreadyLinked(context, referenceIds, workItemId);
    }

    private List<int> FilterOutRefsAlreadyLinked(
      TestManagementRequestContext context,
      List<int> refIds,
      int workItemId)
    {
      IEnumerable<WorkItem> workItems = this.WorkItemHelper.GetWorkItems(context.RequestContext, new List<int>()
      {
        workItemId
      }, true);
      if (workItems == null || !workItems.Any<WorkItem>())
        return refIds;
      bool isRequestInTcm = context.RequestContext.ServiceInstanceType() == TestManagementServerConstants.TCMServiceInstanceType;
      Dictionary<int, int>.KeyCollection keys = this.GetTestCaseRefToResourceIdMap(workItems.First<WorkItem>(), refIds, isRequestInTcm).Keys;
      return refIds.Except<int>((IEnumerable<int>) keys).ToList<int>();
    }

    private Dictionary<int, int> GetTestCaseRefToResourceIdMap(
      WorkItem workItem,
      List<int> testCaseRefs,
      bool isRequestInTcm)
    {
      Dictionary<string, int> resourceLocationToResourceIdMap = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<int, int> tcRefToResourceIdMap = new Dictionary<int, int>();
      if (workItem != null && workItem.Relations != null && workItem.Relations.Any<WorkItemRelation>())
      {
        resourceLocationToResourceIdMap = workItem.Relations.Where<WorkItemRelation>((Func<WorkItemRelation, bool>) (link => string.Equals(link.Rel, WorkItemLinkTypes.ArtifactLink, StringComparison.OrdinalIgnoreCase) && link.Attributes != null && link.Attributes.ContainsKey(WorkItemLinkAttributes.Name) && object.Equals(link.Attributes[WorkItemLinkAttributes.Name], (object) WorkItemLinkedArtifactName.Test))).ToDictionary<WorkItemRelation, string, int>((Func<WorkItemRelation, string>) (r => r.Url), (Func<WorkItemRelation, int>) (r => Convert.ToInt32(r.Attributes[WorkItemLinkAttributes.Id])));
        testCaseRefs.ForEach((Action<int>) (r =>
        {
          string testCaseReference = TestManagementServiceUtility.GetArtiFactUriForTestCaseReference(r, isRequestInTcm);
          if (!resourceLocationToResourceIdMap.ContainsKey(testCaseReference))
            return;
          tcRefToResourceIdMap[r] = resourceLocationToResourceIdMap[testCaseReference];
        }));
      }
      return tcRefToResourceIdMap;
    }

    internal IWorkItemHelper WorkItemHelper
    {
      get
      {
        if (this._workItemHelper == null)
          this._workItemHelper = (IWorkItemHelper) new TestResultsWorkItemHelper();
        return this._workItemHelper;
      }
      set => this._workItemHelper = value;
    }
  }
}
