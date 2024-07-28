// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.ReportingResources
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  internal static class ReportingResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (ReportingResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ReportingResources.s_resMgr;

    private static string Get(string resourceName) => ReportingResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ReportingResources.Get(resourceName) : ReportingResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ReportingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ReportingResources.GetInt(resourceName) : (int) ReportingResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ReportingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ReportingResources.GetBool(resourceName) : (bool) ReportingResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ReportingResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ReportingResources.Get(resourceName, culture);
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

    public static string CantCreateChartWithExistingTransform() => ReportingResources.Get(nameof (CantCreateChartWithExistingTransform));

    public static string CantCreateChartWithExistingTransform(CultureInfo culture) => ReportingResources.Get(nameof (CantCreateChartWithExistingTransform), culture);

    public static string CantUpdateChartWithoutTransform() => ReportingResources.Get(nameof (CantUpdateChartWithoutTransform));

    public static string CantUpdateChartWithoutTransform(CultureInfo culture) => ReportingResources.Get(nameof (CantUpdateChartWithoutTransform), culture);

    public static string ChartConfigurationDoesNotExist() => ReportingResources.Get(nameof (ChartConfigurationDoesNotExist));

    public static string ChartConfigurationDoesNotExist(CultureInfo culture) => ReportingResources.Get(nameof (ChartConfigurationDoesNotExist), culture);

    public static string InvalidColorConfiguration(object arg0) => ReportingResources.Format(nameof (InvalidColorConfiguration), arg0);

    public static string InvalidColorConfiguration(object arg0, CultureInfo culture) => ReportingResources.Format(nameof (InvalidColorConfiguration), culture, arg0);

    public static string InvalidMeasureAggregation(object arg0) => ReportingResources.Format(nameof (InvalidMeasureAggregation), arg0);

    public static string InvalidMeasureAggregation(object arg0, CultureInfo culture) => ReportingResources.Format(nameof (InvalidMeasureAggregation), culture, arg0);

    public static string InvalidOrderDirection(object arg0) => ReportingResources.Format(nameof (InvalidOrderDirection), arg0);

    public static string InvalidOrderDirection(object arg0, CultureInfo culture) => ReportingResources.Format(nameof (InvalidOrderDirection), culture, arg0);

    public static string TooManyChartsPerGroup() => ReportingResources.Get(nameof (TooManyChartsPerGroup));

    public static string TooManyChartsPerGroup(CultureInfo culture) => ReportingResources.Get(nameof (TooManyChartsPerGroup), culture);

    public static string TooManyColorsPerChart() => ReportingResources.Get(nameof (TooManyColorsPerChart));

    public static string TooManyColorsPerChart(CultureInfo culture) => ReportingResources.Get(nameof (TooManyColorsPerChart), culture);

    public static string NoDataServiceIsRegisteredForThatType(object arg0) => ReportingResources.Format(nameof (NoDataServiceIsRegisteredForThatType), arg0);

    public static string NoDataServiceIsRegisteredForThatType(object arg0, CultureInfo culture) => ReportingResources.Format(nameof (NoDataServiceIsRegisteredForThatType), culture, arg0);

    public static string NoSuchChartScopeProvider(object arg0) => ReportingResources.Format(nameof (NoSuchChartScopeProvider), arg0);

    public static string NoSuchChartScopeProvider(object arg0, CultureInfo culture) => ReportingResources.Format(nameof (NoSuchChartScopeProvider), culture, arg0);

    public static string ChartProviderNotEnabled(object arg0) => ReportingResources.Format(nameof (ChartProviderNotEnabled), arg0);

    public static string ChartProviderNotEnabled(object arg0, CultureInfo culture) => ReportingResources.Format(nameof (ChartProviderNotEnabled), culture, arg0);

    public static string NoValidChartConfigurationDetected() => ReportingResources.Get(nameof (NoValidChartConfigurationDetected));

    public static string NoValidChartConfigurationDetected(CultureInfo culture) => ReportingResources.Get(nameof (NoValidChartConfigurationDetected), culture);

    public static string TitleCannotBeEmpty() => ReportingResources.Get(nameof (TitleCannotBeEmpty));

    public static string TitleCannotBeEmpty(CultureInfo culture) => ReportingResources.Get(nameof (TitleCannotBeEmpty), culture);

    public static string TransformOptionsEnumerableContainsANullTransformOptionsValue() => ReportingResources.Get(nameof (TransformOptionsEnumerableContainsANullTransformOptionsValue));

    public static string TransformOptionsEnumerableContainsANullTransformOptionsValue(
      CultureInfo culture)
    {
      return ReportingResources.Get(nameof (TransformOptionsEnumerableContainsANullTransformOptionsValue), culture);
    }

    public static string UnsupportedChartType(object arg0) => ReportingResources.Format(nameof (UnsupportedChartType), arg0);

    public static string UnsupportedChartType(object arg0, CultureInfo culture) => ReportingResources.Format(nameof (UnsupportedChartType), culture, arg0);

    public static string HistoryRangeLabel_LastYear() => ReportingResources.Get(nameof (HistoryRangeLabel_LastYear));

    public static string HistoryRangeLabel_LastYear(CultureInfo culture) => ReportingResources.Get(nameof (HistoryRangeLabel_LastYear), culture);

    public static string HistoryRangeLabel_Last12Weeks() => ReportingResources.Get(nameof (HistoryRangeLabel_Last12Weeks));

    public static string HistoryRangeLabel_Last12Weeks(CultureInfo culture) => ReportingResources.Get(nameof (HistoryRangeLabel_Last12Weeks), culture);

    public static string HistoryRangeLabel_Last4Weeks() => ReportingResources.Get(nameof (HistoryRangeLabel_Last4Weeks));

    public static string HistoryRangeLabel_Last4Weeks(CultureInfo culture) => ReportingResources.Get(nameof (HistoryRangeLabel_Last4Weeks), culture);

    public static string HistoryRangeLabel_Last2Weeks() => ReportingResources.Get(nameof (HistoryRangeLabel_Last2Weeks));

    public static string HistoryRangeLabel_Last2Weeks(CultureInfo culture) => ReportingResources.Get(nameof (HistoryRangeLabel_Last2Weeks), culture);

    public static string HistoryRangeLabel_Last7Days() => ReportingResources.Get(nameof (HistoryRangeLabel_Last7Days));

    public static string HistoryRangeLabel_Last7Days(CultureInfo culture) => ReportingResources.Get(nameof (HistoryRangeLabel_Last7Days), culture);

    public static string HistoryRangeLabel_Snapshot() => ReportingResources.Get(nameof (HistoryRangeLabel_Snapshot));

    public static string HistoryRangeLabel_Snapshot(CultureInfo culture) => ReportingResources.Get(nameof (HistoryRangeLabel_Snapshot), culture);

    public static string AggregationFunctionLabel_Sum() => ReportingResources.Get(nameof (AggregationFunctionLabel_Sum));

    public static string AggregationFunctionLabel_Sum(CultureInfo culture) => ReportingResources.Get(nameof (AggregationFunctionLabel_Sum), culture);

    public static string CFDDatesMustBeUTC() => ReportingResources.Get(nameof (CFDDatesMustBeUTC));

    public static string CFDDatesMustBeUTC(CultureInfo culture) => ReportingResources.Get(nameof (CFDDatesMustBeUTC), culture);

    public static string CfdTooManyDoneElements() => ReportingResources.Get(nameof (CfdTooManyDoneElements));

    public static string CfdTooManyDoneElements(CultureInfo culture) => ReportingResources.Get(nameof (CfdTooManyDoneElements), culture);

    public static string UnsupportedChartBackgroundColor(object arg0) => ReportingResources.Format(nameof (UnsupportedChartBackgroundColor), arg0);

    public static string UnsupportedChartBackgroundColor(object arg0, CultureInfo culture) => ReportingResources.Format(nameof (UnsupportedChartBackgroundColor), culture, arg0);

    public static string TooManyOptionsPerTransform() => ReportingResources.Get(nameof (TooManyOptionsPerTransform));

    public static string TooManyOptionsPerTransform(CultureInfo culture) => ReportingResources.Get(nameof (TooManyOptionsPerTransform), culture);
  }
}
