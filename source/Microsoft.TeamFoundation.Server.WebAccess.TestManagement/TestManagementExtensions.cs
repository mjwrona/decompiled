// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagementExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public static class TestManagementExtensions
  {
    private const string AllSessionsIdentifier = "all";
    private const string MySessionsIdentifier = "my";
    private const string Number7 = "7";
    private const string Number14 = "14";
    private const string Number30 = "30";
    private const string Number60 = "60";
    private const string Number90 = "90";
    private const string NoneIdentifier = "none";
    private const string SelectQueryIdentifier = "select-query";

    public static MvcHtmlString TestPointOutcomeFilter(this HtmlHelper htmlHelper) => TestManagementExtensions.CreateFilter(htmlHelper, TestManagementServerResources.TestPointOutcomeFilter, "testpoint-outcome-filter");

    public static MvcHtmlString TesterFilter(this HtmlHelper htmlHelper) => TestManagementExtensions.CreateFilter(htmlHelper, TestManagementResources.TesterFilter, "tester-filter");

    public static MvcHtmlString ConfigurationFilter(this HtmlHelper htmlHelper) => TestManagementExtensions.CreateFilter(htmlHelper, TestManagementServerResources.ConfigurationFilter, "configuration-filter");

    public static MvcHtmlString SessionOwnerFilter(this HtmlHelper htmlHelper)
    {
      List<PivotFilterItem> filterItems = new List<PivotFilterItem>();
      PivotFilterItem pivotFilterItem1 = new PivotFilterItem(TestManagementServerResources.AllSessionsText, (object) "all");
      pivotFilterItem1.Selected = false;
      PivotFilterItem pivotFilterItem2 = new PivotFilterItem(TestManagementServerResources.MySessionsText, (object) "my");
      pivotFilterItem2.Selected = false;
      filterItems.Add(pivotFilterItem1);
      filterItems.Add(pivotFilterItem2);
      return TestManagementExtensions.CreateInvisibleFilter(htmlHelper, TestManagementServerResources.ViewFilterText, filterItems, "session-owner-filter");
    }

    public static MvcHtmlString SessionPeriodFilter(this HtmlHelper htmlHelper)
    {
      List<PivotFilterItem> filterItems = new List<PivotFilterItem>();
      PivotFilterItem pivotFilterItem1 = new PivotFilterItem(TestManagementServerResources.Last90DaysText, (object) "90");
      pivotFilterItem1.Selected = false;
      PivotFilterItem pivotFilterItem2 = new PivotFilterItem(TestManagementServerResources.Last60DaysText, (object) "60");
      pivotFilterItem2.Selected = false;
      PivotFilterItem pivotFilterItem3 = new PivotFilterItem(TestManagementServerResources.Last30DaysText, (object) "30");
      pivotFilterItem3.Selected = false;
      PivotFilterItem pivotFilterItem4 = new PivotFilterItem(TestManagementServerResources.Last14DaysText, (object) "14");
      pivotFilterItem4.Selected = false;
      PivotFilterItem pivotFilterItem5 = new PivotFilterItem(TestManagementServerResources.Last7DaysText, (object) "7");
      pivotFilterItem5.Selected = false;
      filterItems.Add(pivotFilterItem1);
      filterItems.Add(pivotFilterItem2);
      filterItems.Add(pivotFilterItem3);
      filterItems.Add(pivotFilterItem4);
      filterItems.Add(pivotFilterItem5);
      return TestManagementExtensions.CreateInvisibleFilter(htmlHelper, TestManagementServerResources.PeriodText, filterItems, "session-period-filter");
    }

    public static MvcHtmlString QuerySelectorFilter(this HtmlHelper htmlHelper)
    {
      List<PivotFilterItem> filterItems = new List<PivotFilterItem>();
      PivotFilterItem pivotFilterItem1 = new PivotFilterItem(TestManagementServerResources.NoneText, (object) "none");
      pivotFilterItem1.Selected = false;
      PivotFilterItem pivotFilterItem2 = new PivotFilterItem(TestManagementServerResources.SelectQueryText, (object) "select-query");
      pivotFilterItem2.Selected = false;
      filterItems.Add(pivotFilterItem1);
      filterItems.Add(pivotFilterItem2);
      return TestManagementExtensions.CreateInvisibleFilter(htmlHelper, TestManagementServerResources.QueryText, filterItems, "query-selector-filter");
    }

    public static MvcHtmlString PaneFilter(this HtmlHelper htmlHelper, TfsWebContext tfsWebContext)
    {
      using (PerfManager.Measure(tfsWebContext.TfsRequestContext, "BusinessLayer", "TestManagementExtensions.PaneFilter"))
      {
        PivotFilterItem pivotFilterItem1 = new PivotFilterItem(TestManagementServerResources.PaneFilter_TestCase, (object) "testCasePane");
        pivotFilterItem1.Title = TestManagementServerResources.TestCaseToolTip;
        PivotFilterItem pivotFilterItem2 = new PivotFilterItem(TestManagementServerResources.PaneFilter_TestSuite, (object) "referencedSuites");
        pivotFilterItem2.Title = TestManagementServerResources.TestSuitesToolTip;
        PivotFilterItem pivotFilterItem3 = new PivotFilterItem(TestManagementServerResources.PaneFilter_TestResults, (object) "resultsPane");
        pivotFilterItem3.Title = TestManagementServerResources.TestResultsToolTip;
        string a;
        using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(tfsWebContext.TfsRequestContext))
          a = userSettingsHive.ReadSetting<string>("/PaneMode", "testCasePane");
        if (string.Equals(a, "referencedSuites", StringComparison.OrdinalIgnoreCase))
          pivotFilterItem2.Selected = true;
        else if (string.Equals(a, "resultsPane", StringComparison.OrdinalIgnoreCase))
          pivotFilterItem3.Selected = true;
        else
          pivotFilterItem1.Selected = true;
        return TestManagementExtensions.CreateFilter(htmlHelper, TestManagementServerResources.PaneFilter, new List<PivotFilterItem>()
        {
          pivotFilterItem1,
          pivotFilterItem2,
          pivotFilterItem3
        }, "pane-filter");
      }
    }

    public static MvcHtmlString PositionFilter(
      this HtmlHelper htmlHelper,
      TfsWebContext tfsWebContext,
      ViewDataDictionary viewData)
    {
      using (PerfManager.Measure(tfsWebContext.TfsRequestContext, "BusinessLayer", "TestManagementExtensions.PositionFilter"))
      {
        string a;
        using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(tfsWebContext.TfsRequestContext))
          a = userSettingsHive.ReadSetting<string>("/PanePosition", "off");
        PivotFilterItem pivotFilterItem1 = new PivotFilterItem(TestManagementServerResources.TestCasePaneFilter_Bottom, (object) "bottom");
        PivotFilterItem pivotFilterItem2 = new PivotFilterItem(TestManagementServerResources.TestCasePaneFilter_Right, (object) "right");
        PivotFilterItem pivotFilterItem3 = new PivotFilterItem(TestManagementServerResources.TestCasePaneFilter_Off, (object) "off");
        string str = "vertical";
        if (string.Equals(a, "off", StringComparison.OrdinalIgnoreCase))
        {
          pivotFilterItem3.Selected = true;
          str += " no-split";
        }
        else if (string.Equals(a, "right", StringComparison.OrdinalIgnoreCase))
        {
          pivotFilterItem2.Selected = true;
          str = "horizontal";
        }
        else
          pivotFilterItem1.Selected = true;
        viewData["splitterClass"] = (object) str;
        viewData["panePosition"] = (object) a;
        return htmlHelper.PivotFilter(TestManagementServerResources.PositionFilter, (IEnumerable<PivotFilterItem>) new PivotFilterItem[2]
        {
          pivotFilterItem1,
          pivotFilterItem2
        }, (object) new{ @class = "work-items-pane-filter" });
      }
    }

    public static bool IsSharedParametersTestCasePaneFilterOn(
      this HtmlHelper htmlHelper,
      TfsWebContext tfsWebContext)
    {
      string a;
      using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(tfsWebContext.TfsRequestContext))
        a = userSettingsHive.ReadSetting<string>("/SharedParameterReferencedTestCasesPane", "off");
      return !string.Equals(a, "off", StringComparison.OrdinalIgnoreCase) && string.Equals(a, "on", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetLastSelectedSharedParameterId(
      this HtmlHelper htmlHelper,
      TfsWebContext tfsWebContext)
    {
      using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(tfsWebContext.TfsRequestContext))
        return userSettingsHive.ReadSetting<string>("/SelectedSharedParameterId", "0");
    }

    public static bool IsConfigurationVariablesPaneFilterOn(
      this HtmlHelper htmlHelper,
      TfsWebContext tfsWebContext)
    {
      string a;
      using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(tfsWebContext.TfsRequestContext))
        a = userSettingsHive.ReadSetting<string>("/ConfigurationVariablesPaneStatus", "off");
      return !string.Equals(a, "off", StringComparison.OrdinalIgnoreCase) && string.Equals(a, "on", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetLastSelectedTestConfigurationId(
      this HtmlHelper htmlHelper,
      TfsWebContext tfsWebContext)
    {
      using (PerfManager.Measure(tfsWebContext.TfsRequestContext, "BusinessLayer", "TestManagementExtensions.GetLastSelectedTestConfigurationId"))
      {
        string testConfigurationId;
        using (AccountUserSettingsHive userSettingsHive = new AccountUserSettingsHive(tfsWebContext.TfsRequestContext))
          testConfigurationId = userSettingsHive.ReadSetting<string>("/SelectedTestConfigurationId", "0");
        return testConfigurationId;
      }
    }

    public static MvcHtmlString ViewFilter(this HtmlHelper htmlHelper, TfsWebContext tfsWebContext)
    {
      using (PerfManager.Measure(tfsWebContext.TfsRequestContext, "BusinessLayer", "TestManagementExtensions.ViewFilter"))
      {
        PivotFilterItem pivotFilterItem1 = new PivotFilterItem(TestManagementServerResources.ListText, (object) "list");
        PivotFilterItem pivotFilterItem2 = new PivotFilterItem(TestManagementServerResources.GridText, (object) "grid");
        pivotFilterItem1.Selected = true;
        return htmlHelper.PivotFilter(TestManagementServerResources.ViewFilterText, (IEnumerable<PivotFilterItem>) new PivotFilterItem[2]
        {
          pivotFilterItem1,
          pivotFilterItem2
        }, (object) new{ @class = "view-filter" });
      }
    }

    public static MvcHtmlString TestTabItems(this HtmlHelper htmlHelper, UrlHelper urlHelper)
    {
      MvcHtmlString mvcHtmlString = new MvcHtmlString(string.Empty);
      return htmlHelper.PivotViews((IEnumerable<PivotView>) new PivotView[2]
      {
        new PivotView(TestManagementServerResources.TestHubTestsTabTitle)
        {
          Id = "tests",
          Link = urlHelper.FragmentAction("tests", (object) "my")
        },
        new PivotView(TestManagementServerResources.TestHubChartsTabTitle)
        {
          Id = "charts",
          Link = urlHelper.FragmentAction("charts", (object) "my")
        }
      }, (object) new{ @class = "test-items-tabs" });
    }

    public static MvcHtmlString IsAdvancedExtensionEnabled(
      this HtmlHelper htmlHelper,
      TfsWebContext tfsWebContext)
    {
      using (PerfManager.Measure(tfsWebContext.TfsRequestContext, "BusinessLayer", "TestManagementExtensions.IsAdvancedExtensionEnabled"))
      {
        bool flag = LicenseCheckHelper.IsAdvancedTestExtensionEnabled(tfsWebContext.TfsRequestContext);
        JsObject data = new JsObject();
        data["advancedTestExtensionEnabled"] = (object) flag;
        return htmlHelper.JsonIsland((object) data, (object) new
        {
          @class = "__advancedTestExtensionEnabled"
        });
      }
    }

    private static MvcHtmlString CreateFilter(
      HtmlHelper htmlHelper,
      string filterName,
      string filterClass)
    {
      PivotFilter dropdownPivotFilter = TestManagementExtensions.CreateDropdownPivotFilter(filterName, (IEnumerable<PivotFilterItem>) new List<PivotFilterItem>());
      return htmlHelper.PivotFilter(dropdownPivotFilter, (object) new
      {
        @class = filterClass
      });
    }

    private static MvcHtmlString CreateFilter(
      HtmlHelper htmlHelper,
      string filterName,
      List<PivotFilterItem> filterItems,
      string filterClass)
    {
      PivotFilter dropdownPivotFilter = TestManagementExtensions.CreateDropdownPivotFilter(filterName, (IEnumerable<PivotFilterItem>) filterItems);
      return htmlHelper.PivotFilter(dropdownPivotFilter, (object) new
      {
        @class = filterClass
      });
    }

    private static MvcHtmlString CreateInvisibleFilter(
      HtmlHelper htmlHelper,
      string filterName,
      List<PivotFilterItem> filterItems,
      string filterClass)
    {
      PivotFilter dropdownPivotFilter = TestManagementExtensions.CreateDropdownPivotFilter(filterName, (IEnumerable<PivotFilterItem>) filterItems);
      return htmlHelper.PivotFilter(dropdownPivotFilter, (object) new
      {
        @class = filterClass,
        style = "display: none"
      });
    }

    private static PivotFilter CreateDropdownPivotFilter(
      string name,
      IEnumerable<PivotFilterItem> items)
    {
      return new PivotFilter(name, items)
      {
        Behavior = PivotFilterBehavior.Dropdown
      };
    }
  }
}
