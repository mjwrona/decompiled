// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Tracing.TraceExtensions
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.Tracing;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Orchestration.Tracing
{
  internal static class TraceExtensions
  {
    private static readonly Lazy<JsonSerializerSettings> s_serializerSettings = new Lazy<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() => new JsonSerializerSettings()
    {
      TypeNameHandling = TypeNameHandling.Auto,
      Formatting = Formatting.Indented
    }));

    public static string ToJsonString(object data) => JsonConvert.SerializeObject(data, TraceExtensions.s_serializerSettings.Value);

    public static void Trace(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      Func<string> generateMessage)
    {
      TraceExtensions.ExceptionHandlingWrapper(source, (Action) (() => source.Trace(eventLevel, generateMessage())));
    }

    public static void Trace(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      string format,
      params object[] args)
    {
      TraceExtensions.ExceptionHandlingWrapper(source, (Action) (() => source.Trace(eventLevel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args))));
    }

    public static void TraceSession(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      string sessionId,
      Func<string> generateMessage)
    {
      TraceExtensions.ExceptionHandlingWrapper(source, (Action) (() => source.TraceSession(eventLevel, sessionId, generateMessage())));
    }

    public static void TraceSession(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      string sessionId,
      string format,
      params object[] args)
    {
      TraceExtensions.ExceptionHandlingWrapper(source, (Action) (() => source.TraceSession(eventLevel, sessionId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args))));
    }

    public static void TraceInstance(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      OrchestrationInstance orchestrationInstance,
      string format,
      params object[] args)
    {
      TraceExtensions.ExceptionHandlingWrapper(source, (Action) (() => source.TraceInstance(eventLevel, orchestrationInstance == null ? string.Empty : orchestrationInstance.InstanceId, orchestrationInstance == null ? string.Empty : orchestrationInstance.ExecutionId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args))));
    }

    public static void TraceInstance(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      OrchestrationInstance orchestrationInstance,
      Func<string> generateMessage)
    {
      TraceExtensions.ExceptionHandlingWrapper(source, (Action) (() => source.TraceInstance(eventLevel, orchestrationInstance == null ? string.Empty : orchestrationInstance.InstanceId, orchestrationInstance == null ? string.Empty : orchestrationInstance.ExecutionId, generateMessage())));
    }

    public static Exception TraceException(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      Exception exception)
    {
      return source.TraceException(eventLevel, exception, string.Empty);
    }

    public static Exception TraceException(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      Exception exception,
      Func<string> generateMessage)
    {
      return source.TraceExceptionCore(eventLevel, (string) null, (string) null, exception, generateMessage);
    }

    public static Exception TraceException(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      Exception exception,
      string format,
      params object[] args)
    {
      return source.TraceExceptionCore(eventLevel, (string) null, (string) null, exception, format, args);
    }

    public static Exception TraceExceptionInstance(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      OrchestrationInstance orchestrationInstance,
      Exception exception)
    {
      return source.TraceExceptionCore(eventLevel, orchestrationInstance.InstanceId, orchestrationInstance.ExecutionId, exception, string.Empty);
    }

    public static Exception TraceExceptionInstance(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      OrchestrationInstance orchestrationInstance,
      Exception exception,
      Func<string> generateMessage)
    {
      return source.TraceExceptionCore(eventLevel, orchestrationInstance.InstanceId, orchestrationInstance.ExecutionId, exception, generateMessage);
    }

    public static Exception TraceExceptionInstance(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      OrchestrationInstance orchestrationInstance,
      Exception exception,
      string format,
      params object[] args)
    {
      return source.TraceExceptionCore(eventLevel, orchestrationInstance.InstanceId, orchestrationInstance.ExecutionId, exception, format, args);
    }

    public static Exception TraceExceptionSession(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      string sessionId,
      Exception exception)
    {
      return source.TraceExceptionCore(eventLevel, sessionId, (string) null, exception, string.Empty);
    }

    public static Exception TraceExceptionSession(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      string sessionId,
      Exception exception,
      Func<string> generateMessage)
    {
      return source.TraceExceptionCore(eventLevel, sessionId, (string) null, exception, generateMessage);
    }

    public static Exception TraceExceptionSession(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      string sessionId,
      Exception exception,
      string format,
      params object[] args)
    {
      return source.TraceExceptionCore(eventLevel, sessionId, (string) null, exception, format, args);
    }

    private static Exception TraceExceptionCore(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      string iid,
      string eid,
      Exception exception,
      string format,
      params object[] args)
    {
      string newFormat = format + "\nException: " + exception.GetType()?.ToString() + " : " + exception.Message + "\n\t" + exception.StackTrace;
      TraceExtensions.ExceptionHandlingWrapper(source, (Action) (() =>
      {
        if (string.IsNullOrEmpty(iid) && string.IsNullOrEmpty(eid))
          source.Trace(eventLevel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, newFormat, args));
        else if (string.IsNullOrEmpty(eid))
          source.TraceSession(eventLevel, iid, string.Format((IFormatProvider) CultureInfo.InvariantCulture, newFormat, args));
        else
          source.TraceInstance(eventLevel, iid, eid, string.Format((IFormatProvider) CultureInfo.InvariantCulture, newFormat, args));
      }));
      return exception;
    }

    private static Exception TraceExceptionCore(
      this OrchestrationTraceSource source,
      EventLevel eventLevel,
      string iid,
      string eid,
      Exception exception,
      Func<string> generateMessage)
    {
      string newFormat = generateMessage() + "\nException: " + exception.GetType()?.ToString() + " : " + exception.Message + "\n\t" + exception.StackTrace;
      TraceExtensions.ExceptionHandlingWrapper(source, (Action) (() =>
      {
        if (string.IsNullOrEmpty(iid) && string.IsNullOrEmpty(eid))
          source.Trace(eventLevel, newFormat);
        else if (string.IsNullOrEmpty(eid))
          source.TraceSession(eventLevel, iid, newFormat);
        else
          source.TraceInstance(eventLevel, iid, eid, newFormat);
      }));
      return exception;
    }

    private static string GetFormattedString(
      string iid,
      string eid,
      string message,
      params object[] args)
    {
      string format;
      if (!string.IsNullOrEmpty(iid))
      {
        if (!string.IsNullOrEmpty(eid))
          format = "iid=" + iid + ";eid=" + eid + ";msg=" + message;
        else
          format = "iid=" + iid + ";msg=" + message;
      }
      else
        format = "msg=" + message;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);
    }

    private static void ExceptionHandlingWrapper(OrchestrationTraceSource source, Action innerFunc)
    {
      try
      {
        innerFunc();
      }
      catch (Exception ex1)
      {
        if (Utils.IsFatal(ex1))
        {
          throw;
        }
        else
        {
          try
          {
            source.Trace(EventLevel.Critical, "Failed to log actual trace because one or more trace listeners threw an exception.");
          }
          catch (Exception ex2)
          {
            if (!Utils.IsFatal(ex2))
              return;
            throw;
          }
        }
      }
    }
  }
}
