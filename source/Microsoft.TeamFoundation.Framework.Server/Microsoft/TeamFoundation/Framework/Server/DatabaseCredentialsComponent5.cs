// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseCredentialsComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseCredentialsComponent5 : DatabaseCredentialsComponent2
  {
    public override void UpdateDatabaseCredential(
      TeamFoundationDatabaseCredential credential,
      bool markDemotedCredForDeletion)
    {
      throw new NotImplementedException();
    }

    public void UpdateDatabaseCredential(TeamFoundationDatabaseCredential credential)
    {
      if (!credential.IsUpdateRequired())
        return;
      if (credential.IsPrimaryCredential && !credential.IsIsPrimaryCredentialDirty)
      {
        int num = credential.IsCredentialStatusDirty ? 1 : 0;
      }
      if (credential.DatabaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_UpdateDatabaseCredential");
      this.BindInt("@id", credential.Id);
      this.BindBinary("@version", credential.Version, SqlDbType.Timestamp);
      this.BindString("@name", credential.Name, 64, false, SqlDbType.NVarChar);
      if (credential.IsDatabaseIdDirty)
        this.BindInt("@databaseId", credential.DatabaseId);
      if (credential.IsIsPrimaryCredentialDirty)
        this.BindBoolean("@isPrimaryCredential", credential.IsPrimaryCredential);
      if (credential.IsCredentialStatusDirty)
        this.BindByte("@credentialStatus", (byte) credential.CredentialStatus);
      if (credential.IsUserIdDirty)
        this.BindString("@userId", credential.UserId, 128, false, SqlDbType.NVarChar);
      if (credential.IsPasswordEncryptedDirty)
      {
        if (this.Version >= 3)
          this.BindBinary("@passwordEncrypted", credential.PasswordEncrypted, 1024, SqlDbType.VarBinary);
        else
          this.BindBinary("@passwordEncrypted", credential.PasswordEncrypted, 256, SqlDbType.Binary);
      }
      if (credential.IsSigningKeyIdDirty && this.Version >= 3)
        this.BindGuid("@signingKeyId", credential.SigningKeyId);
      if (credential.IsDescriptionDirty)
        this.BindString("@description", credential.Description, -1, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      credential.Updated();
    }
  }
}
