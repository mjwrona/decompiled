// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.WindowsProcessLock
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.VisualStudio.LocalLogger;
using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal sealed class WindowsProcessLock : IProcessLock, IDisposable
  {
    private readonly Mutex mutex;
    private readonly string mutexPath;

    public WindowsProcessLock(string name)
    {
      this.mutexPath = name;
      this.mutex = new Mutex(false, name);
    }

    public void Acquire(Action action, CancellationToken cancelToken)
    {
      try
      {
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WindowsProcessLock.Acquire() start for mutex name: {0}", new object[1]
        {
          (object) this.mutexPath
        }));
        if (!this.mutex.WaitOne() || cancelToken.IsCancellationRequested || action == null)
          return;
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Info, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WindowsProcessLock.Acquire() captured mutex, calls action()"));
        action();
      }
      catch (AbandonedMutexException ex)
      {
        CoreEventSource.Log.LogVerbose("Another process/thread abandon the Mutex, try to acquire it and become the active transmitter.");
        LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WindowsProcessLock.Acquire() exception happens: {0}", new object[1]
        {
          (object) ex.Message
        }));
        throw new ProcessLockException("Lock was abandoned", (Exception) ex, true);
      }
    }

    public void Dispose()
    {
      if (this.mutex == null)
        return;
      this.mutex.Dispose();
    }
  }
}
