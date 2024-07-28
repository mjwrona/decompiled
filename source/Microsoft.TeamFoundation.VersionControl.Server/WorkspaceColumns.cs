// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class WorkspaceColumns : VersionControlObjectBinder<WorkspaceInternal>
  {
    private SqlColumnBinder workspaceID = new SqlColumnBinder("WorkspaceId");
    private SqlColumnBinder ownerID = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder workspaceName = new SqlColumnBinder("WorkspaceName");
    private SqlColumnBinder computer = new SqlColumnBinder("Computer");
    private SqlColumnBinder comment = new SqlColumnBinder("Comment");
    private SqlColumnBinder lastAccessDate = new SqlColumnBinder("LastAccessDate");
    private SqlColumnBinder lastMappingsUpdate = new SqlColumnBinder("LastMappingsUpdate");
    private SqlColumnBinder isLocal = new SqlColumnBinder("IsLocal");
    private SqlColumnBinder pendingChangeSignature = new SqlColumnBinder("PendingChangeSig");
    private SqlColumnBinder fileTime = new SqlColumnBinder("FileTime");

    protected override WorkspaceInternal Bind()
    {
      WorkspaceInternal workspaceInternal = new WorkspaceInternal();
      workspaceInternal.Id = this.workspaceID.GetInt32((IDataReader) this.Reader);
      workspaceInternal.OwnerId = this.ownerID.GetGuid((IDataReader) this.Reader);
      workspaceInternal.Name = this.workspaceName.GetString((IDataReader) this.Reader, false);
      workspaceInternal.Computer = this.computer.GetString((IDataReader) this.Reader, false);
      workspaceInternal.Comment = this.comment.GetString((IDataReader) this.Reader, true);
      workspaceInternal.LastAccessDate = this.lastAccessDate.GetDateTime((IDataReader) this.Reader);
      try
      {
        workspaceInternal.LastMappingsUpdate = this.lastMappingsUpdate.GetDateTime((IDataReader) this.Reader);
        workspaceInternal.IsLocal = this.isLocal.GetBoolean((IDataReader) this.Reader);
        workspaceInternal.PendingChangeSignature = this.pendingChangeSignature.GetGuid((IDataReader) this.Reader);
        if (this.fileTime.GetBoolean((IDataReader) this.Reader))
          workspaceInternal.WorkspaceOptions = WorkspaceOptions.SetFileTimeToCheckin;
      }
      catch (IndexOutOfRangeException ex)
      {
      }
      return workspaceInternal;
    }
  }
}
