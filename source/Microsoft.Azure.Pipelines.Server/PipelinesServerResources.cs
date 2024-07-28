// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.PipelinesServerResources
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Pipelines.Server
{
  internal static class PipelinesServerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (PipelinesServerResources), typeof (PipelinesServerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => PipelinesServerResources.s_resMgr;

    private static string Get(string resourceName) => PipelinesServerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? PipelinesServerResources.Get(resourceName) : PipelinesServerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) PipelinesServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? PipelinesServerResources.GetInt(resourceName) : (int) PipelinesServerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) PipelinesServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? PipelinesServerResources.GetBool(resourceName) : (bool) PipelinesServerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => PipelinesServerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = PipelinesServerResources.Get(resourceName, culture);
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

    public static string ExternalPipelinesNotSupportedError() => PipelinesServerResources.Get(nameof (ExternalPipelinesNotSupportedError));

    public static string ExternalPipelinesNotSupportedError(CultureInfo culture) => PipelinesServerResources.Get(nameof (ExternalPipelinesNotSupportedError), culture);

    public static string ExternalRunsNotSupportedError() => PipelinesServerResources.Get(nameof (ExternalRunsNotSupportedError));

    public static string ExternalRunsNotSupportedError(CultureInfo culture) => PipelinesServerResources.Get(nameof (ExternalRunsNotSupportedError), culture);

    public static string InvalidContinuationToken() => PipelinesServerResources.Get(nameof (InvalidContinuationToken));

    public static string InvalidContinuationToken(CultureInfo culture) => PipelinesServerResources.Get(nameof (InvalidContinuationToken), culture);

    public static string UnsupportedSortExpressionDirection(object arg0) => PipelinesServerResources.Format(nameof (UnsupportedSortExpressionDirection), arg0);

    public static string UnsupportedSortExpressionDirection(object arg0, CultureInfo culture) => PipelinesServerResources.Format(nameof (UnsupportedSortExpressionDirection), culture, arg0);

    public static string UnsupportedSortExpressionField(object arg0) => PipelinesServerResources.Format(nameof (UnsupportedSortExpressionField), arg0);

    public static string UnsupportedSortExpressionField(object arg0, CultureInfo culture) => PipelinesServerResources.Format(nameof (UnsupportedSortExpressionField), culture, arg0);

    public static string InvalidTimelineRecordType(object arg0) => PipelinesServerResources.Format(nameof (InvalidTimelineRecordType), arg0);

    public static string InvalidTimelineRecordType(object arg0, CultureInfo culture) => PipelinesServerResources.Format(nameof (InvalidTimelineRecordType), culture, arg0);

    public static string TimelineRecordIsntChildOfStageRecord() => PipelinesServerResources.Get(nameof (TimelineRecordIsntChildOfStageRecord));

    public static string TimelineRecordIsntChildOfStageRecord(CultureInfo culture) => PipelinesServerResources.Get(nameof (TimelineRecordIsntChildOfStageRecord), culture);

    public static string LogNotFound(object arg0) => PipelinesServerResources.Format(nameof (LogNotFound), arg0);

    public static string LogNotFound(object arg0, CultureInfo culture) => PipelinesServerResources.Format(nameof (LogNotFound), culture, arg0);

    public static string LogsNotFound(object arg0) => PipelinesServerResources.Format(nameof (LogsNotFound), arg0);

    public static string LogsNotFound(object arg0, CultureInfo culture) => PipelinesServerResources.Format(nameof (LogsNotFound), culture, arg0);

    public static string RunNotFound(object arg0) => PipelinesServerResources.Format(nameof (RunNotFound), arg0);

    public static string RunNotFound(object arg0, CultureInfo culture) => PipelinesServerResources.Format(nameof (RunNotFound), culture, arg0);
  }
}
