// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Features.Charts.ChartsResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Features.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 49538BC1-A38B-4EF6-AE82-2B8AD0FFF17F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Features.Charts.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.Features.Charts
{
  internal static class ChartsResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ChartsResources), typeof (ChartsResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ChartsResources.s_resMgr;

    private static string Get(string resourceName) => ChartsResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ChartsResources.Get(resourceName) : ChartsResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ChartsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ChartsResources.GetInt(resourceName) : (int) ChartsResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ChartsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ChartsResources.GetBool(resourceName) : (bool) ChartsResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ChartsResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ChartsResources.Get(resourceName, culture);
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

    public static string AccessibilityLabelPrefixFormat(object arg0, object arg1) => ChartsResources.Format(nameof (AccessibilityLabelPrefixFormat), arg0, arg1);

    public static string AccessibilityLabelPrefixFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ChartsResources.Format(nameof (AccessibilityLabelPrefixFormat), culture, arg0, arg1);
    }

    public static string AccessibilityLabelElementSeparatorFormat(object arg0, object arg1) => ChartsResources.Format(nameof (AccessibilityLabelElementSeparatorFormat), arg0, arg1);

    public static string AccessibilityLabelElementSeparatorFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ChartsResources.Format(nameof (AccessibilityLabelElementSeparatorFormat), culture, arg0, arg1);
    }

    public static string ChartEditor_AvailableScreenreader_Format(object arg0, object arg1) => ChartsResources.Format(nameof (ChartEditor_AvailableScreenreader_Format), arg0, arg1);

    public static string ChartEditor_AvailableScreenreader_Format(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ChartsResources.Format(nameof (ChartEditor_AvailableScreenreader_Format), culture, arg0, arg1);
    }

    public static string ChartsRemainingItems() => ChartsResources.Get(nameof (ChartsRemainingItems));

    public static string ChartsRemainingItems(CultureInfo culture) => ChartsResources.Get(nameof (ChartsRemainingItems), culture);

    public static string ChartType_A11yLabel_Area() => ChartsResources.Get(nameof (ChartType_A11yLabel_Area));

    public static string ChartType_A11yLabel_Area(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_Area), culture);

    public static string ChartType_A11yLabel_AreaRange() => ChartsResources.Get(nameof (ChartType_A11yLabel_AreaRange));

    public static string ChartType_A11yLabel_AreaRange(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_AreaRange), culture);

    public static string ChartType_A11yLabel_Bar() => ChartsResources.Get(nameof (ChartType_A11yLabel_Bar));

    public static string ChartType_A11yLabel_Bar(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_Bar), culture);

    public static string ChartType_A11yLabel_Column() => ChartsResources.Get(nameof (ChartType_A11yLabel_Column));

    public static string ChartType_A11yLabel_Column(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_Column), culture);

    public static string ChartType_A11yLabel_ColumnLine() => ChartsResources.Get(nameof (ChartType_A11yLabel_ColumnLine));

    public static string ChartType_A11yLabel_ColumnLine(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_ColumnLine), culture);

    public static string ChartType_A11yLabel_Histogram() => ChartsResources.Get(nameof (ChartType_A11yLabel_Histogram));

    public static string ChartType_A11yLabel_Histogram(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_Histogram), culture);

    public static string ChartType_A11yLabel_Hybrid() => ChartsResources.Get(nameof (ChartType_A11yLabel_Hybrid));

    public static string ChartType_A11yLabel_Hybrid(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_Hybrid), culture);

    public static string ChartType_A11yLabel_Line() => ChartsResources.Get(nameof (ChartType_A11yLabel_Line));

    public static string ChartType_A11yLabel_Line(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_Line), culture);

    public static string ChartType_A11yLabel_Pie() => ChartsResources.Get(nameof (ChartType_A11yLabel_Pie));

    public static string ChartType_A11yLabel_Pie(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_Pie), culture);

    public static string ChartType_A11yLabel_Scatter() => ChartsResources.Get(nameof (ChartType_A11yLabel_Scatter));

    public static string ChartType_A11yLabel_Scatter(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_Scatter), culture);

    public static string ChartType_A11yLabel_StackedArea() => ChartsResources.Get(nameof (ChartType_A11yLabel_StackedArea));

    public static string ChartType_A11yLabel_StackedArea(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_StackedArea), culture);

    public static string ChartType_A11yLabel_StackedBar() => ChartsResources.Get(nameof (ChartType_A11yLabel_StackedBar));

    public static string ChartType_A11yLabel_StackedBar(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_StackedBar), culture);

    public static string ChartType_A11yLabel_StackedColumn() => ChartsResources.Get(nameof (ChartType_A11yLabel_StackedColumn));

    public static string ChartType_A11yLabel_StackedColumn(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_StackedColumn), culture);

    public static string ChartType_A11yLabel_Table() => ChartsResources.Get(nameof (ChartType_A11yLabel_Table));

    public static string ChartType_A11yLabel_Table(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_Table), culture);

    public static string HtmlTable_DateLabel() => ChartsResources.Get(nameof (HtmlTable_DateLabel));

    public static string HtmlTable_DateLabel(CultureInfo culture) => ChartsResources.Get(nameof (HtmlTable_DateLabel), culture);

    public static string HtmlTable_DurationLabel() => ChartsResources.Get(nameof (HtmlTable_DurationLabel));

    public static string HtmlTable_DurationLabel(CultureInfo culture) => ChartsResources.Get(nameof (HtmlTable_DurationLabel), culture);

    public static string HtmlTable_SampleLabel() => ChartsResources.Get(nameof (HtmlTable_SampleLabel));

    public static string HtmlTable_SampleLabel(CultureInfo culture) => ChartsResources.Get(nameof (HtmlTable_SampleLabel), culture);

    public static string HtmlTable_SeriesLabel(object arg0) => ChartsResources.Format(nameof (HtmlTable_SeriesLabel), arg0);

    public static string HtmlTable_SeriesLabel(object arg0, CultureInfo culture) => ChartsResources.Format(nameof (HtmlTable_SeriesLabel), culture, arg0);

    public static string ChartType_A11yLabel_Funnel() => ChartsResources.Get(nameof (ChartType_A11yLabel_Funnel));

    public static string ChartType_A11yLabel_Funnel(CultureInfo culture) => ChartsResources.Get(nameof (ChartType_A11yLabel_Funnel), culture);

    public static string ChartEditor_AvailableScreenreader_NoTitle_Format(object arg0) => ChartsResources.Format(nameof (ChartEditor_AvailableScreenreader_NoTitle_Format), arg0);

    public static string ChartEditor_AvailableScreenreader_NoTitle_Format(
      object arg0,
      CultureInfo culture)
    {
      return ChartsResources.Format(nameof (ChartEditor_AvailableScreenreader_NoTitle_Format), culture, arg0);
    }
  }
}
