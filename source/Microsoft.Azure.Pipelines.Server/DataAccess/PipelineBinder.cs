// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.DataAccess.PipelineBinder
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.Azure.Pipelines.Server.DataAccess
{
  internal class PipelineBinder : ObjectBinder<Pipeline>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_pipelineId = new SqlColumnBinder("PipelineId");
    private SqlColumnBinder m_revision = new SqlColumnBinder("Revision");
    private SqlColumnBinder m_configurationData = new SqlColumnBinder("ConfigurationData");
    private readonly PipelinesComponent m_component;

    public PipelineBinder(PipelinesComponent component) => this.m_component = component;

    protected override Pipeline Bind() => (Pipeline) null;
  }
}
