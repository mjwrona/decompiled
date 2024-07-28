// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DeploymentGateRefBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DeploymentGateRefBinder : ObjectBinder<DeploymentGateRef>
  {
    private SqlColumnBinder id = new SqlColumnBinder("StepId");
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder releaseEnvironmentId = new SqlColumnBinder("ReleaseEnvironmentId");
    private SqlColumnBinder deploymentId = new SqlColumnBinder("DeploymentId");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder gateType = new SqlColumnBinder("GateType");
    private SqlColumnBinder runPlanId = new SqlColumnBinder("RunPlanId");
    private SqlColumnBinder stepId = new SqlColumnBinder("StepId");
    private SqlColumnBinder attempt = new SqlColumnBinder("Attempt");
    private SqlColumnBinder environmentName = new SqlColumnBinder("EnvironmentName");
    private SqlColumnBinder environmentRank = new SqlColumnBinder("EnvironmentRank");

    protected override DeploymentGateRef Bind()
    {
      DeploymentGateRef deploymentGateRef1 = new DeploymentGateRef();
      deploymentGateRef1.Id = this.id.GetInt32((IDataReader) this.Reader, 0, 0);
      deploymentGateRef1.ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader);
      deploymentGateRef1.ReleaseEnvironmentId = this.releaseEnvironmentId.GetInt32((IDataReader) this.Reader);
      deploymentGateRef1.DeploymentId = this.deploymentId.GetInt32((IDataReader) this.Reader);
      deploymentGateRef1.RunPlanId = new Guid?(this.runPlanId.GetGuid((IDataReader) this.Reader, true));
      deploymentGateRef1.Status = (GateStatus) this.status.GetByte((IDataReader) this.Reader);
      deploymentGateRef1.GateType = (EnvironmentStepType) this.gateType.GetByte((IDataReader) this.Reader);
      deploymentGateRef1.ReleaseEnvironmentStepId = this.stepId.GetInt32((IDataReader) this.Reader);
      deploymentGateRef1.Attempt = this.attempt.GetInt32((IDataReader) this.Reader);
      deploymentGateRef1.EnvironmentRank = this.environmentRank.GetInt32((IDataReader) this.Reader);
      deploymentGateRef1.EnvironmentName = this.environmentName.GetString((IDataReader) this.Reader, false);
      DeploymentGateRef deploymentGateRef2 = deploymentGateRef1;
      Guid? nullable1 = deploymentGateRef2.RunPlanId;
      Guid empty = Guid.Empty;
      if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
      {
        DeploymentGateRef deploymentGateRef3 = deploymentGateRef2;
        nullable1 = new Guid?();
        Guid? nullable2 = nullable1;
        deploymentGateRef3.RunPlanId = nullable2;
      }
      return deploymentGateRef2;
    }
  }
}
