// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess.OrchestrationHubDescriptionBinder
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Concurrent;
using System.Data;

namespace Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess
{
  internal sealed class OrchestrationHubDescriptionBinder : ObjectBinder<OrchestrationHubDescription>
  {
    private SqlColumnBinder m_hubType = new SqlColumnBinder("HubType");
    private SqlColumnBinder m_hubName = new SqlColumnBinder("HubName");
    private SqlColumnBinder m_compressionStyle = new SqlColumnBinder("CompressionStyle");
    private SqlColumnBinder m_compressionThreshold = new SqlColumnBinder("CompressionThreshold");
    private SqlColumnBinder m_maxConcurrentActivities = new SqlColumnBinder("MaxConcurrentActivities");
    private SqlColumnBinder m_maxConcurrentOrchestrations = new SqlColumnBinder("MaxConcurrentOrchestrations");
    private SqlColumnBinder m_activityDispatcherId = new SqlColumnBinder("ActivityDispatcherId");
    private SqlColumnBinder m_orchestrationDispatcherId = new SqlColumnBinder("OrchestrationDispatcherId");

    protected override OrchestrationHubDescription Bind()
    {
      OrchestrationHubDescription orchestrationHubDescription1 = new OrchestrationHubDescription()
      {
        HubType = this.m_hubType.GetString((IDataReader) this.Reader, false),
        HubName = this.m_hubName.GetString((IDataReader) this.Reader, false),
        CompressionSettings = new CompressionSettings()
        {
          Style = (CompressionStyle) this.m_compressionStyle.GetByte((IDataReader) this.Reader),
          ThresholdInBytes = this.m_compressionThreshold.GetInt32((IDataReader) this.Reader, 0)
        },
        MaxConcurrentActivities = this.m_maxConcurrentActivities.GetInt32((IDataReader) this.Reader),
        MaxConcurrentOrchestrations = this.m_maxConcurrentOrchestrations.GetInt32((IDataReader) this.Reader)
      };
      OrchestrationHubDescription orchestrationHubDescription2 = orchestrationHubDescription1;
      OrchestrationDispatcherDescriptor dispatcherDescriptor1 = new OrchestrationDispatcherDescriptor();
      dispatcherDescriptor1.HubName = orchestrationHubDescription1.HubName;
      dispatcherDescriptor1.JobId = this.m_orchestrationDispatcherId.GetGuid((IDataReader) this.Reader, false);
      orchestrationHubDescription2.OrchestrationDispatcher = dispatcherDescriptor1;
      ConcurrentDictionary<string, ActivityDispatcherDescriptor> activityDispatchers = orchestrationHubDescription1.ActivityDispatchers;
      string empty = string.Empty;
      ActivityDispatcherDescriptor dispatcherDescriptor2 = new ActivityDispatcherDescriptor();
      dispatcherDescriptor2.HubName = orchestrationHubDescription1.HubName;
      dispatcherDescriptor2.JobId = this.m_activityDispatcherId.GetGuid((IDataReader) this.Reader, false);
      activityDispatchers[empty] = dispatcherDescriptor2;
      return orchestrationHubDescription1;
    }
  }
}
