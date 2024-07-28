// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.ExponentialBackoffRetryInvoker
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement
{
  public class ExponentialBackoffRetryInvoker
  {
    private Func<Exception, bool> m_funcShouldRetryOnException;

    private ExponentialBackoffRetryInvoker()
    {
    }

    [StaticSafe]
    public static ExponentialBackoffRetryInvoker Instance => new ExponentialBackoffRetryInvoker();

    public T InvokeWithFaultCheck<T>(
      Func<object> function,
      IIndexerFaultService faultService,
      int retryCount,
      int initialRetryDelayInMillis,
      bool throwOnFailure,
      TraceMetaData traceMetadata,
      Action<T> validate = null)
    {
      this.SetRetryConditions(new Func<Exception, bool>(faultService.ShouldRetryOnError));
      return this.InvokeInternal<T>(function, retryCount, initialRetryDelayInMillis, throwOnFailure, traceMetadata, faultService, validate);
    }

    public T Invoke<T>(
      Func<object> function,
      int retryCount,
      int initialRetryDelayInMillis,
      bool throwOnFailure,
      TraceMetaData traceMetadata)
    {
      return this.InvokeInternal<T>(function, retryCount, initialRetryDelayInMillis, throwOnFailure, traceMetadata, (IIndexerFaultService) null, (Action<T>) null);
    }

    public void SetRetryConditions(Func<Exception, bool> shouldRetryOnException) => this.m_funcShouldRetryOnException = shouldRetryOnException;

    private T InvokeInternal<T>(
      Func<object> function,
      int retryCount,
      int initialRetryDelayInMillis,
      bool throwOnFailure,
      TraceMetaData traceMetadata,
      IIndexerFaultService faultService,
      Action<T> validate)
    {
      if (function == null)
        throw new ArgumentNullException(nameof (function));
      if (traceMetadata == null)
        throw new ArgumentNullException(nameof (traceMetadata));
      if (initialRetryDelayInMillis <= 0)
        throw new ArgumentOutOfRangeException(nameof (initialRetryDelayInMillis));
      if (retryCount < 0)
        retryCount = 0;
      int num = 0;
      int millisecondsTimeout = initialRetryDelayInMillis;
      do
      {
        try
        {
          object obj1 = function();
          if (num > 0)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FaultManagementExponentialBackoffRetrySuccessCount", "Indexing Pipeline", 1.0);
          if (!(obj1 is T obj2))
            obj2 = (T) Convert.ChangeType(obj1, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
          T obj3 = obj2;
          if (validate != null)
          {
            try
            {
              validate(obj3);
            }
            catch (Exception ex)
            {
              faultService?.SetError(ex);
            }
          }
          return obj3;
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(traceMetadata.TracePoint, traceMetadata.TraceArea, traceMetadata.TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Method '{0}' failed on '{1}' attempt with exception {2}", (object) function.Method.Name, (object) (num + 1), (object) ex)));
          if (num >= retryCount)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FaultManagementExponentialBackoffApiRetryFailureCount", "Indexing Pipeline", 1.0);
          if (num < retryCount)
          {
            if (this.m_funcShouldRetryOnException != null)
            {
              if (this.m_funcShouldRetryOnException(ex))
                goto label_28;
            }
            else
              goto label_28;
          }
          faultService?.SetError(ex);
          if (throwOnFailure)
            ExceptionDispatchInfo.Capture(ex).Throw();
          else
            break;
        }
label_28:
        Thread.Sleep(millisecondsTimeout);
        millisecondsTimeout *= 2;
      }
      while (num++ < retryCount);
      return default (T);
    }
  }
}
