// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskOrchestrationPlanReferenceBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal sealed class TaskOrchestrationPlanReferenceBinder : 
    ObjectBinder<TaskOrchestrationPlanReference>
  {
    private SqlColumnBinder m_planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
    private SqlColumnBinder m_artifactUri = new SqlColumnBinder("ArtifactUri");

    protected override TaskOrchestrationPlanReference Bind() => new TaskOrchestrationPlanReference()
    {
      PlanId = this.m_planId.GetGuid((IDataReader) this.Reader),
      Version = 1,
      ArtifactUri = new Uri(this.m_artifactUri.GetString((IDataReader) this.Reader, false), UriKind.Absolute),
      ContainerId = this.m_containerId.GetInt64((IDataReader) this.Reader)
    };
  }
}
