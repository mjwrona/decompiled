// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlConnectionInfoWrapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract]
  public sealed class SqlConnectionInfoWrapper
  {
    public SqlConnectionInfoWrapper()
    {
    }

    internal SqlConnectionInfoWrapper(IVssRequestContext requestContext, ISqlConnectionInfo info)
    {
      this.ConnectionString = info.ConnectionString;
      if (!(info is ISupportSqlCredential supportSqlCredential))
        return;
      this.UserId = supportSqlCredential.UserId;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationSigningService service = vssRequestContext.GetService<ITeamFoundationSigningService>();
      this.SigningKeyId = service.GetDatabaseSigningKey(requestContext);
      byte[] byteArray = supportSqlCredential.Password.ToByteArray();
      byte[] inArray = service.Encrypt(vssRequestContext, this.SigningKeyId, byteArray, SigningAlgorithm.SHA256);
      Array.Clear((Array) byteArray, 0, byteArray.Length);
      this.PasswordEncrypted = Convert.ToBase64String(inArray);
    }

    [DataMember]
    public string ConnectionString { get; set; }

    [DataMember]
    public string UserId { get; set; }

    [DataMember]
    public string PasswordEncrypted { get; set; }

    [DataMember]
    public Guid SigningKeyId { get; set; }

    public bool IsValidSecurityConfiguration
    {
      get
      {
        bool securityConfiguration = false;
        if (!string.IsNullOrEmpty(this.ConnectionString))
        {
          SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(this.ConnectionString);
          if (connectionStringBuilder.IntegratedSecurity)
          {
            securityConfiguration = string.IsNullOrEmpty(this.UserId) && string.IsNullOrEmpty(this.PasswordEncrypted);
          }
          else
          {
            int num1 = string.IsNullOrEmpty(this.UserId) ? 0 : (this.PasswordEncrypted != null ? 1 : 0);
            bool flag1 = !string.IsNullOrEmpty(connectionStringBuilder.UserID) && !string.IsNullOrEmpty(connectionStringBuilder.Password);
            bool flag2 = connectionStringBuilder.Encrypt && string.IsNullOrWhiteSpace(this.UserId) && string.IsNullOrWhiteSpace(this.PasswordEncrypted) && string.IsNullOrWhiteSpace(connectionStringBuilder.UserID) && string.IsNullOrWhiteSpace(connectionStringBuilder.Password);
            int num2 = flag1 ? 1 : 0;
            securityConfiguration = (num1 | num2 | (flag2 ? 1 : 0)) != 0;
          }
        }
        return securityConfiguration;
      }
    }

    public ISqlConnectionInfo ToSqlConnectionInfo(IVssRequestContext requestContext)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(this.ConnectionString);
      if (!connectionStringBuilder.IntegratedSecurity && !string.IsNullOrEmpty(connectionStringBuilder.UserID))
        return SqlConnectionInfoFactory.Create(this.ConnectionString);
      ISqlConnectionInfo connectionInfo;
      if (!string.IsNullOrEmpty(this.UserId) && SqlConnectionInfoFactory.TryCreate(this.ConnectionString, this.UserId, out connectionInfo))
        return connectionInfo;
      SecureString password = (SecureString) null;
      if (!string.IsNullOrEmpty(this.PasswordEncrypted))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        byte[] bytes = vssRequestContext.GetService<ITeamFoundationSigningService>().Decrypt(vssRequestContext, this.SigningKeyId, Convert.FromBase64String(this.PasswordEncrypted), SigningAlgorithm.SHA256);
        password = bytes.ToSecureString();
        Array.Clear((Array) bytes, 0, bytes.Length);
      }
      return SqlConnectionInfoFactory.Create(this.ConnectionString, this.UserId, password);
    }

    internal ISqlConnectionInfo ToSqlConnectionInfoRaw(ISqlConnectionInfo connectionInfo)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(this.ConnectionString);
      if (!connectionStringBuilder.IntegratedSecurity && !string.IsNullOrEmpty(connectionStringBuilder.UserID))
        return SqlConnectionInfoFactory.Create(this.ConnectionString);
      SecureString password = (SecureString) null;
      ISqlConnectionInfo connectionInfo1;
      if (!string.IsNullOrEmpty(this.UserId) && SqlConnectionInfoFactory.TryCreate(this.ConnectionString, this.UserId, out connectionInfo1))
        return connectionInfo1;
      if (!string.IsNullOrEmpty(this.PasswordEncrypted))
        password = TeamFoundationSigningService.DecryptRaw(connectionInfo, this.SigningKeyId, Convert.FromBase64String(this.PasswordEncrypted), SigningAlgorithm.SHA256).ToSecureString();
      return SqlConnectionInfoFactory.Create(this.ConnectionString, this.UserId, password);
    }

    public override string ToString()
    {
      string connectionString = this.ConnectionString;
      if (string.IsNullOrEmpty(connectionString))
        return string.Empty;
      if (connectionString.IndexOf("Password", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        connectionStringBuilder.Remove("Password");
        connectionString = connectionStringBuilder.ToString();
      }
      return connectionString;
    }
  }
}
