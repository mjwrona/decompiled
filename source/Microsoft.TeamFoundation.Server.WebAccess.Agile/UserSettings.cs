// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.UserSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UserSettings : IUserSettings
  {
    private IVssRequestContext m_requestContext;

    protected UserSettings()
    {
    }

    public UserSettings(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public T GetValue<T>(string settingName) => this.GetValue<T>(settingName, default (T));

    public virtual T GetValue<T>(string settingName, T defaultValue)
    {
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(this.m_requestContext))
        return userSettingsHive.ReadSetting<T>("/" + settingName, defaultValue);
    }

    public virtual void SetValue<T>(string settingName, T value)
    {
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(this.m_requestContext))
        userSettingsHive.WriteSetting<T>("/" + settingName, value);
    }
  }
}
