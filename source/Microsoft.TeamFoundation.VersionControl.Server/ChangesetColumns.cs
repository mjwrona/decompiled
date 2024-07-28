// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangesetColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ChangesetColumns : VersionControlObjectBinder<Changeset>
  {
    private SqlColumnBinder changesetId = new SqlColumnBinder("changeSetId");
    private SqlColumnBinder ownerId = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder creationDate = new SqlColumnBinder("CreationDate");
    private SqlColumnBinder comment = new SqlColumnBinder("Comment");
    private SqlColumnBinder committerId = new SqlColumnBinder("CommitterId");
    private SqlColumnBinder overrideComment = new SqlColumnBinder("OverrideComment");
    private SqlColumnBinder checkinNoteId = new SqlColumnBinder("CheckInNoteId");

    protected override Changeset Bind() => new Changeset()
    {
      CreationDate = this.creationDate.GetDateTime((IDataReader) this.Reader),
      ChangesetId = this.changesetId.GetInt32((IDataReader) this.Reader),
      ownerId = this.ownerId.GetGuid((IDataReader) this.Reader, true),
      Comment = this.comment.GetString((IDataReader) this.Reader, true),
      committerId = this.committerId.GetGuid((IDataReader) this.Reader),
      checkinNoteId = this.checkinNoteId.GetInt32((IDataReader) this.Reader, 0),
      PolicyOverride = {
        Comment = this.overrideComment.GetString((IDataReader) this.Reader, true)
      }
    };
  }
}
