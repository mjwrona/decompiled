// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlLoginColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class SqlLoginColumns : ObjectBinder<SqlLoginInfo>
  {
    private SqlColumnBinder m_loginNameColumn = new SqlColumnBinder("loginName");
    private SqlColumnBinder m_sidColumn = new SqlColumnBinder("sid");
    private SqlColumnBinder m_hasAccessColumn = new SqlColumnBinder("hasAccess");
    private SqlColumnBinder m_isEnabledColumn = new SqlColumnBinder("enabled");
    private SqlColumnBinder m_isSysAdminColumn = new SqlColumnBinder("isSysAdmin");
    private SqlColumnBinder m_isNTGroupColumn = new SqlColumnBinder("isNTGroup");
    private SqlColumnBinder m_isNTUserColumn = new SqlColumnBinder("isNTUser");
    private SqlColumnBinder m_utcCreateDateColumn = new SqlColumnBinder("utcCreateDate");
    private SqlColumnBinder m_utcModifyDateColumn = new SqlColumnBinder("utcModifyDate");

    protected override SqlLoginInfo Bind()
    {
      SqlDataReader reader = this.Reader;
      return new SqlLoginInfo(this.m_loginNameColumn.GetString((IDataReader) reader, false), this.m_sidColumn.GetBytes((IDataReader) reader, false), this.m_hasAccessColumn.GetBoolean((IDataReader) reader), this.m_isEnabledColumn.GetBoolean((IDataReader) reader), this.m_isSysAdminColumn.GetBoolean((IDataReader) reader), this.m_isNTUserColumn.GetBoolean((IDataReader) reader), this.m_isNTGroupColumn.GetBoolean((IDataReader) reader), this.m_utcCreateDateColumn.GetDateTime((IDataReader) reader), this.m_utcModifyDateColumn.GetDateTime((IDataReader) reader));
    }
  }
}
