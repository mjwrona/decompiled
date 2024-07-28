// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.Resources
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.TeamFoundation.PermissionsReport.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.TeamFoundation.PermissionsReport.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.TeamFoundation.PermissionsReport.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.PermissionsReport.Resources.Get(resourceName) : Microsoft.TeamFoundation.PermissionsReport.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.TeamFoundation.PermissionsReport.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.PermissionsReport.Resources.GetInt(resourceName) : (int) Microsoft.TeamFoundation.PermissionsReport.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.TeamFoundation.PermissionsReport.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.PermissionsReport.Resources.GetBool(resourceName) : (bool) Microsoft.TeamFoundation.PermissionsReport.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.TeamFoundation.PermissionsReport.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.TeamFoundation.PermissionsReport.Resources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string PermissionsReportNotFound(object arg0) => Microsoft.TeamFoundation.PermissionsReport.Resources.Format(nameof (PermissionsReportNotFound), arg0);

    public static string PermissionsReportNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.PermissionsReport.Resources.Format(nameof (PermissionsReportNotFound), culture, arg0);
  }
}
