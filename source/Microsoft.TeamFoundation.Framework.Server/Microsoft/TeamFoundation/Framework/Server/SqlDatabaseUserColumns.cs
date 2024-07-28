// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlDatabaseUserColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class SqlDatabaseUserColumns : ObjectBinder<SqlDatabaseUser>
  {
    private SqlColumnBinder m_nameColumn = new SqlColumnBinder("name");
    private SqlColumnBinder m_userIdColumn = new SqlColumnBinder("uid");
    private SqlColumnBinder m_sidColumn = new SqlColumnBinder("sid");
    private SqlColumnBinder m_createDateColumn = new SqlColumnBinder("createdate");
    private SqlColumnBinder m_updateDateColumn = new SqlColumnBinder("updatedate");
    private SqlColumnBinder m_hasDbAccessColumn = new SqlColumnBinder("hasdbaccess");
    private SqlColumnBinder m_isNTUserColumn = new SqlColumnBinder("isntuser");
    private SqlColumnBinder m_isNTGroupColumn = new SqlColumnBinder("isntgroup");
    private SqlColumnBinder m_isSqlUserColumn = new SqlColumnBinder("issqluser");

    protected override SqlDatabaseUser Bind()
    {
      SqlDataReader reader = this.Reader;
      return new SqlDatabaseUser()
      {
        Name = this.m_nameColumn.GetString((IDataReader) reader, false),
        UserId = (int) this.m_userIdColumn.GetInt16((IDataReader) reader),
        Sid = this.m_sidColumn.GetBytes((IDataReader) reader, true),
        CreateDate = this.m_createDateColumn.GetDateTime((IDataReader) reader),
        UpdateDate = this.m_updateDateColumn.GetDateTime((IDataReader) reader),
        HasDbAccess = this.m_hasDbAccessColumn.GetBoolean((IDataReader) reader),
        IsNTUser = this.m_isNTUserColumn.GetBoolean((IDataReader) reader),
        IsNTGroup = this.m_isNTGroupColumn.GetBoolean((IDataReader) reader),
        IsSqlUser = this.m_isSqlUserColumn.GetBoolean((IDataReader) reader)
      };
    }
  }
}
