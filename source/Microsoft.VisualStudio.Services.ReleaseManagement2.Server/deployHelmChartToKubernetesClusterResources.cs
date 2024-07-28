// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.deployHelmChartToKubernetesClusterResources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  internal static class deployHelmChartToKubernetesClusterResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (deployHelmChartToKubernetesClusterResources), IntrospectionExtensions.GetTypeInfo(typeof (deployHelmChartToKubernetesClusterResources)).Assembly);

    public static ResourceManager Manager => deployHelmChartToKubernetesClusterResources.s_resMgr;

    private static string Get(string resourceName) => deployHelmChartToKubernetesClusterResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? deployHelmChartToKubernetesClusterResources.Get(resourceName) : deployHelmChartToKubernetesClusterResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) deployHelmChartToKubernetesClusterResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? deployHelmChartToKubernetesClusterResources.GetInt(resourceName) : (int) deployHelmChartToKubernetesClusterResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) deployHelmChartToKubernetesClusterResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? deployHelmChartToKubernetesClusterResources.GetBool(resourceName) : (bool) deployHelmChartToKubernetesClusterResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => deployHelmChartToKubernetesClusterResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = deployHelmChartToKubernetesClusterResources.Get(resourceName, culture);
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

    public static string AzureResourceGroupLabel() => deployHelmChartToKubernetesClusterResources.Get(nameof (AzureResourceGroupLabel));

    public static string AzureResourceGroupLabel(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (AzureResourceGroupLabel), culture);

    public static string AzureResourceGroupMarkdown() => deployHelmChartToKubernetesClusterResources.Get(nameof (AzureResourceGroupMarkdown));

    public static string AzureResourceGroupMarkdown(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (AzureResourceGroupMarkdown), culture);

    public static string AzureSubscriptionEndpointLabel() => deployHelmChartToKubernetesClusterResources.Get(nameof (AzureSubscriptionEndpointLabel));

    public static string AzureSubscriptionEndpointLabel(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (AzureSubscriptionEndpointLabel), culture);

    public static string AzureSubscriptionEndpointMarkdown() => deployHelmChartToKubernetesClusterResources.Get(nameof (AzureSubscriptionEndpointMarkdown));

    public static string AzureSubscriptionEndpointMarkdown(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (AzureSubscriptionEndpointMarkdown), culture);

    public static string ConnectionTypeLabel() => deployHelmChartToKubernetesClusterResources.Get(nameof (ConnectionTypeLabel));

    public static string ConnectionTypeLabel(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (ConnectionTypeLabel), culture);

    public static string Description() => deployHelmChartToKubernetesClusterResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (Description), culture);

    public static string KubernetesClusterLabel() => deployHelmChartToKubernetesClusterResources.Get(nameof (KubernetesClusterLabel));

    public static string KubernetesClusterLabel(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (KubernetesClusterLabel), culture);

    public static string KubernetesClusterMarkdown() => deployHelmChartToKubernetesClusterResources.Get(nameof (KubernetesClusterMarkdown));

    public static string KubernetesClusterMarkdown(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (KubernetesClusterMarkdown), culture);

    public static string KubernetesServiceEndpointLabel() => deployHelmChartToKubernetesClusterResources.Get(nameof (KubernetesServiceEndpointLabel));

    public static string KubernetesServiceEndpointLabel(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (KubernetesServiceEndpointLabel), culture);

    public static string KubernetesServiceEndpointMarkdown() => deployHelmChartToKubernetesClusterResources.Get(nameof (KubernetesServiceEndpointMarkdown));

    public static string KubernetesServiceEndpointMarkdown(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (KubernetesServiceEndpointMarkdown), culture);

    public static string Name() => deployHelmChartToKubernetesClusterResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => deployHelmChartToKubernetesClusterResources.Get(nameof (Name), culture);
  }
}
