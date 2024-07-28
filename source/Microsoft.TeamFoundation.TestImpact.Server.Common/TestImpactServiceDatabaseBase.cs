// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestImpactServiceDatabaseBase
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public abstract class TestImpactServiceDatabaseBase : TeamFoundationSqlResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> SqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    private IVssRequestContext _requestContext;
    protected const string TestImpactSchema = "sch_TestImpact";
    protected const string m_DataspaceCategory = "TestManagement";

    static TestImpactServiceDatabaseBase() => TestImpactServiceDatabaseBase.RegisterException(SqlMessageCode.ObjectNotFound, typeof (TestImpactObjectNotFoundSqlException));

    public TestImpactServiceDatabaseBase()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal TestImpactServiceDatabaseBase(
      string connectionString,
      IVssRequestContext requestContext)
      : this()
    {
      this.Initialize(SqlConnectionInfoFactory.Create(connectionString), 3600, 20, 1, 1, (ITFLogger) null, (CircuitBreakerDatabaseProperties) null);
      this.PartitionId = 1;
      this._requestContext = requestContext;
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      base.Initialize(requestContext, databaseCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
      this._requestContext = requestContext;
    }

    protected override sealed void Initialize(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      int serviceVersion,
      ITFLogger logger,
      CircuitBreakerDatabaseProperties circuitBreakerProperties)
    {
      base.Initialize(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, serviceVersion, logger, circuitBreakerProperties);
    }

    protected override SqlDataReader ExecuteReader()
    {
      try
      {
        return base.ExecuteReader();
      }
      catch (TestImpactServiceSqlException ex)
      {
        throw Utility.MapTestExecutionServiceException(ex);
      }
    }

    protected new int ExecuteNonQuery()
    {
      try
      {
        return base.ExecuteNonQuery();
      }
      catch (TestImpactServiceSqlException ex)
      {
        throw Utility.MapTestExecutionServiceException(ex);
      }
    }

    public override void Dispose()
    {
      base.Dispose();
      GC.SuppressFinalize((object) this);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) TestImpactServiceDatabaseBase.SqlExceptionFactories;

    protected IVssRequestContext TfsRequestContext => this._requestContext;

    private static void RegisterException(SqlMessageCode sqlErrorCode, Type exceptionType) => TestImpactServiceDatabaseBase.SqlExceptionFactories.Add((int) sqlErrorCode, new SqlExceptionFactory(exceptionType));

    internal static string GetDatabaseObjectName(string objectName) => "sch_TestImpact." + objectName;
  }
}
