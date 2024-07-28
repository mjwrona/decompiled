// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationSqlResourceComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Framework.Server.Component;
using Microsoft.VisualStudio.Services.Audit;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class TeamFoundationSqlResourceComponent : 
    DatabaseResourceComponent,
    ISqlResourceComponent,
    IDBResourceComponent,
    IDisposable,
    ICancelable
  {
    private IVssRequestContext m_requestContext;
    private IDisposable m_sqlResourceLockName;
    private int m_containerErrorCode;
    private string m_sqlSprocName;
    private ISqlConnectionInfo m_connectionInfo;
    private SqlConnection m_sqlConnection;
    private SqlCommand m_sqlCommand;
    private SqlTransaction m_sqlTransaction;
    private StringBuilder m_sqlCommandText;
    private int m_numParameters;
    private int m_lastStatementIndex;
    private int m_rowCount;
    private SqlDataReader m_sqlDataReader;
    private int m_partitionId;
    private IDataspaceService m_dataspaceService;
    private int m_retriesRemaining;
    private int m_totalWaitTime;
    private int m_commandTimeout;
    private object m_scalarResult;
    private object m_unknownParam;
    private object m_unknownResult;
    private int m_serviceVersion = 1;
    private SqlResourceComponentFeatures m_selectedFeatures;
    private string m_auditActionId;
    private Dictionary<string, object> m_auditData;
    private Dictionary<string, Dictionary<string, string>> m_commonAuditCollectionItemsData;
    private HashSet<string> m_auditExcludedParameters;
    private Guid m_auditProjectId;
    private bool m_shouldExcludeSqlParameters;
    private Guid m_author;
    private StringBuilder m_sqlMessages;
    private int m_logicalReads;
    private int m_physicalReads;
    private int m_cpuTime;
    private int m_elapsedTime;
    private bool m_zeroDataspaceIdErrorLogged;
    private bool m_versionVerificationLockAcquired;
    private static readonly string s_verifyVersionStmt = "DECLARE @result INT\r\nWHILE 1 = 1\r\nBEGIN\r\n    EXEC @result = sp_getapplock @Resource = '" + FrameworkServerConstants.VerifyVersionLock + "', @LockMode = 'Shared', @LockOwner = 'Session'\r\n    if @result >= 0\r\n        BREAK\r\n    -- Sleep 100 ms\r\n    WAITFOR DELAY '00:00:00.100';\r\nEND\r\nEXEC prc_GetServiceVersion @serviceName = @serviceName";
    private static readonly string s_verifyVersionWithoutLockStmt = "EXEC prc_GetServiceVersion @serviceName = @serviceName";
    private TeamFoundationSqlResourceComponent.ErrorForensics m_errorForensics;
    private TeamFoundationSqlResourceComponent.ExecutionTrace m_executionTrace;
    private bool m_usingMainConfigDbPool;
    private bool m_configDbPrimaryPoolCountIncremented;
    private bool m_disposed;
    protected bool m_batchPrepareExecution;
    protected bool m_isAnonymousOrPublicRequest;
    protected bool m_blockAnonymousOrPublicUserWrites;
    private static int s_configDbPrimaryPoolCount;
    private static DateTime s_configDbPrimaryPoolCountMinErrorTime;
    private static int s_configDbPrimaryPoolCountReported;
    private static object s_configDbPrimaryPoolCountMinErrorLock = new object();
    private static int s_configDbPrimaryPoolBatchedErrorCount;
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static readonly Type s_oldTvpInterface = typeof (ITeamFoundationTableValueParameter<string>);
    private static readonly string s_oldTvpInterfaceName = TeamFoundationSqlResourceComponent.s_oldTvpInterface.Name;
    public const int SqlMaxBatchParameters = 2098;
    public const int MaxStatementLengthEstimate = 65536;
    private const string c_GenClientContext = "GenClientContext";
    private const string c_verifyServiceVersionPerfTimerGroup = "SQL.VerifyServiceVersion";
    protected static readonly TimeSpan s_loginErrorRetryPause = TimeSpan.FromSeconds(1.0);
    protected static readonly TimeSpan s_azureRetryDelay = TimeSpan.FromSeconds(5.0);
    protected const int c_maxWaitTime = 45000;
    protected const int c_maxRetryableExecutionTime = 300000;
    protected const int c_maxExecutionTime = 3600000;
    protected const int c_recordSerializationLimit = 5000;
    internal const string c_logicalReadsText = "logical reads ";
    internal const string c_physicalReadsText = "physical reads";
    internal const string c_readAheadReadsText = "read-ahead reads";
    internal const string c_cpuTimeText = "CPU time =";
    internal const string c_elapsedTimeText = "elapsed time =";
    internal static readonly string[] s_keywords = new string[5]
    {
      "CPU time =",
      "logical reads ",
      "physical reads",
      "elapsed time =",
      "read-ahead reads"
    };
    private const int c_logicalReadsOffset = 1;
    private const int c_physicalReadsOffset = 2;
    private const int c_readAheadReadsOffset = 4;
    private const int c_cpuTimeOffset = 0;
    private const int c_elapsedTimeOffset = 3;
    private static readonly Lazy<string> EmptyLazyString = new Lazy<string>((Func<string>) (() => string.Empty));
    public static readonly string ExtendedPropertyConfigurationInProgressStamp = "TFS_CONFIGURATION_IN_PROGRESS";
    public static readonly string ExtendedPropertyDeploymentTypeStamp = "TFS_DEPLOYMENT_TYPE";
    [Obsolete("This can only be used for warehouse database", false)]
    public static readonly string ExtendedPropertyInstanceStamp = "TFS_INSTANCE";
    public static readonly string ExtendedPropertyNonFrameworkInstanceStamp = "TFS_INSTANCE";
    public static readonly string ExtendedPropertyProductVersionStamp = "TFS_PRODUCT_VERSION";
    public static readonly string ExtendedPropertySchemaVersion = "TFS_SCHEMA_VERSION";
    public static readonly string ExtendedPropertyServiceLevelStamp = "TFS_SERVICE_LEVEL";
    public static readonly string ExtendedPropertyServiceLevelToStamp = "TFS_SERVICE_LEVEL_TO";
    public static readonly string ExtendedPropertyFinalConfigurationServiceLevelStamp = "TFS_FINAL_CONFIGURATION_SERVICE_LEVEL";
    public static readonly string ExtendedPropertyReleaseDescriptionStamp = "TFS_RELEASE_DESCRIPTION";
    public static readonly string ExtendedPropertyRecoveryModelBackupStamp = "RECOVERY_MODEL_BACKUP";
    public static readonly string ExtendedPropertyRemoveStepsSucceededStamp = "TFS_REMOVE_STEPS_SUCCEEDED";
    internal static readonly string ExtendedPropertyPreupgradeServiceLevelStamp = "TFS_PREUPGRADE_SERVICE_LEVEL";
    public static readonly string ExtendedPropertyHostDeletedInfo = "TFS_HOST_DELETED_INFO";
    public static readonly string ExtendedPropertySnapshotCollectionId = "TFS_SNAPSHOT_COLLECTIONID";
    public static readonly string ExtendedPropertyPreviousCollectionId = "TFS_PREVIOUS_COLLECTION_ID";
    public static readonly string ExtendedPropertyDatabaseType = "TFS_DATABASE_TYPE";
    public static readonly string DatabaseTypeAccount = "Account";
    public static readonly string DatabaseTypeApplication = "Application";
    public static readonly string DatabaseTypeConfiguration = "Configuration";
    public static readonly string DatabaseTypeCollection = "Collection";
    public static readonly string DatabaseTypeWarehouse = "Warehouse";
    public static readonly string DatabaseCategoryWarehouse = "Warehouse";
    private static readonly string s_prepareExecutionStatement = "EXEC prc_PrepareExecution @contextInfo, @partitionId;";
    private static readonly string s_prepareExecutionNullStatement = "EXEC prc_PrepareExecution @contextInfo, null;";
    private static readonly string s_prepareExecutionStatementV2 = "EXEC prc_PrepareExecution @contextInfo, @partitionId, @anonymousAccess, @dataspaceIdRestrictions;";
    private static readonly string s_prepareExecutionNullStatementV2 = "EXEC prc_PrepareExecution @contextInfo, null, @anonymousAccess, @dataspaceIdRestrictions;";
    private static readonly string s_prepareAuditContextInfo = "IF (OBJECT_ID('prc_PrepareAuditContextInfoV3', 'P') IS NOT NULL)\r\n            BEGIN\r\n                EXEC prc_PrepareAuditContextInfoV3 @auditCorrelationId, @auditActivityId, @auditActorCUID, @auditActorUserId, @auditActorClientId, @auditAuthenticationMechanism, @auditUserAgent, @auditIpAddress, @auditScopeId, @auditScopeType, @auditActionId, @auditCallerProcedure, @auditAdditionalData, @auditProjectId;\r\n            END\r\n            ELSE\r\n            BEGIN\r\n                EXEC prc_PrepareAuditContextInfoV2 @auditCorrelationId, @auditActivityId, @auditActorCUID, @auditActorUserId, @auditAuthenticationMechanism, @auditUserAgent, @auditIpAddress, @auditScopeId, @auditScopeType, @auditActionId, @auditCallerProcedure, @auditAdditionalData, @auditProjectId;\r\n            END;";
    private static readonly string s_waitForDatabaseCopy = "; EXEC prc_WaitForDatabaseCopy";
    [Obsolete("This should only be found on old database, you likely want to examine ExtendedPropertyDatabaseType instead", false)]
    public static readonly string ExtendedPropertyDatabaseCategories = "TFS_DATABASE_CATEGORIES";
    private static readonly string s_zeroDataspaceIdErrorPrefix = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "%error=\"{0}\"", (object) 800307);
    protected int? m_dataspaceIdRestriction;
    protected bool m_isDataspaceRestrictionsEnforced;
    private byte m_prepareExecutionVersion;
    private ReplicationType m_replicationType;
    private static object s_connectionPoolLock = new object();
    private static Stopwatch s_clearConnectionPoolStopWatch;
    protected static readonly HashSet<Type> s_circuitBreakerTrippableExceptions = new HashSet<Type>();
    private static readonly ConcurrentDictionary<string, DateTime> s_auditLargeEventTracingThrottle = new ConcurrentDictionary<string, DateTime>();
    internal static TimeSpan s_auditLargeEventTracingThrottleTime = TimeSpan.FromHours(1.0);
    private static readonly ConcurrentDictionary<string, DateTime> s_truncatedStringTracingThrottle = new ConcurrentDictionary<string, DateTime>();
    internal static TimeSpan s_truncatedStringTracingThrottleTime = TimeSpan.FromMinutes(1.0);
    internal const int c_auditEventMaxLength = 4000;
    internal const int c_auditEventTooLargeTracepoint = 64116;
    internal const int c_auditTracedLargeEventTracepoint = 64118;
    public static readonly string SchemaVersion = "Azure DevOps Server 2022.2";
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    internal static readonly ServiceLevel CurrentServiceLevel = new ServiceLevel("Dev19.M235.2");

    static TeamFoundationSqlResourceComponent()
    {
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(480000, new SqlExceptionFactory(typeof (DateTimeShiftDetectedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(-2, new SqlExceptionFactory(typeof (DatabaseOperationTimeoutException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1105, new SqlExceptionFactory(typeof (DatabaseFullException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1102, new SqlExceptionFactory(typeof (DatabaseFullException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1101, new SqlExceptionFactory(typeof (DatabaseFullException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1121, new SqlExceptionFactory(typeof (DatabaseFullException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(8999, new SqlExceptionFactory(typeof (DatabaseFullException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, sqlException, sqlError) => (Exception) new DatabaseFullException(FrameworkResources.DatabaseFullException_TempDbAllocation((object) sqlError.Server, (object) sqlError.Number, (object) sqlError.Message)))));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(9002, new SqlExceptionFactory(typeof (DatabaseFullException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, sqlException, sqlError) => (Exception) new DatabaseFullException(FrameworkResources.DatabaseFullException_TransactionLogFull((object) sqlError.Server, (object) sqlError.Number, (object) sqlError.Message)))));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(5149, new SqlExceptionFactory(typeof (DatabaseFileGrowthException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(3980, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(4060, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(58, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10053, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1229, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1231, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1265, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1326, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10054, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(87, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(951, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(945, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(929, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(922, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(952, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(921, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(928, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(6005, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(926, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(20, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(-1, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(231, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1450, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(983, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(976, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10064, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(11001, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(11002, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(11004, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10065, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17825, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17826, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17829, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17807, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(6, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17836, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18456, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18452, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17832, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17197, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(51, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(52, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(53, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(232, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1236, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1225, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(5, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17830, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(64, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(109, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(121, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17827, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(172, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17828, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(233, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(2, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(0, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1311, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10051, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17142, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17144, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17148, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17813, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10013, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10050, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17806, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(-2146892993, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(-2146893018, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1396, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10060, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10061, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(258, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40971, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(701, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(601, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(605, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(823, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(21, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(845, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(14, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(2801, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(802, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(50, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(8645, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(8628, new SqlExceptionFactory(typeof (DatabaseRuntimeException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(3908, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(943, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(923, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(924, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(4061, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(4064, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(4063, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(4062, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(9001, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(942, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(946, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(948, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(950, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(3906, new SqlExceptionFactory(typeof (DatabaseReadOnlyException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(978, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17835, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(7644, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(8145, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(349, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17892, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18401, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18470, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18486, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18451, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18450, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18457, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18487, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18488, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18461, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18458, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18460, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(18459, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17810, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(17809, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(229, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(2812, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(500, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(2766, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(949, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(8649, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(-2146893055, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(6610, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(201, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(206, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(8144, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(8146, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(2715, new SqlExceptionFactory(typeof (DatabaseConfigurationException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40604, new SqlExceptionFactory(typeof (AzureDatabaseQuotaReachedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(988, new SqlExceptionFactory(typeof (AzureServerUnavailableException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40133, new SqlExceptionFactory(typeof (AzureOperationNotSupportedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40197, new SqlExceptionFactory(typeof (AzureProcessingException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40540, new SqlExceptionFactory(typeof (DatabaseConnectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10928, new SqlExceptionFactory(typeof (AzureServiceBusyException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(10929, new SqlExceptionFactory(typeof (AzureServiceBusyException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40501, new SqlExceptionFactory(typeof (AzureServiceBusyException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40512, new SqlExceptionFactory(typeof (AzureDeprecatedFeatureException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40532, new SqlExceptionFactory(typeof (AzureLoginBadUserException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40544, new SqlExceptionFactory(typeof (AzureDatabaseQuotaReachedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40550, new SqlExceptionFactory(typeof (AzureSessionTerminatedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40552, new SqlExceptionFactory(typeof (AzureSessionTerminatedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40553, new SqlExceptionFactory(typeof (AzureSessionTerminatedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40551, new SqlExceptionFactory(typeof (AzureSessionTerminatedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40549, new SqlExceptionFactory(typeof (AzureSessionTerminatedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40613, new SqlExceptionFactory(typeof (AzureServerUnavailableException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(40615, new SqlExceptionFactory(typeof (AzureClientIPRestrictedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(480001, new SqlExceptionFactory(typeof (ISleepIfBusyInTransactionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(480002, new SqlExceptionFactory(typeof (SqlFaultInjectionException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(33504, new SqlExceptionFactory(typeof (UnauthorizedWriteException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, sqlException, sqlError) => requestContext == null || requestContext.IsAnonymousPrincipal() || requestContext.IsPublicUser() ? (Exception) new UnauthorizedWriteException(requestContext, sqlException, sqlError) : (Exception) new HostInReadOnlyModeException(requestContext, sqlException, sqlError))));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1110008, new SqlExceptionFactory(typeof (PartitionMarkedAsDeletedException)));
      TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.Add(1110007, new SqlExceptionFactory(typeof (LogAuditEventNotInTransactionException)));
      TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions.Add(typeof (DatabaseOperationTimeoutException));
      TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions.Add(typeof (DatabaseFullException));
      TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions.Add(typeof (DatabaseConnectionException));
      TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions.Add(typeof (DatabaseRuntimeException));
      TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions.Add(typeof (DatabaseConfigurationException));
      TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions.Add(typeof (AzureDatabaseQuotaReachedException));
      TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions.Add(typeof (AzureServerUnavailableException));
      TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions.Add(typeof (AzureServiceBusyException));
      TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions.Add(typeof (AzureSessionTerminatedException));
    }

    protected TeamFoundationSqlResourceComponent()
      : base("BaseSqlComponent-", "SqlResourceComponent")
    {
      this.NonVersionedComponentClassName = DatabaseResourceComponent.GetNonVersionedComponentClassName(this.GetType());
      this.ContainerErrorCode = 50000;
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger, true);
    }

    protected void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger,
      bool acquireLock)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(dataspaceCategory, nameof (dataspaceCategory));
      if (this.IsInitialized)
        throw new InvalidOperationException(FrameworkResources.AlreadyInitialized());
      this.NameOfClass = this.GetType().Name;
      if (dataspaceCategory.Equals("Framework", StringComparison.OrdinalIgnoreCase))
        dataspaceCategory = "Default";
      this.m_requestContext = requestContext;
      this.DataspaceCategory = dataspaceCategory;
      this.m_dataspaceService = this.RequestContext.GetService<IDataspaceService>();
      ITeamFoundationDatabaseProperties databaseProperties = this.m_dataspaceService.GetDatabaseProperties(requestContext, dataspaceCategory, dataspaceIdentifier);
      if (this.Connection == null && requestContext.RequestPriority == VssRequestContextPriority.Low)
      {
        IVssRequestContext requestContext1 = this.RequestContext.To(TeamFoundationHostType.Deployment);
        using (IDisposableReadOnlyList<IComponentInitExtension> extensions = requestContext1.GetExtensions<IComponentInitExtension>())
        {
          foreach (IComponentInitExtension componentInitExtension in (IEnumerable<IComponentInitExtension>) extensions)
            componentInitExtension.OnInit(requestContext1, databaseProperties, this.NameOfClass);
        }
      }
      this.CircuitBreaker = new DatabaseCircuitBreaker((DatabaseResourceComponent) this, this.RequestContext);
      this.CircuitBreaker.BaseCircuitBreakerDatabaseProperties = new CircuitBreakerDatabaseProperties(databaseProperties);
      ISqlConnectionInfo connectionInfo;
      switch (connectionType)
      {
        case DatabaseConnectionType.IntentReadOnly:
          connectionInfo = requestContext.IsFeatureEnabled(FrameworkServerConstants.ReadOnlySqlComponent) ? databaseProperties.ReadOnlyConnectionInfo : databaseProperties.SqlConnectionInfo;
          break;
        case DatabaseConnectionType.Dbo:
          connectionInfo = databaseProperties.DboConnectionInfo ?? databaseProperties.SqlConnectionInfo;
          break;
        default:
          connectionInfo = databaseProperties.SqlConnectionInfo;
          break;
      }
      if (connectionInfo == null)
      {
        string str = "SQLConnectionInfo was unexpectedly null. Check for tracepoint 98011.";
        requestContext.TraceAlways(64115, TraceLevel.Error, this.TraceArea, this.m_traceLayer, str);
        throw new InvalidOperationException(str);
      }
      string databaseName = CircuitBreakerDatabaseProperties.SanitizeDatabaseName(connectionInfo.InitialCatalog);
      try
      {
        this.SetupComponentCircuitBreaker(requestContext, databaseName, connectionInfo.ApplicationIntent);
        if (acquireLock && requestContext.ServiceHost != null && !requestContext.ServiceHost.IsProduction)
          this.m_sqlResourceLockName = requestContext.AcquireConnectionLock(ConnectionLockNameType.SQL);
        this.m_isAnonymousOrPublicRequest = requestContext.IsAnonymousPrincipal() || requestContext.IsPublicUser();
        this.m_blockAnonymousOrPublicUserWrites = !this.RequestContext.HasWriteAccess();
        DatabaseServiceObjective result;
        if (requestContext.IsServicingContext && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          result = DatabaseServiceObjective.NotAzure;
        else if (!EnumUtility.TryParse<DatabaseServiceObjective>(databaseProperties.GetDatabaseResourceStats(requestContext)?.ServiceObjective, out result))
          result = DatabaseServiceObjective.NotAzure;
        this.InitializeSettings(connectionInfo, databaseProperties.RequestTimeout, databaseProperties.DeadlockPause, databaseProperties.DeadlockRetries, databaseProperties.LoggingOptions, 0, databaseProperties.ExecutionTimeThreshold, serviceVersion, result, logger);
        this.DatabaseProperties = databaseProperties;
        if ((this.LoggingOptions & TeamFoundationDatabaseLoggingOptions.XEvents) == TeamFoundationDatabaseLoggingOptions.None && (databaseProperties.Flags & TeamFoundationDatabaseFlags.RlsEnabled) == TeamFoundationDatabaseFlags.None)
        {
          if (!this.m_blockAnonymousOrPublicUserWrites)
            goto label_37;
        }
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (databaseProperties.Status != TeamFoundationDatabaseStatus.Online && (databaseProperties.Status != TeamFoundationDatabaseStatus.Offline || !(databaseProperties.StatusReason == FrameworkServerConstants.GeoreplicationMigrationStatusReason)))
          {
            if (databaseProperties.Status != TeamFoundationDatabaseStatus.Servicing)
              goto label_37;
          }
          this.m_batchPrepareExecution = true;
          if ((databaseProperties.Flags & (TeamFoundationDatabaseFlags.RlsEnabled | TeamFoundationDatabaseFlags.DataspaceRlsEnabled)) == (TeamFoundationDatabaseFlags.RlsEnabled | TeamFoundationDatabaseFlags.DataspaceRlsEnabled))
            this.DataspaceRlsEnabled = true;
        }
      }
      catch
      {
        this.Dispose();
        throw;
      }
label_37:
      this.RequestContext.AddDisposableResource((IDisposable) this);
    }

    public void InitializeDataspaceService(IDataspaceService dataspaceService) => this.m_dataspaceService = this.m_dataspaceService == null ? dataspaceService : throw new InvalidOperationException();

    protected override void Initialize(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      int serviceVersion,
      ITFLogger logger,
      CircuitBreakerDatabaseProperties circuitBreakerProperties)
    {
      this.Initialize(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, serviceVersion, logger, circuitBreakerProperties, DatabaseServiceObjective.NotAzure);
    }

    protected void Initialize(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      int serviceVersion,
      ITFLogger logger,
      CircuitBreakerDatabaseProperties circuitBreakerProperties,
      DatabaseServiceObjective serviceObjective)
    {
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(connectionInfo, nameof (connectionInfo));
      if (this.IsInitialized)
        throw new InvalidOperationException(FrameworkResources.AlreadyInitialized());
      this.CircuitBreaker = new DatabaseCircuitBreaker((DatabaseResourceComponent) this, this.RequestContext);
      this.CircuitBreaker.BaseCircuitBreakerDatabaseProperties = circuitBreakerProperties;
      this.InitializeSettings(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, DatabaseManagementConstants.DefaultDatabaseLoggingOptions, 0, new TimeSpan(0, 0, 30), serviceVersion, serviceObjective, logger);
    }

    protected virtual ISqlConnectionInfo PrepareConnectionString(ISqlConnectionInfo connectionInfo) => connectionInfo;

    protected virtual int GetMaxPoolSize(ISqlConnectionInfo connectionInfo)
    {
      string connectionString = connectionInfo.ConnectionString;
      return (connectionString != null ? (connectionString.IndexOf("Max Pool Size", StringComparison.OrdinalIgnoreCase) < 0 ? 1 : 0) : 0) != 0 ? 100 : connectionInfo.MaxPoolSize;
    }

    private void InitializeSettings(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      TeamFoundationDatabaseLoggingOptions loggingOptions,
      int performanceOptions,
      TimeSpan executionTimeThreshold,
      int serviceVersion,
      DatabaseServiceObjective serviceObjective,
      ITFLogger logger)
    {
      this.TraceEnter(64000, nameof (InitializeSettings));
      try
      {
        this.m_commandTimeout = commandTimeout;
        this.PerformanceOptions = performanceOptions;
        this.ExecutionTimeThreshold = executionTimeThreshold;
        this.DeadlockPause = deadlockPause;
        this.MaxDeadlockRetries = maxDeadlockRetries;
        this.m_serviceVersion = serviceVersion;
        this.Logger = logger ?? this.GetDefaultLogger();
        this.m_retriesRemaining = this.MaxDeadlockRetries;
        ISqlConnectionInfo connectionInfo1 = this.PrepareConnectionString(connectionInfo);
        this.m_connectionInfo = connectionInfo1;
        this.InitialCatalog = connectionInfo1.InitialCatalog;
        this.ApplicationIntent = connectionInfo1.ApplicationIntent;
        int maxPoolSize = this.GetMaxPoolSize(connectionInfo1);
        int concurrentRequests = serviceObjective.GetMaxConcurrentRequests();
        this.MaxPoolSize = Math.Min(maxPoolSize, concurrentRequests);
        this.Trace(64113, TraceLevel.Info, "{0} ConnectionMaxPoolSize: {1}, SloMaxConcurrentRequests: {2}, use smaller value {3}", (object) this.GetType().Name, (object) maxPoolSize, (object) concurrentRequests, (object) this.MaxPoolSize);
        this.m_sqlConnection = this.m_connectionInfo.CreateSqlConnection();
        this.Trace(64001, TraceLevel.Info, "Database: {0}, command timeout: {1}, deadlock pause {2}, deadlock retries: {3}, logging options: {4}, MaxPoolSize: {5}", (object) this.m_sqlConnection.Database, (object) commandTimeout, (object) deadlockPause, (object) maxDeadlockRetries, (object) loggingOptions, (object) this.MaxPoolSize);
        if (this.InitialCatalog != null && this.InitialCatalog.IndexOf("Configuration", StringComparison.Ordinal) > -1)
          this.m_usingMainConfigDbPool = true;
        if (this.m_usingMainConfigDbPool)
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ConfigDBActiveSqlConnections").Increment();
        if (!string.IsNullOrWhiteSpace(this.InitialCatalog))
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ActiveSqlConnectionsInstance", this.InitialCatalog).Increment();
        if (TeamFoundationTracingService.IsRawTracingEnabled(64006, TraceLevel.Info, this.TraceArea, this.m_traceLayer, (string[]) null) && this.m_usingMainConfigDbPool && !(this is LockingComponent))
        {
          this.m_configDbPrimaryPoolCountIncremented = true;
          int num1 = Interlocked.Increment(ref TeamFoundationSqlResourceComponent.s_configDbPrimaryPoolCount);
          this.Trace(64003, TraceLevel.Verbose, "Active connections to ConfigDB using primary pool: {0}.", (object) num1);
          if ((double) num1 > 75.0 * (double) this.MaxPoolSize / 100.0)
            this.Trace(64004, TraceLevel.Warning, "Active connections to ConfigDB using primary pool: {0}. MaxPoolSize: {1}.", (object) num1, (object) this.MaxPoolSize);
          if ((double) num1 > 90.0 * (double) this.MaxPoolSize / 100.0)
          {
            bool flag = false;
            int num2 = 0;
            lock (TeamFoundationSqlResourceComponent.s_configDbPrimaryPoolCountMinErrorLock)
            {
              if (DateTime.UtcNow > TeamFoundationSqlResourceComponent.s_configDbPrimaryPoolCountMinErrorTime || num1 > TeamFoundationSqlResourceComponent.s_configDbPrimaryPoolCountReported)
              {
                TeamFoundationSqlResourceComponent.s_configDbPrimaryPoolCountReported = num1;
                TeamFoundationSqlResourceComponent.s_configDbPrimaryPoolCountMinErrorTime = DateTime.UtcNow.AddSeconds(30.0);
                flag = true;
                num2 = TeamFoundationSqlResourceComponent.s_configDbPrimaryPoolBatchedErrorCount;
                TeamFoundationSqlResourceComponent.s_configDbPrimaryPoolBatchedErrorCount = 0;
              }
              else
                ++TeamFoundationSqlResourceComponent.s_configDbPrimaryPoolBatchedErrorCount;
            }
            if (flag)
              this.Trace(64005, TraceLevel.Error, "Active connections to ConfigDB using primary pool: {0}. MaxPoolSize: {1}. ({2} previous errors.)", (object) num1, (object) this.MaxPoolSize, (object) num2);
          }
        }
        if (loggingOptions != TeamFoundationDatabaseLoggingOptions.None)
        {
          try
          {
            this.m_sqlConnection.InfoMessage += new SqlInfoMessageEventHandler(this.MessageHandler);
          }
          catch (Exception ex)
          {
            this.TraceException(64006, ex);
          }
        }
        this.LoggingOptions = loggingOptions;
        this.OverrideRetryTimeouts = -1;
      }
      finally
      {
        this.TraceLeave(64002, nameof (InitializeSettings));
      }
    }

    public bool IsDisposed => this.m_disposed;

    public int Version => this.m_serviceVersion;

    public string DataSource => this.m_connectionInfo?.DataSource;

    public int PartitionId
    {
      get
      {
        if (this.m_partitionId == 0 && this.m_requestContext != null)
          this.m_partitionId = this.m_requestContext.ServiceHost.PartitionId;
        return this.m_partitionId;
      }
      set => this.m_partitionId = value;
    }

    public static string Master => "master";

    public virtual int GetDataspaceId(Guid dataspaceIdentifier) => this.GetDataspaceId(dataspaceIdentifier, this.DataspaceCategory, false);

    public virtual int GetDataspaceId(Guid dataspaceIdentifier, string dataspaceCategory) => this.GetDataspaceId(dataspaceIdentifier, dataspaceCategory, false);

    public virtual int GetDataspaceId(Guid dataspaceIdentifier, bool createIfNotExists) => this.GetDataspaceId(dataspaceIdentifier, this.DataspaceCategory, createIfNotExists);

    public virtual int GetDataspaceId(
      Guid dataspaceIdentifier,
      string dataspaceCategory,
      bool createIfNotExists)
    {
      ArgumentUtility.CheckForNull<IDataspaceService>(this.m_dataspaceService, "m_dataspaceService");
      int dataspaceId;
      using (this.AcquireExemptionLock())
      {
        Dataspace dataspace = this.m_dataspaceService.QueryDataspace(this.RequestContext, dataspaceCategory, dataspaceIdentifier, !createIfNotExists);
        if (dataspace == null)
        {
          this.m_dataspaceService.CreateDataspace(this.RequestContext, dataspaceCategory, dataspaceIdentifier, DataspaceState.Active);
          dataspace = this.m_dataspaceService.QueryDataspace(this.RequestContext, dataspaceCategory, dataspaceIdentifier, true);
        }
        dataspaceId = dataspace.DataspaceId;
      }
      this.AddDataspaceId(dataspaceId, dataspaceIdentifier);
      return dataspaceId;
    }

    protected void AddDataspaceId(int dataspaceId, Guid dataspaceIdentifier)
    {
      if (dataspaceIdentifier == Guid.Empty || this.m_dataspaceIdRestriction.HasValue && this.m_dataspaceIdRestriction.Value != dataspaceId)
      {
        if (this.m_isDataspaceRestrictionsEnforced && this.m_dataspaceIdRestriction.HasValue && this.m_dataspaceIdRestriction.Value != dataspaceId)
          throw new InvalidOperationException("You cannot use EnforceDataRestrictions() in conjunction with GetDataspaceId() if the dataspaceIds are different in the two calls");
        this.DataspaceRlsEnabled = false;
      }
      else
      {
        if (this.m_dataspaceIdRestriction.HasValue)
          return;
        this.m_dataspaceIdRestriction = new int?(dataspaceId);
      }
    }

    public virtual void EnforceDataspaceRestrictions()
    {
      this.m_isDataspaceRestrictionsEnforced = true;
      this.GetDataspaceId(this.RequestContext.DataspaceIdentifier, this.DataspaceCategory, false);
    }

    public int[] GetDataspaceIds(Guid[] dataspaceIdentifiers)
    {
      ArgumentUtility.CheckForNull<Guid[]>(dataspaceIdentifiers, nameof (dataspaceIdentifiers));
      int[] dataspaceIds = new int[dataspaceIdentifiers.Length];
      for (int index = 0; index < dataspaceIdentifiers.Length; ++index)
        dataspaceIds[index] = this.GetDataspaceId(dataspaceIdentifiers[index]);
      return dataspaceIds;
    }

    public bool TryGetDataspaceId(Guid dataspaceIdentifier, out int dataspaceId)
    {
      ArgumentUtility.CheckForNull<IDataspaceService>(this.m_dataspaceService, "m_dataspaceService");
      using (this.AcquireExemptionLock())
      {
        Dataspace dataspace = this.m_dataspaceService.QueryDataspace(this.RequestContext, this.DataspaceCategory, dataspaceIdentifier, false);
        if (dataspace != null)
        {
          dataspaceId = dataspace.DataspaceId;
          return true;
        }
      }
      dataspaceId = 0;
      return false;
    }

    public virtual Guid GetDataspaceIdentifier(int dataspaceId)
    {
      ArgumentUtility.CheckForNull<IDataspaceService>(this.m_dataspaceService, "m_dataSpaceService");
      using (this.AcquireExemptionLock())
        return this.m_dataspaceService.QueryDataspace(this.RequestContext, dataspaceId).DataspaceIdentifier;
    }

    protected virtual Dataspace GetDataspace(int dataspaceId)
    {
      ArgumentUtility.CheckForNull<IDataspaceService>(this.m_dataspaceService, "m_dataSpaceService");
      using (this.AcquireExemptionLock())
        return this.m_dataspaceService.QueryDataspace(this.RequestContext, dataspaceId);
    }

    protected int DeadlockPause { get; set; }

    protected int PerformanceOptions { get; set; }

    protected TimeSpan ExecutionTimeThreshold { get; set; }

    protected int MaxDeadlockRetries { get; set; }

    protected ITFLogger Logger { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    protected int RetriesRemaining
    {
      get => this.m_retriesRemaining;
      set => this.m_retriesRemaining = value;
    }

    protected ISqlConnectionInfo ConnectionInfo => this.m_connectionInfo;

    internal ITeamFoundationDatabaseProperties DatabaseProperties { get; private set; }

    protected TeamFoundationDatabaseLoggingOptions LoggingOptions { get; private set; }

    public override void Dispose()
    {
      if (this.m_disposed)
        return;
      this.CloseReader();
      if (this.LoggingOptions != TeamFoundationDatabaseLoggingOptions.None && this.m_requestContext != null && this.m_sqlMessages != null && this.m_sqlMessages.Length > 0)
        this.m_requestContext.LogItem("SqlMessages", this.m_sqlMessages.ToString());
      this.CloseCommand();
      if (this.m_sqlTransaction != null)
        this.RollbackTransaction();
      if (this.m_versionVerificationLockAcquired)
        this.ReleaseVerificationLock();
      if (this.m_sqlConnection != null)
      {
        if (this.m_sqlConnection.State != ConnectionState.Closed)
        {
          this.m_sqlConnection.Close();
          ++this.m_errorForensics.ClosedConnectionCount;
        }
        this.m_sqlConnection.Dispose();
        VssPerformanceCounter performanceCounter;
        if (this.m_usingMainConfigDbPool)
        {
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ConfigDBActiveSqlConnections");
          performanceCounter.Decrement();
        }
        if (!string.IsNullOrWhiteSpace(this.InitialCatalog))
        {
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ActiveSqlConnectionsInstance", this.InitialCatalog);
          performanceCounter.Decrement();
        }
      }
      if (this.m_configDbPrimaryPoolCountIncremented)
        Interlocked.Decrement(ref TeamFoundationSqlResourceComponent.s_configDbPrimaryPoolCount);
      this.m_sqlResourceLockName?.Dispose();
      this.m_sqlResourceLockName = (IDisposable) null;
      GC.SuppressFinalize((object) this);
      if (this.m_requestContext != null)
        this.m_requestContext.RequestContextInternal().RemoveDisposableResource((IDisposable) this);
      this.m_disposed = true;
    }

    public override void Cancel()
    {
      TeamFoundationTracingService.TraceRaw(64010, TraceLevel.Warning, this.TraceArea, this.m_traceLayer, "SQL command canceled");
      SqlCommand sqlCommand = this.m_sqlCommand;
      if (sqlCommand == null)
        return;
      if (this.m_requestContext != null && this.m_requestContext.RequestContextInternal().MethodExecutionThread != Thread.CurrentThread)
      {
        double totalSeconds = (DateTime.UtcNow - this.m_errorForensics.ExecutionBeginTime).TotalSeconds;
        TeamFoundationTracingService.TraceRaw(64012, (TraceLevel) (totalSeconds > 10.0 ? 1 : 2), this.TraceArea, this.m_traceLayer, "ds:{0} db:{1} - Canceling command {2} after {3} s", (object) this.DataSource, (object) this.InitialCatalog, (object) sqlCommand.CommandText, (object) totalSeconds);
      }
      sqlCommand.Cancel();
    }

    public void BeginTransaction(IsolationLevel isolationLevel)
    {
      this.VerifyInitialized();
      if (this.m_sqlConnection.State != ConnectionState.Open)
      {
        this.Trace(64053, TraceLevel.Verbose, "SQL connection is {0}, opening it.", (object) this.m_sqlConnection.State);
        this.m_sqlConnection.Open();
        ++this.m_errorForensics.OpenedConnectionCount;
      }
      this.m_sqlTransaction = this.m_sqlConnection.BeginTransaction(isolationLevel);
    }

    public void CommitTransaction()
    {
      this.VerifyInitialized();
      this.CloseReader();
      if (this.m_sqlTransaction == null)
        return;
      this.m_sqlTransaction.Commit();
      this.m_sqlTransaction.Dispose();
      this.m_sqlTransaction = (SqlTransaction) null;
    }

    public void RollbackTransaction()
    {
      this.VerifyInitialized();
      this.CloseReader();
      if (this.m_sqlTransaction == null)
        return;
      if (this.m_sqlTransaction.Connection != null)
      {
        try
        {
          this.m_sqlTransaction.Rollback();
        }
        catch (InvalidOperationException ex)
        {
          this.TraceException(64037, (Exception) ex);
        }
      }
      this.m_sqlTransaction.Dispose();
      this.m_sqlTransaction = (SqlTransaction) null;
    }

    public override bool VerifyServiceVersion(
      string serviceName,
      int serviceVersion,
      out int databaseVersion,
      out int minDatabaseVersion)
    {
      this.VerifyInitialized();
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceVersion));
      ArgumentUtility.CheckForOutOfRange(serviceVersion, nameof (serviceVersion), 1);
      bool flag = this.ConnectionInfo.ApplicationIntent != ApplicationIntent.ReadOnly;
      string sqlStatement = flag ? TeamFoundationSqlResourceComponent.s_verifyVersionStmt : TeamFoundationSqlResourceComponent.s_verifyVersionWithoutLockStmt;
      bool prepareExecution = this.m_batchPrepareExecution;
      this.m_batchPrepareExecution = false;
      try
      {
        this.PrepareSqlBatch(sqlStatement.Length, false);
      }
      finally
      {
        this.m_batchPrepareExecution = prepareExecution;
      }
      this.AddStatement(sqlStatement);
      this.BindString("@serviceName", serviceName, serviceName.Length, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.CircuitBreaker.ExecuteCommandWithComponentCircuitBreaker(ExecuteType.ExecuteReader, CommandBehavior.Default, "SQL.VerifyServiceVersion");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.m_sqlDataReader, nameof (VerifyServiceVersion), this.RequestContext))
      {
        this.m_versionVerificationLockAcquired = flag;
        ServiceVersionEntryColumns binder = new ServiceVersionEntryColumns();
        resultCollection.AddBinder<ServiceVersionEntry>((ObjectBinder<ServiceVersionEntry>) binder);
        if (binder.MoveNext())
        {
          databaseVersion = binder.Current.Version;
          minDatabaseVersion = binder.Current.MinVersion;
        }
        else
          databaseVersion = minDatabaseVersion = -1;
      }
      return serviceVersion >= minDatabaseVersion && serviceVersion <= databaseVersion || databaseVersion == -1;
    }

    public event SqlInfoMessageEventHandler InfoMessage
    {
      add
      {
        this.VerifyInitialized();
        this.m_sqlConnection.InfoMessage += value;
      }
      remove
      {
        this.VerifyInitialized();
        this.m_sqlConnection.InfoMessage -= value;
      }
    }

    public bool IsSqlAzure
    {
      get
      {
        ISqlConnectionInfo connectionInfo = this.m_connectionInfo;
        return connectionInfo != null && connectionInfo.IsSqlAzure;
      }
    }

    protected SqlConnection Connection => this.m_sqlConnection;

    protected Guid Author
    {
      get
      {
        if (this.m_author == Guid.Empty && this.m_requestContext != null)
        {
          using (this.AcquireExemptionLock())
            this.m_author = this.m_requestContext.GetService<ITeamFoundationSqlNotificationService>().Author;
        }
        return this.m_author;
      }
    }

    protected SqlResourceComponentFeatures SelectedFeatures
    {
      get => this.m_selectedFeatures;
      set => this.m_selectedFeatures = value;
    }

    protected int BoundParameters => this.m_numParameters;

    protected virtual SqlDataReader DataReader => this.m_sqlDataReader;

    protected virtual SqlCommand Command => this.m_sqlCommand;

    protected string CommandText => this.m_sqlCommandText.ToString();

    internal string LastExecutedCommandText => this.m_sqlCommand != null ? TeamFoundationSqlResourceComponent.CommandTracer.SqlCommandToString(this.m_sqlCommand) : string.Empty;

    public int CommandTimeout
    {
      get => this.m_commandTimeout;
      set
      {
        this.m_commandTimeout = value;
        if (this.m_sqlCommand == null)
          return;
        this.m_sqlCommand.CommandTimeout = value;
      }
    }

    protected string ProcedureName => this.m_sqlSprocName;

    protected int StatementIndex => this.m_lastStatementIndex;

    protected int ContainerErrorCode
    {
      get => this.m_containerErrorCode;
      set => this.m_containerErrorCode = value;
    }

    private string CommandTextForTracing => this.ProcedureName ?? this.CommandText.Substring(0, Math.Min(this.CommandText.Length, (int) sbyte.MaxValue));

    protected T ExecuteQueryStoredProcedure<T>(
      int traceEnter,
      int traceException,
      int traceLeave,
      Action prepareStoredProcedure,
      System.Func<ResultCollection, T> processAndReturnResult,
      Action<Exception, int> processTraceException = null,
      System.Func<Exception, Exception> processException = null)
    {
      try
      {
        this.TraceEnter(traceEnter, nameof (ExecuteQueryStoredProcedure));
        prepareStoredProcedure();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
          return processAndReturnResult(resultCollection);
      }
      catch (Exception ex)
      {
        if (processTraceException != null)
          processTraceException(ex, traceException);
        else
          this.TraceException(traceException, ex);
        if (processException != null)
          throw processException(ex);
        throw;
      }
      finally
      {
        this.TraceLeave(traceLeave, nameof (ExecuteQueryStoredProcedure));
      }
    }

    protected void ExecuteNonQueryStoredProcedure<Tcomponent>(
      Tcomponent component,
      int traceEnter,
      int traceException,
      int traceLeave,
      Action prepareStoredProcedure)
      where Tcomponent : TeamFoundationSqlResourceComponent
    {
      try
      {
        this.TraceEnter(traceEnter, nameof (ExecuteNonQueryStoredProcedure));
        prepareStoredProcedure();
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(traceException, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(traceLeave, nameof (ExecuteNonQueryStoredProcedure));
      }
    }

    protected virtual SqlCommand PrepareStoredProcedure(string storedProcedure)
    {
      bool bindPartitionId = (this.m_selectedFeatures & SqlResourceComponentFeatures.BindPartitionIdByDefault) != 0;
      return this.PrepareStoredProcedure(storedProcedure, bindPartitionId, this.m_commandTimeout);
    }

    protected virtual SqlCommand PrepareStoredProcedure(
      string storedProcedure,
      ReplicationType replicationType)
    {
      bool bindPartitionId = (this.m_selectedFeatures & SqlResourceComponentFeatures.BindPartitionIdByDefault) != 0;
      return this.PrepareStoredProcedure(storedProcedure, bindPartitionId, new SqlCommandTimeouts(this.m_commandTimeout), replicationType);
    }

    protected virtual SqlCommand PrepareStoredProcedure(string storedProcedure, int commandTimeout)
    {
      bool bindPartitionId = (this.m_selectedFeatures & SqlResourceComponentFeatures.BindPartitionIdByDefault) != 0;
      return this.PrepareStoredProcedure(storedProcedure, bindPartitionId, commandTimeout);
    }

    protected virtual SqlCommand PrepareStoredProcedure(
      string storedProcedure,
      SqlCommandTimeouts timeoutConfiguration)
    {
      bool bindPartitionId = (this.m_selectedFeatures & SqlResourceComponentFeatures.BindPartitionIdByDefault) != 0;
      return this.PrepareStoredProcedure(storedProcedure, bindPartitionId, timeoutConfiguration);
    }

    protected virtual SqlCommand PrepareStoredProcedure(
      string storedProcedure,
      bool bindPartitionId)
    {
      return this.PrepareStoredProcedure(storedProcedure, bindPartitionId, this.m_commandTimeout);
    }

    protected virtual SqlCommand PrepareStoredProcedure(
      string storedProcedure,
      bool bindPartitionId,
      int commandTimeout)
    {
      return this.PrepareStoredProcedure(storedProcedure, bindPartitionId, new SqlCommandTimeouts(commandTimeout));
    }

    protected virtual SqlCommand PrepareStoredProcedure(
      string storedProcedure,
      bool bindPartitionId,
      SqlCommandTimeouts timeoutConfiguration,
      ReplicationType replicationType = ReplicationType.Asynchronous)
    {
      this.VerifyInitialized();
      int timeout = this.CalculateTimeout(timeoutConfiguration);
      this.NewCommand(storedProcedure.Length, timeout);
      this.m_sqlCommand.CommandType = CommandType.StoredProcedure;
      this.m_sqlSprocName = storedProcedure;
      this.m_replicationType = replicationType;
      if (this.m_batchPrepareExecution)
      {
        this.TrimExecFromSqlCommand();
        if (bindPartitionId)
        {
          if (this.m_blockAnonymousOrPublicUserWrites || this.DataspaceRlsEnabled)
          {
            this.m_prepareExecutionVersion = (byte) 2;
            this.m_sqlCommandText.Append(TeamFoundationSqlResourceComponent.s_prepareExecutionStatementV2).Append(" EXEC ");
          }
          else
          {
            this.m_prepareExecutionVersion = (byte) 1;
            this.m_sqlCommandText.Append(TeamFoundationSqlResourceComponent.s_prepareExecutionStatement).Append(" EXEC ");
          }
        }
        else if (this.m_blockAnonymousOrPublicUserWrites || this.DataspaceRlsEnabled)
        {
          this.m_prepareExecutionVersion = (byte) 2;
          this.m_sqlCommandText.Append(TeamFoundationSqlResourceComponent.s_prepareExecutionNullStatementV2).Append(" EXEC ");
        }
        else
        {
          this.m_prepareExecutionVersion = (byte) 1;
          this.m_sqlCommandText.Append(TeamFoundationSqlResourceComponent.s_prepareExecutionNullStatement).Append(" EXEC ");
        }
      }
      if (this.ShouldBindAuditContextInfoParameters())
      {
        this.TrimExecFromSqlCommand();
        this.m_sqlCommandText.Append(TeamFoundationSqlResourceComponent.s_prepareAuditContextInfo).Append(" EXEC ");
      }
      this.m_sqlCommandText.Append(storedProcedure);
      if (this.m_sqlTransaction != null)
        this.m_sqlCommand.Transaction = this.m_sqlTransaction;
      if (bindPartitionId)
      {
        if (this.PartitionId == 0)
          throw new InvalidOperationException("PartitionId is not initialized. If SQL resource component was initialized with connection string, you must set PartitionId property of the component.");
        this.BindInt("@partitionId", this.PartitionId);
      }
      return this.m_sqlCommand;
    }

    protected virtual SqlCommand PrepareSqlBatch(
      int lengthEstimate,
      SqlCommandTimeouts timeoutConfiguration)
    {
      bool bindPartitionId = (this.m_selectedFeatures & SqlResourceComponentFeatures.BindPartitionIdByDefault) != 0;
      return this.PrepareSqlBatch(lengthEstimate, bindPartitionId, timeoutConfiguration);
    }

    protected virtual SqlCommand PrepareSqlBatch(int lengthEstimate)
    {
      bool bindPartitionId = (this.m_selectedFeatures & SqlResourceComponentFeatures.BindPartitionIdByDefault) != 0;
      return this.PrepareSqlBatch(lengthEstimate, bindPartitionId);
    }

    protected virtual SqlCommand PrepareSqlBatch(int lengthEstimate, int commandTimeout)
    {
      bool bindPartitionId = (this.m_selectedFeatures & SqlResourceComponentFeatures.BindPartitionIdByDefault) != 0;
      return this.PrepareSqlBatch(lengthEstimate, bindPartitionId, commandTimeout);
    }

    protected virtual SqlCommand PrepareSqlBatch(
      int lengthEstimate,
      bool bindPartitionId,
      int commandTimeout)
    {
      return this.PrepareSqlBatch(lengthEstimate, bindPartitionId, new SqlCommandTimeouts(commandTimeout));
    }

    protected virtual SqlCommand PrepareSqlBatch(int lengthEstimate, bool bindPartitionId) => this.PrepareSqlBatch(lengthEstimate, bindPartitionId, this.m_commandTimeout);

    protected virtual SqlCommand PrepareSqlBatch(
      int lengthEstimate,
      bool bindPartitionId,
      SqlCommandTimeouts timeoutConfiguration)
    {
      this.VerifyInitialized();
      int timeout = this.CalculateTimeout(timeoutConfiguration);
      this.NewCommand(Math.Min(lengthEstimate, 65536), timeout);
      this.m_sqlCommand.CommandType = CommandType.Text;
      if (this.m_sqlTransaction != null)
        this.m_sqlCommand.Transaction = this.m_sqlTransaction;
      if (this.m_batchPrepareExecution)
      {
        if (bindPartitionId)
        {
          if (this.m_blockAnonymousOrPublicUserWrites || this.DataspaceRlsEnabled)
          {
            this.m_prepareExecutionVersion = (byte) 2;
            this.AddStatement(TeamFoundationSqlResourceComponent.s_prepareExecutionStatementV2);
          }
          else
          {
            this.m_prepareExecutionVersion = (byte) 1;
            this.AddStatement(TeamFoundationSqlResourceComponent.s_prepareExecutionStatement);
          }
        }
        else if (this.m_blockAnonymousOrPublicUserWrites)
        {
          this.m_prepareExecutionVersion = (byte) 2;
          this.AddStatement(TeamFoundationSqlResourceComponent.s_prepareExecutionNullStatementV2);
        }
        else
        {
          this.m_prepareExecutionVersion = (byte) 1;
          this.AddStatement(TeamFoundationSqlResourceComponent.s_prepareExecutionNullStatement);
        }
      }
      if (bindPartitionId)
      {
        if (this.PartitionId == 0)
          throw new InvalidOperationException("PartitionId is not initialized. If SQL resource component was initialized with connection string, you must set PartitionId property of the component.");
        this.BindInt("@partitionId", this.PartitionId);
      }
      return this.m_sqlCommand;
    }

    protected void PrepareForAuditingAction(
      string actionId,
      Dictionary<string, object> auditData = null,
      Guid projectId = default (Guid),
      Dictionary<string, Dictionary<string, string>> commonAuditCollectionItemsData = null,
      IEnumerable<string> excludeParameters = null,
      bool excludeSqlParameters = false)
    {
      IVssRequestContext requestContext = this.m_requestContext;
      if ((requestContext != null ? (!requestContext.ExecutionEnvironment.IsHostedDeployment ? 1 : 0) : 0) != 0)
        return;
      this.m_auditActionId = actionId;
      this.m_auditData = auditData;
      this.m_auditProjectId = projectId;
      this.m_commonAuditCollectionItemsData = commonAuditCollectionItemsData;
      this.m_auditExcludedParameters = new HashSet<string>((IEnumerable<string>) ((object) excludeParameters ?? (object) Array.Empty<string>()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        "@partitionId",
        "@contextInfo",
        "@dataspaceId",
        "@dataspaceIdRestrictions"
      };
      this.m_shouldExcludeSqlParameters = excludeSqlParameters;
    }

    private bool ShouldBindAuditContextInfoParameters()
    {
      if (this.m_requestContext == null || string.IsNullOrEmpty(this.m_auditActionId))
        return false;
      using (this.m_requestContext.AcquireExemptionLock())
        return this.m_requestContext.ShouldAuditLogEvents();
    }

    internal static Dictionary<string, object> GetAuditDataAsDictionary(
      string auditActionId,
      Dictionary<string, object> auditData,
      SqlParameterCollection sqlParameters,
      HashSet<string> auditExcludedParameters,
      Dictionary<string, Dictionary<string, string>> commonAuditCollectionItemsData,
      bool excludeSqlParameters)
    {
      Dictionary<string, object> dataAsDictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (auditData != null)
      {
        foreach (KeyValuePair<string, object> keyValuePair in auditData)
        {
          if (dataAsDictionary.ContainsKey(keyValuePair.Key))
            throw new AuditLogEntryContainsDuplicateDataKeyException("Audit item is already being logged with ActionId=" + auditActionId + ";key=" + keyValuePair.Key);
          dataAsDictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }
      }
      if (excludeSqlParameters)
        return dataAsDictionary;
      foreach (SqlParameter sqlParameter in (DbParameterCollection) sqlParameters)
      {
        if (!auditExcludedParameters.Contains(sqlParameter.ParameterName) && sqlParameter.Value != null)
        {
          string key = sqlParameter.ParameterName.TrimStart('@');
          if (dataAsDictionary.ContainsKey(key))
            throw new AuditLogEntryContainsDuplicateDataKeyException("Audit item is already being logged with ActionId=" + auditActionId + ";key=" + key + "; SQL Parameters are automatically audited, and do not need to be manually added to the audit data.");
          object list;
          if (sqlParameter.SqlDbType == SqlDbType.Structured && sqlParameter.Value is IEnumerable<SqlDataRecord> source)
          {
            Dictionary<string, string> commonItemValues = (Dictionary<string, string>) null;
            if (commonAuditCollectionItemsData != null && commonAuditCollectionItemsData.TryGetValue(key, out commonItemValues))
              dataAsDictionary.Add(key + "_commonValues", (object) commonItemValues);
            list = (object) source.Select<SqlDataRecord, Dictionary<string, object>>((System.Func<SqlDataRecord, Dictionary<string, object>>) (r => TeamFoundationSqlResourceComponent.KeyValuePairs(r, commonItemValues))).ToList<Dictionary<string, object>>();
          }
          else
            list = (object) sqlParameter.Value.ToString();
          dataAsDictionary.Add(key, list);
        }
      }
      return dataAsDictionary;
    }

    private static Dictionary<string, object> KeyValuePairs(
      SqlDataRecord record,
      Dictionary<string, string> commonItemValues)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      for (int ordinal = 0; ordinal < record.FieldCount; ++ordinal)
      {
        string name = record.GetName(ordinal);
        object obj = record.GetValue(ordinal);
        string str;
        if (commonItemValues == null || !commonItemValues.TryGetValue(name, out str) || !(obj?.ToString() == (str ?? DBNull.Value.ToString())))
          dictionary[name] = obj;
      }
      return dictionary;
    }

    private void BindAuditContextInfoParameters()
    {
      if (!this.ShouldBindAuditContextInfoParameters() || this.m_requestContext == null)
        return;
      IVssRequestContext requestContext = this.m_requestContext;
      string auditActionId = this.m_auditActionId;
      Guid auditProjectId = this.m_auditProjectId;
      Guid targetHostId = new Guid();
      Guid projectId = auditProjectId;
      AuditLogEntry auditLogEntryRaw = requestContext.CreateAuditLogEntryRaw(auditActionId, targetHostId: targetHostId, projectId: projectId);
      if (auditLogEntryRaw == null)
        return;
      string parameterValue = TeamFoundationSqlResourceComponent.GetAuditDataAsDictionary(this.m_auditActionId, this.m_auditData, this.m_sqlCommand.Parameters, this.m_auditExcludedParameters, this.m_commonAuditCollectionItemsData, this.m_shouldExcludeSqlParameters).Serialize<Dictionary<string, object>>();
      if (parameterValue.Length > 4000)
      {
        this.m_requestContext.TraceAlways(64116, TraceLevel.Info, this.TraceArea, this.m_traceLayer, string.Format("Audit item exceeded the {0} character limit and will not be logged with ActionId={1}, size={2}", (object) 4000, (object) this.m_auditActionId, (object) parameterValue.Length));
        DateTime utcNow = DateTime.UtcNow;
        DateTime dateTime;
        if ((!TeamFoundationSqlResourceComponent.s_auditLargeEventTracingThrottle.TryGetValue(this.m_auditActionId, out dateTime) ? 1 : (utcNow - dateTime > TeamFoundationSqlResourceComponent.s_auditLargeEventTracingThrottleTime ? 1 : 0)) != 0)
        {
          this.m_requestContext.TraceAlways(64118, TraceLevel.Info, this.TraceArea, this.m_traceLayer, this.m_auditActionId + ":" + parameterValue);
          TeamFoundationSqlResourceComponent.s_auditLargeEventTracingThrottle[this.m_auditActionId] = utcNow;
        }
        parameterValue = (string) null;
      }
      this.BindString("@auditAdditionalData", parameterValue, 4000, true, SqlDbType.NVarChar);
      this.BindGuid("@auditProjectId", this.m_auditProjectId);
      this.BindGuid("@auditCorrelationId", auditLogEntryRaw.CorrelationId);
      this.BindGuid("@auditActivityId", auditLogEntryRaw.ActivityId);
      this.BindGuid("@auditActorCUID", auditLogEntryRaw.ActorCUID);
      this.BindGuid("@auditActorUserId", auditLogEntryRaw.ActorUserId);
      this.BindGuid("@auditActorClientId", auditLogEntryRaw.ActorClientId);
      this.BindString("@auditActorUPN", auditLogEntryRaw.ActorUPN, 64, true, SqlDbType.VarChar);
      this.BindString("@auditAuthenticationMechanism", auditLogEntryRaw.AuthenticationMechanism, 96, true, SqlDbType.VarChar);
      this.BindString("@auditUserAgent", auditLogEntryRaw.UserAgent, 512, true, SqlDbType.VarChar);
      this.BindString("@auditCallerProcedure", this.m_sqlSprocName, 128, true, SqlDbType.VarChar);
      this.BindString("@auditActionId", this.m_auditActionId, 512, true, SqlDbType.VarChar);
      this.BindString("@auditIpAddress", auditLogEntryRaw.IPAddress, 45, true, SqlDbType.VarChar);
      this.BindGuid("@auditScopeId", auditLogEntryRaw.ScopeId);
      this.BindInt("@auditScopeType", (int) auditLogEntryRaw.ScopeType);
    }

    protected virtual void AddStatement(string sqlStatement) => this.AddStatement(sqlStatement, 0, true, true);

    protected int AddStatement(string sqlStatement, int parameterCount) => this.AddStatement(sqlStatement, parameterCount, true, true);

    protected int AddStatement(string sqlStatement, int parameterCount, bool allowExecute) => this.AddStatement(sqlStatement, parameterCount, allowExecute, true);

    protected virtual int AddStatement(
      string sqlStatement,
      int parameterCount,
      bool allowExecute,
      bool addStatementIndex)
    {
      this.VerifyInitialized();
      if (allowExecute && this.m_numParameters + parameterCount > 2098)
        this.FlushBatch();
      this.m_numParameters += parameterCount;
      ++this.m_lastStatementIndex;
      this.m_sqlCommandText.AppendLine(addStatementIndex ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, sqlStatement, (object) this.m_lastStatementIndex) : sqlStatement);
      return this.m_lastStatementIndex;
    }

    protected void FlushBatch()
    {
      this.ExecuteNonQuery();
      this.PrepareSqlBatch(this.m_sqlCommandText.Capacity);
    }

    protected string GetParameterName(string name, int index) => name + index.ToString();

    protected virtual SqlParameter BindNullValue(string parameterName, SqlDbType dbType)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, dbType);
      sqlParameter.IsNullable = true;
      sqlParameter.Value = (object) DBNull.Value;
      return sqlParameter;
    }

    protected virtual SqlParameter BindShort(string parameterName, short parameterValue)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.SmallInt);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableShort(
      string parameterName,
      short parameterValue,
      short nullValue)
    {
      if ((int) parameterValue == (int) nullValue)
        return this.BindNullValue(parameterName, SqlDbType.SmallInt);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.SmallInt);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableShort(string parameterName, short? parameterValue)
    {
      if (!parameterValue.HasValue)
        return this.BindNullValue(parameterName, SqlDbType.SmallInt);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.SmallInt);
      sqlParameter.Value = (object) parameterValue.Value;
      return sqlParameter;
    }

    protected virtual SqlParameter BindString(
      string parameterName,
      string parameterValue,
      int maxLength,
      bool allowNull,
      SqlDbType dbType)
    {
      BindStringBehavior behavior = allowNull ? BindStringBehavior.EmptyStringToNull : BindStringBehavior.NullToEmptyString;
      return this.BindString(parameterName, parameterValue, maxLength, behavior, dbType);
    }

    protected virtual SqlParameter BindString(
      string parameterName,
      string parameterValue,
      int maxLength,
      BindStringBehavior behavior,
      SqlDbType dbType)
    {
      if (parameterValue == null && BindStringBehavior.NullToEmptyString == behavior)
        parameterValue = string.Empty;
      else if (parameterValue == null || parameterValue.Length == 0 && BindStringBehavior.EmptyStringToNull == behavior)
        return this.BindNullValue(parameterName, dbType);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, dbType);
      if (maxLength > 0 && parameterValue.Length > maxLength)
      {
        if (this.IsTracing(64030, TraceLevel.Warning))
        {
          this.Trace(64030, TraceLevel.Warning, string.Format("Parameter '{0}' has a value that exceeds the max length of {1} and will be truncated.", (object) parameterName, (object) maxLength));
        }
        else
        {
          string key = this.TraceArea + ":" + this.m_traceLayer + ":" + parameterName;
          DateTime utcNow = DateTime.UtcNow;
          DateTime dateTime;
          if ((!TeamFoundationSqlResourceComponent.s_truncatedStringTracingThrottle.TryGetValue(key, out dateTime) ? 1 : (utcNow - dateTime > TeamFoundationSqlResourceComponent.s_truncatedStringTracingThrottleTime ? 1 : 0)) != 0)
          {
            this.TraceAlways(64030, TraceLevel.Warning, string.Format("[Throttled] Parameter '{0}' has a value that exceeds the max length of {1} and will be truncated.", (object) parameterName, (object) maxLength));
            TeamFoundationSqlResourceComponent.s_truncatedStringTracingThrottle[key] = utcNow;
          }
        }
        sqlParameter.Value = (object) parameterValue.Substring(0, maxLength);
      }
      else
        sqlParameter.Value = (object) parameterValue;
      if (maxLength > 0)
        sqlParameter.Size = maxLength;
      return sqlParameter;
    }

    protected virtual SqlParameter BindSysname(
      string parameterName,
      string parameterValue,
      bool allowNull)
    {
      return this.BindString(parameterName, parameterValue, 128, allowNull, SqlDbType.NVarChar);
    }

    protected virtual SqlParameter BindGuid(string parameterName, Guid parameterValue)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.UniqueIdentifier);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableGuid(string parameterName, Guid parameterValue)
    {
      if (parameterValue == Guid.Empty)
        return this.BindNullValue(parameterName, SqlDbType.UniqueIdentifier);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.UniqueIdentifier);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableGuid(string parameterName, Guid? parameterValue)
    {
      if (!parameterValue.HasValue)
        return this.BindNullValue(parameterName, SqlDbType.UniqueIdentifier);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.UniqueIdentifier);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindInt(string parameterName, int parameterValue)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.Int);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableInt(string parameterName, int? parameterValue)
    {
      if (!parameterValue.HasValue)
        return this.BindNullValue(parameterName, SqlDbType.Int);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.Int);
      sqlParameter.Value = (object) parameterValue.Value;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableInt(
      string parameterName,
      int parameterValue,
      int nullValue)
    {
      if (parameterValue == nullValue)
        return this.BindNullValue(parameterName, SqlDbType.Int);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.Int);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableLong(string parameterName, long? parameterValue)
    {
      if (!parameterValue.HasValue)
        return this.BindNullValue(parameterName, SqlDbType.BigInt);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.BigInt);
      sqlParameter.Value = (object) parameterValue.Value;
      return sqlParameter;
    }

    protected virtual SqlParameter BindDecimal(string parameterName, Decimal parameterValue) => this.BindNullableDecimal(parameterName, new Decimal?(parameterValue));

    protected virtual SqlParameter BindNullableDecimal(
      string parameterName,
      Decimal? parameterValue)
    {
      if (!parameterValue.HasValue)
        return this.BindNullValue(parameterName, SqlDbType.Decimal);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.Decimal);
      sqlParameter.Value = (object) parameterValue.Value;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableDecimal(
      string parameterName,
      Decimal parameterValue,
      Decimal nullValue)
    {
      Decimal? parameterValue1 = new Decimal?();
      if (parameterValue != nullValue)
        parameterValue1 = new Decimal?(parameterValue);
      return this.BindNullableDecimal(parameterName, parameterValue1);
    }

    protected virtual SqlParameter BindDateTimeOffset(
      string parameterName,
      DateTimeOffset parameterValue)
    {
      return this.BindNullableDateTimeOffset(parameterName, new DateTimeOffset?(parameterValue));
    }

    protected virtual SqlParameter BindNullableDateTimeOffset(
      string parameterName,
      DateTimeOffset? parameterValue)
    {
      if (!parameterValue.HasValue)
        return this.BindNullValue(parameterName, SqlDbType.DateTimeOffset);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.DateTimeOffset);
      sqlParameter.Value = (object) parameterValue.Value;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableDateTimeOffset(
      string parameterName,
      DateTimeOffset parameterValue,
      DateTimeOffset nullValue)
    {
      DateTimeOffset? parameterValue1 = new DateTimeOffset?();
      if (parameterValue != nullValue)
        parameterValue1 = new DateTimeOffset?(parameterValue);
      return this.BindNullableDateTimeOffset(parameterName, parameterValue1);
    }

    protected virtual SqlParameter BindDouble(string parameterName, double parameterValue)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.Float);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableDouble(string parameterName, double? parameterValue) => !parameterValue.HasValue ? this.BindNullValue(parameterName, SqlDbType.Float) : this.BindDouble(parameterName, parameterValue.Value);

    protected virtual SqlParameter BindLong(string parameterName, long parameterValue)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.BigInt);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindRowVersion(string parameterName, ulong parameterValue)
    {
      byte[] parameterValue1 = new byte[8]
      {
        (byte) (parameterValue >> 56),
        (byte) (parameterValue >> 48),
        (byte) (parameterValue >> 40),
        (byte) (parameterValue >> 32),
        (byte) (parameterValue >> 24),
        (byte) (parameterValue >> 16),
        (byte) (parameterValue >> 8),
        (byte) parameterValue
      };
      return this.BindBinary(parameterName, parameterValue1, SqlDbType.Timestamp);
    }

    protected virtual SqlParameter BindByte(string parameterName, byte parameterValue)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.TinyInt);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindByte(
      string parameterName,
      byte parameterValue,
      byte nullValue)
    {
      if ((int) parameterValue == (int) nullValue)
        return this.BindNullValue(parameterName, SqlDbType.TinyInt);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.TinyInt);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableByte(
      string parameterName,
      byte parameterValue,
      byte nullValue)
    {
      if ((int) parameterValue == (int) nullValue)
        return this.BindNullValue(parameterName, SqlDbType.TinyInt);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.TinyInt);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableByte(string parameterName, byte? parameterValue)
    {
      if (!parameterValue.HasValue)
        return this.BindNullValue(parameterName, SqlDbType.TinyInt);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.TinyInt);
      sqlParameter.Value = (object) parameterValue.Value;
      return sqlParameter;
    }

    protected virtual SqlParameter BindBoolean(string parameterName, bool parameterValue)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.Bit);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableBoolean(string parameterName, bool? parameterValue) => !parameterValue.HasValue ? this.BindNullValue(parameterName, SqlDbType.Bit) : this.BindBoolean(parameterName, parameterValue.Value);

    protected virtual SqlParameter BindDate(string parameterName, DateTime parameterValue)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.Date);
      sqlParameter.Value = (object) parameterValue.Date;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableDate(string parameterName, DateTime? parameterValue) => !parameterValue.HasValue ? this.BindNullValue(parameterName, SqlDbType.Date) : this.BindDate(parameterName, parameterValue.Value);

    protected virtual SqlParameter BindDateTime(string parameterName, DateTime parameterValue) => this.BindDateTime(parameterName, parameterValue, false);

    protected virtual SqlParameter BindDateTime(
      string parameterName,
      DateTime parameterValue,
      bool trimToSqlMinValue)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.DateTime);
      if (trimToSqlMinValue && DateTime.Compare(parameterValue, SqlDateTime.MinValue.Value) < 0)
        sqlParameter.Value = (object) SqlDateTime.MinValue.Value;
      else
        sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableDateTime(
      string parameterName,
      DateTime? parameterValue)
    {
      return !parameterValue.HasValue ? this.BindNullValue(parameterName, SqlDbType.DateTime) : this.BindDateTime(parameterName, parameterValue.Value);
    }

    protected virtual SqlParameter BindDateTime2(string parameterName, DateTime parameterValue)
    {
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.DateTime2);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindNullableDateTime2(
      string parameterName,
      DateTime? parameterValue)
    {
      return !parameterValue.HasValue ? this.BindNullValue(parameterName, SqlDbType.DateTime2) : this.BindDateTime2(parameterName, parameterValue.Value);
    }

    protected virtual SqlParameter BindBinary(
      string parameterName,
      byte[] parameterValue,
      int length,
      SqlDbType sqlType)
    {
      if (!string.IsNullOrEmpty(this.m_auditActionId) && !this.m_auditExcludedParameters.Contains(parameterName))
        this.Trace(64117, TraceLevel.Warning, "The audit event with action id: " + this.m_auditActionId + " should exclude a binary parameter named: " + parameterName + ".");
      if (parameterValue == null)
        return this.BindNullValue(parameterName, sqlType);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, sqlType);
      if (length > 0)
      {
        sqlParameter.Value = (object) parameterValue;
        sqlParameter.Size = length;
      }
      else
        sqlParameter.Value = (object) Array.Empty<byte>();
      return sqlParameter;
    }

    protected virtual SqlParameter BindBinary(
      string parameterName,
      byte[] parameterValue,
      SqlDbType sqlType)
    {
      return this.BindBinary(parameterName, parameterValue, parameterValue != null ? parameterValue.Length : 0, sqlType);
    }

    protected virtual SqlParameter BindXml(string parameterName, string parameterValue)
    {
      if (parameterValue == null)
        return this.BindNullValue(parameterName, SqlDbType.NVarChar);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.NVarChar);
      sqlParameter.Value = (object) parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindXml(
      string parameterName,
      TeamFoundationDatabaseXmlWriter xmlWriter)
    {
      if (xmlWriter == null)
        return this.BindNullValue(parameterName, SqlDbType.NVarChar);
      SqlParameter sqlParameter = this.Command.Parameters.Add(parameterName, SqlDbType.NVarChar);
      sqlParameter.Value = (object) xmlWriter.GetXmlString();
      return sqlParameter;
    }

    protected virtual SqlParameter BindVariant(string parameterName, object parameterValue)
    {
      if (parameterValue == null)
        return this.BindNullValue(parameterName, SqlDbType.NVarChar);
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.Add(parameterName, SqlDbType.Variant);
      sqlParameter.Value = parameterValue;
      return sqlParameter;
    }

    protected virtual SqlParameter BindDefault(string parameterName, object parameterValue) => this.m_sqlCommand.Parameters.AddWithValue(parameterName, parameterValue);

    [Obsolete("BindTable<T>() is now obsolete. Please convert your code to use type-specific binding methods. For example, use BindStringTable() instead of BindTable<String>().")]
    protected virtual SqlParameter BindTable<T>(
      string parameterName,
      TeamFoundationTableValueParameter<T> tvp)
    {
      object obj = (object) null;
      if (tvp != null && !tvp.IsNullOrEmpty)
        obj = (object) tvp;
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.AddWithValue(parameterName, obj);
      sqlParameter.TypeName = tvp.TypeName;
      sqlParameter.SqlDbType = SqlDbType.Structured;
      return sqlParameter;
    }

    public virtual SqlParameter BindTable(
      string parameterName,
      string typeName,
      IEnumerable<SqlDataRecord> rows,
      bool convertEmptyToNull = true)
    {
      if (convertEmptyToNull && (rows == null || !rows.Any<SqlDataRecord>()))
        rows = (IEnumerable<SqlDataRecord>) null;
      SqlParameter sqlParameter = this.m_sqlCommand.Parameters.AddWithValue(parameterName, (object) rows);
      sqlParameter.TypeName = typeName;
      sqlParameter.SqlDbType = SqlDbType.Structured;
      return sqlParameter;
    }

    protected virtual void Execute(ExecuteType executeType, CommandBehavior behavior) => this.CircuitBreaker.ExecuteCommandWithComponentCircuitBreaker(executeType, behavior, "SQL");

    protected virtual Task ExecuteAsync(ExecuteType executeType, CommandBehavior behavior)
    {
      IVssRequestContext requestContext = this.RequestContext;
      if (requestContext != null)
        requestContext.AssertAsyncExecutionEnabled();
      return this.CircuitBreaker.ExecuteCommandWithComponentCircuitBreakerAsync(executeType, behavior, "SQL");
    }

    private void BindPrepareExecutionParameters()
    {
      if ((!this.m_batchPrepareExecution || this.m_sqlCommand.CommandType == CommandType.TableDirect) && !this.ShouldBindAuditContextInfoParameters() && this.m_replicationType != ReplicationType.Synchronous)
        return;
      if (this.m_sqlCommand.CommandType == CommandType.StoredProcedure)
      {
        this.m_sqlCommand.CommandType = CommandType.Text;
        List<SqlParameter> sqlParameterList = new List<SqlParameter>();
        foreach (SqlParameter parameter in (DbParameterCollection) this.m_sqlCommand.Parameters)
        {
          if (!parameter.ParameterName.StartsWith("@"))
            parameter.ParameterName = parameter.ParameterName.Insert(0, "@");
          if (parameter.SqlDbType == SqlDbType.Structured)
          {
            if ((!(parameter.Value is IEnumerable<SqlDataRecord> sqlDataRecords) || !(sqlDataRecords.GetType().GetInterface(TeamFoundationSqlResourceComponent.s_oldTvpInterfaceName) != (Type) null)) && sqlDataRecords.IsNullOrEmpty<SqlDataRecord>())
            {
              this.m_sqlCommandText.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " {0}=default,", (object) parameter.ParameterName));
              sqlParameterList.Add(parameter);
              continue;
            }
            if (sqlDataRecords != null && this.DataspaceRlsEnabled)
              parameter.Value = (object) sqlDataRecords.ToList<SqlDataRecord>();
          }
          if (parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
            this.m_sqlCommandText.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " {0}={0} OUTPUT,", (object) parameter.ParameterName));
          else if (parameter.Direction == ParameterDirection.Input)
            this.m_sqlCommandText.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, " {0}={0},", (object) parameter.ParameterName));
        }
        foreach (SqlParameter sqlParameter in sqlParameterList)
          this.m_sqlCommand.Parameters.Remove(sqlParameter);
        if (this.m_sqlCommandText.Length > 0 && this.m_sqlCommandText[this.m_sqlCommandText.Length - 1] == ',')
          this.m_sqlCommandText.Remove(this.m_sqlCommandText.Length - 1, 1);
      }
      if (this.m_batchPrepareExecution && this.m_sqlCommand.CommandType != CommandType.TableDirect)
      {
        this.BindBinary("@contextInfo", ContextInfo.Encode(this.RequestContext, this.m_sqlSprocName), SqlDbType.VarBinary);
        if (this.m_prepareExecutionVersion >= (byte) 2)
        {
          if (this.DataspaceRlsEnabled && this.m_dataspaceIdRestriction.HasValue)
            this.BindInt("@dataspaceIdRestrictions", this.m_dataspaceIdRestriction.Value);
          else
            this.BindNullValue("@dataspaceIdRestrictions", SqlDbType.Int);
          if (this.m_blockAnonymousOrPublicUserWrites)
            this.BindBoolean("@anonymousAccess", true);
          else
            this.BindNullValue("@anonymousAccess", SqlDbType.Bit);
        }
      }
      this.BindAuditContextInfoParameters();
      if (this.m_replicationType != ReplicationType.Synchronous)
        return;
      this.m_sqlCommandText.Append(TeamFoundationSqlResourceComponent.s_waitForDatabaseCopy);
    }

    internal override void ExecuteCommand(
      ExecuteType executeType,
      CommandBehavior behavior,
      string performanceGroupName)
    {
      this.BindPrepareExecutionParameters();
      this.m_executionTrace = new TeamFoundationSqlResourceComponent.ExecutionTrace();
      PerformanceTimer performanceTimer1 = PerformanceTimer.StartMeasure(this.RequestContext, performanceGroupName);
      bool flag1 = string.Equals(performanceGroupName, "SQL", StringComparison.Ordinal);
      PerformanceTimer performanceTimer2 = new PerformanceTimer();
      PerformanceTimer performanceTimer3 = new PerformanceTimer();
      bool flag2 = flag1 && this.ApplicationIntent == ApplicationIntent.ReadOnly;
      PerformanceTimer performanceTimer4 = new PerformanceTimer();
      if (flag2)
        performanceTimer4.Start(this.RequestContext, "SQLReadOnly");
      Guid uniqueIdentifier = this.RequestContext != null ? this.RequestContext.UniqueIdentifier : Guid.Empty;
      this.m_totalWaitTime = 0;
      TeamFoundationSqlResourceComponent.CommandTracer commandTracer = new TeamFoundationSqlResourceComponent.CommandTracer(this.m_sqlCommand);
      string commandTextForTracing = this.CommandTextForTracing;
      performanceTimer1.AddProperty("Command", (object) commandTextForTracing);
      performanceTimer1.AddProperty("Area", (object) this.TraceArea);
      if (flag2)
      {
        performanceTimer4.AddProperty("Command", (object) commandTextForTracing);
        performanceTimer4.AddProperty("Area", (object) this.TraceArea);
      }
      this.TraceEnter(64020, string.Format("Execute {0} ds:{1} db:{2}", (object) commandTextForTracing, (object) this.DataSource, (object) this.Database));
      try
      {
        this.VerifyInitialized();
        ++this.m_errorForensics.ExecuteCalledCount;
        this.m_errorForensics.ExecutionBeginTime = DateTime.UtcNow;
        this.m_errorForensics.ExecuteAttempts = 0;
        this.m_errorForensics.SqlExecuteCalls = 0;
        this.m_errorForensics.LastSqlExecuteBeginTime = new DateTime();
        this.m_errorForensics.LastSqlExecuteEndTime = new DateTime();
        this.m_executionTrace.Database = this.m_errorForensics.Database = this.InitialCatalog;
        this.m_executionTrace.DataSource = this.m_errorForensics.DataSource = this.DataSource;
        if (this.m_sqlCommand == null)
        {
          this.Trace(64021, TraceLevel.Error, "Execute called with null SQL command");
          throw new NoNullAllowedException(FrameworkResources.NullStoredProcException());
        }
        Lazy<string> parametersForTracing = this.GetStringParametersForTracing();
        if (this.m_sqlCommand.Parameters.Count == 0)
          this.Trace(64023, TraceLevel.Info, "no parameters");
        else
          this.Trace(64023, TraceLevel.Info, "{0}", (object) parametersForTracing);
        VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalSqlBatches");
        performanceCounter.Increment();
        this.CloseReader();
        this.m_sqlCommand.CommandText = this.CommandText;
        this.m_executionTrace.Operation = this.CommandTextForTracing;
        if ((this.LoggingOptions & TeamFoundationDatabaseLoggingOptions.CommandText) != TeamFoundationDatabaseLoggingOptions.None)
        {
          this.Trace(64022, TraceLevel.Info, "Database logging enabled: CommandText");
          this.SqlMessages.AppendLine(TeamFoundationSqlResourceComponent.CommandTracer.SqlCommandToString(this.m_sqlCommand));
        }
        else if ((this.LoggingOptions & TeamFoundationDatabaseLoggingOptions.Statistics) != TeamFoundationDatabaseLoggingOptions.None)
        {
          this.Trace(64025, TraceLevel.Info, "Database logging enabled for stored procedure {0}: Statistics", (object) this.m_sqlSprocName);
          this.SqlMessages.AppendLine(this.m_sqlSprocName);
          this.SqlMessages.Append('-', 20);
          this.SqlMessages.Append(Environment.NewLine);
        }
        else if ((this.LoggingOptions & TeamFoundationDatabaseLoggingOptions.XEvents) != TeamFoundationDatabaseLoggingOptions.None)
          this.Trace(64042, TraceLevel.Info, "Database logging enabled for stored procedure {0}: XEvents", (object) this.m_sqlSprocName);
        bool flag3 = false;
        while (!flag3)
        {
          ++this.m_errorForensics.ExecuteAttempts;
          this.m_errorForensics.SqlOperation = string.Empty;
          if (flag1)
          {
            performanceTimer3.End();
            performanceTimer3 = PerformanceTimer.StartMeasure(this.RequestContext, "SQLRetries");
          }
          bool flag4 = false;
          if (this.m_usingMainConfigDbPool)
          {
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ConfigDBCurrentSQLExecutionsPerSec");
            performanceCounter.Increment();
          }
          if (!string.IsNullOrWhiteSpace(this.InitialCatalog))
          {
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSQLExecutionsPerSecInstance", this.InitialCatalog);
            performanceCounter.Increment();
          }
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSQLComponentCallsPerSecInstance", this.NonVersionedComponentClassName);
          performanceCounter.Increment();
          try
          {
            Stopwatch stopwatch1 = new Stopwatch();
            this.m_errorForensics.SqlOperation = "read SqlConnection.State";
            if (this.m_sqlConnection.State != ConnectionState.Open)
            {
              this.Trace(64026, TraceLevel.Info, "Opening SQL connection currently in state {0}", (object) this.m_sqlConnection.State);
              this.m_errorForensics.SqlOperation = "execute SqlConnection.Open()";
              stopwatch1.Start();
              try
              {
                this.m_sqlConnection.Open();
                ++this.m_errorForensics.OpenedConnectionCount;
              }
              finally
              {
                stopwatch1.Stop();
                this.m_executionTrace.ConnectTime += (int) stopwatch1.ElapsedMilliseconds;
                VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlConnectionTime").IncrementMilliseconds(stopwatch1.ElapsedMilliseconds);
                VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlConnectionTimeBase").Increment();
              }
            }
            this.m_errorForensics.SqlOperation = string.Empty;
            Stopwatch stopwatch2 = new Stopwatch();
            try
            {
              if (this.RequestContext != null)
              {
                this.RequestContext.EnterCancelableRegion((ICancelable) this);
                if (this.RequestContext.Items.ContainsKey(RequestContextItemsKeys.TrackDatabaseStatistics))
                {
                  if ((this.LoggingOptions & TeamFoundationDatabaseLoggingOptions.Statistics) != TeamFoundationDatabaseLoggingOptions.None)
                  {
                    try
                    {
                      using (SqlCommand sqlCommand = new SqlCommand("SET STATISTICS TIME ON", this.m_sqlConnection))
                      {
                        sqlCommand.CommandTimeout = 1;
                        sqlCommand.ExecuteNonQuery();
                      }
                    }
                    catch (Exception ex)
                    {
                      this.TraceException(1109819422, ex);
                    }
                  }
                }
              }
              flag4 = true;
              this.Trace(64027, TraceLevel.Info, "Call the appropriate m_sqlCommand.ExecuteXXX function for ExecuteType: {0}", (object) executeType);
              this.m_errorForensics.SqlOperation = "execute SqlCommand.ExecuteXXX";
              ++this.m_errorForensics.SqlExecuteCalls;
              this.m_errorForensics.LastSqlExecuteBeginTime = DateTime.UtcNow;
              stopwatch2.Start();
              VssPerformanceEventSource.Log.SQLStart(uniqueIdentifier, this.m_sqlCommand.CommandText, this.DataSource, this.Database);
              if (flag1)
              {
                performanceTimer2.Invalidate();
                performanceTimer2 = PerformanceTimer.StartMeasure(this.RequestContext, "FinalSQLCommand");
              }
              this.PreExecuteFaultInjection();
              switch (executeType)
              {
                case ExecuteType.ExecuteReader:
                  using (this.GetSlowQueryTraceWatch(parametersForTracing))
                  {
                    this.m_sqlDataReader = this.m_sqlCommand.ExecuteReader(behavior);
                    break;
                  }
                case ExecuteType.ExecuteNonQuery:
                  using (this.GetSlowQueryTraceWatch(parametersForTracing))
                  {
                    this.m_rowCount = this.m_sqlCommand.ExecuteNonQuery();
                    break;
                  }
                case ExecuteType.ExecuteScalar:
                  using (this.GetSlowQueryTraceWatch(parametersForTracing))
                  {
                    this.m_scalarResult = this.m_sqlCommand.ExecuteScalar();
                    break;
                  }
                case ExecuteType.Unknown:
                  using (this.GetSlowQueryTraceWatch(parametersForTracing))
                  {
                    try
                    {
                      this.m_sqlDataReader = this.m_sqlCommand.ExecuteReader(behavior);
                      this.m_unknownResult = this.ExecuteUnknown(this.m_sqlDataReader, this.m_unknownParam);
                      break;
                    }
                    catch (Exception ex)
                    {
                      this.CloseReader();
                      throw;
                    }
                  }
              }
              this.m_errorForensics.SqlOperation = string.Empty;
              this.Trace(64028, TraceLevel.Info, "Call was successful, break out of retry loop");
              flag3 = true;
              this.m_executionTrace.Success = true;
            }
            finally
            {
              stopwatch2.Stop();
              VssPerformanceEventSource.Log.SQLStop(uniqueIdentifier, this.m_sqlCommand.CommandText, this.DataSource, this.Database, stopwatch2.ElapsedMilliseconds);
              this.m_errorForensics.LastSqlExecuteEndTime = DateTime.UtcNow;
              int elapsedMilliseconds = (int) stopwatch2.ElapsedMilliseconds;
              this.m_errorForensics.ExecutionTime = elapsedMilliseconds;
              this.m_executionTrace.ExecutionTime += elapsedMilliseconds;
              if (SqlStatisticsContext.CollectingStatistics)
                SqlStatisticsContext.AddSqlExecute(this.m_sqlCommand.CommandText, this.m_errorForensics.LastSqlExecuteEndTime - this.m_errorForensics.LastSqlExecuteBeginTime);
              if (this.RequestContext != null)
                this.RequestContext.ExitCancelableRegion((ICancelable) this);
            }
          }
          catch (RequestCanceledException ex)
          {
            HashSet<Type> trippableExceptions = TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions;
            DatabaseResourceComponent.TagIgnorableExceptionsForCircuitBreaker((Exception) ex, trippableExceptions);
            throw;
          }
          catch (SqlTypeException ex1)
          {
            this.TraceException(64035, (Exception) ex1);
            this.Trace(64041, TraceLevel.Error, this.GetStringParametersForTracing().Value);
            try
            {
              this.TranslateException(ex1);
            }
            catch (Exception ex2)
            {
              ex2.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
              throw;
            }
            ex1.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
            throw;
          }
          catch (Exception ex3)
          {
            this.TraceSqlError(ex3);
            this.Trace(64029, TraceLevel.Warning, "{0}\r\nExecuteType: {1}\r\nSql Command: {2}\r\n\r\n{3}", (object) this.m_errorForensics, (object) executeType, (object) SecretUtility.ScrubSecrets(commandTracer.ToString(), false), (object) ex3);
            try
            {
              if (!this.HandleException(ex3))
                throw;
            }
            catch (Exception ex4)
            {
              HashSet<Type> trippableExceptions = TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions;
              DatabaseResourceComponent.TagIgnorableExceptionsForCircuitBreaker(ex4, trippableExceptions);
              if (ex4 is UnauthorizedWriteException unauthorizedWriteException)
              {
                string str = EnvironmentWrapper.ToReadableStackTrace().ToString();
                if (str.Length > 20000)
                  str = str.Substring(0, 20000);
                this.Trace(1283862202, TraceLevel.Error, "Unauthorized write detected: {0}, Stack: {1}", (object) unauthorizedWriteException.DebugMessage, (object) str);
              }
              throw;
            }
            if (flag4)
            {
              if (!this.ResetTVPs())
              {
                this.Trace(64033, TraceLevel.Error, "Unable to reset all TVPs for retry.");
                throw;
              }
              else
                this.Trace(64034, TraceLevel.Info, "TVPs reset.  Attempt to retry.");
            }
            ++this.m_executionTrace.Retries;
            if (this.LoggingOptions != TeamFoundationDatabaseLoggingOptions.None)
              this.m_logicalReads = this.m_physicalReads = this.m_cpuTime = this.m_elapsedTime = 0;
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSqlExecutionRetries");
            performanceCounter.Increment();
          }
        }
        if (!this.ShouldBindAuditContextInfoParameters())
          return;
        using (this.AcquireExemptionLock())
          this.m_requestContext.HandlePostAuditLog();
      }
      finally
      {
        if (flag1)
        {
          performanceTimer2.End();
          performanceTimer3.Invalidate();
        }
        if (flag2)
          performanceTimer4.End();
        performanceTimer1.End();
        this.m_executionTrace.TotalTime = (int) performanceTimer1.Duration;
        if (this.RequestContext != null)
        {
          if (!string.IsNullOrEmpty(this.m_sqlSprocName))
          {
            if (this.RequestContext.IsTracing(64040, TraceLevel.Info, this.TraceArea, this.m_traceLayer, new string[1]
            {
              this.m_sqlSprocName
            }))
              this.RequestContext.Trace(64040, TraceLevel.Info, this.TraceArea, this.m_traceLayer, new string[1]
              {
                this.m_sqlSprocName
              }, "{0} was called by {1}", (object) this.m_sqlSprocName, (object) new StackTracer().ToString());
          }
          this.RequestContext.RequestTracer.RequestTracerInternal().TraceSql(64039, this.m_executionTrace.DataSource, this.m_executionTrace.Database, this.m_executionTrace.Operation, this.m_executionTrace.Retries, this.m_executionTrace.Success, this.m_executionTrace.TotalTime, this.m_executionTrace.ConnectTime, this.m_executionTrace.ExecutionTime, this.m_executionTrace.WaitTime, this.m_executionTrace.SqlErrorCode, this.m_executionTrace.ErrorMessage);
        }
        this.TraceLeave(64036, "Execute");
      }
    }

    internal override async Task ExecuteCommandAsync(
      ExecuteType executeType,
      CommandBehavior behavior,
      string performanceGroupName)
    {
      TeamFoundationSqlResourceComponent resourceComponent = this;
      resourceComponent.BindPrepareExecutionParameters();
      resourceComponent.m_executionTrace = new TeamFoundationSqlResourceComponent.ExecutionTrace();
      PerformanceTimer timer = PerformanceTimer.StartMeasure(resourceComponent.RequestContext, performanceGroupName);
      bool timeSqlPerformance = string.Equals(performanceGroupName, "SQL", StringComparison.Ordinal);
      PerformanceTimer finalCommandTimer = new PerformanceTimer();
      PerformanceTimer retryTimer = new PerformanceTimer();
      bool timeSqlReadOnly = timeSqlPerformance && resourceComponent.ApplicationIntent == ApplicationIntent.ReadOnly;
      PerformanceTimer readOnlyTimer = new PerformanceTimer();
      if (timeSqlReadOnly)
        readOnlyTimer.Start(resourceComponent.RequestContext, "SQLReadOnly");
      Guid uniqueIdentifier = resourceComponent.RequestContext != null ? resourceComponent.RequestContext.UniqueIdentifier : Guid.Empty;
      resourceComponent.m_totalWaitTime = 0;
      TeamFoundationSqlResourceComponent.CommandTracer commandTracer = new TeamFoundationSqlResourceComponent.CommandTracer(resourceComponent.m_sqlCommand);
      string commandTextForTracing = resourceComponent.CommandTextForTracing;
      timer.AddProperty("Command", (object) commandTextForTracing);
      timer.AddProperty("Area", (object) resourceComponent.TraceArea);
      if (timeSqlReadOnly)
      {
        readOnlyTimer.AddProperty("Command", (object) commandTextForTracing);
        readOnlyTimer.AddProperty("Area", (object) resourceComponent.TraceArea);
      }
      resourceComponent.TraceEnter(64020, string.Format("Execute {0} ds:{1} db:{2}", (object) commandTextForTracing, (object) resourceComponent.Database, (object) resourceComponent.InitialCatalog));
      try
      {
        resourceComponent.VerifyInitialized();
        ++resourceComponent.m_errorForensics.ExecuteCalledCount;
        resourceComponent.m_errorForensics.ExecutionBeginTime = DateTime.UtcNow;
        resourceComponent.m_errorForensics.ExecuteAttempts = 0;
        resourceComponent.m_errorForensics.SqlExecuteCalls = 0;
        resourceComponent.m_errorForensics.LastSqlExecuteBeginTime = new DateTime();
        resourceComponent.m_errorForensics.LastSqlExecuteEndTime = new DateTime();
        resourceComponent.m_executionTrace.Database = resourceComponent.m_errorForensics.Database = resourceComponent.InitialCatalog;
        resourceComponent.m_executionTrace.DataSource = resourceComponent.m_errorForensics.DataSource = resourceComponent.DataSource;
        if (resourceComponent.m_sqlCommand == null)
        {
          resourceComponent.Trace(64021, TraceLevel.Error, "Execute called with null SQL command");
          throw new NoNullAllowedException(FrameworkResources.NullStoredProcException());
        }
        Lazy<string> parameterString = resourceComponent.GetStringParametersForTracing();
        if (resourceComponent.m_sqlCommand.Parameters.Count == 0)
          resourceComponent.Trace(64023, TraceLevel.Info, "no parameters");
        else
          resourceComponent.Trace(64023, TraceLevel.Info, "{0}", (object) parameterString);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalSqlBatches").Increment();
        resourceComponent.CloseReader();
        resourceComponent.m_sqlCommand.CommandText = resourceComponent.CommandText;
        resourceComponent.m_executionTrace.Operation = resourceComponent.CommandTextForTracing;
        if ((resourceComponent.LoggingOptions & TeamFoundationDatabaseLoggingOptions.CommandText) != TeamFoundationDatabaseLoggingOptions.None)
        {
          resourceComponent.Trace(64022, TraceLevel.Info, "Database logging enabled: CommandText");
          resourceComponent.SqlMessages.AppendLine(TeamFoundationSqlResourceComponent.CommandTracer.SqlCommandToString(resourceComponent.m_sqlCommand));
        }
        else if ((resourceComponent.LoggingOptions & TeamFoundationDatabaseLoggingOptions.Statistics) != TeamFoundationDatabaseLoggingOptions.None)
        {
          resourceComponent.Trace(64025, TraceLevel.Info, "Database logging enabled for stored procedure {0}: Statistics", (object) resourceComponent.m_sqlSprocName);
          resourceComponent.SqlMessages.AppendLine(resourceComponent.m_sqlSprocName);
          resourceComponent.SqlMessages.Append('-', 20);
          resourceComponent.SqlMessages.Append(Environment.NewLine);
        }
        bool success = false;
        while (!success)
        {
          ++resourceComponent.m_errorForensics.ExecuteAttempts;
          resourceComponent.m_errorForensics.SqlOperation = string.Empty;
          if (timeSqlPerformance)
          {
            retryTimer.End();
            retryTimer = PerformanceTimer.StartMeasure(resourceComponent.RequestContext, "SQLRetries");
          }
          bool requiresReset = false;
          if (resourceComponent.m_usingMainConfigDbPool)
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ConfigDBCurrentSQLExecutionsPerSec").Increment();
          VssPerformanceCounter performanceCounter;
          if (!string.IsNullOrWhiteSpace(resourceComponent.InitialCatalog))
          {
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSQLExecutionsPerSecInstance", resourceComponent.InitialCatalog);
            performanceCounter.Increment();
          }
          performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSQLComponentCallsPerSecInstance", resourceComponent.NonVersionedComponentClassName);
          performanceCounter.Increment();
          CancellationToken cancellationToken = resourceComponent.RequestContext != null ? resourceComponent.RequestContext.CancellationToken : CancellationToken.None;
          try
          {
            Stopwatch connectionWatch = new Stopwatch();
            resourceComponent.m_errorForensics.SqlOperation = "read SqlConnection.State";
            if (resourceComponent.m_sqlConnection.State != ConnectionState.Open)
            {
              resourceComponent.Trace(64026, TraceLevel.Info, "Opening SQL connection currently in state {0}", (object) resourceComponent.m_sqlConnection.State);
              resourceComponent.m_errorForensics.SqlOperation = "execute SqlConnection.Open()";
              connectionWatch.Start();
              try
              {
                await resourceComponent.m_sqlConnection.OpenAsync(cancellationToken);
                ++resourceComponent.m_errorForensics.OpenedConnectionCount;
              }
              finally
              {
                connectionWatch.Stop();
                resourceComponent.m_executionTrace.ConnectTime += (int) connectionWatch.ElapsedMilliseconds;
                VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlConnectionTime").IncrementMilliseconds(connectionWatch.ElapsedMilliseconds);
                VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AverageSqlConnectionTimeBase").Increment();
              }
            }
            resourceComponent.m_errorForensics.SqlOperation = string.Empty;
            Stopwatch executionWatch = new Stopwatch();
            try
            {
              if (resourceComponent.RequestContext != null)
                resourceComponent.RequestContext.EnterCancelableRegion((ICancelable) resourceComponent);
              requiresReset = true;
              resourceComponent.Trace(64027, TraceLevel.Info, "Call the appropriate m_sqlCommand.ExecuteXXX function for ExecuteType: {0}", (object) executeType);
              resourceComponent.m_errorForensics.SqlOperation = "execute SqlCommand.ExecuteXXX";
              ++resourceComponent.m_errorForensics.SqlExecuteCalls;
              resourceComponent.m_errorForensics.LastSqlExecuteBeginTime = DateTime.UtcNow;
              executionWatch.Start();
              VssPerformanceEventSource.Log.SQLStart(uniqueIdentifier, resourceComponent.m_sqlCommand.CommandText, resourceComponent.Database, resourceComponent.InitialCatalog);
              if (timeSqlPerformance)
              {
                finalCommandTimer.Invalidate();
                finalCommandTimer = PerformanceTimer.StartMeasure(resourceComponent.RequestContext, "FinalSQLCommand");
              }
              resourceComponent.PreExecuteFaultInjection();
              TraceWatch traceWatch;
              switch (executeType)
              {
                case ExecuteType.ExecuteReader:
                  traceWatch = resourceComponent.GetSlowQueryTraceWatch(parameterString);
                  try
                  {
                    SqlDataReader sqlDataReader = await resourceComponent.m_sqlCommand.ExecuteReaderAsync(behavior, cancellationToken);
                    resourceComponent.m_sqlDataReader = sqlDataReader;
                  }
                  finally
                  {
                    traceWatch?.Dispose();
                  }
                  traceWatch = (TraceWatch) null;
                  break;
                case ExecuteType.ExecuteNonQuery:
                  traceWatch = resourceComponent.GetSlowQueryTraceWatch(parameterString);
                  try
                  {
                    int num = await resourceComponent.m_sqlCommand.ExecuteNonQueryAsync(cancellationToken);
                    resourceComponent.m_rowCount = num;
                  }
                  finally
                  {
                    traceWatch?.Dispose();
                  }
                  traceWatch = (TraceWatch) null;
                  break;
                case ExecuteType.ExecuteScalar:
                  traceWatch = resourceComponent.GetSlowQueryTraceWatch(parameterString);
                  try
                  {
                    object obj = await resourceComponent.m_sqlCommand.ExecuteScalarAsync(cancellationToken);
                    resourceComponent.m_scalarResult = obj;
                  }
                  finally
                  {
                    traceWatch?.Dispose();
                  }
                  traceWatch = (TraceWatch) null;
                  break;
                case ExecuteType.Unknown:
                  traceWatch = resourceComponent.GetSlowQueryTraceWatch(parameterString);
                  try
                  {
                    SqlDataReader sqlDataReader = await resourceComponent.m_sqlCommand.ExecuteReaderAsync(behavior, cancellationToken);
                    resourceComponent.m_sqlDataReader = sqlDataReader;
                    resourceComponent.m_unknownResult = resourceComponent.ExecuteUnknown(resourceComponent.m_sqlDataReader, resourceComponent.m_unknownParam);
                  }
                  catch (Exception ex)
                  {
                    resourceComponent.CloseReader();
                    throw;
                  }
                  finally
                  {
                    traceWatch?.Dispose();
                  }
                  traceWatch = (TraceWatch) null;
                  break;
              }
              resourceComponent.m_errorForensics.SqlOperation = string.Empty;
              resourceComponent.Trace(64028, TraceLevel.Info, "Call was successful, break out of retry loop");
              success = true;
              resourceComponent.m_executionTrace.Success = true;
            }
            finally
            {
              executionWatch.Stop();
              VssPerformanceEventSource.Log.SQLStop(uniqueIdentifier, resourceComponent.m_sqlCommand.CommandText, resourceComponent.DataSource, resourceComponent.Database, executionWatch.ElapsedMilliseconds);
              resourceComponent.m_errorForensics.LastSqlExecuteEndTime = DateTime.UtcNow;
              int elapsedMilliseconds = (int) executionWatch.ElapsedMilliseconds;
              resourceComponent.m_errorForensics.ExecutionTime = elapsedMilliseconds;
              resourceComponent.m_executionTrace.ExecutionTime += elapsedMilliseconds;
              if (SqlStatisticsContext.CollectingStatistics)
              {
                TimeSpan time = resourceComponent.m_errorForensics.LastSqlExecuteEndTime - resourceComponent.m_errorForensics.LastSqlExecuteBeginTime;
                SqlStatisticsContext.AddSqlExecute(resourceComponent.m_sqlCommand.CommandText, time);
              }
              if (resourceComponent.RequestContext != null)
                resourceComponent.RequestContext.ExitCancelableRegion((ICancelable) resourceComponent);
            }
            connectionWatch = (Stopwatch) null;
            executionWatch = (Stopwatch) null;
          }
          catch (RequestCanceledException ex)
          {
            HashSet<Type> trippableExceptions = TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions;
            DatabaseResourceComponent.TagIgnorableExceptionsForCircuitBreaker((Exception) ex, trippableExceptions);
            throw;
          }
          catch (SqlTypeException ex1)
          {
            resourceComponent.TraceException(64035, (Exception) ex1);
            resourceComponent.Trace(64041, TraceLevel.Error, resourceComponent.GetStringParametersForTracing().Value);
            try
            {
              resourceComponent.TranslateException(ex1);
            }
            catch (Exception ex2)
            {
              ex2.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
              throw;
            }
            ex1.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
            throw;
          }
          catch (Exception ex3)
          {
            resourceComponent.TraceSqlError(ex3);
            resourceComponent.Trace(64029, TraceLevel.Warning, "{0}\r\nExecuteType: {1}\r\nSql Command: {2}\r\n\r\n{3}", (object) resourceComponent.m_errorForensics, (object) executeType, (object) SecretUtility.ScrubSecrets(commandTracer.ToString(), false), (object) ex3);
            try
            {
              if (!resourceComponent.HandleException(ex3))
                throw;
            }
            catch (Exception ex4)
            {
              HashSet<Type> trippableExceptions = TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions;
              DatabaseResourceComponent.TagIgnorableExceptionsForCircuitBreaker(ex4, trippableExceptions);
              throw;
            }
            if (requiresReset)
            {
              if (!resourceComponent.ResetTVPs())
              {
                resourceComponent.Trace(64033, TraceLevel.Error, "Unable to reset all TVPs for retry.");
                throw;
              }
              else
                resourceComponent.Trace(64034, TraceLevel.Info, "TVPs reset.  Attempt to retry.");
            }
            ++resourceComponent.m_executionTrace.Retries;
            if (resourceComponent.LoggingOptions != TeamFoundationDatabaseLoggingOptions.None)
              resourceComponent.m_logicalReads = resourceComponent.m_physicalReads = resourceComponent.m_cpuTime = resourceComponent.m_elapsedTime = 0;
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CurrentSqlExecutionRetries").Increment();
          }
          cancellationToken = new CancellationToken();
        }
        parameterString = (Lazy<string>) null;
      }
      finally
      {
        if (timeSqlPerformance)
        {
          finalCommandTimer.End();
          retryTimer.Invalidate();
        }
        if (timeSqlReadOnly)
          readOnlyTimer.End();
        timer.End();
        resourceComponent.m_executionTrace.TotalTime = (int) timer.Duration;
        if (resourceComponent.RequestContext != null)
        {
          if (!string.IsNullOrEmpty(resourceComponent.m_sqlSprocName))
          {
            if (resourceComponent.RequestContext.IsTracing(64040, TraceLevel.Info, resourceComponent.TraceArea, resourceComponent.m_traceLayer, new string[1]
            {
              resourceComponent.m_sqlSprocName
            }))
            {
              StackTracer stackTracer = new StackTracer();
              resourceComponent.RequestContext.Trace(64040, TraceLevel.Info, resourceComponent.TraceArea, resourceComponent.m_traceLayer, new string[1]
              {
                resourceComponent.m_sqlSprocName
              }, "{0} was called by {1}", (object) resourceComponent.m_sqlSprocName, (object) stackTracer.ToString());
            }
          }
          resourceComponent.RequestContext.RequestTracer.RequestTracerInternal().TraceSql(64039, resourceComponent.m_executionTrace.DataSource, resourceComponent.m_executionTrace.Database, resourceComponent.m_executionTrace.Operation, resourceComponent.m_executionTrace.Retries, resourceComponent.m_executionTrace.Success, resourceComponent.m_executionTrace.TotalTime, resourceComponent.m_executionTrace.ConnectTime, resourceComponent.m_executionTrace.ExecutionTime, resourceComponent.m_executionTrace.WaitTime, resourceComponent.m_executionTrace.SqlErrorCode, resourceComponent.m_executionTrace.ErrorMessage);
        }
        resourceComponent.TraceLeave(64036, "Execute");
      }
      timer = new PerformanceTimer();
      finalCommandTimer = new PerformanceTimer();
      retryTimer = new PerformanceTimer();
      readOnlyTimer = new PerformanceTimer();
      commandTracer = new TeamFoundationSqlResourceComponent.CommandTracer();
    }

    protected virtual void PreExecuteFaultInjection()
    {
    }

    private void TraceSqlError(Exception exception)
    {
      StringBuilder stringBuilder1 = (StringBuilder) null;
      StringBuilder stringBuilder2 = (StringBuilder) null;
      StringBuilder stringBuilder3 = (StringBuilder) null;
      if (exception is SqlException sqlException)
      {
        foreach (SqlError error in sqlException.Errors)
        {
          if ((error.Number != 0 || error.Class != (byte) 11) && !error.IsInformational())
          {
            SqlExceptionFactory exceptionFactory = (SqlExceptionFactory) null;
            if (this.TranslatedExceptions == null || !this.TranslatedExceptions.TryGetValue(error.Number, out exceptionFactory))
            {
              if (stringBuilder1 == null)
                stringBuilder1 = new StringBuilder(20);
              stringBuilder1.AppendFormat("{0};", (object) error.Number);
              if (stringBuilder2 == null)
                stringBuilder2 = new StringBuilder();
              stringBuilder2.AppendFormat("{0};", (object) error.Procedure);
              if (stringBuilder3 == null)
                stringBuilder3 = new StringBuilder();
              stringBuilder3.AppendFormat("{0};", (object) error.Message);
            }
          }
        }
      }
      if (sqlException == null)
      {
        this.Trace(64019, TraceLevel.Error, "ds:{0} db:{1} AppIntent:{2} Exception:{3} Command:{4}", (object) this.DataSource, (object) this.Database, (object) DatabaseResourceComponent.ToString(this.ApplicationIntent), (object) exception.Message, (object) TeamFoundationSqlResourceComponent.CommandTracer.SqlCommandToString(this.Command));
      }
      else
      {
        if (stringBuilder1 == null && stringBuilder2 == null && stringBuilder3 == null)
          return;
        this.Trace(sqlException.Number == this.m_containerErrorCode ? 64018 : 64024, TraceLevel.Error, "ds:{0} db:{1} AppIntent:{2} Msg:{3} Level:{4} State:{5} LineNumber:{6} Errors:{7} Exception:{8} Command:{9} SProcs:{10} Messages:{11}", (object) this.DataSource, (object) this.Database, (object) DatabaseResourceComponent.ToString(this.ApplicationIntent), (object) sqlException.Number, (object) sqlException.Class, (object) sqlException.State, (object) sqlException.LineNumber, stringBuilder1 == null ? (object) string.Empty : (object) stringBuilder1.ToString(), (object) exception.Message, (object) TeamFoundationSqlResourceComponent.CommandTracer.SqlCommandToString(this.Command), stringBuilder2 == null ? (object) string.Empty : (object) stringBuilder2.ToString(), stringBuilder3 == null ? (object) string.Empty : (object) stringBuilder3.ToString());
      }
    }

    protected virtual int ExecuteNonQuery()
    {
      this.ExecuteNonQuery(false);
      return this.m_rowCount;
    }

    protected virtual object ExecuteNonQuery(bool bindReturnValue)
    {
      this.VerifyInitialized();
      object obj = (object) null;
      if (bindReturnValue)
        this.m_sqlCommand.Parameters.Add("@returnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
      this.Execute(ExecuteType.ExecuteNonQuery, CommandBehavior.Default);
      if (bindReturnValue)
        obj = this.m_sqlCommand.Parameters["@returnValue"].Value;
      return obj;
    }

    protected virtual async Task<int> ExecuteNonQueryAsync()
    {
      object obj = await this.ExecuteNonQueryAsync(false).ConfigureAwait(false);
      return this.m_rowCount;
    }

    protected virtual async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
    {
      int num = await this.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
      return this.m_rowCount;
    }

    protected virtual async Task<object> ExecuteNonQueryAsync(bool bindReturnValue)
    {
      this.VerifyInitialized();
      object returnValue = (object) null;
      if (bindReturnValue)
        this.m_sqlCommand.Parameters.Add("@returnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
      await this.ExecuteAsync(ExecuteType.ExecuteNonQuery, CommandBehavior.Default).ConfigureAwait(false);
      if (bindReturnValue)
        returnValue = this.m_sqlCommand.Parameters["@returnValue"].Value;
      object obj = returnValue;
      returnValue = (object) null;
      return obj;
    }

    protected SqlDataReader ExecuteReader(CommandBehavior behavior)
    {
      this.Execute(ExecuteType.ExecuteReader, behavior);
      return this.m_sqlDataReader;
    }

    protected virtual SqlDataReader ExecuteReader()
    {
      this.Execute(ExecuteType.ExecuteReader, CommandBehavior.Default);
      return this.m_sqlDataReader;
    }

    protected virtual IDataReader ExecuteReaderTestable() => (IDataReader) this.ExecuteReader();

    protected Task<SqlDataReader> ExecuteReaderAsync() => this.ExecuteReaderAsync(CommandBehavior.Default);

    protected async Task<SqlDataReader> ExecuteReaderAsync(CommandBehavior behavior)
    {
      await this.ExecuteAsync(ExecuteType.ExecuteReader, behavior).ConfigureAwait(false);
      return this.m_sqlDataReader;
    }

    protected object ExecuteScalar()
    {
      this.Execute(ExecuteType.ExecuteScalar, CommandBehavior.Default);
      return this.m_scalarResult;
    }

    protected async Task<object> ExecuteScalarAsync()
    {
      await this.ExecuteAsync(ExecuteType.ExecuteScalar, CommandBehavior.Default);
      return this.m_scalarResult;
    }

    protected object ExecuteUnknown(object parameter)
    {
      this.m_unknownParam = parameter;
      this.Execute(ExecuteType.Unknown, CommandBehavior.Default);
      return this.m_unknownResult;
    }

    protected virtual object ExecuteUnknown(SqlDataReader reader, object parameter) => throw new NotImplementedException();

    protected bool HandleException(Exception exception)
    {
      bool flag = true;
      SqlException ex = exception as SqlException;
      TimeSpan sleepTime = TimeSpan.Zero;
      this.TraceEnter(64050, nameof (HandleException));
      try
      {
        if (!this.CanRetryOnException(exception, out sleepTime) && (ex == null || !this.HandleCustomException(ex)))
        {
          this.m_executionTrace.Success = false;
          if (exception is InvalidOperationException)
          {
            this.Trace(64051, TraceLevel.Error, "ds:{0} db:{1} exception:{2}", (object) this.DataSource, (object) this.Database, (object) exception);
            throw new DatabaseConnectionException(exception as InvalidOperationException);
          }
          if (ex != null)
          {
            this.Trace(64052, TraceLevel.Warning, "ds:{0} db:{1} errors:{2} exception:{3}", (object) this.DataSource, (object) this.Database, (object) this.GetSqlErrorNumberAsString(ex), (object) ex);
            this.MapException(ex);
          }
          else
          {
            this.Trace(64054, TraceLevel.Error, "ds:{0} db:{1} exception:{2}", (object) this.DataSource, (object) this.Database, (object) exception);
            flag = false;
          }
        }
        return flag;
      }
      finally
      {
        if (sleepTime > TimeSpan.Zero)
          this.Sleep(sleepTime);
        this.TraceLeave(64055, nameof (HandleException));
      }
    }

    protected virtual bool HandleCustomException(SqlException ex) => false;

    protected List<int> GetSqlErrorNumbers(SqlException ex)
    {
      List<int> sqlErrorNumbers = new List<int>();
      foreach (SqlError error in ex.Errors)
      {
        if (this.m_containerErrorCode != 0 && error.Number == this.m_containerErrorCode)
          sqlErrorNumbers.AddRange((IEnumerable<int>) TeamFoundationServiceException.ExtractInts(error, "error"));
        else if (!error.IsInformational())
          sqlErrorNumbers.Add(error.Number);
      }
      return sqlErrorNumbers;
    }

    protected string GetSqlErrorNumberAsString(SqlException ex)
    {
      List<int> sqlErrorNumbers = this.GetSqlErrorNumbers(ex);
      if (sqlErrorNumbers.Count == 1)
        return string.Format("{0}", (object) sqlErrorNumbers[0]);
      StringBuilder stringBuilder = new StringBuilder(sqlErrorNumbers.Count * 8);
      foreach (int num in sqlErrorNumbers)
        stringBuilder.AppendFormat("{0};", (object) num);
      return stringBuilder.ToString();
    }

    protected bool CanRetryOnException(Exception exception, out TimeSpan sleepTime)
    {
      bool flag1 = false;
      SqlException sqlException = exception as SqlException;
      sleepTime = TimeSpan.Zero;
      this.TraceEnter(64060, nameof (CanRetryOnException));
      this.m_executionTrace.ErrorMessage = exception.Message;
      try
      {
        switch (exception)
        {
          case InvalidOperationException _:
            sleepTime = TeamFoundationSqlResourceComponent.s_loginErrorRetryPause;
            this.m_executionTrace.SqlErrorCode = 4317;
            this.TraceException(64067, exception);
            break;
          case SqlException ex:
            bool flag2 = false;
            using (List<int>.Enumerator enumerator = this.GetSqlErrorNumbers(ex).GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                int current = enumerator.Current;
                if (current == 480002)
                {
                  this.m_executionTrace.SqlErrorCode = current;
                  this.Trace(64067, TraceLevel.Error, "Sql Fault Injection Encountered: {0}", (object) exception);
                  break;
                }
                if (!flag2)
                {
                  this.m_executionTrace.SqlErrorCode = current;
                  if (current == 40501)
                    this.TraceThrottling(sqlException.Message);
                  if (this.CanRetryOnSqlError(current, sqlException.Class, out sleepTime))
                  {
                    flag2 = true;
                    flag1 = flag1 || current == 1205;
                  }
                }
              }
              break;
            }
          default:
            this.Trace(64066, TraceLevel.Error, "Fatal Error: Unknown Exception in CanRetryOnException {0}", (object) exception);
            break;
        }
        if (sleepTime > TimeSpan.Zero)
        {
          if (this.m_retriesRemaining == 0 || this.m_totalWaitTime > 45000 || this.m_executionTrace.ExecutionTime > 3600000 || this.m_errorForensics.ExecutionTime > 300000)
          {
            this.Trace(flag1 ? 64076 : 64064, TraceLevel.Error, string.Format("Exhausted all retries for ds:{0}, db:{1}, AppIntent:{2}, total_wait_time:{3}, max_wait_time:{4}, executionTime:{5}, c_maxExecutionTime:{6}, errorForensics.ExecutionTime:{7} c_maxRetryableExecutionTime:{8}, Exception:\"{9}\"", (object) this.DataSource, (object) this.Database, (object) DatabaseResourceComponent.ToString(this.ApplicationIntent), (object) this.m_totalWaitTime, (object) 45000, (object) this.m_executionTrace.ExecutionTime, (object) 3600000, (object) this.m_errorForensics.ExecutionTime, (object) 300000, (object) exception.ToReadableStackTrace()));
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalFailedRetrySequences").Increment();
            sleepTime = TimeSpan.Zero;
          }
          else
          {
            --this.m_retriesRemaining;
            return true;
          }
        }
        return false;
      }
      finally
      {
        this.TraceLeave(64063, nameof (CanRetryOnException));
      }
    }

    protected bool CanRetryOnSqlError(int errorNumber, byte errorClass, out TimeSpan sleepTime)
    {
      sleepTime = TimeSpan.Zero;
      switch (TeamFoundationSqlResourceComponent.GetRetryReason(errorNumber, (int) errorClass, this.Connection))
      {
        case SqlErrorRetryReason.Deadlock:
          sleepTime = TimeSpan.FromMilliseconds((double) this.DeadlockPause);
          this.Trace(64061, TraceLevel.Warning, "An exception that can be retried has been detected; will retry operation momentarily.  SQL error number: {0}", (object) errorNumber);
          break;
        case SqlErrorRetryReason.SqlAzureError:
          bool flag = true;
          if (errorNumber == 3906)
          {
            if (this.ApplicationIntent == ApplicationIntent.ReadOnly)
              flag = false;
            IVssRequestContext requestContext = this.RequestContext;
            if ((requestContext != null ? (requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.RedirectWritesOnReadOnlyDatabase) ? 1 : 0) : 0) != 0)
              flag = false;
          }
          if (flag)
          {
            if (errorNumber == 0)
              this.m_retriesRemaining = Math.Min(this.m_retriesRemaining, 3);
            string str = TeamFoundationDataTierService.ManipulateDataSource(this.DataSource, DataSourceOptions.RemoveProtocolAndDomain);
            if (errorNumber == 3906 && str.EndsWith(SqlFailoverGroupConstants.FailoverGroupSuffix, StringComparison.OrdinalIgnoreCase))
            {
              if (this.Connection.State != ConnectionState.Closed)
                this.Connection.Close();
              lock (TeamFoundationSqlResourceComponent.s_connectionPoolLock)
              {
                if (TeamFoundationSqlResourceComponent.s_clearConnectionPoolStopWatch == null || TeamFoundationSqlResourceComponent.s_clearConnectionPoolStopWatch.Elapsed > TimeSpan.FromSeconds(10.0))
                {
                  this.Trace(64073, TraceLevel.Info, "Clearing Sql connection pool for ds: " + this.DataSource + " db: " + this.DataSource + " " + string.Format("for SQL error number: {0} while failover group is enabled", (object) errorNumber));
                  SqlConnection.ClearPool(this.Connection);
                  TeamFoundationSqlResourceComponent.s_clearConnectionPoolStopWatch = Stopwatch.StartNew();
                }
                else
                  this.Trace(64074, TraceLevel.Info, string.Format("Sql connection pool last cleared {0} seconds ago, skip clearing the pool", (object) TeamFoundationSqlResourceComponent.s_clearConnectionPoolStopWatch.Elapsed.Seconds));
              }
            }
            sleepTime = TeamFoundationSqlResourceComponent.s_azureRetryDelay;
            this.Trace(64065, TraceLevel.Error, "An exception that can be retried has been detected; will retry operation momentarily. waiting: {0}ms  SQL error number: {1} ds:{2} db:{3}", (object) sleepTime, (object) errorNumber, (object) this.DataSource, (object) this.Database);
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalThrottlingEvents").Increment();
            break;
          }
          break;
        case SqlErrorRetryReason.SqlConnectionError:
          if (this.Connection.State != ConnectionState.Closed)
            this.Connection.Close();
          switch (errorNumber)
          {
            case 10060:
              this.m_retriesRemaining = Math.Min(this.m_retriesRemaining, 3);
              break;
            case 18456:
              this.m_retriesRemaining = Math.Min(this.m_retriesRemaining, 3);
              break;
          }
          if (this.m_sqlTransaction == null)
          {
            sleepTime = TeamFoundationSqlResourceComponent.s_loginErrorRetryPause;
            this.Trace(64062, TraceLevel.Warning, "An exception that can be retried has been detected; will retry operation momentarily.  SQL error number: {0}", (object) errorNumber);
            break;
          }
          break;
      }
      return sleepTime > TimeSpan.Zero;
    }

    public static SqlErrorRetryReason GetRetryReason(
      int errorNumber,
      int errorClass = 0,
      SqlConnection connection = null)
    {
      if (errorNumber == 1205 || errorNumber == 8628 || errorNumber == 8645 || errorNumber == 2801)
        return SqlErrorRetryReason.Deadlock;
      if (errorNumber == 988 || errorNumber == 40174 || errorNumber == 42019 || errorNumber == 49918 || errorNumber == 10928 || errorNumber == 10929 || errorNumber == 18401 || errorNumber == 40197 || errorNumber == 40501 || errorNumber == 40613 || errorNumber == 40540 || errorNumber == 3906 || errorNumber == 0 && errorClass != 11)
        return SqlErrorRetryReason.SqlAzureError;
      return errorNumber == 53 || errorNumber == 121 || errorNumber == 233 || errorNumber == 10054 || errorNumber == 10053 || errorNumber == 18456 || errorNumber == -2 && (connection == null || connection.State != ConnectionState.Open) || errorNumber == 64 || errorNumber == 10060 || errorNumber == 983 || errorNumber == 258 || errorNumber == 976 || errorNumber == 1225 && errorClass == 20 || errorNumber == 10050 && errorClass == 20 ? SqlErrorRetryReason.SqlConnectionError : SqlErrorRetryReason.None;
    }

    protected virtual void MapException(SqlException ex) => this.MapException(ex, QueryExecutionState.Executing);

    protected virtual void MapException(SqlException ex, QueryExecutionState queryState)
    {
      this.TraceEnter(64070, nameof (MapException));
      try
      {
        foreach (int sqlErrorNumber in this.GetSqlErrorNumbers(ex))
        {
          if (sqlErrorNumber == 1205)
          {
            if (queryState == QueryExecutionState.ReturningResults)
              throw new DBResultDeadlockException();
            throw new DBExecutingDeadlockException();
          }
        }
        this.Trace(64072, TraceLevel.Warning, "Fatal Error because of exception for ds:{0} db:{1} - Exception {2}", (object) this.DataSource, (object) this.Database, (object) ex);
        this.TranslateException(ex);
        this.ReportException((Exception) ex);
        throw ex;
      }
      finally
      {
        this.TraceLeave(64071, nameof (MapException));
      }
    }

    protected void VerifyNotSqlAzure()
    {
      if (this.IsSqlAzure)
        throw new NotSupportedException(FrameworkResources.NotSupportedOnSqlAzure());
    }

    protected void VerifySqlAzure()
    {
      if (!this.IsSqlAzure)
        throw new NotSupportedException(FrameworkResources.OnlySupportedOnSqlAzure());
    }

    protected void VerifyInMasterDbOnAzure()
    {
      if (this.IsSqlAzure && !string.Equals(this.InitialCatalog, TeamFoundationSqlResourceComponent.Master, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(this.InitialCatalog))
        throw new InvalidOperationException(FrameworkResources.MethodCanOnlyBeExecutedOnMaster());
    }

    protected void VerifyInMaster()
    {
      if (!string.Equals(this.InitialCatalog, TeamFoundationSqlResourceComponent.Master, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(this.InitialCatalog))
        throw new InvalidOperationException(FrameworkResources.MethodCanOnlyBeExecutedOnMaster());
    }

    protected void VerifyNotInMaster()
    {
      if (string.Equals(this.InitialCatalog, TeamFoundationSqlResourceComponent.Master, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(this.InitialCatalog))
        throw new InvalidOperationException(FrameworkResources.MethodCanNotBeExecutedOnMaster());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected virtual void VerifyInitialized()
    {
      if (!this.IsInitialized)
        throw new InvalidOperationException(FrameworkResources.ObjectIsNotInitialized());
    }

    protected virtual void TranslateException(SqlTypeException typeException)
    {
    }

    private void TraceThrottling(string message)
    {
      Exception exception;
      int reasonCode;
      if (TeamFoundationSqlResourceComponent.TryParseAzureErrorCode(message, out exception, out reasonCode))
      {
        this.Trace(64068, TraceLevel.Error, new ReasonCode(reasonCode).ToString());
      }
      else
      {
        string format = "Error parsing Azure throttling message. (Did the message format change?) Message: {0}.";
        if (exception == null)
          this.Trace(64069, TraceLevel.Error, format, (object) message);
        else
          this.Trace(64069, TraceLevel.Error, format + " Parsing exception: {1}", (object) message, (object) exception);
      }
    }

    internal static bool TryParseAzureErrorCode(
      string message,
      out Exception exception,
      out int reasonCode)
    {
      bool azureErrorCode = false;
      reasonCode = -1;
      exception = (Exception) null;
      try
      {
        if (int.TryParse(Regex.Match(message, "Code: (?<errorcode>[\\d:]+)").Groups["errorcode"].Value, out reasonCode))
          azureErrorCode = true;
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      return azureErrorCode;
    }

    private int CalculateTimeout(SqlCommandTimeouts timeoutConfig)
    {
      int val1 = timeoutConfig.Timeout;
      if (val1 > InternalDatabaseProperties.DefaultDatabaseRequestTimeout && this.m_commandTimeout > InternalDatabaseProperties.DefaultDatabaseRequestTimeout)
        val1 = Math.Max(val1, this.m_commandTimeout);
      if (this.m_isAnonymousOrPublicRequest && (val1 == 0 || val1 > timeoutConfig.AnonymousTimeout))
        val1 = timeoutConfig.AnonymousTimeout;
      return val1;
    }

    private void TranslateException(SqlException sqlException)
    {
      this.Trace(64080, TraceLevel.Info, "Attempting to translate {0} SQL errors", (object) sqlException.Errors.Count);
      foreach (SqlError error in sqlException.Errors)
      {
        if (!error.IsInformational())
        {
          if (error.Number == 0 && error.Class == (byte) 11)
          {
            this.m_executionTrace.Success = false;
            this.Trace(64082, TraceLevel.Info, sqlException.Message);
            if (this.RequestContext != null)
              this.RequestContext.RequestContextInternal().CheckCanceled();
            throw new DatabaseOperationCanceledException(sqlException);
          }
          this.TranslateException(error.Number, sqlException, error);
          if (string.CompareOrdinal(error.Procedure, "GenClientContext") == 0)
          {
            this.m_executionTrace.Success = false;
            this.TraceException(64081, (Exception) sqlException);
            throw new DatabaseConnectionException(sqlException);
          }
          if (this.m_containerErrorCode != 0 && error.Number == this.m_containerErrorCode)
          {
            this.m_executionTrace.Success = true;
            foreach (int errorNumber in TeamFoundationServiceException.ExtractInts(error, "error"))
              this.TranslateException(errorNumber, sqlException, error);
          }
        }
      }
    }

    public static Exception TranslateSqlException(SqlException sqlException)
    {
      foreach (SqlError error in sqlException.Errors)
      {
        SqlExceptionFactory exceptionFactory;
        if (TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.TryGetValue(error.Number, out exceptionFactory))
        {
          Exception exception = exceptionFactory.Create((IVssRequestContext) null, error.Number, sqlException, error);
          if (exception != null)
            return exception;
        }
      }
      return (Exception) sqlException;
    }

    protected virtual IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) null;

    private bool IsOldStyleTVP(object obj)
    {
      if (obj == null)
        return false;
      Type type = obj.GetType();
      while (type.BaseType != (Type) null && type.BaseType != typeof (object))
        type = type.BaseType;
      return type != (Type) null && type.GetInterface(TeamFoundationSqlResourceComponent.s_oldTvpInterface.Name) != (Type) null;
    }

    private bool ResetTVPs()
    {
      this.Trace(64031, TraceLevel.Info, "Reset TVPs before attempting retry");
      foreach (SqlParameter parameter in (DbParameterCollection) this.m_sqlCommand.Parameters)
      {
        if (parameter.SqlDbType == SqlDbType.Structured && this.IsOldStyleTVP(parameter.Value))
        {
          if (parameter.Value is IEnumerator<SqlDataRecord> enumerator)
          {
            try
            {
              enumerator.Reset();
            }
            catch (RequestCanceledException ex)
            {
              throw;
            }
            catch (Exception ex)
            {
              this.Trace(64032, TraceLevel.Error, "Reset TVPs failed at sproc '{0}'  - parameter '{1}'  Exception '{2}'", (object) this.m_sqlCommand.CommandText, (object) parameter.ParameterName, (object) ex);
              return false;
            }
          }
        }
      }
      return true;
    }

    private void TranslateException(int errorNumber, SqlException sqlException, SqlError sqlError)
    {
      this.TraceEnter(64090, nameof (TranslateException));
      SqlExceptionFactory exceptionFactory = (SqlExceptionFactory) null;
      if (errorNumber == 18054 && sqlException != null)
      {
        this.Trace(64091, TraceLevel.Info, "Attempt to parse error code out of message: {0}", (object) sqlException.Message);
        string message = sqlException.Message;
        if (message != null && message.StartsWith("Error ", StringComparison.OrdinalIgnoreCase))
        {
          int startIndex = 6;
          int index = startIndex;
          int length = message.Length;
          while (index < length && char.IsDigit(message[index]))
            ++index;
          int result = 0;
          if (int.TryParse(message.Substring(startIndex, index - startIndex), out result))
            errorNumber = result;
        }
      }
      IDictionary<int, SqlExceptionFactory> translatedExceptions = this.TranslatedExceptions;
      if (translatedExceptions != null)
      {
        this.Trace(64092, TraceLevel.Verbose, "Derived component has {0} mapped exceptions", (object) translatedExceptions.Count);
        translatedExceptions.TryGetValue(errorNumber, out exceptionFactory);
      }
      if (exceptionFactory == null)
      {
        this.Trace(64093, TraceLevel.Verbose, "Derived component did not map exception, attempt lookup in base mapping");
        TeamFoundationSqlResourceComponent.s_sqlExceptionFactories.TryGetValue(errorNumber, out exceptionFactory);
      }
      if (exceptionFactory != null)
      {
        this.Trace(64094, TraceLevel.Verbose, "Found an exception factory: error number: {0}, type: {1}", (object) errorNumber, (object) exceptionFactory.GetType().FullName);
        Exception exception = exceptionFactory.Create(this.RequestContext, errorNumber, sqlException, sqlError);
        if (exception != null)
        {
          this.Trace(64095, TraceLevel.Warning, "Translated SQL error {0} to type {1}", (object) errorNumber, (object) exception.GetType().FullName);
          Type type = exception.GetType();
          bool flag1 = type == typeof (DatabaseConfigurationException);
          bool flag2 = TeamFoundationSqlResourceComponent.s_circuitBreakerTrippableExceptions.Contains(type) && !flag1;
          if (flag2 | flag1)
            this.Trace(flag2 ? 64097 : 64096, TraceLevel.Error, "ds:{0} db:{1} AppIntent:{2} Error:{3} Level:{4} State:{5}", (object) this.DataSource, (object) this.Database, (object) DatabaseResourceComponent.ToString(this.ApplicationIntent), (object) errorNumber, (object) sqlException.Class, (object) sqlException.State);
          throw exception;
        }
      }
      this.TraceLeave(64098, nameof (TranslateException));
    }

    private void ReportException(Exception exception)
    {
      this.TraceException(64100, exception);
      if (this.RequestContext == null || this.RequestContext.ServiceHost == null || !this.RequestContext.ServiceHost.SendWatsonReports)
        return;
      string[] additionalInfo = (string[]) null;
      if (this.m_sqlCommand != null && this.m_sqlCommand.CommandText != null && this.m_sqlCommand.CommandText.IndexOf("Data Source=", StringComparison.OrdinalIgnoreCase) < 0)
        additionalInfo = new string[1]
        {
          this.m_sqlCommand.CommandText
        };
      string watsonReportingName = this.GetType().FullName;
      if (!string.IsNullOrEmpty(this.DataspaceCategory))
        watsonReportingName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}({1})", (object) watsonReportingName, (object) this.DataspaceCategory);
      this.RequestContext.ServiceHost.ReportException(watsonReportingName, "Database", exception, additionalInfo);
    }

    private void Sleep(TimeSpan sleepTime)
    {
      if (this.OverrideRetryTimeouts >= 0)
      {
        this.Trace(64056, TraceLevel.Info, "Overriding the sleep timeout of {0} Milliseconds since the OverrideRetryTimeouts property is set to true, only sleeping {1} Milliseconds.", (object) sleepTime, (object) this.OverrideRetryTimeouts);
        this.Sleep(this.OverrideRetryTimeouts);
      }
      else
        this.Sleep((int) sleepTime.TotalMilliseconds);
    }

    protected virtual void Sleep(int milliseconds)
    {
      this.m_totalWaitTime += milliseconds;
      this.m_errorForensics.TotalWaitTime = this.m_errorForensics.TotalWaitTime.Add(new TimeSpan(0, 0, 0, 0, milliseconds));
      this.m_executionTrace.WaitTime += milliseconds;
      Thread.Sleep(milliseconds);
    }

    protected IDisposable AcquireExemptionLock() => this.m_sqlResourceLockName == null ? (IDisposable) null : this.RequestContext.AcquireExemptionLock();

    private void NewCommand(int lengthEstimate, int commandTimeout)
    {
      this.CloseReader();
      this.CloseCommand();
      this.m_sqlCommand = this.m_sqlConnection.CreateCommand();
      this.m_sqlCommand.CommandTimeout = commandTimeout;
      ++this.m_errorForensics.SqlCommandsCreated;
      this.m_sqlCommandText = new StringBuilder(lengthEstimate);
      this.m_sqlSprocName = (string) null;
      this.m_numParameters = 0;
      this.m_lastStatementIndex = -1;
    }

    private void CloseReader()
    {
      if (this.m_sqlDataReader == null)
        return;
      if (this.LoggingOptions != TeamFoundationDatabaseLoggingOptions.None)
      {
        if (!this.m_sqlDataReader.IsClosed)
        {
          try
          {
            do
              ;
            while (this.m_sqlDataReader.NextResult());
          }
          catch (SqlException ex)
          {
            this.TraceException(64008, (Exception) ex, TraceLevel.Warning);
          }
          catch (Exception ex)
          {
            this.TraceException(64009, ex);
          }
        }
        if (this.RequestContext != null)
        {
          TimeSpan time = this.m_errorForensics.LastSqlExecuteEndTime - this.m_errorForensics.ExecutionBeginTime;
          MethodTime methodTime = new MethodTime(this.m_sqlSprocName ?? string.Empty, time, this.m_logicalReads, this.m_physicalReads, this.m_cpuTime / 2, this.m_elapsedTime);
          this.RequestContext.RequestLogger.RequestLoggerInternal().LogSqlCall(methodTime);
        }
        this.m_logicalReads = this.m_physicalReads = this.m_cpuTime = this.m_elapsedTime = 0;
      }
      this.m_sqlDataReader.Close();
      this.m_sqlDataReader = (SqlDataReader) null;
    }

    private void CloseCommand()
    {
      if (this.m_sqlCommand == null)
        return;
      this.m_sqlCommand.Parameters.Clear();
      this.m_sqlCommand.Dispose();
      this.m_sqlCommand = (SqlCommand) null;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    protected void ReleaseVerificationLock()
    {
      this.TraceEnter(64110, nameof (ReleaseVerificationLock));
      if (this.m_versionVerificationLockAcquired)
      {
        if (this.m_sqlConnection.State == ConnectionState.Open)
        {
          try
          {
            this.Trace(64111, TraceLevel.Verbose, "Calling sp_releaseapplock");
            this.PrepareStoredProcedure("sp_releaseapplock", false);
            this.BindString("@Resource", FrameworkServerConstants.VerifyVersionLock, FrameworkServerConstants.VerifyVersionLock.Length, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
            this.BindString("@LockOwner", "session", "session".Length, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
            this.BindPrepareExecutionParameters();
            this.m_sqlCommand.CommandText = this.CommandText;
            this.m_sqlCommand.ExecuteNonQuery();
            this.m_versionVerificationLockAcquired = false;
          }
          catch (Exception ex)
          {
            this.TraceException(64112, ex);
          }
        }
      }
      this.TraceLeave(64110, nameof (ReleaseVerificationLock));
    }

    protected void LogInfoMessage(SqlInfoMessageEventArgs e, ITFLogger logger, bool addSqlInfo = false)
    {
      foreach (SqlError error in e.Errors)
      {
        if (!error.IsStatistical())
        {
          if (addSqlInfo)
            this.Logger.Info("SQL Info: {0}", (object) error.Message);
          else
            this.Logger.Info(error.Message);
        }
      }
    }

    internal static void ParseMetrics(
      string message,
      ref int logicalReads,
      ref int physicalReads,
      ref int cpuTime,
      ref int elapsedTime)
    {
      int offset = 0;
      do
      {
        KeyValuePair<int, int> keywords = StateMachine.FindKeywords(TeamFoundationSqlResourceComponent.s_keywords, message, offset);
        offset = keywords.Value;
        if (offset > 0)
        {
          int index = offset;
          int num = 0;
          while (char.IsWhiteSpace(message[index]) && index < message.Length)
            ++index;
          for (; char.IsDigit(message[index]) && index < message.Length; ++index)
            num = num * 10 + ((int) message[index] - 48);
          switch (keywords.Key)
          {
            case 0:
              cpuTime += num;
              break;
            case 1:
              logicalReads += num;
              break;
            case 2:
              physicalReads += num;
              break;
            case 3:
              elapsedTime += num;
              break;
            case 4:
              physicalReads += num;
              break;
          }
        }
      }
      while (offset > 0);
    }

    private void MessageHandler(object sender, SqlInfoMessageEventArgs args)
    {
      this.SqlMessages.AppendLine(args.Message);
      if ((this.LoggingOptions & TeamFoundationDatabaseLoggingOptions.Statistics) != TeamFoundationDatabaseLoggingOptions.None)
      {
        try
        {
          TeamFoundationSqlResourceComponent.ParseMetrics(args.Message, ref this.m_logicalReads, ref this.m_physicalReads, ref this.m_cpuTime, ref this.m_elapsedTime);
        }
        catch (Exception ex)
        {
          this.TraceException(64099, ex);
        }
      }
      if (this.m_zeroDataspaceIdErrorLogged || !args.Message.StartsWith(TeamFoundationSqlResourceComponent.s_zeroDataspaceIdErrorPrefix, StringComparison.OrdinalIgnoreCase))
        return;
      this.m_zeroDataspaceIdErrorLogged = true;
      this.Trace(56006, TraceLevel.Error, string.Format("{0}; Stack trace: {1}", (object) args.Message, (object) EnvironmentWrapper.ToReadableStackTrace()));
    }

    private StringBuilder SqlMessages
    {
      get
      {
        if (this.m_sqlMessages == null)
          this.m_sqlMessages = new StringBuilder(1024);
        return this.m_sqlMessages;
      }
    }

    internal static object Scrub(object parameter)
    {
      switch (parameter)
      {
        case string message:
          return (object) SecretUtility.ScrubSecrets(message, false);
        case SqlString sqlString:
          return sqlString.IsNull || SecretUtility.ScrubSecrets(sqlString.Value, false) == sqlString.Value ? (object) sqlString : (object) new SqlString(SecretUtility.ScrubSecrets(sqlString.Value, false), sqlString.LCID, sqlString.SqlCompareOptions);
        default:
          return parameter;
      }
    }

    private Lazy<string> GetStringParametersForTracing() => this.m_sqlCommand.Parameters.Count == 0 ? TeamFoundationSqlResourceComponent.EmptyLazyString : new Lazy<string>((Func<string>) (() => string.Join(" ", this.EnumerateParameters(this.m_sqlCommand).Select<SqlParameter, string>((System.Func<SqlParameter, string>) (par => string.Format("{0}:{1}", (object) par.ParameterName, this.FormatParameterValue(par)))))));

    private IEnumerable<SqlParameter> EnumerateParameters(SqlCommand command) => Enumerable.Range(0, command.Parameters.Count).Select<int, SqlParameter>((System.Func<int, SqlParameter>) (idx => command.Parameters[idx]));

    private object FormatParameterValue(SqlParameter parameter)
    {
      if (parameter.SqlDbType != SqlDbType.Structured || !(parameter.Value is IEnumerable<SqlDataRecord>) || this.IsOldStyleTVP(parameter.Value))
        return TeamFoundationSqlResourceComponent.Scrub(parameter.Value);
      if (!(parameter.Value is IEnumerable<SqlDataRecord> source))
        return (object) "SqlDataRecord[0]";
      StringBuilder stringBuilder = new StringBuilder();
      int num1 = 0;
      int num2 = source.Count<SqlDataRecord>();
      bool minimize = num2 > 1;
      using (IEnumerator<SqlDataRecord> enumerator = source.GetEnumerator())
      {
        while (stringBuilder.Length < 5000)
        {
          if (enumerator.MoveNext())
          {
            if (num1 > 0)
              stringBuilder.Append(",");
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            stringBuilder.Append(enumerator.Current.SerializeClrAndSqlFieldValues(TeamFoundationSqlResourceComponent.\u003C\u003EO.\u003C0\u003E__Scrub ?? (TeamFoundationSqlResourceComponent.\u003C\u003EO.\u003C0\u003E__Scrub = new System.Func<object, object>(TeamFoundationSqlResourceComponent.Scrub)), minimize));
            ++num1;
          }
          else
            break;
        }
      }
      if (num1 < num2)
      {
        if (num1 > 0)
          stringBuilder.Append(",");
        stringBuilder.Append(string.Format("\"(truncated after exceding record serialization limit = {0}\"", (object) 5000));
      }
      return (object) string.Format("SqlDataRecord[{0}]=[{1}]", (object) num2, (object) stringBuilder);
    }

    private void TrimExecFromSqlCommand()
    {
      string str = "EXEC ";
      if (this.m_sqlCommandText.Length < str.Length || !this.m_sqlCommandText.ToString().EndsWith(str))
        return;
      this.m_sqlCommandText.Remove(this.m_sqlCommandText.Length - str.Length, str.Length);
    }

    private bool IsInitialized => this.m_sqlConnection != null;

    protected override IVssRequestContext RequestContext => this.m_requestContext;

    internal TraceWatch GetSlowQueryTraceWatch(Lazy<string> parameterString) => new TraceWatch(this.RequestContext, 64038, TraceLevel.Error, this.ExecutionTimeThreshold, this.TraceArea, this.m_traceLayer, "ExecuteNonQuery {0} ds:{1} db:{2} AppIntent:{3} {4}", new object[5]
    {
      (object) this.CommandTextForTracing,
      (object) this.DataSource,
      (object) this.Database,
      (object) DatabaseResourceComponent.ToString(this.ApplicationIntent),
      (object) parameterString
    });

    internal int OverrideRetryTimeouts { get; set; }

    protected bool DataspaceRlsEnabled { get; set; }

    private struct ExecutionTrace
    {
      public string DataSource;
      public string Database;
      public string Operation;
      public short Retries;
      public bool Success;
      public int TotalTime;
      public int WaitTime;
      public int SqlErrorCode;
      public int ConnectTime;
      public int ExecutionTime;
      public string ErrorMessage;
    }

    private struct ErrorForensics
    {
      public string SqlOperation;
      public int SqlCommandsCreated;
      public int ExecuteCalledCount;
      public DateTime ExecutionBeginTime;
      public int OpenedConnectionCount;
      public int ClosedConnectionCount;
      public int ExecuteAttempts;
      public int ExecutionTime;
      public TimeSpan TotalWaitTime;
      public int SqlExecuteCalls;
      public DateTime LastSqlExecuteBeginTime;
      public DateTime LastSqlExecuteEndTime;
      public string DataSource;
      public string Database;

      public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\n{0}\r\n\r\n# of Sql Commands created:     {1}\r\nExecute called:                {2} times\r\nExecute last called at:        {3}\r\n\r\nOpened SqlConnection: {4} times\r\nClosed SqlConnection: {5} times\r\n\r\nExecution Attempt #{6}\r\nLast Execution Time #{7}ms\r\nTotal Wait Time #{8}\r\nSql Execute called:          {9} times\r\nLast Sql Execute Begin Time: {10}\r\nLast Sql Execute End Time:   {11}\r\nds:{12}\r\ndb:{13}\r\n", (object) this.SqlOperation, (object) this.SqlCommandsCreated, (object) this.ExecuteCalledCount, (object) this.ExecutionBeginTime, (object) this.OpenedConnectionCount, (object) this.ClosedConnectionCount, (object) this.ExecuteAttempts, (object) this.ExecutionTime, (object) this.TotalWaitTime, (object) this.SqlExecuteCalls, (object) this.LastSqlExecuteBeginTime, (object) this.LastSqlExecuteEndTime, (object) this.DataSource, (object) this.Database);
    }

    private struct CommandTracer
    {
      private readonly SqlCommand m_sqlCommand;

      public CommandTracer(SqlCommand sqlCommand) => this.m_sqlCommand = sqlCommand;

      public static string SqlCommandToString(SqlCommand sqlCommand)
      {
        StringBuilder stringBuilder = new StringBuilder();
        if (sqlCommand.CommandType == CommandType.StoredProcedure)
        {
          stringBuilder.Append("EXEC " + sqlCommand.CommandText + " ");
          foreach (SqlParameter parameter in (DbParameterCollection) sqlCommand.Parameters)
            stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1},", (object) parameter.ParameterName, parameter.Value));
          stringBuilder.Remove(stringBuilder.Length - 1, 1);
          stringBuilder.Append(Environment.NewLine);
        }
        else
          stringBuilder.AppendLine(sqlCommand.CommandText);
        stringBuilder.Append('-', 20);
        return stringBuilder.ToString();
      }

      public override string ToString() => TeamFoundationSqlResourceComponent.CommandTracer.SqlCommandToString(this.m_sqlCommand);
    }

    internal class SqlFaultInject
    {
      private static T Construct<T>(params object[] p) => (T) ((IEnumerable<ConstructorInfo>) typeof (T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)).FirstOrDefault<ConstructorInfo>((System.Func<ConstructorInfo, bool>) (c => c.GetParameters().Length == p.Length)).Invoke(p);

      internal static SqlException GetSqlException(int errorNumber) => TeamFoundationSqlResourceComponent.SqlFaultInject.GetSqlException(errorNumber, (byte) 0, (byte) 20, "MOCK", "Mock Message", string.Empty, 0);

      internal static SqlException GetSqlException(
        int infoNumber,
        byte errorState,
        byte errorClass,
        string serverName,
        string errorMessage,
        string procedure,
        int lineNumber)
      {
        SqlErrorCollection sqlErrorCollection = TeamFoundationSqlResourceComponent.SqlFaultInject.Construct<SqlErrorCollection>();
        SqlError sqlError = TeamFoundationSqlResourceComponent.SqlFaultInject.Construct<SqlError>((object) infoNumber, (object) errorState, (object) errorClass, (object) serverName, (object) errorMessage, (object) procedure, (object) lineNumber);
        typeof (SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object) sqlErrorCollection, new object[1]
        {
          (object) sqlError
        });
        foreach (MethodInfo method in typeof (SqlException).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
        {
          if (string.CompareOrdinal(method.Name, "CreateException") == 0 && method.GetParameters().Length == 2)
            return method.Invoke((object) null, new object[2]
            {
              (object) sqlErrorCollection,
              (object) "11.0.0"
            }) as SqlException;
        }
        throw new MissingMethodException();
      }
    }
  }
}
