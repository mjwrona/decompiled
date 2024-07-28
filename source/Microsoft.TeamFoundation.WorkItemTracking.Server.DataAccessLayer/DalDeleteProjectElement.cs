// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalDeleteProjectElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalDeleteProjectElement : DalSqlElement
  {
    public void JoinBatch(string projectId, bool witOnly)
    {
      this.m_outputs = 0;
      this.m_index = this.SqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
      this.SqlBatch.AppendSql("declare @status int;");
      this.SqlBatch.AppendSql("exec @status = dbo.");
      this.SqlBatch.AppendSql("ProjectDelete");
      this.SqlBatch.AppendSql(" ");
      this.AppendPartitionIdVariable();
      if (this.Version >= 18)
        this.AppendSql("1, ");
      this.SqlBatch.AppendSql(this.SqlBatch.AddParameterNVarChar(projectId));
      if (this.Version >= 22)
      {
        this.AppendSql(", ");
        this.AppendSql(DalSqlElement.Inline((object) witOnly));
      }
      this.SqlBatch.AppendSql(";if @@trancount = 0 or @status <> 0 return;");
      this.SqlBatch.AppendSql(Environment.NewLine);
    }
  }
}
