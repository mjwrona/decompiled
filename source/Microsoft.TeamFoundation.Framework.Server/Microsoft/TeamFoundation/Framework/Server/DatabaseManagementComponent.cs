// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[18]
    {
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent>(1),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent2>(2),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent3>(3),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent4>(4),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent5>(5),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent6>(6),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent7>(7),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent8>(8),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent9>(9),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent9>(10),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent11>(11),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent12>(12),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent12>(13),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent12>(14),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent15>(15),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent16>(16),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent17>(17),
      (IComponentCreator) new ComponentCreator<DatabaseManagementComponent18>(18)
    }, "DatabaseManagement");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800074,
        new SqlExceptionFactory(typeof (DatabaseNotFoundException))
      },
      {
        800075,
        new SqlExceptionFactory(typeof (DatabasePoolNotFoundException))
      },
      {
        800076,
        new SqlExceptionFactory(typeof (DatabasePoolAlreadyExistsException))
      },
      {
        800077,
        new SqlExceptionFactory(typeof (DatabasePoolCannotBeDeletedException))
      },
      {
        800078,
        new SqlExceptionFactory(typeof (DatabasePoolFullException))
      },
      {
        800083,
        new SqlExceptionFactory(typeof (DatabaseAlreadyRegisteredException))
      },
      {
        800102,
        new SqlExceptionFactory(typeof (DatabasePropertiesStaleException))
      },
      {
        800082,
        new SqlExceptionFactory(typeof (SourceDatabaseIdMismatchException))
      }
    };

    public virtual ITeamFoundationDatabaseProperties AcquireDatabasePartition(
      string poolName,
      AcquirePartitionOptions acquireOptions)
    {
      poolName = this.GetLegacyPoolName(poolName);
      this.PrepareStoredProcedure("prc_AcquireDatabasePartition");
      this.BindString("@poolName", poolName, 256, false, SqlDbType.VarChar);
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        return (ITeamFoundationDatabaseProperties) null;
      InternalDatabaseProperties result;
      this.CreatePropertiesBinder(dataReader, this.ProcedureName).Bind(out result);
      return (ITeamFoundationDatabaseProperties) result;
    }

    public virtual ITeamFoundationDatabaseProperties AcquireDatabasePartition(
      int databaseIdToAcquire,
      AcquirePartitionOptions acquireOptions)
    {
      throw new NotImplementedException();
    }

    public virtual ResultCollection GetDatabasePoolsToGrow()
    {
      this.PrepareStoredProcedure("prc_GetDatabasePoolsToGrow");
      ResultCollection databasePoolsToGrow = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      databasePoolsToGrow.AddBinder<TeamFoundationDatabasePool>((ObjectBinder<TeamFoundationDatabasePool>) this.CreatePoolBinder());
      return databasePoolsToGrow;
    }

    public int GetNumberOfDatabases(string poolName, TeamFoundationDatabaseStatus? status)
    {
      poolName = this.GetLegacyPoolName(poolName);
      this.PrepareStoredProcedure("prc_GetNumberOfDatabases");
      this.BindString("@poolName", poolName, 256, false, SqlDbType.VarChar);
      if (!status.HasValue)
        this.BindNullValue("@status", SqlDbType.Int);
      else
        this.BindInt("@status", (int) status.Value);
      return (int) this.ExecuteScalar();
    }

    public virtual void ReleaseDatabasePartition(
      int databaseId,
      bool partitionDeleted,
      bool decrementMaxSize)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_ReleaseDatabasePartition");
      this.BindInt("@databaseId", databaseId);
      this.BindBoolean("@partitionDeleted", partitionDeleted);
      this.ExecuteNonQuery();
    }

    public virtual ITeamFoundationDatabaseProperties RegisterDatabase(
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
      throw new NotSupportedException(FrameworkResources.DatabaseManagementComponentVersionMismatch());
    }

    public void RemoveDatabase(int databaseId)
    {
      this.PrepareStoredProcedure("prc_RemoveDatabase");
      this.BindInt("@databaseId", databaseId);
      this.ExecuteNonQuery();
    }

    public virtual TeamFoundationDatabasePool GetDatabasePool(string poolName)
    {
      poolName = this.GetLegacyPoolName(poolName);
      this.PrepareStoredProcedure("prc_GetDatabasePool");
      this.BindString("@poolName", poolName, 256, false, SqlDbType.VarChar);
      return this.CreatePoolBinder(this.ExecuteReader(), this.ProcedureName).Items.FirstOrDefault<TeamFoundationDatabasePool>();
    }

    public InternalDatabaseProperties GetDatabase(int databaseId)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_GetDatabase");
      this.BindInt("@databaseId", databaseId);
      SqlDataReader dataReader = this.ExecuteReader();
      if (!dataReader.Read())
        throw new DatabaseNotFoundException(databaseId);
      InternalDatabaseProperties result;
      this.CreatePropertiesBinder(dataReader, this.ProcedureName).Bind(out result);
      return result;
    }

    public ResultCollection QueryDatabases()
    {
      this.PrepareStoredProcedure("prc_QueryDatabases");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<InternalDatabaseProperties>((ObjectBinder<InternalDatabaseProperties>) this.CreatePropertiesBinder());
      return resultCollection;
    }

    public virtual List<InternalDatabaseProperties> QueryDatabases(string poolName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(poolName, nameof (poolName));
      return this.QueryDatabases().GetCurrent<InternalDatabaseProperties>().Where<InternalDatabaseProperties>((System.Func<InternalDatabaseProperties, bool>) (db => string.Equals(db.PoolName, poolName, StringComparison.OrdinalIgnoreCase))).ToList<InternalDatabaseProperties>();
    }

    public virtual List<InternalDatabaseProperties> QueryDatabases(
      TeamFoundationDatabaseType databaseType)
    {
      HashSet<string> poolNames = new HashSet<string>(this.QueryDatabasePools().GetCurrent<TeamFoundationDatabasePool>().Items.Where<TeamFoundationDatabasePool>((System.Func<TeamFoundationDatabasePool, bool>) (pool => pool.DatabaseType == databaseType)).Select<TeamFoundationDatabasePool, string>((System.Func<TeamFoundationDatabasePool, string>) (pool => pool.PoolName)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return this.QueryDatabases().GetCurrent<InternalDatabaseProperties>().Where<InternalDatabaseProperties>((System.Func<InternalDatabaseProperties, bool>) (db => poolNames.Contains(db.PoolName))).ToList<InternalDatabaseProperties>();
    }

    public virtual ResultCollection QueryDatabasePools()
    {
      this.PrepareStoredProcedure("prc_QueryDatabasePools");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationDatabasePool>((ObjectBinder<TeamFoundationDatabasePool>) this.CreatePoolBinder());
      return resultCollection;
    }

    public virtual void SetDatabaseStatus(
      int databaseId,
      TeamFoundationDatabaseStatus status,
      string statusReason)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_SetDatabaseStatus");
      this.BindInt("@databaseId", databaseId);
      this.BindInt("@status", (int) status);
      this.BindString("@statusReason", statusReason, int.MaxValue, true, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public virtual ITeamFoundationDatabaseProperties UpdateDatabase(
      TeamFoundationDatabaseProperties properties)
    {
      if (!properties.IsUpdateRequired())
        return properties.GetCachedProperties();
      if (properties.DatabaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_UpdateDatabase");
      this.BindInt("@databaseId", properties.DatabaseId);
      if (properties.IsConnectionInfoDirty)
      {
        if (!string.IsNullOrEmpty(properties.ConnectionInfoWrapper.UserId) || !string.IsNullOrEmpty(properties.ConnectionInfoWrapper.PasswordEncrypted))
          throw new NotSupportedException(FrameworkResources.DatabaseManagementComponentVersionMismatch());
        this.BindString("@connectionString", properties.ConnectionInfoWrapper.ConnectionString, 520, false, SqlDbType.NVarChar);
      }
      else
        this.BindNullValue("@connectionString", SqlDbType.NVarChar);
      if (properties.IsServiceLevelDirty)
        this.BindString("@serviceLevel", properties.ServiceLevel, (int) byte.MaxValue, true, SqlDbType.VarChar);
      else
        this.BindNullValue("@serviceLevel", SqlDbType.VarChar);
      if (properties.IsPoolNameDirty)
        this.BindString("@poolName", this.GetLegacyPoolName(properties.PoolName), 256, false, SqlDbType.VarChar);
      else
        this.BindNullValue("@poolName", SqlDbType.Int);
      if (properties.IsTenantsDirty)
        this.BindInt("@tenants", properties.Tenants);
      else
        this.BindNullValue("@tenants", SqlDbType.Int);
      if (properties.IsMaxTenantsDirty)
        this.BindInt("@maxTenants", properties.MaxTenants);
      else
        this.BindNullValue("@maxTenants", SqlDbType.Int);
      this.ExecuteNonQuery();
      properties.Updated();
      return (ITeamFoundationDatabaseProperties) properties;
    }

    public virtual void UpdateDatabasePool(TeamFoundationDatabasePool pool)
    {
      if (!pool.IsUpdateRequired())
        return;
      this.PrepareStoredProcedure("prc_UpdateDatabasePool");
      this.BindInt("@poolId", pool.PoolId);
      if (pool.IsPoolNameDirty)
        this.BindString("@poolName", this.GetLegacyPoolName(pool.PoolName), 256, false, SqlDbType.VarChar);
      else
        this.BindNullValue("@poolName", SqlDbType.VarChar);
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
      this.ExecuteNonQuery();
      pool.Updated();
    }

    public virtual int CreateDatabasePool(TeamFoundationDatabasePool pool)
    {
      string legacyPoolName = this.GetLegacyPoolName(pool.PoolName);
      int legacyDatabaseType = this.GetLegacyDatabaseType(pool.DatabaseType);
      this.PrepareStoredProcedure("prc_CreateDatabasePool");
      this.BindInt("@databaseType", legacyDatabaseType);
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

    public void DeleteDatabasePool(string poolName)
    {
      poolName = this.GetLegacyPoolName(poolName);
      this.PrepareStoredProcedure("prc_DeleteDatabasePool");
      this.BindString("@poolName", poolName, 256, false, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public void UpdateHostsDatabaseIds(
      IEnumerable<Guid> collectionHostIds,
      int currentId,
      int newId)
    {
      this.PrepareStoredProcedure("prc_UpdateHostsDatabaseId");
      this.BindGuidTable("@hostIds", collectionHostIds);
      this.BindInt("@srcDbId", currentId);
      this.BindInt("@destDbId", newId);
      this.ExecuteNonQuery();
    }

    public virtual void IncrementTenantsPendingDelete(int databaseId, int tenantCount) => throw new NotSupportedException();

    public virtual void FlushDatabaseCache() => throw new NotSupportedException();

    public virtual List<DatabaseManagementViewResult> QueryWhatsRunning(
      ISqlConnectionInfo connectionInfo)
    {
      throw new NotSupportedException();
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) DatabaseManagementComponent.s_sqlExceptionFactories;

    protected virtual DatabasePropertiesBinder CreatePropertiesBinder() => new DatabasePropertiesBinder();

    protected virtual DatabasePropertiesBinder CreatePropertiesBinder(
      SqlDataReader dataReader,
      string storedProcedure)
    {
      return new DatabasePropertiesBinder(dataReader, storedProcedure);
    }

    protected virtual DatabasePoolBinder CreatePoolBinder() => new DatabasePoolBinder();

    protected virtual DatabasePoolBinder CreatePoolBinder(
      SqlDataReader dataReader,
      string storedProcedure)
    {
      return new DatabasePoolBinder(dataReader, storedProcedure);
    }

    protected virtual string GetLegacyPoolName(string poolName)
    {
      if (poolName.Equals(DatabaseManagementConstants.ConfigurationPoolName, StringComparison.Ordinal))
        return "DEPLOYMENT";
      if (!poolName.Equals(DatabaseManagementConstants.DefaultPartitionPoolName, StringComparison.Ordinal))
        return poolName;
      if (this.ConnectionInfo.InitialCatalog.StartsWith("Tfs_", StringComparison.OrdinalIgnoreCase))
        return "DefaultCollectionPool";
      if (this.ConnectionInfo.InitialCatalog.StartsWith("Sps_", StringComparison.OrdinalIgnoreCase))
        return "DefaultAccountPool";
      if (this.ConnectionInfo.InitialCatalog.StartsWith("ServiceHooks_", StringComparison.OrdinalIgnoreCase))
        return "DefaultApplicationPool";
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not map to legacy pool name! PoolName: {0}", (object) poolName));
    }

    internal static string TranslateLegacyPoolName(string poolName)
    {
      if (poolName.Equals("DEPLOYMENT", StringComparison.Ordinal))
        return DatabaseManagementConstants.ConfigurationPoolName;
      return poolName.Equals("DefaultCollectionPool", StringComparison.Ordinal) || poolName.Equals("DefaultAccountPool", StringComparison.Ordinal) || poolName.Equals("DefaultApplicationPool", StringComparison.Ordinal) ? DatabaseManagementConstants.DefaultPartitionPoolName : poolName;
    }

    protected virtual int GetLegacyDatabaseType(TeamFoundationDatabaseType databaseType)
    {
      if (databaseType != TeamFoundationDatabaseType.Partition)
        return (int) databaseType;
      return this.ConnectionInfo.InitialCatalog.StartsWith("Tfs_", StringComparison.OrdinalIgnoreCase) ? 1 : 2;
    }

    internal static TeamFoundationDatabaseType TranslateLegacyDatabaseType(int databaseType)
    {
      switch (databaseType)
      {
        case 1:
        case 2:
          return TeamFoundationDatabaseType.Partition;
        case 3:
          return TeamFoundationDatabaseType.Configuration;
        default:
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The database type {0} was unexpected and could not be translated", (object) databaseType));
      }
    }

    public static bool TryCreateComponent(
      ISqlConnectionInfo connectionInfo,
      out DatabaseManagementComponent component)
    {
      return TeamFoundationResourceManagementService.TryCreateComponentRaw<DatabaseManagementComponent>(connectionInfo, 3600, 0, 10, out component, true);
    }

    protected static string GetFullConnectionStringForBackCompat(ISqlConnectionInfo connectionInfo) => connectionInfo.GetFullConnectionStringInsecure();

    protected class DatabaseIdColumn
    {
      public SqlColumnBinder poolId = new SqlColumnBinder(nameof (poolId));
      public SqlColumnBinder databaseId = new SqlColumnBinder(nameof (databaseId));
    }
  }
}
