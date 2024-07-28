// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.DashboardFeatureMonitoringService
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.DataAccess;
using Microsoft.TeamFoundation.Dashboards.Telemetry;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  internal class DashboardFeatureMonitoringService : 
    IDashboardFeatureMonitoringService,
    IVssFrameworkService
  {
    public void SummarizeState(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.IsSystemContext)
        throw new InvalidOperationException("SummarizeState must be called with a system context.");
      using (TelemetryCollector telemetryCollector = TelemetryCollector.TraceMonitor(systemRequestContext, 10017500, nameof (DashboardFeatureMonitoringService), "DashboardFeatureMonitoringService.SummaryStats"))
      {
        using (DashboardMonitoringSqlResourceComponent component = systemRequestContext.CreateComponent<DashboardMonitoringSqlResourceComponent>())
        {
          telemetryCollector.Properties["PartitionId"] = systemRequestContext.ServiceHost.PartitionId.ToString();
          List<DashboardsMonitoringRecord> summaryStats = component.GetSummaryStats();
          this.PackStats(systemRequestContext, summaryStats, telemetryCollector);
          string json = this.CondenseToJson(systemRequestContext, component.GetWidgetPopularity());
          telemetryCollector.Properties["WidgetPopularity"] = json;
        }
      }
    }

    private void PackStats(
      IVssRequestContext systemRequestContext,
      List<DashboardsMonitoringRecord> monitoringRecords,
      TelemetryCollector telemetryCollector)
    {
      foreach (DashboardsMonitoringRecord monitoringRecord in monitoringRecords)
      {
        string measurementName = monitoringRecord.MeasurementName;
        int measurementValue = monitoringRecord.MeasurementValue;
        telemetryCollector.Properties[measurementName] = measurementValue.ToString();
      }
    }

    private string CondenseToJson(
      IVssRequestContext systemRequestContext,
      List<DashboardsMonitoringRecord> monitoringRecords)
    {
      return JsonConvert.SerializeObject((object) monitoringRecords, Formatting.Indented, new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      });
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
