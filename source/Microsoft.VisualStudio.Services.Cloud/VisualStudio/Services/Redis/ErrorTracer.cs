// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.ErrorTracer
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Redis
{
  public class ErrorTracer : Tracer
  {
    private static readonly Tracer s_globalTracer = (Tracer) new ErrorTracer("<GLOBAL>");

    public ErrorTracer(string cacheArea)
      : base(cacheArea)
    {
    }

    public new static Tracer Global => ErrorTracer.s_globalTracer;

    public override void RedisError(IVssRequestContext requestContext, Exception ex)
    {
      this.IncrementPerfCounter();
      requestContext.TraceException(8110001, TraceLevel.Error, "Redis", "RedisCache", ex);
    }

    public override void RedisError(
      IVssRequestContext requestContext,
      string message,
      params object[] args)
    {
      this.IncrementPerfCounter();
      VssRequestContextExtensions.Trace(requestContext, 8110001, TraceLevel.Error, "Redis", "RedisCache", message, args);
    }

    private void IncrementPerfCounter()
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_TotalExceptions", this.m_cacheArea).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Redis_ExceptionsPerSec", this.m_cacheArea).Increment();
    }
  }
}
