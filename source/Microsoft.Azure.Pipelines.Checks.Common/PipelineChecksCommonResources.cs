// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Common.PipelineChecksCommonResources
// Assembly: Microsoft.Azure.Pipelines.Checks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C585FB3-01FB-4B82-B4E2-03BD94D0A581
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Pipelines.Checks.Common
{
  internal static class PipelineChecksCommonResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (PipelineChecksCommonResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => PipelineChecksCommonResources.s_resMgr;

    private static string Get(string resourceName) => PipelineChecksCommonResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? PipelineChecksCommonResources.Get(resourceName) : PipelineChecksCommonResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) PipelineChecksCommonResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? PipelineChecksCommonResources.GetInt(resourceName) : (int) PipelineChecksCommonResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) PipelineChecksCommonResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? PipelineChecksCommonResources.GetBool(resourceName) : (bool) PipelineChecksCommonResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => PipelineChecksCommonResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = PipelineChecksCommonResources.Get(resourceName, culture);
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

    public static string InvalidResourceType(object arg0) => PipelineChecksCommonResources.Format(nameof (InvalidResourceType), arg0);

    public static string InvalidResourceType(object arg0, CultureInfo culture) => PipelineChecksCommonResources.Format(nameof (InvalidResourceType), culture, arg0);
  }
}
