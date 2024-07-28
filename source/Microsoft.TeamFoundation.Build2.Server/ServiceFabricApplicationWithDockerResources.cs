// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ServiceFabricApplicationWithDockerResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class ServiceFabricApplicationWithDockerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ServiceFabricApplicationWithDockerResources), typeof (ServiceFabricApplicationWithDockerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ServiceFabricApplicationWithDockerResources.s_resMgr;

    private static string Get(string resourceName) => ServiceFabricApplicationWithDockerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ServiceFabricApplicationWithDockerResources.Get(resourceName) : ServiceFabricApplicationWithDockerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ServiceFabricApplicationWithDockerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ServiceFabricApplicationWithDockerResources.GetInt(resourceName) : (int) ServiceFabricApplicationWithDockerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ServiceFabricApplicationWithDockerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ServiceFabricApplicationWithDockerResources.GetBool(resourceName) : (bool) ServiceFabricApplicationWithDockerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ServiceFabricApplicationWithDockerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ServiceFabricApplicationWithDockerResources.Get(resourceName, culture);
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

    public static string Description() => ServiceFabricApplicationWithDockerResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => ServiceFabricApplicationWithDockerResources.Get(nameof (Description), culture);

    public static string Name() => ServiceFabricApplicationWithDockerResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => ServiceFabricApplicationWithDockerResources.Get(nameof (Name), culture);

    public static string ProjectDescription() => ServiceFabricApplicationWithDockerResources.Get(nameof (ProjectDescription));

    public static string ProjectDescription(CultureInfo culture) => ServiceFabricApplicationWithDockerResources.Get(nameof (ProjectDescription), culture);

    public static string ProjectLabel() => ServiceFabricApplicationWithDockerResources.Get(nameof (ProjectLabel));

    public static string ProjectLabel(CultureInfo culture) => ServiceFabricApplicationWithDockerResources.Get(nameof (ProjectLabel), culture);

    public static string SourceDescription() => ServiceFabricApplicationWithDockerResources.Get(nameof (SourceDescription));

    public static string SourceDescription(CultureInfo culture) => ServiceFabricApplicationWithDockerResources.Get(nameof (SourceDescription), culture);

    public static string SourceLabel() => ServiceFabricApplicationWithDockerResources.Get(nameof (SourceLabel));

    public static string SourceLabel(CultureInfo culture) => ServiceFabricApplicationWithDockerResources.Get(nameof (SourceLabel), culture);
  }
}
