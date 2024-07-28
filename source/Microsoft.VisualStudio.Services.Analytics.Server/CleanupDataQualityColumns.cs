// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.CleanupDataQualityColumns
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class CleanupDataQualityColumns : ObjectBinder<CleanupDataQualityResult>
  {
    private SqlColumnBinder CompleteColumn = new SqlColumnBinder("Complete");
    private SqlColumnBinder RowsColumn = new SqlColumnBinder("DeletedRows");

    protected override CleanupDataQualityResult Bind() => new CleanupDataQualityResult()
    {
      Complete = this.CompleteColumn.GetBoolean((IDataReader) this.Reader),
      DeletedRows = this.RowsColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
