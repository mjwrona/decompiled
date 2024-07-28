// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagementController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", false)]
  [SupportedRouteArea(NavigationContextLevels.Project | NavigationContextLevels.Team)]
  public class TestManagementController : TestManagementAreaController
  {
    private TestPlansHelper m_testPlanHelper;
    private TestSuitesHelper m_testSuitesHelper;
    private TestRunHelper m_testRunHelper;
    private const string c_extAdUrl = "https://aka.ms/extensionadvertisement";
    private const string c_className = "tests-with-ads";
    private const string c_ownerFilterValue = "all";

    public TestManagementController() => this.m_executeContributedRequestHandlers = true;

    internal TestRunHelper TestRunHelper
    {
      get
      {
        if (this.m_testRunHelper == null)
          this.m_testRunHelper = new TestRunHelper(this.TestContext, (IAccountUserSettingsHive) new AccountUserSettingsHive(this.TestContext.TfsRequestContext));
        return this.m_testRunHelper;
      }
    }

    internal TestPlansHelper TestPlanHelper
    {
      get
      {
        if (this.m_testPlanHelper == null)
          this.m_testPlanHelper = new TestPlansHelper(this.TestContext);
        return this.m_testPlanHelper;
      }
    }

    internal TestSuitesHelper TestSuitesHelper
    {
      get
      {
        if (this.m_testSuitesHelper == null)
          this.m_testSuitesHelper = new TestSuitesHelper(this.TestContext);
        return this.m_testSuitesHelper;
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525100, 525200)]
    public ActionResult Index()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", nameof (Index), 10, true))
      {
        if (this.TestContext.TfsRequestContext.IsFeatureEnabled("WebAccess.TestManagement.TestPlanLiteHub"))
          return this.Plans();
        return this.IsTestHubNeedsToShowAdvertisements() ? (ActionResult) this.ShowAds() : (ActionResult) this.ShowTestHub();
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(525100, 525200)]
    public ActionResult Plans()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "Index", 10, true))
        return this.IsTestHubNeedsToShowAdvertisements() ? (ActionResult) this.ShowAds() : (ActionResult) this.ShowTestHubLite();
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult SharedParameters()
    {
      if (!TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext))
        return (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);
      return this.IsProjectConfiguredForSharedParameters() ? (ActionResult) this.ShowSharedParametersHub() : (ActionResult) this.View("Error", (object) this.GetTestManagementErrorModel());
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Configurations() => TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext) ? (ActionResult) this.ShowTestConfigurationsHub() : (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);

    [AcceptVerbs(HttpVerbs.Get)]
    [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
    public ActionResult Runs() => this.TestContext.TestRequestContext.SecurityManager.HasViewTestResultsPermission((TestManagementRequestContext) this.TestContext.TestRequestContext, Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName).String) ? (ActionResult) this.ShowRunsHub() : (ActionResult) new HttpUnauthorizedResult(WACommonResources.NotAuthorizedToAccessPage);

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult ResultsById()
    {
      string[] strArray = this.RouteData.GetRouteValue<string>("parameters", (string) null).Split('-');
      return strArray.Length == 2 ? (ActionResult) new ClientRedirectResult(ConfigurationExtensions.FragmentAction(this.Url, "index", (RouteValueDictionary) null, "tests", (object) new
      {
        planId = strArray[0],
        suiteId = strArray[1]
      })) : (ActionResult) this.ShowTestHub();
    }

    private ViewResult ShowTestHub()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestManagementController.ShowTestHub"))
      {
        this.ConfigureLeftHubSplitter(TestManagementServerResources.TestPlanExplorerSplitterLabel);
        this.PopulateTestPlanHubDataInJsonIsland();
        this.PopulateTestPlanQueriesInJsonIsland();
        this.PopulateWorkItemColorsInJsonIsland();
        return this.GetTestsView();
      }
    }

    private ViewResult ShowTestHubLite()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestManagementController.ShowTestHub"))
      {
        this.PopulateWorkItemColorsInJsonIsland();
        return this.View("TestsLite");
      }
    }

    private ViewResult GetTestsView()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestManagementController.GetTestsView"))
        return this.View("Tests");
    }

    private void PopulateTestPlanHubDataInJsonIsland()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestManagementController.PopulateTestPlanHubDataInJsonIsland"))
      {
        try
        {
          int selectedPlanId = 0;
          int selectedSuiteId = 0;
          this.ParseRequestAndGetPlanAndSuiteIds(ref selectedPlanId, ref selectedSuiteId);
          TestPlansWithSelectionModel testPlans = this.TestPlanHelper.FetchTestPlansIncludingSpecifiedPlan(selectedPlanId);
          if (testPlans.TestPlans.Count == 0)
            return;
          this.FindPlanAndSuiteIdToBeSelected(testPlans, ref selectedPlanId, ref selectedSuiteId);
          if (selectedPlanId <= 0)
            return;
          this.ViewData["TestPlans"] = (object) testPlans;
          bool disableShowPointCount = this.TestContext.TfsRequestContext.IsFeatureEnabled("WebAccess.TestManagement.PointCountFeature");
          TestSuitesQueryResultModel testSuitesForPlan = this.TestSuitesHelper.GetTestSuitesForPlan(selectedPlanId, disableShowPointCount);
          this.ViewData["TestSuitesForSelectedPlan"] = (object) testSuitesForPlan;
          if (this.DoesSuiteExist(testSuitesForPlan.TestSuites, selectedSuiteId))
            this.ViewData["TestPointsForSelectedSuite"] = (object) this.TestPlanHelper.GetFilteredTestPointsForSuite(selectedPlanId, selectedSuiteId, true, (TestPointGridDisplayColumn[]) null);
          this.ViewData["TeamFieldForLastSelectedTestPlan"] = (object) this.TestPlanHelper.GetTeamFieldForPlans(new List<int>()
          {
            selectedPlanId
          });
        }
        catch (Exception ex)
        {
          this.TestContext.TraceError("BusinessLayer", ex.ToString());
        }
      }
    }

    private void PopulateTestPlanQueriesInJsonIsland()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestManagementController.PopulateTestPlanQueriesInJsonIsland"))
      {
        try
        {
          string defaultTestPlanQuery = this.TestPlanHelper.GetDefaultTestPlanQuery();
          this.ViewData["DefaultPlanQuery"] = (object) defaultTestPlanQuery;
          this.ViewData["TestPlanQueryFromRegistry"] = (object) this.TestPlanHelper.GetConvertedFilteredTestPlanQueryFromRegistry(defaultTestPlanQuery);
        }
        catch (Exception ex)
        {
          this.TestContext.TraceError("BusinessLayer", ex.ToString());
        }
      }
    }

    private void PopulateWorkItemColorsInJsonIsland()
    {
      try
      {
        this.ViewData["workitemtype-colors"] = (object) this.TestPlanHelper.GetWorkItemColorModelList();
      }
      catch (Exception ex)
      {
        this.TestContext.TraceError("BusinessLayer", ex.ToString());
      }
    }

    private bool DoesSuiteExist(List<TestSuiteModel> testSuites, int selectedSuiteId) => testSuites.FirstOrDefault<TestSuiteModel>((Func<TestSuiteModel, bool>) (suite => suite.Id == selectedSuiteId)) != null;

    private void FindPlanAndSuiteIdToBeSelected(
      TestPlansWithSelectionModel testPlans,
      ref int selectedPlanId,
      ref int selectedSuiteId)
    {
      TestPlanModel testPlan = (TestPlanModel) null;
      if (selectedPlanId <= 0 || !this.TryGetTestPlan(testPlans.TestPlans, selectedPlanId, ref testPlan))
      {
        selectedPlanId = testPlans.SelectedTestPlan;
        selectedSuiteId = testPlans.SelectedTestSuite;
      }
      else
      {
        if (selectedSuiteId > 0)
          return;
        selectedSuiteId = testPlan.RootSuiteId;
      }
    }

    private bool TryGetTestPlan(
      List<TestPlanModel> testPlans,
      int selectedPlanId,
      ref TestPlanModel testPlan)
    {
      return (testPlan = testPlans.FirstOrDefault<TestPlanModel>((Func<TestPlanModel, bool>) (plan => plan.Id == selectedPlanId))) != null;
    }

    private void ParseRequestAndGetPlanAndSuiteIds(ref int selectedPlanId, ref int selectedSuiteId)
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.Url.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped));
      if (!int.TryParse(queryString["planId"], out selectedPlanId) || selectedPlanId <= 0)
        return;
      int.TryParse(queryString["suiteId"], out selectedSuiteId);
    }

    private ViewResult ShowAds()
    {
      string adPageUrl = this.getAdPageUrl();
      return this.View("HubPageAds", (object) new AdPageModel()
      {
        Url = adPageUrl,
        ClassName = "tests-with-ads"
      });
    }

    private string getAdPageUrl()
    {
      string location = StaticResources.Versioned.Content.GetLocation("TcmAdvPage.html", this.TestContext.TfsRequestContext);
      return !location.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !location.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? VirtualPathUtility.ToAbsolute(location) : location;
    }

    private ViewResult ShowSharedParametersHub() => this.View("DataSets");

    private ViewResult ShowRunsHub()
    {
      this.PopulateWorkItemMetaDataInJsonIsland();
      this.PopulateTestSessionsDataInJsonIsland();
      this.PopulateUserSettingDataInJsonIsland();
      return this.View("Runs");
    }

    private ViewResult ShowTestConfigurationsHub()
    {
      this.ConfigureLeftHubSplitter(TestManagementServerResources.TestConfigurationExplorerSplitterLabel);
      return this.View("Configurations");
    }

    private bool IsProjectConfiguredForSharedParameters() => this.TfsRequestContext.GetService<IWorkItemTypeCategoryService>().TryGetWorkItemTypeCategory(this.TfsRequestContext, this.TfsWebContext.Project.Id, "Microsoft.SharedParameterCategory", out WorkItemTypeCategory _);

    private TestManagementErrorModel GetTestManagementErrorModel()
    {
      TestManagementErrorModel managementErrorModel = new TestManagementErrorModel();
      if (!this.TfsWebContext.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        managementErrorModel.ErrorText = WACommonResources.FeatureEnablementSettings_Error_Missing;
      return managementErrorModel;
    }

    public bool IsBasicUserWithNoTestPlans() => this.IsBasicUser() && this.TestPlanHelper.GetAllTestPlans().TestPlans.Count <= 0;

    public bool IsTestHubNeedsToShowAdvertisements()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestManagementController.IsTestHubNeedsToShowAdvertisements"))
      {
        if (this.TfsWebContext.FeatureContext.IsFeatureInAdvertisingMode(LicenseFeatures.TestManagementForBasicUsersId))
          return true;
        if (!this.IsBasicUserWithNoTestPlans())
          return false;
        this.PublishTelemetryForShowingAdvertisementPage();
        return true;
      }
    }

    private bool IsBasicUser() => !TestManagementAreaRegistration.DoesUserHaveTestManagementAdvancedAccess(this.TfsWebContext);

    private void PublishTelemetryForShowingAdvertisementPage() => new Microsoft.TeamFoundation.TestManagement.Server.TelemetryLogger().PublishDataAsKeyValue(this.TfsRequestContext, "ShowTestPlanAdvertisement ", "ShowAdvertisementsPage", "1");

    private void PopulateWorkItemMetaDataInJsonIsland()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestManagementController.PopulateWorkItemMetaDataInJsonIsland"))
      {
        try
        {
          this.ViewData["WorkItemColors"] = (object) this.TestPlanHelper.GetWorkItemColorModelList();
          this.ViewData["WorkItemTypeCategories"] = (object) this.TestRunHelper.GetWorkItemTypeCategoryModelList();
        }
        catch (Exception ex)
        {
          this.TestContext.TraceError("BusinessLayer", ex.ToString());
        }
      }
    }

    private void PopulateTestSessionsDataInJsonIsland()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestManagementController.PopulateTestSessionsDataInJsonIsland"))
      {
        try
        {
          ExploratorySessionSettingModel sessionUserSettings = this.TestRunHelper.GetExploratorySessionUserSettings();
          int period = 7;
          bool flag = true;
          List<int> sources = new List<int>() { 3, 5 };
          WebApiTeam teamContext = this.GetTeamContext(sessionUserSettings);
          if (sessionUserSettings != null)
          {
            period = int.Parse(sessionUserSettings.PeriodFilter);
            flag = sessionUserSettings.OwnerFilter == "all";
          }
          this.ViewData["TestSessions"] = (object) this.TestRunHelper.GetTestSessionList(teamContext, period, (flag ? 1 : 0) != 0, true, sources, new List<int>()
          {
            4
          });
        }
        catch (Exception ex)
        {
          this.TestContext.TraceError("BusinessLayer", ex.ToString());
        }
      }
    }

    private WebApiTeam GetTeamContext(ExploratorySessionSettingModel userSettings)
    {
      if (this.TestContext.TfsWebContext.Team != null)
        return this.TestContext.TfsWebContext.Team;
      return userSettings != null && !string.IsNullOrEmpty(userSettings.TeamFilter) ? TestHelperBase.GetTeamObject(this.TestContext.TfsRequestContext, this.TfsWebContext.Project.Id, userSettings.TeamFilter) : TestHelperBase.GetDefaultTeam(this.TestContext.TfsRequestContext, this.TfsWebContext.Project.Id);
    }

    private void PopulateUserSettingDataInJsonIsland()
    {
      using (PerfManager.Measure(this.TestContext.TfsRequestContext, "BusinessLayer", "TestManagementController.PopulateUserSettingDataInJsonIsland"))
      {
        try
        {
          this.ViewData["ExploratorySessionUserSettings"] = (object) this.TestRunHelper.GetExploratorySessionUserSettings();
        }
        catch (Exception ex)
        {
          this.TestContext.TraceError("BusinessLayer", ex.ToString());
        }
      }
    }

    public static T InvokeAction<T>(Func<T> func)
    {
      try
      {
        return func();
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException != null)
          throw ex.InnerException;
        throw;
      }
    }
  }
}
