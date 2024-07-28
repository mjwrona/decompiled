// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineWebHookSqlComponent
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class PipelineWebHookSqlComponent : DeploymentSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<PipelineWebHookSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<PipelineWebHookSqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<PipelineWebHookSqlComponent3>(3)
    }, "DeploymentPipelineWebHook", "Deployment");

    public PipelineWebHookSqlComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void CreateWebHook(WebHook webHook)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (CreateWebHook)))
      {
        this.PrepareStoredProcedure("Deployment.prc_CreatePipelineWebHook");
        this.BindGuid("@webHookId", webHook.WebHookId);
        this.BindString("@artifactType", webHook.ArtifactType, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindString("@uniqueResourceIdentifier", webHook.UniqueArtifactIdentifier, 2048, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindGuid("@connectionId", webHook.ConnectionId);
        this.ExecuteNonQuery();
      }
    }

    public virtual void DeleteWebHook(WebHook webHook)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (DeleteWebHook)))
      {
        this.PrepareStoredProcedure("Deployment.prc_DeletePipelineWebHook");
        this.BindGuid("@webHookId", webHook.WebHookId);
        this.ExecuteNonQuery();
      }
    }

    public virtual WebHook GetWebHook(string uniqueArtifactIdentifier, bool includeSubscriptions)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetWebHook)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineWebHook");
        this.BindString("@uniqueResourceIdentifier", uniqueArtifactIdentifier, 2048, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindBoolean("@includeSubscriptions", includeSubscriptions);
        return this.GetWebHook(includeSubscriptions);
      }
    }

    public virtual WebHook GetWebHook(Guid webHookId, bool includeSubscriptions)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetWebHook)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineWebHook");
        this.BindGuid("@webHookId", webHookId);
        this.BindBoolean("@includeSubscriptions", includeSubscriptions);
        return this.GetWebHook(includeSubscriptions);
      }
    }

    private WebHook GetWebHook(bool includeSubscriptions)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WebHook>((ObjectBinder<WebHook>) new PipelineWebHookBinder());
        WebHook webHook = resultCollection.GetCurrent<WebHook>().Items.FirstOrDefault<WebHook>();
        if (webHook != null & includeSubscriptions)
        {
          resultCollection.AddBinder<PipelineWebHookSubscription>((ObjectBinder<PipelineWebHookSubscription>) new PipelineWebHookSubscriptionsBinder(this));
          resultCollection.NextResult();
          foreach (PipelineWebHookSubscription hookSubscription in resultCollection.GetCurrent<PipelineWebHookSubscription>().Items)
          {
            if (hookSubscription != null)
              webHook.Subscriptions.Add((IWebHookSubscription) hookSubscription);
          }
        }
        return webHook;
      }
    }

    public virtual WebHook GetIncomingWebHook(string webHookName, bool includeSubscriptions) => throw new ServiceVersionNotSupportedException("DeploymentPipelineWebHook", 1, 3);

    public virtual Guid CreateWebHookName(Guid webHookId, string webHookName) => throw new ServiceVersionNotSupportedException("DeploymentPipelineWebHook", 1, 3);

    public virtual void DeleteWebHookName(WebHook webHook) => throw new ServiceVersionNotSupportedException("DeploymentPipelineWebHook", 1, 3);
  }
}
