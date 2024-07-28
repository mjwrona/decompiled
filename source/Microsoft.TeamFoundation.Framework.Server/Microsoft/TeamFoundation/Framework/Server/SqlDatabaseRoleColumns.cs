// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlDatabaseRoleColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class SqlDatabaseRoleColumns : ObjectBinder<SqlDatabaseRole>
  {
    private SqlColumnBinder m_nameColumn = new SqlColumnBinder("name");
    private SqlColumnBinder m_principalIdColumn = new SqlColumnBinder("principal_id");
    private SqlColumnBinder m_utcCreateDateColumn = new SqlColumnBinder("utcCreateDate");
    private SqlColumnBinder m_utcModifyDateColumn = new SqlColumnBinder("utcModifyDate");
    private SqlColumnBinder m_isFixedRoleColumn = new SqlColumnBinder("is_fixed_role");
    private SqlColumnBinder m_sidColumn = new SqlColumnBinder("sid");
    private SqlColumnBinder m_ownerColumn = new SqlColumnBinder("owner");

    protected override SqlDatabaseRole Bind()
    {
      SqlDataReader reader = this.Reader;
      return new SqlDatabaseRole()
      {
        Name = this.m_nameColumn.GetString((IDataReader) reader, false),
        Id = this.m_principalIdColumn.GetInt32((IDataReader) reader),
        CreateDate = this.m_utcCreateDateColumn.GetDateTime((IDataReader) reader),
        ModifyDate = this.m_utcModifyDateColumn.GetDateTime((IDataReader) reader),
        IsFixedRole = this.m_isFixedRoleColumn.GetBoolean((IDataReader) reader),
        Sid = new SecurityIdentifier(this.m_sidColumn.GetBytes((IDataReader) reader, false), 0),
        Owner = this.m_ownerColumn.GetString((IDataReader) reader, false)
      };
    }
  }
}
