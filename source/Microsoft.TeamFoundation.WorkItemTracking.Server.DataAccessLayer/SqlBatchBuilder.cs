// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.SqlBatchBuilder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class SqlBatchBuilder : IDisposable
  {
    public SqlBatchBuilder.SqlBatchAction PostExecutionActions;
    public int? ExecutionTimeout;
    private IVssRequestContext m_requestContext;
    private StringBuilder m_sqlBatch;
    private List<SqlParameter> m_parameterList;
    private int m_dataTablesCount;
    private Payload m_resultPayload;
    private List<KeyValuePair<string, SqlParameter>> m_outputParamList;
    private bool m_handleCustomException;
    private PayloadConverter m_payloadConverter;
    private int? m_clientVersion;
    private int m_dtVersion;
    private SqlAccess m_component;
    private DatabaseConnectionType m_connectionType;
    private IDictionary<int, IPayloadTableSchema> m_tableSchemas;
    private bool m_globalConvertersAdded;
    private readonly StackTrace m_constructorStackTrace;
    private const string c_sqlRuntimeTraceAlways = "/Service/WorkItemTracking/DAL/SqlRuntimeTraceAlwaysSeconds";
    private int m_tracingSecondsLimit;
    internal const string s_Area = "WorkItemTracking";
    internal static readonly CommandPropertiesSetter DefaultCommandPropertiesForSQLBatch = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(50).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.MaxValue).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(10000).WithMetricsRollingStatisticalWindowBuckets(10).WithExecutionMaxConcurrentRequests(int.MaxValue);

    protected SqlBatchBuilder(
      IVssRequestContext requestContext,
      bool handleCustomException,
      DatabaseConnectionType connectionType = DatabaseConnectionType.Default)
    {
      this.m_requestContext = requestContext;
      this.m_connectionType = connectionType;
      this.m_sqlBatch = new StringBuilder();
      this.m_parameterList = new List<SqlParameter>();
      this.m_outputParamList = new List<KeyValuePair<string, SqlParameter>>();
      this.m_handleCustomException = handleCustomException;
      this.m_tracingSecondsLimit = this.m_requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/DAL/SqlRuntimeTraceAlwaysSeconds", true, 300);
      this.AppendSql("set nocount on");
      this.AppendSql(Environment.NewLine);
      if (!TeamFoundationTracingService.IsRawTracingEnabled(1804575006, TraceLevel.Info, "Policy", "PolicyEvaluationTransaction", (string[]) null))
        return;
      this.m_constructorStackTrace = new StackTrace(1);
    }

    internal void AddPayloadTableConverter(int index, PayloadTableConverter tableConverter)
    {
      if (this.m_payloadConverter == null)
        this.m_payloadConverter = new PayloadConverter();
      this.m_payloadConverter[index + this.m_dataTablesCount] = tableConverter;
    }

    public int ParameterCount => this.m_parameterList.Count;

    internal SqlAccess Component
    {
      get
      {
        if (this.m_component == null)
        {
          this.m_component = (SqlAccess) DalSqlResourceComponent.CreateComponent(this.m_requestContext, this.m_connectionType);
          this.m_component.NeedHandleCustomException = this.m_handleCustomException;
          if (this.m_requestContext.ServiceHost != null && !this.m_requestContext.ServiceHost.IsProduction)
            this.m_component.SqlBatchExemptionLock = this.m_requestContext.AcquireExemptionLock();
          if (this.ExecutionTimeout.HasValue && this.ExecutionTimeout.Value > 0)
            this.m_component.CommandTimeoutOverride = new int?(this.ExecutionTimeout.Value);
        }
        return this.m_component;
      }
    }

    public void Dispose()
    {
      this.DisposeComponent();
      GC.SuppressFinalize((object) this);
    }

    private void DisposeComponent()
    {
      if (this.m_component == null)
        return;
      this.m_component.Dispose();
      this.m_component = (SqlAccess) null;
    }

    ~SqlBatchBuilder()
    {
      if (this.m_component == null)
        return;
      if (this.m_constructorStackTrace != null)
        TeamFoundationTracingService.TraceRaw(900695, TraceLevel.Error, "DataAccessLayer", nameof (SqlBatchBuilder), "SqlBatchBuilder finalizer without dispose! - call stack: {0}", (object) this.m_constructorStackTrace);
      else
        TeamFoundationTracingService.TraceRaw(900695, TraceLevel.Error, "DataAccessLayer", nameof (SqlBatchBuilder), "SqlBatchBuilder finalizer without dispose!");
    }

    public void ReInitializeBatch()
    {
      this.m_sqlBatch = new StringBuilder();
      this.m_parameterList.Clear();
      this.m_outputParamList.Clear();
      this.m_dataTablesCount = 0;
      this.DisposeComponent();
      this.AppendSql("set nocount on");
      this.AppendSql(Environment.NewLine);
    }

    public SqlBatchBuilder AppendSql(string newSql)
    {
      this.m_sqlBatch.Append(newSql);
      return this;
    }

    public void AppendSql(params string[] sqlSegments)
    {
      foreach (string sqlSegment in sqlSegments)
        this.AppendSql(sqlSegment);
    }

    public SqlBatchBuilder AppendSqlLine(string sql)
    {
      this.AppendSql(sql);
      this.AppendSql(Environment.NewLine);
      return this;
    }

    public void AppendSqlLine(params string[] sqlSegments)
    {
      this.AppendSql(sqlSegments);
      this.AppendSql(Environment.NewLine);
    }

    public string AddParameterNVarChar(string parameterValue, string parameterName = null)
    {
      if (parameterName == null)
        parameterName = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = parameterValue.Length > 4000 ? new SqlParameter(parameterName, SqlDbType.NVarChar) : new SqlParameter(parameterName, SqlDbType.NVarChar, 4000);
      parameter.Value = (object) parameterValue;
      this.AddParameter(parameter);
      return parameterName;
    }

    public string AddParameterBit(bool parameterValue, string parameterName = null)
    {
      if (parameterName == null)
        parameterName = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = new SqlParameter(parameterName, SqlDbType.Bit);
      parameter.Value = (object) parameterValue;
      this.AddParameter(parameter);
      return parameterName;
    }

    public string AddParameterBit(string parameterValue) => this.AddParameterBit(SqlBatchBuilder.ConvertToBoolean(parameterValue));

    public string AddParameterInt(int parameterValue, string parameterName = null)
    {
      if (parameterName == null)
        parameterName = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = new SqlParameter(parameterName, SqlDbType.Int);
      parameter.Value = (object) parameterValue;
      this.AddParameter(parameter);
      return parameterName;
    }

    public string AddParameterInt(string parameterValue) => this.AddParameterInt(SqlBatchBuilder.ConvertToInt32(parameterValue));

    public string AddParameterDouble(double parameterValue)
    {
      string parameterName = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = new SqlParameter(parameterName, SqlDbType.Float);
      parameter.Value = (object) parameterValue;
      this.AddParameter(parameter);
      return parameterName;
    }

    public string AddParameterDouble(string parameterValue) => this.AddParameterDouble(SqlBatchBuilder.ConvertToDouble(parameterValue));

    public string AddParameterBinary(byte[] parameterValue, string parameterName = null)
    {
      if (parameterName == null)
        parameterName = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = new SqlParameter(parameterName, SqlDbType.Binary, parameterValue.Length);
      parameter.Value = (object) parameterValue;
      this.AddParameter(parameter);
      return parameterName;
    }

    public string AddOutputParameterBinary(byte[] parameterValue)
    {
      string str = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = new SqlParameter(str, SqlDbType.Binary, parameterValue.Length);
      parameter.Value = (object) parameterValue;
      parameter.Direction = ParameterDirection.Output;
      this.AddParameter(parameter);
      this.m_outputParamList.Add(new KeyValuePair<string, SqlParameter>(str, parameter));
      return str;
    }

    public string AddParameterUniqueIdentifier(Guid parameterValue, string parameterName = null)
    {
      if (parameterName == null)
        parameterName = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = new SqlParameter(parameterName, SqlDbType.UniqueIdentifier);
      parameter.Value = (object) parameterValue;
      this.AddParameter(parameter);
      return parameterName;
    }

    public string AddParameterUniqueIdentifier(string parameterValue) => this.AddParameterUniqueIdentifier(SqlBatchBuilder.ConvertToGuid(parameterValue));

    public string AddOutputParameterInt()
    {
      string str = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = new SqlParameter(str, SqlDbType.Int);
      parameter.Direction = ParameterDirection.Output;
      this.AddParameter(parameter);
      this.m_outputParamList.Add(new KeyValuePair<string, SqlParameter>(str, parameter));
      return str;
    }

    public string AddParameterImage(byte[] parameterValue)
    {
      string parameterName = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = new SqlParameter(parameterName, SqlDbType.Image, parameterValue.Length);
      parameter.Value = (object) parameterValue;
      this.AddParameter(parameter);
      return parameterName;
    }

    public string AddParameterDateTime(DateTime parameterValue)
    {
      string parameterName = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = new SqlParameter(parameterName, SqlDbType.DateTime);
      parameter.Value = (object) parameterValue;
      this.AddParameter(parameter);
      return parameterName;
    }

    public string AddParameterDateTime(string parameterValue) => this.AddParameterDateTime(SqlBatchBuilder.ConvertToDateTime(parameterValue));

    public string AddParameterXml(string parameterValue, string parameterName = null)
    {
      if (parameterName == null)
        parameterName = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      SqlParameter parameter = new SqlParameter(parameterName, SqlDbType.Xml);
      parameter.Value = (object) parameterValue;
      this.AddParameter(parameter);
      return parameterName;
    }

    public string AddParameterTable<T>(WorkItemTrackingTableValueParameter<T> tvp)
    {
      string parameterName = "@P" + (this.m_parameterList.Count + 1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.AddParameter(SqlBatchBuilder.CreateTableParameter<T>(parameterName, tvp));
      return parameterName;
    }

    public string AddParameterTable<T>(
      WorkItemTrackingTableValueParameter<T> tvp,
      string parameterName)
    {
      this.AddParameter(SqlBatchBuilder.CreateTableParameter<T>(parameterName, tvp));
      return parameterName;
    }

    public static SqlParameter CreateTableParameter<T>(
      string parameterName,
      WorkItemTrackingTableValueParameter<T> tvp)
    {
      object obj = (object) null;
      if (tvp != null && !tvp.IsNullOrEmpty)
        obj = (object) tvp;
      return new SqlParameter(parameterName, obj)
      {
        TypeName = tvp.TypeName,
        SqlDbType = SqlDbType.Structured
      };
    }

    private void AddParameter(SqlParameter parameter)
    {
      if (this.m_parameterList.Count >= 2098)
        throw new ArgumentException(DalResourceStrings.Get("UpdateTooManySqlParameters"), "updateElement");
      this.m_parameterList.Add(parameter);
    }

    internal static bool ConvertToBoolean(string value)
    {
      value = value.ToLowerInvariant().Trim();
      if (value.Equals("1", StringComparison.Ordinal))
        return true;
      if (value.Equals("0", StringComparison.Ordinal) || value.Equals("-1", StringComparison.Ordinal))
        return false;
      bool result;
      if (!bool.TryParse(value, out result))
        throw new LegacyValidationException(DalResourceStrings.Format("ErrorConversionFailed", (object) value, (object) "Boolean"));
      return result;
    }

    internal static int ConvertToInt32(string value)
    {
      value = value.ToLowerInvariant().Trim();
      int result;
      if (!int.TryParse(value, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        throw new LegacyValidationException(DalResourceStrings.Format("ErrorConversionFailed", (object) value, (object) "Int32"));
      return result;
    }

    internal static Guid ConvertToGuid(string value)
    {
      value = value.ToLowerInvariant().Trim();
      Guid result;
      if (!Guid.TryParse(value, out result))
        throw new LegacyValidationException(DalResourceStrings.Format("ErrorConversionFailed", (object) value, (object) "Guid"));
      return result;
    }

    internal static double ConvertToDouble(string value)
    {
      value = value.ToLowerInvariant().Trim();
      double result;
      if (!double.TryParse(value, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        throw new LegacyValidationException(DalResourceStrings.Format("ErrorConversionFailed", (object) value, (object) "Double"));
      return result;
    }

    internal static DateTime ConvertToDateTime(string value)
    {
      DateTime result;
      if (DateTime.TryParse(value, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
      {
        result = result.ToUniversalTime();
        return SqlBatchBuilder.CheckIsValidSqlDateTime(result);
      }
      throw new LegacyValidationException(DalResourceStrings.Format("ErrorConversionFailed", (object) value, (object) "DateTime"));
    }

    internal static DateTime CheckIsValidSqlDateTime(DateTime date)
    {
      try
      {
        return DateTime.SpecifyKind(new SqlDateTime(date).Value, date.Kind);
      }
      catch
      {
        throw new LegacyValidationException(DalResourceStrings.Format("ErrorConversionFailed", (object) date.ToString(), (object) "SqlDateTime"));
      }
    }

    public void ExecuteBatch() => this.ExecuteBatch(int.MaxValue);

    public virtual void ExecuteBatch(int inMemoryTableCount)
    {
      Action run = (Action) (() =>
      {
        try
        {
          this.RequestContext.TraceEnter(900293, "DataAccessLayer", nameof (SqlBatchBuilder), nameof (ExecuteBatch));
          this.AppendSql("set nocount off");
          bool executionSucceeded = true;
          try
          {
            this.ExecuteBatchQuery(this.m_sqlBatch.ToString(), this.m_parameterList, inMemoryTableCount);
          }
          catch
          {
            executionSucceeded = false;
            throw;
          }
          finally
          {
            if (this.PostExecutionActions != null)
              this.PostExecutionActions(this.RequestContext, executionSucceeded);
          }
          if (this.m_dataTablesCount != this.m_resultPayload.TableCount)
          {
            this.RequestContext.Trace(900515, TraceLevel.Error, "DataAccessLayer", nameof (SqlBatchBuilder), "Expected " + this.m_dataTablesCount.ToString() + " tables - got " + this.m_resultPayload.TableCount.ToString());
            throw new LegacyValidationException(DalResourceStrings.Get("UnexpectedReturnedDataSetException"), 602001);
          }
          this.RequestContext.TraceLeave(900294, "DataAccessLayer", nameof (SqlBatchBuilder), nameof (ExecuteBatch));
        }
        catch (WorkItemTrackingQueryResultSizeLimitExceededException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
      });
      new CommandService(this.RequestContext, CommandSetter.WithGroupKey((CommandGroupKey) "WorkItemTracking.").AndCommandKey((CommandKey) ("WorkItemBatchBuilder.ExecuteBatch-" + SqlBatchBuilder.RemoveIllegalCharacters(this.Component.InitialCatalog))).AndCommandPropertiesDefaults(SqlBatchBuilder.DefaultCommandPropertiesForSQLBatch), run).Execute();
    }

    public int AddExpectedReturnedDataTables(int dataTablesExpected)
    {
      int dataTablesCount = this.m_dataTablesCount;
      this.m_dataTablesCount += dataTablesExpected;
      return dataTablesCount;
    }

    public IVssRequestContext RequestContext => this.m_requestContext;

    public Payload ResultPayload => this.m_resultPayload;

    public int ResultTableCount => this.m_dataTablesCount;

    public StringBuilder SqlBatch
    {
      get => this.m_sqlBatch;
      set => this.m_sqlBatch = value;
    }

    public SqlParameter GetOutputParam(string parameterName)
    {
      foreach (KeyValuePair<string, SqlParameter> outputParam in this.m_outputParamList)
      {
        if (TFStringComparer.QueryOperator.Equals(outputParam.Key, parameterName))
          return outputParam.Value;
      }
      return (SqlParameter) null;
    }

    public List<SqlParameter> ParameterList => this.m_parameterList;

    public int ClientVersion
    {
      get
      {
        if (!this.m_clientVersion.HasValue)
          this.m_clientVersion = new int?(this.RequestContext.GetClientVersion());
        return this.m_clientVersion.Value;
      }
    }

    public virtual int Version
    {
      get
      {
        if (this.m_resultPayload != null)
          return this.m_dtVersion;
        this.m_dtVersion = this.Component.Version;
        return this.m_dtVersion;
      }
    }

    private void ExecuteBatchQuery(
      string sqlBatch,
      List<SqlParameter> parameterList,
      int inMemoryTableCount)
    {
      this.RequestContext.TraceEnter(900295, "DataAccessLayer", nameof (SqlBatchBuilder), nameof (ExecuteBatchQuery));
      this.AddGlobalConverters();
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        this.Component.ExecuteBatchPayload(sqlBatch, parameterList, inMemoryTableCount, this.m_payloadConverter, out this.m_resultPayload, this.m_dataTablesCount, this.m_tableSchemas);
        this.RequestContext.TraceLeave(900296, "DataAccessLayer", nameof (SqlBatchBuilder), nameof (ExecuteBatchQuery));
      }
      finally
      {
        stopwatch.Stop();
        if (this.m_component != null && this.m_resultPayload != null && this.m_resultPayload.SqlAccess != null)
          this.m_component = (SqlAccess) null;
        if (stopwatch.Elapsed.TotalSeconds >= (double) this.m_tracingSecondsLimit)
        {
          if (this.RequestContext != null)
          {
            try
            {
              this.RequestContext.TraceAlways(900695, TraceLevel.Info, "DataAccessLayer", nameof (SqlBatchBuilder), "Sql Batch ran longer than {0} seconds.  Elapsed runtime in seconds: {1}.  (Tracing limit controlled by regkey: '{2}')  Sql Batch content: {3}", (object) this.m_tracingSecondsLimit, (object) stopwatch.Elapsed.TotalSeconds, (object) "/Service/WorkItemTracking/DAL/SqlRuntimeTraceAlwaysSeconds", (object) sqlBatch);
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
    }

    private void AddGlobalConverters()
    {
      if (this.m_globalConvertersAdded)
        return;
      if (this.ClientVersion < 5)
      {
        if (this.m_payloadConverter == null)
          this.m_payloadConverter = new PayloadConverter();
        this.m_payloadConverter.AddGlobalProcessColumnCallback((ProcessColumnCallback) ((table, column) =>
        {
          if (!column.Name.Equals("System.AuthorizedDate"))
            return;
          if (table.Columns.DatasetColumnExists("System.ChangedDate"))
          {
            if (table.Converter == null)
              table.Converter = new PayloadTableConverter();
            table.Converter.AddWriteAction("System.AuthorizedDate", (PayloadTableWriteAction) ((tbl, payloadFieldIndex, row) =>
            {
              if (!tbl.Columns.Contains("System.ChangedDate"))
                return;
              row.SetValue("System.ChangedDate", row["System.AuthorizedDate"]);
            }));
          }
          else
          {
            int index = table.Columns["System.AuthorizedDate"].Index;
            table.Columns.AddColumn(new PayloadColumn(index, "System.ChangedDate", column.DataType));
            if (table.Converter == null)
              table.Converter = new PayloadTableConverter();
            table.Converter.AddReadAction("System.AuthorizedDate", (PayloadTableReadAction) ((reader, tbl, dsFieldIndex, row) => row.SetValue("System.ChangedDate", (object) reader.GetDateTime(dsFieldIndex))), false);
          }
        }));
      }
      this.m_globalConvertersAdded = true;
    }

    private static string RemoveIllegalCharacters(string input) => !string.IsNullOrWhiteSpace(input) ? input.Replace('_', '-') : input;

    internal void SetTableSchema(int index, IPayloadTableSchema tableSchema)
    {
      if (this.m_tableSchemas == null)
        this.m_tableSchemas = (IDictionary<int, IPayloadTableSchema>) new Dictionary<int, IPayloadTableSchema>();
      this.m_tableSchemas[index + this.m_dataTablesCount] = tableSchema;
    }

    public override string ToString() => this.m_sqlBatch.ToString();

    public delegate void SqlBatchAction(IVssRequestContext requestContext, bool executionSucceeded);
  }
}
