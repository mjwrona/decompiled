// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePropertiesBinder4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabasePropertiesBinder4 : DatabasePropertiesBinder3
  {
    protected SqlColumnBinder UserNameColumn = new SqlColumnBinder("UserId");
    protected SqlColumnBinder UserPasswordColumn = new SqlColumnBinder("PasswordEncrypted");

    internal DatabasePropertiesBinder4()
    {
    }

    internal DatabasePropertiesBinder4(SqlDataReader dataReader, string storedProcedure)
      : base(dataReader, storedProcedure)
    {
    }

    protected override InternalDatabaseProperties Bind()
    {
      InternalDatabaseProperties databaseProperties = new InternalDatabaseProperties();
      int int32_1 = this.DatabaseIdColumn.GetInt32((IDataReader) this.Reader);
      string str1 = this.ConnectionStringColumn.GetString((IDataReader) this.Reader, false);
      string databaseName = this.DatabaseNameColumn.GetString((IDataReader) this.Reader, false);
      string serviceLevel = this.ServiceLevelColumn.GetString((IDataReader) this.Reader, true);
      string poolName = this.GetPoolName(this.PoolNameColumn.GetString((IDataReader) this.Reader, false));
      int int32_2 = this.TenantsColumn.GetInt32((IDataReader) this.Reader);
      int int32_3 = this.MaxTenantsColumn.GetInt32((IDataReader) this.Reader);
      int int32_4 = this.TenantsPendingDeleteColumn.GetInt32((IDataReader) this.Reader);
      TeamFoundationDatabaseStatus status = (TeamFoundationDatabaseStatus) Enum.ToObject(typeof (TeamFoundationDatabaseStatus), this.StatusColumn.GetInt32((IDataReader) this.Reader));
      DateTime dateTime1 = this.StatusChangedDateColumn.GetDateTime((IDataReader) this.Reader);
      string statusReason = this.StatusReasonColumn.GetString((IDataReader) this.Reader, true);
      DateTime dateTime2 = this.LastTenantAddedColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      DateTime dateTime3 = this.AcquisitionOrderColumn.GetDateTime((IDataReader) this.Reader);
      TeamFoundationDatabaseFlags flags = (TeamFoundationDatabaseFlags) Enum.ToObject(typeof (TeamFoundationDatabaseFlags), this.FlagsColumn.GetInt32((IDataReader) this.Reader, 0, 0));
      int int32_5 = this.DatabaseSizeColumn.GetInt32((IDataReader) this.Reader, 0);
      int int32_6 = this.DatabaseCapacityColumn.GetInt32((IDataReader) this.Reader, 0);
      DateTime dateTime4 = this.SizeChangedDateColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      string str2 = this.UserNameColumn.GetString((IDataReader) this.Reader, true);
      byte[] bytes = this.UserPasswordColumn.GetBytes((IDataReader) this.Reader, true);
      SqlConnectionInfoWrapper connectionInfo = new SqlConnectionInfoWrapper()
      {
        ConnectionString = str1,
        UserId = str2,
        PasswordEncrypted = Convert.ToBase64String(bytes)
      };
      databaseProperties.Initialize(int32_1, connectionInfo, databaseName, serviceLevel, poolName, int32_2, int32_3, int32_4, int32_5, int32_6, dateTime4, status, dateTime1, statusReason, dateTime2, dateTime3, flags, (long) DatabaseManagementConstants.InvalidDatabaseVersion);
      return databaseProperties;
    }
  }
}
