// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ChangeServerIdComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ChangeServerIdComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800026,
        new SqlExceptionFactory(typeof (HostDoesNotExistException))
      }
    };
    private ServiceLevel m_serviceLevel;
    private static readonly ServiceLevel s_dev15M99ServiceLevel = new ServiceLevel("Dev15.M99");
    private static readonly ServiceLevel s_dev16M125ServiceLevel = new ServiceLevel("Dev16.M125");

    protected override void Initialize(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      int serviceVersion,
      ITFLogger logger,
      CircuitBreakerDatabaseProperties circuitBreakerProperties)
    {
      base.Initialize(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, serviceVersion, logger, circuitBreakerProperties);
      this.InfoMessage += new SqlInfoMessageEventHandler(this.OnInfoMessage);
    }

    public override void Dispose()
    {
      this.InfoMessage -= new SqlInfoMessageEventHandler(this.OnInfoMessage);
      base.Dispose();
    }

    protected override ITFLogger GetDefaultLogger() => (ITFLogger) new ServerTraceLogger();

    public Guid CloneApplicationHost(Guid applicationHostId, Guid? targetHostId = null)
    {
      this.Logger.Info("Executing ChangeServerIdComponent.CloneApplicationHost. ConnectionString: {0}", (object) ConnectionStringUtility.MaskPassword(this.Connection.ConnectionString));
      this.GetServiceLevel();
      this.PrepareStoredProcedure("prc_CloneApplicationHost");
      this.BindGuid("@applicationHostId", applicationHostId);
      if (targetHostId.HasValue)
        this.BindGuid("@targetHostId", targetHostId.Value);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        Guid guid = reader.Read() ? ChangeServerIdComponent.HostIdColumn.TargetHostId.GetGuid((IDataReader) reader) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
        this.Logger.Info("ChangeServerIdComponent.CloneApplicationHost completed successfully. New host ID: {0:D}.", (object) guid);
        return guid;
      }
    }

    public Guid CloneCollectionHost(Guid collectionHostId, Guid? targetHostId = null)
    {
      this.Logger.Info("Executing ChangeServerIdComponent.CloneCollectionHost. ConnectionString: {0}. Collection Host ID: {1:D}.", (object) ConnectionStringUtility.MaskPassword(this.Connection.ConnectionString), (object) collectionHostId);
      this.GetServiceLevel();
      this.PrepareStoredProcedure("prc_CloneCollectionHost");
      this.BindGuid("@collectionHostId", collectionHostId);
      if (targetHostId.HasValue)
        this.BindGuid("@targetHostId", targetHostId.Value);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        Guid guid = reader.Read() ? ChangeServerIdComponent.HostIdColumn.TargetHostId.GetGuid((IDataReader) reader) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
        this.Logger.Info("ChangeServerIdComponent.CloneCollectionHost completed successfully. New host ID: {0:D}.", (object) guid);
        return guid;
      }
    }

    public void ChangeHostId(Guid sourceHostId, Guid targetHostId)
    {
      this.Logger.Info("Executing ChangeServerIdComponent.ChangeHostId. ConnectionString: {0}. Source Host ID: {1:D}. Target Host ID: {2:D}.", (object) ConnectionStringUtility.MaskPassword(this.ConnectionInfo.ConnectionString), (object) sourceHostId, (object) targetHostId);
      ServiceLevel serviceLevel = this.GetServiceLevel();
      if (serviceLevel >= ChangeServerIdComponent.s_dev15M99ServiceLevel && serviceLevel < ChangeServerIdComponent.s_dev16M125ServiceLevel)
        this.TruncateSearchTables();
      this.PrepareStoredProcedure("prc_ChangeHostId");
      this.BindGuid("@sourceHostId", sourceHostId);
      this.BindGuid("@targetHostId", targetHostId);
      this.ExecuteNonQuery();
      this.Logger.Info("ChangeServerIdComponent.ChangeHostId completed successfully.");
    }

    public void ChangeHostIdWarehouse(Guid targetHostId)
    {
      this.Logger.Info("Executing ChangeServerIdComponent.ChangeHostIdWarehouse. ConnectionString: {0}. Target Host ID: {1:D}.", (object) ConnectionStringUtility.MaskPassword(this.Connection.ConnectionString), (object) targetHostId);
      this.PrepareStoredProcedure("prc_ChangeHostId");
      this.BindGuid("@targetHostId", targetHostId);
      this.ExecuteNonQuery();
      this.Logger.Info("ChangeServerIdComponent.ChangeHostIdWarehouse completed successfully.");
    }

    public string GetConnectionString(Guid hostId, string databaseCategory)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      ArgumentUtility.CheckStringForNullOrEmpty(databaseCategory, nameof (databaseCategory));
      this.Logger.Info("Executing ChangeServerIdComponent.GetConnectionString. ConnectionString: {0}. Host ID: {1:D}. Database category: {2}", (object) ConnectionStringUtility.MaskPassword(this.Connection.ConnectionString), (object) hostId, (object) databaseCategory);
      this.PrepareStoredProcedure("prc_GetConnectionString");
      this.BindGuid("@hostId", hostId);
      this.BindString("@databaseCategory", databaseCategory, databaseCategory.Length, false, SqlDbType.NVarChar);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        if (!sqlDataReader.Read())
        {
          this.Logger.Info("Connection string not found.");
          return (string) null;
        }
        string connectionString = sqlDataReader.GetString(0);
        this.Logger.Info("Connection string: {0}.", (object) ConnectionStringUtility.MaskPassword(connectionString));
        return connectionString;
      }
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ChangeServerIdComponent.s_sqlExceptionFactories;

    private void OnInfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
      try
      {
        this.LogInfoMessage(e, this.Logger);
      }
      catch (Exception ex)
      {
        this.Logger.Info("An error occurred while logging the following info message: {0}.", (object) e.Message);
        this.Logger.Error(ex);
      }
    }

    private void TruncateSearchTables()
    {
      string[] strArray = new string[7]
      {
        "Search.tbl_ClassificationNode",
        "Search.tbl_DisabledFiles",
        "Search.tbl_IndexingUnit",
        "Search.tbl_IndexingUnitChangeEvent",
        "Search.tbl_IndexingUnitChangeEventArchive",
        "Search.tbl_ItemLevelFailures",
        "Search.tbl_ResourceLockTable"
      };
      foreach (string str in strArray)
      {
        string sqlStatement = "IF (OBJECT_ID(N'" + str + "') IS NOT NULL) BEGIN TRUNCATE TABLE " + str + " END";
        this.PrepareSqlBatch(sqlStatement.Length);
        this.AddStatement(sqlStatement);
        this.ExecuteNonQuery();
        this.Logger.Info(str + " table truncated");
      }
    }

    private ServiceLevel GetServiceLevel()
    {
      if (this.m_serviceLevel == (ServiceLevel) null)
      {
        using (ExtendedAttributeComponent componentRaw = this.ConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
          this.m_serviceLevel = new ServiceLevel(componentRaw.ReadServiceLevelStamp());
        this.Logger.Info("ChangeServerIdComponent.GetServiceLevel. ServiceLevel: {0}", (object) this.m_serviceLevel);
      }
      return this.m_serviceLevel;
    }

    private static class HostIdColumn
    {
      public static SqlColumnBinder TargetHostId = new SqlColumnBinder("targetHostId");
    }
  }
}
