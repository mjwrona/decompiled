// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.SignalRHubProxyPerformanceCounters
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.SignalR
{
  internal sealed class SignalRHubProxyPerformanceCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Framework.Server.Perf_SignalR_";

    public SignalRHubProxyPerformanceCounters(string instanceName)
    {
      this.HubMethodInvocationsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_SignalR_HubMethodInvocationsPerSec", instanceName);
      this.HubMethodInvocationsTotal = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_SignalR_HubMethodInvocationsTotal", instanceName);
      this.SkippedHubMethodInvocationsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_SignalR_SkippedHubMethodInvocationsPerSec", instanceName);
      this.SkippedHubMethodInvocationsTotal = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_SignalR_SkippedHubMethodInvocationsTotal", instanceName);
      this.HubMethodResolutionsTotal = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_SignalR_HubMethodResolutionsTotal", instanceName);
      this.SkippedHubMethodResolutionsTotal = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_SignalR_SkippedHubMethodResolutionsTotal", instanceName);
      this.HubMethodResolutionsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_SignalR_HubMethodResolutionsPerSec", instanceName);
      this.SkippedHubMethodResolutionsPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_SignalR_SkippedHubMethodResolutionsPerSec", instanceName);
    }

    public VssPerformanceCounter HubMethodInvocationsPerSec { get; }

    public VssPerformanceCounter HubMethodInvocationsTotal { get; }

    public VssPerformanceCounter SkippedHubMethodInvocationsPerSec { get; }

    public VssPerformanceCounter SkippedHubMethodInvocationsTotal { get; }

    public VssPerformanceCounter HubMethodResolutionsTotal { get; }

    public VssPerformanceCounter SkippedHubMethodResolutionsTotal { get; }

    public VssPerformanceCounter HubMethodResolutionsPerSec { get; }

    public VssPerformanceCounter SkippedHubMethodResolutionsPerSec { get; }
  }
}
