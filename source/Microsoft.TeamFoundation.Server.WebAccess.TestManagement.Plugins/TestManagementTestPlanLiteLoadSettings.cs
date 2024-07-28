// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestManagementTestPlanLiteLoadSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestManagementTestPlanLiteLoadSettings
  {
    private Guid project;
    private Guid team;
    private const string c_testManagementMoniker = "TestManagement";
    private const string c_testHubMoniker = "TestHub";
    private const string c_selectedTester = "SelectedTester";
    private const string c_selectedOutCome = "SelectedOutCome";
    private const string c_selectedConfiguration = "SelectedConfiguration";
    private const string c_selectedSuiteIdKey = "SelectedSuiteId";
    private const string c_selectedPlanIdKey = "SelectedPlanId";

    public string OutcomeFilter { get; }

    public Guid TesterFilter { get; }

    public int ConfigurationFilter { get; }

    public string PlanFilter { get; }

    public int SelectedSuiteId { get; }

    public int SelectedPlanId { get; }

    public TestManagementTestPlanLiteLoadSettings(
      IVssRequestContext requestContext,
      Guid project,
      Guid team)
    {
      this.project = project;
      this.team = team;
      using (TestManagementWebUserSettingsHive hive = new TestManagementWebUserSettingsHive(requestContext))
      {
        string defaultValue = "DefaultRegistrySettingValue";
        hive.Cache(this.GetCachePathPattern());
        this.OutcomeFilter = hive.ReadSetting<string>(this.ComposeKey("SelectedOutCome"), defaultValue);
        if (this.OutcomeFilter.Equals(defaultValue))
          this.SetDefaultValuesForRegistrySettings(hive);
        this.TesterFilter = hive.ReadSetting<Guid>(this.ComposeKey("SelectedTester"), new Guid());
        this.ConfigurationFilter = hive.ReadSetting<int>(this.ComposeKey("SelectedConfiguration"), -1);
        this.PlanFilter = hive.ReadSetting<string>(this.GetTestPlanFilterSettingKey(requestContext, project, team), string.Empty);
        this.SelectedSuiteId = hive.ReadSetting<int>(this.ComposeKey(nameof (SelectedSuiteId), true), 0);
        this.SelectedPlanId = hive.ReadSetting<int>(this.ComposeKey(nameof (SelectedPlanId), true), 0);
      }
    }

    private void SetDefaultValuesForRegistrySettings(TestManagementWebUserSettingsHive hive)
    {
      hive.WriteSetting<string>(this.ComposeKey("SelectedOutCome"), string.Empty);
      hive.WriteSetting<string>(this.ComposeKey("SelectedTester"), string.Empty);
      hive.WriteSetting<string>(this.ComposeKey("SelectedConfiguration"), string.Empty);
    }

    private string GetTestPlanFilterSettingKey(
      IVssRequestContext requextContext,
      Guid projectId,
      Guid teamId)
    {
      return this.GetBaseTestPlanFilterSettingKey(requextContext, projectId, teamId) + "/TestPlanSelection";
    }

    private string GetBaseTestPlanFilterSettingKey(
      IVssRequestContext requextContext,
      Guid projectId,
      Guid teamId)
    {
      Guid guid;
      string str1;
      if (!(teamId != Guid.Empty))
      {
        str1 = string.Empty;
      }
      else
      {
        guid = teamId;
        str1 = "/" + guid.ToString();
      }
      string str2 = str1;
      guid = projectId;
      return "/TestManagement/" + guid.ToString() + str2;
    }

    protected string ComposeKey(string property, bool includeTeam = false) => "TestManagement" + "/" + (this.project.ToString() + "/") + (!includeTeam || !(this.team != Guid.Empty) ? string.Empty : this.team.ToString() + "/") + "TestHub" + "/" + property;

    private string GetCachePathPattern() => "TestManagement" + "/" + (this.project.ToString() + "/") + "...";
  }
}
