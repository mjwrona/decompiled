// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DeploymentAttemptDataBinder
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
  public class DeploymentAttemptDataBinder : ReleaseManagementObjectBinderBase<DeploymentAttemptData>
  {
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder("ReleaseDefinitionId");
    private SqlColumnBinder definitionEnvironmentId = new SqlColumnBinder("DefinitionEnvironmentId");
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder releaseEnvironmentId = new SqlColumnBinder("ReleaseEnvironmentId");
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder attempt = new SqlColumnBinder("Attempt");
    private SqlColumnBinder deploymentReason = new SqlColumnBinder("Reason");
    private SqlColumnBinder deploymentStatus = new SqlColumnBinder("DeploymentStatus");
    private SqlColumnBinder operationStatus = new SqlColumnBinder("OperationStatus");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");

    public DeploymentAttemptDataBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override DeploymentAttemptData Bind()
    {
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      return new DeploymentAttemptData()
      {
        DeploymentId = this.id.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader),
        DefinitionEnvironmentId = this.definitionEnvironmentId.GetInt32((IDataReader) this.Reader),
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        ReleaseEnvironmentId = this.releaseEnvironmentId.GetInt32((IDataReader) this.Reader),
        Attempt = this.attempt.GetInt32((IDataReader) this.Reader),
        Reason = (DeploymentReason) this.deploymentReason.GetByte((IDataReader) this.Reader),
        Status = (DeploymentStatus) this.deploymentStatus.GetByte((IDataReader) this.Reader),
        OperationStatus = (DeploymentOperationStatus) this.operationStatus.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
