// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.MentionRecordBinder
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Mention.Server.Data
{
  internal class MentionRecordBinder : ObjectBinder<MentionRecord>
  {
    private SqlColumnBinder sourceId = new SqlColumnBinder("SourceId");
    private SqlColumnBinder sourceDataspaceId = new SqlColumnBinder("SourceDataspaceId");
    private SqlColumnBinder sourceType = new SqlColumnBinder("SourceType");
    private SqlColumnBinder rawText = new SqlColumnBinder("RawText");
    private SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
    private SqlColumnBinder artifactType = new SqlColumnBinder("ArtifactType");
    private SqlColumnBinder commentId = new SqlColumnBinder("CommentId");
    private SqlColumnBinder mentionAction = new SqlColumnBinder("MentionAction");
    private SqlColumnBinder targetId = new SqlColumnBinder("TargetId");
    private SqlColumnBinder storageKey = new SqlColumnBinder("StorageKey");
    private SqlColumnBinder targetDataspaceId = new SqlColumnBinder("TargetDataspaceId");
    private SqlColumnBinder normalizedSourceId = new SqlColumnBinder("NormalizedSourceId");
    private SqlColumnBinder updateState = new SqlColumnBinder("UpdateState");

    protected override MentionRecord Bind() => new MentionRecord()
    {
      SourceId = this.sourceId.GetString((IDataReader) this.Reader, false),
      SourceDataspaceId = this.sourceDataspaceId.GetInt32((IDataReader) this.Reader),
      SourceType = this.sourceType.GetString((IDataReader) this.Reader, false),
      RawText = this.rawText.GetString((IDataReader) this.Reader, false),
      ArtifactId = this.artifactId.GetString((IDataReader) this.Reader, false),
      ArtifactType = this.artifactType.GetString((IDataReader) this.Reader, false),
      CommentId = new int?(this.commentId.GetInt32((IDataReader) this.Reader)),
      MentionAction = this.mentionAction.GetString((IDataReader) this.Reader, false),
      TargetId = this.targetId.GetString((IDataReader) this.Reader, false),
      StorageKey = this.storageKey.GetNullableGuid((IDataReader) this.Reader),
      TargetDataspaceId = this.targetDataspaceId.GetInt32((IDataReader) this.Reader),
      NormalizedSourceId = this.normalizedSourceId.GetString((IDataReader) this.Reader, false),
      UpdateState = (MentionUpdateState) this.updateState.GetByte((IDataReader) this.Reader)
    };
  }
}
