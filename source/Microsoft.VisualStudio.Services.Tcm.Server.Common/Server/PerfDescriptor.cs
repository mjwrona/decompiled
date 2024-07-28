// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PerfDescriptor
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class PerfDescriptor : IDisposable
  {
    private IVssRequestContext m_context;
    private string m_layer;
    private string m_actionName;
    private int m_level;
    private Stopwatch m_stopWatch;
    private Action<long, Guid> m_elaspsedTimeDelegate;
    private Action<CustomerIntelligenceData> m_publishDurationDelegate;
    private long m_timeSpentInDescendants;
    private long m_inclusiveTimeInMs;
    private long m_exclusiveTimeInMs;
    private long m_duration;
    private bool m_isMocked;

    public PerfDescriptor(bool isMock) => this.m_isMocked = true;

    internal PerfDescriptor(
      IVssRequestContext context,
      string layer,
      string actionName,
      int level,
      Action<long, Guid> elapsedTimeDelegate,
      Action<CustomerIntelligenceData> publishDurationDelegate = null,
      long duration = 0)
    {
      this.m_context = context;
      this.m_layer = layer;
      this.m_actionName = actionName;
      this.m_level = level;
      this.m_elaspsedTimeDelegate = elapsedTimeDelegate;
      this.m_publishDurationDelegate = publishDurationDelegate;
      this.m_duration = duration;
      this.m_stopWatch = new Stopwatch();
      this.m_stopWatch.Start();
    }

    internal void AddTimeInDescendant(long duration) => this.m_timeSpentInDescendants += duration;

    public void Dispose()
    {
      if (this.m_isMocked)
        return;
      try
      {
        if (this.m_context == null)
          return;
        this.m_stopWatch.Stop();
        this.m_inclusiveTimeInMs = this.m_duration == 0L ? this.m_stopWatch.ElapsedMilliseconds : this.m_duration;
        this.m_exclusiveTimeInMs = this.m_inclusiveTimeInMs - this.m_timeSpentInDescendants;
        if (this.m_elaspsedTimeDelegate != null)
          this.m_elaspsedTimeDelegate(this.m_inclusiveTimeInMs, this.m_context.ActivityId);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("Layer", this.m_layer);
        properties.Add("ActionName", this.m_actionName);
        properties.Add("Level", (double) this.m_level);
        properties.Add("ActivityId", (object) this.m_context.ActivityId);
        properties.Add("UniqueIdentifier", (object) this.m_context.UniqueIdentifier);
        properties.Add("InclusiveTimeMs", (double) this.m_inclusiveTimeInMs);
        properties.Add("ExclusiveTimeMs", (double) this.m_exclusiveTimeInMs);
        if (this.m_publishDurationDelegate != null)
          this.m_publishDurationDelegate(properties);
        else
          this.m_context.GetService<CustomerIntelligenceService>().Publish(this.m_context, "Performance", "Layer", properties);
      }
      catch (Exception ex)
      {
        if (this.m_context == null)
          return;
        this.m_context.TraceException(PerfManager.Layer, ex);
      }
    }

    internal static PerfDescriptor Empty => new PerfDescriptor((IVssRequestContext) null, string.Empty, string.Empty, 0, (Action<long, Guid>) null);

    public static PerfDescriptor GetMockPerfDescriptor() => new PerfDescriptor(true);
  }
}
