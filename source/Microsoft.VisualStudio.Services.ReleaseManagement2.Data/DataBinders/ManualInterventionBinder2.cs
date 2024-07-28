// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ManualInterventionBinder2
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

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ManualInterventionBinder2 : ManualInterventionBinder
  {
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder releaseEnvironmentId = new SqlColumnBinder("ReleaseEnvironmentId");
    private SqlColumnBinder releaseDeployPhaseId = new SqlColumnBinder("ReleaseDeployPhaseId");
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder approver = new SqlColumnBinder("Approver");
    private SqlColumnBinder comments = new SqlColumnBinder("Comments");
    private SqlColumnBinder activityData = new SqlColumnBinder("TaskActivityData");
    private SqlColumnBinder createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder modifiedOn = new SqlColumnBinder("ModifiedOn");
    private SqlColumnBinder instructions = new SqlColumnBinder("Instructions");
    private SqlColumnBinder releaseDeployPhaseRank = new SqlColumnBinder("ReleaseDeployPhaseRank");
    private SqlColumnBinder releaseName = new SqlColumnBinder("ReleaseName");
    private SqlColumnBinder environmentName = new SqlColumnBinder("ReleaseEnvironmentName");
    private SqlColumnBinder definitionId = new SqlColumnBinder("ReleaseDefinitionId");
    private SqlColumnBinder definitionName = new SqlColumnBinder("ReleaseDefinitionName");
    private SqlColumnBinder definitionPath = new SqlColumnBinder("ReleaseDefinitionPath");
    private SqlColumnBinder timeoutJobId = new SqlColumnBinder("TimeoutJobId");

    protected override ManualIntervention Bind()
    {
      TaskActivityData taskActivityData = ServerModelUtility.FromString<TaskActivityData>(this.activityData.GetString((IDataReader) this.Reader, false));
      this.releaseDeployPhaseRank.GetInt32((IDataReader) this.Reader);
      return new ManualIntervention()
      {
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        ReleaseName = this.releaseName.GetString((IDataReader) this.Reader, false),
        ReleaseEnvironmentId = this.releaseEnvironmentId.GetInt32((IDataReader) this.Reader),
        ReleaseEnvironmentName = this.environmentName.GetString((IDataReader) this.Reader, false),
        ReleaseDefinitionId = this.definitionId.GetInt32((IDataReader) this.Reader),
        ReleaseDefinitionName = this.definitionName.GetString((IDataReader) this.Reader, false),
        ReleaseDefinitionPath = PathHelper.DBPathToServerPath(this.definitionPath.GetString((IDataReader) this.Reader, string.Empty)),
        ReleaseDeployPhaseId = this.releaseDeployPhaseId.GetInt32((IDataReader) this.Reader),
        Id = this.id.GetInt32((IDataReader) this.Reader),
        Approver = this.approver.GetGuid((IDataReader) this.Reader, true),
        Comments = this.comments.GetString((IDataReader) this.Reader, true),
        Status = (ManualInterventionStatus) this.status.GetByte((IDataReader) this.Reader),
        CreatedOn = this.createdOn.GetDateTime((IDataReader) this.Reader),
        ModifiedOn = this.modifiedOn.GetDateTime((IDataReader) this.Reader),
        TaskActivityData = taskActivityData,
        Instructions = this.instructions.GetString((IDataReader) this.Reader, true),
        TimeoutJobId = this.timeoutJobId.GetGuid((IDataReader) this.Reader, true, Guid.Empty)
      };
    }
  }
}
