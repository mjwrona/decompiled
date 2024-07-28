// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DeploymentGateBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DeploymentGateBinder : ReleaseManagementObjectBinderBase<DeploymentGate>
  {
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder releaseEnvironmentId = new SqlColumnBinder("ReleaseEnvironmentId");
    private SqlColumnBinder deploymentId = new SqlColumnBinder("DeploymentId");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder gateType = new SqlColumnBinder("GateType");
    private SqlColumnBinder runPlanId = new SqlColumnBinder("RunPlanId");
    private SqlColumnBinder stepId = new SqlColumnBinder("StepId");
    private SqlColumnBinder startedOn = new SqlColumnBinder("StartedOn");
    private SqlColumnBinder lastModifiedOn = new SqlColumnBinder("LastModifiedOn");
    private SqlColumnBinder stabilizationCompletedOn = new SqlColumnBinder("StabilizationCompletedOn");

    public DeploymentGateBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override DeploymentGate Bind()
    {
      DateTime dateTime = this.stabilizationCompletedOn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      DeploymentGate deploymentGate1 = new DeploymentGate()
      {
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        ReleaseEnvironmentId = this.releaseEnvironmentId.GetInt32((IDataReader) this.Reader),
        DeploymentId = this.deploymentId.GetInt32((IDataReader) this.Reader),
        RunPlanId = new Guid?(this.runPlanId.GetGuid((IDataReader) this.Reader, true)),
        Status = (GateStatus) this.status.GetByte((IDataReader) this.Reader),
        GateType = (EnvironmentStepType) this.gateType.GetByte((IDataReader) this.Reader),
        ReleaseEnvironmentStepId = this.stepId.GetInt32((IDataReader) this.Reader),
        StartedOn = new DateTime?(this.startedOn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue)),
        LastModifiedOn = new DateTime?(this.lastModifiedOn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue)),
        StabilizationCompletedOn = new DateTime?()
      };
      Guid? nullable1 = deploymentGate1.RunPlanId;
      Guid empty = Guid.Empty;
      if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
      {
        DeploymentGate deploymentGate2 = deploymentGate1;
        nullable1 = new Guid?();
        Guid? nullable2 = nullable1;
        deploymentGate2.RunPlanId = nullable2;
      }
      if (dateTime != DateTime.MinValue)
        deploymentGate1.StabilizationCompletedOn = new DateTime?(dateTime);
      return deploymentGate1;
    }
  }
}
