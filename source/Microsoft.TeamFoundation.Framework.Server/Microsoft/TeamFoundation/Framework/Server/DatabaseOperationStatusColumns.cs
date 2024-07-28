// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseOperationStatusColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseOperationStatusColumns : ObjectBinder<DatabaseOperationStatus>
  {
    private SqlColumnBinder m_databaseStateColumn = new SqlColumnBinder("state_desc");
    private SqlColumnBinder m_lastModifyTimeColumn = new SqlColumnBinder("last_modify_time");
    private SqlColumnBinder m_startTimeColumn = new SqlColumnBinder("start_time");
    private SqlColumnBinder m_errorCodeColumn = new SqlColumnBinder("error_code");
    private SqlColumnBinder m_errorDescColumn = new SqlColumnBinder("error_desc");

    protected override DatabaseOperationStatus Bind() => new DatabaseOperationStatus()
    {
      State = this.m_databaseStateColumn.GetString((IDataReader) this.Reader, false),
      StartTime = this.m_startTimeColumn.GetDateTime((IDataReader) this.Reader),
      LastModifyTime = this.m_lastModifyTimeColumn.GetDateTime((IDataReader) this.Reader),
      ErrorCode = this.m_errorCodeColumn.GetInt32((IDataReader) this.Reader),
      ErrorDesc = this.m_errorDescColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
