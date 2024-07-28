// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.WorkspaceMappingBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class WorkspaceMappingBinder : BuildObjectBinder<WorkspaceMapping>
  {
    private SqlColumnBinder workspaceId = new SqlColumnBinder("WorkspaceId");
    private SqlColumnBinder serverItem = new SqlColumnBinder("ServerItem");
    private SqlColumnBinder localItem = new SqlColumnBinder("LocalItem");
    private SqlColumnBinder mappingType = new SqlColumnBinder("MappingType");
    private SqlColumnBinder depth = new SqlColumnBinder("Depth");

    protected override WorkspaceMapping Bind() => new WorkspaceMapping()
    {
      WorkspaceId = this.workspaceId.GetInt32((IDataReader) this.Reader),
      ServerItem = this.serverItem.GetVersionControlPath(this.Reader, false),
      LocalItem = this.localItem.GetLocalPath(this.Reader, true),
      MappingType = this.mappingType.GetWorkspaceMappingType(this.Reader),
      Depth = this.depth.GetInt32((IDataReader) this.Reader)
    };
  }
}
