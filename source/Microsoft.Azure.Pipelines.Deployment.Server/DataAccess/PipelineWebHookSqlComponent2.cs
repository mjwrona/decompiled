// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineWebHookSqlComponent2
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
  public class PipelineWebHookSqlComponent2 : PipelineWebHookSqlComponent
  {
    public override WebHook GetWebHook(string uniqueArtifactIdentifier, bool includeSubscriptions)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetWebHook)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineWebHook");
        this.BindString("@uniqueResourceIdentifier", uniqueArtifactIdentifier, 2048, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindBoolean("@includeSubscriptions", includeSubscriptions);
        return this.GetWebHook(includeSubscriptions);
      }
    }

    public override WebHook GetWebHook(Guid webHookId, bool includeSubscriptions)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetWebHook)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineWebHook");
        this.BindGuid("@webHookId", webHookId);
        this.BindBoolean("@includeSubscriptions", includeSubscriptions);
        return this.GetWebHook(includeSubscriptions);
      }
    }

    protected virtual WebHook GetWebHook(bool includeSubscriptions)
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<WebHook>((ObjectBinder<WebHook>) new PipelineWebHookBinder());
        WebHook webHook = resultCollection.GetCurrent<WebHook>().Items.FirstOrDefault<WebHook>();
        if (webHook != null & includeSubscriptions)
        {
          resultCollection.AddBinder<PipelineWebHookSubscription>((ObjectBinder<PipelineWebHookSubscription>) new PipelineWebHookSubscriptionsBinder2(this));
          resultCollection.NextResult();
          foreach (PipelineWebHookSubscription hookSubscription in resultCollection.GetCurrent<PipelineWebHookSubscription>().Items)
            webHook.Subscriptions.Add((IWebHookSubscription) hookSubscription);
        }
        return webHook;
      }
    }
  }
}
