// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskOrchestrationPlanReferenceBinder2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal sealed class TaskOrchestrationPlanReferenceBinder2 : 
    ObjectBinder<TaskOrchestrationPlanReference>
  {
    private TaskTrackingComponent m_component;
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_planType = new SqlColumnBinder("PlanType");
    private SqlColumnBinder m_planVersion = new SqlColumnBinder("PlanVersion");
    private SqlColumnBinder m_planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder m_artifactUri = new SqlColumnBinder("ArtifactUri");
    private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");

    public TaskOrchestrationPlanReferenceBinder2(TaskTrackingComponent component) => this.m_component = component;

    protected override TaskOrchestrationPlanReference Bind() => new TaskOrchestrationPlanReference()
    {
      ScopeIdentifier = this.m_component.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)),
      PlanType = this.m_planType.GetString((IDataReader) this.Reader, false),
      Version = this.m_planVersion.GetInt32((IDataReader) this.Reader),
      PlanId = this.m_planId.GetGuid((IDataReader) this.Reader),
      ArtifactUri = new Uri(this.m_artifactUri.GetString((IDataReader) this.Reader, false), UriKind.Absolute),
      ContainerId = this.m_containerId.GetInt64((IDataReader) this.Reader)
    };
  }
}
