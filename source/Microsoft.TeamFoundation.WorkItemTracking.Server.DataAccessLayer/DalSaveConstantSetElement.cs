// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSaveConstantSetElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSaveConstantSetElement : DalObjectPersistenceElement<ServerConstantSet>
  {
    public DalSaveConstantSetElement() => this.AddPrerequisiteSingletonElement<DalGetTempIdMapElement>();

    public void JoinBatch(ElementGroup group, bool overwrite)
    {
      if (this.ValidObjects.Count < 1)
        return;
      this.SetOutputs(0);
      this.SetGroup(group);
      if (this.Version >= 7)
      {
        this.AppendSql("insert into ");
        this.AppendSql("@tempIdMap");
        this.AppendSql(Environment.NewLine);
        this.AppendSql("exec ");
        this.AppendSql("@status");
        this.AppendSql(" = dbo.");
        this.AppendSql("SaveConstantSets");
        this.AppendSql(" ");
        this.AppendPartitionIdVariable();
        this.AppendSql("@PersonId");
        this.AppendSql(",");
        this.AppendSql("@NowUtc");
        this.AppendSql(",");
        this.AppendSql(this.SqlBatch.AddParameterTable<ServerConstantSet>(this.Version < 12 ? (WorkItemTrackingTableValueParameter<ServerConstantSet>) new ConstantSetTable((IEnumerable<ServerConstantSet>) this.ValidObjects) : (WorkItemTrackingTableValueParameter<ServerConstantSet>) new ConstantSetTable2((IEnumerable<ServerConstantSet>) this.ValidObjects), "@sets"));
        this.AppendSql(",");
        this.AppendSql("@tempIdMap");
        this.AppendSql(Environment.NewLine);
        this.AppendSql("if ");
        this.AppendSql("@status");
        this.AppendSql(" <> 0 begin rollback transaction; return; end");
        this.AppendSql(Environment.NewLine);
        if (!overwrite)
          return;
        this.AppendSql(Environment.NewLine);
        this.AppendSql("insert into ");
        this.AppendSql("#sets");
        this.AppendSql(" select ss.");
        this.AppendSql("ParentID");
        this.AppendSql(", ss.");
        this.AppendSql("ConstID");
        this.AppendSql(Environment.NewLine);
        this.AppendSql("from ");
        this.AppendSql("@sets");
        this.AppendSql(" s join ");
        this.AppendSql("@tempIdMap");
        this.AppendSql(" t on t.TempId = s.SetId + (");
        this.AppendSql(DalSqlElement.Inline((object) -20000));
        this.AppendSql(")");
        this.AppendSql(Environment.NewLine);
        this.AppendSql("join Sets ss on ss.");
        this.AppendSql("PartitionId");
        this.AppendSql(" = ");
        this.AppendSql("@partitionId");
        this.AppendSql(" and ss.");
        this.AppendSql("RuleSetID");
        this.AppendSql(" = t.Id");
        this.AppendSql(Environment.NewLine);
        this.AppendSql("option(optimize for (");
        this.AppendSql("@partitionId");
        this.AppendSql(" unknown))");
        this.AppendSql(Environment.NewLine);
      }
      else
      {
        foreach (ServerConstantSet validObject in (IEnumerable<ServerConstantSet>) this.ValidObjects)
        {
          this.AppendSql("declare ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.SetId));
          this.AppendSql(" as int; ");
          this.AppendSql("exec dbo.");
          this.AppendSql("ChangeRuleSet");
          this.AppendSql(" ");
          this.AppendPartitionIdVariable();
          this.AppendSql("@PersonId");
          this.AppendSql(",");
          this.AppendSql("@NowUtc");
          this.AppendSql(",");
          this.AppendSql("null,");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.SetId));
          this.AppendSql(" output,");
          this.AppendSql("0,");
          if (validObject.TempConstId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(DalSqlElement.InlineInt(validObject.TempConstId));
          }
          else
            this.AppendSql(DalSqlElement.InlineInt(validObject.ConstId));
          this.AppendSql(",");
          if (validObject.TempParentId != 0)
          {
            this.AppendSql("@O");
            this.AppendSql(DalSqlElement.InlineInt(validObject.TempParentId));
          }
          else
            this.AppendSql(DalSqlElement.InlineInt(validObject.ParentId));
          this.AppendSql(";if ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.SetId));
          this.AppendSql(" is null begin rollback transaction; return; end;");
          this.AppendSql("insert into ");
          this.AppendSql("@tempIdMap");
          this.AppendSql(" select ");
          this.AppendSql(DalSqlElement.InlineInt(validObject.SetId - 20000));
          this.AppendSql(",");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.SetId));
          this.AppendSql(Environment.NewLine);
          if (overwrite)
          {
            this.AppendSql(";insert into ");
            this.AppendSql("#sets");
            this.AppendSql(" select ");
            if (validObject.TempParentId != 0)
            {
              this.AppendSql("@O");
              this.AppendSql(DalSqlElement.InlineInt(validObject.TempParentId));
            }
            else
              this.AppendSql(DalSqlElement.InlineInt(validObject.ParentId));
            this.AppendSql(",");
            if (validObject.TempConstId != 0)
            {
              this.AppendSql("@O");
              this.AppendSql(DalSqlElement.InlineInt(validObject.TempConstId));
            }
            else
              this.AppendSql(DalSqlElement.InlineInt(validObject.ConstId));
          }
          this.AppendSql(Environment.NewLine);
        }
      }
    }
  }
}
