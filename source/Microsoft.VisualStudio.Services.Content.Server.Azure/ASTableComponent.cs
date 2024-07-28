// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ASTableComponent
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class ASTableComponent : TeamFoundationSqlResourceComponent
  {
    protected const int PartitionKeyMaxLength = 350;
    protected const int RowKeyMaxLength = 350;
    protected const int PropertyNameMaxLength = 100;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ASTableComponent>(1),
      (IComponentCreator) new ComponentCreator<ASTableComponent2>(2),
      (IComponentCreator) new ComponentCreator<ASTableComponent3>(3)
    }, "ASTable");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static string s_area = nameof (ASTableComponent);
    protected static SqlMetaData[] typ_Entity = new SqlMetaData[4]
    {
      new SqlMetaData("Idx", SqlDbType.Int),
      new SqlMetaData("PartitionKey", SqlDbType.VarChar, 350L),
      new SqlMetaData("RowKey", SqlDbType.VarChar, 350L),
      new SqlMetaData("ETag", SqlDbType.VarChar, 50L)
    };
    protected static SqlMetaData[] typ_Operation = new SqlMetaData[2]
    {
      new SqlMetaData("Idx", SqlDbType.Int),
      new SqlMetaData("Operation", SqlDbType.VarChar, 50L)
    };
    protected static SqlMetaData[] typ_Property = new SqlMetaData[4]
    {
      new SqlMetaData("Idx", SqlDbType.Int),
      new SqlMetaData("PropertyName", SqlDbType.VarChar, 100L),
      new SqlMetaData("PropertyType", SqlDbType.SmallInt),
      new SqlMetaData("PropertyValue", SqlDbType.NVarChar, SqlMetaData.Max)
    };

    protected internal int TableId { protected get; set; }

    static ASTableComponent()
    {
      ASTableComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      ASTableComponent.s_sqlExceptionFactories.Add(1710002, SQLETagMismatchException.CreateSqlExceptionFactory());
      ASTableComponent.s_sqlExceptionFactories.Add(1710003, SQLEntityNotFoundException.CreateSqlExceptionFactory());
      ASTableComponent.s_sqlExceptionFactories.Add(1710004, SQLEntityAlreadyExistsException.CreateSqlExceptionFactory());
    }

    public ASTableComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ASTableComponent.s_sqlExceptionFactories;

    protected override string TraceArea => ASTableComponent.s_area;

    public virtual List<SqlOperationResult> ExecuteBatch(SqlOperationData opdata)
    {
      if (opdata == null)
        throw new ArgumentNullException(nameof (opdata));
      if (opdata.Entities.Select<SqlEntity, string>((System.Func<SqlEntity, string>) (e => e.PartitionKey)).Distinct<string>().Count<string>() > 1)
        throw new ArgumentException("All entities in a given batch must have the same partition key.", nameof (opdata));
      foreach (SqlEntity entity in opdata.Entities)
      {
        if (entity.PartitionKey.Length > 350)
          throw new ArgumentException("PartitionKey too long. (PartitionKey: " + entity.PartitionKey + ")");
        if (entity.RowKey.Length > 350)
          throw new ArgumentException("RowKey too long. (RowKey: " + entity.PartitionKey + ")");
      }
      foreach (SqlProperty property in opdata.Properties)
      {
        if (property.PropertyName.Length > 100)
          throw new ArgumentException("PropertyName too long. (PropertyName: " + property.PropertyName + ")");
      }
      this.PrepareStoredProcedure("ASTable.prc_Execute");
      this.BindSQLOperationData(opdata);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "ASTable.prc_Execute", this.RequestContext))
      {
        resultCollection.AddBinder<SqlOperationResult>((ObjectBinder<SqlOperationResult>) new OperationResultColumns());
        return resultCollection.GetCurrent<SqlOperationResult>().Items;
      }
    }

    public virtual SQLTableData RangeQuery(
      string pkMax,
      string pkMin,
      string rkMax,
      string rkMin,
      int maxToTake)
    {
      this.PrepareStoredProcedure("ASTable.prc_RangeQuery");
      this.BindString("minPartitionKey", pkMin, 350, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("maxPartitionKey", pkMax, 350, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("minRowKey", rkMin, 350, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("maxRowKey", rkMax, 350, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      List<SqlEntity> entities = (List<SqlEntity>) null;
      List<SqlProperty> props = (List<SqlProperty>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "ASTable.prc_RangeQuery", this.RequestContext))
      {
        resultCollection.AddBinder<SqlEntity>((ObjectBinder<SqlEntity>) new EntityColumns());
        resultCollection.AddBinder<SqlProperty>((ObjectBinder<SqlProperty>) new PropertyColumns());
        entities = resultCollection.GetCurrent<SqlEntity>().Items;
        resultCollection.NextResult();
        props = resultCollection.GetCurrent<SqlProperty>().Items;
      }
      return new SQLTableData(entities, props);
    }

    public virtual SQLTableData PointQuery(string pk, string rk)
    {
      this.PrepareStoredProcedure("ASTable.prc_PointQuery");
      this.BindString("partitionKey", pk, 350, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("rowKey", rk, 350, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      List<SqlEntity> entities = (List<SqlEntity>) null;
      List<SqlProperty> props = (List<SqlProperty>) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "ASTable.prc_PointQuery", this.RequestContext))
      {
        resultCollection.AddBinder<SqlEntity>((ObjectBinder<SqlEntity>) new EntityColumns());
        resultCollection.AddBinder<SqlProperty>((ObjectBinder<SqlProperty>) new PropertyColumns());
        entities = resultCollection.GetCurrent<SqlEntity>().Items;
        resultCollection.NextResult();
        props = resultCollection.GetCurrent<SqlProperty>().Items;
      }
      return new SQLTableData(entities, props);
    }

    public virtual List<string> ListPrimaryKeys(int total, string pkMinExclusive)
    {
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("PartitionKey");
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_ListPartitionKeys.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.BindArgumentsForPKListing(total, pkMinExclusive);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        List<string> stringList = new List<string>(total);
        while (reader.Read())
        {
          string str = sqlColumnBinder.GetString((IDataReader) reader, false);
          stringList.Add(str);
        }
        return stringList;
      }
    }

    protected override SqlCommand PrepareStoredProcedure(string storedProcedure)
    {
      SqlCommand sqlCommand = base.PrepareStoredProcedure(storedProcedure);
      this.BindInt("@tableId", this.TableId);
      return sqlCommand;
    }

    protected override SqlCommand PrepareSqlBatch(int lengthEstimate)
    {
      SqlCommand sqlCommand = base.PrepareSqlBatch(lengthEstimate);
      this.BindInt("@tableId", this.TableId);
      return sqlCommand;
    }

    protected virtual void BindSQLOperationData(SqlOperationData opdata)
    {
      this.BindTable("operations", "ASTable.typ_Operation", opdata.Operations.Select<SqlOperation, SqlDataRecord>((System.Func<SqlOperation, SqlDataRecord>) (operation =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ASTableComponent.typ_Operation);
        sqlDataRecord.SetInt32(0, operation.Idx);
        sqlDataRecord.SetString(1, operation.OperationType);
        return sqlDataRecord;
      })));
      this.BindTable("entities", "ASTable.typ_Entity", opdata.Entities.Select<SqlEntity, SqlDataRecord>((System.Func<SqlEntity, SqlDataRecord>) (entity =>
      {
        SqlDataRecord record = new SqlDataRecord(ASTableComponent.typ_Entity);
        record.SetInt32(0, entity.Idx);
        record.SetString(1, entity.PartitionKey);
        record.SetString(2, entity.RowKey);
        record.SetString(3, entity.ETag == "*" ? (string) null : entity.ETag, BindStringBehavior.EmptyStringToNull);
        return record;
      })));
      this.BindTable("properties", "ASTable.typ_Property", opdata.Properties.Select<SqlProperty, SqlDataRecord>((System.Func<SqlProperty, SqlDataRecord>) (prop =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ASTableComponent.typ_Property);
        sqlDataRecord.SetInt32(0, prop.Idx);
        sqlDataRecord.SetString(1, prop.PropertyName);
        sqlDataRecord.SetInt16(2, prop.PropertyType);
        sqlDataRecord.SetString(3, prop.PropertyValue);
        return sqlDataRecord;
      })));
    }

    protected void BindArgumentsForPKListing(int total, string pkMinExclusive)
    {
      this.BindInt(nameof (total), total);
      this.BindString("exclusiveMinPartitionKey", pkMinExclusive, 350, BindStringBehavior.Unchanged, SqlDbType.VarChar);
    }
  }
}
