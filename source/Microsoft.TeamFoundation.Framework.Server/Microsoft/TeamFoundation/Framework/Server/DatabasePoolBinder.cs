// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePoolBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabasePoolBinder : ObjectBinder<TeamFoundationDatabasePool>
  {
    protected SqlColumnBinder PoolIdColumn = new SqlColumnBinder("PoolId");
    protected SqlColumnBinder DatabaseTypeColumn = new SqlColumnBinder("DatabaseType");
    protected SqlColumnBinder CollationColumn = new SqlColumnBinder("Collation");
    protected SqlColumnBinder PoolNameColumn = new SqlColumnBinder("PoolName");
    protected SqlColumnBinder InitialCapacityColumn = new SqlColumnBinder("InitialCapacity");
    protected SqlColumnBinder CreateThresholdColumn = new SqlColumnBinder("CreateThreshold");
    protected SqlColumnBinder GrowByColumn = new SqlColumnBinder("GrowBy");
    protected SqlColumnBinder ServicingOperationsColumn = new SqlColumnBinder("ServicingOperations");
    protected SqlColumnBinder MaxDatabaseLimitColumn = new SqlColumnBinder("MaxDatabaseLimit");

    internal DatabasePoolBinder()
    {
    }

    internal DatabasePoolBinder(SqlDataReader dataReader, string storedProcedure)
      : base(dataReader, storedProcedure)
    {
    }

    internal void Bind(out TeamFoundationDatabasePool result) => result = this.Bind();

    protected override TeamFoundationDatabasePool Bind()
    {
      TeamFoundationDatabasePool foundationDatabasePool = new TeamFoundationDatabasePool();
      foundationDatabasePool.Initialize(this.PoolIdColumn.GetInt32((IDataReader) this.Reader), this.GetDatabaseType(this.DatabaseTypeColumn.GetInt32((IDataReader) this.Reader)), TeamFoundationDatabaseCredentialPolicy.None, this.CollationColumn.GetString((IDataReader) this.Reader, false), this.GetPoolName(this.PoolNameColumn.GetString((IDataReader) this.Reader, false)), this.InitialCapacityColumn.GetInt32((IDataReader) this.Reader), this.CreateThresholdColumn.GetInt32((IDataReader) this.Reader), this.GrowByColumn.GetInt32((IDataReader) this.Reader), this.ServicingOperationsColumn.GetString((IDataReader) this.Reader, true), this.MaxDatabaseLimitColumn.GetInt32((IDataReader) this.Reader), (string) null, TeamFoundationDatabasePoolFlags.None, DatabaseManagementConstants.DefaultServiceObjective);
      return foundationDatabasePool;
    }

    protected virtual string GetPoolName(string poolName) => DatabaseManagementComponent.TranslateLegacyPoolName(poolName);

    protected virtual TeamFoundationDatabaseType GetDatabaseType(int databaseType) => DatabaseManagementComponent.TranslateLegacyDatabaseType(databaseType);
  }
}
