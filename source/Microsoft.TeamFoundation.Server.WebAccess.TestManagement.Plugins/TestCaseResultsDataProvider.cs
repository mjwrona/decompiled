// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestCaseResultsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestCaseResultsDataProvider : IExtensionDataProvider
  {
    public string Name => "TestManagement.Provider.TestCaseResultsDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      Guid empty1 = Guid.Empty;
      Guid empty2 = Guid.Empty;
      string empty3 = string.Empty;
      string empty4 = string.Empty;
      using (PerformanceTimer.StartMeasure(requestContext, "TestCaseResultsDataProvider.GetProjectAndTeamData"))
        (_, empty3, _, empty4) = this.GetProjectAndTeamData(requestContext);
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(requestContext);
      ProjectInfo projectInfo = managementRequestContext.ProjectServiceHelper.GetProjectFromName(empty3);
      int testCaseId = 0;
      int num = 0;
      object obj;
      if (providerContext != null && providerContext.Properties != null && providerContext.Properties.TryGetValue("testCaseId", out obj))
        testCaseId = Convert.ToInt32(obj);
      if (testCaseId == 0)
      {
        testCaseId = this.ParseRequestAndGetTestCaseId(new Uri(empty4));
        num = this.ParseRequestAndGetContextPointId(new Uri(empty4));
      }
      TestResultsQuery resultsQuery = new TestResultsQuery()
      {
        ResultsFilter = new ResultsFilter()
        {
          TestCaseId = testCaseId,
          AutomatedTestName = string.Empty
        }
      };
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList;
      using (PerformanceTimer.StartMeasure(requestContext, "TestCaseResultsDataProvider.GetTestResults"))
      {
        if (!requestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        {
          testCaseResultList = this.GetTestResults(managementRequestContext, projectInfo, resultsQuery);
        }
        else
        {
          new ResultsHelper((TestManagementRequestContext) managementRequestContext).CheckForViewTestResultPermission(projectInfo.Id);
          TestResultsHttpClient testResultsHttpClient = requestContext.GetClient<TestResultsHttpClient>();
          TestResultsQuery testResultsQuery = Utils.InvokeAction<TestResultsQuery>((Func<TestResultsQuery>) (() => testResultsHttpClient.GetTestResultsByQueryAsync(resultsQuery, projectInfo.Id, (object) null, new CancellationToken())?.Result));
          requestContext.CheckPermissionToReadPublicIdentityInfo();
          testResultsQuery.InitializeSecureObject((ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
          {
            Project = new ShallowReference()
            {
              Id = projectInfo.Id.ToString()
            }
          });
          testCaseResultList = testResultsQuery.Results;
        }
      }
      bool flag = false;
      Dictionary<int, Microsoft.TeamFoundation.TestManagement.Server.TestPoint> dictionary = new Dictionary<int, Microsoft.TeamFoundation.TestManagement.Server.TestPoint>();
      IList<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> testPoints;
      using (PerformanceTimer.StartMeasure(requestContext, "TestCaseResultsDataProvider.GetPointsByQuery"))
      {
        testPoints = this.GetTestPoints(managementRequestContext, empty3, testCaseId);
        flag = testPoints != null && testPoints.Count > 0;
        if (flag)
          dictionary = testPoints.ToDictionary<Microsoft.TeamFoundation.TestManagement.Server.TestPoint, int>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestPoint, int>) (row => row.PointId));
      }
      this.PopulateSuiteAndConfigurationDetails(managementRequestContext, projectInfo, testCaseResultList);
      this.PopulatePlanAndSuiteNames(managementRequestContext, testCaseResultList.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(), projectInfo.Name);
      IList<TestCaseAssociatedResult> associatedResultList = (IList<TestCaseAssociatedResult>) new List<TestCaseAssociatedResult>();
      TestPointDetailedReference detailedReference = (TestPointDetailedReference) null;
      string str = (string) null;
      if (flag && testPoints.First<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>().WorkItemProperties != null && ((IEnumerable<object>) testPoints.First<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>().WorkItemProperties).Count<object>() > 0)
        str = ((IEnumerable<object>) testPoints.First<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>().WorkItemProperties).First<object>().ToString();
      using (PerformanceTimer.StartMeasure(requestContext, "TestCaseResultsDataProvider.ConvertToDataContract"))
      {
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult in (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResultList)
        {
          if (testCaseResult != null && testCaseResult.TestPoint != null)
          {
            if (str == null)
              str = testCaseResult.TestCase.Name;
            if (flag && dictionary.ContainsKey(Convert.ToInt32(testCaseResult.TestPoint.Id)))
            {
              Microsoft.TeamFoundation.TestManagement.Server.TestPoint point = dictionary[Convert.ToInt32(testCaseResult.TestPoint.Id)];
              TestCaseAssociatedResult associatedResult = new TestCaseAssociatedResult()
              {
                RunId = Convert.ToInt32(testCaseResult.TestRun.Id),
                ResultId = testCaseResult.Id,
                PointId = point.PointId,
                RunBy = testCaseResult.RunBy,
                CompletedDate = testCaseResult.CompletedDate,
                Plan = new TestPlanReference()
                {
                  Id = point.PlanId,
                  Name = point.PlanName
                },
                Suite = new TestSuiteReference()
                {
                  Id = point.SuiteId,
                  Name = point.SuiteName
                },
                Configuration = new TestConfigurationReference()
                {
                  Id = point.ConfigurationId,
                  Name = point.ConfigurationName
                },
                Outcome = Utils.GetUserFriendlyTestOutcome(testCaseResult.Outcome, testCaseResult.State)
              };
              associatedResult.Tester = this.GetIdentity(managementRequestContext, point);
              associatedResultList.Add(associatedResult);
              if (associatedResult.PointId == num)
                detailedReference = new TestPointDetailedReference()
                {
                  PointId = associatedResult.PointId,
                  Plan = associatedResult.Plan,
                  Suite = associatedResult.Suite,
                  Configuration = associatedResult.Configuration,
                  Tester = associatedResult.Tester
                };
            }
            else if (testCaseResult.Configuration != null && testCaseResult.TestPlan != null && testCaseResult.TestSuite != null)
            {
              TestCaseAssociatedResult associatedResult = new TestCaseAssociatedResult()
              {
                RunId = Convert.ToInt32(testCaseResult.TestRun.Id),
                ResultId = testCaseResult.Id,
                PointId = Convert.ToInt32(testCaseResult.TestPoint.Id),
                RunBy = testCaseResult.RunBy,
                CompletedDate = testCaseResult.CompletedDate,
                Plan = new TestPlanReference()
                {
                  Id = Convert.ToInt32(testCaseResult.TestPlan.Id),
                  Name = testCaseResult.TestPlan.Name
                },
                Suite = new TestSuiteReference()
                {
                  Id = Convert.ToInt32(testCaseResult.TestSuite.Id),
                  Name = testCaseResult.TestSuite.Name
                },
                Configuration = new TestConfigurationReference()
                {
                  Id = Convert.ToInt32(testCaseResult.Configuration.Id),
                  Name = testCaseResult.Configuration.Name
                },
                Outcome = Utils.GetUserFriendlyTestOutcome(testCaseResult.Outcome, testCaseResult.State)
              };
              associatedResultList.Add(associatedResult);
            }
          }
        }
      }
      return (object) new TestCaseResultsData()
      {
        Results = associatedResultList,
        TestCaseName = str,
        ContextPoint = detailedReference
      };
    }

    protected internal virtual IdentityRef GetIdentity(
      TfsTestManagementRequestContext tfsTestManagementRequestContext,
      Microsoft.TeamFoundation.TestManagement.Server.TestPoint point)
    {
      PointsHelper pointsHelper = new PointsHelper((TestManagementRequestContext) tfsTestManagementRequestContext);
      Microsoft.VisualStudio.Services.Identity.Identity identity = pointsHelper.ReadIdentityByAccountId(point.AssignedTo);
      return identity != null ? pointsHelper.CreateTeamFoundationIdentityReference(identity) : (IdentityRef) null;
    }

    protected internal virtual void PopulatePlanAndSuiteNames(
      TfsTestManagementRequestContext tfsTestManagementRequestContext,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results,
      string projectName)
    {
      List<int> source = new List<int>();
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result in results)
      {
        if (result.TestPlan != null)
          source.Add(Convert.ToInt32(result.TestPlan.Id));
      }
      IEnumerable<int> testPlanIds = source.Distinct<int>();
      Dictionary<int, string> testPlanNames = this.GetTestPlanNames(tfsTestManagementRequestContext, testPlanIds, projectName);
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result in results)
      {
        if (result.TestPlan != null)
        {
          result.TestPlan.Name = testPlanNames[Convert.ToInt32(result.TestPlan.Id)];
          if (result.TestSuite != null && !string.IsNullOrEmpty(result.TestSuite.Name))
            result.TestSuite.Name = result.TestSuite.Name.Equals(TestManagementServerResources.RootSuiteTitle, StringComparison.OrdinalIgnoreCase) ? result.TestPlan.Name : result.TestSuite.Name;
        }
      }
    }

    protected internal virtual Dictionary<int, string> GetTestPlanNames(
      TfsTestManagementRequestContext tfsTestManagementRequestContext,
      IEnumerable<int> testPlanIds,
      string projectName)
    {
      Dictionary<int, string> testPlanNames = new Dictionary<int, string>();
      List<Microsoft.TeamFoundation.TestManagement.Server.TestPlan> source = Microsoft.TeamFoundation.TestManagement.Server.TestPlan.Fetch((TestManagementRequestContext) tfsTestManagementRequestContext, testPlanIds.Select<int, IdAndRev>((Func<int, IdAndRev>) (planId => new IdAndRev()
      {
        Id = planId
      })).ToArray<IdAndRev>(), new List<int>(), projectName, false, false);
      foreach (int testPlanId in testPlanIds)
      {
        int planId = testPlanId;
        Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan = source.Where<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestPlan, bool>) (p => p.PlanId == planId)).FirstOrDefault<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>();
        testPlanNames[planId] = testPlan == null ? (string) null : testPlan.Name;
      }
      return testPlanNames;
    }

    protected internal virtual void PopulateSuiteAndConfigurationDetails(
      TfsTestManagementRequestContext tfsTestManagementRequestContext,
      ProjectInfo projectInfo,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsList)
    {
      new ResultsHelper((TestManagementRequestContext) tfsTestManagementRequestContext).PopulateSuiteAndConfigurationDetails(resultsList.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(), projectInfo.Id);
    }

    protected internal virtual IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetTestResults(
      TfsTestManagementRequestContext tfsTestManagementRequestContext,
      ProjectInfo projectInfo,
      TestResultsQuery resultsQuery)
    {
      return new ResultsHelper((TestManagementRequestContext) tfsTestManagementRequestContext).GetTestResults(new GuidAndString(projectInfo.Uri, projectInfo.Id), resultsQuery).Results;
    }

    protected internal virtual IList<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> GetTestPoints(
      TfsTestManagementRequestContext tfsTestManagementRequestContext,
      string projectName,
      int testCaseId)
    {
      return (IList<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>) new PointsHelper((TestManagementRequestContext) tfsTestManagementRequestContext).GetPointsByQuery(projectName, new List<int>()
      {
        testCaseId
      }, 0, int.MaxValue);
    }

    protected internal virtual (Guid projectId, string projectName, Guid teamId, string pageUrl) GetProjectAndTeamData(
      IVssRequestContext requestContext)
    {
      return Utils.GetProjectAndTeamData(requestContext);
    }

    protected internal virtual int ParseRequestAndGetTestCaseId(Uri url)
    {
      int result = 0;
      int.TryParse(HttpUtility.ParseQueryString(url.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped))["testCaseId"], out result);
      return result;
    }

    protected internal virtual int ParseRequestAndGetContextPointId(Uri url)
    {
      int result = int.MinValue;
      int.TryParse(HttpUtility.ParseQueryString(url.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped))["contextPointId"], out result);
      return result;
    }
  }
}
