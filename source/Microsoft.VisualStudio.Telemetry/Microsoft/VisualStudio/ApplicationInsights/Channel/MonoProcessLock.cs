// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.MonoProcessLock
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Threading;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal sealed class MonoProcessLock : IProcessLock, IDisposable
  {
    private readonly FileBasedMutex mutex;

    public MonoProcessLock(string name) => this.mutex = new FileBasedMutex(name);

    public void Acquire(Action action, CancellationToken cancelToken)
    {
      try
      {
        if (!this.mutex.AcquireMutex(cancelToken))
          throw new ProcessLockException("Cannot acquire file-based mutex");
        if (cancelToken.IsCancellationRequested || action == null)
          return;
        action();
      }
      catch (Exception ex)
      {
        throw new ProcessLockException("Exception while acquiring file-based mutext", ex);
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
