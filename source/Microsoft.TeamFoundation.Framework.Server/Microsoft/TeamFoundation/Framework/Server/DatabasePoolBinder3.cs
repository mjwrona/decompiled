// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePoolBinder3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabasePoolBinder3 : DatabasePoolBinder2
  {
    protected SqlColumnBinder DeltaOperationPrefixesColumn = new SqlColumnBinder("DeltaOperationPrefixes");
    protected SqlColumnBinder FlagsColumn = new SqlColumnBinder("Flags");
    protected SqlColumnBinder ServiceObjectiveColumn = new SqlColumnBinder("ServiceObjective");

    internal DatabasePoolBinder3()
    {
    }

    internal DatabasePoolBinder3(SqlDataReader dataReader, string storedProcedure)
      : base(dataReader, storedProcedure)
    {
    }

    protected override TeamFoundationDatabasePool Bind()
    {
      TeamFoundationDatabasePool foundationDatabasePool = new TeamFoundationDatabasePool();
      foundationDatabasePool.Initialize(this.PoolIdColumn.GetInt32((IDataReader) this.Reader), this.GetDatabaseType(this.DatabaseTypeColumn.GetInt32((IDataReader) this.Reader)), (TeamFoundationDatabaseCredentialPolicy) this.CredentialPolicyColumn.GetByte((IDataReader) this.Reader), this.CollationColumn.GetString((IDataReader) this.Reader, false), this.GetPoolName(this.PoolNameColumn.GetString((IDataReader) this.Reader, false)), this.InitialCapacityColumn.GetInt32((IDataReader) this.Reader), this.CreateThresholdColumn.GetInt32((IDataReader) this.Reader), this.GrowByColumn.GetInt32((IDataReader) this.Reader), this.ServicingOperationsColumn.GetString((IDataReader) this.Reader, true), this.MaxDatabaseLimitColumn.GetInt32((IDataReader) this.Reader), this.DeltaOperationPrefixesColumn.GetString((IDataReader) this.Reader, (string) null), (TeamFoundationDatabasePoolFlags) this.FlagsColumn.GetInt32((IDataReader) this.Reader, 0, 0), this.ServiceObjectiveColumn.GetString((IDataReader) this.Reader, DatabaseManagementConstants.DefaultServiceObjective));
      return foundationDatabasePool;
    }

    protected override string GetPoolName(string poolName) => poolName;

    protected override TeamFoundationDatabaseType GetDatabaseType(int databaseType) => (TeamFoundationDatabaseType) databaseType;
  }
}
