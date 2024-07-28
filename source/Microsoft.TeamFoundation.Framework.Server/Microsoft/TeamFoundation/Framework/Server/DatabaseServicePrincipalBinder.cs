// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseServicePrincipalBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseServicePrincipalBinder : ObjectBinder<DatabaseServicePrincipal>
  {
    private SqlColumnBinder m_principal_id = new SqlColumnBinder("principal_id");
    private SqlColumnBinder m_principal_name = new SqlColumnBinder("name");
    private SqlColumnBinder m_type_desc = new SqlColumnBinder("type_desc");
    private SqlColumnBinder m_authentication_type_desc = new SqlColumnBinder("authentication_type_desc");
    private SqlColumnBinder m_create_date = new SqlColumnBinder("create_date");
    private SqlColumnBinder m_modify_date = new SqlColumnBinder("modify_date");
    private SqlColumnBinder m_state_desc = new SqlColumnBinder("state_desc");
    private SqlColumnBinder m_permission_name = new SqlColumnBinder("permission_name");

    internal DatabaseServicePrincipalBinder()
    {
    }

    internal DatabaseServicePrincipalBinder(SqlDataReader reader)
      : base(reader, "prc_QueryDatabaseServicePrincipals")
    {
    }

    protected override DatabaseServicePrincipal Bind() => new DatabaseServicePrincipal()
    {
      PrincipalId = this.m_principal_id.GetInt32((IDataReader) this.Reader),
      PrincipalName = this.m_principal_name.GetString((IDataReader) this.Reader, true),
      TypeDesc = this.m_type_desc.GetString((IDataReader) this.Reader, true),
      AuthenticationTypeDesc = this.m_authentication_type_desc.GetString((IDataReader) this.Reader, true),
      CreateDate = this.m_create_date.GetDateTime((IDataReader) this.Reader),
      ModifyDate = this.m_modify_date.GetDateTime((IDataReader) this.Reader),
      StateDesc = this.m_state_desc.GetString((IDataReader) this.Reader, true),
      PermissionName = this.m_permission_name.GetString((IDataReader) this.Reader, true)
    };
  }
}
