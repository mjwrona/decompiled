// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess.OrchestrationHubMessageCountBinder
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess
{
  internal class OrchestrationHubMessageCountBinder : ObjectBinder<OrchestrationHubMessageCount>
  {
    private SqlColumnBinder m_hubName = new SqlColumnBinder("HubName");
    private SqlColumnBinder m_activityDispatcherId = new SqlColumnBinder("ActivityDispatcherId");
    private SqlColumnBinder m_orchestrationDispatcherId = new SqlColumnBinder("OrchestrationDispatcherId");
    private SqlColumnBinder m_activityMessageCount = new SqlColumnBinder("ActivityMessageCount");
    private SqlColumnBinder m_orchestrationMessageCount = new SqlColumnBinder("OrchestrationMessageCount");

    protected override OrchestrationHubMessageCount Bind() => new OrchestrationHubMessageCount()
    {
      HubName = this.m_hubName.GetString((IDataReader) this.Reader, false),
      ActivityDispatcherId = this.m_activityDispatcherId.GetGuid((IDataReader) this.Reader),
      OrchestrationDispatcherId = this.m_orchestrationDispatcherId.GetGuid((IDataReader) this.Reader),
      ActivityMessagesCount = this.m_activityMessageCount.GetInt32((IDataReader) this.Reader),
      OrchestrationMessagesCount = this.m_orchestrationMessageCount.GetInt32((IDataReader) this.Reader)
    };
  }
}
