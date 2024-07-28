// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ASPNetWithContainersResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class ASPNetWithContainersResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ASPNetWithContainersResources), typeof (ASPNetWithContainersResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ASPNetWithContainersResources.s_resMgr;

    private static string Get(string resourceName) => ASPNetWithContainersResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ASPNetWithContainersResources.Get(resourceName) : ASPNetWithContainersResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ASPNetWithContainersResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ASPNetWithContainersResources.GetInt(resourceName) : (int) ASPNetWithContainersResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ASPNetWithContainersResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ASPNetWithContainersResources.GetBool(resourceName) : (bool) ASPNetWithContainersResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ASPNetWithContainersResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ASPNetWithContainersResources.Get(resourceName, culture);
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

    public static string Description() => ASPNetWithContainersResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => ASPNetWithContainersResources.Get(nameof (Description), culture);

    public static string DockerFileDescription() => ASPNetWithContainersResources.Get(nameof (DockerFileDescription));

    public static string DockerFileDescription(CultureInfo culture) => ASPNetWithContainersResources.Get(nameof (DockerFileDescription), culture);

    public static string DockerFileLabel() => ASPNetWithContainersResources.Get(nameof (DockerFileLabel));

    public static string DockerFileLabel(CultureInfo culture) => ASPNetWithContainersResources.Get(nameof (DockerFileLabel), culture);

    public static string Name() => ASPNetWithContainersResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => ASPNetWithContainersResources.Get(nameof (Name), culture);

    public static string RegistryDescription() => ASPNetWithContainersResources.Get(nameof (RegistryDescription));

    public static string RegistryDescription(CultureInfo culture) => ASPNetWithContainersResources.Get(nameof (RegistryDescription), culture);

    public static string RegistryLabel() => ASPNetWithContainersResources.Get(nameof (RegistryLabel));

    public static string RegistryLabel(CultureInfo culture) => ASPNetWithContainersResources.Get(nameof (RegistryLabel), culture);

    public static string SolutionDescription() => ASPNetWithContainersResources.Get(nameof (SolutionDescription));

    public static string SolutionDescription(CultureInfo culture) => ASPNetWithContainersResources.Get(nameof (SolutionDescription), culture);

    public static string SolutionLabel() => ASPNetWithContainersResources.Get(nameof (SolutionLabel));

    public static string SolutionLabel(CultureInfo culture) => ASPNetWithContainersResources.Get(nameof (SolutionLabel), culture);

    public static string SubscriptionDescription() => ASPNetWithContainersResources.Get(nameof (SubscriptionDescription));

    public static string SubscriptionDescription(CultureInfo culture) => ASPNetWithContainersResources.Get(nameof (SubscriptionDescription), culture);

    public static string SubscriptionLabel() => ASPNetWithContainersResources.Get(nameof (SubscriptionLabel));

    public static string SubscriptionLabel(CultureInfo culture) => ASPNetWithContainersResources.Get(nameof (SubscriptionLabel), culture);
  }
}
