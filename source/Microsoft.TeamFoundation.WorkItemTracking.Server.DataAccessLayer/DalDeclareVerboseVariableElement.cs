// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalDeclareVerboseVariableElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalDeclareVerboseVariableElement : DalSqlElement
  {
    public void JoinBatch(bool verbose)
    {
      this.m_outputs = 0;
      this.SqlBatch.AppendSql("declare ");
      this.SqlBatch.AppendSql("@fVerbose");
      this.SqlBatch.AppendSql(" as bit; set ");
      this.SqlBatch.AppendSql("@fVerbose");
      if (verbose)
        this.SqlBatch.AppendSql("=1;");
      else
        this.SqlBatch.AppendSql("=0;");
      this.SqlBatch.AppendSql(Environment.NewLine);
      this.m_index = this.SqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
    }
  }
}
