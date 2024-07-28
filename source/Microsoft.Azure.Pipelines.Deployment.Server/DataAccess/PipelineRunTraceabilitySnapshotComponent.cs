// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineRunTraceabilitySnapshotComponent
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Helpers;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class PipelineRunTraceabilitySnapshotComponent : DeploymentSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PipelineRunTraceabilitySnapshotComponent>(1)
    }, "PipelineRunTraceabilitySnapshot", "Deployment");

    public PipelineRunTraceabilitySnapshotComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public void SaveBaseRunDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineRunTraceabilitySnapshotObject snapshotObject)
    {
      if (snapshotObject != null && snapshotObject.CurrentRunId > 0)
      {
        using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (SaveBaseRunDetails)))
        {
          PipelineRunTraceabilitySnapshot traceabilitySnapshot = snapshotObject.ToPipelineRunTraceabilitySnapshot();
          this.PrepareStoredProcedure("Deployment.prc_SaveBaseRunDetailsForTraceability");
          this.BindInt("@dataspaceId", this.GetDefaultDataspaceId(projectId));
          this.BindInt("@currentRunId", traceabilitySnapshot.CurrentRunId);
          this.BindString("@baseRunArtifactVersions", traceabilitySnapshot.BaseRunArtifactVersions, -1, true, SqlDbType.NVarChar);
          this.BindString("@baseRunDetails", traceabilitySnapshot.BaseRunDetails, -1, true, SqlDbType.NVarChar);
          this.BindNullableInt("@commitsCount", traceabilitySnapshot.CommitsCount);
          this.BindNullableInt("@workItemsCount", traceabilitySnapshot.WorkItemsCount);
          this.ExecuteNonQuery();
        }
      }
      else
        this.TraceIncorrectCurrentRunID(requestContext);
    }

    public PipelineRunTraceabilitySnapshotObject GetRunTraceabilitySnapshot(
      IVssRequestContext requestContext,
      Guid projectId,
      int runId)
    {
      if (runId > 0)
      {
        using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetRunTraceabilitySnapshot)))
        {
          this.PrepareStoredProcedure("Deployment.prc_GetRunTraceabilitySnapshot");
          this.BindInt("@dataspaceId", this.GetDefaultDataspaceId(projectId));
          this.BindInt("@currentRunId", runId);
          using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
          {
            resultCollection.AddBinder<PipelineRunTraceabilitySnapshot>((ObjectBinder<PipelineRunTraceabilitySnapshot>) new PipelineRunTraceabilitySnapshotDataBinder());
            PipelineRunTraceabilitySnapshot snapshotData = resultCollection.GetCurrent<PipelineRunTraceabilitySnapshot>().FirstOrDefault<PipelineRunTraceabilitySnapshot>();
            return snapshotData != null ? snapshotData.ToPipelineRunTraceabilitySnapshotObject() : (PipelineRunTraceabilitySnapshotObject) null;
          }
        }
      }
      else
      {
        this.TraceIncorrectCurrentRunID(requestContext);
        return (PipelineRunTraceabilitySnapshotObject) null;
      }
    }

    private int GetDefaultDataspaceId(Guid dataspaceIdentifier) => this.GetDataspaceId(dataspaceIdentifier, "Default", true);

    private void TraceIncorrectCurrentRunID(IVssRequestContext requestContext) => requestContext.Trace(100161013, TraceLevel.Info, "Deployment", "TraceabilityComponentLayer", "Incorrect currentRunId");
  }
}
