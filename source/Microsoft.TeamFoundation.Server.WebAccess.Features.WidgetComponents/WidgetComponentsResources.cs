// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Features.WidgetComponents.WidgetComponentsResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Features.WidgetComponents, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 01430B34-4C00-4D23-8456-39ADD60E691C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Features.WidgetComponents.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.Features.WidgetComponents
{
  internal static class WidgetComponentsResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (WidgetComponentsResources), typeof (WidgetComponentsResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => WidgetComponentsResources.s_resMgr;

    private static string Get(string resourceName) => WidgetComponentsResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? WidgetComponentsResources.Get(resourceName) : WidgetComponentsResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) WidgetComponentsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? WidgetComponentsResources.GetInt(resourceName) : (int) WidgetComponentsResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) WidgetComponentsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? WidgetComponentsResources.GetBool(resourceName) : (bool) WidgetComponentsResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => WidgetComponentsResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = WidgetComponentsResources.Get(resourceName, culture);
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

    public static string LayoutEngine_NoLayoutError() => WidgetComponentsResources.Get(nameof (LayoutEngine_NoLayoutError));

    public static string LayoutEngine_NoLayoutError(CultureInfo culture) => WidgetComponentsResources.Get(nameof (LayoutEngine_NoLayoutError), culture);
  }
}
