// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.RevisedPointsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  internal class RevisedPointsHelper : PointsHelper
  {
    public RevisedPointsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> GetNewPoints(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      string pointIds,
      string testcaseid,
      int skip,
      int top,
      int watermark = -2147483648,
      bool returnIdentityRef = false,
      bool includePointDetails = true,
      bool isRecursive = false,
      bool excludeRunBy = false)
    {
      this.ValidateProjectArgument(projectInfo);
      TeamProjectReference projectReference = this.GetProjectReference(projectInfo.Name);
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan;
      if (!this.TryGetPlanFromPlanId(projectReference.Name, planId, out testPlan))
        throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(this.RequestContext, planId, ObjectTypes.TestPlan);
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(projectReference.Name, suiteId, out testSuite))
        throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
      List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> pointsHelper = this.GetPointsHelper(projectReference, testPlan, testSuite, "", "", testcaseid, pointIds, includePointDetails, skip, top, watermark, true, returnIdentityRef, isRecursive, excludeRunBy);
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> newPoints = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>();
      List<Guid> source = new List<Guid>();
      foreach (Microsoft.TeamFoundation.TestManagement.Server.TestPoint testPoint in pointsHelper)
      {
        if (testPoint.LastUpdatedBy != Guid.Empty)
          source.Add(testPoint.LastUpdatedBy);
      }
      Dictionary<Guid, IdentityRef> identityMapping = this.GetIdentityMapping(source.Distinct<Guid>().ToList<Guid>());
      List<string> itemFieldsForPoints = this.GetWorkItemFieldsForPoints(string.Empty);
      foreach (Microsoft.TeamFoundation.TestManagement.Server.TestPoint testPoint in pointsHelper)
      {
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint pointToDataContract = this.ConvertToTestPlanningTestPointToDataContract(testPoint, projectReference, itemFieldsForPoints, testPlan, testSuite, includePointDetails, identityMapping);
        if (pointToDataContract != null)
          newPoints.Add(pointToDataContract);
      }
      return newPoints;
    }

    public List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> GetPoints(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      string pointIds,
      bool returnIdentityRef = false,
      bool includePointDetails = true)
    {
      this.ValidateProjectArgument(projectInfo);
      TeamProjectReference projectReference = this.GetProjectReference(projectInfo.Name);
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan;
      if (!this.TryGetPlanFromPlanId(projectReference.Name, planId, out testPlan))
        throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(this.RequestContext, planId, ObjectTypes.TestPlan);
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(projectReference.Name, suiteId, out testSuite))
        throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
      List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> pointByIdHelper = this.GetPointByIdHelper(projectReference, testPlan, testSuite, pointIds, "", true, returnIdentityRef);
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> points = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>();
      List<string> itemFieldsForPoints = this.GetWorkItemFieldsForPoints(string.Empty);
      foreach (Microsoft.TeamFoundation.TestManagement.Server.TestPoint testPoint in pointByIdHelper)
      {
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint pointToDataContract = this.ConvertToTestPlanningTestPointToDataContract(testPoint, projectReference, itemFieldsForPoints, testPlan, testSuite, includePointDetails);
        if (pointToDataContract != null)
          points.Add(pointToDataContract);
      }
      return points;
    }

    public List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> PatchNewTestPoints(
      ProjectInfo projectInfo,
      int planId,
      int suiteId,
      List<TestPointUpdateParams> updateTestPointParams,
      bool includePointDetails = true,
      bool returnIdentityRef = false)
    {
      this.ValidateProjectArgument(projectInfo);
      TeamProjectReference projectReference = this.GetProjectReference(projectInfo.Name);
      this.ValidateProjectArgument(projectReference);
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan;
      if (!this.TryGetPlanFromPlanId(projectReference.Name, planId, out testPlan))
        throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(this.RequestContext, planId, ObjectTypes.TestPlan);
      ServerTestSuite testSuite;
      if (!this.TryGetSuiteFromSuiteId(projectReference.Name, suiteId, out testSuite))
        throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(this.RequestContext, suiteId, ObjectTypes.TestSuite);
      if (updateTestPointParams == null)
        throw new ArgumentException(nameof (updateTestPointParams)).Expected(this.RequestContext.ServiceName);
      if (updateTestPointParams.Count == 0)
        return new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>();
      int num = this.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/BulkUpdateTestPointMaxLimit", 5000);
      if (updateTestPointParams.Count > num)
        throw new ArgumentOutOfRangeException("Too many points to update");
      List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> testPointList1 = new List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>();
      bool? resetToActive;
      for (int index = 0; index < updateTestPointParams.Count; ++index)
      {
        if (updateTestPointParams.ElementAt<TestPointUpdateParams>(index) == null)
          throw new ArgumentException("updateTestPointParams.ElementAt(" + index.ToString() + ")").Expected(this.RequestContext.ServiceName);
        resetToActive = updateTestPointParams.ElementAt<TestPointUpdateParams>(index).IsActive;
        bool flag1 = false;
        if (resetToActive.GetValueOrDefault() == flag1 & resetToActive.HasValue)
          throw new ArgumentException("IsActive").Expected(this.RequestContext.ServiceName);
        resetToActive = updateTestPointParams.ElementAt<TestPointUpdateParams>(index).IsActive;
        bool flag2 = true;
        if (resetToActive.GetValueOrDefault() == flag2 & resetToActive.HasValue && updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Results != null)
          throw new ArgumentException("IsActive").Expected(this.RequestContext.ServiceName);
      }
      if (this.RequestContext.IsFeatureEnabled("TestManagement.Server.BulkUpdateTestPoints"))
      {
        testPointList1 = this.BulkPatchTestPoints(projectReference, testPlan, testSuite, updateTestPointParams, returnIdentityRef: returnIdentityRef);
      }
      else
      {
        for (int index = 0; index < updateTestPointParams.Count; ++index)
        {
          resetToActive = updateTestPointParams.ElementAt<TestPointUpdateParams>(index).IsActive;
          if (resetToActive.HasValue || updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Results != null || updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Tester != null)
          {
            resetToActive = new bool?();
            PointUpdateModel pointUpdateModel1 = new PointUpdateModel(resetToActive);
            PointUpdateModel pointUpdateModel2 = updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Results != null ? new PointUpdateModel(updateTestPointParams.ElementAt<TestPointUpdateParams>(index).IsActive, updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Results.Outcome.ToString(), updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Tester) : new PointUpdateModel(updateTestPointParams.ElementAt<TestPointUpdateParams>(index).IsActive, (string) null, updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Tester);
            testPointList1.Add(this.PatchTestPointsHelper(projectReference, testPlan, testSuite, updateTestPointParams.ElementAt<TestPointUpdateParams>(index).Id.ToString((IFormatProvider) CultureInfo.InvariantCulture), pointUpdateModel2, true, returnIdentityRef: returnIdentityRef).ElementAt<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>(0));
          }
        }
      }
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> testPointList2 = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>();
      List<string> itemFieldsForPoints = this.GetWorkItemFieldsForPoints(string.Empty);
      foreach (Microsoft.TeamFoundation.TestManagement.Server.TestPoint testPoint in testPointList1)
      {
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint pointToDataContract = this.ConvertToTestPlanningTestPointToDataContract(testPoint, projectReference, itemFieldsForPoints, testPlan, testSuite, includePointDetails);
        if (pointToDataContract != null)
          testPointList2.Add(pointToDataContract);
      }
      return testPointList2;
    }

    internal virtual Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint ConvertToTestPlanningTestPointToDataContract(
      Microsoft.TeamFoundation.TestManagement.Server.TestPoint testPoint,
      TeamProjectReference teamProjectReference,
      List<string> workItemFields,
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan,
      ServerTestSuite serverTestSuite,
      bool includePointDetails = false,
      Dictionary<Guid, IdentityRef> lastUpdatedBy = null)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(teamProjectReference, "projectReference", this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>(testPlan, nameof (testPlan), this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<ServerTestSuite>(serverTestSuite, "testSuite", this.RequestContext.ServiceName);
      if (testPoint == null)
        return (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint) null;
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint pointToDataContract = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint()
      {
        Id = testPoint.PointId
      };
      Outcome result1;
      Enum.TryParse<Outcome>(testPoint.LastResultOutcome.ToString(), out result1);
      byte num = testPoint.State;
      PointState result2;
      Enum.TryParse<PointState>(num.ToString(), out result2);
      pointToDataContract.Results = new TestPointResults()
      {
        Outcome = result1,
        State = result2
      };
      if (includePointDetails)
      {
        pointToDataContract.Results.LastRunBuildNumber = testPoint.LastRunBuildNumber;
        if (testPoint.LastResultDetails != null)
          pointToDataContract.Results.LastResultDetails = new Microsoft.TeamFoundation.TestManagement.WebApi.LastResultDetails()
          {
            DateCompleted = testPoint.LastResultDetails.DateCompleted,
            Duration = testPoint.LastResultDetails.Duration,
            RunBy = new IdentityRef()
            {
              Id = testPoint.LastResultDetails.RunBy.ToString(),
              DisplayName = testPoint.LastResultDetails.RunByName
            }
          };
        num = testPoint.LastResultState;
        ResultState result3;
        Enum.TryParse<ResultState>(num.ToString(), out result3);
        LastResolutionState result4;
        Enum.TryParse<LastResolutionState>(testPoint.LastResolutionStateId.ToString(), out result4);
        pointToDataContract.Results.LastResultState = result3;
        pointToDataContract.Results.LastResolutionState = result4;
      }
      pointToDataContract.Results.LastTestRunId = testPoint.LastTestRunId;
      pointToDataContract.Results.LastResultId = testPoint.LastTestResultId;
      PointWorkItemProperty[] workItemProperties = this.ConstructWorkItemProperties(testPoint, workItemFields);
      string testCaseName = this.GetTestCaseName(workItemProperties);
      WorkItemIdentity workItemField = this.GetWorkItemField<WorkItemIdentity>(workItemProperties, WorkItemFieldRefNames.AssignedTo);
      pointToDataContract.testCaseReference = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestCaseReference()
      {
        Id = testPoint.TestCaseId,
        Name = testCaseName,
        AssignedTo = workItemField?.IdentityRef,
        State = this.GetWorkItemField<string>(workItemProperties, WorkItemFieldRefNames.State)
      };
      pointToDataContract.TestPlan = new TestPlanReference()
      {
        Id = testPlan.PlanId
      };
      pointToDataContract.TestSuite = new TestSuiteReference()
      {
        Id = testPoint.SuiteId
      };
      pointToDataContract.IsAutomated = this.GetAutomationStatus(workItemProperties);
      if (includePointDetails)
      {
        pointToDataContract.Tester = new IdentityRef()
        {
          Id = testPoint.AssignedToString,
          DisplayName = testPoint.AssignedToName
        };
        if (!string.IsNullOrEmpty(testPoint.Comment))
          pointToDataContract.Comment = testPoint.Comment;
        Microsoft.TeamFoundation.TestManagement.Server.TestFailureType testFailureType = Microsoft.TeamFoundation.TestManagement.Server.TestFailureType.Query((TestManagementRequestContext) this.TfsTestManagementRequestContext, (int) testPoint.FailureType, teamProjectReference.Name).FirstOrDefault<Microsoft.TeamFoundation.TestManagement.Server.TestFailureType>();
        if (testFailureType != null)
        {
          Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.FailureType result5;
          Enum.TryParse<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.FailureType>(testFailureType.Name, out result5);
          pointToDataContract.Results.FailureType = result5;
        }
        pointToDataContract.TestPlan.Name = testPlan.Name;
        pointToDataContract.TestSuite.Name = string.IsNullOrEmpty(testPoint.SuiteName) ? (serverTestSuite.ParentId == 0 ? testPlan.Name : serverTestSuite.Title) : (serverTestSuite.ParentId != 0 || pointToDataContract.TestSuite.Id != serverTestSuite.Id ? testPoint.SuiteName : testPlan.Name);
        pointToDataContract.Project = teamProjectReference;
        pointToDataContract.LastUpdatedDate = testPoint.LastUpdated;
        pointToDataContract.LastUpdatedBy = this.GetIdentity(testPoint.LastUpdatedBy, lastUpdatedBy);
        pointToDataContract.links = new ReferenceLinks();
        pointToDataContract.links.AddLink("_self", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPoint, (object) new
        {
          project = teamProjectReference.Name,
          planId = testPlan.PlanId,
          suiteId = serverTestSuite.Id,
          pointIds = pointToDataContract.Id
        }));
        pointToDataContract.links.AddLink("sourcePlan", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestPlan, (object) new
        {
          project = teamProjectReference.Name,
          planId = testPlan.PlanId
        }));
        pointToDataContract.links.AddLink("sourceSuite", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.TestSuiteProject, (object) new
        {
          project = teamProjectReference.Name,
          planId = testPlan.PlanId,
          suiteId = serverTestSuite.Id
        }));
        pointToDataContract.links.AddLink("sourceProject", this.ProjectServiceHelper.GetProjectRepresentation(teamProjectReference.Name).Url);
        if (pointToDataContract.testCaseReference != null)
          pointToDataContract.links.AddLink("testCases", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "testplan", TestPlanResourceIds.SuiteTestCase, (object) new
          {
            project = teamProjectReference.Name,
            planId = testPlan.PlanId,
            suiteId = serverTestSuite.Id,
            testCaseIds = pointToDataContract.testCaseReference.Id
          }));
        if (pointToDataContract.Results.LastTestRunId != 0)
        {
          pointToDataContract.links.AddLink("run", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestRunProject, (object) new
          {
            runId = pointToDataContract.Results.LastTestRunId,
            project = teamProjectReference.Name
          }));
          if (pointToDataContract.Results.LastResultId != 0)
            pointToDataContract.links.AddLink("result", UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestResultProject, (object) new
            {
              runId = pointToDataContract.Results.LastTestRunId,
              testCaseResultId = pointToDataContract.Results.LastResultId,
              project = teamProjectReference.Name
            }));
        }
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentityByAccountId(testPoint.AssignedTo);
        if (identity != null)
          pointToDataContract.Tester = this.CreateTeamFoundationIdentityReference(identity);
        pointToDataContract.LastResetToActive = testPoint.LastResetToActive;
        pointToDataContract.Configuration = new TestConfigurationReference()
        {
          Id = testPoint.ConfigurationId,
          Name = testPoint.ConfigurationName
        };
        if (pointToDataContract.Results.State == PointState.Ready)
          pointToDataContract.IsActive = true;
      }
      return pointToDataContract;
    }

    internal T GetWorkItemField<T>(PointWorkItemProperty[] workItemProperties, string witField)
    {
      if (workItemProperties != null)
      {
        PointWorkItemProperty workItemProperty = ((IEnumerable<PointWorkItemProperty>) workItemProperties).FirstOrDefault<PointWorkItemProperty>((Func<PointWorkItemProperty, bool>) (x => string.Equals(x.WorkItem.Key, witField, StringComparison.OrdinalIgnoreCase)));
        if (workItemProperty != null && workItemProperty.WorkItem.Value is T)
          return (T) workItemProperty.WorkItem.Value;
      }
      return default (T);
    }
  }
}
