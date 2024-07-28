// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.AsyncInvoker
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement
{
  public static class AsyncInvoker
  {
    public static T InvokeAsyncWait<T>(
      Func<CancellationTokenSource, Task<T>> function,
      int waitTimeInMs,
      TraceMetaData traceMetadata)
    {
      if (function == null)
        throw new ArgumentNullException(nameof (function));
      if (traceMetadata == null)
        throw new ArgumentNullException(nameof (traceMetadata));
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
      try
      {
        Task<T> task = function(cancellationTokenSource);
        if (!task.Wait(waitTimeInMs))
        {
          cancellationTokenSource.Cancel();
          cancellationTokenSource.Token.ThrowIfCancellationRequested();
        }
        return task.Result;
      }
      catch (Exception ex)
      {
        if (ex.InnerException is OperationCanceledException)
          Tracer.PublishKpi("FaultManagementAsyncApiCancellationCount", "Indexing Pipeline", 1.0);
        throw;
      }
      finally
      {
        cancellationTokenSource.Dispose();
      }
    }
  }
}
