// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectWatermarkColumns
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
  internal class ProjectWatermarkColumns : ObjectBinder<ProjectInfo>
  {
    private SqlColumnBinder idColumn = new SqlColumnBinder("project_id");
    private SqlColumnBinder nameColumn = new SqlColumnBinder("project_name");
    protected SqlColumnBinder stateColumn = new SqlColumnBinder("state");
    private SqlColumnBinder revisionColumn = new SqlColumnBinder("Revision");

    protected override ProjectInfo Bind()
    {
      string databasePath = this.nameColumn.GetString((IDataReader) this.Reader, true);
      string state = this.GetState();
      ProjectInfo projectInfo = new ProjectInfo()
      {
        Id = this.idColumn.GetGuid((IDataReader) this.Reader, false),
        Name = !string.IsNullOrEmpty(databasePath) ? DBPath.DatabaseToUserPath(databasePath) : string.Empty,
        State = !string.IsNullOrEmpty(state) ? (ProjectState) System.Enum.Parse(typeof (ProjectState), state) : ProjectState.Unchanged,
        Revision = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(this.revisionColumn.GetBytes((IDataReader) this.Reader, false), 0))
      };
      projectInfo.KnownNames.Add(projectInfo.Name);
      return projectInfo;
    }

    protected virtual string GetState() => !this.stateColumn.ColumnExists((IDataReader) this.Reader) ? (string) null : this.stateColumn.GetString((IDataReader) this.Reader, true);
  }
}
