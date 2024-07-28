// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent8
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent8 : DeploymentSqlComponent7
  {
    public override DeploymentGate AddDeploymentGate(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int deploymentId,
      int stepId,
      EnvironmentStepType gateType)
    {
      this.PrepareStoredProcedure("Release.prc_AddDeploymentGate", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (deploymentId), deploymentId);
      this.BindInt(nameof (stepId), stepId);
      this.BindByte(nameof (gateType), (byte) gateType);
      return this.GetDeploymentGateObject();
    }

    public override DeploymentGate UpdateDeploymentGate(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      GateStatus status,
      Guid? runPlanId,
      DeploymentOperationStatus operationStatus,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateDeploymentGate", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (stepId), stepId);
      this.BindByte(nameof (status), (byte) status);
      this.BindNullableGuid(nameof (runPlanId), runPlanId);
      this.BindInt(nameof (operationStatus), (int) operationStatus);
      this.BindGuid(nameof (changedBy), changedBy);
      return this.GetDeploymentGateObject();
    }

    public override DeploymentGate GetDeploymentGate(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int stepId)
    {
      this.PrepareStoredProcedure("Release.prc_GetDeploymentGate", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (stepId), stepId);
      return this.GetDeploymentGateObject();
    }

    public override DeploymentGate HandleGreenlightingStabilizationCompletion(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int stepId,
      Guid runPlanId)
    {
      this.PrepareStoredProcedure("Release.prc_HandleGreenlightingStabilizationCompletion", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (stepId), stepId);
      return this.GetDeploymentGateObject();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Too heavy to be a property")]
    protected virtual DeploymentGate GetDeploymentGateObject()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentGate>((ObjectBinder<DeploymentGate>) this.GetDeploymentGateBinder());
        return resultCollection.GetCurrent<DeploymentGate>().Items.FirstOrDefault<DeploymentGate>();
      }
    }
  }
}
