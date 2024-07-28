// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseCredentialBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseCredentialBinder : ObjectBinder<TeamFoundationDatabaseCredential>
  {
    protected SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
    protected SqlColumnBinder Version = new SqlColumnBinder(nameof (Version));
    protected SqlColumnBinder DatabaseId = new SqlColumnBinder(nameof (DatabaseId));
    protected SqlColumnBinder IsPrimaryCredential = new SqlColumnBinder(nameof (IsPrimaryCredential));
    protected SqlColumnBinder CredentialStatus = new SqlColumnBinder(nameof (CredentialStatus));
    protected SqlColumnBinder UserId = new SqlColumnBinder(nameof (UserId));
    protected SqlColumnBinder PasswordEncrypted = new SqlColumnBinder(nameof (PasswordEncrypted));
    protected SqlColumnBinder InUseStartTime = new SqlColumnBinder(nameof (InUseStartTime));
    protected SqlColumnBinder InUseEndTime = new SqlColumnBinder(nameof (InUseEndTime));

    protected override TeamFoundationDatabaseCredential Bind()
    {
      TeamFoundationDatabaseCredential databaseCredential = new TeamFoundationDatabaseCredential();
      int int32_1 = this.Id.GetInt32((IDataReader) this.Reader);
      byte[] bytes1 = this.Version.GetBytes((IDataReader) this.Reader, false);
      int int32_2 = this.DatabaseId.GetInt32((IDataReader) this.Reader);
      bool boolean = this.IsPrimaryCredential.GetBoolean((IDataReader) this.Reader);
      TeamFoundationDatabaseCredentialStatus credentialStatus = (TeamFoundationDatabaseCredentialStatus) this.CredentialStatus.GetByte((IDataReader) this.Reader);
      string userId = this.UserId.GetString((IDataReader) this.Reader, false);
      byte[] bytes2 = this.PasswordEncrypted.GetBytes((IDataReader) this.Reader, false);
      DateTime? inUseStartTime = this.InUseStartTime.IsNull((IDataReader) this.Reader) ? new DateTime?() : new DateTime?(this.InUseStartTime.GetDateTime((IDataReader) this.Reader));
      DateTime? inUseEndTime = this.InUseEndTime.IsNull((IDataReader) this.Reader) ? new DateTime?() : new DateTime?(this.InUseEndTime.GetDateTime((IDataReader) this.Reader));
      databaseCredential.Initialize(int32_1, bytes1, int32_2, boolean, credentialStatus, userId, bytes2, Guid.Empty, inUseStartTime, inUseEndTime, (string) null, (string) null);
      return databaseCredential;
    }
  }
}
