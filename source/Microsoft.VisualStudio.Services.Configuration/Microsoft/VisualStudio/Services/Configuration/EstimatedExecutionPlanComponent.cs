// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.EstimatedExecutionPlanComponent
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class EstimatedExecutionPlanComponent : TeamFoundationSqlResourceComponent
  {
    public List<EstimatedExecutionPlanComponent.StoredProcedureDescription> GetStoredProcedures(
      string nameFilter = null,
      DateTime? modifiedAfter = null,
      bool useDatabaseCreation = false)
    {
      return this.GetSpecificEstimateableItems<EstimatedExecutionPlanComponent.StoredProcedureDescription>((ObjectBinder<EstimatedExecutionPlanComponent.StoredProcedureDescription>) new EstimatedExecutionPlanComponent.EstimateableItemBinder<EstimatedExecutionPlanComponent.StoredProcedureDescription>(), "stmt_QuerySprocs.sql", nameFilter, modifiedAfter, useDatabaseCreation);
    }

    public List<EstimatedExecutionPlanComponent.ViewDescription> GetViews(
      string nameFilter = null,
      DateTime? modifiedAfter = null,
      bool useDatabaseCreation = false)
    {
      return this.GetSpecificEstimateableItems<EstimatedExecutionPlanComponent.ViewDescription>((ObjectBinder<EstimatedExecutionPlanComponent.ViewDescription>) new EstimatedExecutionPlanComponent.EstimateableItemBinder<EstimatedExecutionPlanComponent.ViewDescription>(), "stmt_QueryViews.sql", nameFilter, modifiedAfter, useDatabaseCreation);
    }

    public List<EstimatedExecutionPlanComponent.FunctionDescription> GetFunctions(
      string nameFilter = null,
      DateTime? modifiedAfter = null,
      bool useDatabaseCreation = false)
    {
      return this.GetSpecificEstimateableItems<EstimatedExecutionPlanComponent.FunctionDescription>((ObjectBinder<EstimatedExecutionPlanComponent.FunctionDescription>) new EstimatedExecutionPlanComponent.FunctionDescriptionBinder(), "stmt_QueryFunctions.sql", nameFilter, modifiedAfter, useDatabaseCreation);
    }

    public List<EstimatedExecutionPlanComponent.EstimateableItem> GetEstimateableItems(
      string nameFilter = null,
      DateTime? modifiedAfter = null,
      bool useDatabaseCreation = false)
    {
      List<EstimatedExecutionPlanComponent.StoredProcedureDescription> storedProcedures = this.GetStoredProcedures(nameFilter, modifiedAfter, useDatabaseCreation);
      List<EstimatedExecutionPlanComponent.ViewDescription> views = this.GetViews(nameFilter, modifiedAfter, useDatabaseCreation);
      List<EstimatedExecutionPlanComponent.FunctionDescription> functions = this.GetFunctions(nameFilter, modifiedAfter, useDatabaseCreation);
      List<EstimatedExecutionPlanComponent.EstimateableItem> estimateableItems = new List<EstimatedExecutionPlanComponent.EstimateableItem>(storedProcedures.Count + views.Count + functions.Count);
      foreach (EstimatedExecutionPlanComponent.StoredProcedureDescription procedureDescription in storedProcedures)
        estimateableItems.Add((EstimatedExecutionPlanComponent.EstimateableItem) procedureDescription);
      foreach (EstimatedExecutionPlanComponent.ViewDescription viewDescription in views)
        estimateableItems.Add((EstimatedExecutionPlanComponent.EstimateableItem) viewDescription);
      foreach (EstimatedExecutionPlanComponent.FunctionDescription functionDescription in functions)
        estimateableItems.Add((EstimatedExecutionPlanComponent.EstimateableItem) functionDescription);
      return estimateableItems;
    }

    public string GetEstimatedExecutionPlan(
      EstimatedExecutionPlanComponent.EstimateableItem item)
    {
      ArgumentUtility.CheckForNull<EstimatedExecutionPlanComponent.EstimateableItem>(item, nameof (item));
      string str = item.FullyDefined ? item.ExecutionStatement : throw new EstimatedExecutionPlanComponent.NotFullyDefinedException(item);
      this.StartCollectingExecutionPlans();
      try
      {
        this.PrepareSqlBatch(str.Length);
        this.AddStatement(str);
        object obj = this.ExecuteScalar();
        if (obj == null)
        {
          TraceError(15080500, "Null was returned", str);
          return (string) null;
        }
        if (obj is int num)
        {
          TraceError(15080501, string.Format("{0} was returned", (object) num), str);
          this.StartCollectingExecutionPlans();
          this.PrepareSqlBatch(str.Length);
          this.AddStatement(str);
          obj = this.ExecuteScalar();
          TraceError(15080502, string.Format("After getting {0} got {1} on retry", (object) num, obj), str);
        }
        if (obj != null && obj is string estimatedExecutionPlan)
          return estimatedExecutionPlan;
        TraceError(15080503, string.Format("Got [{0}] {1}", (object) obj?.GetType().Name, obj), str);
        return obj?.ToString();
      }
      finally
      {
        this.StopCollectingExecutionPlans();
      }

      void TraceError(int tracePoint, string details, string sql)
      {
        string message = details + " while computing estimated execution plan for " + sql;
        this.Trace(tracePoint, TraceLevel.Error, message);
        this.Logger.Warning(string.Format("[{0}] {1}", (object) tracePoint, (object) message));
      }
    }

    protected string ExtractStatementFromResource(string fileName) => EmbeddedResourceUtil.GetResourceAsString(fileName);

    private void StartCollectingExecutionPlans()
    {
      this.PrepareSqlBatch("SET SHOWPLAN_XML ON".Length);
      this.AddStatement("SET SHOWPLAN_XML ON");
      this.ExecuteNonQuery();
    }

    private void StopCollectingExecutionPlans()
    {
      this.PrepareSqlBatch("SET SHOWPLAN_XML OFF".Length);
      this.AddStatement("SET SHOWPLAN_XML OFF");
      this.ExecuteNonQuery();
    }

    private List<T> GetSpecificEstimateableItems<T>(
      ObjectBinder<T> binder,
      string sqlStatementFileName,
      string nameFilter,
      DateTime? modifiedAfter,
      bool useDatabaseCreation)
      where T : EstimatedExecutionPlanComponent.EstimateableItem, new()
    {
      this.Logger.Info("Getting {0}. nameFilter: {1} modifiedAfter: {2} userDatabaseCreation: {3}", (object) typeof (T).Name, (object) nameFilter, (object) modifiedAfter, (object) useDatabaseCreation);
      if (useDatabaseCreation && modifiedAfter.HasValue)
        throw new ArgumentException("useDatabaseCreation can only be used in modifedAfter is false");
      string statementFromResource = this.ExtractStatementFromResource(sqlStatementFileName);
      this.PrepareSqlBatch(statementFromResource.Length);
      this.AddStatement(statementFromResource);
      this.BindString("@name_Filter", nameFilter, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindNullableDateTime("@modified_after", modifiedAfter);
      this.BindBoolean("@use_DB_creation", useDatabaseCreation);
      new ResultCollection((IDataReader) this.ExecuteReader(), sqlStatementFileName, (IVssRequestContext) null).AddBinder<T>(binder);
      List<T> items = binder.Items;
      this.Logger.Info("Found: {0}", (object) items.Count);
      return items;
    }

    [Serializable]
    public abstract class EstimateableItem
    {
      public string Name { get; set; }

      public string Schema { get; set; }

      public DateTime LastModified { get; set; }

      public bool FullyDefined { get; set; }

      public bool Whitelisted { get; set; }

      public string WellFormedName => StringUtil.QuoteName(this.Schema) + "." + StringUtil.QuoteName(this.Name);

      public abstract string ExecutionStatement { get; }
    }

    public sealed class StoredProcedureDescription : EstimatedExecutionPlanComponent.EstimateableItem
    {
      public override string ExecutionStatement => "Exec " + this.WellFormedName;
    }

    public sealed class ViewDescription : EstimatedExecutionPlanComponent.EstimateableItem
    {
      public override string ExecutionStatement => "SELECT * FROM " + this.WellFormedName;
    }

    public sealed class FunctionDescription : EstimatedExecutionPlanComponent.EstimateableItem
    {
      private string m_executionStatement;

      internal void SetExecutionStatement(string value) => this.m_executionStatement = value;

      public override string ExecutionStatement => this.m_executionStatement;
    }

    public class SqlScriptDescription : EstimatedExecutionPlanComponent.EstimateableItem
    {
      private readonly string m_sql;

      public SqlScriptDescription(string sql)
      {
        this.m_sql = sql;
        this.FullyDefined = true;
      }

      public override string ExecutionStatement => this.m_sql;
    }

    [Serializable]
    public class NotFullyDefinedException : Exception
    {
      public NotFullyDefinedException(
        EstimatedExecutionPlanComponent.EstimateableItem item)
      {
        this.Item = item;
      }

      protected NotFullyDefinedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
        this.Item = (EstimatedExecutionPlanComponent.EstimateableItem) info.GetValue(nameof (Item), typeof (EstimatedExecutionPlanComponent.EstimateableItem));
      }

      public override void GetObjectData(SerializationInfo info, StreamingContext context)
      {
        base.GetObjectData(info, context);
        info.AddValue("Item", (object) this.Item);
      }

      public EstimatedExecutionPlanComponent.EstimateableItem Item { get; private set; }

      public override string Message => string.Format("EstimateableItem item {0} is not fully Defined.  This is likely caused because the item is encrypted, and as such a real Estimated Execution Plan cannot be computed.", (object) this.Item.WellFormedName);
    }

    private class EstimateableItemBinder<T> : ObjectBinder<T> where T : EstimatedExecutionPlanComponent.EstimateableItem, new()
    {
      private SqlColumnBinder nameBinder = new SqlColumnBinder("name");
      private SqlColumnBinder schemaNameBinder = new SqlColumnBinder("schema_name");
      private SqlColumnBinder modifyDateBinder = new SqlColumnBinder("modify_date");
      private SqlColumnBinder definitionLengthBinder = new SqlColumnBinder("definition_length");
      private SqlColumnBinder whitelistedBinder = new SqlColumnBinder("whitelisted");

      protected override T Bind()
      {
        T obj = new T();
        obj.Name = this.nameBinder.GetString((IDataReader) this.Reader, false);
        obj.Schema = this.schemaNameBinder.GetString((IDataReader) this.Reader, false);
        obj.LastModified = this.modifyDateBinder.GetDateTime((IDataReader) this.Reader);
        obj.FullyDefined = this.definitionLengthBinder.GetInt32((IDataReader) this.Reader) > 0;
        obj.Whitelisted = this.whitelistedBinder.GetBoolean((IDataReader) this.Reader);
        return obj;
      }
    }

    private class FunctionDescriptionBinder : 
      EstimatedExecutionPlanComponent.EstimateableItemBinder<EstimatedExecutionPlanComponent.FunctionDescription>
    {
      private SqlColumnBinder ExecutionStatementBinder = new SqlColumnBinder("execution_statement");

      protected override EstimatedExecutionPlanComponent.FunctionDescription Bind()
      {
        EstimatedExecutionPlanComponent.FunctionDescription functionDescription = base.Bind();
        functionDescription.SetExecutionStatement(this.ExecutionStatementBinder.GetString((IDataReader) this.Reader, false));
        return functionDescription;
      }
    }
  }
}
