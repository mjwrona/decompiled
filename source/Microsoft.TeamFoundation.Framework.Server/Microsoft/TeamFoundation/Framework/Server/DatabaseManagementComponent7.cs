// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent7
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent7 : DatabaseManagementComponent6
  {
    public override ITeamFoundationDatabaseProperties UpdateDatabase(
      TeamFoundationDatabaseProperties properties)
    {
      if (!properties.IsUpdateRequired())
        return properties.GetCachedProperties();
      if (properties.DatabaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_UpdateDatabase");
      this.BindInt("@databaseId", properties.DatabaseId);
      if (properties.IsConnectionInfoDirty)
        this.BindString("@connectionString", properties.ConnectionInfoWrapper.ConnectionString, 520, false, SqlDbType.NVarChar);
      if (properties.IsServiceLevelDirty)
        this.BindString("@serviceLevel", properties.ServiceLevel, (int) byte.MaxValue, true, SqlDbType.VarChar);
      if (properties.IsPoolNameDirty)
        this.BindString("@poolName", this.GetLegacyPoolName(properties.PoolName), 256, false, SqlDbType.VarChar);
      if (properties.IsTenantsDirty)
        this.BindInt("@tenants", properties.Tenants);
      if (properties.IsMaxTenantsDirty)
        this.BindInt("@maxTenants", properties.MaxTenants);
      if (properties.IsFlagsDirty)
        this.BindInt("@flags", (int) properties.Flags);
      if (properties.IsTenantsPendingDeleteDirty)
        this.BindInt("@tenantsPendingDelete", properties.TenantsPendingDelete);
      if (properties.IsAcquisitionOrderDirty)
        this.BindDateTime("@acquisitionOrder", properties.AcquisitionOrder);
      if (properties.IsDatabaseSizeDirty)
        this.BindInt("@databaseSize", properties.DatabaseSize);
      if (properties.IsDatabaseCapacityDirty)
        this.BindInt("@databaseCapacity", properties.DatabaseCapacity);
      this.ExecuteNonQuery();
      properties.Updated();
      return (ITeamFoundationDatabaseProperties) properties;
    }

    protected override DatabasePropertiesBinder CreatePropertiesBinder() => (DatabasePropertiesBinder) new DatabasePropertiesBinder4();

    protected override DatabasePropertiesBinder CreatePropertiesBinder(
      SqlDataReader dataReader,
      string storedProcedure)
    {
      return (DatabasePropertiesBinder) new DatabasePropertiesBinder4(dataReader, storedProcedure);
    }
  }
}
