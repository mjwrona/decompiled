// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalChangeActionElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalChangeActionElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      int actionId,
      string name,
      int workItemTypeId,
      int tempWorkItemTypeId,
      int fromConstantId,
      int tempFromConstantId,
      int toConstantId,
      int tempToConstantId,
      bool deleted,
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
      this.AppendSql("ChangeAction");
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
      this.AppendSql(DalSqlElement.InlineBit(deleted));
      this.AppendSql(",");
      if (newItem)
      {
        this.AppendSql(this.SqlBatch.AddParameterNVarChar(name));
        this.AppendSql(",");
        if (tempWorkItemTypeId != 0)
        {
          this.AppendSql("@O");
          this.AppendSql(tempWorkItemTypeId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }
        else
          this.AppendSql(workItemTypeId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.AppendSql(",");
        if (tempFromConstantId != 0)
        {
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(tempFromConstantId));
        }
        else
          this.AppendSql(DalSqlElement.InlineInt(fromConstantId));
        this.AppendSql(",");
        if (tempToConstantId != 0)
        {
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(tempToConstantId));
        }
        else
          this.AppendSql(DalSqlElement.InlineInt(toConstantId));
      }
      else
        this.AppendSql("default,default,default,default");
      if (newItem)
      {
        this.AppendSql(";if ");
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(actionId));
        this.AppendSql(" is null begin rollback transaction; return; end;");
        this.AppendSql("select ");
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(actionId));
        if (overwrite)
        {
          this.AppendSql(";insert into ");
          this.AppendSql("#actionIds");
          this.AppendSql(" select ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(actionId));
        }
      }
      this.AppendSql(Environment.NewLine);
    }
  }
}
