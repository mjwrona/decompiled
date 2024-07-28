// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineWebHookPublisherSqlComponent
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class PipelineWebHookPublisherSqlComponent : DeploymentSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PipelineWebHookPublisherSqlComponent>(1)
    }, "DeploymentPipelineWebHookPublisher", "Deployment");

    public PipelineWebHookPublisherSqlComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public void CreateWebHookPublisher(
      IVssRequestContext requestContext,
      PipelineWebHookPublisher publisher)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (CreateWebHookPublisher)))
      {
        this.PrepareStoredProcedure("Deployment.prc_CreatePipelineWebHookPublisher");
        this.BindDataspaceId(publisher.Project.Id);
        this.BindGuid("@webHookId", publisher.WebHookId);
        this.BindInt("@pipelineDefinitionId", publisher.PipelineDefinitionId);
        this.BindString("@payloadUrl", publisher.PayloadUrl, 2048, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    public void DeleteWebHookPublisher(
      IVssRequestContext requestContext,
      PipelineWebHookPublisher publisher)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (DeleteWebHookPublisher)))
      {
        this.PrepareStoredProcedure("Deployment.prc_DeletePipelineWebHookPublisher");
        this.BindDataspaceId(publisher.Project.Id);
        this.BindGuid("@webHookId", publisher.WebHookId);
        this.BindInt("@pipelineDefinitionId", publisher.PipelineDefinitionId);
        this.ExecuteNonQuery();
      }
    }

    public PipelineWebHookPublisher GetWebHookPublisher(
      IVssRequestContext requestContext,
      PipelineWebHookPublisher publisher)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetWebHookPublisher)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineWebHookPublisher");
        this.BindDataspaceId(publisher.Project.Id);
        this.BindGuid("@webHookId", publisher.WebHookId);
        this.BindInt("@pipelineDefinitionId", publisher.PipelineDefinitionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<PipelineWebHookPublisher>((ObjectBinder<PipelineWebHookPublisher>) new PipelineWebHookPublisherBinder(this));
          return resultCollection.GetCurrent<PipelineWebHookPublisher>().Items.FirstOrDefault<PipelineWebHookPublisher>();
        }
      }
    }
  }
}
