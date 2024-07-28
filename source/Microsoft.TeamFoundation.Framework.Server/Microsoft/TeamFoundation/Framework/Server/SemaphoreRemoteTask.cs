// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SemaphoreRemoteTask
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SemaphoreRemoteTask : IDisposable
  {
    private Task m_task;
    private readonly Action<Exception> m_logException;
    private CancellationTokenSource m_cancellationTokenSource = new CancellationTokenSource();

    public SemaphoreRemoteTask(
      string semaphoreName,
      Action work,
      TimeSpan checkInterval,
      TimeSpan minInterval,
      Action<Exception> logException)
    {
      this.m_task = this.WaitForSignal(work, SemaphoreRemoteTask.GetSemaphoreName(semaphoreName), this.m_cancellationTokenSource.Token, checkInterval, minInterval);
      this.m_logException = logException;
    }

    public static string GetSemaphoreName(string semaphorePrefix)
    {
      using (Process currentProcess = Process.GetCurrentProcess())
        return string.Format("Global\\{0}_{1}_{2}", (object) semaphorePrefix, (object) currentProcess.ProcessName, (object) currentProcess.Id);
    }

    private void LogException(Exception exception)
    {
      try
      {
        Action<Exception> logException = this.m_logException;
        if (logException == null)
          return;
        logException(exception);
      }
      catch (Exception ex)
      {
      }
    }

    private async Task WaitForSignal(
      Action action,
      string semaphoreName,
      CancellationToken cancellationToken,
      TimeSpan checkInterval,
      TimeSpan minInterval)
    {
      using (Semaphore semaphore = new Semaphore(0, 1, semaphoreName))
      {
        while (!cancellationToken.IsCancellationRequested)
        {
          TimeSpan delay;
          if (semaphore.WaitOne(TimeSpan.Zero))
          {
            try
            {
              action();
            }
            catch (Exception ex)
            {
              this.LogException(ex);
            }
            finally
            {
              delay = minInterval;
            }
          }
          else
            delay = checkInterval;
          await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
      }
    }

    public void Dispose()
    {
      if (this.m_cancellationTokenSource == null)
        return;
      try
      {
        this.m_cancellationTokenSource.Cancel();
        this.m_task?.Wait();
      }
      catch (AggregateException ex)
      {
        foreach (Exception innerException in ex.InnerExceptions)
        {
          switch (innerException)
          {
            case System.OperationCanceledException _:
            case TaskCanceledException _:
              continue;
            default:
              this.LogException((Exception) ex);
              continue;
          }
        }
      }
      finally
      {
        this.m_cancellationTokenSource.Dispose();
        this.m_cancellationTokenSource = (CancellationTokenSource) null;
      }
      this.m_task = (Task) null;
    }
  }
}
