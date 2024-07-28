// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PointsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server.TcmService;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class PointsHelper : TfsRestApiHelper
  {
    private TestPlansHelper m_plansHelper;
    private const string c_allPointsInSuiteQuery = "SELECT * FROM TestPoint WHERE SuiteId = {0} ORDER BY PointId";
    private const string c_allPointsInSuiteQuery_Filters = "SELECT * FROM TestPoint WHERE SuiteId = {0}{1} ORDER BY PointId";
    private const string c_allPointsInRecursiveSuiteQuery = "SELECT * FROM TestPoint WHERE RecursiveSuiteId = {0} ORDER BY PointId";
    private const string c_allPointsInRecusriveSuiteQuery_Filters = "SELECT * FROM TestPoint WHERE RecursiveSuiteId = {0}{1} ORDER BY PointId";
    private const int c_pointResultsBatchSize = 1000;
    private const int c_recursivePointResultsMaxSize = 10000;
    private Dictionary<int, WorkItemReference> testcaseCache = new Dictionary<int, WorkItemReference>();

    public PointsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<TestPoint> GetPointsHelper(
      TeamProjectReference teamProjectReference,
      TestPlan testPlan,
      ServerTestSuite serverTestSuite,
      string witFields,
      string configurationId,
      string testCaseId,
      string testPointIds,
      bool includePointDetails,
      int skip,
      int top,
      int watermark = -2147483648,
      bool newApiFilter = false,
      bool returnIdentityRef = false,
      bool isRecursive = false,
      bool excludeRunBy = false)
    {
      this.RequestContext.TraceInfo("RestLayer", "PointsHelper.GetPointsHelper projectId = {0} planId = {1}, suiteId = {2}, skip = {3}, top={4}, witFields = {5}", (object) teamProjectReference?.Id, (object) testPlan?.PlanId, (object) serverTestSuite?.Id, (object) skip, (object) top, (object) witFields);
      return this.ExecuteAction<List<TestPoint>>("PointsHelper.GetPointsHelper", (Func<List<TestPoint>>) (() =>
      {
        this.CheckForViewTestResultPermission(teamProjectReference.Name);
        List<TestPoint> testPointList1 = new List<TestPoint>();
        List<string> itemFieldsForPoints = this.GetWorkItemFieldsForPoints(witFields);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPointList2 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
        List<TestPoint> points;
        if (serverTestSuite != null)
        {
          if (serverTestSuite.PlanId != testPlan.PlanId && newApiFilter)
            throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSuiteDoesNotExistInPlan, (object) serverTestSuite.Id, (object) testPlan.PlanId), ObjectTypes.TestSuite);
          if (serverTestSuite.SuiteType != (byte) 2)
            serverTestSuite.Repopulate((TestManagementRequestContext) this.TfsTestManagementRequestContext, TestSuiteSource.Web);
          string pointsInSuiteQuery = this.GetAllPointsInSuiteQuery(serverTestSuite.Id, newApiFilter, configurationId, testCaseId, testPointIds, isRecursive);
          ResultsStoreQuery query = new ResultsStoreQuery()
          {
            TeamProjectName = teamProjectReference.Name,
            TimeZone = TimeZoneInfo.Utc.ToSerializedString(),
            QueryText = pointsInSuiteQuery
          };
          if (isRecursive && top > 10000)
            top = 10000;
          points = TestPoint.Query((TestManagementRequestContext) this.TfsTestManagementRequestContext, testPlan.PlanId, skip, top, watermark, query, out List<TestPointStatistic> _, false, itemFieldsForPoints.ToArray(), true, returnIdentityRef);
        }
        else
        {
          List<int> ids = this.GetIds(testPointIds, nameof (testPointIds));
          if (!ids.Any<int>())
            throw new InvalidPropertyException("");
          List<TestPoint> testPoints = this.FetchTestPoints(testPlan.PlanId, teamProjectReference.Name, ids, itemFieldsForPoints, returnIdentityRef);
          testPoints.RemoveAll((Predicate<TestPoint>) (testpoint => testpoint.PointId < watermark));
          points = !newApiFilter ? this.FilterTestPoints(testPoints, configurationId, testCaseId, testPointIds, skip, top) : this.FilterTestPoints(testPoints, testCaseId, testPointIds, skip, top);
        }
        return this.TfsTestManagementRequestContext.PlannedTestingTCMServiceHelper.UpdatePointsWithLatestResults((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectReference.Id, testPlan.PlanId, points, excludeRunBy, teamProjectReference.Name);
      }), 1015060, "TestManagement");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> GetPoints(
      string projectId,
      int planId,
      int suiteId,
      string witFields,
      string configurationId,
      string testCaseId,
      string testPointIds,
      bool includePointDetails,
      int skip,
      int top,
      int watermark = 0,
      bool newApiFilter = false,
      bool returnIdentityRef = false)
    {
      this.RequestContext.TraceInfo("RestLayer", "PointsHelper.GetPoints projectId = {0} planId = {1}, suiteId = {2}, skip = {3}, top={4}, witFields = {5}", (object) projectId, (object) planId, (object) suiteId, (object) skip, (object) top, (object) witFields);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>>("PointsHelper.GetPoints", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId), this.RequestContext.ServiceName);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        TestPlan testPlan;
        if (!this.TryGetPlanFromPlanId(projectReference.Name, planId, out testPlan))
          throw new TestObjectNotFoundException(this.RequestContext, planId, ObjectTypes.TestPlan);
        ServerTestSuite testSuite = (ServerTestSuite) null;
        if (suiteId > 0 && !this.TryGetSuiteFromSuiteId(projectReference.Name, suiteId, out testSuite))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        List<TestPoint> pointsHelper = this.GetPointsHelper(projectReference, testPlan, testSuite, witFields, configurationId, testCaseId, testPointIds, includePointDetails, skip, top, watermark, newApiFilter, returnIdentityRef);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> points = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
        List<Guid> source = new List<Guid>();
        foreach (TestPoint testPoint in pointsHelper)
        {
          if (testPoint.LastUpdatedBy != Guid.Empty)
            source.Add(testPoint.LastUpdatedBy);
        }
        Dictionary<Guid, IdentityRef> identityMapping = this.GetIdentityMapping(source.Distinct<Guid>().ToList<Guid>());
        List<string> itemFieldsForPoints = this.GetWorkItemFieldsForPoints(witFields);
        foreach (TestPoint testPoint in pointsHelper)
          points.Add(this.ConvertTestPointToDataContract(testPoint, projectReference, itemFieldsForPoints, includePointDetails, testPlan, testSuite, identityMapping));
        return points;
      }), 1015060, "TestManagement");
    }

    public List<TestPoint> GetPointsByQuery(
      string projectName,
      List<int> testCaseIds,
      int skip,
      int top)
    {
      this.RequestContext.TraceInfo("BusinessLayer", "PointsHelper.GetPointsByQuery projectName = {0}, skip = {1}, top = {2}", (object) projectName, (object) skip, (object) top);
      return this.ExecuteAction<List<TestPoint>>("PointsHelper.GetPointsByQuery", (Func<List<TestPoint>>) (() =>
      {
        List<TestPoint> testPointList = new List<TestPoint>();
        string[] testCaseProperties = new string[1]
        {
          WorkItemFieldNames.Title
        };
        return TestPoint.GetPointsByQuery((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, testCaseIds.Distinct<int>().ToArray<int>(), (string[]) null, (Guid[]) null, testCaseProperties, skip, top, true);
      }), 1015060, "TestManagement", "BusinessLayer");
    }

    internal virtual List<TestPoint> FilterTestPoints(
      List<TestPoint> testPoints,
      string configurationId,
      string testCaseId,
      string pointIds,
      int skip,
      int top)
    {
      int configId;
      if (!string.IsNullOrEmpty(configurationId) && int.TryParse(configurationId, out configId))
      {
        testPoints = testPoints.Where<TestPoint>((Func<TestPoint, bool>) (testPoint => testPoint.ConfigurationId == configId)).ToList<TestPoint>();
      }
      else
      {
        int testId;
        if (!string.IsNullOrEmpty(testCaseId) && int.TryParse(testCaseId, out testId))
          testPoints = testPoints.Where<TestPoint>((Func<TestPoint, bool>) (testPoint => testPoint.TestCaseId == testId)).ToList<TestPoint>();
        else if (!string.IsNullOrEmpty(pointIds))
        {
          HashSet<int> pointIdMap = this.GetIds(pointIds, "testPointIds").ToHashSet<int>();
          testPoints = testPoints.Where<TestPoint>((Func<TestPoint, bool>) (testPoint => pointIdMap.Contains(testPoint.PointId))).ToList<TestPoint>();
        }
      }
      return testPoints == null ? new List<TestPoint>() : testPoints.Skip<TestPoint>(skip).Take<TestPoint>(top).ToList<TestPoint>();
    }

    internal virtual List<TestPoint> FilterTestPoints(
      List<TestPoint> testPoints,
      string testCaseId,
      string pointIds,
      int skip,
      int top)
    {
      List<int> TestCaseIds = this.GetIds(testCaseId, "testCaseIds");
      if (!string.IsNullOrEmpty(pointIds))
      {
        HashSet<int> pointIdMap = this.GetIds(pointIds, "testPointIds").ToHashSet<int>();
        testPoints = testPoints.Where<TestPoint>((Func<TestPoint, bool>) (testPoint => pointIdMap.Contains(testPoint.PointId))).ToList<TestPoint>();
      }
      if (!string.IsNullOrEmpty(testCaseId))
        testPoints = testPoints.Where<TestPoint>((Func<TestPoint, bool>) (testPoint => TestCaseIds.Contains(Convert.ToInt32(testPoint.TestCaseId)))).ToList<TestPoint>();
      return testPoints == null ? new List<TestPoint>() : testPoints.Skip<TestPoint>(skip).Take<TestPoint>(top).ToList<TestPoint>();
    }

    public List<TestPoint> GetPointByIdHelper(
      TeamProjectReference teamProjectReference,
      TestPlan testPlan,
      ServerTestSuite serverTestSuite,
      string pointId,
      string witFields,
      bool newAPI = false,
      bool returnIdentityRef = false)
    {
      this.RequestContext.TraceInfo("RestLayer", "PointsHelper.GetPointByIdHelper projectId = {0} planId = {1}, suiteId = {2}, pointIds = {3}, witFields = {4}", (object) teamProjectReference?.Id, (object) testPlan?.PlanId, (object) serverTestSuite?.Id, (object) pointId, (object) witFields);
      return this.ExecuteAction<List<TestPoint>>("PointsHelper.GetPointByIdHelper", (Func<List<TestPoint>>) (() =>
      {
        this.CheckForViewTestResultPermission(teamProjectReference.Name);
        List<int> ids = this.GetIds(pointId, "testPointIds");
        List<string> itemFieldsForPoints = this.GetWorkItemFieldsForPoints(witFields);
        List<TestPoint> testPointList = new List<TestPoint>();
        List<int> deletedIds = new List<int>();
        if (serverTestSuite.PlanId != testPlan.PlanId && newAPI)
          throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSuiteDoesNotExistInPlan, (object) serverTestSuite.Id, (object) testPlan.PlanId), ObjectTypes.TestSuite);
        IdAndRev[] idsToFetch = new IdAndRev[ids.Count];
        for (int index = 0; index < ids.Count; ++index)
        {
          IdAndRev idAndRev = new IdAndRev()
          {
            Id = ids.ElementAt<int>(index)
          };
          idsToFetch[index] = idAndRev;
        }
        List<TestPoint> points = TestPoint.Fetch((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectReference.Name, testPlan.PlanId, idsToFetch, itemFieldsForPoints.ToArray(), deletedIds, true, returnIdentityRef);
        if (deletedIds.Count > 0)
          throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, deletedIds[0], ObjectTypes.TestPoint);
        return this.TfsTestManagementRequestContext.PlannedTestingTCMServiceHelper.UpdatePointsWithLatestResults((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectReference.Id, testPlan.PlanId, points, projectName: teamProjectReference.Name);
      }), 1015060, "TestManagement");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> GetPointById(
      string projectId,
      int planId,
      int suiteId,
      string pointId,
      string witFields,
      bool newAPI = false,
      bool returnIdentityRef = false)
    {
      this.RequestContext.TraceInfo("RestLayer", "PointsHelper.GetPointById projectId = {0} planId = {1}, suiteId = {2}, pointId = {3}, witFields = {4}, newAPI = {5}, returnIdentityRef = {6}", (object) projectId, (object) planId, (object) suiteId, (object) pointId, (object) witFields, (object) newAPI, (object) returnIdentityRef);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>>("PointsHelper.GetPointById", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId), this.RequestContext.ServiceName);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        TestPlan testPlan;
        if (!this.TryGetPlanFromPlanId(projectReference.Name, planId, out testPlan))
          throw new TestObjectNotFoundException(this.RequestContext, planId, ObjectTypes.TestPlan);
        ServerTestSuite testSuite;
        if (!this.TryGetSuiteFromSuiteId(projectReference.Name, suiteId, out testSuite))
          throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
        List<TestPoint> pointByIdHelper = this.GetPointByIdHelper(projectReference, testPlan, testSuite, pointId, witFields, newAPI, returnIdentityRef);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> pointById = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
        List<string> itemFieldsForPoints = this.GetWorkItemFieldsForPoints(witFields);
        foreach (TestPoint testPoint in pointByIdHelper)
          pointById.Add(this.ConvertTestPointToDataContract(testPoint, projectReference, itemFieldsForPoints, true, testPlan, testSuite));
        return pointById;
      }), 1015060, "TestManagement");
    }

    public List<TestPoint> BulkPatchTestPoints(
      TeamProjectReference teamProjectReference,
      TestPlan testPlan,
      ServerTestSuite serverTestSuite,
      List<TestPointUpdateParams> updateTestPointParams,
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome> bulkUpdateOutcome = null,
      bool returnIdentityRef = false)
    {
      this.RequestContext.TraceInfo("RestLayer", string.Format("PointsHelper.BulkPatchTestPoints projectId = {0}, planId = {1}, suiteId={2}, updateTestPointParams={3}", (object) teamProjectReference?.Id, (object) testPlan?.PlanId, (object) serverTestSuite?.Id, (object) updateTestPointParams));
      return this.ExecuteAction<List<TestPoint>>("PointsHelper.BulkPatchTestPoints", (Func<List<TestPoint>>) (() =>
      {
        ArgumentUtility.CheckForNull<string>(nameof (updateTestPointParams), nameof (updateTestPointParams), this.RequestContext.ServiceName);
        this.CheckForViewTestResultPermission(teamProjectReference.Name);
        List<int> intList1 = new List<int>();
        List<string> itemFieldsForPoints = this.GetWorkItemFieldsForPoints(string.Empty);
        List<TestPoint> testPointList1 = new List<TestPoint>();
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPointList2 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
        List<int> deletedIds = new List<int>();
        Dictionary<int, TestPoint> dictionary = new Dictionary<int, TestPoint>();
        List<UpdatePointStateAndTester> updatePointStateAndTesters = new List<UpdatePointStateAndTester>();
        foreach (TestPointUpdateParams updateTestPointParam in updateTestPointParams)
          intList1.Add(updateTestPointParam.Id);
        List<TestPoint> testPointList3 = TestPoint.Fetch((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectReference.Name, testPlan.PlanId, intList1.Select<int, IdAndRev>((Func<int, IdAndRev>) (pointId => new IdAndRev()
        {
          Id = pointId
        })).ToArray<IdAndRev>(), itemFieldsForPoints.ToArray(), deletedIds, returnIdentityRef: returnIdentityRef);
        if (deletedIds.Count > 0)
          throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, deletedIds[0], ObjectTypes.TestPoint);
        foreach (TestPoint testPoint in testPointList3)
        {
          if (testPoint != null)
            dictionary.Add(testPoint.PointId, testPoint);
        }
        List<TestPoint> testPoints = new List<TestPoint>();
        foreach (TestPointUpdateParams updateTestPointParam in updateTestPointParams)
        {
          TestPoint valueOrDefault = dictionary.GetValueOrDefault<int, TestPoint>(updateTestPointParam.Id, (TestPoint) null);
          if (valueOrDefault == null)
            throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, updateTestPointParam.Id, ObjectTypes.TestPoint);
          bool? isActive = updateTestPointParam.IsActive;
          bool flag1 = true;
          if (isActive.GetValueOrDefault() == flag1 & isActive.HasValue || updateTestPointParam.Results != null)
            testPoints.Add(valueOrDefault);
          UpdatePointStateAndTester pointStateAndTester = new UpdatePointStateAndTester();
          pointStateAndTester.PointId = updateTestPointParam.Id;
          isActive = updateTestPointParam.IsActive;
          if (isActive.HasValue)
          {
            isActive = updateTestPointParam.IsActive;
            bool flag2 = false;
            if (!(isActive.GetValueOrDefault() == flag2 & isActive.HasValue))
            {
              pointStateAndTester.ResetToActive = true;
              goto label_23;
            }
          }
          pointStateAndTester.ResetToActive = false;
label_23:
          if (updateTestPointParam.Tester == null)
            pointStateAndTester.AssignedTo = valueOrDefault.AssignedTo;
          else if (!string.IsNullOrEmpty(updateTestPointParam.Tester.Id))
          {
            if (updateTestPointParam.Tester.Id == Guid.Empty.ToString())
              pointStateAndTester.AssignedTo = Guid.Empty;
            Guid result;
            if (!Guid.TryParse(updateTestPointParam.Tester.Id, out result))
              throw new ArgumentException("tester.id").Expected(this.RequestContext.ServiceName);
            pointStateAndTester.AssignedTo = result;
          }
          else
          {
            if (string.IsNullOrEmpty(updateTestPointParam.Tester.DisplayName))
              throw new ArgumentException("Tester").Expected(this.RequestContext.ServiceName);
            Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentityByDisplayName(updateTestPointParam.Tester.DisplayName);
            if (identity == null)
            {
              pointStateAndTester.AssignedTo = valueOrDefault.AssignedTo;
              this.RequestContext.TraceAlways(1015060, TraceLevel.Info, "TestManagement", "RestLayer", "PointsHelper.BulkPatchTestPoints projectId = {0}, planId = {1}, suiteId = {2}, pointId = {3}, attempted assign to = {4}", (object) teamProjectReference.Id, (object) testPlan.PlanId, (object) serverTestSuite.Id, (object) valueOrDefault.PointId, (object) updateTestPointParam.Tester.DisplayName);
            }
            else
              pointStateAndTester.AssignedTo = identity.Id;
          }
          isActive = updateTestPointParam.IsActive;
          bool flag3 = true;
          pointStateAndTester.State = !(isActive.GetValueOrDefault() == flag3 & isActive.HasValue) ? valueOrDefault.State : (byte) 1;
          pointStateAndTester.TestCaseId = valueOrDefault.TestCaseId;
          pointStateAndTester.ConfigurationId = valueOrDefault.ConfigurationId;
          pointStateAndTester.Revision = valueOrDefault.Revision;
          pointStateAndTester.LastUpdatedBy = this.m_testManagementRequestContext.UserTeamFoundationId;
          pointStateAndTester.LastTestResultId = valueOrDefault.LastTestResultId;
          pointStateAndTester.LastTestRunId = valueOrDefault.LastTestRunId;
          updatePointStateAndTesters.Add(pointStateAndTester);
        }
        PlannedResultsTCMServiceHelper tcmServiceHelper = this.TfsTestManagementRequestContext.PlannedTestingTCMServiceHelper;
        bool updateResultsInTCM = tcmServiceHelper.IsTCMEnabledForPlannedTestResults((TestManagementRequestContext) this.TfsTestManagementRequestContext, testPlan.PlanId);
        if (updateResultsInTCM)
          tcmServiceHelper.UnblockTestPointResultsIfAny((TestManagementRequestContext) this.TfsTestManagementRequestContext, testPoints, teamProjectReference.Id, testPlan.PlanId);
        TestPoint.BulkUpdateTestPointStateAndTester((TestManagementRequestContext) this.TfsTestManagementRequestContext, testPlan.PlanId, serverTestSuite.Id, testPointList3.ToArray(), teamProjectReference.Name, updateResultsInTCM, updatePointStateAndTesters);
        Dictionary<int, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome> bulkUpdateOutcome1 = new Dictionary<int, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>();
        List<int> intList2 = new List<int>();
        for (int index = 0; index < updateTestPointParams.Count; ++index)
        {
          int id = updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Id;
          if (updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Results != null)
          {
            Microsoft.TeamFoundation.TestManagement.Client.TestOutcome outcome = (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Results.Outcome;
            if (!bulkUpdateOutcome1.ContainsKey(id))
            {
              bulkUpdateOutcome1.Add(id, outcome);
              intList2.Add(id);
            }
          }
        }
        if (bulkUpdateOutcome1.Count > 0)
        {
          Microsoft.TeamFoundation.TestManagement.Client.TestOutcome outcome = Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Unspecified;
          this.UpdateOutcomeForTestPoints(TestPoint.Fetch((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectReference.Name, testPlan.PlanId, intList2.Select<int, IdAndRev>((Func<int, IdAndRev>) (pointId => new IdAndRev()
          {
            Id = pointId
          })).ToArray<IdAndRev>(), itemFieldsForPoints.ToArray(), deletedIds, returnIdentityRef: returnIdentityRef), testPlan.PlanId, serverTestSuite.Id, teamProjectReference.Name, outcome, bulkUpdateOutcome1, intList2);
          TestOutcomeUpdateRequest updateRequest = new TestOutcomeUpdateRequest(teamProjectReference.Name, testPlan.PlanId, serverTestSuite.Id, intList1.ToArray(), bulkUpdateOutcome1.Values.FirstOrDefault<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(), this.TfsTestManagementRequestContext.UserTeamFoundationId);
          Guid projectGuid = this.TfsTestManagementRequestContext.ProjectServiceHelper.GetProjectGuid(teamProjectReference.Name);
          ITeamFoundationTestManagementOutcomeService service = this.RequestContext.GetService<ITeamFoundationTestManagementOutcomeService>();
          if (service.ShouldSyncOutcomeAcrossSuites(this.RequestContext, updateRequest.PlanId, projectGuid))
            service.SyncTestOutcome(this.RequestContext, updateRequest);
        }
        List<TestPoint> points = this.FetchTestPoints(testPlan.PlanId, teamProjectReference.Name, intList1, itemFieldsForPoints, returnIdentityRef);
        return tcmServiceHelper.UpdatePointsWithLatestResults((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectReference.Id, testPlan.PlanId, points, projectName: teamProjectReference.Name);
      }), 1015060, "TestManagement");
    }

    public List<TestPoint> PatchTestPointsHelper(
      TeamProjectReference teamProjectReference,
      TestPlan testPlan,
      ServerTestSuite serverTestSuite,
      string pointIds,
      PointUpdateModel pointUpdateModel,
      bool newAPI = false,
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome> bulkUpdateOutcome = null,
      List<int> testPointsIds = null,
      bool returnIdentityRef = false)
    {
      this.RequestContext.TraceInfo("RestLayer", "PointsHelper.PatchTestPoints projectId = {0} planId = {1}, suiteId = {2}, pointIds = {3}, updateModel = {4}", (object) teamProjectReference?.Id, (object) testPlan?.PlanId, (object) serverTestSuite?.Id, (object) pointIds, (object) pointUpdateModel);
      return this.ExecuteAction<List<TestPoint>>("PointsHelper.PatchTestPoints", (Func<List<TestPoint>>) (() =>
      {
        ArgumentUtility.CheckForNull<PointUpdateModel>(pointUpdateModel, nameof (pointUpdateModel), this.RequestContext.ServiceName);
        List<int> intList = new List<int>();
        List<TestPoint> testPointList1 = new List<TestPoint>();
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPointList2 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
        List<string> itemFieldsForPoints = this.GetWorkItemFieldsForPoints(string.Empty);
        this.CheckForViewTestResultPermission(teamProjectReference.Name);
        if (serverTestSuite.PlanId != testPlan.PlanId && newAPI)
          throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSuiteDoesNotExistInPlan, (object) serverTestSuite.Id, (object) testPlan.PlanId), ObjectTypes.TestSuite);
        if (!string.IsNullOrEmpty(pointIds))
        {
          List<int> ids = this.GetIds(pointIds, "testPointIds");
          if (ids.Count > 0)
          {
            List<int> deletedIds = new List<int>();
            List<TestPoint> testPoints = TestPoint.Fetch((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectReference.Name, testPlan.PlanId, ids.Select<int, IdAndRev>((Func<int, IdAndRev>) (pointId => new IdAndRev()
            {
              Id = pointId
            })).ToArray<IdAndRev>(), (string[]) null, deletedIds, returnIdentityRef: returnIdentityRef);
            if (deletedIds.Count > 0)
              throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, deletedIds[0], ObjectTypes.TestPoint);
            bool? resetToActive = pointUpdateModel.ResetToActive;
            if (!resetToActive.HasValue && string.IsNullOrEmpty(pointUpdateModel.Outcome) && pointUpdateModel.Tester == null)
              throw new ArgumentException(nameof (pointUpdateModel)).Expected(this.RequestContext.ServiceName);
            resetToActive = pointUpdateModel.ResetToActive;
            bool flag = true;
            if (resetToActive.GetValueOrDefault() == flag & resetToActive.HasValue)
              this.ResetTestPointsOutcome(testPoints, teamProjectReference, testPlan.PlanId);
            else if (!string.IsNullOrEmpty(pointUpdateModel.Outcome))
            {
              Microsoft.TeamFoundation.TestManagement.Client.TestOutcome result;
              if (!Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(pointUpdateModel.Outcome, true, out result) || result == Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Unspecified)
                throw new ArgumentException("Outcome").Expected(this.RequestContext.ServiceName);
              this.UpdateOutcomeForTestPoints(testPoints, testPlan.PlanId, serverTestSuite.Id, teamProjectReference.Name, result);
            }
            if (pointUpdateModel.Tester != null)
            {
              if (!string.IsNullOrEmpty(pointUpdateModel.Tester.DisplayName))
              {
                this.UpdateTesterForPoints(testPoints, teamProjectReference.Name, pointUpdateModel.Tester.DisplayName);
              }
              else
              {
                if (string.IsNullOrEmpty(pointUpdateModel.Tester.Id))
                  throw new ArgumentException("Tester").Expected(this.RequestContext.ServiceName);
                Guid result;
                if (pointUpdateModel.Tester.Id == Guid.Empty.ToString())
                  result = Guid.Empty;
                else if (!Guid.TryParse(pointUpdateModel.Tester.Id, out result))
                  throw new ArgumentException("tester.id").Expected(this.RequestContext.ServiceName);
                this.UpdateTesterForPoints(testPoints, teamProjectReference.Name, result);
              }
            }
          }
          return this.TfsTestManagementRequestContext.PlannedTestingTCMServiceHelper.UpdatePointsWithLatestResults((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectReference.Id, testPlan.PlanId, this.FetchTestPoints(testPlan.PlanId, teamProjectReference.Name, ids, itemFieldsForPoints, returnIdentityRef), projectName: teamProjectReference.Name);
        }
        if (testPointsIds == null)
          throw new ArgumentNullException(nameof (pointIds)).Expected(this.RequestContext.ServiceName);
        List<int> deletedIds1 = new List<int>();
        List<TestPoint> testPoints1 = TestPoint.Fetch((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectReference.Name, testPlan.PlanId, testPointsIds.Select<int, IdAndRev>((Func<int, IdAndRev>) (pointId => new IdAndRev()
        {
          Id = pointId
        })).ToArray<IdAndRev>(), (string[]) null, deletedIds1, returnIdentityRef: returnIdentityRef);
        if (deletedIds1.Count > 0)
          throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, deletedIds1[0], ObjectTypes.TestPoint);
        if (bulkUpdateOutcome != null)
        {
          Microsoft.TeamFoundation.TestManagement.Client.TestOutcome outcome = Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Unspecified;
          this.UpdateOutcomeForTestPoints(testPoints1, testPlan.PlanId, serverTestSuite.Id, teamProjectReference.Name, outcome, bulkUpdateOutcome, testPointsIds);
        }
        return this.TfsTestManagementRequestContext.PlannedTestingTCMServiceHelper.UpdatePointsWithLatestResults((TestManagementRequestContext) this.TfsTestManagementRequestContext, teamProjectReference.Id, testPlan.PlanId, this.FetchTestPoints(testPlan.PlanId, teamProjectReference.Name, testPointsIds, itemFieldsForPoints, returnIdentityRef), projectName: teamProjectReference.Name);
      }), 1015060, "TestManagement");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> PatchTestPoints(
      string projectId,
      int planId,
      int suiteId,
      string pointIds,
      PointUpdateModel pointUpdateModel,
      bool newAPI = false,
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome> bulkUpdateOutcome = null,
      List<int> testPointsIds = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectId, nameof (projectId), this.RequestContext.ServiceName);
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      TestPlan testPlan;
      if (!this.TryGetPlanFromPlanId(projectReference.Name, planId, out testPlan))
        throw new TestObjectNotFoundException(this.RequestContext, planId, ObjectTypes.TestPlan);
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(projectReference.Name, suiteId, out testSuite))
        throw new TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
      List<TestPoint> testPointList1 = this.PatchTestPointsHelper(projectReference, testPlan, testSuite, pointIds, pointUpdateModel, newAPI, bulkUpdateOutcome, testPointsIds);
      List<string> itemFieldsForPoints = this.GetWorkItemFieldsForPoints(string.Empty);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPointList2 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
      foreach (TestPoint testPoint in testPointList1)
        testPointList2.Add(this.ConvertTestPointToDataContract(testPoint, projectReference, itemFieldsForPoints, true));
      return testPointList2;
    }

    internal virtual List<int> GetIds(string Ids, string argument)
    {
      if (Ids == null)
        return new List<int>();
      List<int> ids = new List<int>();
      string[] source = Ids.Split(new string[1]{ "," }, StringSplitOptions.RemoveEmptyEntries);
      try
      {
        if (source != null)
        {
          if (source.Length != 0)
            ids = ((IEnumerable<string>) source).ToList<string>().ConvertAll<int>((Converter<string, int>) (pointId => int.Parse(pointId)));
        }
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case ArgumentNullException _:
          case OverflowException _:
          case FormatException _:
            throw new ArgumentException(argument, ex).Expected(this.RequestContext.ServiceName);
          default:
            throw;
        }
      }
      return ids;
    }

    internal virtual void ResetTestPointsOutcome(
      List<TestPoint> testPoints,
      TeamProjectReference project,
      int planId)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(project, nameof (project), this.RequestContext.ServiceName);
      PlannedResultsTCMServiceHelper tcmServiceHelper = this.TfsTestManagementRequestContext.PlannedTestingTCMServiceHelper;
      bool updateResultsInTCM = tcmServiceHelper.IsTCMEnabledForPlannedTestResults((TestManagementRequestContext) this.TfsTestManagementRequestContext, planId);
      if (updateResultsInTCM)
        tcmServiceHelper.UnblockTestPointResultsIfAny((TestManagementRequestContext) this.TfsTestManagementRequestContext, testPoints, project.Id, planId);
      UpdatedProperties[] updatedPropertiesArray = TestPoint.ResetTestPoints((TestManagementRequestContext) this.TfsTestManagementRequestContext, testPoints.ToArray(), project.Name, updateResultsInTCM);
      if (updatedPropertiesArray == null || updatedPropertiesArray.Length == 0)
        throw new TestObjectNotFoundException(ServerResources.TestPointsNotFound, ObjectTypes.TestPoint);
    }

    internal virtual void UpdateOutcomeForTestPoints(
      List<TestPoint> testPoints,
      int planId,
      int suiteId,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.Client.TestOutcome outcome,
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome> bulkUpdateOutcome = null,
      List<int> testPointIds = null)
    {
      DateTime utcNow = DateTime.UtcNow;
      TestCaseResult[] testCaseResults = PlannedTestResultsHelper.CreateTestCaseResults(testPoints, utcNow, this.TfsTestManagementRequestContext.UserTeamFoundationId, 100000);
      string runTitle1 = this.GetRunTitle(suiteId, planId, projectName);
      List<int> deletedIds = new List<int>();
      List<TestPlan> source = this.FetchTestPlans((IEnumerable<int>) new List<int>()
      {
        planId
      }, projectName, deletedIds, false);
      if (deletedIds.Count > 0)
        throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, deletedIds[0], ObjectTypes.TestPlan);
      bool flag = this.TfsTestManagementRequestContext.PlannedTestingTCMServiceHelper.IsTCMEnabledForPlannedTestResults((TestManagementRequestContext) this.TfsTestManagementRequestContext, planId);
      if (flag && bulkUpdateOutcome == null)
      {
        TestOutcomeUpdateRequest updateRequest = new TestOutcomeUpdateRequest()
        {
          TestPointIds = testPoints.Select<TestPoint, int>((Func<TestPoint, int>) (p => p.PointId)).ToArray<int>(),
          Outcome = outcome,
          UserId = this.TfsTestManagementRequestContext.UserTeamFoundationId,
          ProjectName = projectName
        };
        this.TfsTestManagementRequestContext.PlannedTestingTCMServiceHelper.TryBulkMarkTestPoints((TestManagementRequestContext) this.TfsTestManagementRequestContext, source.FirstOrDefault<TestPlan>(), runTitle1, (ITestOutcomeUpdateRequest) updateRequest);
      }
      else if (flag && bulkUpdateOutcome != null)
      {
        PlannedResultsTCMServiceHelper tcmServiceHelper = this.TfsTestManagementRequestContext.PlannedTestingTCMServiceHelper;
        TfsTestManagementRequestContext managementRequestContext = this.TfsTestManagementRequestContext;
        TestPlan testPlan = source.FirstOrDefault<TestPlan>();
        string runTitle2 = runTitle1;
        TestOutcomeUpdateRequest updateRequest = new TestOutcomeUpdateRequest();
        updateRequest.UserId = this.TfsTestManagementRequestContext.UserTeamFoundationId;
        updateRequest.ProjectName = projectName;
        Dictionary<int, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome> bulkUpdate = bulkUpdateOutcome;
        List<int> testPointIds1 = testPointIds;
        tcmServiceHelper.TryBulkMarkTestPoints((TestManagementRequestContext) managementRequestContext, testPlan, runTitle2, (ITestOutcomeUpdateRequest) updateRequest, bulkUpdate, testPointIds1);
      }
      else
      {
        TestRunHelper.CreateTestRun(runTitle1, source.First<TestPlan>(), testCaseResults, outcome, utcNow, this.TfsTestManagementRequestContext.UserTeamFoundationId, (TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, TestRunType.Web);
        if (bulkUpdateOutcome != null)
        {
          foreach (TestCaseResult testCaseResult in testCaseResults)
          {
            testCaseResult.Outcome = (byte) bulkUpdateOutcome.GetValueOrDefault<int, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(testCaseResult.TestPointId, Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Unspecified);
            testCaseResult.State = (byte) 5;
          }
        }
        else
          TestResultHelper.UpdateResultWithOutcome(testCaseResults, outcome);
        TestResultHelper.SaveTestResults(testCaseResults, (TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName);
      }
    }

    internal virtual void UpdateTesterForPoints(
      List<TestPoint> testPoints,
      string projectName,
      string testerDisplayName)
    {
      Microsoft.VisualStudio.Services.Identity.Identity tester = this.ReadIdentityByDisplayName(testerDisplayName);
      this.UpdateTesterForPointsImpl(testPoints, projectName, tester);
    }

    internal virtual void UpdateTesterForPoints(
      List<TestPoint> testPoints,
      string projectName,
      Guid testerTeamFoundationId)
    {
      if (testerTeamFoundationId == Guid.Empty)
        this.UpdateTesterForPointsImpl(testPoints, projectName, new Microsoft.VisualStudio.Services.Identity.Identity());
      else
        this.UpdateTesterForPointsImpl(testPoints, projectName, this.ReadIdentityByAccountId(testerTeamFoundationId));
    }

    internal virtual void UpdateTesterForPointsImpl(
      List<TestPoint> testPoints,
      string projectName,
      Microsoft.VisualStudio.Services.Identity.Identity tester)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(tester, nameof (tester), this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<List<TestPoint>>(testPoints, nameof (testPoints), this.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName), this.RequestContext.ServiceName);
      bool considerUnassignedTesters = false;
      if (tester.Id == Guid.Empty)
        considerUnassignedTesters = true;
      UpdatedProperties[] updatedPropertiesArray = TestPoint.AssignTester((TestManagementRequestContext) this.TfsTestManagementRequestContext, testPoints.ToArray(), projectName, tester.Id, considerUnassignedTesters);
      if (updatedPropertiesArray == null || updatedPropertiesArray.Length == 0)
        throw new TestObjectNotFoundException(ServerResources.TestPointsNotFound, ObjectTypes.TestPoint);
    }

    internal virtual string GetRunTitle(int suiteId, int planId, string projectName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName), this.RequestContext.ServiceName);
      string empty = string.Empty;
      string suiteName = this.GetSuiteName(planId, suiteId, projectName);
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BulkMarkRunTitle, string.IsNullOrEmpty(suiteName) ? (object) this.PlansHelper.GetPlanName(planId, projectName) : (object) suiteName);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint ConvertBasicTestPointToDataContract(
      TestPoint testPoint,
      List<string> workItemFields)
    {
      if (testPoint == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint()
      {
        Id = testPoint.PointId,
        TestCase = new WorkItemReference()
        {
          Id = testPoint.TestCaseId.ToString()
        },
        Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) testPoint.LastResultOutcome),
        WorkItemProperties = (object[]) this.ConstructWorkItemProperties(testPoint, workItemFields),
        State = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestPointState), (object) testPoint.State),
        LastResultState = Enum.GetName(typeof (TestResultState), (object) testPoint.LastResultState)
      };
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint ConvertTestPointToDataContract(
      TestPoint testPoint,
      TeamProjectReference projectReference,
      List<string> workItemFields,
      bool includePointDetails = false,
      TestPlan testPlan = null,
      ServerTestSuite testSuite = null,
      Dictionary<Guid, IdentityRef> lastUpdatedBy = null)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectReference, nameof (projectReference), this.RequestContext.ServiceName);
      string name = projectReference.Name;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint dataContract = this.ConvertBasicTestPointToDataContract(testPoint, workItemFields);
      if (dataContract == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint) null;
      dataContract.AssignedTo = new IdentityRef()
      {
        Id = testPoint.AssignedTo.ToString(),
        DisplayName = testPoint.AssignedToName
      };
      dataContract.Configuration = new ShallowReference()
      {
        Id = testPoint.ConfigurationId.ToString(),
        Name = testPoint.ConfigurationName
      };
      dataContract.LastTestRun = new ShallowReference()
      {
        Id = testPoint.LastTestRunId.ToString()
      };
      dataContract.LastResult = new ShallowReference()
      {
        Id = testPoint.LastTestResultId.ToString()
      };
      dataContract.Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestPointProject, (object) new
      {
        pointIds = testPoint.PointId,
        project = name,
        planId = testPoint.PlanId,
        suiteId = testPoint.SuiteId
      });
      dataContract.LastRunBuildNumber = testPoint.LastRunBuildNumber;
      if (this.LastResultsAreNotEmpty(testPoint.LastResultDetails))
        dataContract.LastResultDetails = new Microsoft.TeamFoundation.TestManagement.WebApi.LastResultDetails()
        {
          DateCompleted = testPoint.LastResultDetails.DateCompleted,
          Duration = testPoint.LastResultDetails.Duration,
          RunBy = new IdentityRef()
          {
            Id = testPoint.LastResultDetails.RunBy.ToString(),
            DisplayName = testPoint.LastResultDetails.RunByName
          }
        };
      PointWorkItemProperty[] workItemProperties = this.ConstructWorkItemProperties(testPoint, workItemFields);
      string testCaseName = this.GetTestCaseName(workItemProperties);
      dataContract.Automated = this.GetAutomationStatus(workItemProperties);
      dataContract.TestCase = this.GetTestCaseWorkitemReference(testPoint);
      if (dataContract.TestCase != null)
        dataContract.TestCase.Name = testCaseName;
      if (includePointDetails)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentityByAccountId(testPoint.AssignedTo);
        if (identity != null)
          dataContract.AssignedTo = this.CreateTeamFoundationIdentityReference(identity);
        if (!string.IsNullOrEmpty(testPoint.Comment))
          dataContract.Comment = testPoint.Comment;
        dataContract.Configuration = new ShallowReference()
        {
          Id = testPoint.ConfigurationId.ToString(),
          Name = testPoint.ConfigurationName
        };
        TestFailureType testFailureType = TestFailureType.Query((TestManagementRequestContext) this.TfsTestManagementRequestContext, (int) testPoint.FailureType, name).FirstOrDefault<TestFailureType>();
        if (testFailureType != null)
          dataContract.FailureType = testFailureType.Name;
        dataContract.LastResolutionStateId = testPoint.LastResolutionStateId;
        if (testPoint.LastTestRunId != 0)
          dataContract.LastTestRun = this.GetRunRepresentation(name, testPoint.LastTestRunId);
        if (testPoint.LastTestResultId != 0)
          dataContract.LastResult = this.GetTestResultRepresentation(name, testPoint.LastTestRunId, testPoint.LastTestResultId);
        dataContract.LastUpdatedDate = testPoint.LastUpdated;
        dataContract.LastUpdatedBy = this.GetIdentity(testPoint.LastUpdatedBy, lastUpdatedBy);
        dataContract.Revision = testPoint.Revision;
        Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint testPoint1 = dataContract;
        string projectName = name;
        int suiteId = testPoint.SuiteId;
        List<ServerTestSuite> testSuites;
        if (testSuite == null)
        {
          testSuites = (List<ServerTestSuite>) null;
        }
        else
        {
          testSuites = new List<ServerTestSuite>();
          testSuites.Add(testSuite);
        }
        TestPlan testPlan1 = testPlan;
        ShallowReference suiteRepresentation = this.GetSuiteRepresentation(projectName, suiteId, testSuites, testPlan1);
        testPoint1.Suite = suiteRepresentation;
        dataContract.TestPlan = this.GetPlanRepresentation(name, testPoint.PlanId, testPlan);
        dataContract.LastResetToActive = testPoint.LastResetToActive;
        dataContract.WorkItemProperties = (object[]) this.ConstructWorkItemProperties(testPoint, workItemFields);
      }
      return dataContract;
    }

    internal bool GetAutomationStatus(PointWorkItemProperty[] workItemProperties)
    {
      if (workItemProperties != null)
      {
        PointWorkItemProperty workItemProperty = ((IEnumerable<PointWorkItemProperty>) workItemProperties).FirstOrDefault<PointWorkItemProperty>((Func<PointWorkItemProperty, bool>) (x => x.WorkItem.Key == WorkItemFieldNames.TestName));
        if (workItemProperty != null)
        {
          object obj = workItemProperty.WorkItem.Value;
          if (obj != null && obj.ToString() != string.Empty)
            return true;
        }
      }
      return false;
    }

    internal string GetTestCaseName(PointWorkItemProperty[] workItemProperties)
    {
      if (workItemProperties != null)
      {
        PointWorkItemProperty workItemProperty = ((IEnumerable<PointWorkItemProperty>) workItemProperties).FirstOrDefault<PointWorkItemProperty>((Func<PointWorkItemProperty, bool>) (x => x.WorkItem.Key == WorkItemFieldNames.Title));
        if (workItemProperty != null)
        {
          object obj = workItemProperty.WorkItem.Value;
          if (obj != null && obj.ToString() != string.Empty)
            return obj.ToString();
        }
      }
      return string.Empty;
    }

    internal virtual WorkItemReference GetTestCaseWorkitemReference(TestPoint testPoint)
    {
      if (testPoint == null || testPoint.TestCaseId <= 0)
        return (WorkItemReference) null;
      int testCaseId = testPoint.TestCaseId;
      if (this.testcaseCache.ContainsKey(testCaseId))
        return this.testcaseCache[testCaseId];
      WorkItemReference itemRepresentation = this.GetWorkItemRepresentation(testCaseId);
      this.testcaseCache[testCaseId] = itemRepresentation;
      return itemRepresentation;
    }

    private bool LastResultsAreNotEmpty(LastResultDetails lastResultDetails) => lastResultDetails != null && (!lastResultDetails.DateCompleted.Equals(DateTime.MinValue) || !lastResultDetails.Duration.Equals(0L) || !lastResultDetails.RunBy.Equals(Guid.Empty));

    internal virtual string GetAllPointsInSuiteQuery(
      int suiteId,
      bool newApiFilter,
      string configurationId = null,
      string testCaseId = null,
      string testPointIds = null,
      bool isRecursive = false)
    {
      string str = "";
      bool flag = false;
      if (newApiFilter)
      {
        if (!string.IsNullOrEmpty(testPointIds))
        {
          flag = true;
          str += string.Format(" AND PointId IN ({0})", (object) testPointIds);
        }
        if (!string.IsNullOrEmpty(testCaseId))
        {
          flag = true;
          str += string.Format(" AND TestCaseId IN ({0})", (object) testCaseId);
        }
        if (!string.IsNullOrEmpty(configurationId))
        {
          flag = true;
          str += string.Format(" AND ConfigurationId IN ({0})", (object) configurationId);
        }
      }
      else if (!string.IsNullOrEmpty(configurationId))
      {
        flag = true;
        str += string.Format(" AND ConfigurationId = {0}", (object) configurationId);
      }
      else if (!string.IsNullOrEmpty(testCaseId))
      {
        flag = true;
        str += string.Format(" AND TestCaseId = {0}", (object) testCaseId);
      }
      else if (!string.IsNullOrEmpty(testPointIds))
      {
        flag = true;
        str += string.Format(" AND PointId IN ({0})", (object) testPointIds);
      }
      return !isRecursive ? (flag ? string.Format("SELECT * FROM TestPoint WHERE SuiteId = {0}{1} ORDER BY PointId", (object) suiteId, (object) str) : string.Format("SELECT * FROM TestPoint WHERE SuiteId = {0} ORDER BY PointId", (object) suiteId)) : (flag ? string.Format("SELECT * FROM TestPoint WHERE RecursiveSuiteId = {0}{1} ORDER BY PointId", (object) suiteId, (object) str) : string.Format("SELECT * FROM TestPoint WHERE RecursiveSuiteId = {0} ORDER BY PointId", (object) suiteId));
    }

    internal virtual string GetSuiteName(int planId, int suiteId, string projectName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName), this.RequestContext.ServiceName);
      List<int> intList = new List<int>();
      List<ServerTestSuite> source = ServerTestSuite.Fetch((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, new IdAndRev[1]
      {
        new IdAndRev() { Id = suiteId }
      }, (List<int>) null);
      if (intList.Count > 0)
        throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, intList[0], ObjectTypes.TestSuite);
      if (source != null && source.Count > 0 && source.First<ServerTestSuite>().ParentId == 0)
        return this.PlansHelper.GetPlanName(planId, projectName);
      return source != null && source.Count > 0 ? source.First<ServerTestSuite>().Title : string.Empty;
    }

    internal List<string> GetWorkItemFieldsForPoints(string witFields)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      bool flag5 = false;
      List<string> source = new List<string>();
      if (!string.IsNullOrEmpty(witFields))
        source = ((IEnumerable<string>) witFields.Split(new string[1]
        {
          ","
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
      if (source.Count == 0)
      {
        source.Add(WorkItemFieldNames.AutomationStatus);
        source.Add(WorkItemFieldNames.TestName);
        source.Add(WorkItemFieldNames.Title);
        source.Add(WorkItemFieldNames.Owner);
        source.Add(WorkItemFieldNames.State);
      }
      else
      {
        for (int index = 0; index < source.Count; ++index)
        {
          if (source.ElementAt<string>(index).ToLower().Equals(WorkItemFieldNames.AutomationStatus.ToLower()))
            flag1 = true;
          if (source.ElementAt<string>(index).ToLower().Equals(WorkItemFieldNames.TestName.ToLower()))
            flag2 = true;
          if (source.ElementAt<string>(index).ToLower().Equals(WorkItemFieldNames.Title.ToLower()))
            flag3 = true;
          if (source.ElementAt<string>(index).ToLower().Equals(WorkItemFieldNames.Owner.ToLower()))
            flag4 = true;
          if (source.ElementAt<string>(index).ToLower().Equals(WorkItemFieldNames.State.ToLower()))
            flag5 = true;
        }
        if (!flag1)
          source.Add(WorkItemFieldNames.AutomationStatus);
        if (!flag2)
          source.Add(WorkItemFieldNames.TestName);
        if (!flag3)
          source.Add(WorkItemFieldNames.Title);
        if (!flag4)
          source.Add(WorkItemFieldNames.Owner);
        if (!flag5)
          source.Add(WorkItemFieldNames.State);
      }
      return source;
    }

    internal virtual PointWorkItemProperty[] ConstructWorkItemProperties(
      TestPoint testPoint,
      List<string> workItemFields)
    {
      List<PointWorkItemProperty> workItemPropertyList = new List<PointWorkItemProperty>();
      if (testPoint != null && testPoint.WorkItemProperties != null)
      {
        for (int index = 0; index < workItemFields.Count; ++index)
        {
          PointWorkItemProperty workItemProperty = new PointWorkItemProperty()
          {
            WorkItem = new KeyValuePair<string, object>(workItemFields[index], testPoint.WorkItemProperties[index])
          };
          workItemPropertyList.Add(workItemProperty);
        }
      }
      return workItemPropertyList.ToArray();
    }

    internal List<TestPoint> FetchTestPoints(
      int planId,
      string projectName,
      List<int> pointIds,
      List<string> workItemFields,
      bool returnIdentityRef = false)
    {
      List<int> deletedIds = new List<int>();
      List<TestPoint> testPointList = TestPoint.Fetch((TestManagementRequestContext) this.TfsTestManagementRequestContext, projectName, planId, pointIds.Select<int, IdAndRev>((Func<int, IdAndRev>) (pointId => new IdAndRev()
      {
        Id = pointId
      })).ToArray<IdAndRev>(), workItemFields.ToArray(), deletedIds, returnIdentityRef: returnIdentityRef);
      if (deletedIds.Count <= 0)
        return testPointList;
      throw new TestObjectNotFoundException(this.TestManagementRequestContext.RequestContext, deletedIds[0], ObjectTypes.TestPoint);
    }

    private TestPlansHelper PlansHelper
    {
      get
      {
        if (this.m_plansHelper == null)
          this.m_plansHelper = new TestPlansHelper(this.TestManagementRequestContext);
        return this.m_plansHelper;
      }
    }

    internal void UpdateOutcomeForTestPoints(
      List<TestPoint> list,
      int v1,
      int v2,
      string v3,
      Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome> isAny)
    {
      throw new NotImplementedException();
    }
  }
}
