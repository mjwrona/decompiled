// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalChangeConstantSetElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalChangeConstantSetElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      int actionId,
      int parentId,
      int tempParentId,
      int constantId,
      int tempConstantId,
      bool delete,
      bool deletedChanged,
      string cacheStamp,
      bool newItem,
      bool overwrite)
    {
      this.m_elementGroup = group;
      if (newItem)
        this.m_outputs = 1;
      else
        this.m_outputs = 0;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      this.AppendSql("exec dbo.");
      this.AppendSql("ChangeRuleSet");
      this.AppendSql(" ");
      this.AppendPartitionIdVariable();
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
      if (deletedChanged)
      {
        this.AppendSql(DalSqlElement.InlineBit(delete));
        this.AppendSql(",");
      }
      else
        this.AppendSql("0,");
      if (tempConstantId != 0)
      {
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempConstantId));
        this.AppendSql(",");
      }
      else if (newItem)
      {
        this.AppendSql(DalSqlElement.InlineInt(constantId));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (tempParentId != 0)
      {
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempParentId));
      }
      else if (newItem)
        this.AppendSql(DalSqlElement.InlineInt(parentId));
      else
        this.AppendSql("default");
      this.AppendSql(";if ");
      this.AppendSql("@O");
      this.AppendSql(actionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.AppendSql(" is null begin rollback transaction; return; end;");
    }
  }
}
