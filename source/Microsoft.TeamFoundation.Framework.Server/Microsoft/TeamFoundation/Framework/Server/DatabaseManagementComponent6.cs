// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent6
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent6 : DatabaseManagementComponent5
  {
    public override ITeamFoundationDatabaseProperties RegisterDatabase(
      string connectionString,
      string databaseName,
      string serviceLevel,
      string poolName,
      int tenants,
      int maxTenants,
      TeamFoundationDatabaseStatus status,
      DateTime statusChangedDate,
      string statusReason,
      DateTime? lastTenantAdded,
      bool registerCredential,
      string userId,
      byte[] passwordEncrypted,
      Guid signingKeyId,
      TeamFoundationDatabaseFlags flags,
      string serviceObjective)
    {
      poolName = this.GetLegacyPoolName(poolName);
      this.PrepareStoredProcedure("prc_RegisterDatabase");
      this.BindString("@connectionString", connectionString, 520, false, SqlDbType.NVarChar);
      this.BindString("@serviceLevel", serviceLevel, (int) byte.MaxValue, true, SqlDbType.VarChar);
      this.BindString("@poolName", poolName, 256, false, SqlDbType.VarChar);
      this.BindInt("@tenants", tenants);
      this.BindInt("@maxTenants", maxTenants);
      this.BindInt("@status", (int) status);
      this.BindDateTime("@statusChangedDate", statusChangedDate);
      this.BindString("@statusReason", statusReason, int.MaxValue, true, SqlDbType.VarChar);
      if (lastTenantAdded.HasValue)
      {
        DateTime? nullable = lastTenantAdded;
        DateTime minValue = DateTime.MinValue;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != minValue ? 1 : 0) : 0) : 1) != 0)
        {
          this.BindDateTime("@lastTenantAdded", lastTenantAdded.Value);
          goto label_4;
        }
      }
      this.BindNullValue("@lastTenantAdded", SqlDbType.DateTime);
label_4:
      this.BindBoolean("@registerCredential", registerCredential);
      this.BindString("@userId", userId, 128, true, SqlDbType.NVarChar);
      this.BindBinary("@passwordEncrypted", passwordEncrypted, 256, SqlDbType.Binary);
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        throw new DatabaseNotFoundException();
      InternalDatabaseProperties result;
      this.CreatePropertiesBinder(dataReader, this.ProcedureName).Bind(out result);
      return (ITeamFoundationDatabaseProperties) result;
    }

    public override int CreateDatabasePool(TeamFoundationDatabasePool pool)
    {
      string legacyPoolName = this.GetLegacyPoolName(pool.PoolName);
      int legacyDatabaseType = this.GetLegacyDatabaseType(pool.DatabaseType);
      this.PrepareStoredProcedure("prc_CreateDatabasePool");
      this.BindInt("@databaseType", legacyDatabaseType);
      this.BindByte("@credentialPolicy", (byte) pool.CredentialPolicy);
      this.BindString("@collation", pool.Collation, 256, false, SqlDbType.VarChar);
      this.BindString("@poolName", legacyPoolName, 256, false, SqlDbType.VarChar);
      this.BindInt("@initialCapacity", pool.InitialCapacity);
      this.BindInt("@createThreshold", pool.CreateThreshold);
      this.BindInt("@growBy", pool.GrowBy);
      this.BindInt("@size", 0);
      this.BindString("@servicingOperations", pool.ServicingOperations, 8000, true, SqlDbType.VarChar);
      this.BindInt("@maxDatabaseLimit", pool.MaxDatabaseLimit);
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new DatabaseManagementComponent.DatabaseIdColumn().poolId.GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
    }

    public override void UpdateDatabasePool(TeamFoundationDatabasePool pool)
    {
      if (!pool.IsUpdateRequired())
        return;
      this.PrepareStoredProcedure("prc_UpdateDatabasePool");
      this.BindInt("@poolId", pool.PoolId);
      if (pool.IsPoolNameDirty)
        this.BindString("@poolName", this.GetLegacyPoolName(pool.PoolName), 256, false, SqlDbType.VarChar);
      else
        this.BindNullValue("@poolName", SqlDbType.VarChar);
      if (pool.IsCredentialPolicyDirty)
        this.BindByte("@credentialPolicy", (byte) pool.CredentialPolicy);
      else
        this.BindNullValue("@credentialPolicy", SqlDbType.TinyInt);
      if (pool.IsInitialCapacityDirty)
        this.BindInt("@initialCapacity", pool.InitialCapacity);
      else
        this.BindNullValue("@initialCapacity", SqlDbType.Int);
      if (pool.IsCreateThresholdDirty)
        this.BindInt("@createThreshold", pool.CreateThreshold);
      else
        this.BindNullValue("@createThreshold", SqlDbType.Int);
      if (pool.IsGrowByDirty)
        this.BindInt("@growBy", pool.GrowBy);
      else
        this.BindNullValue("@growBy", SqlDbType.Int);
      if (pool.IsServicingOperationsDirty)
        this.BindString("@servicingOperations", pool.ServicingOperations, 8000, true, SqlDbType.VarChar);
      else
        this.BindNullValue("@servicingOperations", SqlDbType.VarChar);
      if (pool.IsMaxDatabaseLimitDirty)
        this.BindInt("@maxDatabaseLimit", pool.MaxDatabaseLimit);
      else
        this.BindNullValue("@maxDatabaseLimit", SqlDbType.Int);
      if (this.Version >= 11)
      {
        if (pool.IsDeltaOperationPrefixesDirty)
          this.BindString("@deltaOperationPrefixes", pool.DeltaOperationPrefixes, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        else
          this.BindNullValue("@deltaOperationPrefixes", SqlDbType.VarChar);
      }
      if (this.Version >= 13)
      {
        if (pool.AreFlagsDirty())
          this.BindInt("@flags", (int) pool.GetFlags());
        else
          this.BindNullValue("@flags", SqlDbType.Int);
      }
      if (this.Version >= 14)
      {
        if (pool.IsServiceObjectiveDirty)
          this.BindString("@serviceObjective", pool.ServiceObjective, 20, true, SqlDbType.VarChar);
        else
          this.BindNullValue("@serviceObjective", SqlDbType.VarChar);
      }
      this.ExecuteNonQuery();
      pool.Updated();
    }

    public override void SetDatabaseStatus(
      int databaseId,
      TeamFoundationDatabaseStatus status,
      string statusReason)
    {
      throw new NotSupportedException();
    }

    protected override DatabasePoolBinder CreatePoolBinder() => (DatabasePoolBinder) new DatabasePoolBinder2();

    protected override DatabasePoolBinder CreatePoolBinder(
      SqlDataReader dataReader,
      string storedProcedure)
    {
      return (DatabasePoolBinder) new DatabasePoolBinder2(dataReader, storedProcedure);
    }
  }
}
