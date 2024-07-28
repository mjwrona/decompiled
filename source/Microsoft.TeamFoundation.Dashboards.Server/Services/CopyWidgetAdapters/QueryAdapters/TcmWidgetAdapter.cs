// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.CopyWidgetAdapters.QueryAdapters.TcmWidgetAdapter
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Microsoft.TeamFoundation.Dashboards.Services.CopyWidgetAdapters.QueryAdapters
{
  public class TcmWidgetAdapter : WidgetAdapterBase
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
        if (nullable1.HasValue)
        {
          nullable1 = chartConfiguration1.ChartId;
          if (nullable1.HasValue)
            goto label_5;
        }
        if (chartConfiguration1.TransformOptions == null)
        {
          widget1.Settings = (string) null;
          goto label_11;
        }
label_5:
        nullable1 = chartConfiguration1.ChartId;
        if (nullable1.HasValue)
        {
          nullable1 = chartConfiguration1.ChartId;
          if (nullable1.HasValue && chartConfiguration1.TransformOptions == null && chartConfiguration1.GroupKey == null)
          {
            ChartConfiguration chartConfiguration2 = new ChartConfiguration();
            try
            {
              ChartConfigurationService service = requestContext.GetService<ChartConfigurationService>();
              IVssRequestContext requestContext1 = requestContext;
              Guid dataspaceId = sourceDashboardConsumer.GetDataspaceId();
              nullable1 = chartConfiguration1.ChartId;
              Guid id = nullable1.Value;
              ChartConfiguration chartConfiguration3 = service.GetChartConfiguration(requestContext1, dataspaceId, id);
              ChartConfiguration chartConfiguration4 = chartConfiguration3;
              nullable1 = new Guid?();
              Guid? nullable2 = nullable1;
              chartConfiguration4.ChartId = nullable2;
              widget1.Settings = JsonConvert.SerializeObject((object) chartConfiguration3, new JsonSerializerSettings()
              {
                ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
              });
            }
            catch (ChartConfigurationDoesNotExistException ex)
            {
              requestContext.TraceException(10017701, "Dashboard", nameof (TcmWidgetAdapter), (Exception) ex);
              widget1.Settings = (string) null;
            }
          }
        }
      }
      else
        widget1.Settings = (string) null;
label_11:
      return widget1;
    }
  }
}
