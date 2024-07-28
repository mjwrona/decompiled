// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.LocalLogger.LocalFileLogger
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.LocalLogger
{
  public sealed class LocalFileLogger : ILocalFileLogger, IDisposable
  {
    private const string UnknownName = "unknown";
    private static int sequenceNumber;
    private readonly ITextWriterFactory textWriterFactory;
    private int disposeCount;
    private TextWriter writer;
    private bool isEnabled;

    public LocalFileLogger()
      : this(LocalFileLogger.GenerateLogFileName())
    {
    }

    public LocalFileLogger(string logFilePath)
      : this((ITextWriterFactory) new DefaultTextWriterFactory(), logFilePath)
    {
    }

    internal LocalFileLogger(ITextWriterFactory textWriterFactory, string logFilePath)
    {
      logFilePath.RequiresArgumentNotNullAndNotEmpty(nameof (logFilePath));
      textWriterFactory.RequiresArgumentNotNull<ITextWriterFactory>(nameof (textWriterFactory));
      this.FullLogPath = logFilePath;
      this.textWriterFactory = textWriterFactory;
    }

    public bool Enabled
    {
      get => this.isEnabled;
      set
      {
        if (this.disposeCount > 0 || this.isEnabled == value)
          return;
        if (value)
          this.CreateOrOpenFile();
        else
          this.CloseFile();
        this.isEnabled = value;
      }
    }

    public string FullLogPath { get; }

    public void Log(LocalLoggerSeverity severity, string componentId, string text)
    {
      if (this.disposeCount > 0 || this.writer == null || string.IsNullOrEmpty(text))
        return;
      if (string.IsNullOrEmpty(componentId))
        componentId = "unknown";
      try
      {
        this.writer?.WriteLineAsync(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0:yyyy-MM-dd HH:mm:ss.fff}]\t{1}\t{2}\t{3}\t{4}", (object) DateTime.Now, (object) NativeMethods.GetCurrentThreadId(), (object) severity.ToString(), (object) componentId, (object) text));
      }
      catch
      {
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    internal static string GenerateLogFileName()
    {
      string tempPath = System.IO.Path.GetTempPath();
      string lowerInvariant = System.IO.Path.GetFileNameWithoutExtension(NativeMethods.GetFullProcessExeName() ?? "unknown").ToLowerInvariant();
      int num = Interlocked.Increment(ref LocalFileLogger.sequenceNumber);
      string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "vstel_{0:yyyyMMddHHmmss}_{1}_{2}_{3}.log", (object) DateTime.Now, (object) lowerInvariant, (object) NativeMethods.GetCurrentProcessId(), (object) num);
      return System.IO.Path.Combine(tempPath, path2);
    }

    private void Dispose(bool disposing)
    {
      if (Interlocked.Increment(ref this.disposeCount) != 1)
        return;
      this.CloseFile();
    }

    private void CreateOrOpenFile()
    {
      try
      {
        this.writer = this.textWriterFactory.CreateTextWriter(this.FullLogPath);
      }
      catch
      {
      }
    }

    private void CloseFile()
    {
      this.writer?.Dispose();
      this.writer = (TextWriter) null;
    }
  }
}
