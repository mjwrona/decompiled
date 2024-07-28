// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlTraceFlagStatusColumns
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Configuration
{
  internal class SqlTraceFlagStatusColumns : ObjectBinder<SqlTraceFlagStatus>
  {
    private SqlColumnBinder m_traceFlagColumn = new SqlColumnBinder("TraceFlag");
    private SqlColumnBinder m_statusColumn = new SqlColumnBinder("Status");
    private SqlColumnBinder m_globalColumn = new SqlColumnBinder("Global");
    private SqlColumnBinder m_sessionColumn = new SqlColumnBinder("Session");

    protected override SqlTraceFlagStatus Bind() => new SqlTraceFlagStatus()
    {
      TraceFlag = this.m_traceFlagColumn.GetInt16((IDataReader) this.Reader),
      Status = this.m_statusColumn.GetInt16((IDataReader) this.Reader) == (short) 1,
      Global = this.m_globalColumn.GetInt16((IDataReader) this.Reader) == (short) 1,
      Session = this.m_sessionColumn.GetInt16((IDataReader) this.Reader) == (short) 1
    };
  }
}
