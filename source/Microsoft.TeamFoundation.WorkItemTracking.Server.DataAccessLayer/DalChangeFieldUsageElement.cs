// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalChangeFieldUsageElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalChangeFieldUsageElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      int actionId,
      bool deleted,
      int fldId,
      int tempFldId,
      bool oftenQueried,
      string cacheStamp,
      bool newItem)
    {
      this.m_elementGroup = group;
      if (newItem)
        this.m_outputs = 1;
      else
        this.m_outputs = 0;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      this.AppendSql("exec dbo.");
      this.AppendSql("ChangeFieldUsage");
      this.AppendSql(" ");
      if (this.Version >= 25)
      {
        this.AppendSql("@partitionId");
        this.AppendSql(",");
      }
      this.AppendSql("@PersonId");
      this.AppendSql(",");
      this.AppendSql("@NowUtc");
      this.AppendSql(",");
      if (newItem || cacheStamp == null || cacheStamp.Trim().Length == 0)
      {
        this.AppendSql("null,");
      }
      else
      {
        this.AppendSql(this.SqlBatch.AddParameterBinary(DalMetadataSelectElement.ConvertLongToByteArray(Convert.ToInt64("0x" + cacheStamp, 16))));
        this.AppendSql(",");
      }
      this.AppendSql("@O");
      this.AppendSql(DalSqlElement.InlineInt(actionId));
      if (newItem)
        this.AppendSql(" output,");
      else
        this.AppendSql(",");
      this.AppendSql(DalSqlElement.InlineBit(deleted));
      this.AppendSql(",");
      this.AppendSql(DalSqlElement.InlineBit(oftenQueried));
      this.AppendSql(",");
      if (newItem)
      {
        if (tempFldId != 0)
        {
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(tempFldId));
          this.AppendSql(",");
        }
        else
        {
          this.AppendSql(DalSqlElement.InlineInt(fldId));
          this.AppendSql(",");
        }
      }
      else
        this.AppendSql("default,");
      this.AppendSql("-100");
      if (newItem)
      {
        this.AppendSql(";if ");
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(actionId));
        this.AppendSql(" is null begin rollback transaction; return; end;");
        this.AppendSql("select ");
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(actionId));
      }
      this.AppendSql(Environment.NewLine);
    }
  }
}
