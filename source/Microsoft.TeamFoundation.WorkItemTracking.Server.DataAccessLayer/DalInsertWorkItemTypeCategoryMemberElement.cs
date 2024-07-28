// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalInsertWorkItemTypeCategoryMemberElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalInsertWorkItemTypeCategoryMemberElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      int actionId,
      int categoryID,
      int tempCategoryID,
      int witID,
      int tempWitID,
      bool overwrite)
    {
      string empty = string.Empty;
      this.m_elementGroup = group;
      this.m_outputs = 1;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      this.AppendSql("declare ");
      this.AppendSql("@O");
      this.AppendSql(DalSqlElement.InlineInt(actionId));
      this.AppendSql(" as int; ");
      this.AppendSql("set ");
      this.AppendSql("@O");
      this.AppendSql(DalSqlElement.InlineInt(actionId));
      this.AppendSql(" = null;");
      this.AppendSql("exec dbo.");
      this.AppendSql("AddWorkItemTypeCategoryMember");
      this.AppendSql(" ");
      this.AppendPartitionIdVariable();
      if (tempCategoryID > 0)
      {
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempCategoryID));
        this.AppendSql(", ");
      }
      else
      {
        this.AppendSql(this.SqlBatch.AddParameterInt(categoryID));
        this.AppendSql(", ");
      }
      if (tempWitID > 0)
      {
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempWitID));
        this.AppendSql(", ");
      }
      else
      {
        this.AppendSql(this.SqlBatch.AddParameterInt(witID));
        this.AppendSql(", ");
      }
      this.AppendSql("@PersonId");
      this.AppendSql(", ");
      this.AppendSql("@NowUtc");
      this.AppendSql(", ");
      this.AppendSql("default");
      if (this.Version > 2)
      {
        this.AppendSql(", ");
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(actionId));
        this.AppendSql(" output");
      }
      this.AppendSql(";if (@@trancount = 0) begin rollback transaction; return; end;");
      if (this.Version > 2)
      {
        this.AppendSql("select ");
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(actionId));
        if (overwrite)
        {
          this.AppendSql(";insert into ");
          this.AppendSql("#categoryMembers");
          this.AppendSql(" select ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(actionId));
        }
      }
      this.AppendSql(Environment.NewLine);
    }
  }
}
