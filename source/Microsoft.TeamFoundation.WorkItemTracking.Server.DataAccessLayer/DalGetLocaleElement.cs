// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalGetLocaleElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalGetLocaleElement : DalSqlElement
  {
    public virtual void JoinBatch()
    {
      this.SqlBatch.AppendSql("declare ");
      this.SqlBatch.AppendSql("@Collation");
      this.SqlBatch.AppendSql(" as sysname exec dbo.sp_executesql N'set ");
      this.SqlBatch.AppendSql("@Collation =cast(DATABASEPROPERTYEX(DB_NAME(),''collation'')");
      this.SqlBatch.AppendSql(" as sysname)',N'");
      this.SqlBatch.AppendSql("@Collation sysname output',");
      this.SqlBatch.AppendSql("@Collation=@Collation output ");
      this.SqlBatch.AppendSql("select collationproperty(@Collation,'LCID')");
      this.SqlBatch.AppendSql(Environment.NewLine);
      this.SqlBatch.AppendSql("select collationproperty(@Collation,'ComparisonStyle')");
      this.SqlBatch.AppendSql(Environment.NewLine);
      this.m_outputs = 2;
      this.m_index = this.SqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
    }

    public virtual int GetLocale() => (int) this.SqlBatch.ResultPayload.Tables[this.m_index].Rows[0][0];

    public virtual int GetComparisonStyle() => (int) this.SqlBatch.ResultPayload.Tables[this.m_index + 1].Rows[0][0];
  }
}
