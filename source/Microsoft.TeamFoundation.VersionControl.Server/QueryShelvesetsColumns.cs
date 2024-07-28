// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryShelvesetsColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryShelvesetsColumns : VersionControlObjectBinder<Shelveset>
  {
    internal SqlColumnBinder ownerId = new SqlColumnBinder("OwnerId");
    internal SqlColumnBinder shelvesetName = new SqlColumnBinder("ShelvesetName");
    internal SqlColumnBinder comment = new SqlColumnBinder("Comment");
    internal SqlColumnBinder policyOverrideComment = new SqlColumnBinder("PolicyOverrideComment");
    internal SqlColumnBinder creationDate = new SqlColumnBinder("CreationDate");
    internal SqlColumnBinder shelvesetId = new SqlColumnBinder("ShelvesetId");
    internal SqlColumnBinder checkinNoteId = new SqlColumnBinder("CheckInNoteId");

    protected override Shelveset Bind() => new Shelveset()
    {
      ownerId = this.ownerId.GetGuid((IDataReader) this.Reader),
      Name = this.shelvesetName.GetString((IDataReader) this.Reader, false),
      Comment = this.comment.GetString((IDataReader) this.Reader, true),
      PolicyOverrideComment = this.policyOverrideComment.GetString((IDataReader) this.Reader, true),
      CreationDate = this.creationDate.GetDateTime((IDataReader) this.Reader),
      shelvesetId = this.shelvesetId.GetInt32((IDataReader) this.Reader),
      checkinNoteId = this.checkinNoteId.GetInt32((IDataReader) this.Reader, 0)
    };
  }
}
