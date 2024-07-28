// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.TfsResources
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Tfs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3EB20FA-6669-4C21-BA19-EC9C2EBF5243
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Tfs.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  internal static class TfsResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (TfsResources), typeof (TfsResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => TfsResources.s_resMgr;

    private static string Get(string resourceName) => TfsResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? TfsResources.Get(resourceName) : TfsResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) TfsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? TfsResources.GetInt(resourceName) : (int) TfsResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) TfsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? TfsResources.GetBool(resourceName) : (bool) TfsResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => TfsResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = TfsResources.Get(resourceName, culture);
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

    public static string FileExceedsMaxLength(object arg0, object arg1) => TfsResources.Format(nameof (FileExceedsMaxLength), arg0, arg1);

    public static string FileExceedsMaxLength(object arg0, object arg1, CultureInfo culture) => TfsResources.Format(nameof (FileExceedsMaxLength), culture, arg0, arg1);

    public static string FileNotFoundInRepo(object arg0, object arg1, object arg2, object arg3) => TfsResources.Format(nameof (FileNotFoundInRepo), arg0, arg1, arg2, arg3);

    public static string FileNotFoundInRepo(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TfsResources.Format(nameof (FileNotFoundInRepo), culture, arg0, arg1, arg2, arg3);
    }

    public static string AmbiguousBuildDefinitionsFound(object arg0, object arg1) => TfsResources.Format(nameof (AmbiguousBuildDefinitionsFound), arg0, arg1);

    public static string AmbiguousBuildDefinitionsFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TfsResources.Format(nameof (AmbiguousBuildDefinitionsFound), culture, arg0, arg1);
    }

    public static string BuildDefinitionNotFound(object arg0, object arg1) => TfsResources.Format(nameof (BuildDefinitionNotFound), arg0, arg1);

    public static string BuildDefinitionNotFound(object arg0, object arg1, CultureInfo culture) => TfsResources.Format(nameof (BuildDefinitionNotFound), culture, arg0, arg1);
  }
}
