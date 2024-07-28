// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.OrchestrationClock
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public class OrchestrationClock
  {
    private TimeSpan m_timeOffset;

    public OrchestrationClock(TimeSpan timeOffset) => this.m_timeOffset = timeOffset;

    public OrchestrationClock()
      : this(TimeSpan.Zero)
    {
    }

    public OrchestrationClock(DateTime utcNow)
      : this(utcNow - DateTime.UtcNow)
    {
    }

    public static OrchestrationClock Default => new OrchestrationClock();

    public DateTime UtcNow => !(this.m_timeOffset != TimeSpan.Zero) ? DateTime.UtcNow : DateTime.UtcNow.Add(this.m_timeOffset);

    public void AdvanceBy(TimeSpan time)
    {
      if (time.Ticks <= 0L)
        throw new ArgumentException("Time can flow only forward.", nameof (time));
      this.m_timeOffset += time;
    }
  }
}
