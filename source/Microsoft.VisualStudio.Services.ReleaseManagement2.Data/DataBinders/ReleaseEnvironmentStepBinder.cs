// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseEnvironmentStepBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseEnvironmentStepBinder : 
    ReleaseManagementObjectBinderBase<ReleaseEnvironmentStep>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder rank = new SqlColumnBinder("Rank");
    private SqlColumnBinder stepType = new SqlColumnBinder("StepType");
    private SqlColumnBinder approverId = new SqlColumnBinder("ApproverId");
    private SqlColumnBinder actualApproverId = new SqlColumnBinder("ActualApproverId");
    private SqlColumnBinder approverComment = new SqlColumnBinder("ApproverComment");
    private SqlColumnBinder definitionEnvironmentId = new SqlColumnBinder("DefinitionEnvironmentId");
    private SqlColumnBinder definitionEnvironmentRank = new SqlColumnBinder("DefinitionEnvironmentRank");
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder releaseEnvironmentId = new SqlColumnBinder("ReleaseEnvironmentId");
    private SqlColumnBinder trialNumber = new SqlColumnBinder("TrialNumber");
    private SqlColumnBinder isAutomated = new SqlColumnBinder("IsAutomated");
    private SqlColumnBinder createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder modifiedOn = new SqlColumnBinder("ModifiedOn");
    private SqlColumnBinder reassignedFromStepId = new SqlColumnBinder("ReassignedFromStepId");
    private SqlColumnBinder logs = new SqlColumnBinder("Logs");
    private SqlColumnBinder releaseName = new SqlColumnBinder("ReleaseName");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder definitionName = new SqlColumnBinder("DefinitionName");
    private SqlColumnBinder definitionPath = new SqlColumnBinder("DefinitionPath");
    private SqlColumnBinder releaseEnvironmentName = new SqlColumnBinder("ReleaseEnvironmentName");
    private SqlColumnBinder deploymentStartTime = new SqlColumnBinder("DeploymentStartTime");
    private SqlColumnBinder approvalTimeoutJobId = new SqlColumnBinder("ApprovalTimeoutJobId");
    private SqlColumnBinder runPlanId = new SqlColumnBinder("RunPlanId");
    private SqlColumnBinder deploymentLastModifiedOn = new SqlColumnBinder("DeploymentLastModifiedOn");

    public ReleaseEnvironmentStepBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseEnvironmentStep Bind()
    {
      EnvironmentStepType environmentStepType = (EnvironmentStepType) this.stepType.GetByte((IDataReader) this.Reader);
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      DateTime dateTime = this.deploymentStartTime.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      return new ReleaseEnvironmentStep()
      {
        Id = this.id.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        Status = (ReleaseEnvironmentStepStatus) this.status.GetByte((IDataReader) this.Reader),
        Rank = this.rank.GetInt32((IDataReader) this.Reader),
        StepType = environmentStepType,
        ApproverId = this.approverId.GetGuid((IDataReader) this.Reader),
        ActualApproverId = this.actualApproverId.GetGuid((IDataReader) this.Reader),
        ApproverComment = this.approverComment.GetString((IDataReader) this.Reader, false),
        DefinitionEnvironmentId = this.definitionEnvironmentId.GetInt32((IDataReader) this.Reader),
        DefinitionEnvironmentRank = this.definitionEnvironmentRank.GetInt32((IDataReader) this.Reader),
        TrialNumber = this.trialNumber.GetInt32((IDataReader) this.Reader),
        IsAutomated = this.isAutomated.GetBoolean((IDataReader) this.Reader),
        CreatedOn = this.createdOn.GetDateTime((IDataReader) this.Reader),
        ModifiedOn = this.modifiedOn.GetDateTime((IDataReader) this.Reader),
        ReleaseDefinitionId = this.definitionId.GetInt32((IDataReader) this.Reader, 0, 0),
        ReleaseDefinitionName = this.definitionName.GetString((IDataReader) this.Reader, string.Empty),
        ReleaseDefinitionPath = PathHelper.DBPathToServerPath(this.definitionPath.GetString((IDataReader) this.Reader, string.Empty)),
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        ReleaseName = this.releaseName.GetString((IDataReader) this.Reader, string.Empty),
        ReleaseEnvironmentId = this.releaseEnvironmentId.GetInt32((IDataReader) this.Reader),
        ReleaseEnvironmentName = this.releaseEnvironmentName.GetString((IDataReader) this.Reader, string.Empty),
        GroupStepId = this.reassignedFromStepId.GetInt32((IDataReader) this.Reader, 0, 0),
        Logs = this.logs.GetString((IDataReader) this.Reader, true),
        HasStarted = dateTime != DateTime.MinValue,
        DeploymentStartTime = dateTime,
        ApprovalTimeoutJobId = this.approvalTimeoutJobId.GetGuid((IDataReader) this.Reader, true, Guid.Empty),
        RunPlanId = this.runPlanId.GetNullableGuid((IDataReader) this.Reader, new Guid?()),
        DeploymentLastModifiedOn = this.deploymentLastModifiedOn.GetNullableDateTime((IDataReader) this.Reader, new DateTime?())
      };
    }
  }
}
