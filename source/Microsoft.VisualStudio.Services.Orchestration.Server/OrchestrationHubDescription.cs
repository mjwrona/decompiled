// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationHubDescription
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  public class OrchestrationHubDescription
  {
    private ConcurrentDictionary<string, ActivityDispatcherDescriptor> m_activityDispatchers;

    public OrchestrationHubDescription()
    {
      this.MaxConcurrentActivities = 20;
      this.MaxConcurrentOrchestrations = 100;
    }

    public int HubId { get; internal set; }

    public string HubType { get; set; }

    public string HubName { get; set; }

    public CompressionSettings CompressionSettings { get; set; }

    public int MaxConcurrentActivities { get; set; }

    public int MaxConcurrentOrchestrations { get; set; }

    public ConcurrentDictionary<string, ActivityDispatcherDescriptor> ActivityDispatchers
    {
      get
      {
        if (this.m_activityDispatchers == null)
          this.m_activityDispatchers = new ConcurrentDictionary<string, ActivityDispatcherDescriptor>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_activityDispatchers;
      }
    }

    public OrchestrationDispatcherDescriptor OrchestrationDispatcher { get; set; }
  }
}
