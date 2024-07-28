// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlRowLockInfoBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SqlRowLockInfoBinder : ObjectBinder<SqlRowLockInfo>
  {
    private SqlColumnBinder m_schema_name = new SqlColumnBinder("schema_name");
    private SqlColumnBinder m_table_name = new SqlColumnBinder("table_name");
    private SqlColumnBinder m_index_name = new SqlColumnBinder("index_name");
    private SqlColumnBinder m_hobt_id = new SqlColumnBinder("hobt_id");

    internal SqlRowLockInfoBinder()
    {
    }

    internal SqlRowLockInfoBinder(SqlDataReader reader)
      : base(reader, "prc_QueryRowLockInfo")
    {
    }

    protected override SqlRowLockInfo Bind() => new SqlRowLockInfo()
    {
      SchemaName = this.m_schema_name.GetString((IDataReader) this.Reader, true),
      TableName = this.m_table_name.GetString((IDataReader) this.Reader, true),
      IndexName = this.m_index_name.GetString((IDataReader) this.Reader, true),
      HobtId = this.m_hobt_id.GetInt64((IDataReader) this.Reader)
    };
  }
}
