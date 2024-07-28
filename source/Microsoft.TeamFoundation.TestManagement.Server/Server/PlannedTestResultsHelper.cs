// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PlannedTestResultsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class PlannedTestResultsHelper : TfsRestApiHelper, IPlannedTestResultsHelper
  {
    private const string m_strTCWitFieldAutomatedTestId = "Microsoft.VSTS.TCM.AutomatedTestId";

    public PlannedTestResultsHelper(TestManagementRequestContext requesContext)
      : base(requesContext)
    {
    }

    public virtual void UpdatePlannedResultDetails(
      IVssRequestContext tfsRequestContext,
      string teamProjectName,
      int testPlanId,
      bool isAutomated,
      List<Tuple<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResult>> resultsMap)
    {
      IEnumerable<ShallowReference> source = resultsMap.Select<Tuple<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResult>, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Tuple<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResult>, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (r => r.Item1)).Where<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, bool>) (res => res.TestPoint != null && !string.IsNullOrEmpty(res.TestPoint.Id))).Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, ShallowReference>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, ShallowReference>) (res => res.TestPoint));
      Dictionary<int, TestPoint> testPointsFromRefs = this.GetTestPointsFromRefs(tfsRequestContext, teamProjectName, testPlanId, source.ToList<ShallowReference>());
      foreach (Tuple<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResult> results in resultsMap)
      {
        TestPoint point = (TestPoint) null;
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = results.Item1;
        TestCaseResult result = results.Item2;
        result.TestPointId = this.ValidateAndGetTestPointId(tfsRequestContext, teamProjectName, testCaseResult.TestPoint, isAutomated, testPointsFromRefs, out point);
        if (point != null)
          this.PopulateTestPointRelatedFields(tfsRequestContext, point, result);
        else
          result.TestCaseId = this.ValidateAndGetTestCaseId(tfsRequestContext, teamProjectName, testCaseResult.TestCase, result.TestPointId, isAutomated);
      }
    }

    public virtual Dictionary<int, string> GetTestPlanTitles(
      TestManagementRequestContext context,
      Guid projectId,
      List<int> testPlanIds)
    {
      Dictionary<int, string> testPlanTitles = new Dictionary<int, string>();
      string[] fields = new string[3]
      {
        "System.Id",
        "System.Title",
        "System.WorkItemType"
      };
      IList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems = context.WorkItemServiceHelper.GetWorkItems(projectId, (IList<int>) testPlanIds, (IList<string>) fields, WorkItemExpand.None, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Omit);
      if (workItems != null)
        workItems.ForEach<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>((Action<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) (workIem => testPlanTitles[workIem.Id.Value] = workIem.Fields["System.Title"] as string));
      return testPlanTitles;
    }

    public virtual void PopulateTestPointDetails(
      Dictionary<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultDataContracts,
      List<Tuple<TestCaseResultIdentifier, int, IdAndRev>> testResultTupleList,
      string projectName)
    {
      if (testResultTupleList == null)
        return;
      Dictionary<int, List<IdAndRev>> dictionary1 = new Dictionary<int, List<IdAndRev>>();
      Dictionary<int, TestPoint> dictionary2 = new Dictionary<int, TestPoint>();
      List<int> list = testResultTupleList.Select<Tuple<TestCaseResultIdentifier, int, IdAndRev>, int>((Func<Tuple<TestCaseResultIdentifier, int, IdAndRev>, int>) (o => o.Item2)).Where<int>((Func<int, bool>) (x => x > 0)).Distinct<int>().ToList<int>();
      Dictionary<int, ShallowReference> plansRepresentations = this.GetPlansRepresentations(projectName, list);
      Dictionary<int, List<IdAndRev>> dictionary3 = testResultTupleList.GroupBy<Tuple<TestCaseResultIdentifier, int, IdAndRev>, int>((Func<Tuple<TestCaseResultIdentifier, int, IdAndRev>, int>) (o => o.Item2)).ToDictionary<IGrouping<int, Tuple<TestCaseResultIdentifier, int, IdAndRev>>, int, List<IdAndRev>>((Func<IGrouping<int, Tuple<TestCaseResultIdentifier, int, IdAndRev>>, int>) (g => g.Key), (Func<IGrouping<int, Tuple<TestCaseResultIdentifier, int, IdAndRev>>, List<IdAndRev>>) (g => g.Select<Tuple<TestCaseResultIdentifier, int, IdAndRev>, IdAndRev>((Func<Tuple<TestCaseResultIdentifier, int, IdAndRev>, IdAndRev>) (x => x.Item3)).Where<IdAndRev>((Func<IdAndRev, bool>) (x => x.Id > 0)).Distinct<IdAndRev>().ToList<IdAndRev>()));
      foreach (KeyValuePair<int, ShallowReference> keyValuePair in plansRepresentations)
      {
        if (dictionary3.ContainsKey(keyValuePair.Key) && dictionary3[keyValuePair.Key].Count > 0)
        {
          Dictionary<int, TestPoint> testPoints = (Dictionary<int, TestPoint>) null;
          if (this.TryGetPointsFromPointIds(projectName, keyValuePair.Key, dictionary3[keyValuePair.Key].ToArray(), out testPoints))
            dictionary2.TryAddRange<int, TestPoint, Dictionary<int, TestPoint>>((IEnumerable<KeyValuePair<int, TestPoint>>) testPoints);
        }
      }
      foreach (Tuple<TestCaseResultIdentifier, int, IdAndRev> testResultTuple in testResultTupleList)
      {
        if (plansRepresentations.ContainsKey(testResultTuple.Item2))
          testCaseResultDataContracts[testResultTuple.Item1].TestPlan = plansRepresentations[testResultTuple.Item2];
        if (dictionary2.ContainsKey(testResultTuple.Item3.Id))
        {
          testCaseResultDataContracts[testResultTuple.Item1].TestPoint = new ShallowReference()
          {
            Id = dictionary2[testResultTuple.Item3.Id].PointId.ToString()
          };
          if (dictionary2[testResultTuple.Item3.Id].SuiteId > 0)
            testCaseResultDataContracts[testResultTuple.Item1].TestSuite = new ShallowReference()
            {
              Id = dictionary2[testResultTuple.Item3.Id].SuiteId.ToString(),
              Name = dictionary2[testResultTuple.Item3.Id].SuiteName
            };
        }
        this.SecureTestResultWebApiObject(testCaseResultDataContracts[testResultTuple.Item1]);
      }
    }

    public virtual void PopulateTestSuiteDetails(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults,
      string projectName)
    {
      if (testCaseResults == null || !testCaseResults.Any<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>())
        return;
      Dictionary<int, List<IdAndRev>> dictionary1 = new Dictionary<int, List<IdAndRev>>();
      Dictionary<int, TestPoint> dictionary2 = new Dictionary<int, TestPoint>();
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult in testCaseResults)
      {
        int result1;
        int result2;
        if (testCaseResult.TestPlan != null && !string.IsNullOrEmpty(testCaseResult.TestPlan.Id) && int.TryParse(testCaseResult.TestPlan.Id, out result1) && result1 > 0 && testCaseResult.TestPoint != null && !string.IsNullOrEmpty(testCaseResult.TestPoint.Id) && int.TryParse(testCaseResult.TestPoint.Id, out result2) && result2 > int.MinValue)
        {
          if (dictionary1.ContainsKey(result1))
            dictionary1[result1].Add(new IdAndRev()
            {
              Id = result2
            });
          else
            dictionary1[result1] = new List<IdAndRev>()
            {
              new IdAndRev() { Id = result2 }
            };
        }
      }
      foreach (KeyValuePair<int, ShallowReference> plansRepresentation in this.GetPlansRepresentations(projectName, dictionary1.Keys.ToList<int>()))
      {
        if (dictionary1.ContainsKey(plansRepresentation.Key) && dictionary1[plansRepresentation.Key].Count > 0)
        {
          Dictionary<int, TestPoint> testPoints = (Dictionary<int, TestPoint>) null;
          if (this.TryGetPointsFromPointIds(projectName, plansRepresentation.Key, dictionary1[plansRepresentation.Key].Distinct<IdAndRev>().ToArray<IdAndRev>(), out testPoints))
            dictionary2.TryAddRange<int, TestPoint, Dictionary<int, TestPoint>>((IEnumerable<KeyValuePair<int, TestPoint>>) testPoints);
        }
      }
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult in testCaseResults)
      {
        int result;
        if (testCaseResult.TestPoint != null && !string.IsNullOrEmpty(testCaseResult.TestPoint.Id) && int.TryParse(testCaseResult.TestPoint.Id, out result) && result > int.MinValue && dictionary2.ContainsKey(result) && dictionary2[result].SuiteId > 0)
          testCaseResult.TestSuite = new ShallowReference()
          {
            Id = dictionary2[result].SuiteId.ToString(),
            Name = dictionary2[result].SuiteName
          };
        this.SecureTestResultWebApiObject(testCaseResult);
      }
    }

    public static TestCaseResult[] CreateTestCaseResults(
      List<TestPoint> testPoints,
      DateTime dateTime,
      Guid runByTeamFoundationId,
      int startWithResultId)
    {
      return testPoints.Select<TestPoint, TestCaseResult>((Func<TestPoint, TestCaseResult>) (tp =>
      {
        return new TestCaseResult()
        {
          TestCaseId = tp.TestCaseId,
          ConfigurationId = tp.ConfigurationId,
          ConfigurationName = tp.ConfigurationName,
          TestRunId = 0,
          TestResultId = startWithResultId++,
          TestPointId = tp.PointId,
          DateStarted = dateTime,
          DateCompleted = dateTime,
          RunBy = runByTeamFoundationId,
          Owner = tp.AssignedTo
        };
      })).ToArray<TestCaseResult>();
    }

    public void PopulateAndCreatePlannedResults(
      TestManagementRequestContext context,
      Guid projectId,
      string projectName,
      RunCreateModel testRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRun newTestRun)
    {
      if (testRun.PointIds == null || !((IEnumerable<int>) testRun.PointIds).Any<int>())
        return;
      int result1;
      if (string.IsNullOrEmpty(testRun.Plan.Id) || !int.TryParse(testRun.Plan.Id, out result1) || result1 <= 0)
        throw new InvalidPropertyException("plan.Id", ServerResources.InvalidPropertyMessage);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults;
      new TfsTestRunDataContractConverter(context).PopulatePlannedResultDetailsFromCreateModel(projectName, testRun, result1, out testCaseResults);
      if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        Task<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>> testRunAsync = this.TestResultsHttpClient.AddTestResultsToTestRunAsync(testCaseResults.ToArray(), projectId, newTestRun.Id, (object) null, new CancellationToken());
        if (testRunAsync == null)
          return;
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> result2 = testRunAsync.Result;
      }
      else
        context.TcmServiceHelper.TryAddTestResultsToTestRun(this.RequestContext, projectId, newTestRun.Id, testCaseResults.ToArray(), out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> _);
    }

    private int ValidateAndGetTestCaseId(
      IVssRequestContext tfsContext,
      string teamProjectName,
      ShallowReference testCaseRef,
      int testPointId,
      bool isAutomatedRun)
    {
      using (PerfManager.Measure(tfsContext, "CrossService", TraceUtils.GetActionName("GetWorkItemTypeNamesInTestCaseCategory", "WorkItem")))
      {
        int result = 0;
        if (testCaseRef != null && !string.IsNullOrEmpty(testCaseRef.Id))
        {
          if (!int.TryParse(testCaseRef.Id, out result))
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidIdSpecified, (object) "testCase"));
          if (testPointId > int.MinValue)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestCaseIdSpecifiedWhenTestPointIdSpecifiedError, (object) "testCase"));
          this.ValidateTestCaseWorkItemStatus(tfsContext, teamProjectName, result, isAutomatedRun);
        }
        return result;
      }
    }

    private void PopulateTestPointRelatedFields(
      IVssRequestContext tfsRequestContext,
      TestPoint point,
      TestCaseResult result)
    {
      result.TestCaseId = point.TestCaseId;
      result.TestPointId = point.PointId;
      result.ConfigurationName = point.ConfigurationName;
      result.ConfigurationId = point.ConfigurationId;
      if (point.WorkItemProperties == null)
        return;
      ((IEnumerable<object>) point.WorkItemProperties).ToList<object>().ForEach((Action<object>) (witProp =>
      {
        if (!(witProp is KeyValue<string, object> keyValue2) || !keyValue2.Key.Equals("System.Rev", StringComparison.OrdinalIgnoreCase))
          return;
        result.TestCaseRevision = Convert.ToInt32(keyValue2.Value);
      }));
    }

    private void ValidateTestCaseWorkItemStatus(
      IVssRequestContext tfsRequestContext,
      string projectName,
      int testCaseId,
      bool isAutomatedRun)
    {
      using (PerfManager.Measure(tfsRequestContext, "CrossService", TraceUtils.GetActionName(nameof (ValidateTestCaseWorkItemStatus), "WorkItem")))
      {
        ArgumentUtility.CheckGreaterThanZero((float) testCaseId, "testCase", "Test Results");
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItemById = tfsRequestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemById(tfsRequestContext, testCaseId, false, false, false);
        if (workItemById == null)
          throw new TestObjectNotFoundException(tfsRequestContext, testCaseId, ObjectTypes.TestCase);
        if (!TestCaseHelper.GetWorkItemTypeNamesInTestCaseCategory((TestManagementRequestContext) new TfsTestManagementRequestContext(tfsRequestContext), projectName).Contains<string>(workItemById.WorkItemType))
          throw new TestObjectNotFoundException(tfsRequestContext, testCaseId, ObjectTypes.TestCase);
        bool flag = !string.IsNullOrEmpty((string) workItemById.GetFieldValue(tfsRequestContext, "Microsoft.VSTS.TCM.AutomatedTestId"));
        if (isAutomatedRun != flag)
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.AutomationStatusMismatch));
      }
    }

    private int ValidateAndGetTestPointId(
      IVssRequestContext tfsRequestContext,
      string projectName,
      ShallowReference testPointRef,
      bool isAutomatedRun,
      Dictionary<int, TestPoint> pointIdMap,
      out TestPoint point)
    {
      point = (TestPoint) null;
      int result = int.MinValue;
      if (testPointRef != null && !string.IsNullOrEmpty(testPointRef.Id))
      {
        if (!int.TryParse(testPointRef.Id, out result))
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidIdSpecified, (object) "testPoint"));
        TestPoint testPoint;
        if (!pointIdMap.TryGetValue(result, out testPoint))
          throw new TestObjectNotFoundException(tfsRequestContext, result, ObjectTypes.TestPoint);
        if (testPoint.WorkItemProperties != null)
          ((IEnumerable<object>) testPoint.WorkItemProperties).ToList<object>().ForEach((Action<object>) (witProp =>
          {
            if (witProp is KeyValue<string, object> keyValue2 && keyValue2.Key.Equals("Microsoft.VSTS.TCM.AutomatedTestId", StringComparison.OrdinalIgnoreCase) && (isAutomatedRun ? 1 : 0) == (keyValue2.Value == null ? 1 : (string.IsNullOrEmpty(Convert.ToString(keyValue2.Value)) ? 1 : 0)))
              throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.AutomationStatusMismatch));
          }));
        point = testPoint;
      }
      return result;
    }

    private Dictionary<int, TestPoint> GetTestPointsFromRefs(
      IVssRequestContext tfsRequestContext,
      string projectName,
      int planId,
      List<ShallowReference> testPointRefs)
    {
      Dictionary<int, TestPoint> testPointsFromRefs = new Dictionary<int, TestPoint>();
      if (testPointRefs != null && testPointRefs.Any<ShallowReference>())
      {
        List<int> pointIds = new List<int>();
        testPointRefs.ForEach((Action<ShallowReference>) (tpref =>
        {
          int result = 0;
          if (!int.TryParse(tpref.Id, out result) || result < 1)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidIdSpecified, (object) "testPoint"));
          pointIds.Add(result);
        }));
        string[] testCaseProps = new string[3]
        {
          "System.Id",
          "System.Rev",
          "Microsoft.VSTS.TCM.AutomatedTestId"
        };
        List<TestPoint> points = tfsRequestContext.GetService<ITeamFoundationTestManagementPointService>().GetPoints(tfsRequestContext, projectName, planId, pointIds, testCaseProps);
        if (points != null)
        {
          points.ForEach((Action<TestPoint>) (p =>
          {
            object[] witProps = p.WorkItemProperties;
            if (witProps == null || !((IEnumerable<object>) witProps).Any<object>())
              return;
            p.WorkItemProperties = (object[]) ((IEnumerable<string>) testCaseProps).Select<string, KeyValue<string, object>>((Func<string, int, KeyValue<string, object>>) ((tp, index) => new KeyValue<string, object>(tp, witProps[index]))).ToArray<KeyValue<string, object>>();
          }));
          return points.ToDictionary<TestPoint, int, TestPoint>((Func<TestPoint, int>) (p => p.PointId), (Func<TestPoint, TestPoint>) (p => p));
        }
      }
      return testPointsFromRefs;
    }

    private void SecureTestResultWebApiObject(Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testResult) => testResult.EnsureSecureObject();

    public virtual ShallowReference GetShallowTestPlan(
      TestManagementRequestContext context,
      string projectName,
      int testPlanId)
    {
      string[] strArray = new string[3]
      {
        "System.Id",
        "System.Title",
        "System.WorkItemType"
      };
      ProjectInfo projectFromName = context.ProjectServiceHelper.GetProjectFromName(projectName);
      IWorkItemServiceHelper itemServiceHelper = context.WorkItemServiceHelper;
      Guid id = projectFromName.Id;
      List<int> ids = new List<int>();
      ids.Add(testPlanId);
      string[] fields = strArray;
      IList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems = itemServiceHelper.GetWorkItems(id, (IList<int>) ids, (IList<string>) fields, WorkItemExpand.None, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Omit);
      ShallowReference shallowTestPlan = (ShallowReference) null;
      if (workItems != null && workItems.Count > 0)
      {
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem = workItems.First<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>();
        shallowTestPlan = new ShallowReference()
        {
          Id = testPlanId.ToString(),
          Name = workItem.Fields["System.Title"] as string,
          Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestPlanProject, (object) new
          {
            planId = testPlanId,
            project = projectName
          })
        };
      }
      return shallowTestPlan;
    }

    public Dictionary<int, ShallowReference> GetShallowTestPlans(
      TestManagementRequestContext context,
      string projectName,
      List<int> testPlanIds)
    {
      string[] fields = new string[3]
      {
        "System.Id",
        "System.Title",
        "System.WorkItemType"
      };
      ProjectInfo projectFromName = context.ProjectServiceHelper.GetProjectFromName(projectName);
      IList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems = context.WorkItemServiceHelper.GetWorkItems(projectFromName.Id, (IList<int>) testPlanIds, (IList<string>) fields, WorkItemExpand.None, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Omit);
      Dictionary<int, ShallowReference> planIdMappedToDetails = new Dictionary<int, ShallowReference>();
      if (workItems != null)
        workItems.ForEach<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>((Action<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) (workItem =>
        {
          int key = workItem.Id.Value;
          ShallowReference shallowReference = new ShallowReference()
          {
            Id = key.ToString(),
            Name = workItem.Fields["System.Title"] as string,
            Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestPlanProject, (object) new
            {
              planId = key,
              project = projectName
            })
          };
          planIdMappedToDetails[key] = shallowReference;
        }));
      return planIdMappedToDetails;
    }

    public virtual string GetTestPlanIteration(
      TestManagementRequestContext context,
      string projectName,
      int testPlanId)
    {
      string[] strArray = new string[2]
      {
        "System.Id",
        "System.IterationPath"
      };
      ProjectInfo projectFromName = context.ProjectServiceHelper.GetProjectFromName(projectName);
      IWorkItemServiceHelper itemServiceHelper = context.WorkItemServiceHelper;
      Guid id = projectFromName.Id;
      List<int> ids = new List<int>();
      ids.Add(testPlanId);
      string[] fields = strArray;
      IList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems = itemServiceHelper.GetWorkItems(id, (IList<int>) ids, (IList<string>) fields, WorkItemExpand.None, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Omit);
      return workItems != null && workItems.Count > 0 ? workItems.First<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>().Fields["System.IterationPath"] as string : (string) null;
    }

    internal TestResultsHttpClient TestResultsHttpClient => this.TestManagementRequestContext.RequestContext.GetClient<TestResultsHttpClient>();
  }
}
