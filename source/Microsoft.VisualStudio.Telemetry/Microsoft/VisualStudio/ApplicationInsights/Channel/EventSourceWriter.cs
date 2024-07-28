// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.EventSourceWriter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  public sealed class EventSourceWriter : IDisposable
  {
    private readonly string instrumentationKey;
    private readonly string providerName;
    private bool disposed;

    internal EventSourceWriter(string instrumentationKey, bool developerMode = false)
    {
      this.instrumentationKey = instrumentationKey;
      string str = EventSourceWriter.RemoveInvalidInstrumentationKeyChars(this.instrumentationKey.ToLowerInvariant());
      this.providerName = (developerMode ? "Microsoft.ApplicationInsights.Dev." : "Microsoft.ApplicationInsights.") + str;
    }

    internal Guid ProviderId => Guid.Empty;

    internal string ProviderName => this.providerName;

    internal string InstrumentationKey => this.instrumentationKey;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    internal void WriteTelemetry(ITelemetry telemetryItem)
    {
      if (telemetryItem != null)
        return;
      CoreEventSource.Log.LogVerbose("telemetryItem param is null in EventSourceWriter.WriteTelemetry()");
    }

    internal void WriteEvent<T>(string eventName, TelemetryContext context, T data)
    {
    }

    private static string RemoveInvalidInstrumentationKeyChars(string input) => new Regex("(?:[^a-z0-9.])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Replace(input, string.Empty);

    private void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      int num = disposing ? 1 : 0;
      this.disposed = true;
    }
  }
}
