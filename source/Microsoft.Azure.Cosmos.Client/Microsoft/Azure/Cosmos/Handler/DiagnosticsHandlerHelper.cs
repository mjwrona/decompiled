// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Handler.DiagnosticsHandlerHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Documents.Rntbd;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Handler
{
  internal class DiagnosticsHandlerHelper
  {
    private const string Diagnostickey = "diagnostic";
    private const string Telemetrykey = "telemetry";
    public static readonly TimeSpan DiagnosticsRefreshInterval = TimeSpan.FromSeconds(10.0);
    private readonly SystemUsageRecorder diagnosticSystemUsageRecorder = new SystemUsageRecorder("diagnostic", 6, DiagnosticsHandlerHelper.DiagnosticsRefreshInterval);
    private static readonly TimeSpan ClientTelemetryRefreshInterval = TimeSpan.FromSeconds(5.0);
    private readonly SystemUsageRecorder telemetrySystemUsageRecorder = new SystemUsageRecorder("telemetry", 120, DiagnosticsHandlerHelper.ClientTelemetryRefreshInterval);
    private static bool isDiagnosticsMonitoringEnabled = false;
    private static bool isTelemetryMonitoringEnabled = false;
    public static readonly DiagnosticsHandlerHelper Instance = new DiagnosticsHandlerHelper();
    private readonly SystemUsageMonitor systemUsageMonitor;

    private DiagnosticsHandlerHelper()
    {
      DiagnosticsHandlerHelper.isDiagnosticsMonitoringEnabled = false;
      try
      {
        DiagnosticsHandlerHelper.isTelemetryMonitoringEnabled = ClientTelemetryOptions.IsClientTelemetryEnabled();
        List<SystemUsageRecorder> usageRecorders = new List<SystemUsageRecorder>()
        {
          this.diagnosticSystemUsageRecorder
        };
        if (DiagnosticsHandlerHelper.isTelemetryMonitoringEnabled)
          usageRecorders.Add(this.telemetrySystemUsageRecorder);
        this.systemUsageMonitor = SystemUsageMonitor.CreateAndStart((IReadOnlyList<SystemUsageRecorder>) usageRecorders);
        DiagnosticsHandlerHelper.isDiagnosticsMonitoringEnabled = true;
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError(ex.Message);
        DiagnosticsHandlerHelper.isDiagnosticsMonitoringEnabled = false;
        DiagnosticsHandlerHelper.isTelemetryMonitoringEnabled = false;
      }
    }

    public SystemUsageHistory GetDiagnosticsSystemHistory()
    {
      if (!DiagnosticsHandlerHelper.isDiagnosticsMonitoringEnabled)
        return (SystemUsageHistory) null;
      try
      {
        return this.diagnosticSystemUsageRecorder.Data;
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError(ex.Message);
        DiagnosticsHandlerHelper.isDiagnosticsMonitoringEnabled = false;
        return (SystemUsageHistory) null;
      }
    }

    public SystemUsageHistory GetClientTelemetrySystemHistory()
    {
      if (!DiagnosticsHandlerHelper.isTelemetryMonitoringEnabled)
        return (SystemUsageHistory) null;
      try
      {
        return this.telemetrySystemUsageRecorder.Data;
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError(ex.Message);
        DiagnosticsHandlerHelper.isTelemetryMonitoringEnabled = false;
        return (SystemUsageHistory) null;
      }
    }
  }
}
