// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseEnvironmentListBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseEnvironmentListBinder : ObjectBinder<ReleaseEnvironment>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder definitionEnvironmentId = new SqlColumnBinder("DefinitionEnvironmentId");
    private SqlColumnBinder rank = new SqlColumnBinder("Rank");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder scheduledDeploymentTime = new SqlColumnBinder("ScheduledDeploymentTime");

    protected override ReleaseEnvironment Bind()
    {
      DateTime dateTime = this.scheduledDeploymentTime.GetDateTime((IDataReader) this.Reader, SqlDateTime.MinValue.Value);
      return new ReleaseEnvironment()
      {
        Id = this.id.GetInt32((IDataReader) this.Reader),
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        Name = this.name.GetString((IDataReader) this.Reader, false),
        DefinitionEnvironmentId = this.definitionEnvironmentId.GetInt32((IDataReader) this.Reader),
        Rank = this.rank.GetInt32((IDataReader) this.Reader),
        Status = (ReleaseEnvironmentStatus) this.status.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0),
        ScheduledDeploymentTime = dateTime == DateTime.MinValue ? new DateTime?() : new DateTime?(dateTime)
      };
    }
  }
}
