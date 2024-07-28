// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.ExceptionUtility
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Common.Diagnostics;
using Microsoft.Azure.NotificationHubs.Properties;
using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal class ExceptionUtility
  {
    private readonly DiagnosticTrace diagnosticTrace;

    public ExceptionUtility(object diagnosticTrace) => this.diagnosticTrace = (DiagnosticTrace) diagnosticTrace;

    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static bool IsInfrastructureException(Exception exception)
    {
      if (exception == null)
        return false;
      return exception is ThreadAbortException || exception is AppDomainUnloadedException;
    }

    internal Exception ThrowHelper(
      Exception exception,
      TraceEventType eventType,
      TraceRecord extendedData)
    {
      switch (eventType)
      {
        case TraceEventType.Critical:
        case TraceEventType.Error:
          return Fx.Exception.AsError(exception);
        case TraceEventType.Warning:
          return Fx.Exception.AsWarning(exception);
        case TraceEventType.Information:
          return Fx.Exception.AsInformation(exception);
        default:
          return Fx.Exception.AsVerbose(exception);
      }
    }

    internal Exception ThrowHelper(Exception exception, TraceEventType eventType) => this.ThrowHelper(exception, eventType, (TraceRecord) null);

    internal ArgumentException ThrowHelperArgument(string paramName, string message) => Fx.Exception.Argument(paramName, message);

    internal ArgumentNullException ThrowHelperArgumentNull(string paramName) => Fx.Exception.ArgumentNull(paramName);

    internal ArgumentNullException ThrowHelperArgumentNull(string paramName, string message) => Fx.Exception.ArgumentNull(paramName, message);

    internal Exception ThrowHelperCallback(Exception innerException) => this.ThrowHelperCallback(Microsoft.Azure.NotificationHubs.SR.GetString(Resources.GenericCallbackException), innerException);

    internal Exception ThrowHelperCallback(string message, Exception innerException) => this.ThrowHelperCritical((Exception) new CallbackException(message, innerException));

    internal Exception ThrowHelperCritical(Exception exception) => this.ThrowHelper(exception, TraceEventType.Critical);

    internal Exception ThrowHelperError(Exception exception) => this.ThrowHelper(exception, TraceEventType.Error);

    internal void TraceHandledException(Exception exception, TraceEventType eventType)
    {
      if (this.diagnosticTrace == null)
        return;
      this.diagnosticTrace.TraceEvent(eventType, TraceCode.TraceHandledException, "Handled exception", (TraceRecord) null, exception);
    }
  }
}
