// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalBeginLocalTranElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalBeginLocalTranElement : DalSqlElement
  {
    public void JoinBatch(ElementGroup group)
    {
      this.SetOutputs(0);
      this.SetGroup(group);
      this.AppendSql("set xact_abort on;");
      this.AppendSql("set implicit_transactions off;");
      this.AppendSql("set transaction isolation level serializable;");
      this.AppendSql("begin transaction");
      this.AppendSql(Environment.NewLine);
    }
  }
}
