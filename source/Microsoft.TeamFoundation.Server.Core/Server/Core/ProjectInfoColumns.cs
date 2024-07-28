// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectInfoColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Net;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectInfoColumns : ObjectBinder<ProjectInfo>
  {
    private SqlColumnBinder idColumn = new SqlColumnBinder("project_id");
    private SqlColumnBinder nameColumn = new SqlColumnBinder("project_name");
    protected SqlColumnBinder abbreviationColumn = new SqlColumnBinder("project_abbreviation");
    private SqlColumnBinder stateColumn = new SqlColumnBinder("state");
    private SqlColumnBinder lastUpdateColumn = new SqlColumnBinder("last_update");
    private SqlColumnBinder userUpdateColumn = new SqlColumnBinder("user_update");
    protected SqlColumnBinder revisionColumn = new SqlColumnBinder("Revision");
    protected SqlColumnBinder descriptionColumn = new SqlColumnBinder("ProjectDescription");
    protected SqlColumnBinder projectVisibilityColumn = new SqlColumnBinder("ProjectVisibility");

    protected override ProjectInfo Bind()
    {
      string databasePath = this.nameColumn.GetString((IDataReader) this.Reader, true);
      string abbreviation = this.GetAbbreviation();
      ProjectInfo projectInfo = new ProjectInfo()
      {
        Id = this.idColumn.GetGuid((IDataReader) this.Reader, false),
        Name = !string.IsNullOrEmpty(databasePath) ? DBPath.DatabaseToUserPath(databasePath) : string.Empty,
        Abbreviation = abbreviation != null ? DBPath.DatabaseToUserPath(abbreviation) : (string) null,
        Description = this.GetDescription(),
        State = (ProjectState) System.Enum.Parse(typeof (ProjectState), this.stateColumn.GetString((IDataReader) this.Reader, false)),
        LastUpdateTime = this.lastUpdateColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
        Revision = this.GetRevision(),
        UserId = this.userUpdateColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty),
        Visibility = this.GetProjectVisibility()
      };
      projectInfo.SetProjectUri();
      projectInfo.KnownNames.Add(projectInfo.Name);
      return projectInfo;
    }

    protected virtual string GetAbbreviation() => !this.abbreviationColumn.ColumnExists((IDataReader) this.Reader) ? (string) null : this.abbreviationColumn.GetString((IDataReader) this.Reader, true);

    protected virtual long GetRevision()
    {
      byte[] bytes = this.revisionColumn.ColumnExists((IDataReader) this.Reader) ? this.revisionColumn.GetBytes((IDataReader) this.Reader, false) : (byte[]) null;
      return bytes == null ? 0L : IPAddress.NetworkToHostOrder(BitConverter.ToInt64(bytes, 0));
    }

    protected virtual string GetDescription() => (string) null;

    protected virtual ProjectVisibility GetProjectVisibility() => ProjectVisibility.Private;
  }
}
