// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseDeployPhaseBinder
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
  public class ReleaseDeployPhaseBinder : ReleaseManagementObjectBinderBase<ReleaseDeployPhase>
  {
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder releaseEnvironmentId = new SqlColumnBinder("ReleaseEnvironmentId");
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder rank = new SqlColumnBinder("Rank");
    private SqlColumnBinder phaseType = new SqlColumnBinder("PhaseType");
    private SqlColumnBinder attempt = new SqlColumnBinder("Attempt");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder runPlanId = new SqlColumnBinder("RunPlanId");
    private SqlColumnBinder deploymentStartTime = new SqlColumnBinder("DeploymentStartTime");
    private SqlColumnBinder logs = new SqlColumnBinder("Logs");
    private SqlColumnBinder deploymentId = new SqlColumnBinder("DeploymentId");
    private SqlColumnBinder deploymentLastModifiedOn = new SqlColumnBinder("DeploymentLastModifiedOn");

    public ReleaseDeployPhaseBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseDeployPhase Bind()
    {
      DeployPhaseTypes deployPhaseTypes = (DeployPhaseTypes) this.phaseType.GetByte((IDataReader) this.Reader, (byte) 1, (byte) 1);
      ReleaseDeployPhase releaseDeployPhase;
      switch (deployPhaseTypes)
      {
        case DeployPhaseTypes.DeploymentGates:
          releaseDeployPhase = (ReleaseDeployPhase) new ReleaseGatesPhase();
          break;
        default:
          releaseDeployPhase = new ReleaseDeployPhase();
          break;
      }
      Guid? nullable1 = new Guid?(this.runPlanId.GetGuid((IDataReader) this.Reader, true));
      Guid? nullable2 = nullable1;
      Guid empty = Guid.Empty;
      if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
        nullable1 = new Guid?();
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      DateTime dateTime = this.deploymentStartTime.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      int int32 = this.rank.GetInt32((IDataReader) this.Reader);
      int num = int32 == 0 ? 1 : int32;
      releaseDeployPhase.ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader);
      releaseDeployPhase.ProjectId = guid;
      releaseDeployPhase.ReleaseEnvironmentId = this.releaseEnvironmentId.GetInt32((IDataReader) this.Reader);
      releaseDeployPhase.Id = this.id.GetInt32((IDataReader) this.Reader);
      releaseDeployPhase.Rank = num;
      releaseDeployPhase.PhaseType = deployPhaseTypes;
      releaseDeployPhase.Attempt = this.attempt.GetInt32((IDataReader) this.Reader);
      releaseDeployPhase.Status = (DeployPhaseStatus) this.status.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0);
      releaseDeployPhase.RunPlanId = nullable1;
      releaseDeployPhase.DeploymentStartTime = dateTime == DateTime.MinValue ? new DateTime?() : new DateTime?(dateTime);
      releaseDeployPhase.Logs = this.logs.GetString((IDataReader) this.Reader, true);
      releaseDeployPhase.DeploymentId = this.deploymentId.GetInt32((IDataReader) this.Reader, 0, 0);
      releaseDeployPhase.DeploymentLastModifiedOn = this.deploymentLastModifiedOn.GetNullableDateTime((IDataReader) this.Reader, new DateTime?());
      return releaseDeployPhase;
    }
  }
}
