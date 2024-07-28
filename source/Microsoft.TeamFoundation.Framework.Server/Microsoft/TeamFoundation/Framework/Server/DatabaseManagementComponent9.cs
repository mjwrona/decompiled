// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent9
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent9 : DatabaseManagementComponent8
  {
    public override ITeamFoundationDatabaseProperties AcquireDatabasePartition(
      string poolName,
      AcquirePartitionOptions acquireOptions)
    {
      poolName = this.GetLegacyPoolName(poolName);
      this.PrepareStoredProcedure("prc_AcquireDatabasePartition");
      this.BindString("@poolName", poolName, 256, false, SqlDbType.VarChar);
      this.BindBoolean("@canAcquireServicing", acquireOptions.HasFlag((Enum) AcquirePartitionOptions.IgnoreServiceLevel));
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        return (ITeamFoundationDatabaseProperties) null;
      InternalDatabaseProperties result;
      this.CreatePropertiesBinder(dataReader, this.ProcedureName).Bind(out result);
      return (ITeamFoundationDatabaseProperties) result;
    }

    public override ITeamFoundationDatabaseProperties AcquireDatabasePartition(
      int databaseIdToAcquire,
      AcquirePartitionOptions acquireOptions)
    {
      if (databaseIdToAcquire == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_AcquireDatabasePartition");
      this.BindInt("@databaseIdToAcquire", databaseIdToAcquire);
      this.BindBoolean("@canAcquireServicing", acquireOptions.HasFlag((Enum) AcquirePartitionOptions.IgnoreServiceLevel));
      this.BindIncrementMaxDatabases(acquireOptions);
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        return (ITeamFoundationDatabaseProperties) null;
      InternalDatabaseProperties result;
      this.CreatePropertiesBinder(dataReader, this.ProcedureName).Bind(out result);
      return (ITeamFoundationDatabaseProperties) result;
    }

    protected virtual void BindIncrementMaxDatabases(AcquirePartitionOptions acquireOptions)
    {
    }

    protected override DatabasePropertiesBinder CreatePropertiesBinder() => (DatabasePropertiesBinder) new DatabasePropertiesBinder6();

    protected override DatabasePropertiesBinder CreatePropertiesBinder(
      SqlDataReader dataReader,
      string storedProcedure)
    {
      return (DatabasePropertiesBinder) new DatabasePropertiesBinder6(dataReader, storedProcedure);
    }

    protected override DatabasePoolBinder CreatePoolBinder() => (DatabasePoolBinder) new DatabasePoolBinder3();

    protected override DatabasePoolBinder CreatePoolBinder(
      SqlDataReader dataReader,
      string storedProcedure)
    {
      return (DatabasePoolBinder) new DatabasePoolBinder3(dataReader, storedProcedure);
    }

    protected override string GetLegacyPoolName(string poolName) => poolName;

    protected override int GetLegacyDatabaseType(TeamFoundationDatabaseType databaseType) => (int) databaseType;
  }
}
