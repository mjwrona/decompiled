// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ManualInterventionListBinder
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
  public class ManualInterventionListBinder : ReleaseManagementObjectBinderBase<ManualIntervention>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder activityData = new SqlColumnBinder("TaskActivityData");
    private SqlColumnBinder approver = new SqlColumnBinder("Approver");
    private SqlColumnBinder comments = new SqlColumnBinder("Comments");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder modifiedOn = new SqlColumnBinder("ModifiedOn");
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder releaseEnvironmentId = new SqlColumnBinder("ReleaseEnvironmentId");
    private SqlColumnBinder releaseDeployPhaseId = new SqlColumnBinder("ReleaseDeployPhaseId");
    private SqlColumnBinder instructions = new SqlColumnBinder("Instructions");

    public ManualInterventionListBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ManualIntervention Bind()
    {
      string str = this.activityData.GetString((IDataReader) this.Reader, false);
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      return new ManualIntervention()
      {
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        ReleaseEnvironmentId = this.releaseEnvironmentId.GetInt32((IDataReader) this.Reader),
        ReleaseDeployPhaseId = this.releaseDeployPhaseId.GetInt32((IDataReader) this.Reader),
        Id = this.id.GetInt32((IDataReader) this.Reader),
        Approver = this.approver.GetGuid((IDataReader) this.Reader, true),
        Comments = this.comments.GetString((IDataReader) this.Reader, true),
        TaskActivityData = ServerModelUtility.FromString<TaskActivityData>(str),
        Status = (ManualInterventionStatus) this.status.GetByte((IDataReader) this.Reader),
        CreatedOn = this.createdOn.GetDateTime((IDataReader) this.Reader),
        ModifiedOn = this.modifiedOn.GetDateTime((IDataReader) this.Reader),
        Instructions = this.instructions.GetString((IDataReader) this.Reader, string.Empty)
      };
    }
  }
}
