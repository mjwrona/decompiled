// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.CollectionPipelineDefinitionTriggerBinder2
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
  public class CollectionPipelineDefinitionTriggerBinder2 : PipelineDefinitionTriggerBinder2
  {
    private PipelineTriggerSqlComponent2 m_sqlComponent;
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");

    public CollectionPipelineDefinitionTriggerBinder2(PipelineTriggerSqlComponent2 component) => this.m_sqlComponent = component;

    protected override PipelineDefinitionTrigger Bind()
    {
      Guid guid = this.m_dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.m_sqlComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      PipelineDefinitionTrigger definitionTrigger = base.Bind();
      if (definitionTrigger != null)
        definitionTrigger.Project = new ProjectInfo()
        {
          Id = guid
        };
      return definitionTrigger;
    }
  }
}
