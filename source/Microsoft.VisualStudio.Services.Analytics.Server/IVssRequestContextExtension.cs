// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.IVssRequestContextExtension
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.OnPrem;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public static class IVssRequestContextExtension
  {
    private const int DefaultChunkSize = 1000;
    private const int DefaultMaxChunkCount = 1000;

    public static void TraceLogAndThrowException(
      this IVssRequestContext requestContext,
      int tracePoint,
      string area,
      string layer,
      Exception exception,
      Exception newException)
    {
      requestContext.TraceException(tracePoint, area, layer, exception);
      throw newException;
    }

    public static void ValidateAnalyticsEnabled(this IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      switch (requestContext.GetService<IAnalyticsStateService>().GetState(requestContext))
      {
        case AnalyticsState.Enabled:
          break;
        case AnalyticsState.Paused:
          throw new AnalyticsPausedException();
        case AnalyticsState.Deleting:
          throw new AnalyticsDeletingException();
        case AnalyticsState.Preparing:
          break;
        default:
          throw new AnalyticsNotEnabledException();
      }
    }

    public static void TraceLongTextConditionally(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Func<string> message,
      int chunkSize = 1000,
      int maxChunkCount = 1000)
    {
      if (!requestContext.IsTracing(tracepoint, level, area, layer))
        return;
      requestContext.TraceLongTextImpl(tracepoint, level, area, layer, message(), chunkSize, maxChunkCount);
    }

    public static void TraceLongTextAlways(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      int chunkSize = 1000,
      int maxChunkCount = 1000)
    {
      requestContext.TraceLongTextImpl(tracepoint, level, area, layer, message, chunkSize, maxChunkCount, true);
    }

    private static void TraceLongTextImpl(
      this IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string text,
      int chunkSize = 1000,
      int maxChunkCount = 1000,
      bool always = false)
    {
      int length = text.Length;
      int num = (int) Math.Ceiling((double) length / (double) chunkSize);
      int startIndex = 0;
      for (int index = 1; startIndex < length && index <= maxChunkCount; ++index)
      {
        string str = string.Format("{0}/{1} : {2}", (object) index, (object) num, (object) text.Substring(startIndex, Math.Min(chunkSize, length - startIndex)));
        if (always)
          requestContext.TraceAlways(tracepoint, level, area, layer, str);
        else
          requestContext.Trace(tracepoint, level, area, layer, str);
        startIndex += chunkSize;
      }
    }
  }
}
