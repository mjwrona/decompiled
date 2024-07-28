// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Social.Server.SocialEngagementServerResource
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Social.Server
{
  internal static class SocialEngagementServerResource
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (SocialEngagementServerResource), typeof (SocialEngagementServerResource).GetTypeInfo().Assembly);

    public static ResourceManager Manager => SocialEngagementServerResource.s_resMgr;

    private static string Get(string resourceName) => SocialEngagementServerResource.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? SocialEngagementServerResource.Get(resourceName) : SocialEngagementServerResource.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) SocialEngagementServerResource.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? SocialEngagementServerResource.GetInt(resourceName) : (int) SocialEngagementServerResource.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) SocialEngagementServerResource.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? SocialEngagementServerResource.GetBool(resourceName) : (bool) SocialEngagementServerResource.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => SocialEngagementServerResource.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = SocialEngagementServerResource.Get(resourceName, culture);
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

    public static string InvalidArtifactTypeExceptionMessage(object arg0, object arg1) => SocialEngagementServerResource.Format(nameof (InvalidArtifactTypeExceptionMessage), arg0, arg1);

    public static string InvalidArtifactTypeExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return SocialEngagementServerResource.Format(nameof (InvalidArtifactTypeExceptionMessage), culture, arg0, arg1);
    }
  }
}
