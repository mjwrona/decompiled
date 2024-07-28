// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.Resources
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E6307531-8252-47C3-B21C-ECA66F38ED4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ItemStore.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.ItemStore.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.ItemStore.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.ItemStore.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ItemStore.Server.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.ItemStore.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.ItemStore.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.ItemStore.Server.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.ItemStore.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Get(resourceName, culture);
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

    public static string MissingContainer(object arg0) => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Format(nameof (MissingContainer), arg0);

    public static string MissingContainer(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Format(nameof (MissingContainer), culture, arg0);

    public static string UnknownProviderType(object arg0) => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Format(nameof (UnknownProviderType), arg0);

    public static string UnknownProviderType(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Format(nameof (UnknownProviderType), culture, arg0);

    public static string ExperienceNameNotSpecified() => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Get(nameof (ExperienceNameNotSpecified));

    public static string ExperienceNameNotSpecified(CultureInfo culture) => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Get(nameof (ExperienceNameNotSpecified), culture);

    public static string InvalidExperienceName(object arg0) => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Format(nameof (InvalidExperienceName), arg0);

    public static string InvalidExperienceName(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.ItemStore.Server.Resources.Format(nameof (InvalidExperienceName), culture, arg0);
  }
}
