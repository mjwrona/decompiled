// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.CleanupStreamColumns
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class CleanupStreamColumns : ObjectBinder<CleanupStreamResult>
  {
    private SqlColumnBinder CompleteColumn = new SqlColumnBinder("Complete");
    private SqlColumnBinder RowsColumn = new SqlColumnBinder("DeletedRows");
    private SqlColumnBinder StageBatchesColumn = new SqlColumnBinder("DeletedStageBatches");
    private SqlColumnBinder StreamsColumn = new SqlColumnBinder("DeletedStreams");
    private SqlColumnBinder ProcessBatchesColumn = new SqlColumnBinder("DeletedProcessBatches");

    protected override CleanupStreamResult Bind() => new CleanupStreamResult()
    {
      Complete = this.CompleteColumn.GetBoolean((IDataReader) this.Reader),
      DeletedRows = this.RowsColumn.GetInt32((IDataReader) this.Reader),
      DeletedStageBatches = this.StageBatchesColumn.GetInt32((IDataReader) this.Reader),
      DeletedStreams = this.StreamsColumn.GetInt32((IDataReader) this.Reader),
      DeletedProcessBatches = this.ProcessBatchesColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
