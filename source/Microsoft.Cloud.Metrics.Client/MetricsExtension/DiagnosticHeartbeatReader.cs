// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsExtension.DiagnosticHeartbeatReader
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Logging;
using Microsoft.Cloud.Metrics.Client.Metrics.Etw;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.MetricsExtension
{
  public sealed class DiagnosticHeartbeatReader
  {
    private const string EtwSessionsPrefix = "DiagnosticHeartbeatReader-";
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (DiagnosticHeartbeatReader));
    private static readonly Guid ProviderGuid = new Guid("{2F23A2A9-0DE7-4CB4-A778-FBDF5C1E7372}");

    public bool EnableVerboseLogging { get; set; }

    public Task ReadDiagnosticHeartbeatsAsync(
      Action<IDiagnosticHeartbeat> heartbeatAction,
      CancellationToken cancellationToken)
    {
      if (heartbeatAction == null)
        throw new ArgumentNullException(nameof (heartbeatAction));
      if (!this.EnableVerboseLogging)
        Logger.SetMaxLogLevel(LoggerLevel.Error);
      ActiveCollector activeCollector = (ActiveCollector) null;
      Task task = Task.Factory.StartNew((Action) (() =>
      {
        try
        {
          Task.Factory.StartNew((Action) (() => DiagnosticHeartbeatReader.SetupListener(DiagnosticHeartbeatReader.ProviderGuid, heartbeatAction, cancellationToken, out activeCollector)), TaskCreationOptions.LongRunning);
          cancellationToken.WaitHandle.WaitOne();
        }
        finally
        {
          DiagnosticHeartbeatReader.StopEtwSession(activeCollector);
        }
      }), TaskCreationOptions.LongRunning);
      Console.CancelKeyPress += (ConsoleCancelEventHandler) ((sender, args) => DiagnosticHeartbeatReader.StopEtwSession(activeCollector));
      return task;
    }

    private static void StopEtwSession(ActiveCollector activeCollector)
    {
      if (activeCollector == null)
        return;
      ActiveCollector.StopCollector(activeCollector.Name);
    }

    private static void SetupListener(
      Guid providerGuid,
      Action<IDiagnosticHeartbeat> action,
      CancellationToken cancellationToken,
      out ActiveCollector activeCollector)
    {
      Dictionary<Guid, ProviderConfiguration> dictionary = new Dictionary<Guid, ProviderConfiguration>()
      {
        {
          providerGuid,
          new ProviderConfiguration(providerGuid, EtwTraceLevel.Verbose, 0L, 0L)
        }
      };
      CollectorConfiguration config = new CollectorConfiguration("DiagnosticHeartbeatReader--")
      {
        SessionType = SessionType.Realtime,
        Providers = dictionary
      };
      activeCollector = new ActiveCollector(config.Name);
      activeCollector.StartCollector(config);
      RawListener rawListener = (RawListener) null;
      try
      {
        rawListener = DiagnosticHeartbeatReader.CreateRealTimeListener(providerGuid, config.Name, action, 2, cancellationToken);
        if (!ActiveCollector.TryUpdateProviders(config))
        {
          Logger.Log(LoggerLevel.Error, DiagnosticHeartbeatReader.LogId, "Main", "Failed to update ETW providers. Terminating.");
        }
        else
        {
          try
          {
            rawListener.Process();
          }
          finally
          {
            Logger.Log(cancellationToken.IsCancellationRequested ? LoggerLevel.Info : LoggerLevel.Error, DiagnosticHeartbeatReader.LogId, "SetupEtwDataPipeline", "ETW Thread terminated unexpectedly, typically indicates that the ETW session was stopped.");
          }
        }
      }
      finally
      {
        rawListener?.Dispose();
      }
    }

    private static unsafe RawListener CreateRealTimeListener(
      Guid providerGuid,
      string etlSessionConfigName,
      Action<IDiagnosticHeartbeat> action,
      int eventIdFilter,
      CancellationToken cancellationToken)
    {
      return RawListener.CreateRealTimeListener(etlSessionConfigName, (NativeMethods.EventRecordCallback) (eventRecord =>
      {
        if (!(eventRecord->EventHeader.ProviderId == providerGuid) || (int) eventRecord->EventHeader.Id != eventIdFilter)
          return;
        action(DiagnosticHeartbeat.FromEtwEvent(eventRecord));
      }), (NativeMethods.EventTraceBufferCallback) (eventTraceLog =>
      {
        Logger.Log(LoggerLevel.Info, DiagnosticHeartbeatReader.LogId, nameof (CreateRealTimeListener), "DiagnosticHeartbeat, cancelled = {0}", (object) cancellationToken.IsCancellationRequested);
        return !cancellationToken.IsCancellationRequested;
      }));
    }
  }
}
