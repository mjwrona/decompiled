// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestPlansLiteHubDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestPlansLiteHubDataProvider : IExtensionDataProvider
  {
    private const int DefaultPageSize = 1000;
    private const string c_selectedPlanIdKey = "SelectedPlanId";
    private const string c_settingScopeProject = "project";

    public string Name => "TestManagement.Provider.TestPlanLiteHub";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      WebPageDataProviderPageSource pageSource = WebPageDataProviderUtil.GetPageSource(requestContext);
      Guid id = pageSource.Project.Id;
      string name = pageSource.Project.Name;
      ContextIdentifier team = pageSource.Team;
      Guid guid = team != null ? team.Id : Guid.Empty;
      string url = pageSource.Url;
      int selectedPlanId = 0;
      int selectedSuiteId = 0;
      this.ParseRequestAndGetPlanAndSuiteIds(new Uri(url), ref selectedPlanId, ref selectedSuiteId);
      ColumnSettingsHelper columnSettingsHelper = new ColumnSettingsHelper(requestContext);
      ColumnSettingModel[] defaultColumns = columnSettingsHelper.GetDefaultColumns();
      string defaultTestPlanQuery = PlanDefaultQueryHelper.GetDefaultTestPlanQuery(requestContext, guid, id);
      TestManagementTestPlanLiteLoadSettings liteLoadSettings = new TestManagementTestPlanLiteLoadSettings(requestContext, id, guid);
      string wiqlQuery = !string.IsNullOrEmpty(liteLoadSettings.PlanFilter) ? liteLoadSettings.PlanFilter : defaultTestPlanQuery;
      ResultsStoreQuery testPlanFilterQuery = this.GetConvertedTestPlanFilterQuery(requestContext, name, wiqlQuery, id);
      List<TestPointGridDisplayColumn> displayColumns = columnSettingsHelper.GetDisplayColumns((IList<ColumnSettingModel>) defaultColumns);
      if (selectedPlanId < 0)
        return (object) new
        {
          columnOptions = displayColumns,
          columnSortOrder = ColumnSortOrderModel.Default,
          testPlanDefaultQuery = defaultTestPlanQuery
        };
      int lastSelectedPlanId = selectedPlanId == 0 ? liteLoadSettings.SelectedPlanId : selectedPlanId;
      int lastSelectedSuiteId = selectedSuiteId == 0 ? liteLoadSettings.SelectedSuiteId : selectedSuiteId;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestPlanHubData testPlanHubData = Microsoft.TeamFoundation.TestManagement.Server.TestPlan.GetTestPlanHubData(requestContext, id.ToString(), selectedPlanId, selectedSuiteId, liteLoadSettings.ConfigurationFilter, liteLoadSettings.TesterFilter, testPlanFilterQuery, lastSelectedPlanId, lastSelectedSuiteId);
      ISettingsService service = requestContext.GetService<ISettingsService>();
      if (lastSelectedPlanId > 0)
        service.SetValue(requestContext, SettingsUserScope.User, "project", id.ToString(), "SelectedPlanId", (object) lastSelectedPlanId, false);
      if (testPlanHubData != null && testPlanHubData.TestPoints != null)
      {
        testPlanHubData.TestPoints = this.GetFilteredTestPoints(testPlanHubData.TestPoints, liteLoadSettings.OutcomeFilter);
        testPlanHubData.TotalTestPoints = testPlanHubData.TestPoints.Count;
        if (testPlanHubData.TestPoints.Count > 1000)
          testPlanHubData.TestPoints = this.GetPaginatedValues(testPlanHubData.TestPoints);
      }
      if (this.IsAdvTestExtPermissionsProviderEnabled(requestContext))
        return (object) new
        {
          testPlanHubData = testPlanHubData,
          columnOptions = displayColumns,
          columnSortOrder = ColumnSortOrderModel.Default,
          testPlanDefaultQuery = defaultTestPlanQuery
        };
      bool flag = this.IsAdvancedExtensionEnabled(requestContext);
      return (object) new
      {
        testPlanHubData = testPlanHubData,
        columnOptions = displayColumns,
        columnSortOrder = ColumnSortOrderModel.Default,
        testPlanDefaultQuery = defaultTestPlanQuery,
        isAdvancedTestExtensionEnabled = flag
      };
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> GetPaginatedValues(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPoints)
    {
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> source = testPoints.Take<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>(1000);
      return source != null ? source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>() : testPoints;
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> GetFilteredTestPoints(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPoints,
      string outcome)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> collection = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>();
      if (string.IsNullOrEmpty(outcome))
      {
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint testPoint in testPoints)
          testPoint.Outcome = this.GetOutcomeText(testPoint);
        return testPoints;
      }
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint testPoint in testPoints)
      {
        string outcomeText = this.GetOutcomeText(testPoint);
        if (outcomeText.Equals(outcome))
        {
          testPoint.Outcome = outcomeText;
          collection.Add(testPoint);
        }
      }
      testPoints.Clear();
      testPoints.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint>) collection);
      return testPoints;
    }

    private string GetOutcomeText(Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint point) => TestHelperBase.GetTestPointOutcomeText((Microsoft.TeamFoundation.TestManagement.Client.TestPointState) Enum.Parse(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestPointState), point.State), (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) Enum.Parse(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), point.Outcome), (TestResultState) Enum.Parse(typeof (TestResultState), point.LastResultState));

    private void ParseRequestAndGetPlanAndSuiteIds(
      Uri url,
      ref int selectedPlanId,
      ref int selectedSuiteId)
    {
      NameValueCollection queryString = HttpUtility.ParseQueryString(url.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped));
      if (!int.TryParse(queryString["planId"], out selectedPlanId) || selectedPlanId <= 0)
        return;
      int.TryParse(queryString["suiteId"], out selectedSuiteId);
    }

    private ResultsStoreQuery GetConvertedTestPlanFilterQuery(
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

    private bool IsAdvancedExtensionEnabled(IVssRequestContext requestContext) => LicenseCheckHelper.IsAdvancedTestExtensionEnabled(requestContext);

    private bool IsAdvTestExtPermissionsProviderEnabled(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "WebAccess.TestManagement.AdvTestExtPermissionsProvider");
  }
}
