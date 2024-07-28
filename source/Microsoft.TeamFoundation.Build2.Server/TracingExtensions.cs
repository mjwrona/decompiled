// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TracingExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class TracingExtensions
  {
    public static void TraceError(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Error, "Build2", layer, format, arguments);
    }

    public static void TraceError(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Error, "Build2", layer, format, arguments);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      string layer,
      Exception exception)
    {
      requestContext.TraceException(0, "Build2", layer, exception);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      Exception exception)
    {
      requestContext.TraceException(tracepoint, "Build2", layer, exception);
    }

    public static void TraceInfo(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Info, "Build2", layer, format, arguments);
    }

    public static void TraceInfo(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Info, "Build2", layer, format, arguments);
    }

    public static void TraceWarning(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Warning, "Build2", layer, format, arguments);
    }

    public static void TraceWarning(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Warning, "Build2", layer, format, arguments);
    }

    public static void TraceVerbose(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Verbose, "Build2", layer, format, arguments);
    }

    public static void TraceVerbose(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Verbose, "Build2", layer, format, arguments);
    }

    public static void TraceEnter(
      this IVssRequestContext requestContext,
      string layer,
      string method)
    {
      requestContext.TraceEnter(0, "Build2", layer, method);
    }

    public static void TraceEnter(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string method)
    {
      requestContext.TraceEnter(tracepoint, "Build2", layer, method);
    }

    public static void TraceLeave(
      this IVssRequestContext requestContext,
      string layer,
      string method)
    {
      requestContext.TraceLeave(0, "Build2", layer, method);
    }

    public static void TraceLeave(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string method)
    {
      requestContext.TraceLeave(tracepoint, "Build2", layer, method);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable TraceScope(
      this IVssRequestContext requestContext,
      string layer,
      [CallerMemberName] string method = null)
    {
      return (IDisposable) new TracingExtensions.MethodScope(requestContext, layer, method);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable TraceSlowCall(
      this IVssRequestContext requestContext,
      string layer,
      int maxMilliSeconds,
      Lazy<string> message,
      [CallerMemberName] string method = null)
    {
      return (IDisposable) new TracingExtensions.TraceWatchScope(requestContext, layer, maxMilliSeconds, message, TraceLevel.Error, method);
    }

    private struct MethodScope : IDisposable
    {
      private readonly string m_layer;
      private readonly string m_method;
      private readonly IVssRequestContext m_requestContext;

      public MethodScope(IVssRequestContext requestContext, string layer, [CallerMemberName] string method = null)
      {
        this.m_requestContext = requestContext;
        this.m_layer = layer;
        this.m_method = method;
        this.m_requestContext.TraceEnter(this.m_layer, this.m_method);
      }

      public void Dispose() => this.m_requestContext.TraceLeave(this.m_layer, this.m_method);
    }

    private struct TraceWatchScope : IDisposable
    {
      private readonly IVssRequestContext m_requestContext;
      private readonly TraceWatch m_traceWatch;

      public TraceWatchScope(
        IVssRequestContext requestContext,
        string layer,
        int maxMilliSeconds,
        Lazy<string> message,
        TraceLevel traceLevel,
        [CallerMemberName] string method = null)
      {
        this.m_requestContext = requestContext;
        this.m_traceWatch = new TraceWatch(requestContext, 12030109, traceLevel, TimeSpan.FromMilliseconds((double) maxMilliSeconds), "Build2", layer, "Method {0} crossed {1} milliseconds : {2}", new object[3]
        {
          (object) method,
          (object) maxMilliSeconds,
          (object) message
        });
      }

      public void Dispose() => this.m_traceWatch.Dispose();
    }
  }
}
