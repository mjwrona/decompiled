// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalAddLinkTypeElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalAddLinkTypeElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      string referenceName,
      string forwardName,
      string reverseName,
      int rules)
    {
      this.m_elementGroup = group;
      this.m_outputs = 1;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      this.AppendSql("exec dbo.[");
      this.AppendSql("AddLinkType");
      this.AppendSql("] ");
      this.AppendPartitionIdVariable();
      this.AppendSql(this.SqlBatch.AddParameterNVarChar(referenceName));
      this.AppendSql(", ");
      this.AppendSql(this.SqlBatch.AddParameterNVarChar(forwardName));
      this.AppendSql(", ");
      if (string.IsNullOrEmpty(reverseName))
        this.AppendSql("default");
      else
        this.AppendSql(this.SqlBatch.AddParameterNVarChar(reverseName));
      this.AppendSql(", ");
      this.AppendSql(DalSqlElement.InlineInt(rules));
      this.AppendSql(", ");
      this.AppendSql("@PersonId");
      this.AppendSql(", ");
      this.AppendSql("@NowUtc");
      if (this.Version < 42)
      {
        this.AppendSql(", ");
        this.AppendSql("@userSID");
      }
      this.AppendSql(" if @@trancount = 0 return");
      this.AppendSql(Environment.NewLine);
    }
  }
}
