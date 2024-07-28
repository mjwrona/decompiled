// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalEventChangeElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalEventChangeElement : DalSqlElement
  {
    public void JoinBatch(string eventClass, string eventData, Guid eventAuthor) => this.JoinBatchInternal((object) eventClass, eventData, eventAuthor);

    public void JoinBatch(Guid eventClass, string eventData, Guid eventAuthor) => this.JoinBatchInternal((object) eventClass, eventData, eventAuthor);

    private void JoinBatchInternal(object eventClass, string eventData, Guid eventAuthor)
    {
      this.SetOutputs(0);
      this.SqlBatch.AppendSql("exec dbo.[");
      this.SqlBatch.AppendSql("EventChange");
      this.SqlBatch.AppendSql("] ");
      this.AppendPartitionIdVariable();
      this.SqlBatch.AppendSql(this.Param(eventClass));
      this.SqlBatch.AppendSql(",");
      this.SqlBatch.AppendSql(this.Param((object) eventData));
      this.SqlBatch.AppendSql(",");
      this.SqlBatch.AppendSql(DalSqlElement.InlineGuid(eventAuthor));
      this.SqlBatch.AppendSql(Environment.NewLine);
    }
  }
}
