// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionServiceDatabaseBase
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public abstract class TestExecutionServiceDatabaseBase : TeamFoundationSqlResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> SqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    private IVssRequestContext _requestContext;
    private TestExecutionRequestContext _testExecutionRequestContext;

    static TestExecutionServiceDatabaseBase()
    {
      TestExecutionServiceDatabaseBase.RegisterException(SqlMessageCode.ObjectNotFound, typeof (TestExecutionObjectNotFoundSqlException));
      TestExecutionServiceDatabaseBase.RegisterException(SqlMessageCode.MessageQueueDetailsAlreadyExists, typeof (TestExecutionObjectAlreadyExistsSqlException));
      TestExecutionServiceDatabaseBase.RegisterException(SqlMessageCode.InvalidOperation, typeof (TestExecutionServiceInvalidOperationSqlException));
      TestExecutionServiceDatabaseBase.RegisterException(SqlMessageCode.TestEnvironmentAlreadyExists, typeof (TestEnvironmentAlreadyExistsSqlException));
    }

    public TestExecutionServiceDatabaseBase()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public TestExecutionServiceDatabaseBase(
      string connectionString,
      IVssRequestContext requestContext)
      : this()
    {
      this.Initialize(SqlConnectionInfoFactory.Create(connectionString), 3600, 20, 1, 1, (ITFLogger) null, (CircuitBreakerDatabaseProperties) null);
      this.PartitionId = 1;
      this._requestContext = requestContext;
      this.DtaLogger = new DtaLogger(this.TestExecutionRequestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer);
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
      this.DtaLogger = new DtaLogger(this.TestExecutionRequestContext, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer);
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
      catch (TestExecutionServiceSqlException ex)
      {
        throw Utilities.MapTestExecutionServiceException(ex);
      }
    }

    protected new int ExecuteNonQuery()
    {
      try
      {
        return base.ExecuteNonQuery();
      }
      catch (TestExecutionServiceSqlException ex)
      {
        throw Utilities.MapTestExecutionServiceException(ex);
      }
    }

    public override void Dispose()
    {
      base.Dispose();
      GC.SuppressFinalize((object) this);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) TestExecutionServiceDatabaseBase.SqlExceptionFactories;

    protected TestExecutionRequestContext TestExecutionRequestContext
    {
      get
      {
        if (this._testExecutionRequestContext == null)
          this._testExecutionRequestContext = new TestExecutionRequestContext(this._requestContext);
        return this._testExecutionRequestContext;
      }
    }

    protected IVssRequestContext TfsRequestContext => this._requestContext;

    protected DtaLogger DtaLogger { get; private set; }

    private void ConnectionInfoMessage(object sender, SqlInfoMessageEventArgs e) => this.DtaLogger.Verbose(6200000, "SQLLogMessage : " + e.Message);

    private static void RegisterException(SqlMessageCode sqlErrorCode, Type exceptionType) => TestExecutionServiceDatabaseBase.SqlExceptionFactories.Add((int) sqlErrorCode, new SqlExceptionFactory(exceptionType));
  }
}
