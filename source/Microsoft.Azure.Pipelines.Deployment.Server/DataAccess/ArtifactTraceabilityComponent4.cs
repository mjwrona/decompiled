// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactTraceabilityComponent4
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class ArtifactTraceabilityComponent4 : ArtifactTraceabilityComponent3
  {
    public override DownloadTaskTraceabilityError AddArtifactTraceabilityForDownloadTask(
      ArtifactTraceabilityData data)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (AddArtifactTraceabilityForDownloadTask)))
      {
        this.PrepareStoredProcedure("Deployment.prc_AddArtifactTraceabilityForDownloadTask");
        this.BindInt("@dataspaceId", this.GetDataspaceId(data.ProjectId));
        this.BindBoolean("@isSelfArtifact", data.IsSelfArtifact);
        if (data.IsSelfArtifact)
        {
          this.BindString("@artifactType", data.ArtifactType, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
          this.BindString("@uniqueResourceIdentifier", data.UniqueResourceIdentifier, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
          if (data.ArtifactConnectionData != null)
          {
            Guid id = data.ArtifactConnectionData.Id;
            this.BindGuid("@connectionId", data.ArtifactConnectionData.Id);
          }
          this.BindString("@versionId", data.ArtifactVersionId, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        }
        this.BindString("@subArtifactName", data.ArtifactName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBoolean("@downloadAllArtifacts", data.DownloadAllArtifacts);
        this.BindInt("@pipelineDefinitionId", data.PipelineDefinitionId);
        this.BindInt("@pipelineRunId", data.PipelineRunId);
        this.BindString("@artifactAlias", data.ArtifactAlias, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@jobId", data.JobId, 520, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@jobName", data.JobName, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
      return DownloadTaskTraceabilityError.NoError;
    }
  }
}
