// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CollectionBuildOrchestrationDataBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class CollectionBuildOrchestrationDataBinder : ObjectBinder<BuildOrchestrationData>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_orchestrationId = new SqlColumnBinder("OrchestrationId");
    private SqlColumnBinder m_orchestrationType = new SqlColumnBinder("OrchestrationType");

    protected override BuildOrchestrationData Bind() => new BuildOrchestrationData()
    {
      DataspaceId = this.m_dataspaceId.GetInt32((IDataReader) this.Reader),
      BuildId = this.m_buildId.GetInt32((IDataReader) this.Reader),
      Plan = new TaskOrchestrationPlanReference()
      {
        PlanId = this.m_orchestrationId.GetGuid((IDataReader) this.Reader),
        OrchestrationType = new int?(this.m_orchestrationType.GetInt32((IDataReader) this.Reader))
      }
    };
  }
}
