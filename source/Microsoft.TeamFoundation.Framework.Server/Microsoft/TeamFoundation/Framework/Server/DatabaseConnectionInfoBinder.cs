// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseConnectionInfoBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseConnectionInfoBinder : ObjectBinder<DatabaseConnectionInfo>
  {
    private SqlColumnBinder m_hostNameColumn = new SqlColumnBinder("HostName");
    private SqlColumnBinder m_programNameColumn = new SqlColumnBinder("ProgramName");
    private SqlColumnBinder m_hostProcessIdColumn = new SqlColumnBinder("HostProcessId");
    private SqlColumnBinder m_countColumn = new SqlColumnBinder("Count");
    private SqlColumnBinder m_inactiveCountColumn = new SqlColumnBinder("InactiveCount");
    private SqlColumnBinder m_sampleTextColumn = new SqlColumnBinder("SampleText");

    protected override DatabaseConnectionInfo Bind() => new DatabaseConnectionInfo(this.m_hostNameColumn.GetString((IDataReader) this.Reader, false), this.m_programNameColumn.GetString((IDataReader) this.Reader, false), this.m_hostProcessIdColumn.GetInt32((IDataReader) this.Reader), this.m_countColumn.GetInt32((IDataReader) this.Reader), this.m_inactiveCountColumn.GetInt32((IDataReader) this.Reader), this.m_sampleTextColumn.GetString((IDataReader) this.Reader, true));
  }
}
