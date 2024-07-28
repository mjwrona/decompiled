// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TracepointUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class TracepointUtils
  {
    private static readonly JsonSerializerSettings traceDataSerializerSettings = new JsonSerializerSettings()
    {
      DateTimeZoneHandling = DateTimeZoneHandling.Utc,
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    };

    public static bool TraceException(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Exception exception,
      object logData = null,
      TraceLevel traceLevel = TraceLevel.Error,
      [CallerMemberName] string caller = null)
    {
      if (requestContext != null)
      {
        var messageObj = new
        {
          tracepoint = tracepoint,
          caller = caller,
          data = logData,
          exception = exception,
          stackTrace = new StackTrace().ToString()
        };
        TracepointUtils.WriteTraceMessage(requestContext, tracepoint, area, layer, traceLevel, (object) messageObj, false, exception);
      }
      return false;
    }

    public static void Tracepoint(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Func<object> logData = null,
      TraceLevel traceLevel = TraceLevel.Verbose,
      bool traceAlways = false,
      [CallerMemberName] string caller = null)
    {
      if ((requestContext != null ? (!requestContext.IsTracing(tracepoint, traceLevel, area, layer) ? 1 : 0) : 1) != 0)
        return;
      object obj = logData != null ? logData() : (object) null;
      var messageObj = new
      {
        tracepoint = tracepoint,
        caller = caller,
        data = obj
      };
      TracepointUtils.WriteTraceMessage(requestContext, tracepoint, area, layer, traceLevel, (object) messageObj, traceAlways, (Exception) null);
    }

    public static void TraceAlways(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      object logData = null,
      TraceLevel traceLevel = TraceLevel.Verbose,
      [CallerMemberName] string caller = null)
    {
      if (requestContext == null)
        return;
      var messageObj = new
      {
        tracepoint = tracepoint,
        caller = caller,
        data = logData
      };
      TracepointUtils.WriteTraceMessage(requestContext, tracepoint, area, layer, traceLevel, (object) messageObj, true, (Exception) null);
    }

    public static TResult TraceBlock<TResult>(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Func<TResult> action,
      Func<TResult, object> logAtExit,
      Func<object> logAtEntry = null,
      bool logReturnValue = false,
      TraceLevel traceLevel = TraceLevel.Verbose,
      bool traceAlways = false,
      [CallerMemberName] string caller = null)
    {
      Exception observedException = (Exception) null;
      Func<Exception, bool> func = (Func<Exception, bool>) (ex =>
      {
        observedException = ex;
        return false;
      });
      object obj1 = (object) null;
      if (logAtEntry != null)
      {
        try
        {
          obj1 = logAtEntry();
        }
        catch (Exception ex)
        {
          obj1 = (object) ex.ToString();
        }
      }
      TResult result = default (TResult);
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        result = action();
      }
      catch (Exception ex) when (func(ex))
      {
      }
      finally
      {
        stopwatch.Stop();
        string str = (string) null;
        if (observedException != null)
        {
          traceLevel = TraceLevel.Error;
          str = new StackTrace().ToString();
        }
        if (requestContext != null && (traceAlways || requestContext.IsTracing(tracepoint, traceLevel, area, layer)))
        {
          object obj2 = (object) null;
          if (logAtExit != null)
          {
            try
            {
              obj2 = logAtExit(result);
            }
            catch (Exception ex)
            {
              obj2 = (object) ex.ToString();
            }
          }
          object messageObj = !logReturnValue ? (object) new
          {
            tracepoint = tracepoint,
            caller = caller,
            elapsed = stopwatch.ElapsedMilliseconds,
            entryData = obj1,
            exitData = obj2,
            exception = observedException,
            stackTrace = str
          } : (object) new
          {
            tracepoint = tracepoint,
            caller = caller,
            elapsed = stopwatch.ElapsedMilliseconds,
            entryData = obj1,
            exitData = obj2,
            returnValue = result,
            exception = observedException,
            stackTrace = str
          };
          TracepointUtils.WriteTraceMessage(requestContext, tracepoint, area, layer, traceLevel, messageObj, traceAlways, observedException);
        }
      }
      return result;
    }

    private static void WriteTraceMessage(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      TraceLevel traceLevel,
      object messageObj,
      bool traceAlways,
      Exception observedException)
    {
      string str;
      try
      {
        str = JsonConvert.SerializeObject(messageObj, TracepointUtils.traceDataSerializerSettings);
      }
      catch (Exception ex)
      {
        str = JsonConvert.SerializeObject((object) new
        {
          serializationException = ex?.ToString(),
          observedException = observedException?.ToString()
        });
      }
      if (observedException != null)
        requestContext.RequestTracer.TraceException(tracepoint, traceLevel, area, layer, observedException, str, (object[]) null);
      else if (traceAlways)
        requestContext.TraceAlways(tracepoint, traceLevel, area, layer, str, (object[]) null);
      else
        requestContext.Trace(tracepoint, traceLevel, area, layer, str);
    }

    public static void TraceBlock(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Action action,
      Func<object> logAtExit = null,
      Func<object> logAtEntry = null,
      TraceLevel traceLevel = TraceLevel.Verbose,
      bool traceAlways = false,
      [CallerMemberName] string caller = null)
    {
      Func<object> func1 = (Func<object>) (() =>
      {
        action();
        return (object) null;
      });
      IVssRequestContext requestContext1 = requestContext;
      int tracepoint1 = tracepoint;
      string area1 = area;
      string layer1 = layer;
      Func<object> action1 = func1;
      Func<object> func2 = logAtEntry;
      Func<object> logAtExit1 = logAtExit;
      Func<object> logAtEntry1 = func2;
      int traceLevel1 = (int) traceLevel;
      int num = traceAlways ? 1 : 0;
      string caller1 = caller;
      TracepointUtils.TraceBlock<object>(requestContext1, tracepoint1, area1, layer1, action1, logAtExit1, logAtEntry1, traceLevel: (TraceLevel) traceLevel1, traceAlways: num != 0, caller: caller1);
    }

    public static TResult TraceBlock<TResult>(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Func<TResult> action,
      Func<object> logAtEntry = null,
      bool logReturnValue = false,
      TraceLevel traceLevel = TraceLevel.Verbose,
      bool traceAlways = false,
      [CallerMemberName] string caller = null)
    {
      return TracepointUtils.TraceBlock<TResult>(requestContext, tracepoint, area, layer, action, (Func<TResult, object>) null, logAtEntry, logReturnValue, traceLevel, traceAlways, caller);
    }

    public static TResult TraceBlock<TResult>(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Func<TResult> action,
      Func<object> logAtExit,
      Func<object> logAtEntry = null,
      bool logReturnValue = false,
      TraceLevel traceLevel = TraceLevel.Verbose,
      bool traceAlways = false,
      [CallerMemberName] string caller = null)
    {
      Func<TResult, object> func1 = (Func<TResult, object>) null;
      if (logAtExit != null)
        func1 = (Func<TResult, object>) (_ => logAtExit());
      IVssRequestContext requestContext1 = requestContext;
      int tracepoint1 = tracepoint;
      string area1 = area;
      string layer1 = layer;
      Func<TResult> action1 = action;
      Func<object> func2 = logAtEntry;
      Func<TResult, object> logAtExit1 = func1;
      Func<object> logAtEntry1 = func2;
      int num1 = logReturnValue ? 1 : 0;
      int num2 = (int) traceLevel;
      int num3 = traceAlways ? 1 : 0;
      string caller1 = caller;
      return TracepointUtils.TraceBlock<TResult>(requestContext1, tracepoint1, area1, layer1, action1, logAtExit1, logAtEntry1, num1 != 0, (TraceLevel) num2, num3 != 0, caller1);
    }
  }
}
