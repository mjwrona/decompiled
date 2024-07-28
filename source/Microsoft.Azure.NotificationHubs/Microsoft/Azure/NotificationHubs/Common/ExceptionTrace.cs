// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.ExceptionTrace
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common.Diagnostics;
using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal class ExceptionTrace
  {
    private const ushort FailFastEventLogCategory = 6;
    private readonly string eventSourceName;

    public ExceptionTrace(string eventSourceName) => this.eventSourceName = eventSourceName;

    public Exception AsError(Exception exception, EventTraceActivity activity = null) => this.TraceException<Exception>(exception, TraceEventType.Error, activity);

    public Exception AsInformation(Exception exception, EventTraceActivity activity = null) => this.TraceException<Exception>(exception, TraceEventType.Information, activity);

    public Exception AsWarning(Exception exception, EventTraceActivity activity = null) => this.TraceException<Exception>(exception, TraceEventType.Warning, activity);

    public Exception AsVerbose(Exception exception, EventTraceActivity activity = null) => this.TraceException<Exception>(exception, TraceEventType.Verbose, activity);

    public ArgumentException Argument(string paramName, string message) => this.TraceException<ArgumentException>(new ArgumentException(message, paramName), TraceEventType.Error);

    public ArgumentNullException ArgumentNull(string paramName) => this.TraceException<ArgumentNullException>(new ArgumentNullException(paramName), TraceEventType.Error);

    public ArgumentNullException ArgumentNull(string paramName, string message) => this.TraceException<ArgumentNullException>(new ArgumentNullException(paramName, message), TraceEventType.Error);

    public ArgumentException ArgumentNullOrEmpty(string paramName) => this.Argument(paramName, SRCore.ArgumentNullOrEmpty((object) paramName));

    public ArgumentException ArgumentNullOrWhiteSpace(string paramName) => this.Argument(paramName, SRCore.ArgumentNullOrWhiteSpace((object) paramName));

    public ArgumentOutOfRangeException ArgumentOutOfRange(
      string paramName,
      object actualValue,
      string message)
    {
      return this.TraceException<ArgumentOutOfRangeException>(new ArgumentOutOfRangeException(paramName, actualValue, message), TraceEventType.Error);
    }

    public ObjectDisposedException ObjectDisposed(string message) => this.TraceException<ObjectDisposedException>(new ObjectDisposedException((string) null, message), TraceEventType.Error);

    public void TraceHandled(
      Exception exception,
      string catchLocation,
      EventTraceActivity activity = null)
    {
      MessagingClientEtwProvider.Provider.HandledExceptionWithFunctionName(activity, catchLocation, exception.ToStringSlim(), string.Empty);
      this.BreakOnException(exception);
    }

    public void TraceUnhandled(Exception exception) => MessagingClientEtwProvider.Provider.EventWriteUnhandledException(this.eventSourceName + ": " + exception.ToStringSlim());

    public TException TraceException<TException>(
      TException exception,
      TraceEventType level,
      EventTraceActivity activity = null)
      where TException : Exception
    {
      if (!exception.Data.Contains((object) this.eventSourceName))
      {
        exception.Data[(object) this.eventSourceName] = (object) this.eventSourceName;
        switch (level)
        {
          case TraceEventType.Critical:
          case TraceEventType.Error:
            if (MessagingClientEtwProvider.Provider.IsEnabled(EventLevel.Error, (EventKeywords) 140737488355328, (EventChannel) 19))
            {
              MessagingClientEtwProvider.Provider.ThrowingExceptionError(activity, ExceptionTrace.GetDetailsForThrownException((Exception) exception));
              break;
            }
            break;
          case TraceEventType.Warning:
            if (MessagingClientEtwProvider.Provider.IsEnabled(EventLevel.Warning, (EventKeywords) 140737488355328, (EventChannel) 19))
            {
              MessagingClientEtwProvider.Provider.ThrowingExceptionWarning(activity, ExceptionTrace.GetDetailsForThrownException((Exception) exception));
              break;
            }
            break;
        }
      }
      this.BreakOnException((Exception) exception);
      return exception;
    }

    private static string GetDetailsForThrownException(Exception e)
    {
      string str1 = e.GetType().ToString();
      StackTrace stackTrace = new StackTrace();
      string str2 = stackTrace.ToString();
      if (stackTrace.FrameCount > 10)
        str2 = string.Join(Environment.NewLine, str2.Split(new string[1]
        {
          Environment.NewLine
        }, 11, StringSplitOptions.RemoveEmptyEntries), 0, 10) + "...";
      return str1 + Environment.NewLine + str2 + Environment.NewLine + "Exception ToString:" + Environment.NewLine + e.ToStringSlim();
    }

    internal void BreakOnException(Exception exception)
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void TraceFailFast(string message)
    {
      EventLogger logger = new EventLogger(this.eventSourceName, Fx.Trace);
      this.TraceFailFast(message, logger);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void TraceFailFast(string message, EventLogger logger)
    {
      if (logger == null)
        return;
      try
      {
        string str = (string) null;
        try
        {
          str = new StackTrace().ToString();
        }
        catch (Exception ex)
        {
          str = ex.Message;
          if (!Fx.IsFatal(ex))
            return;
          throw;
        }
        finally
        {
          logger.LogEvent(TraceEventType.Critical, (ushort) 6, 3221291110U, message, str);
        }
      }
      catch (Exception ex)
      {
        logger.LogEvent(TraceEventType.Critical, (ushort) 6, 3221291111U, ex.ToString());
        if (!Fx.IsFatal(ex))
          return;
        throw;
      }
    }
  }
}
