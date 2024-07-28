// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TaskCancellationExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class TaskCancellationExtensions
  {
    private const int RUN_CONTINUATIONS_ASYNCHRONOUSLY = 64;

    public static Task EnforceCancellation(
      this Task task,
      CancellationToken cancellationToken,
      Func<string> makeMessage = null,
      [CallerFilePath] string file = "",
      [CallerMemberName] string member = "",
      [CallerLineNumber] int line = -1)
    {
      return (Task) ((Func<Task<TaskCancellationExtensions.Void>>) (async () =>
      {
        await task.ConfigureAwait(false);
        return new TaskCancellationExtensions.Void();
      }))().EnforceCancellation<TaskCancellationExtensions.Void>(cancellationToken, makeMessage, file, member, line);
    }

    public static async Task<TResult> EnforceCancellation<TResult>(
      this Task<TResult> task,
      CancellationToken cancellationToken,
      Func<string> makeMessage = null,
      [CallerFilePath] string file = "",
      [CallerMemberName] string member = "",
      [CallerLineNumber] int line = -1)
    {
      ArgumentUtility.CheckForNull<Task<TResult>>(task, nameof (task));
      if (task.IsCompleted)
        return await task;
      TaskCompletionSource<bool> cancellationTcs = new TaskCompletionSource<bool>((object) 64);
      CancellationTokenRegistration tokenRegistration = cancellationToken.Register((Action) (() => cancellationTcs.SetResult(false)));
      try
      {
        if (await Task.WhenAny((Task) task, (Task) cancellationTcs.Task).ConfigureAwait(false) == task)
          return await task;
      }
      finally
      {
        tokenRegistration.Dispose();
      }
      tokenRegistration = new CancellationTokenRegistration();
      if (!cancellationToken.IsCancellationRequested)
        throw new InvalidOperationException("Task ended but cancellation token is not marked for cancellation.");
      int seconds = 3;
      TaskCompletionSource<bool> lastChanceTcs = new TaskCompletionSource<bool>((object) 64);
      using (CancellationTokenSource lastChanceTimer = new CancellationTokenSource(TimeSpan.FromSeconds((double) seconds)))
      {
        tokenRegistration = lastChanceTimer.Token.Register((Action) (() => lastChanceTcs.SetResult(false)));
        try
        {
          if (await Task.WhenAny((Task) task, (Task) lastChanceTcs.Task).ConfigureAwait(false) == task)
            return await task;
        }
        finally
        {
          tokenRegistration.Dispose();
        }
        tokenRegistration = new CancellationTokenRegistration();
      }
      TaskCancellationExtensions.ObserveExceptionIfNeeded((Task) task);
      string message = string.Format("Task in function {0} at {1}:{2} was still active {3} seconds after operation was cancelled.", (object) member, (object) file, (object) line, (object) seconds);
      if (makeMessage != null)
        message = message + " " + makeMessage();
      throw new OperationCanceledException(message, cancellationToken);
    }

    private static void ObserveExceptionIfNeeded(Task task) => task.ContinueWith<AggregateException>((Func<Task, AggregateException>) (t => t.Exception), TaskContinuationOptions.OnlyOnFaulted);

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private struct Void
    {
    }
  }
}
