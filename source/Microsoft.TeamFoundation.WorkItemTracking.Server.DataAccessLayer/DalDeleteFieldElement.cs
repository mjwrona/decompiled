// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalDeleteFieldElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalDeleteFieldElement : DalSqlElement
  {
    public void JoinBatch(ElementGroup group, int actionId)
    {
      this.m_elementGroup = group;
      this.m_outputs = 1;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      this.AppendSql(Environment.NewLine);
      this.AppendSql("exec dbo.");
      this.AppendSql("DeleteField");
      this.AppendSql(" ");
      this.AppendPartitionIdVariable();
      this.AppendSql("@PersonId");
      this.AppendSql(",");
      this.AppendSql("@NowUtc");
      this.AppendSql(",");
      this.AppendSql("@O");
      this.AppendSql(actionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.AppendSql(",");
      this.AppendSql("@fRollback");
      this.AppendSql(" output");
      this.AppendSql(Environment.NewLine);
    }
  }
}
