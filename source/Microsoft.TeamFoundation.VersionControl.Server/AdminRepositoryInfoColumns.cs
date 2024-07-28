// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.AdminRepositoryInfoColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class AdminRepositoryInfoColumns : VersionControlObjectBinder<AdminRepositoryInfo>
  {
    private SqlColumnBinder identityCount = new SqlColumnBinder("IdentityCount");
    private SqlColumnBinder workspaceType = new SqlColumnBinder("WorkspaceType");
    private SqlColumnBinder workspaceCount = new SqlColumnBinder("WorkspaceCount");
    private SqlColumnBinder itemCount = new SqlColumnBinder("ItemCount");
    private SqlColumnBinder itemType = new SqlColumnBinder("ItemType");
    private SqlColumnBinder maxChangesetId = new SqlColumnBinder("MaxChangeSetID");
    private SqlColumnBinder pendingChangeCount = new SqlColumnBinder("PendingChangeCount");

    protected override AdminRepositoryInfo Bind()
    {
      AdminRepositoryInfo adminRepositoryInfo = new AdminRepositoryInfo();
      adminRepositoryInfo.UserCount = this.identityCount.GetInt32((IDataReader) this.Reader);
      this.Reader.NextResult();
      adminRepositoryInfo.ShelvesetDeletedCount = 0;
      while (this.Reader.Read())
      {
        if ((byte) 1 == this.workspaceType.GetByte((IDataReader) this.Reader))
          adminRepositoryInfo.ShelvesetCount = this.workspaceCount.GetInt32((IDataReader) this.Reader);
        else
          adminRepositoryInfo.WorkspaceCount = this.workspaceCount.GetInt32((IDataReader) this.Reader);
      }
      this.Reader.NextResult();
      while (this.Reader.Read())
      {
        if ((byte) 1 == this.itemType.GetByte((IDataReader) this.Reader))
          adminRepositoryInfo.FolderCount = this.itemCount.GetInt32((IDataReader) this.Reader);
        else
          adminRepositoryInfo.FileCount = this.itemCount.GetInt32((IDataReader) this.Reader);
      }
      this.Reader.NextResult();
      this.Reader.Read();
      adminRepositoryInfo.MaxChangesetID = this.maxChangesetId.GetInt32((IDataReader) this.Reader);
      this.Reader.NextResult();
      this.Reader.Read();
      adminRepositoryInfo.PendingChangeCount = this.pendingChangeCount.GetInt32((IDataReader) this.Reader);
      return adminRepositoryInfo;
    }
  }
}
