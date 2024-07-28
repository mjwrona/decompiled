// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.azureiaasResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class azureiaasResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (azureiaasResources), typeof (azureiaasResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => azureiaasResources.s_resMgr;

    private static string Get(string resourceName) => azureiaasResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? azureiaasResources.Get(resourceName) : azureiaasResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) azureiaasResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? azureiaasResources.GetInt(resourceName) : (int) azureiaasResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) azureiaasResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? azureiaasResources.GetBool(resourceName) : (bool) azureiaasResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => azureiaasResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = azureiaasResources.Get(resourceName, culture);
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

    public static string AzureSubscriptionDescription() => azureiaasResources.Get(nameof (AzureSubscriptionDescription));

    public static string AzureSubscriptionDescription(CultureInfo culture) => azureiaasResources.Get(nameof (AzureSubscriptionDescription), culture);

    public static string AzureSubscriptionLabel() => azureiaasResources.Get(nameof (AzureSubscriptionLabel));

    public static string AzureSubscriptionLabel(CultureInfo culture) => azureiaasResources.Get(nameof (AzureSubscriptionLabel), culture);

    public static string AzureTemplateDescription() => azureiaasResources.Get(nameof (AzureTemplateDescription));

    public static string AzureTemplateDescription(CultureInfo culture) => azureiaasResources.Get(nameof (AzureTemplateDescription), culture);

    public static string AzureTemplateLabel() => azureiaasResources.Get(nameof (AzureTemplateLabel));

    public static string AzureTemplateLabel(CultureInfo culture) => azureiaasResources.Get(nameof (AzureTemplateLabel), culture);

    public static string Description() => azureiaasResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => azureiaasResources.Get(nameof (Description), culture);

    public static string LoadTestDescription() => azureiaasResources.Get(nameof (LoadTestDescription));

    public static string LoadTestDescription(CultureInfo culture) => azureiaasResources.Get(nameof (LoadTestDescription), culture);

    public static string LoadTestLabel() => azureiaasResources.Get(nameof (LoadTestLabel));

    public static string LoadTestLabel(CultureInfo culture) => azureiaasResources.Get(nameof (LoadTestLabel), culture);

    public static string Name() => azureiaasResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => azureiaasResources.Get(nameof (Name), culture);

    public static string TestDropDescription() => azureiaasResources.Get(nameof (TestDropDescription));

    public static string TestDropDescription(CultureInfo culture) => azureiaasResources.Get(nameof (TestDropDescription), culture);

    public static string TestDropLabel() => azureiaasResources.Get(nameof (TestDropLabel));

    public static string TestDropLabel(CultureInfo culture) => azureiaasResources.Get(nameof (TestDropLabel), culture);

    public static string TestSettingsDescription() => azureiaasResources.Get(nameof (TestSettingsDescription));

    public static string TestSettingsDescription(CultureInfo culture) => azureiaasResources.Get(nameof (TestSettingsDescription), culture);

    public static string TestSettingsLabel() => azureiaasResources.Get(nameof (TestSettingsLabel));

    public static string TestSettingsLabel(CultureInfo culture) => azureiaasResources.Get(nameof (TestSettingsLabel), culture);
  }
}
