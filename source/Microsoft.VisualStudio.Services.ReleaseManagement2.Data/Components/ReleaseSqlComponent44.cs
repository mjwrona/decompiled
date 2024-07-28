// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent44
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism for AT/DT")]
  public class ReleaseSqlComponent44 : ReleaseSqlComponent43
  {
    public override ReleaseDeployPhase UpdateReleaseDeployPhase(
      Guid projectId,
      ReleaseDeployPhase releaseDeployPhase,
      DeploymentOperationStatus operationStatus,
      Guid changedBy)
    {
      if (releaseDeployPhase == null)
        throw new ArgumentNullException(nameof (releaseDeployPhase));
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseDeployPhase", projectId);
      this.BindInt("releaseId", releaseDeployPhase.ReleaseId);
      this.BindInt("releaseEnvironmentId", releaseDeployPhase.ReleaseEnvironmentId);
      this.BindInt("releaseDeployPhaseId", releaseDeployPhase.Id);
      this.BindByte("status", (byte) releaseDeployPhase.Status);
      this.BindNullableGuid("runPlanId", releaseDeployPhase.RunPlanId);
      this.BindString("logs", releaseDeployPhase.Logs, 4000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("deploymentAttempt", releaseDeployPhase.Attempt);
      this.BindInt(nameof (operationStatus), (int) operationStatus);
      this.BindGuid("changedby", changedBy);
      return this.GetReleaseDeployPhaseObject();
    }

    public override ReleaseDeployPhase AddReleaseDeployPhase(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int rank,
      int attempt,
      DeployPhaseTypes phaseType,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("Release.prc_AddReleaseDeployPhase", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt("deployPhaseRank", rank);
      this.BindByte("deployPhaseType", (byte) phaseType);
      this.BindInt(nameof (attempt), attempt);
      this.BindGuid("changedby", changedBy);
      return this.GetReleaseDeployPhaseObject();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Too heavy to be a property")]
    protected virtual ReleaseDeployPhase GetReleaseDeployPhaseObject()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetReleaseDeployPhaseBinder());
        ReleaseDeployPhase deployPhaseObject = resultCollection.GetCurrent<ReleaseDeployPhase>().Items.FirstOrDefault<ReleaseDeployPhase>();
        if (deployPhaseObject != null && deployPhaseObject.PhaseType == DeployPhaseTypes.DeploymentGates)
        {
          resultCollection.NextResult();
          resultCollection.AddBinder<DeploymentGate>((ObjectBinder<DeploymentGate>) this.GetDeploymentGateBinder());
          DeploymentGate serverDeploymentGate = resultCollection.GetCurrent<DeploymentGate>().Items.FirstOrDefault<DeploymentGate>();
          ReleaseGatesPhase serverGatesPhase = deployPhaseObject as ReleaseGatesPhase;
          if (serverDeploymentGate != null && serverGatesPhase != null)
            serverGatesPhase.FillReleaseGatesPhaseWithDeploymentGate(serverDeploymentGate);
        }
        return deployPhaseObject;
      }
    }
  }
}
