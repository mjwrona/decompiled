// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources
// Assembly: Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6486B3F7-B3D2-46E4-8024-05D53FB42B10
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Get(resourceName) : Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.GetInt(resourceName) : (int) Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.GetBool(resourceName) : (bool) Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Get(resourceName, culture);
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

    public static string ContributionDoesNotTargetBuildTask(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (ContributionDoesNotTargetBuildTask), arg0);

    public static string ContributionDoesNotTargetBuildTask(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (ContributionDoesNotTargetBuildTask), culture, arg0);

    public static string ContributionDoesNotTargetServiceEndpoint(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (ContributionDoesNotTargetServiceEndpoint), arg0);

    public static string ContributionDoesNotTargetServiceEndpoint(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (ContributionDoesNotTargetServiceEndpoint), culture, arg0);

    public static string ContributionTaskIdsShouldMatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (ContributionTaskIdsShouldMatch), arg0, arg1, arg2, arg3, arg4);
    }

    public static string ContributionTaskIdsShouldMatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (ContributionTaskIdsShouldMatch), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string ExtensionIsPublicAndHasPipelineDecorators(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (ExtensionIsPublicAndHasPipelineDecorators), arg0);

    public static string ExtensionIsPublicAndHasPipelineDecorators(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (ExtensionIsPublicAndHasPipelineDecorators), culture, arg0);

    public static string TaskDefinitionCouldNotBeDeserialized(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (TaskDefinitionCouldNotBeDeserialized), arg0);

    public static string TaskDefinitionCouldNotBeDeserialized(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (TaskDefinitionCouldNotBeDeserialized), culture, arg0);

    public static string TaskJsonNotFound(object arg0) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (TaskJsonNotFound), arg0);

    public static string TaskJsonNotFound(object arg0, CultureInfo culture) => Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.Resources.Format(nameof (TaskJsonNotFound), culture, arg0);
  }
}
