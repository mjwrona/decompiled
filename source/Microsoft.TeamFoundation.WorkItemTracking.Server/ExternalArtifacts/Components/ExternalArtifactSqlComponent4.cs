// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Components.ExternalArtifactSqlComponent4
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.Components
{
  internal class ExternalArtifactSqlComponent4 : ExternalArtifactSqlComponent3
  {
    public override IEnumerable<ExternalDeploymentDataset> GetDeploymentArtifactLinks(int workitemId)
    {
      this.PrepareStoredProcedure("prc_GetDeploymentArtifactLinks");
      this.BindInt("@workitemId", workitemId);
      return (IEnumerable<ExternalDeploymentDataset>) this.ExecuteUnknown<List<ExternalDeploymentDataset>>((System.Func<IDataReader, List<ExternalDeploymentDataset>>) (reader => this.GetExternalDeploymentBinder().BindAll(reader).ToList<ExternalDeploymentDataset>()));
    }

    public override Guid CreateDeploymentArtifactLinks(
      IEnumerable<int> workitemIds,
      ExternalDeploymentDataset deployment)
    {
      this.PrepareStoredProcedure("prc_CreateDeploymentArtifactLinks");
      this.BindInt32Table("@workitemIds", workitemIds);
      this.BindInt("@pipelineId", deployment.PipelineId);
      this.BindString("@pipelineDisplayName", deployment.PipelineDisplayName, 256, false, SqlDbType.NVarChar);
      this.BindString("@pipelineUrl", deployment.PipelineUrl, 2000, false, SqlDbType.NVarChar);
      this.BindInt("@environmentId", deployment.EnvironmentId);
      this.BindString("@environmentDisplayName", deployment.EnvironmentDisplayName, 256, false, SqlDbType.NVarChar);
      this.BindString("@environmentType", deployment.EnvironmentType, 100, false, SqlDbType.NVarChar);
      this.BindInt("@runId", deployment.RunId);
      this.BindInt("@sequenceNumber", deployment.SequenceNumber);
      this.BindString("@displayName", deployment.DisplayName, 800, false, SqlDbType.NVarChar);
      this.BindString("@description", deployment.Description, 4000, false, SqlDbType.NVarChar);
      this.BindString("@status", deployment.Status, 100, false, SqlDbType.NVarChar);
      this.BindString("@group", deployment.Group, 256, false, SqlDbType.NVarChar);
      this.BindString("@url", deployment.Url, 2000, false, SqlDbType.NVarChar);
      this.BindDateTime("@statusDate", deployment.StatusDate);
      this.BindGuid("@createdBy", deployment.CreatedBy);
      object obj = this.ExecuteScalar();
      return obj != null ? (Guid) obj : Guid.Empty;
    }
  }
}
