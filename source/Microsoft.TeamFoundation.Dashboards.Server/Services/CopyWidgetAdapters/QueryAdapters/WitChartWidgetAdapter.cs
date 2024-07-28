// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.CopyWidgetAdapters.QueryAdapters.WitChartWidgetAdapter
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Microsoft.TeamFoundation.Dashboards.Services.CopyWidgetAdapters.QueryAdapters
{
  public class WitChartWidgetAdapter : WidgetAdapterBase
  {
    public override Widget ProcessAndCopyWidget(
      Widget widget,
      IVssRequestContext requestContext,
      IDashboardConsumer sourceDashboardConsumer,
      IDashboardConsumer targetDashboardConsumer,
      bool purgeSettings = false,
      CopyDashboardParameterHandler copyDashboardParameterHandler = null)
    {
      Widget widget1 = base.ProcessAndCopyWidget(widget, requestContext, sourceDashboardConsumer, targetDashboardConsumer, purgeSettings, copyDashboardParameterHandler);
      ChartConfiguration chartConfiguration1;
      if (widget1.Settings != null && JsonUtilities.TryDeserialize<ChartConfiguration>(widget1.Settings, out chartConfiguration1, true))
      {
        Guid? nullable1 = chartConfiguration1.ChartId;
        if (!nullable1.HasValue)
        {
          nullable1 = chartConfiguration1.ChartId;
          if (!nullable1.HasValue && chartConfiguration1.GroupKey == null && chartConfiguration1.TransformOptions == null)
          {
            widget1.Settings = (string) null;
            return widget1;
          }
        }
        nullable1 = chartConfiguration1.ChartId;
        if (nullable1.HasValue)
        {
          nullable1 = chartConfiguration1.ChartId;
          if (nullable1.HasValue && chartConfiguration1.TransformOptions == null && chartConfiguration1.GroupKey == null)
          {
            ChartConfigurationService service = requestContext.GetService<ChartConfigurationService>();
            IVssRequestContext requestContext1 = requestContext;
            Guid dataspaceId = sourceDashboardConsumer.GetDataspaceId();
            nullable1 = chartConfiguration1.ChartId;
            Guid id = nullable1.Value;
            chartConfiguration1 = service.GetChartConfiguration(requestContext1, dataspaceId, id);
            ChartConfiguration chartConfiguration2 = chartConfiguration1;
            nullable1 = new Guid?();
            Guid? nullable2 = nullable1;
            chartConfiguration2.ChartId = nullable2;
            goto label_9;
          }
        }
        if (chartConfiguration1 != null)
        {
          ChartConfiguration chartConfiguration3 = chartConfiguration1;
          nullable1 = new Guid?();
          Guid? nullable3 = nullable1;
          chartConfiguration3.ChartId = nullable3;
        }
label_9:
        Guid result;
        if (Guid.TryParse(chartConfiguration1.GroupKey, out result))
        {
          try
          {
            QueryHierarchyItem query = copyDashboardParameterHandler.GetOrCreateQuery(requestContext, sourceDashboardConsumer, targetDashboardConsumer, result);
            chartConfiguration1.TransformOptions.Filter = query.Id.ToString();
            chartConfiguration1.GroupKey = query.Id.ToString();
          }
          catch (QueryItemNotFoundException ex)
          {
            requestContext.TraceException(10017704, "Dashboard", nameof (WitChartWidgetAdapter), (Exception) ex);
            chartConfiguration1.TransformOptions.Filter = (string) null;
            chartConfiguration1.GroupKey = (string) null;
          }
        }
        else
        {
          chartConfiguration1.TransformOptions.Filter = (string) null;
          chartConfiguration1.GroupKey = (string) null;
        }
        widget1.Settings = JsonConvert.SerializeObject((object) chartConfiguration1, new JsonSerializerSettings()
        {
          ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
        });
      }
      else
        widget1.Settings = (string) null;
      return widget1;
    }
  }
}
