// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventStats
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class EventStats
  {
    private TeamFoundationEvent m_event;

    public EventStats(TeamFoundationEvent ev) => this.m_event = ev;

    public int PreProcessEvaluations { get; set; }

    public int ProcessEvaluations { get; set; }

    public int Notifications { get; set; }

    public Stopwatch PreProcess { get; } = new Stopwatch();

    public Stopwatch Process { get; } = new Stopwatch();

    public override string ToString() => string.Format("Event: {0} evaluations (pre post): {1} {2} notifications: {3} pre process time: {4} process time: {5}", (object) this.m_event.Id, (object) this.PreProcessEvaluations, (object) this.ProcessEvaluations, (object) this.Notifications, (object) this.PreProcess.ElapsedMilliseconds, (object) this.Process.ElapsedMilliseconds);
  }
}
