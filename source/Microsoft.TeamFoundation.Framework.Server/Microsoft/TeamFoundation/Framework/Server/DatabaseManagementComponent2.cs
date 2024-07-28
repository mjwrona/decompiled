// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseManagementComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseManagementComponent2 : DatabaseManagementComponent
  {
    public override void IncrementTenantsPendingDelete(int databaseId, int tenantCount)
    {
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      this.PrepareStoredProcedure("prc_IncrementTenantsPendingDelete");
      this.BindInt("@databaseId", databaseId);
      this.BindInt("@tenantCount", tenantCount);
      this.ExecuteNonQuery();
    }

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
      if (properties.IsFlagsDirty)
        this.BindInt("@flags", (int) properties.Flags);
      this.ExecuteNonQuery();
      properties.Updated();
      return (ITeamFoundationDatabaseProperties) properties;
    }

    protected override DatabasePropertiesBinder CreatePropertiesBinder() => (DatabasePropertiesBinder) new DatabasePropertiesBinder2();

    protected override DatabasePropertiesBinder CreatePropertiesBinder(
      SqlDataReader dataReader,
      string storedProcedure)
    {
      return (DatabasePropertiesBinder) new DatabasePropertiesBinder2(dataReader, storedProcedure);
    }
  }
}
