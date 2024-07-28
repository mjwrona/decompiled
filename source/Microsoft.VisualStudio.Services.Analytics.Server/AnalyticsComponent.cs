// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsComponent
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.OnPrem;
using Microsoft.VisualStudio.Services.Analytics.Transform;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsComponent : TeamFoundationSqlResourceComponent
  {
    private DbContext _context;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[61]
    {
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(1, true),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(2),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(3),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(4),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(5),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(6),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(7),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(8),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(9),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(10),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(11),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(12),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(13),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(14),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(15),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(16),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(17),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(18),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(19),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(20),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(21),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(22),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(23),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(24),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(25),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(26),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(27),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(28),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(29),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(30),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(31),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(32),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(33),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(34),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(35),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(36),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(37),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(38),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(39),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(40),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(41),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(42),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(43),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(44),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(45),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(46),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(47),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(48),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(49),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(50),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(51),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(52),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(53),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(54),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(55),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(56),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(57),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(58),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(59),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(60),
      (IComponentCreator) new ComponentCreator<AnalyticsComponent>(61)
    }, "Analytics");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    private static readonly Regex _percentileRegex = new Regex("\\[AX].\\[(PERCENTILE_((CONT)|(DISC)))]\\(([^,]+),([^),]+)(,(.+))?\\) AS [[]", RegexOptions.Compiled);
    private static readonly Regex _lagLeadRegex = new Regex("\\[AX].\\[((LAG)|(LEAD))]\\(([^,]+),([^),]+)(,([^)]+))\\)", RegexOptions.Compiled);
    private static readonly Regex _rowNumberRegex = new Regex("\\[AX].\\[ROW_NUMBER]\\(([^),]+)(,(.+))?\\)", RegexOptions.Compiled);
    private static readonly Regex _workItemViewRegex = new Regex("\\[vw_WorkItem(RevisionCustom[0-9]+)?]", RegexOptions.Compiled);

    public static int MaxVersion
    {
      get
      {
        IComponentCreator componentCreator = AnalyticsComponent.ComponentFactory.GetLastComponentCreator();
        return componentCreator == null ? int.MaxValue : componentCreator.ServiceVersion;
      }
    }

    public AnalyticsComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.SqlOptions = SqlOptions.None;
      this.SqlHints = new SqlHints();
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) AnalyticsComponent.s_sqlExceptionFactories;

    internal virtual DbContext Context => this._context ?? (this._context = this.CreateContext());

    internal SqlOptions SqlOptions { get; set; }

    internal SqlHints SqlHints { get; set; }

    internal virtual int GetVersion() => this.Version;

    private DbContext CreateContext()
    {
      this.TraceEnter(12012003, nameof (CreateContext));
      try
      {
        return Activator.CreateInstance(this.ContextType, (object) this.CompiledModel, (object) this, (object) this.Connection) as DbContext;
      }
      finally
      {
        this.TraceLeave(12012004, nameof (CreateContext));
      }
    }

    public Type ContextType { get; set; } = typeof (AnalyticsContext);

    public DbCompiledModel CompiledModel { get; set; }

    public int Level { get; internal set; }

    public IEdmModel EdmModel { get; set; }

    public bool DisableAdaptiveJoin { get; set; }

    public virtual IQueryable<T> GetTable<T>(ProjectInfo project = null) where T : class, IPartitionScoped => this.Context.Set<T>().Where<T>((Expression<System.Func<T, bool>>) (wit => wit.PartitionId == this.PartitionId));

    public bool TryGetEntityType(string entityTypeName, out Type type) => ((IEnumerable<PropertyInfo>) this.ContextType.GetProperties()).Where<PropertyInfo>((System.Func<PropertyInfo, bool>) (p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof (DbSet<>))).Select<PropertyInfo, Type>((System.Func<PropertyInfo, Type>) (p => ((IEnumerable<Type>) p.PropertyType.GetGenericArguments()).First<Type>())).ToDictionary<Type, string, Type>((System.Func<Type, string>) (t => t.Name), (System.Func<Type, Type>) (t => t)).TryGetValue(entityTypeName, out type);

    public virtual SqlDataReader ExecuteContextCommand(SqlCommand command)
    {
      string sqlStatement = (ServicePrincipals.IsServicePrincipal(this.RequestContext, this.RequestContext.GetAuthenticatedDescriptor()) ? string.Empty : "/*  ODATA-97D3CA83-0B6F-4E51-8C96-6AD31588457A */ ") + this.PrepareCommandText(command);
      command.CommandText = sqlStatement;
      this.PrepareSqlBatch(sqlStatement.Length, false);
      this.AddStatement(sqlStatement);
      foreach (SqlParameter parameter in (DbParameterCollection) command.Parameters)
      {
        this.HandleProjectScopedQueryFilter(parameter);
        switch (parameter.SqlDbType)
        {
          case SqlDbType.BigInt:
            this.ReBind<long>(parameter, new Func<string, long, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindLong), (Func<string, long?, SqlParameter>) null);
            continue;
          case SqlDbType.Bit:
            this.ReBind<bool>(parameter, new Func<string, bool, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindBoolean), new Func<string, bool?, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindNullableBoolean));
            continue;
          case SqlDbType.Decimal:
            this.ReBind<Decimal>(parameter, new Func<string, Decimal, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindDecimal), new Func<string, Decimal?, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindNullableDecimal));
            continue;
          case SqlDbType.Float:
            this.ReBind<double>(parameter, new Func<string, double, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindDouble), (Func<string, double?, SqlParameter>) null);
            continue;
          case SqlDbType.Int:
            this.ReBind<int>(parameter, new Func<string, int, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindInt), new Func<string, int?, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindNullableInt));
            continue;
          case SqlDbType.NVarChar:
            this.BindString(parameter.ParameterName, (string) parameter.Value, ((string) parameter.Value).Length, BindStringBehavior.Unchanged, parameter.SqlDbType);
            continue;
          case SqlDbType.Real:
            this.ReBind<float>(parameter, new Func<string, float, SqlParameter>(this.BindReal), (Func<string, float?, SqlParameter>) null);
            continue;
          case SqlDbType.UniqueIdentifier:
            this.ReBind<Guid>(parameter, new Func<string, Guid, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindGuid), new Func<string, Guid?, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindNullableGuid));
            continue;
          case SqlDbType.DateTimeOffset:
            this.ReBind<DateTimeOffset>(parameter, new Func<string, DateTimeOffset, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindDateTimeOffset), new Func<string, DateTimeOffset?, SqlParameter>(((TeamFoundationSqlResourceComponent) this).BindNullableDateTimeOffset));
            continue;
          default:
            throw new NotSupportedException(AnalyticsResources.UNSUPPORTED_SQL_DB_TYPE((object) parameter.SqlDbType));
        }
      }
      return this.ExecuteReader();
    }

    protected virtual SqlParameter BindReal(string parameterName, float parameterValue) => this.BindDouble(parameterName, (double) parameterValue);

    private void HandleProjectScopedQueryFilter(SqlParameter p)
    {
      if (!(p.ParameterName == "projectSK"))
        return;
      p.Value = (object) this.GetProjectGuid();
    }

    public virtual Guid? GetProjectGuid()
    {
      object obj = (object) null;
      return this.RequestContext.Items.TryGetValue("AnalyticsProjectSK", out obj) ? new Guid?((Guid) obj) : new Guid?();
    }

    public virtual IReadOnlyCollection<ProviderSyncData> GetSyncDates(
      int minTransformPriority,
      IEnumerable<string> modelTableNames)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetSyncDates");
      this.BindInt("@minTransformPriority", minTransformPriority);
      if (this.Version >= 49)
        this.BindStringTable("@modelTableNames", modelTableNames);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProviderSyncData>((ObjectBinder<ProviderSyncData>) new SyncDateColumns());
        return (IReadOnlyCollection<ProviderSyncData>) resultCollection.GetCurrent<ProviderSyncData>().Items;
      }
    }

    public virtual IReadOnlyCollection<SpaceRequirements> GetSpaceEstimate()
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_QuerySpaceRequirements");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SpaceRequirements>((ObjectBinder<SpaceRequirements>) new AnalyticsComponent.SpaceRequirementsBinder());
        return (IReadOnlyCollection<SpaceRequirements>) resultCollection.GetCurrent<SpaceRequirements>().Items;
      }
    }

    private string PrepareCommandText(SqlCommand command)
    {
      string input1 = command.CommandText;
      if (this.SqlOptions != SqlOptions.None)
      {
        List<string> stringList1 = new List<string>();
        bool flag1 = false;
        bool flag2 = false;
        if ((this.SqlOptions & SqlOptions.TestResultRecompile) == SqlOptions.TestResultRecompile)
        {
          stringList1.Add("RECOMPILE");
          flag1 = true;
        }
        if (command.Parameters.Count > 0)
        {
          foreach (SqlParameter parameter in (DbParameterCollection) command.Parameters)
          {
            if ((this.SqlOptions & SqlOptions.UnknownPartitionId) == SqlOptions.UnknownPartitionId && parameter.ParameterName == "p__linq__0" && parameter.SqlDbType == SqlDbType.Int && !flag1)
              stringList1.Add("OPTIMIZE FOR(@p__linq__0 UNKNOWN)");
          }
        }
        HashSet<string> source = new HashSet<string>();
        if ((this.SqlOptions & SqlOptions.SnapshotJoin) == SqlOptions.SnapshotJoin || (this.SqlOptions & SqlOptions.TestResultJoinOptimization) == SqlOptions.TestResultJoinOptimization || (this.SqlOptions & SqlOptions.AssumeJoinPredicateDependsOnFilter) == SqlOptions.AssumeJoinPredicateDependsOnFilter)
          source.Add("ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS");
        if ((this.SqlOptions & SqlOptions.DisableAdaptiveJoin) == SqlOptions.DisableAdaptiveJoin)
          source.Add("DISABLE_BATCH_MODE_ADAPTIVE_JOINS");
        SqlHints sqlHints;
        if ((this.SqlOptions & SqlOptions.HashJoinFilterHint) == SqlOptions.HashJoinFilterHint || (this.SqlOptions & SqlOptions.HashJoinForBurnDownHint) == SqlOptions.HashJoinForBurnDownHint)
        {
          stringList1.Add("HASH JOIN");
          if (this.SqlHints.BurndownMaxdop != "")
          {
            List<string> stringList2 = stringList1;
            sqlHints = this.SqlHints;
            string str = "MAXDOP " + sqlHints.BurndownMaxdop;
            stringList2.Add(str);
            flag2 = true;
          }
          sqlHints = this.SqlHints;
          if (sqlHints.EnableParallelPlan == "1")
            source.Add("ENABLE_PARALLEL_PLAN_PREFERENCE");
        }
        if (source.Any<string>())
          stringList1.Add("USE HINT(" + string.Join(",", source.Select<string, string>((System.Func<string, string>) (h => "'" + h + "'"))) + ")");
        if ((this.SqlOptions & SqlOptions.NoHintViewForRollup) == SqlOptions.NoHintViewForRollup && (this.SqlOptions & SqlOptions.LoopJoinForRollupHint) == SqlOptions.LoopJoinForRollupHint)
        {
          stringList1.Add("LOOP JOIN");
          sqlHints = this.SqlHints;
          if (sqlHints.RollupMaxdop != "" && !flag2)
          {
            List<string> stringList3 = stringList1;
            sqlHints = this.SqlHints;
            string str = "MAXDOP " + sqlHints.RollupMaxdop;
            stringList3.Add(str);
          }
          sqlHints = this.SqlHints;
          if (sqlHints.RollupForceOrder == "1")
            stringList1.Add("FORCE ORDER");
        }
        if (stringList1.Any<string>())
          input1 = input1 + " OPTION(" + string.Join(",", (IEnumerable<string>) stringList1) + ")";
        if ((this.SqlOptions & SqlOptions.ForcePartitionFilter) == SqlOptions.ForcePartitionFilter)
          input1 = input1.Replace("([Extent1].[PartitionId] = @p__linq__0)", "([Extent1].[PartitionId] = @p__linq__0) AND ($partition.func_AnalyticsWorkItemPartition([Extent1].[PartitionId]) = $partition.func_AnalyticsWorkItemPartition(@p__linq__0))");
        if ((this.SqlOptions & SqlOptions.NoHintViewForRollup) == SqlOptions.NoHintViewForRollup)
          input1 = AnalyticsComponent._workItemViewRegex.Replace(input1, "[vw_WorkItem$1_NoHint]");
      }
      string input2 = AnalyticsComponent._percentileRegex.Replace(input1, (MatchEvaluator) (m =>
      {
        if (m.Groups.Count != 9 || string.IsNullOrWhiteSpace(m.Groups[8].Value))
          return string.Format("{0}({1}) WITHIN GROUP(ORDER BY {2}) OVER(PARTITION BY NULL) AS [", (object) m.Groups[1], (object) m.Groups[6], (object) m.Groups[5]);
        return string.Format("{0}({1}) WITHIN GROUP(ORDER BY {2}) OVER(PARTITION BY {3}) AS [", (object) m.Groups[1], (object) m.Groups[6], (object) m.Groups[5], (object) m.Groups[8]);
      }));
      string input3 = AnalyticsComponent._lagLeadRegex.Replace(input2, "$1($4) OVER(PARTITION BY $7 ORDER BY $5)");
      return AnalyticsComponent._rowNumberRegex.Replace(input3, "ROW_NUMBER() OVER(PARTITION BY $3 ORDER BY $1)");
    }

    private SqlParameter ReBind<T>(
      SqlParameter p,
      Func<string, T, SqlParameter> bindFunc,
      Func<string, T?, SqlParameter> bindNullableFunc)
      where T : struct
    {
      if (!p.IsNullable)
        return bindFunc(p.ParameterName, (T) p.Value);
      return bindNullableFunc != null ? bindNullableFunc(p.ParameterName, this.Convert<T>(p.Value)) : this.BindNullValue(p.ParameterName, p.SqlDbType);
    }

    private T? Convert<T>(object value) where T : struct => value == DBNull.Value ? new T?() : new T?((T) value);

    internal class SpaceRequirementsBinder : ObjectBinder<SpaceRequirements>
    {
      private SqlColumnBinder DataTypeColumn = new SqlColumnBinder("DataType");
      private SqlColumnBinder EstimateInMBColumn = new SqlColumnBinder("EstimateInMB");
      private SqlColumnBinder CurrentInMBColumn = new SqlColumnBinder("CurrentInMB");
      private SqlColumnBinder ModelReadyColumn = new SqlColumnBinder("ModelReady");
      private SqlColumnBinder DetailsColumn = new SqlColumnBinder("Details");

      protected override SpaceRequirements Bind() => new SpaceRequirements()
      {
        DataType = this.DataTypeColumn.GetString((IDataReader) this.Reader, true),
        EstimateInMB = this.EstimateInMBColumn.GetNullableInt32((IDataReader) this.Reader),
        CurrentInMB = this.CurrentInMBColumn.GetNullableInt32((IDataReader) this.Reader),
        ModelReady = this.ModelReadyColumn.GetNullableBoolean((IDataReader) this.Reader),
        Details = this.DetailsColumn.GetString((IDataReader) this.Reader, true)
      };
    }
  }
}
