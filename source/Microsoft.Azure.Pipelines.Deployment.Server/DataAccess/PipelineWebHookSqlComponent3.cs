// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineWebHookSqlComponent3
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class PipelineWebHookSqlComponent3 : PipelineWebHookSqlComponent2
  {
    public override WebHook GetIncomingWebHook(string webHookName, bool includeSubscriptions)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetIncomingWebHook)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetPipelineWebHook");
        this.BindString("@webHookName", webHookName, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindBoolean("@includeSubscriptions", includeSubscriptions);
        return this.GetWebHook(includeSubscriptions);
      }
    }

    public override Guid CreateWebHookName(Guid webHookId, string webHookName)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (CreateWebHookName)))
      {
        this.PrepareStoredProcedure("Deployment.prc_CreatePipelineWebHookName");
        this.BindString("@webHookName", webHookName, 256, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindGuid("@webHookId", webHookId);
        SqlColumnBinder webHookIdBinder = new SqlColumnBinder("WebHookId");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => webHookIdBinder.GetGuid(reader, false))));
          return resultCollection.GetCurrent<Guid>().Items.FirstOrDefault<Guid>();
        }
      }
    }

    public override void DeleteWebHookName(WebHook webHook)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (DeleteWebHookName)))
      {
        this.PrepareStoredProcedure("Deployment.prc_DeletePipelineWebHookName");
        this.BindGuid("@webHookId", webHook.WebHookId);
        this.ExecuteNonQuery();
      }
    }
  }
}
