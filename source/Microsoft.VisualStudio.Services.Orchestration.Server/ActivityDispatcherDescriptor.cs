// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.ActivityDispatcherDescriptor
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  public sealed class ActivityDispatcherDescriptor : DispatcherDescriptor
  {
    private ActivityDispatcherPerformanceCounters m_counters;

    public ActivityDispatcherDescriptor()
      : this(string.Empty)
    {
    }

    public ActivityDispatcherDescriptor(string dispatcherType) => this.Type = dispatcherType ?? string.Empty;

    public string Type { get; set; }

    internal ActivityDispatcherPerformanceCounters Counters
    {
      get
      {
        if (this.m_counters == null)
          this.m_counters = new ActivityDispatcherPerformanceCounters(this.HubName, this.Type);
        return this.m_counters;
      }
    }
  }
}
