// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePropertiesBinder9
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabasePropertiesBinder9 : DatabasePropertiesBinder8
  {
    protected SqlColumnBinder MinServiceObjectiveColumn = new SqlColumnBinder("MinServiceObjective");
    protected SqlColumnBinder MaxServiceObjectiveColumn = new SqlColumnBinder("MaxServiceObjective");

    internal DatabasePropertiesBinder9()
    {
    }

    internal DatabasePropertiesBinder9(SqlDataReader dataReader, string storedProcedure)
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
      string minServiceObjective = this.MinServiceObjectiveColumn.GetString((IDataReader) this.Reader, true);
      string maxServiceObjective = this.MaxServiceObjectiveColumn.GetString((IDataReader) this.Reader, true);
      string str2 = this.UserNameColumn.GetString((IDataReader) this.Reader, true);
      byte[] bytes1 = this.UserPasswordColumn.GetBytes((IDataReader) this.Reader, true);
      Guid guid1 = this.SigningKeyIdColumn.GetGuid((IDataReader) this.Reader, true);
      string str3 = this.DboNameColumn.GetString((IDataReader) this.Reader, (string) null);
      byte[] bytes2 = this.DboPasswordColumn.GetBytes((IDataReader) this.Reader, Array.Empty<byte>());
      Guid guid2 = this.DboSigningKeyIdColumn.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      long int64 = this.VersionColumn.GetInt64((IDataReader) this.Reader);
      int int32_7 = this.RequestTimeoutColumn.GetInt32((IDataReader) this.Reader, InternalDatabaseProperties.DefaultDatabaseRequestTimeout);
      int int32_8 = this.DeadlockRetriesColumn.GetInt32((IDataReader) this.Reader, 20);
      int int32_9 = this.DeadlockPauseColumn.GetInt32((IDataReader) this.Reader, 200);
      TeamFoundationDatabaseLoggingOptions loggingOptions = (TeamFoundationDatabaseLoggingOptions) this.LoggingOptionsColumn.GetByte((IDataReader) this.Reader, (byte) DatabaseManagementConstants.DefaultDatabaseLoggingOptions);
      TimeSpan executionTimeThreshold = new TimeSpan(0, 0, this.ExecutionTimeThresholdColumn.GetInt32((IDataReader) this.Reader, 30));
      bool boolean1 = this.BreakerDisabledColumn.GetBoolean((IDataReader) this.Reader, false);
      byte breakerErrorThresholdPerc = this.BreakerErrorThresholdPercColumn.GetByte((IDataReader) this.Reader, byte.MaxValue);
      bool boolean2 = this.BreakerForceClosedColumn.GetBoolean((IDataReader) this.Reader, false);
      bool boolean3 = this.BreakerForceOpenColumn.GetBoolean((IDataReader) this.Reader, false);
      int int32_10 = this.BreakerMaxBackoffColumn.GetInt32((IDataReader) this.Reader, -1);
      int int32_11 = this.BreakerRequestVolumeThresholdColumn.GetInt32((IDataReader) this.Reader, -1);
      int int32_12 = this.BreakerExecutionTimeoutColumn.GetInt32((IDataReader) this.Reader, -1);
      int int32_13 = this.BreakerMaxExecConcurrentRequestsColumn.GetInt32((IDataReader) this.Reader, -1);
      int int32_14 = this.BreakerMaxFallbackConcurrentRequestsColumn.GetInt32((IDataReader) this.Reader, -1);
      SqlConnectionInfoWrapper defaultConnectionInfoWrapper = new SqlConnectionInfoWrapper()
      {
        ConnectionString = str1,
        UserId = str2,
        PasswordEncrypted = Convert.ToBase64String(bytes1),
        SigningKeyId = guid1
      };
      SqlConnectionInfoWrapper connectionInfoWrapper;
      if (str3 != null)
        connectionInfoWrapper = new SqlConnectionInfoWrapper()
        {
          ConnectionString = str1,
          UserId = str3,
          PasswordEncrypted = Convert.ToBase64String(bytes2),
          SigningKeyId = guid2
        };
      else
        connectionInfoWrapper = (SqlConnectionInfoWrapper) null;
      SqlConnectionInfoWrapper dboConnectionInfoWrapper = connectionInfoWrapper;
      databaseProperties.Initialize(int32_1, defaultConnectionInfoWrapper, dboConnectionInfoWrapper, databaseName, serviceLevel, poolName, int32_2, int32_3, int32_4, int32_5, int32_6, dateTime4, status, dateTime1, statusReason, dateTime2, dateTime3, flags, int64, int32_7, int32_8, int32_9, loggingOptions, executionTimeThreshold, boolean1, breakerErrorThresholdPerc, boolean2, boolean3, int32_10, int32_11, int32_12, int32_13, int32_14, minServiceObjective, maxServiceObjective);
      return databaseProperties;
    }
  }
}
