// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestHubUserSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestHubUserSettings : TestManagementUserSettings
  {
    private const string c_selectedSuiteIdKey = "SelectedSuiteId";
    private const string c_selectedPlanIdKey = "SelectedPlanId";
    private const string c_testHubMoniker = "TestHub";
    private TestHubUserSettings.TestHubColumnSettings m_columnSettings;
    private TestHubUserSettings.TestHubFilterSettings m_filterSettings;
    private int? _selectedPlanId;
    private int? _selectedSuiteId;

    public TestHubUserSettings(TestManagerRequestContext context, IWebUserSettingsHive userSetting)
      : base(context)
    {
      this.SetWebUserSettingsHive(userSetting);
      this.m_columnSettings = new TestHubUserSettings.TestHubColumnSettings(context, "TestHub", userSetting);
      this.m_filterSettings = new TestHubUserSettings.TestHubFilterSettings(context, "TestHub", userSetting);
      this.InitializeTestHubUserSettings();
    }

    public TestHubUserSettings(TestManagerRequestContext context)
      : base(context)
    {
      this.m_columnSettings = new TestHubUserSettings.TestHubColumnSettings(context, "TestHub");
      this.m_filterSettings = new TestHubUserSettings.TestHubFilterSettings(context, "TestHub");
      this.InitializeTestHubUserSettings();
    }

    public int SelectedPlanId
    {
      get => this._selectedPlanId.GetValueOrDefault();
      private set
      {
        this._selectedPlanId = new int?(value);
        this.SetPropertyValue<int>(nameof (SelectedPlanId), value);
      }
    }

    public int SelectedSuiteId
    {
      get => this._selectedSuiteId.GetValueOrDefault();
      private set
      {
        this._selectedSuiteId = new int?(value);
        this.SetPropertyValue<int>(nameof (SelectedSuiteId), value);
      }
    }

    public string SelectedOutCome => this.m_filterSettings.SelectedOutCome;

    public string SelectedTester
    {
      get
      {
        using (PerfManager.Measure(this.TestManagerRequestContext.TfsRequestContext, "BusinessLayer", "TestHubUserSettings.GetSelectedTester"))
          return this.m_filterSettings.SelectedTester;
      }
    }

    public int? SelectedConfiguration
    {
      get
      {
        using (PerfManager.Measure(this.TestManagerRequestContext.TfsRequestContext, "BusinessLayer", "TestHubUserSettings.GetSelectedConfiguration"))
        {
          int result;
          return int.TryParse(this.m_filterSettings.SelectedConfiguration, out result) ? new int?(result) : new int?();
        }
      }
    }

    public void UpdateColumnOptions(IList<ColumnSettingModel> columnSettings, bool removeExisting)
    {
      using (PerfManager.Measure(this.TestManagerRequestContext.TfsRequestContext, "BusinessLayer", "TestHubUserSettings.UpdateColumnOptions"))
        this.m_columnSettings.UpdateColumnOptions(columnSettings, removeExisting);
    }

    public IList<ColumnSettingModel> GetColumnOptions()
    {
      using (PerfManager.Measure(this.TestManagerRequestContext.TfsRequestContext, "BusinessLayer", "TestHubUserSettings.GetColumnOptions"))
        return this.m_columnSettings.GetColumnOptions();
    }

    internal void UpdateSortOrder(ColumnSortOrderModel columnSortOrder) => this.m_columnSettings.UpdateSortOrder(columnSortOrder);

    internal ColumnSortOrderModel GetSortOrder()
    {
      using (PerfManager.Measure(this.TestManagerRequestContext.TfsRequestContext, "BusinessLayer", "TestHubUserSettings.GetSortOrder"))
        return this.m_columnSettings.GetSortOrder();
    }

    public void SetSelectedPlanAndSuite(int selectedPlanId, int selectedSuiteId)
    {
      this.SelectedPlanId = selectedPlanId;
      this.SelectedSuiteId = selectedSuiteId;
    }

    public void SetSelectedPlanAndSuiteIfNotSame(int planId)
    {
      if (this.SelectedPlanId == planId)
        return;
      this.SetSelectedPlanAndSuite(planId, 0);
    }

    internal void UpdateSelectedFilterSettings(
      string selectedTester,
      string selectedOutcome,
      int? selectedConfiguration)
    {
      this.m_filterSettings.UpdateSelectedFilterSettings(selectedTester, selectedOutcome, selectedConfiguration);
    }

    protected override string GetMoniker() => "TestHub";

    private void InitializeTestHubUserSettings()
    {
      using (PerfManager.Measure(this.TestManagerRequestContext.TfsRequestContext, "BusinessLayer", "TestHubUserSettings.InitializeTestHubUserSettings"))
      {
        using (IWebUserSettingsHive userSettingsHive = this.GetWebUserSettingsHive(this.TestManagerRequestContext.TfsRequestContext))
        {
          userSettingsHive.Cache(this.ComposeKey("*"));
          this._selectedPlanId = new int?(this.GetPropertyValue<int>(userSettingsHive, "SelectedPlanId", 0));
          this._selectedSuiteId = new int?(this.GetPropertyValue<int>(userSettingsHive, "SelectedSuiteId", 0));
        }
      }
    }

    private class TestHubColumnSettings : TestManagementUserSettings
    {
      private IList<ColumnSettingModel> c_columnSetting;
      private ColumnSortOrderModel c_columnSortOrderSetting;
      private const string c_columnSettingsString = "ColumnSettings";
      private const string c_columnSortOrderSettingsString = "ColumnSortOrder";
      private string m_monikor;

      public TestHubColumnSettings(
        TestManagerRequestContext context,
        string monikor,
        IWebUserSettingsHive webSetting)
        : base(context)
      {
        this.SetWebUserSettingsHive(webSetting);
        this.m_monikor = monikor;
        this.InitializeColumnSettings();
      }

      public TestHubColumnSettings(TestManagerRequestContext context, string monikor)
        : base(context)
      {
        this.m_monikor = monikor;
        this.InitializeColumnSettings();
      }

      public void UpdateColumnOptions(IList<ColumnSettingModel> columnSettings, bool removeExisting)
      {
        if (!removeExisting)
        {
          columnSettings = this.UpdateCurrentColumnOptions(columnSettings);
          this.c_columnSetting.AddRange<ColumnSettingModel, IList<ColumnSettingModel>>((IEnumerable<ColumnSettingModel>) columnSettings);
        }
        if (columnSettings.Count > 0)
        {
          this.SetPropertyValue<string>("ColumnSettings", string.Join("&", columnSettings.Select<ColumnSettingModel, string>((Func<ColumnSettingModel, string>) (setting => setting.ToString()))));
          this.c_columnSetting = columnSettings;
        }
        else
        {
          this.SetPropertyValue<string>("ColumnSettings", string.Empty);
          this.c_columnSetting = (IList<ColumnSettingModel>) new List<ColumnSettingModel>();
        }
      }

      public IList<ColumnSettingModel> GetColumnOptions() => this.c_columnSetting;

      internal void UpdateSortOrder(ColumnSortOrderModel columnSortOrder)
      {
        this.SetPropertyValue<string>("ColumnSortOrder", columnSortOrder.ToString());
        this.c_columnSortOrderSetting = columnSortOrder;
      }

      internal ColumnSortOrderModel GetSortOrder() => this.c_columnSortOrderSetting;

      protected override UserSettingScope GetScope() => UserSettingScope.Project;

      protected override string GetMoniker() => this.m_monikor;

      private void InitializeColumnSettings()
      {
        using (IWebUserSettingsHive userSettingsHive = this.GetWebUserSettingsHive(this.TestManagerRequestContext.TfsRequestContext))
        {
          userSettingsHive.Cache(this.ComposeKey("*"));
          string propertyValue1 = this.GetPropertyValue<string>(userSettingsHive, "ColumnSettings", string.Empty);
          string propertyValue2 = this.GetPropertyValue<string>(userSettingsHive, "ColumnSortOrder", string.Empty);
          this.c_columnSetting = this.GetColumnSettingModel(propertyValue1);
          this.c_columnSortOrderSetting = this.GetColumnSortOrderModel(propertyValue2);
        }
      }

      private IList<ColumnSettingModel> GetColumnSettingModel(string columnSetting)
      {
        if (string.IsNullOrEmpty(columnSetting))
          return (IList<ColumnSettingModel>) new List<ColumnSettingModel>();
        string[] source = columnSetting.Split('&');
        return source.Length != 0 ? (IList<ColumnSettingModel>) new List<ColumnSettingModel>(((IEnumerable<string>) source).Select<string, ColumnSettingModel>((Func<string, ColumnSettingModel>) (settingString => ColumnSettingModel.FromString(settingString)))) : (IList<ColumnSettingModel>) new List<ColumnSettingModel>();
      }

      private ColumnSortOrderModel GetColumnSortOrderModel(string columnSortOrderSetting) => ColumnSortOrderModel.FromString(columnSortOrderSetting);

      private IList<ColumnSettingModel> UpdateCurrentColumnOptions(
        IList<ColumnSettingModel> columnSettings)
      {
        IDictionary<string, ColumnSettingModel> dictionary = (IDictionary<string, ColumnSettingModel>) new Dictionary<string, ColumnSettingModel>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (ColumnSettingModel columnOption in (IEnumerable<ColumnSettingModel>) this.GetColumnOptions())
        {
          if (!string.IsNullOrWhiteSpace(columnOption.RefName) && !dictionary.ContainsKey(columnOption.RefName))
            dictionary[columnOption.RefName] = columnOption;
        }
        foreach (ColumnSettingModel columnSetting in (IEnumerable<ColumnSettingModel>) columnSettings)
        {
          if (!string.IsNullOrWhiteSpace(columnSetting.RefName) && dictionary.ContainsKey(columnSetting.RefName))
            dictionary[columnSetting.RefName] = columnSetting;
        }
        return (IList<ColumnSettingModel>) dictionary.Values.ToList<ColumnSettingModel>();
      }
    }

    private class TestHubFilterSettings : TestManagementUserSettings
    {
      private const string c_selectedTester = "SelectedTester";
      private const string c_selectedOutCome = "SelectedOutCome";
      private const string c_selectedConfiguration = "SelectedConfiguration";
      private string m_monikor;
      private string _selectedOutcome;
      private string _selectedConfiguration;
      private string _selectedTester;

      public TestHubFilterSettings(
        TestManagerRequestContext context,
        string monikor,
        IWebUserSettingsHive userSetting)
        : base(context)
      {
        this.SetWebUserSettingsHive(userSetting);
        this.m_monikor = monikor;
        this.InitializeFilterSettings();
      }

      public TestHubFilterSettings(TestManagerRequestContext context, string monikor)
        : base(context)
      {
        this.m_monikor = monikor;
        this.InitializeFilterSettings();
      }

      protected override UserSettingScope GetScope() => UserSettingScope.Project;

      public string SelectedOutCome
      {
        get => this._selectedOutcome;
        private set
        {
          this._selectedOutcome = value;
          this.SetPropertyValue<string>(nameof (SelectedOutCome), value);
        }
      }

      public string SelectedTester
      {
        get => this._selectedTester;
        private set
        {
          this._selectedTester = value;
          this.SetPropertyValue<string>(nameof (SelectedTester), value);
        }
      }

      public string SelectedConfiguration
      {
        get => this._selectedConfiguration;
        private set
        {
          this._selectedConfiguration = value;
          this.SetPropertyValue<string>(nameof (SelectedConfiguration), value);
        }
      }

      internal void UpdateSelectedFilterSettings(
        string selectedTester,
        string selectedOutcome,
        int? selectedConfiguration)
      {
        this.SelectedOutCome = selectedOutcome != null ? selectedOutcome : string.Empty;
        this.SelectedTester = selectedTester != null ? selectedTester : string.Empty;
        this.SelectedConfiguration = selectedConfiguration.HasValue ? selectedConfiguration.ToString() : string.Empty;
      }

      protected override string GetMoniker() => this.m_monikor;

      private void InitializeFilterSettings()
      {
        using (IWebUserSettingsHive userSettingsHive = this.GetWebUserSettingsHive(this.TestManagerRequestContext.TfsRequestContext))
        {
          userSettingsHive.Cache(this.ComposeKey("*"));
          this._selectedConfiguration = this.GetPropertyValue<string>(userSettingsHive, "SelectedConfiguration", string.Empty);
          this._selectedTester = this.GetPropertyValue<string>(userSettingsHive, "SelectedTester", string.Empty);
          this._selectedOutcome = this.GetPropertyValue<string>(userSettingsHive, "SelectedOutCome", string.Empty);
        }
      }
    }
  }
}
