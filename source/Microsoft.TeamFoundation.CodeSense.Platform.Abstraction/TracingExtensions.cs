// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.TracingExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7627AC5C-7AFD-416A-A79B-D03A392C9E3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  public static class TracingExtensions
  {
    public static T WithTrace<T>(
      this IVssRequestContext requestContext,
      int tracePoint,
      string traceLayer,
      Func<T> func,
      [CallerMemberName] string methodName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      string area = "CodeSense";
      requestContext.TraceEnter(tracePoint, area, traceLayer, methodName);
      try
      {
        return func();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(tracePoint + 1, area, traceLayer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(tracePoint + 2, area, traceLayer, methodName);
      }
    }

    public static void WithTrace(
      this IVssRequestContext requestContext,
      int tracePoint,
      string traceLayer,
      Action action,
      [CallerMemberName] string methodName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      string area = "CodeSense";
      requestContext.TraceEnter(tracePoint, area, traceLayer, methodName);
      try
      {
        action();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(tracePoint + 1, area, traceLayer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(tracePoint + 2, area, traceLayer, methodName);
      }
    }

    public static void Trace(
      this IVssRequestContext requestContext,
      int tracePoint,
      string traceLayer,
      string format,
      params object[] args)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      string area = "CodeSense";
      VssRequestContextExtensions.Trace(requestContext, tracePoint, TraceLevel.Info, area, traceLayer, format, args);
    }

    public static void TraceRaw(
      int tracePoint,
      TraceLevel level,
      string traceLayer,
      string format,
      params object[] args)
    {
      TeamFoundationTracingService.TraceRaw(tracePoint, level, "CodeSense", traceLayer, format, args);
    }

    public static void TraceConditionallyInChunks(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Func<StringBuilder> message)
    {
      if (level != TraceLevel.Error && !requestContext.IsTracing(tracepoint, level, area, layer))
        return;
      string str1 = Guid.NewGuid().ToString();
      StringBuilder stringBuilder = message();
      for (int startIndex = 0; startIndex < stringBuilder.Length; startIndex += 8000)
      {
        string str2 = stringBuilder.ToString(startIndex, Math.Min(8000, stringBuilder.Length - startIndex));
        requestContext.Trace(tracepoint, level, area, layer, "(Id {0}, Start Index {1}) {2}", (object) str1, (object) startIndex.ToString(), (object) str2);
      }
    }
  }
}
