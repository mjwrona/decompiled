// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AzureWebResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class AzureWebResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (AzureWebResources), typeof (AzureWebResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AzureWebResources.s_resMgr;

    private static string Get(string resourceName) => AzureWebResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AzureWebResources.Get(resourceName) : AzureWebResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AzureWebResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AzureWebResources.GetInt(resourceName) : (int) AzureWebResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AzureWebResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AzureWebResources.GetBool(resourceName) : (bool) AzureWebResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AzureWebResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AzureWebResources.Get(resourceName, culture);
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

    public static string Description() => AzureWebResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => AzureWebResources.Get(nameof (Description), culture);

    public static string Name() => AzureWebResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => AzureWebResources.Get(nameof (Name), culture);

    public static string SolutionDescription() => AzureWebResources.Get(nameof (SolutionDescription));

    public static string SolutionDescription(CultureInfo culture) => AzureWebResources.Get(nameof (SolutionDescription), culture);

    public static string SolutionLabel() => AzureWebResources.Get(nameof (SolutionLabel));

    public static string SolutionLabel(CultureInfo culture) => AzureWebResources.Get(nameof (SolutionLabel), culture);

    public static string SubscriptionDescription() => AzureWebResources.Get(nameof (SubscriptionDescription));

    public static string SubscriptionDescription(CultureInfo culture) => AzureWebResources.Get(nameof (SubscriptionDescription), culture);

    public static string SubscriptionLabel() => AzureWebResources.Get(nameof (SubscriptionLabel));

    public static string SubscriptionLabel(CultureInfo culture) => AzureWebResources.Get(nameof (SubscriptionLabel), culture);

    public static string WebAppDescription() => AzureWebResources.Get(nameof (WebAppDescription));

    public static string WebAppDescription(CultureInfo culture) => AzureWebResources.Get(nameof (WebAppDescription), culture);

    public static string WebAppLabel() => AzureWebResources.Get(nameof (WebAppLabel));

    public static string WebAppLabel(CultureInfo culture) => AzureWebResources.Get(nameof (WebAppLabel), culture);
  }
}
