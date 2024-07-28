// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.TimedCall
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal sealed class TimedCall : IDisposable
  {
    private readonly Tracer m_tracer;
    private PerformanceTimer m_perfTimer;
    private bool m_isDisposed;
    private readonly IVssRequestContext m_requestContext;

    public TimedCall(IVssRequestContext requestContext, Tracer tracer)
    {
      this.m_requestContext = requestContext;
      this.m_tracer = tracer;
      this.m_isDisposed = false;
      this.m_tracer.EnterRedis(this.m_requestContext);
      this.m_perfTimer = PerformanceTimer.StartMeasure(requestContext, "Redis");
    }

    void IDisposable.Dispose()
    {
      if (this.m_isDisposed)
        return;
      this.m_perfTimer.End();
      this.m_tracer.ExitRedis(this.m_requestContext, this.m_perfTimer.Duration);
      this.m_perfTimer.Dispose();
      this.m_isDisposed = true;
    }
  }
}
