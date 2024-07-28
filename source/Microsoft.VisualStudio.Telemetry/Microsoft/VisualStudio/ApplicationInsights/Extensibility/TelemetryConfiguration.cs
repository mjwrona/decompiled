// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.TelemetryConfiguration
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility
{
  public sealed class TelemetryConfiguration : IDisposable
  {
    private static object syncRoot = new object();
    private static TelemetryConfiguration active;
    private readonly SnapshottingList<IContextInitializer> contextInitializers = new SnapshottingList<IContextInitializer>();
    private readonly SnapshottingList<ITelemetryInitializer> telemetryInitializers = new SnapshottingList<ITelemetryInitializer>();
    private readonly SnapshottingList<object> telemetryModules = new SnapshottingList<object>();
    private string instrumentationKey = string.Empty;
    private bool disableTelemetry;

    public static TelemetryConfiguration Active
    {
      get
      {
        if (TelemetryConfiguration.active == null)
        {
          lock (TelemetryConfiguration.syncRoot)
          {
            if (TelemetryConfiguration.active == null)
            {
              TelemetryConfiguration.active = new TelemetryConfiguration();
              TelemetryConfigurationFactory.Instance.Initialize(TelemetryConfiguration.active);
            }
          }
        }
        return TelemetryConfiguration.active;
      }
      internal set
      {
        lock (TelemetryConfiguration.syncRoot)
          TelemetryConfiguration.active = value;
      }
    }

    public string InstrumentationKey
    {
      get => this.instrumentationKey;
      set => this.instrumentationKey = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public bool DisableTelemetry
    {
      get => this.disableTelemetry;
      set
      {
        if (value)
          CoreEventSource.Log.TrackingWasDisabled();
        else
          CoreEventSource.Log.TrackingWasEnabled();
        this.disableTelemetry = value;
      }
    }

    public IList<IContextInitializer> ContextInitializers => (IList<IContextInitializer>) this.contextInitializers;

    public IList<ITelemetryInitializer> TelemetryInitializers => (IList<ITelemetryInitializer>) this.telemetryInitializers;

    public IList<object> TelemetryModules => (IList<object>) this.telemetryModules;

    public ITelemetryChannel TelemetryChannel { get; set; }

    public static TelemetryConfiguration CreateDefault()
    {
      TelemetryConfiguration configuration = new TelemetryConfiguration();
      TelemetryConfigurationFactory.Instance.Initialize(configuration);
      return configuration;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      Interlocked.CompareExchange<TelemetryConfiguration>(ref TelemetryConfiguration.active, (TelemetryConfiguration) null, this);
      this.TelemetryChannel?.Dispose();
    }
  }
}
