// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Traceability.Server.Resources
// Assembly: Microsoft.TeamFoundation.Traceability.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C62AF110-A283-470F-B32B-FE03F2A1E0D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Traceability.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Traceability.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.TeamFoundation.Traceability.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.TeamFoundation.Traceability.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.TeamFoundation.Traceability.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Traceability.Server.Resources.Get(resourceName) : Microsoft.TeamFoundation.Traceability.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.TeamFoundation.Traceability.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Traceability.Server.Resources.GetInt(resourceName) : (int) Microsoft.TeamFoundation.Traceability.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.TeamFoundation.Traceability.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.Traceability.Server.Resources.GetBool(resourceName) : (bool) Microsoft.TeamFoundation.Traceability.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.TeamFoundation.Traceability.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.TeamFoundation.Traceability.Server.Resources.Get(resourceName, culture);
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

    public static string BaseArtifactSourceVersionNullError(object arg0, object arg1) => Microsoft.TeamFoundation.Traceability.Server.Resources.Format(nameof (BaseArtifactSourceVersionNullError), arg0, arg1);

    public static string BaseArtifactSourceVersionNullError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Traceability.Server.Resources.Format(nameof (BaseArtifactSourceVersionNullError), culture, arg0, arg1);
    }

    public static string ChangesProviderNotFound(object arg0) => Microsoft.TeamFoundation.Traceability.Server.Resources.Format(nameof (ChangesProviderNotFound), arg0);

    public static string ChangesProviderNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Traceability.Server.Resources.Format(nameof (ChangesProviderNotFound), culture, arg0);

    public static string MultipleProvidersWarning(object arg0) => Microsoft.TeamFoundation.Traceability.Server.Resources.Format(nameof (MultipleProvidersWarning), arg0);

    public static string MultipleProvidersWarning(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Traceability.Server.Resources.Format(nameof (MultipleProvidersWarning), culture, arg0);

    public static string ReposNotMatchingError(object arg0, object arg1, object arg2) => Microsoft.TeamFoundation.Traceability.Server.Resources.Format(nameof (ReposNotMatchingError), arg0, arg1, arg2);

    public static string ReposNotMatchingError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.Traceability.Server.Resources.Format(nameof (ReposNotMatchingError), culture, arg0, arg1, arg2);
    }

    public static string WorkItemsProviderNotFound(object arg0) => Microsoft.TeamFoundation.Traceability.Server.Resources.Format(nameof (WorkItemsProviderNotFound), arg0);

    public static string WorkItemsProviderNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.Traceability.Server.Resources.Format(nameof (WorkItemsProviderNotFound), culture, arg0);
  }
}
