// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalChangeWorkItemTypeElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalChangeWorkItemTypeElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      int actionId,
      int nameConstantId,
      int tempNameConstantId,
      bool nameChanged,
      int projectId,
      bool projectIdChanged,
      int descriptionId,
      int tempDescriptionId,
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
      this.AppendSql("ChangeWorkItemType");
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
      this.AppendSql("@O" + actionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (newItem)
        this.AppendSql(" output,");
      else
        this.AppendSql(",");
      this.AppendSql(DalSqlElement.InlineBit(deleted));
      this.AppendSql(",");
      if (nameChanged)
      {
        if (tempNameConstantId > 0)
        {
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(tempNameConstantId));
        }
        else
          this.AppendSql(DalSqlElement.InlineInt(nameConstantId));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (projectIdChanged)
      {
        this.AppendSql(DalSqlElement.InlineInt(projectId));
        this.AppendSql(",");
      }
      else
        this.AppendSql("default,");
      if (tempDescriptionId > 0)
      {
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempDescriptionId));
      }
      else if (descriptionId >= 0)
        this.AppendSql(DalSqlElement.InlineInt(descriptionId));
      else
        this.AppendSql("default");
      if (this.Version > 3)
      {
        this.AppendSql(",");
        this.AppendSql(DalSqlElement.InlineBit(overwrite));
      }
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
          this.AppendSql(";set ");
          this.AppendSql("@workItemTypeId");
          this.AppendSql("=");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(actionId));
          this.AppendSql(";set ");
          this.AppendSql("@workItemTypeNameConstId");
          this.AppendSql("=");
          if (tempNameConstantId > 0)
          {
            this.AppendSql("@O");
            this.AppendSql(DalSqlElement.InlineInt(tempNameConstantId));
          }
          else
            this.AppendSql(DalSqlElement.InlineInt(nameConstantId));
        }
      }
      this.AppendSql(Environment.NewLine);
    }
  }
}
