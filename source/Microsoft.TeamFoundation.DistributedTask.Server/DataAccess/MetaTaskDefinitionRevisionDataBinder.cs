// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MetaTaskDefinitionRevisionDataBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MetaTaskDefinitionRevisionDataBinder : ObjectBinder<MetaTaskDefinitionRevisionData>
  {
    private SqlColumnBinder DefinitionIdColumn = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder DefinitionRevisionColumn = new SqlColumnBinder("DefinitionRevision");
    private SqlColumnBinder ChangedByColumn = new SqlColumnBinder("ChangedBy");
    private SqlColumnBinder ChangedDateColumn = new SqlColumnBinder("ChangedDate");
    private SqlColumnBinder ChangeTypeColumn = new SqlColumnBinder("ChangeType");
    private SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
    private SqlColumnBinder CommentColumn = new SqlColumnBinder("Comment");

    protected override MetaTaskDefinitionRevisionData Bind() => new MetaTaskDefinitionRevisionData()
    {
      DefinitionId = this.DefinitionIdColumn.GetGuid((IDataReader) this.Reader),
      Revision = this.DefinitionRevisionColumn.GetInt32((IDataReader) this.Reader),
      ChangedBy = this.ChangedByColumn.GetGuid((IDataReader) this.Reader),
      ChangedDate = this.ChangedDateColumn.GetDateTime((IDataReader) this.Reader),
      ChangeType = (AuditAction) this.ChangeTypeColumn.GetInt32((IDataReader) this.Reader, 2, 2),
      FileId = this.FileIdColumn.GetInt32((IDataReader) this.Reader),
      Comment = this.CommentColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
