// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.TeamUserSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamUserSettings : UserSettings
  {
    private string m_teamPrefix;

    protected TeamUserSettings()
    {
    }

    public TeamUserSettings(IVssRequestContext requestContext, WebApiTeam team)
      : base(requestContext)
    {
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      this.m_teamPrefix = team.Id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public override T GetValue<T>(string settingName, T defaultValue) => base.GetValue<T>(this.m_teamPrefix + settingName, defaultValue);

    public override void SetValue<T>(string settingName, T value) => base.SetValue<T>(this.m_teamPrefix + settingName, value);
  }
}
