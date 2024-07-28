// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitTemplateResources
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitTemplateResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (GitTemplateResources), typeof (GitTemplateResources).GetTypeInfo().Assembly);
    public const string readme_README_md = "readme_README_md";

    public static ResourceManager Manager => GitTemplateResources.s_resMgr;

    public static string Get(string resourceName) => GitTemplateResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? GitTemplateResources.Get(resourceName) : GitTemplateResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) GitTemplateResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? GitTemplateResources.GetInt(resourceName) : (int) GitTemplateResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) GitTemplateResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? GitTemplateResources.GetBool(resourceName) : (bool) GitTemplateResources.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => GitTemplateResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = GitTemplateResources.Get(resourceName, culture);
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
  }
}
