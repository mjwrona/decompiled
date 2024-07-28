// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.Profile.Builders.ProfilePreferencesBuilder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.Web.Profile.Builders
{
  public class ProfilePreferencesBuilder
  {
    public static ProfilePreferencesModel GetAvailablePreferences(
      IVssRequestContext requestContext,
      HttpContextBase httpContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IInstalledLanguageService service = vssRequestContext.GetService<IInstalledLanguageService>();
      DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, requestContext.GetTimeZone());
      IEnumerable<int> installedLanguages = (IEnumerable<int>) service.GetAllInstalledLanguages(vssRequestContext);
      ProfilePreferencesBuilder.Pattern dateTimePatterns = ProfilePreferencesBuilder.GetAllDateTimePatterns(installedLanguages, now);
      return new ProfilePreferencesModel()
      {
        TimeZones = (IList<TimeZoneInfoModel>) CommonUtility.AllTimeZones,
        Cultures = (IList<CultureInfoModel>) ProfilePreferencesBuilder.GetCultures(installedLanguages, now, httpContext.Request.UserLanguages, service.GetInstalledLanguages(vssRequestContext), service.GetUserInstalledLanguages(vssRequestContext)),
        DatePatterns = (IList<PatternModel>) dateTimePatterns.dateFormats,
        TimePatterns = (IList<PatternModel>) dateTimePatterns.timeFormats
      };
    }

    public static ProfilePreferencesBuilder.Pattern GetAllDateTimePatterns(
      IEnumerable<int> lcids,
      DateTime now)
    {
      ProfilePreferencesBuilder.Pattern dateTimePatterns = new ProfilePreferencesBuilder.Pattern();
      dateTimePatterns.dateFormats = new List<PatternModel>();
      dateTimePatterns.timeFormats = new List<PatternModel>();
      HashSet<string> Patterns1 = new HashSet<string>();
      HashSet<string> Patterns2 = new HashSet<string>();
      foreach (int lcid in lcids)
      {
        DateTimeFormatInfo dateTimeFormat = CultureInfo.GetCultureInfo(lcid).DateTimeFormat;
        ProfilePreferencesBuilder.GetDateTimePatterns(dateTimeFormat.GetAllDateTimePatterns('d'), Patterns1, dateTimePatterns.dateFormats, dateTimeFormat, now);
        ProfilePreferencesBuilder.GetDateTimePatterns(dateTimeFormat.GetAllDateTimePatterns('t'), Patterns2, dateTimePatterns.timeFormats, dateTimeFormat, now);
      }
      dateTimePatterns.dateFormats = dateTimePatterns.dateFormats.OrderByDescending<PatternModel, string>((Func<PatternModel, string>) (f => f.Format)).ToList<PatternModel>();
      return dateTimePatterns;
    }

    private static void GetDateTimePatterns(
      string[] currentPatterns,
      HashSet<string> Patterns,
      List<PatternModel> formats,
      DateTimeFormatInfo formatCopy,
      DateTime now)
    {
      foreach (string currentPattern in currentPatterns)
      {
        if (!Patterns.Contains(currentPattern))
        {
          string format = currentPattern.Replace("/", "'/'");
          string str = now.ToString(format);
          formats.Add(new PatternModel()
          {
            Format = format,
            DisplayFormat = string.Format((IFormatProvider) formatCopy, "{0} ({1})", (object) str, (object) currentPattern)
          });
          Patterns.Add(currentPattern);
        }
      }
    }

    public static List<CultureInfoModel> GetCultures(
      IEnumerable<int> lcids,
      DateTime now,
      string[] userLanguage,
      ISet<int> installedLanguages,
      ISet<int> userInstalledLanguages)
    {
      List<CultureInfoModel> list = lcids.Select<int, CultureInfoModel>((Func<int, CultureInfoModel>) (lcid => new CultureInfoModel(CultureInfo.GetCultureInfo(lcid), now))).ToList<CultureInfoModel>();
      CultureInfo preferredCulture;
      TeamFoundationApplicationCore.GetPreferredCulture(userLanguage, installedLanguages, userInstalledLanguages, out preferredCulture);
      if (preferredCulture != null)
        list.Insert(0, (CultureInfoModel) new DefaultCultureInfoModel(preferredCulture, now));
      return list;
    }

    public struct Pattern
    {
      public List<PatternModel> dateFormats { get; set; }

      public List<PatternModel> timeFormats { get; set; }
    }
  }
}
