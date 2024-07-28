// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryTextWriter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryTextWriter : ITelemetryWriter, IDisposable
  {
    private TextWriter writer;

    public TelemetryTextWriter(string filePath)
    {
      try
      {
        StreamWriter text = ReparsePointAware.CreateText(filePath);
        text.AutoFlush = true;
        this.writer = TextWriter.Synchronized((TextWriter) text);
      }
      catch (UnauthorizedAccessException ex)
      {
      }
    }

    public async Task WriteLineAsync(string text)
    {
      if (this.writer == null)
        return;
      if (this.writer == StreamWriter.Null)
        return;
      try
      {
        await this.writer.WriteLineAsync(text);
      }
      catch (ObjectDisposedException ex)
      {
      }
      catch (InvalidOperationException ex)
      {
      }
    }

    public void Dispose()
    {
      TextWriter writer = this.writer;
      if (writer == null || writer == StreamWriter.Null || Interlocked.CompareExchange<TextWriter>(ref this.writer, (TextWriter) StreamWriter.Null, writer) != writer)
        return;
      writer.Dispose();
    }
  }
}
