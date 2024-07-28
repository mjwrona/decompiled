// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AzureCloudResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class AzureCloudResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (AzureCloudResources), typeof (AzureCloudResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AzureCloudResources.s_resMgr;

    private static string Get(string resourceName) => AzureCloudResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AzureCloudResources.Get(resourceName) : AzureCloudResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AzureCloudResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AzureCloudResources.GetInt(resourceName) : (int) AzureCloudResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AzureCloudResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AzureCloudResources.GetBool(resourceName) : (bool) AzureCloudResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AzureCloudResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AzureCloudResources.Get(resourceName, culture);
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

    public static string CloudServiceProjectDescription() => AzureCloudResources.Get(nameof (CloudServiceProjectDescription));

    public static string CloudServiceProjectDescription(CultureInfo culture) => AzureCloudResources.Get(nameof (CloudServiceProjectDescription), culture);

    public static string CloudServiceProjectLabel() => AzureCloudResources.Get(nameof (CloudServiceProjectLabel));

    public static string CloudServiceProjectLabel(CultureInfo culture) => AzureCloudResources.Get(nameof (CloudServiceProjectLabel), culture);

    public static string ConnectedServiceDescription() => AzureCloudResources.Get(nameof (ConnectedServiceDescription));

    public static string ConnectedServiceDescription(CultureInfo culture) => AzureCloudResources.Get(nameof (ConnectedServiceDescription), culture);

    public static string ConnectedServiceNameLabel() => AzureCloudResources.Get(nameof (ConnectedServiceNameLabel));

    public static string ConnectedServiceNameLabel(CultureInfo culture) => AzureCloudResources.Get(nameof (ConnectedServiceNameLabel), culture);

    public static string Description() => AzureCloudResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => AzureCloudResources.Get(nameof (Description), culture);

    public static string Name() => AzureCloudResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => AzureCloudResources.Get(nameof (Name), culture);

    public static string ServiceLocationDescription() => AzureCloudResources.Get(nameof (ServiceLocationDescription));

    public static string ServiceLocationDescription(CultureInfo culture) => AzureCloudResources.Get(nameof (ServiceLocationDescription), culture);

    public static string ServiceLocationLabel() => AzureCloudResources.Get(nameof (ServiceLocationLabel));

    public static string ServiceLocationLabel(CultureInfo culture) => AzureCloudResources.Get(nameof (ServiceLocationLabel), culture);

    public static string ServiceNameLabel() => AzureCloudResources.Get(nameof (ServiceNameLabel));

    public static string ServiceNameLabel(CultureInfo culture) => AzureCloudResources.Get(nameof (ServiceNameLabel), culture);

    public static string SeviceNameDescription() => AzureCloudResources.Get(nameof (SeviceNameDescription));

    public static string SeviceNameDescription(CultureInfo culture) => AzureCloudResources.Get(nameof (SeviceNameDescription), culture);

    public static string SolutionDescription() => AzureCloudResources.Get(nameof (SolutionDescription));

    public static string SolutionDescription(CultureInfo culture) => AzureCloudResources.Get(nameof (SolutionDescription), culture);

    public static string SolutionLabel() => AzureCloudResources.Get(nameof (SolutionLabel));

    public static string SolutionLabel(CultureInfo culture) => AzureCloudResources.Get(nameof (SolutionLabel), culture);

    public static string StorageAccountDescription() => AzureCloudResources.Get(nameof (StorageAccountDescription));

    public static string StorageAccountDescription(CultureInfo culture) => AzureCloudResources.Get(nameof (StorageAccountDescription), culture);

    public static string StorageAccountLabel() => AzureCloudResources.Get(nameof (StorageAccountLabel));

    public static string StorageAccountLabel(CultureInfo culture) => AzureCloudResources.Get(nameof (StorageAccountLabel), culture);
  }
}
