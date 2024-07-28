// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseLogFileUsageColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseLogFileUsageColumns : ObjectBinder<DatabaseLogFileUsage>
  {
    private SqlColumnBinder m_sizeColumn = new SqlColumnBinder("LogFileSizeMB");
    private SqlColumnBinder m_usedColumn = new SqlColumnBinder("LogFileUsedMB");
    private SqlColumnBinder m_availableColumn = new SqlColumnBinder("LogFileAvailableMB");

    protected override DatabaseLogFileUsage Bind() => new DatabaseLogFileUsage()
    {
      LogFileSizeMB = this.m_sizeColumn.GetInt64((IDataReader) this.Reader),
      LogFileUsedMB = this.m_usedColumn.GetInt64((IDataReader) this.Reader),
      LogFileAvailableMB = this.m_availableColumn.GetInt64((IDataReader) this.Reader)
    };
  }
}
