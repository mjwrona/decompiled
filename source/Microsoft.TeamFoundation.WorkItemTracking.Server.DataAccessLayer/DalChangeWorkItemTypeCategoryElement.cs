// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalChangeWorkItemTypeCategoryElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalChangeWorkItemTypeCategoryElement : DalSqlElement
  {
    public void JoinBatch(
      ElementGroup group,
      int categoryID,
      int tempCategoryID,
      int projectID,
      string name,
      string refName,
      int defaultWitID,
      int tempDefaultWitID,
      bool overwrite)
    {
      int num = categoryID < 0 ? 1 : 0;
      this.m_elementGroup = group;
      if (num != 0)
        this.m_outputs = 1;
      else
        this.m_outputs = 0;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      if (num != 0)
      {
        this.AppendSql("declare ");
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempCategoryID));
        this.AppendSql(" as int; ");
        this.AppendSql("set ");
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempCategoryID));
        this.AppendSql(" = null;");
      }
      this.AppendSql("exec dbo.");
      this.AppendSql("ChangeWorkItemTypeCategory");
      this.AppendSql(" ");
      this.AppendPartitionIdVariable();
      if (num != 0)
      {
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempCategoryID));
        this.AppendSql(" output, ");
        this.AppendSql(this.SqlBatch.AddParameterInt(projectID));
        this.AppendSql(", ");
      }
      else
      {
        this.AppendSql(this.SqlBatch.AddParameterInt(categoryID));
        this.AppendSql(", ");
        this.AppendSql("default, ");
      }
      this.AppendSql(this.SqlBatch.AddParameterNVarChar(name));
      this.AppendSql(", ");
      if (num != 0)
      {
        this.AppendSql(this.SqlBatch.AddParameterNVarChar(refName));
        this.AppendSql(", ");
      }
      else
        this.AppendSql("default, ");
      if (tempDefaultWitID > 0)
      {
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempDefaultWitID));
        this.AppendSql(", ");
      }
      else
      {
        this.AppendSql(this.SqlBatch.AddParameterInt(defaultWitID));
        this.AppendSql(", ");
      }
      this.AppendSql("@PersonId");
      this.AppendSql(", ");
      this.AppendSql("@NowUtc");
      if (this.Version > 3)
      {
        this.AppendSql(",");
        this.AppendSql(DalSqlElement.InlineBit(overwrite));
      }
      if (num != 0)
      {
        this.AppendSql(";if ((");
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempCategoryID));
        this.AppendSql(" is null) or (@@trancount = 0)) begin rollback transaction; return; end;");
        this.AppendSql("select ");
        this.AppendSql("@O");
        this.AppendSql(DalSqlElement.InlineInt(tempCategoryID));
        if (overwrite)
        {
          this.AppendSql(";insert into ");
          this.AppendSql("#categoryIds");
          this.AppendSql(" select ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(tempCategoryID));
        }
        this.AppendSql(Environment.NewLine);
      }
      else
      {
        this.AppendSql("; if(@@trancount = 0) begin rollback transaction; return; end;");
        this.AppendSql(Environment.NewLine);
      }
    }
  }
}
