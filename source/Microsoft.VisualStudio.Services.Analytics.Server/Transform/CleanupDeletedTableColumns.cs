// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.CleanupDeletedTableColumns
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class CleanupDeletedTableColumns : ObjectBinder<CleanupDeletedTableResult>
  {
    private SqlColumnBinder CompleteColumn = new SqlColumnBinder("Complete");
    private SqlColumnBinder TableNameColumn = new SqlColumnBinder("TableName");
    private SqlColumnBinder RowsColumn = new SqlColumnBinder("DeletedRows");

    protected override CleanupDeletedTableResult Bind() => new CleanupDeletedTableResult()
    {
      Complete = this.CompleteColumn.GetBoolean((IDataReader) this.Reader),
      TableName = this.TableNameColumn.GetString((IDataReader) this.Reader, true),
      DeletedRows = this.RowsColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
