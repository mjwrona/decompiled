// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagementUserSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public abstract class TestManagementUserSettings
  {
    private IAccountUserSettingsHive accountSetting;
    private IWebUserSettingsHive webSetting;
    private const string c_testManagementMoniker = "TestManagement";

    public TestManagementUserSettings(TestManagerRequestContext context) => this.TestManagerRequestContext = context;

    public IWebUserSettingsHive GetWebUserSettingsHive(IVssRequestContext requestContext)
    {
      if (this.webSetting == null)
        this.webSetting = (IWebUserSettingsHive) new TestManagementWebUserSettingsHive(requestContext);
      return this.webSetting;
    }

    public void SetWebUserSettingsHive(IWebUserSettingsHive settingHive) => this.webSetting = settingHive;

    public IAccountUserSettingsHive GetAccountUserSettingsHive(IVssRequestContext requestContext)
    {
      if (this.accountSetting == null)
        this.accountSetting = (IAccountUserSettingsHive) new AccountUserSettingsHive(requestContext);
      return this.accountSetting;
    }

    public void SetAccountUserSettingsHive(IAccountUserSettingsHive settingHive) => this.accountSetting = settingHive;

    protected TestManagerRequestContext TestManagerRequestContext { get; private set; }

    protected string ComposeKey(string property)
    {
      string str = string.Empty;
      Guid guid;
      if (this.TestManagerRequestContext.Team != null && this.GetScope() == UserSettingScope.Team)
      {
        guid = this.TestManagerRequestContext.Team.Id;
        str = guid.ToString() + "/";
      }
      string[] strArray = new string[8];
      strArray[0] = "TestManagement";
      strArray[1] = "/";
      guid = this.TestManagerRequestContext.CurrentProjectGuid;
      strArray[2] = guid.ToString();
      strArray[3] = "/";
      strArray[4] = str;
      strArray[5] = this.GetMoniker();
      strArray[6] = "/";
      strArray[7] = property;
      return string.Concat(strArray);
    }

    protected T GetPropertyValue<T>(IWebUserSettingsHive hive, string property, T defaultValue) => hive.ReadSetting<T>(this.ComposeKey(property), defaultValue);

    protected void SetPropertyValue<T>(string property, T value)
    {
      using (IWebUserSettingsHive userSettingsHive = this.GetWebUserSettingsHive(this.TestManagerRequestContext.TfsRequestContext))
        userSettingsHive.WriteSetting<T>(this.ComposeKey(property), value);
    }

    protected virtual UserSettingScope GetScope() => UserSettingScope.Team;

    protected abstract string GetMoniker();
  }
}
