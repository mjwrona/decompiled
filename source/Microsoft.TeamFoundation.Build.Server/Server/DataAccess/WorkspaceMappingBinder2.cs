// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.WorkspaceMappingBinder2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class WorkspaceMappingBinder2 : BuildObjectBinder<WorkspaceMapping>
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder workspaceId = new SqlColumnBinder("WorkspaceId");
    private SqlColumnBinder serverItem = new SqlColumnBinder("ServerItem");
    private SqlColumnBinder localItem = new SqlColumnBinder("LocalItem");
    private SqlColumnBinder mappingType = new SqlColumnBinder("MappingType");
    private SqlColumnBinder depth = new SqlColumnBinder("Depth");

    public WorkspaceMappingBinder2(BuildSqlResourceComponent component)
      : base(component)
    {
    }

    protected override WorkspaceMapping Bind() => new WorkspaceMapping()
    {
      WorkspaceId = this.workspaceId.GetInt32((IDataReader) this.Reader),
      ServerItem = this.Component.DataspaceDBPathToVersionControlPath(this.serverItem.GetString((IDataReader) this.Reader, false)),
      LocalItem = this.localItem.GetLocalPath(this.Reader, true),
      MappingType = this.mappingType.GetWorkspaceMappingType(this.Reader),
      Depth = this.depth.GetInt32((IDataReader) this.Reader),
      ProjectId = this.Component.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader))
    };
  }
}
