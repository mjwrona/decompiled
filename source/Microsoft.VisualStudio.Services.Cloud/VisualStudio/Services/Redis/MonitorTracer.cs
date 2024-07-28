// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.MonitorTracer
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal class MonitorTracer : PerformanceTracer
  {
    public MonitorTracer()
      : base("<GLOBAL>")
    {
    }

    public override void RedisError(IVssRequestContext requestContext, Exception ex)
    {
      if (requestContext.ServiceHost.IsProduction)
        return;
      base.RedisError(requestContext, ex);
    }

    public override void RedisError(
      IVssRequestContext requestContext,
      string message,
      params object[] args)
    {
      if (requestContext.ServiceHost.IsProduction)
        return;
      base.RedisError(requestContext, message, args);
    }

    public override void RedisServerInfo(
      IVssRequestContext requestContext,
      string message,
      params object[] args)
    {
      requestContext.TraceAlways(8131005, TraceLevel.Info, "Redis", "RedisMonitor", message, args);
    }

    public override void RedisSlowCommand(
      IVssRequestContext requestContext,
      string message,
      params object[] args)
    {
      requestContext.TraceAlways(8131006, TraceLevel.Info, "Redis", "RedisMonitor", message, args);
    }

    public override IDisposable TimedCall(IVssRequestContext requestContext) => (IDisposable) null;
  }
}
