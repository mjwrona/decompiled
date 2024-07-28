// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestPlansHubRefreshDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.TestPlanning;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestPlansHubRefreshDataProvider : IExtensionDataProvider
  {
    private const int c_DefaultTestCasePageSize = 500;
    private const int c_DefaultTestPointPageSize = 1000;
    private const string c_DefineColumnOptionsKey = "DefineColumnOptions";
    private const string c_DefinePivotName = "define";
    private const string c_ExecuteColumnOptionsKey = "ExecuteColumnOptions";
    private const string c_ExecutePivotName = "execute";
    private const string c_NewDefineColumnOptionKey = "NewDefineColumnOptions";
    private const string c_NewExecuteColumnOptionKey = "NewExecuteColumnOptions";
    private const int c_NumberOfSuitesToFetch = 50;
    private const int c_NumberOfTestCasesOrPointsToFetch = 25;
    private const string c_SelectedPlanIdKey = "SelectedPlanId";
    private const string c_SelectedPivotIdKey = "SelectedPivotId";
    private const string c_SelectedSuiteIdKey = "SelectedSuiteId";
    private const string c_TestCasesPageSizeRegistryPath = "/Service/TestManagement/Settings/TestHubRefreshTestCasesPageSize";
    private const string c_TestPointsPageSizeRegistryPath = "/Service/TestManagement/Settings/TestHubRefreshTestPointsPageSize";

    public string Name => "TestManagement.Provider.TestPlansHubRefreshDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      requestContext.GetService<IClientFeatureProviderService>().AddFeatureState(requestContext, providerContext.SharedData, "ms.vss-test-web.nwp-test-steps-control-feature");
      Guid empty1 = Guid.Empty;
      Guid empty2 = Guid.Empty;
      string empty3 = string.Empty;
      string empty4 = string.Empty;
      using (PerformanceTimer.StartMeasure(requestContext, "TestPlansHubRefreshDataProvider.GetProjectAndTeamData"))
        (empty1, empty3, _, empty4) = this.GetProjectAndTeamData(requestContext);
      (int selectedPlanId, int selectedSuiteId) = this.ParseRequestAndGetPlanAndSuiteId(new Uri(empty4));
      IDictionary<string, object> routeValues = requestContext.GetService<IContributionRoutingService>().GetRouteValues(requestContext);
      bool flag1 = this.IsAdvancedExtensionEnabled(requestContext);
      string str1 = "";
      object obj;
      if (routeValues != null && routeValues.TryGetValue("pivots", out obj))
      {
        str1 = (string) obj;
        if (!flag1 && str1.Equals("define"))
          str1 = "execute";
      }
      if (selectedPlanId < 0)
        return (object) new TestPlansHubRefreshData();
      string lastSelectedPivotId = "";
      bool flag2 = true;
      string[] second1 = new string[0];
      string[] second2 = new string[0];
      ISettingsService service1 = requestContext.GetService<ISettingsService>();
      int planId;
      int suiteId;
      using (PerformanceTimer.StartMeasure(requestContext, "TestPlansHubRefreshDataProvider.SelectTestPlanId"))
      {
        if (selectedPlanId == 0)
        {
          flag2 = false;
          int num1 = service1.GetValue<int>(requestContext, SettingsUserScope.User, "project", empty1.ToString(), "SelectedPlanId", 0, false);
          int num2 = service1.GetValue<int>(requestContext, SettingsUserScope.User, "project", empty1.ToString(), "SelectedSuiteId", 0, false);
          planId = num1;
          suiteId = 0;
          if (num2 > 0)
            suiteId = num2;
        }
        else
        {
          planId = selectedPlanId;
          suiteId = selectedSuiteId;
        }
        lastSelectedPivotId = !string.IsNullOrEmpty(str1) ? str1 : service1.GetValue<string>(requestContext, SettingsUserScope.User, "project", empty1.ToString(), "SelectedPivotId", "", false);
        string str2 = service1.GetValue<string>(requestContext, SettingsUserScope.User, "project", empty1.ToString(), "NewExecuteColumnOptions", "", false);
        if (!string.IsNullOrEmpty(str2))
        {
          second1 = str2.Split(',');
        }
        else
        {
          string str3 = service1.GetValue<string>(requestContext, SettingsUserScope.User, "project", empty1.ToString(), "ExecuteColumnOptions", "", false);
          if (!string.IsNullOrEmpty(str3))
            second1 = str3.Split(',');
          second1 = ((IEnumerable<string>) new string[3]
          {
            "System.Id",
            "TCM.TestPointConfigurationName",
            "TCM.TestPointTester"
          }).Concat<string>((IEnumerable<string>) second1).ToArray<string>();
          service1.SetValue(requestContext, SettingsUserScope.User, "project", empty1.ToString(), "NewExecuteColumnOptions", (object) string.Join(",", second1), false);
        }
        string str4 = service1.GetValue<string>(requestContext, SettingsUserScope.User, "project", empty1.ToString(), "NewDefineColumnOptions", "", false);
        if (!string.IsNullOrEmpty(str4))
        {
          second2 = str4.Split(',');
        }
        else
        {
          string str5 = service1.GetValue<string>(requestContext, SettingsUserScope.User, "project", empty1.ToString(), "DefineColumnOptions", "", false);
          if (!string.IsNullOrEmpty(str5))
            second2 = str5.Split(',');
          second2 = ((IEnumerable<string>) new string[3]
          {
            "System.Id",
            "System.AssignedTo",
            "System.State"
          }).Concat<string>((IEnumerable<string>) second2).ToArray<string>();
          service1.SetValue(requestContext, SettingsUserScope.User, "project", empty1.ToString(), "NewDefineColumnOptions", (object) string.Join(",", second2), false);
        }
      }
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      WorkItemTrackingFieldService service2 = requestContext.GetService<WorkItemTrackingFieldService>();
      ProcessFieldService service3 = requestContext.GetService<ProcessFieldService>();
      for (int index = 0; index < second2.Length; ++index)
      {
        FieldEntry field;
        service2.TryGetField(requestContext, second2[index], out field);
        if (field != null && !service3.IsSystemField(field.ReferenceName) && !service3.IsOOBField(requestContext, field.Name, field.ReferenceName))
          dictionary1[field.ReferenceName] = field.Name;
      }
      for (int index = 0; index < second1.Length; ++index)
      {
        FieldEntry field;
        service2.TryGetField(requestContext, second1[index], out field);
        if (field != null && !service3.IsSystemField(field.ReferenceName) && !service3.IsOOBField(requestContext, field.Name, field.ReferenceName))
          dictionary2[field.ReferenceName] = field.Name;
      }
      TfsTestManagementRequestContext requestContext1 = new TfsTestManagementRequestContext(requestContext);
      int skipRows;
      int topToFetch;
      int watermark;
      int topRemaining;
      Microsoft.TeamFoundation.TestManagement.Server.Utils.SetParametersForPaging(0, 50, (string) null, out skipRows, out topToFetch, out watermark, out topRemaining);
      ProjectInfo projectFromName = requestContext1.ProjectServiceHelper.GetProjectFromName(empty3);
      RevisedTestSuitesHelper testSuitesHelper = new RevisedTestSuitesHelper((TestManagementRequestContext) requestContext1);
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> testSuitesForPlan;
      using (PerformanceTimer.StartMeasure(requestContext, "TestPlansHubRefreshDataProvider.GetTestSuitesForPlan"))
      {
        try
        {
          testSuitesForPlan = testSuitesHelper.GetTestSuitesForPlan(projectFromName, planId, Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteExpand.Children, skipRows, topToFetch, watermark, true, includeDetailedPlan: true);
        }
        catch (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException ex)
        {
          return (object) new TestPlansHubRefreshData()
          {
            ErrorMessage = ex.Message
          };
        }
      }
      if (suiteId == 0 && testSuitesForPlan != null && testSuitesForPlan.Any<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>())
      {
        flag2 = false;
        suiteId = testSuitesForPlan[0].Id;
        if (testSuitesForPlan[0].Children != null && testSuitesForPlan[0].Children.Any<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>() && testSuitesForPlan[0].Children[0] != null)
          suiteId = testSuitesForPlan[0].Children[0].Id;
      }
      if (string.IsNullOrEmpty(lastSelectedPivotId))
        lastSelectedPivotId = "execute";
      TestPlanDetailedReference detailedReference1 = new TestPlanDetailedReference();
      detailedReference1.Id = planId;
      TestPlanDetailedReference detailedReference2 = detailedReference1;
      if (testSuitesForPlan != null && testSuitesForPlan.Any<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>())
        detailedReference2 = testSuitesForPlan[0].Plan as TestPlanDetailedReference;
      List<TestCase> testCases = (List<TestCase>) null;
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> testPoints = (List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>) null;
      string str6 = (string) null;
      string str7 = (string) null;
      IVssRegistryService service4 = requestContext.GetService<IVssRegistryService>();
      int num3 = service4.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestHubRefreshTestCasesPageSize", 500);
      int num4 = service4.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestHubRefreshTestPointsPageSize", 1000);
      if (this.ShouldLoadExecuteTabData(lastSelectedPivotId))
      {
        RevisedPointsHelper revisedPointsHelper = new RevisedPointsHelper((TestManagementRequestContext) requestContext1);
        Microsoft.TeamFoundation.TestManagement.Server.Utils.SetParametersForPointsPaging(0, 25, (string) null, out skipRows, out topToFetch, out watermark, out topRemaining);
        using (PerformanceTimer.StartMeasure(requestContext, "TestPlansHubRefreshDataProvider.GetNewPoints"))
        {
          try
          {
            testPoints = revisedPointsHelper.GetNewPoints(projectFromName, planId, suiteId, "", "", skipRows, topToFetch, watermark, true, false, excludeRunBy: true);
          }
          catch (Exception ex)
          {
            switch (ex)
            {
              case Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException _:
              case Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException _:
                if (flag2)
                  return (object) new TestPlansHubRefreshData()
                  {
                    ErrorMessage = ex.Message
                  };
                suiteId = this.GetDefaultValueForSelectedSuiteId(testSuitesForPlan);
                testPoints = revisedPointsHelper.GetNewPoints(projectFromName, planId, suiteId, "", "", skipRows, topToFetch, watermark, true, false, excludeRunBy: true);
                break;
              default:
                throw;
            }
          }
          if (testPoints != null)
          {
            if (testPoints.Count >= topToFetch)
            {
              if (testPoints[topToFetch - 1] != null)
              {
                str7 = Microsoft.TeamFoundation.TestManagement.Server.Utils.GenerateContinuationToken(testPoints[topToFetch - 1].Id, topRemaining);
                testPoints.RemoveAt(topToFetch - 1);
              }
            }
          }
        }
      }
      else if (this.ShouldLoadDefineTabData(lastSelectedPivotId))
      {
        Microsoft.TeamFoundation.TestManagement.Server.Utils.SetParametersForPaging(0, 25, (string) null, out skipRows, out topToFetch, out watermark, out topRemaining);
        using (PerformanceTimer.StartMeasure(requestContext, "TestPlansHubRefreshDataProvider.GetTestCaseList"))
        {
          try
          {
            TestSuiteDetailedReference detailedReference3 = new TestSuiteDetailedReference();
            detailedReference3.Id = suiteId;
            TestSuiteDetailedReference testSuiteDetailedReference = detailedReference3;
            testCases = testSuitesHelper.GetTestCaseList(projectFromName, (TestPlanReference) detailedReference2, testSuiteDetailedReference, watermark, skipRows, topToFetch, false, returnIdentityRef: true, excludeFlags: ExcludeFlags.PointAssignments | ExcludeFlags.ExtraInformation);
          }
          catch (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException ex)
          {
            return (object) new TestPlansHubRefreshData()
            {
              ErrorMessage = ex.Message
            };
          }
        }
        if (testCases != null && testCases.Count >= topToFetch && testCases[topToFetch - 1] != null)
        {
          str6 = Microsoft.TeamFoundation.TestManagement.Server.Utils.GenerateContinuationToken(Convert.ToInt32(testCases[topToFetch - 1].Order), topRemaining);
          testCases.RemoveAt(topToFetch - 1);
        }
      }
      using (PerformanceTimer.StartMeasure(requestContext, "TestPlansHubRefreshDataProvider.RemoveFieldsAndAdvancedLicense"))
        (testSuitesForPlan, testCases, testPoints) = this.RemoveUnnecessaryFields(testSuitesForPlan, testCases, testPoints);
      return (object) new TestPlansHubRefreshData()
      {
        TestPlan = detailedReference2,
        TestSuites = testSuitesForPlan,
        TestCases = testCases,
        TestPoints = testPoints,
        SelectedPivotId = lastSelectedPivotId,
        SelectedSuiteId = suiteId,
        TestCasesContinuationToken = str6,
        TestPointsContinuationToken = str7,
        IsAdvancedExtensionEnabled = flag1,
        TestCasePageSize = num3,
        TestPointPageSize = num4,
        ExecuteColumnOptionFields = second1,
        DefineColumnOptionFields = second2,
        DefineTabCustomColumnFieldMap = dictionary1,
        ExecuteTabCustomColumnFieldMap = dictionary2
      };
    }

    protected internal virtual int GetDefaultValueForSelectedSuiteId(
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> testSuitesDataTree)
    {
      int forSelectedSuiteId = 0;
      if (testSuitesDataTree != null && testSuitesDataTree.Any<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>())
      {
        forSelectedSuiteId = testSuitesDataTree[0].Id;
        if (testSuitesDataTree[0].Children != null && testSuitesDataTree[0].Children.Any<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>() && testSuitesDataTree[0].Children[0] != null)
          forSelectedSuiteId = testSuitesDataTree[0].Children[0].Id;
      }
      return forSelectedSuiteId;
    }

    protected internal virtual (Guid projectId, string projectName, Guid teamId, string pageUrl) GetProjectAndTeamData(
      IVssRequestContext requestContext)
    {
      return Utils.GetProjectAndTeamData(requestContext);
    }

    protected internal virtual bool ShouldLoadDefineTabData(string lastSelectedPivotId) => lastSelectedPivotId.Equals("define");

    protected internal virtual bool ShouldLoadExecuteTabData(string lastSelectedPivotId) => lastSelectedPivotId.Equals("execute");

    protected internal virtual TestManagementTestPlanLiteLoadSettings GetTestPlanLoadSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      return new TestManagementTestPlanLiteLoadSettings(requestContext, projectId, teamId);
    }

    protected internal virtual (ColumnSettingModel[] columns, List<TestPointGridDisplayColumn> columnOptionsData) GetColumnsData(
      IVssRequestContext requestContext)
    {
      ColumnSettingsHelper columnSettingsHelper = new ColumnSettingsHelper(requestContext);
      ColumnSettingModel[] defaultColumns = columnSettingsHelper.GetDefaultColumns();
      List<TestPointGridDisplayColumn> displayColumns = columnSettingsHelper.GetDisplayColumns((IList<ColumnSettingModel>) defaultColumns);
      return (defaultColumns, displayColumns);
    }

    protected internal virtual (List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> testSuites, List<TestCase> testCases, List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> testPoints) RemoveUnnecessaryFields(
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> testSuites,
      List<TestCase> testCases,
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint> testPoints)
    {
      if (testSuites != null && testSuites.Any<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>())
        testSuites = this.RemoveUnnecessaryFieldsFromSuite(testSuites);
      if (testCases != null && testCases.Any<TestCase>())
        testCases = testCases.Select<TestCase, TestCase>((Func<TestCase, TestCase>) (testCase =>
        {
          testCase.TestPlan = (TestPlanReference) null;
          testCase.Project = (TeamProjectReference) null;
          testCase.TestSuite = (TestSuiteReference) null;
          testCase.PointAssignments = (List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.PointAssignment>) null;
          testCase.links = (ReferenceLinks) null;
          return testCase;
        })).ToList<TestCase>();
      if (testPoints != null && testPoints.Any<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>())
        testPoints = testPoints.Select<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint, Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>((Func<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint, Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>) (testPoint =>
        {
          testPoint.TestPlan = (TestPlanReference) null;
          testPoint.Project = (TeamProjectReference) null;
          testPoint.TestSuite = (TestSuiteReference) null;
          testPoint.links = (ReferenceLinks) null;
          return testPoint;
        })).ToList<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint>();
      return (testSuites, testCases, testPoints);
    }

    protected internal virtual List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> RemoveUnnecessaryFieldsFromSuite(
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> testSuites)
    {
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite> source = testSuites;
      if (source == null)
        return (List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>) null;
      if (source.Any<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite>())
      {
        foreach (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite testSuite in source)
        {
          testSuite.Plan = (TestPlanReference) null;
          testSuite.Links = (ReferenceLinks) null;
          testSuite.LastUpdatedBy = (IdentityRef) null;
          testSuite.Children = this.RemoveUnnecessaryFieldsFromSuite(testSuite.Children);
        }
      }
      return source;
    }

    protected internal virtual (int selectedPlanId, int selectedSuiteId) ParseRequestAndGetPlanAndSuiteId(
      Uri url)
    {
      int result1 = 0;
      int result2 = 0;
      NameValueCollection queryString = HttpUtility.ParseQueryString(url.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped));
      if (int.TryParse(queryString["planId"], out result1) && result1 > 0)
        int.TryParse(queryString["suiteId"], out result2);
      return (result1, result2);
    }

    protected internal virtual ResultsStoreQuery GetConvertedTestPlanFilterQuery(
      IVssRequestContext requextContext,
      string projectName,
      string wiqlQuery,
      Guid projectId)
    {
      return string.IsNullOrEmpty(wiqlQuery) ? (ResultsStoreQuery) null : this.GetResultsStoreQuery(requextContext.GetService<IWitHelper>().ConvertWiqlQueryFromIdsToName(requextContext, projectId, wiqlQuery), projectName);
    }

    private ResultsStoreQuery GetResultsStoreQuery(string queryText, string projectName) => new ResultsStoreQuery()
    {
      TeamProjectName = projectName,
      TimeZone = TimeZoneInfo.Utc.ToSerializedString(),
      QueryText = queryText
    };

    protected internal bool IsAdvancedExtensionEnabled(IVssRequestContext requestContext) => LicenseCheckHelper.IsAdvancedTestExtensionEnabled(requestContext) || requestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationLicensingService>().IsFeatureSupported(requestContext, LicenseFeatures.TestManagementId);
  }
}
