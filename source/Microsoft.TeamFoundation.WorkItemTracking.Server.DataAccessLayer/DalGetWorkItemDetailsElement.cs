// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalGetWorkItemDetailsElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalGetWorkItemDetailsElement : DalGetWorkItemDetailsElementBase
  {
    public void JoinBatch(int workItemId, IVssIdentity user)
    {
      this.m_outputs = 2;
      this.m_index = this.SqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
      this.SqlBatch.AppendSql("exec dbo.");
      this.SqlBatch.AppendSql("GetWorkItemDetail");
      this.SqlBatch.AppendSql(" ");
      this.AppendPartitionIdVariable();
      this.SqlBatch.AppendSql(workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (this.Version < 8)
      {
        this.SqlBatch.AppendSql(",");
        this.SqlBatch.AppendSql(this.SqlBatch.AddParameterNVarChar(user.Descriptor.Identifier));
      }
      this.SqlBatch.AppendSql(Environment.NewLine);
    }

    public Payload GetResults()
    {
      Payload results = new Payload();
      for (int index = 0; index < 2; ++index)
      {
        PayloadTable table = this.SqlBatch.ResultPayload.Tables[this.m_index];
        table.TableName = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.SqlBatch.ResultPayload.Tables.Remove(table);
        if (index == 0)
          table = this.FilterResultTable(table);
        results.Tables.Add(table);
      }
      return results;
    }
  }
}
