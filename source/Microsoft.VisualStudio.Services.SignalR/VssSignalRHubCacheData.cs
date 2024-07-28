// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssSignalRHubCacheData
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.SignalR
{
  internal class VssSignalRHubCacheData
  {
    private readonly VssSignalRHubGroupCache m_cache;
    private readonly IDictionary<string, VssSignalRHub> m_hubs;

    public VssSignalRHubCacheData(
      VssSignalRHubGroupCache cache,
      IDictionary<string, VssSignalRHub> hubs)
    {
      this.m_hubs = hubs;
      this.m_cache = cache;
    }

    public IDictionary<string, VssSignalRHub> Hubs => this.m_hubs;

    public void AddHubGroup(IVssRequestContext requestContext, VssSignalRHubGroup group)
    {
      VssSignalRHub vssSignalRhub;
      if (!this.m_hubs.TryGetValue(group.GroupId.HubName, out vssSignalRhub))
      {
        vssSignalRhub = new VssSignalRHub(this.m_cache, group.GroupId.HubName, VssSignalRHubGroupCache.DefaultMaxCacheSize, VssSignalRHubGroupCache.DefaultInactivityInterval);
        this.m_hubs.Add(group.GroupId.HubName, vssSignalRhub);
      }
      vssSignalRhub.Groups.Add(group.GroupId.GroupName, group, true);
    }

    public bool RemoveHubGroup(IVssRequestContext requestContext, VssSignalRHubGroupId groupId)
    {
      VssSignalRHub vssSignalRhub;
      return this.m_hubs.TryGetValue(groupId.HubName, out vssSignalRhub) && vssSignalRhub.Groups.Remove(groupId.GroupName);
    }

    public bool TryGetHubGroup(
      IVssRequestContext requestContext,
      VssSignalRHubGroupId groupId,
      out VssSignalRHubGroup hubGroup)
    {
      VssSignalRHub vssSignalRhub;
      if (this.m_hubs.TryGetValue(groupId.HubName, out vssSignalRhub))
        return vssSignalRhub.Groups.TryGetValue(groupId.GroupName, out hubGroup);
      hubGroup = (VssSignalRHubGroup) null;
      return false;
    }
  }
}
