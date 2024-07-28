// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineWebHookPublisherBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class PipelineWebHookPublisherBinder : ObjectBinder<PipelineWebHookPublisher>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_webHookId = new SqlColumnBinder("WebHookId");
    private SqlColumnBinder m_pipelineDefinitionId = new SqlColumnBinder("PipelineDefinitionId");
    private SqlColumnBinder m_payloadUrl = new SqlColumnBinder("PayloadUrl");

    public PipelineWebHookPublisherBinder(PipelineWebHookPublisherSqlComponent component) => this.m_sqlComponent = component;

    protected override PipelineWebHookPublisher Bind()
    {
      PipelineWebHookPublisher webHookPublisher = new PipelineWebHookPublisher();
      Guid guid = this.m_dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.m_sqlComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      webHookPublisher.Project = new ProjectInfo()
      {
        Id = guid
      };
      webHookPublisher.WebHookId = this.m_webHookId.GetGuid((IDataReader) this.Reader, false);
      webHookPublisher.PipelineDefinitionId = this.m_pipelineDefinitionId.GetInt32((IDataReader) this.Reader);
      webHookPublisher.PayloadUrl = this.m_payloadUrl.GetString((IDataReader) this.Reader, true);
      return webHookPublisher;
    }

    protected PipelineWebHookPublisherSqlComponent m_sqlComponent { get; }
  }
}
