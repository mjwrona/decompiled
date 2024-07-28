// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.BaseJsonLogFile`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Telemetry
{
  internal abstract class BaseJsonLogFile<T> : TelemetryDisposableObject, ITelemetryLogFile<T>
  {
    private readonly object telemetryWriterLocker = new object();
    private ITelemetryWriter telemetryWriter;
    private bool writeComma;
    private bool isInitialized;
    protected ITelemetryLogSettingsProvider settingsProvider;

    public BaseJsonLogFile(ITelemetryWriter writer = null) => this.telemetryWriter = writer;

    public void Initialize(ITelemetryLogSettingsProvider settingsProvider)
    {
      if (this.isInitialized)
        return;
      lock (this.telemetryWriterLocker)
      {
        if (this.isInitialized)
          return;
        settingsProvider.RequiresArgumentNotNull<ITelemetryLogSettingsProvider>(nameof (settingsProvider));
        this.settingsProvider = settingsProvider;
        if (this.telemetryWriter == null)
          this.telemetryWriter = (ITelemetryWriter) new TelemetryTextWriter(this.settingsProvider.FilePath);
        this.WriteHeader();
        this.isInitialized = true;
      }
    }

    public void WriteAsync(T eventData)
    {
      if (!this.isInitialized)
        throw new InvalidOperationException("JsonLogFile is not initialized");
      if (this.telemetryWriter == null)
        return;
      lock (this.telemetryWriterLocker)
      {
        if (this.telemetryWriter == null)
          return;
        if (this.writeComma)
          this.telemetryWriter.WriteLineAsync(",");
        this.writeComma = true;
        this.telemetryWriter.WriteLineAsync(this.ConvertEventToString(eventData));
      }
    }

    protected abstract string ConvertEventToString(T eventData);

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      if (this.telemetryWriter == null)
        return;
      lock (this.telemetryWriterLocker)
      {
        if (this.telemetryWriter == null)
          return;
        this.WriteFooter();
        this.telemetryWriter.Dispose();
        this.telemetryWriter = (ITelemetryWriter) null;
      }
    }

    private void WriteHeader()
    {
      this.telemetryWriter.WriteLineAsync("{");
      foreach (KeyValuePair<string, string> mainIdentifier in this.settingsProvider.MainIdentifiers)
        this.telemetryWriter.WriteLineAsync(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\":\"{1}\",", new object[2]
        {
          (object) mainIdentifier.Key,
          (object) mainIdentifier.Value
        }));
      this.telemetryWriter.WriteLineAsync("\"events\":[");
    }

    private void WriteFooter()
    {
      this.telemetryWriter.WriteLineAsync("]");
      this.telemetryWriter.WriteLineAsync("}");
    }
  }
}
