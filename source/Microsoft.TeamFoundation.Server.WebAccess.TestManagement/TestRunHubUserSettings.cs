// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestRunHubUserSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.TestManagement.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestRunHubUserSettings : TestManagementUserSettings
  {
    private TestRunHubUserSettings.TestRunHubExploratorySessionSettings m_exploratorySessionSettings;
    private const string c_testHubMoniker = "TestRun";

    public TestRunHubUserSettings(
      TestManagerRequestContext context,
      IAccountUserSettingsHive userSetting)
      : base(context)
    {
      this.SetAccountUserSettingsHive(userSetting);
      this.m_exploratorySessionSettings = new TestRunHubUserSettings.TestRunHubExploratorySessionSettings(context, "TestRun", userSetting);
    }

    public TestRunHubUserSettings(TestManagerRequestContext context)
      : base(context)
    {
      this.m_exploratorySessionSettings = new TestRunHubUserSettings.TestRunHubExploratorySessionSettings(context, "TestRun");
    }

    public ExploratorySessionSettingModel GetExploratorySessionSettings()
    {
      using (PerfManager.Measure(this.TestManagerRequestContext.TfsRequestContext, "BusinessLayer", "TestRunUserSettings.GetExploratorySessionSettings"))
        return this.m_exploratorySessionSettings.GetSettings();
    }

    protected override string GetMoniker() => "TestRun";

    private class TestRunHubExploratorySessionSettings : TestManagementUserSettings
    {
      private ExploratorySessionSettingModel m_exploratorySessionSetting;
      private bool m_detailPaneStateSetting;
      private string m_teamFilterSetting;
      private string m_ownerFilterSetting;
      private string m_periodFilterSetting;
      private string m_queryFilterNameSetting;
      private string m_queryFilterValueSetting;
      private string m_groupBySettingSetting;
      private string m_filterBySettingSetting;
      private string m_monikor;
      private const string c_detailPaneStateSettingString = "DetailPaneState";
      private const string c_teamFilterSettingString = "TeamFilter";
      private const string c_ownerFilterSettingString = "OwnerFilter";
      private const string c_periodFilterSettingString = "PeriodFilter";
      private const string c_queryFilterNameSettingString = "QueryFilterName";
      private const string c_queryFilterValueSettingString = "QueryFilterValue";
      private const string c_groupBySettingSettingString = "GroupBySetting";
      private const string c_filterBySettingSettingString = "FilterBySetting";
      private const string c_ExploratorySessionMoniker = "ExploratorySession";
      private const bool c_defaultDetailPaneState = true;
      private readonly string c_defaultTeamFilter = string.Empty;
      private const string c_defaultOwnerFilter = "all";
      private const string c_defaultPeriodFilter = "7";
      private const string c_defaultQueryFilterName = "None";
      private const string c_defaultQueryFilterValue = "none";
      private const string c_defaultGroupBySetting = "group-by-explored-workitems";
      private const string c_defaultFilterBySetting = "filter-by-all";

      public TestRunHubExploratorySessionSettings(
        TestManagerRequestContext context,
        string monikor,
        IAccountUserSettingsHive accountSetting)
        : base(context)
      {
        this.SetAccountUserSettingsHive(accountSetting);
        this.m_monikor = monikor + "/ExploratorySession";
        this.InitializeSettings();
      }

      public TestRunHubExploratorySessionSettings(TestManagerRequestContext context, string monikor)
        : base(context)
      {
        this.m_monikor = monikor + "/ExploratorySession";
        this.InitializeSettings();
      }

      public ExploratorySessionSettingModel GetSettings()
      {
        using (new AccountUserSettingsHive(this.TestManagerRequestContext.TfsRequestContext))
          this.m_exploratorySessionSetting = new ExploratorySessionSettingModel()
          {
            DetailPaneState = this.m_detailPaneStateSetting,
            TeamFilter = this.m_teamFilterSetting,
            OwnerFilter = this.m_ownerFilterSetting,
            PeriodFilter = this.m_periodFilterSetting,
            QueryFilterName = this.m_queryFilterNameSetting,
            QueryFilterValue = this.m_queryFilterValueSetting,
            GroupBySetting = this.m_groupBySettingSetting,
            FilterBySetting = this.m_filterBySettingSetting
          };
        return this.m_exploratorySessionSetting;
      }

      protected override UserSettingScope GetScope() => UserSettingScope.Project;

      protected override string GetMoniker() => this.m_monikor;

      private void InitializeSettings()
      {
        using (IAccountUserSettingsHive userSettingsHive = this.GetAccountUserSettingsHive(this.TestManagerRequestContext.TfsRequestContext))
        {
          this.m_detailPaneStateSetting = userSettingsHive.ReadSetting<bool>("/" + this.ComposeKey("DetailPaneState"), true);
          this.m_ownerFilterSetting = userSettingsHive.ReadSetting<string>("/" + this.ComposeKey("OwnerFilter"), "all");
          this.m_teamFilterSetting = userSettingsHive.ReadSetting<string>("/" + this.ComposeKey("TeamFilter"), this.c_defaultTeamFilter);
          this.m_periodFilterSetting = userSettingsHive.ReadSetting<string>("/" + this.ComposeKey("PeriodFilter"), "7");
          this.m_queryFilterNameSetting = userSettingsHive.ReadSetting<string>("/" + this.ComposeKey("QueryFilterName"), "None");
          this.m_queryFilterValueSetting = userSettingsHive.ReadSetting<string>("/" + this.ComposeKey("QueryFilterValue"), "none");
          this.m_groupBySettingSetting = userSettingsHive.ReadSetting<string>("/" + this.ComposeKey("GroupBySetting"), "group-by-explored-workitems");
          this.m_filterBySettingSetting = userSettingsHive.ReadSetting<string>("/" + this.ComposeKey("FilterBySetting"), "filter-by-all");
        }
      }
    }
  }
}
