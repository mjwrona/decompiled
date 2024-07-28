// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePropertiesBinder2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabasePropertiesBinder2 : DatabasePropertiesBinder
  {
    protected SqlColumnBinder TenantsPendingDeleteColumn = new SqlColumnBinder("TenantsPendingDelete");
    protected SqlColumnBinder FlagsColumn = new SqlColumnBinder("Flags");

    internal DatabasePropertiesBinder2()
    {
    }

    internal DatabasePropertiesBinder2(SqlDataReader dataReader, string storedProcedure)
      : base(dataReader, storedProcedure)
    {
    }

    protected override InternalDatabaseProperties Bind()
    {
      InternalDatabaseProperties databaseProperties = new InternalDatabaseProperties();
      databaseProperties.Initialize(this.DatabaseIdColumn.GetInt32((IDataReader) this.Reader), this.ConnectionStringColumn.GetString((IDataReader) this.Reader, false), this.DatabaseNameColumn.GetString((IDataReader) this.Reader, false), this.ServiceLevelColumn.GetString((IDataReader) this.Reader, true), this.GetPoolName(this.PoolNameColumn.GetString((IDataReader) this.Reader, false)), this.TenantsColumn.GetInt32((IDataReader) this.Reader), this.MaxTenantsColumn.GetInt32((IDataReader) this.Reader), this.TenantsPendingDeleteColumn.GetInt32((IDataReader) this.Reader), (TeamFoundationDatabaseStatus) Enum.ToObject(typeof (TeamFoundationDatabaseStatus), this.StatusColumn.GetInt32((IDataReader) this.Reader)), this.StatusChangedDateColumn.GetDateTime((IDataReader) this.Reader), this.StatusReasonColumn.GetString((IDataReader) this.Reader, true), this.LastTenantAddedColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue), this.AcquisitionOrderColumn.GetDateTime((IDataReader) this.Reader), (TeamFoundationDatabaseFlags) Enum.ToObject(typeof (TeamFoundationDatabaseFlags), this.FlagsColumn.GetInt32((IDataReader) this.Reader, 0, 0)));
      return databaseProperties;
    }
  }
}
