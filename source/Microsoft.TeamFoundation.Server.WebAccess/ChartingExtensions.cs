// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ChartingExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class ChartingExtensions
  {
    public static MvcHtmlString ChartTile(
      this HtmlHelper htmlHelper,
      object model,
      string controlClassName)
    {
      TagBuilder tagBuilder = new TagBuilder("div");
      if (!string.IsNullOrEmpty(controlClassName))
        tagBuilder.MergeAttribute("class", controlClassName);
      if (model != null)
        tagBuilder.InnerHtml = ChartingExtensions.CreateChartJson(htmlHelper, (ChartConfigurationBundle) model);
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static string CreateChartJson(
      HtmlHelper htmlHelper,
      ChartConfigurationBundle chartConfigurationBundle)
    {
      ChartConfiguration chartConfiguration = chartConfigurationBundle.ChartConfiguration;
      return htmlHelper.JsonIsland((object) new
      {
        chartConfiguration = new
        {
          chartId = chartConfiguration.ChartId,
          chartType = chartConfiguration.ChartType,
          groupKey = chartConfiguration.GroupKey,
          scope = chartConfiguration.Scope,
          title = chartConfiguration.Title,
          transformOptions = ChartingExtensions.PackTransformOptions(chartConfiguration.TransformOptions),
          userColors = ChartingExtensions.PackUserColors(chartConfiguration)
        },
        uri = chartConfigurationBundle.Uri,
        chartHostListSettings = chartConfigurationBundle.chartPinningState
      }, (object) new{ @class = "options" }).ToString();
    }

    private static object PackTransformOptions(TransformOptions transformOptions) => (object) new
    {
      filter = transformOptions.Filter,
      historyRange = transformOptions.HistoryRange,
      filterContext = transformOptions.FilterContext,
      groupBy = transformOptions.GroupBy,
      measure = transformOptions.Measure,
      orderBy = transformOptions.OrderBy,
      series = transformOptions.Series,
      transformId = transformOptions.TransformId
    };

    private static object PackUserColors(ChartConfiguration chartConfiguration)
    {
      object obj = (object) null;
      if (chartConfiguration.UserColors != null)
      {
        List<ChartingExtensions.CustomColorConfiguration> colorConfigurationList = new List<ChartingExtensions.CustomColorConfiguration>();
        foreach (ColorConfiguration userColor in chartConfiguration.UserColors)
          colorConfigurationList.Add(new ChartingExtensions.CustomColorConfiguration()
          {
            backgroundColor = userColor.BackgroundColor,
            chartId = userColor.ChartId,
            value = userColor.Value
          });
        obj = (object) colorConfigurationList;
      }
      return obj;
    }

    public class CustomColorConfiguration
    {
      public string backgroundColor { get; set; }

      public Guid? chartId { get; set; }

      public string value { get; set; }
    }
  }
}
