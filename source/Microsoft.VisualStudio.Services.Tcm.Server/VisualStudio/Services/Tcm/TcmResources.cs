// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmResources
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Tcm
{
  internal static class TcmResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (TcmResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => TcmResources.s_resMgr;

    private static string Get(string resourceName) => TcmResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? TcmResources.Get(resourceName) : TcmResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) TcmResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? TcmResources.GetInt(resourceName) : (int) TcmResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) TcmResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? TcmResources.GetBool(resourceName) : (bool) TcmResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => TcmResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = TcmResources.Get(resourceName, culture);
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

    public static string CoverageBadgeLeftText() => TcmResources.Get(nameof (CoverageBadgeLeftText));

    public static string CoverageBadgeLeftText(CultureInfo culture) => TcmResources.Get(nameof (CoverageBadgeLeftText), culture);

    public static string CoverageBadgeNoBuilds() => TcmResources.Get(nameof (CoverageBadgeNoBuilds));

    public static string CoverageBadgeNoBuilds(CultureInfo culture) => TcmResources.Get(nameof (CoverageBadgeNoBuilds), culture);

    public static string CoverageBadgeNoDefinition() => TcmResources.Get(nameof (CoverageBadgeNoDefinition));

    public static string CoverageBadgeNoDefinition(CultureInfo culture) => TcmResources.Get(nameof (CoverageBadgeNoDefinition), culture);

    public static string CoverageBadgeNoCoverage() => TcmResources.Get(nameof (CoverageBadgeNoCoverage));

    public static string CoverageBadgeNoCoverage(CultureInfo culture) => TcmResources.Get(nameof (CoverageBadgeNoCoverage), culture);
  }
}
