// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.SettingsUtilities
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  public class SettingsUtilities : ISettingsUtilities
  {
    public T GetTeamUserSettingValue<T>(
      IVssRequestContext context,
      WebApiTeam team,
      string settingName)
    {
      return this.GetTeamUserSettingValue<T>(context, team, settingName, default (T));
    }

    public T GetTeamUserSettingValue<T>(
      IVssRequestContext context,
      WebApiTeam team,
      string settingName,
      T defaultValue)
    {
      settingName = team.Id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture) + settingName;
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(context))
        return userSettingsHive.ReadSetting<T>("/" + settingName, defaultValue);
    }
  }
}
