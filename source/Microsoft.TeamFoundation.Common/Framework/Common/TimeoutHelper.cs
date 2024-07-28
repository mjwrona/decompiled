// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.TimeoutHelper
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TimeoutHelper
  {
    private TimeSpan m_timeout;
    private Stopwatch m_stopwatch;
    private static TimeSpan s_maxDuration = TimeSpan.FromMilliseconds((double) int.MaxValue);

    public TimeoutHelper(TimeSpan timeout)
    {
      this.m_timeout = TimeoutHelper.Trim(timeout);
      this.m_stopwatch = Stopwatch.StartNew();
    }

    public TimeSpan Value => this.m_timeout;

    public void Resume()
    {
      if (this.m_stopwatch.IsRunning)
        return;
      this.m_stopwatch.Start();
    }

    public void Suspend()
    {
      if (!this.m_stopwatch.IsRunning)
        return;
      this.m_stopwatch.Stop();
    }

    public TimeSpan ElapsedTime() => this.m_stopwatch.Elapsed;

    public TimeSpan RemainingTime()
    {
      TimeSpan timeSpan = this.m_timeout - this.m_stopwatch.Elapsed;
      if (timeSpan < TimeSpan.Zero)
        timeSpan = TimeSpan.Zero;
      return timeSpan;
    }

    public int RemainingTimeInMilliseconds() => Math.Max(0, (int) (this.m_timeout - this.m_stopwatch.Elapsed).TotalMilliseconds);

    public static TimeSpan Min(TimeSpan left, TimeSpan right) => !(left <= right) ? right : left;

    public static TimeSpan Max(TimeSpan left, TimeSpan right) => !(left >= right) ? right : left;

    public static TimeSpan Trim(TimeSpan timeout)
    {
      if (timeout > TimeoutHelper.s_maxDuration)
        return TimeoutHelper.s_maxDuration;
      return timeout < TimeSpan.Zero ? TimeSpan.Zero : timeout;
    }

    public static bool WaitOne(WaitHandle waitHandle, TimeSpan timeout)
    {
      ArgumentUtility.CheckForNull<WaitHandle>(waitHandle, nameof (waitHandle));
      if (!(timeout == TimeSpan.MaxValue))
        return waitHandle.WaitOne(timeout, true);
      waitHandle.WaitOne();
      return true;
    }
  }
}
