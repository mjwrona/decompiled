// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UserPreferencesModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [Serializable]
  internal class UserPreferencesModel
  {
    public UserPreferencesModel()
    {
    }

    public UserPreferencesModel(UserPreferences userPreferences)
    {
      if (userPreferences.Culture != null)
        this.LCID = new int?(userPreferences.Culture.LCID);
      this.Theme = ThemesUtility.DefaultThemeName;
      this.TypeAheadDisabled = userPreferences.TypeAheadDisabled;
      this.WorkItemFormChromeBorder = userPreferences.WorkItemFormChromeBorder;
      this.TimeZoneId = userPreferences.TimeZone == null ? (string) null : userPreferences.TimeZone.Id;
      this.Calendar = userPreferences.Calendar;
      this.DatePattern = userPreferences.DatePattern;
      this.TimePattern = userPreferences.TimePattern;
    }

    public string CustomDisplayName { get; set; }

    public string PreferredEmail { get; set; }

    public bool IsEmailConfirmationPending { get; set; }

    public string Theme { get; set; }

    public bool TypeAheadDisabled { get; set; }

    public string TimeZoneId { get; set; }

    public int? LCID { get; set; }

    public string Calendar { get; set; }

    public string DatePattern { get; set; }

    public string TimePattern { get; set; }

    public bool ResetEmail { get; set; }

    public bool ResetDisplayName { get; set; }

    public bool WorkItemFormChromeBorder { get; set; }

    public UserPreferences GetUserPreferences(HttpContextBase httpContext)
    {
      UserPreferences userPreferences1 = new UserPreferences();
      userPreferences1.WebAccessTheme = ThemesUtility.IsThemeNameValid(ThemesUtility.DefaultThemeName, httpContext) ? ThemesUtility.DefaultThemeName : (string) null;
      userPreferences1.WorkItemFormChromeBorder = this.WorkItemFormChromeBorder;
      userPreferences1.TypeAheadDisabled = this.TypeAheadDisabled;
      if (!string.IsNullOrEmpty(this.TimeZoneId))
        userPreferences1.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId);
      int? lcid = this.LCID;
      if (lcid.HasValue)
      {
        UserPreferences userPreferences2 = userPreferences1;
        lcid = this.LCID;
        CultureInfo cultureInfo1 = CultureInfo.GetCultureInfo(lcid.Value);
        userPreferences2.Culture = cultureInfo1;
        UserPreferences userPreferences3 = userPreferences1;
        lcid = this.LCID;
        CultureInfo cultureInfo2 = CultureInfo.GetCultureInfo(lcid.Value);
        userPreferences3.Language = cultureInfo2;
        userPreferences1.DatePattern = this.DatePattern;
        userPreferences1.TimePattern = this.TimePattern;
        userPreferences1.Calendar = this.Calendar;
      }
      return userPreferences1;
    }
  }
}
