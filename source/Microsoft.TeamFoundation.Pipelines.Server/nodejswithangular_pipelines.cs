// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.nodejswithangular_pipelines
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal static class nodejswithangular_pipelines
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (nodejswithangular_pipelines), typeof (nodejswithangular_pipelines).GetTypeInfo().Assembly);

    public static ResourceManager Manager => nodejswithangular_pipelines.s_resMgr;

    private static string Get(string resourceName) => nodejswithangular_pipelines.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? nodejswithangular_pipelines.Get(resourceName) : nodejswithangular_pipelines.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) nodejswithangular_pipelines.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? nodejswithangular_pipelines.GetInt(resourceName) : (int) nodejswithangular_pipelines.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) nodejswithangular_pipelines.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? nodejswithangular_pipelines.GetBool(resourceName) : (bool) nodejswithangular_pipelines.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => nodejswithangular_pipelines.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = nodejswithangular_pipelines.Get(resourceName, culture);
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

    public static string Id() => nodejswithangular_pipelines.Get(nameof (Id));

    public static string Id(CultureInfo culture) => nodejswithangular_pipelines.Get(nameof (Id), culture);

    public static string IconAssetPath() => nodejswithangular_pipelines.Get(nameof (IconAssetPath));

    public static string IconAssetPath(CultureInfo culture) => nodejswithangular_pipelines.Get(nameof (IconAssetPath), culture);

    public static string Description() => nodejswithangular_pipelines.Get(nameof (Description));

    public static string Description(CultureInfo culture) => nodejswithangular_pipelines.Get(nameof (Description), culture);

    public static string Name() => nodejswithangular_pipelines.Get(nameof (Name));

    public static string Name(CultureInfo culture) => nodejswithangular_pipelines.Get(nameof (Name), culture);

    public static string Parameters() => nodejswithangular_pipelines.Get(nameof (Parameters));

    public static string Parameters(CultureInfo culture) => nodejswithangular_pipelines.Get(nameof (Parameters), culture);

    public static string DataSourceBindings() => nodejswithangular_pipelines.Get(nameof (DataSourceBindings));

    public static string DataSourceBindings(CultureInfo culture) => nodejswithangular_pipelines.Get(nameof (DataSourceBindings), culture);

    public static string Assets() => nodejswithangular_pipelines.Get(nameof (Assets));

    public static string Assets(CultureInfo culture) => nodejswithangular_pipelines.Get(nameof (Assets), culture);
  }
}
