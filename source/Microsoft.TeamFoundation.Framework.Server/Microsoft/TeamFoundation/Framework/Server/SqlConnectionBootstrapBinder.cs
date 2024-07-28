// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlConnectionBootstrapBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SqlConnectionBootstrapBinder : ObjectBinder<ISqlConnectionInfo>
  {
    private ISqlConnectionInfo m_connectionInfo;
    private SqlColumnBinder ConnectionStringColumn = new SqlColumnBinder("ConnectionString");
    private SqlColumnBinder UserIdColumn = new SqlColumnBinder("UserId");
    private SqlColumnBinder PasswordColumn = new SqlColumnBinder("PasswordEncrypted");
    private SqlColumnBinder SigningKeyIdColumn = new SqlColumnBinder("SigningKeyId");

    internal SqlConnectionBootstrapBinder(ISqlConnectionInfo connectionInfo) => this.m_connectionInfo = connectionInfo;

    protected override ISqlConnectionInfo Bind()
    {
      string connectionString = this.ConnectionStringColumn.GetString((IDataReader) this.Reader, false);
      string userName = (string) null;
      SecureString password = (SecureString) null;
      ISqlConnectionInfo connectionInfo = (ISqlConnectionInfo) null;
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      if (!connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID))
      {
        userName = this.UserIdColumn.ColumnExists((IDataReader) this.Reader) ? this.UserIdColumn.GetString((IDataReader) this.Reader, true) : (string) null;
        byte[] bytes = this.PasswordColumn.ColumnExists((IDataReader) this.Reader) ? this.PasswordColumn.GetBytes((IDataReader) this.Reader, true) : (byte[]) null;
        Guid frameworkSigningKey = FrameworkServerConstants.FrameworkSigningKey;
        Guid identifier = this.SigningKeyIdColumn.ColumnExists((IDataReader) this.Reader) ? this.SigningKeyIdColumn.GetGuid((IDataReader) this.Reader, true) : frameworkSigningKey;
        if (!string.IsNullOrEmpty(userName) && !SqlConnectionInfoFactory.TryCreate(connectionString, userName, out connectionInfo) && bytes != null && bytes.Length != 0)
          password = TeamFoundationSigningService.DecryptRaw(this.m_connectionInfo, identifier, bytes, SigningAlgorithm.SHA256).ToSecureString();
      }
      if (connectionInfo == null)
        connectionInfo = SqlConnectionInfoFactory.Create(connectionString, userName, password);
      return connectionInfo;
    }
  }
}
