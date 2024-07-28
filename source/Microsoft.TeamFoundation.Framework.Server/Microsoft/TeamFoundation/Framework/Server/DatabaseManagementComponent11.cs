// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent11
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent11 : DatabaseManagementComponent9
  {
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
      this.BindString("@servicingOperations", pool.ServicingOperations, 8000, true, SqlDbType.VarChar);
      this.BindInt("@maxDatabaseLimit", pool.MaxDatabaseLimit);
      this.BindString("@deltaOperationPrefixes", pool.DeltaOperationPrefixes, 256, true, SqlDbType.VarChar);
      this.BindSize(pool);
      if (this.Version >= 13)
        this.BindInt("@flags", (int) pool.GetFlags());
      if (this.Version >= 14)
      {
        this.BindEdition(pool);
        this.BindString("@serviceObjective", pool.ServiceObjective, 20, true, SqlDbType.VarChar);
      }
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new DatabaseManagementComponent.DatabaseIdColumn().poolId.GetInt32((IDataReader) reader) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
    }

    protected virtual void BindSize(TeamFoundationDatabasePool pool) => this.BindInt("@size", 0);

    protected virtual void BindEdition(TeamFoundationDatabasePool pool) => this.BindNullValue("@edition", SqlDbType.VarChar);
  }
}
