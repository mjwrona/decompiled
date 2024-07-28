// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ServiceFabricApplicationResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class ServiceFabricApplicationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ServiceFabricApplicationResources), typeof (ServiceFabricApplicationResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ServiceFabricApplicationResources.s_resMgr;

    private static string Get(string resourceName) => ServiceFabricApplicationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ServiceFabricApplicationResources.Get(resourceName) : ServiceFabricApplicationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ServiceFabricApplicationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ServiceFabricApplicationResources.GetInt(resourceName) : (int) ServiceFabricApplicationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ServiceFabricApplicationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ServiceFabricApplicationResources.GetBool(resourceName) : (bool) ServiceFabricApplicationResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ServiceFabricApplicationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ServiceFabricApplicationResources.Get(resourceName, culture);
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

    public static string Description() => ServiceFabricApplicationResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => ServiceFabricApplicationResources.Get(nameof (Description), culture);

    public static string Name() => ServiceFabricApplicationResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => ServiceFabricApplicationResources.Get(nameof (Name), culture);

    public static string ProjectDescription() => ServiceFabricApplicationResources.Get(nameof (ProjectDescription));

    public static string ProjectDescription(CultureInfo culture) => ServiceFabricApplicationResources.Get(nameof (ProjectDescription), culture);

    public static string ProjectLabel() => ServiceFabricApplicationResources.Get(nameof (ProjectLabel));

    public static string ProjectLabel(CultureInfo culture) => ServiceFabricApplicationResources.Get(nameof (ProjectLabel), culture);

    public static string SourceDescription() => ServiceFabricApplicationResources.Get(nameof (SourceDescription));

    public static string SourceDescription(CultureInfo culture) => ServiceFabricApplicationResources.Get(nameof (SourceDescription), culture);

    public static string SourceLabel() => ServiceFabricApplicationResources.Get(nameof (SourceLabel));

    public static string SourceLabel(CultureInfo culture) => ServiceFabricApplicationResources.Get(nameof (SourceLabel), culture);
  }
}
