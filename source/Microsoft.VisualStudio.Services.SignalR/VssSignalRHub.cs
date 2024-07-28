// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssSignalRHub
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.SignalR
{
  internal class VssSignalRHub
  {
    private readonly string m_name;
    private readonly Capture<int> m_maxCacheSize;
    private readonly Capture<TimeSpan> m_inactivityInterval;
    private readonly VssMemoryCacheList<string, VssSignalRHubGroup> m_groups;

    public VssSignalRHub(
      VssSignalRHubGroupCache cache,
      string name,
      int maxCacheSize,
      TimeSpan inactivityInterval)
    {
      this.m_name = name;
      this.m_maxCacheSize = new Capture<int>(maxCacheSize);
      this.m_inactivityInterval = new Capture<TimeSpan>(inactivityInterval);
      VssCacheExpiryProvider<string, VssSignalRHubGroup> expiryProvider = new VssCacheExpiryProvider<string, VssSignalRHubGroup>(new Capture<TimeSpan>(VssCacheExpiryProvider.NoExpiry), this.m_inactivityInterval);
      this.m_groups = new VssMemoryCacheList<string, VssSignalRHubGroup>((IVssCachePerformanceProvider) cache, (IEqualityComparer<string>) StringComparer.Ordinal, (int) this.m_maxCacheSize, expiryProvider);
    }

    public string Name => this.m_name;

    public Capture<TimeSpan> InactivityInterval => this.m_inactivityInterval;

    public Capture<int> MaxCacheSize => this.m_maxCacheSize;

    public VssMemoryCacheList<string, VssSignalRHubGroup> Groups => this.m_groups;
  }
}
