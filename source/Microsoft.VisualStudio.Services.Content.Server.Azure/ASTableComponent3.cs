// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ASTableComponent3
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class ASTableComponent3 : ASTableComponent2
  {
    public override SQLTableData RangeQuery(
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
      this.BindInt(nameof (maxToTake), maxToTake);
      List<SqlEntity> items1;
      List<SqlProperty> items2;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "ASTable.prc_RangeQuery", this.RequestContext))
      {
        resultCollection.AddBinder<SqlEntity>((ObjectBinder<SqlEntity>) new EntityColumns());
        resultCollection.AddBinder<SqlProperty>((ObjectBinder<SqlProperty>) new PropertyColumns());
        items1 = resultCollection.GetCurrent<SqlEntity>().Items;
        resultCollection.NextResult();
        items2 = resultCollection.GetCurrent<SqlProperty>().Items;
      }
      return new SQLTableData(items1, items2);
    }

    protected override void BindSQLOperationData(SqlOperationData opdata)
    {
      this.BindTable("operations", "ASTable.typ_Operation", opdata.Operations.Select<SqlOperation, SqlDataRecord>((System.Func<SqlOperation, SqlDataRecord>) (operation =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ASTableComponent.typ_Operation);
        sqlDataRecord.SetInt32(0, operation.Idx);
        sqlDataRecord.SetString(1, operation.OperationType);
        return sqlDataRecord;
      })));
      this.BindTable("entities", "ASTable.typ_Entity2", opdata.Entities.Select<SqlEntity, SqlDataRecord>((System.Func<SqlEntity, SqlDataRecord>) (entity =>
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
  }
}
