// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseCredentialsComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseCredentialsComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[5]
    {
      (IComponentCreator) new ComponentCreator<DatabaseCredentialsComponent>(1),
      (IComponentCreator) new ComponentCreator<DatabaseCredentialsComponent2>(2),
      (IComponentCreator) new ComponentCreator<DatabaseCredentialsComponent2>(3),
      (IComponentCreator) new ComponentCreator<DatabaseCredentialsComponent2>(4),
      (IComponentCreator) new ComponentCreator<DatabaseCredentialsComponent5>(5)
    }, "DatabaseCredentials");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800099,
        new SqlExceptionFactory(typeof (DatabaseCredentialNotFoundException))
      },
      {
        800100,
        new SqlExceptionFactory(typeof (StaleDatabaseCredentialException))
      }
    };

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) DatabaseCredentialsComponent.s_sqlExceptionFactories;

    public virtual TeamFoundationDatabaseCredential RegisterDatabaseCredential(
      int databaseId,
      string userId,
      byte[] passwordEncrypted,
      Guid signingKeyId,
      bool markOtherProvisioningCredsForDeletion,
      string credentialName,
      string description)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_RegisterDatabaseCredential");
      this.BindInt("@databaseId", databaseId);
      this.BindString("@userId", userId, 128, false, SqlDbType.NVarChar);
      this.BindBinary("@passwordEncrypted", passwordEncrypted, 256, SqlDbType.Binary);
      this.BindBoolean("@markOtherProvisioningCredsForDeletion", markOtherProvisioningCredsForDeletion);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationDatabaseCredential>((ObjectBinder<TeamFoundationDatabaseCredential>) this.GetDatabaseCredentialBinder());
        return resultCollection.GetCurrent<TeamFoundationDatabaseCredential>().First<TeamFoundationDatabaseCredential>();
      }
    }

    public List<TeamFoundationDatabaseCredential> QueryDatabaseCredentials()
    {
      this.PrepareStoredProcedure("prc_QueryDatabaseCredentials");
      this.BindNullValue("@databaseId", SqlDbType.Int);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationDatabaseCredential>((ObjectBinder<TeamFoundationDatabaseCredential>) this.GetDatabaseCredentialBinder());
        return resultCollection.GetCurrent<TeamFoundationDatabaseCredential>().Items;
      }
    }

    public List<Guid> QueryDatabaseCredentialSigningKeys()
    {
      if (this.Version < 4)
        return new List<Guid>();
      this.PrepareStoredProcedure("prc_QueryDatabaseCredentialSigningKeys");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder keyIdColumn = new SqlColumnBinder("SigningKeyId");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => keyIdColumn.GetGuid(reader))));
        return resultCollection.GetCurrent<Guid>().Items;
      }
    }

    public List<TeamFoundationDatabaseCredential> QueryDatabaseCredentials(int databaseId)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_QueryDatabaseCredentials");
      this.BindInt("@databaseId", databaseId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationDatabaseCredential>((ObjectBinder<TeamFoundationDatabaseCredential>) this.GetDatabaseCredentialBinder());
        return resultCollection.GetCurrent<TeamFoundationDatabaseCredential>().Items;
      }
    }

    public virtual void UpdateDatabaseCredential(
      TeamFoundationDatabaseCredential credential,
      bool markDemotedCredForDeletion)
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
      if (credential.IsDatabaseIdDirty)
        this.BindInt("@databaseId", credential.DatabaseId);
      if (credential.IsIsPrimaryCredentialDirty)
        this.BindBoolean("@isPrimaryCredential", credential.IsPrimaryCredential);
      if (credential.IsCredentialStatusDirty)
        this.BindByte("@credentialStatus", (byte) credential.CredentialStatus);
      if (credential.IsUserIdDirty)
        this.BindString("@userId", credential.UserId, 128, false, SqlDbType.NVarChar);
      if (credential.IsPasswordEncryptedDirty)
        this.BindBinary("@passwordEncrypted", credential.PasswordEncrypted, 256, SqlDbType.Binary);
      this.BindBoolean("@markDemotedCredForDeletion", markDemotedCredForDeletion);
      this.ExecuteNonQuery();
      credential.Updated();
    }

    public void RemoveDatabaseCredentials(IEnumerable<int> ids)
    {
      this.PrepareStoredProcedure("prc_RemoveDatabaseCredentials");
      this.BindInt32Table("@ids", ids);
      this.ExecuteNonQuery();
    }

    protected virtual DatabaseCredentialBinder GetDatabaseCredentialBinder() => new DatabaseCredentialBinder();
  }
}
