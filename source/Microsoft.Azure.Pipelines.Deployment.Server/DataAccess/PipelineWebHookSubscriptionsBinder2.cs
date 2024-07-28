// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineWebHookSubscriptionsBinder2
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class PipelineWebHookSubscriptionsBinder2 : ObjectBinder<PipelineWebHookSubscription>
  {
    private SqlColumnBinder m_pipelineDefinitionId = new SqlColumnBinder("PipelineDefinitionId");

    public PipelineWebHookSubscriptionsBinder2(PipelineWebHookSqlComponent2 component) => this.m_sqlComponent = component;

    protected override PipelineWebHookSubscription Bind() => new PipelineWebHookSubscription()
    {
      PipelineDefinitionId = this.m_pipelineDefinitionId.GetInt32((IDataReader) this.Reader)
    };

    protected PipelineWebHookSqlComponent2 m_sqlComponent { get; }
  }
}
