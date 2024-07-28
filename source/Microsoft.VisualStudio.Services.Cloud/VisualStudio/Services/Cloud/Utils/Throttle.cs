// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Utils.Throttle
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud.Utils
{
  public class Throttle
  {
    private int m_maxCallsPerInterval;
    private readonly long m_intervalInTicks;
    private readonly object m_lockObj = new object();
    private long m_intervalStartTicks;
    private int m_callsThisInterval;
    private bool m_hasHitThrottle;

    public Throttle(TimeSpan interval, int maxCallsPerInterval)
    {
      if (interval.Ticks <= 0L)
        throw new ArgumentOutOfRangeException("interval.Ticks must be greater than 0");
      if (maxCallsPerInterval < 0)
        throw new ArgumentOutOfRangeException("maxCallsPerInterval must be greater than or equal to 0");
      this.m_intervalInTicks = interval.Ticks;
      this.m_maxCallsPerInterval = maxCallsPerInterval;
    }

    public bool ShouldAllowAction(IVssRequestContext requestContext)
    {
      if (this.m_maxCallsPerInterval == 0)
        return true;
      bool flag1 = false;
      bool flag2 = true;
      lock (this.m_lockObj)
      {
        long ticks = DateTime.UtcNow.Ticks;
        if (this.m_intervalStartTicks < ticks - this.m_intervalInTicks)
        {
          this.m_intervalStartTicks = ticks;
          this.m_hasHitThrottle = false;
          this.m_callsThisInterval = 1;
        }
        else
          ++this.m_callsThisInterval;
        flag1 = !this.m_hasHitThrottle && this.m_callsThisInterval == this.m_maxCallsPerInterval;
        flag2 = this.m_callsThisInterval <= this.m_maxCallsPerInterval;
      }
      if (flag1)
      {
        this.m_hasHitThrottle = true;
        string message = string.Format("Tracing has been paused until {0} due to hitting maximum call limit of {1} within {2}ms.", (object) new DateTime(this.m_intervalStartTicks + this.m_intervalInTicks), (object) this.m_maxCallsPerInterval, (object) TimeSpan.FromTicks(this.m_intervalInTicks).TotalMilliseconds);
        requestContext.Trace(198000, TraceLevel.Info, nameof (Throttle), nameof (ShouldAllowAction), message);
      }
      return flag2;
    }
  }
}
