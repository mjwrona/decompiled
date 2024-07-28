// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.UserProfilePreferencesModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class UserProfilePreferencesModel
  {
    public UserProfilePreferencesModel()
    {
    }

    public UserProfilePreferencesModel(TfsWebContext webContext)
    {
      IVssRequestContext tfsRequestContext = webContext.TfsRequestContext;
      IVssRequestContext vssRequestContext = tfsRequestContext.To(TeamFoundationHostType.Deployment);
      IInstalledLanguageService service = vssRequestContext.GetService<IInstalledLanguageService>();
      this.AllTimeZones = new List<TimeZoneInfoModel>();
      TimeZoneInfoModel timeZoneInfoModel = new TimeZoneInfoModel(tfsRequestContext.GetCollectionTimeZone());
      timeZoneInfoModel.DisplayName = string.Format(WACommonResources.AccountTimeZone, (object) timeZoneInfoModel.DisplayName);
      this.AllTimeZones.Add(timeZoneInfoModel);
      this.AllTimeZones.AddRange((IEnumerable<TimeZoneInfoModel>) CommonUtility.AllTimeZones);
      this.AllCultures = new List<CultureInfoModel>();
      DateTime local = tfsRequestContext.GetTimeZone().ConvertToLocal(DateTime.UtcNow);
      foreach (int installedLanguage in (IEnumerable<int>) service.GetAllInstalledLanguages(vssRequestContext))
        this.AllCultures.Add(new CultureInfoModel(CultureInfo.GetCultureInfo(installedLanguage), local));
      CultureInfo preferredCulture;
      TeamFoundationApplicationCore.GetPreferredCulture(webContext.RequestContext.HttpContext.Request.UserLanguages, service.GetInstalledLanguages(vssRequestContext), service.GetUserInstalledLanguages(vssRequestContext), out preferredCulture);
      this.AllCultures.Insert(0, (CultureInfoModel) new DefaultCultureInfoModel(preferredCulture ?? CultureInfo.CurrentCulture, local));
      this.AllThemes = new List<ThemeModel>();
      ThemeModel[] themes = ThemesUtility.GetThemes(StaticResources.Versioned.Themes.GetPhysicalLocation(string.Empty), webContext.RequestContext.HttpContext);
      this.AllThemes.AddRange((IEnumerable<ThemeModel>) themes);
      this.SelectedTheme = themes[0].ThemeName;
      this.SelectedTimeZone = timeZoneInfoModel.Id;
      UserPreferences userPreferences = tfsRequestContext.GetService<IUserPreferencesService>().GetUserPreferences(tfsRequestContext);
      if (userPreferences.Culture != null)
        this.SelectedCulture = userPreferences.Culture.LCID;
      this.SelectedTheme = userPreferences.WebAccessTheme;
      this.TypeAheadDisabled = userPreferences.TypeAheadDisabled;
      this.WorkItemFormChromeBorder = userPreferences.WorkItemFormChromeBorder;
      this.SelectedTimeZone = userPreferences.TimeZone == null ? (string) null : userPreferences.TimeZone.Id;
      this.SelectedCalendar = userPreferences.Calendar;
      this.SelectedDateFormat = userPreferences.DatePattern;
      this.SelectedTimeFormat = userPreferences.TimePattern;
    }

    public List<ThemeModel> AllThemes { get; private set; }

    public List<CultureInfoModel> AllCultures { get; private set; }

    public List<TimeZoneInfoModel> AllTimeZones { get; private set; }

    public int SelectedCulture { get; set; }

    public string SelectedCalendar { get; set; }

    public string SelectedTimeZone { get; set; }

    public string SelectedTheme { get; set; }

    public bool TypeAheadDisabled { get; set; }

    public bool WorkItemFormChromeBorder { get; set; }

    public string SelectedDateFormat { get; set; }

    public string SelectedTimeFormat { get; set; }
  }
}
