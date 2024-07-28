// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.Resources
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.Feed.Common.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.Feed.Common.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.Feed.Common.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Feed.Common.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.Feed.Common.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.Feed.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Feed.Common.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.Feed.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.Feed.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.Feed.Common.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.Feed.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.Feed.Common.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.Feed.Common.Resources.Get(resourceName, culture);
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

    public static string Error_FeedIdIsInvalid(object arg0) => Microsoft.VisualStudio.Services.Feed.Common.Resources.Format(nameof (Error_FeedIdIsInvalid), arg0);

    public static string Error_FeedIdIsInvalid(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Common.Resources.Format(nameof (Error_FeedIdIsInvalid), culture, arg0);

    public static string Error_InvalidPackageArtifactUri(object arg0) => Microsoft.VisualStudio.Services.Feed.Common.Resources.Format(nameof (Error_InvalidPackageArtifactUri), arg0);

    public static string Error_InvalidPackageArtifactUri(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Common.Resources.Format(nameof (Error_InvalidPackageArtifactUri), culture, arg0);

    public static string Error_NotSupportedProtocolOperation(object arg0, object arg1) => Microsoft.VisualStudio.Services.Feed.Common.Resources.Format(nameof (Error_NotSupportedProtocolOperation), arg0, arg1);

    public static string Error_NotSupportedProtocolOperation(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Common.Resources.Format(nameof (Error_NotSupportedProtocolOperation), culture, arg0, arg1);
    }

    public static string Error_UpstreamLocationMustBeAbsolute() => Microsoft.VisualStudio.Services.Feed.Common.Resources.Get(nameof (Error_UpstreamLocationMustBeAbsolute));

    public static string Error_UpstreamLocationMustBeAbsolute(CultureInfo culture) => Microsoft.VisualStudio.Services.Feed.Common.Resources.Get(nameof (Error_UpstreamLocationMustBeAbsolute), culture);

    public static string Error_WellKnownUpstreamWrongProtocol(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return Microsoft.VisualStudio.Services.Feed.Common.Resources.Format(nameof (Error_WellKnownUpstreamWrongProtocol), arg0, arg1, arg2, arg3);
    }

    public static string Error_WellKnownUpstreamWrongProtocol(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.Feed.Common.Resources.Format(nameof (Error_WellKnownUpstreamWrongProtocol), culture, arg0, arg1, arg2, arg3);
    }
  }
}
