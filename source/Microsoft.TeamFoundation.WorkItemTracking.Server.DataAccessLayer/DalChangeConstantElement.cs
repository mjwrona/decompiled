// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalChangeConstantElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalChangeConstantElement : DalObjectPersistenceElement<ServerConstant>
  {
    public DalChangeConstantElement() => this.AddPrerequisiteSingletonElement<DalGetTempIdMapElement>();

    public void JoinBatch(ElementGroup group, bool ignoreCase = false)
    {
      if (this.ValidObjects.Count < 1)
        return;
      this.SetOutputs(0);
      this.SetGroup(group);
      if (this.Version >= 7)
      {
        IEnumerable<ServerConstant> source1 = this.ValidObjects.Where<ServerConstant>((Func<ServerConstant, bool>) (o => !o.LookupAccount));
        if (source1.Any<ServerConstant>())
        {
          this.AppendSql("insert into ");
          this.AppendSql("@tempIdMap");
          this.AppendSql(Environment.NewLine);
          this.AppendSql("exec ");
          this.AppendSql("@status");
          this.AppendSql(" = dbo.");
          this.AppendSql("AddConstants");
          this.AppendSql(" ");
          this.AppendPartitionIdVariable();
          this.AppendSql("@PersonId");
          this.AppendSql(",");
          this.AppendSql(this.SqlBatch.AddParameterTable<ServerConstant>((WorkItemTrackingTableValueParameter<ServerConstant>) new ConstantTable((IEnumerable<ServerConstant>) source1.ToList<ServerConstant>()), "@constants"));
          this.AppendSql(",");
          this.AppendSql(this.SqlBatch.AddParameterBit(ignoreCase, "@ignoreCase"));
          this.AppendSql(Environment.NewLine);
          this.AppendSql("if ");
          this.AppendSql("@status");
          this.AppendSql(" <> 0 return");
          this.AppendSql(Environment.NewLine);
        }
        IEnumerable<ServerConstant> source2 = this.ValidObjects.Where<ServerConstant>((Func<ServerConstant, bool>) (o => o.LookupAccount));
        if (source2.Any<ServerConstant>())
        {
          this.AppendSql("insert into ");
          this.AppendSql("@tempIdMap");
          this.AppendSql(Environment.NewLine);
          this.AppendSql("exec ");
          this.AppendSql("@status");
          this.AppendSql(" = dbo.");
          this.AppendSql("LookupIdentityConstants");
          this.AppendSql(" ");
          this.AppendPartitionIdVariable();
          this.AppendSql("@PersonId");
          this.AppendSql(",");
          this.AppendSql(this.SqlBatch.AddParameterTable<ServerConstant>((WorkItemTrackingTableValueParameter<ServerConstant>) new ConstantTable((IEnumerable<ServerConstant>) source2.ToList<ServerConstant>()), "@identityConstants"));
          this.AppendSql(Environment.NewLine);
          this.AppendSql("if ");
          this.AppendSql("@status");
          this.AppendSql(" <> 0 return");
          this.AppendSql(Environment.NewLine);
        }
        foreach (ServerConstant validObject in (IEnumerable<ServerConstant>) this.ValidObjects)
        {
          this.AppendSql("declare ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.ConstId));
          this.AppendSql(" as int; ");
          this.AppendSql("select ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.ConstId));
          this.AppendSql(" = Id from ");
          this.AppendSql("@tempIdMap");
          this.AppendSql(" where TempId = ");
          this.AppendSql(DalSqlElement.InlineInt(validObject.ConstId - 20000));
          this.AppendSql(Environment.NewLine);
        }
      }
      else
      {
        foreach (ServerConstant validObject in (IEnumerable<ServerConstant>) this.ValidObjects)
        {
          this.AppendSql("declare ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.ConstId));
          this.AppendSql(" as int; ");
          this.AppendSql("exec dbo.");
          this.AppendSql("AddConstant");
          this.AppendSql(" ");
          this.AppendPartitionIdVariable();
          this.AppendSql("@PersonId");
          this.AppendSql(",");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.ConstId));
          this.AppendSql(" output,");
          this.AppendSql(!validObject.LookupAccount ? this.SqlBatch.AddParameterNVarChar(validObject.DisplayPart) : this.SqlBatch.AddParameterNVarChar(validObject.DomainPart + "\\" + validObject.NamePart));
          this.AppendSql(",");
          this.AppendSql(DalSqlElement.InlineBit(validObject.LookupAccount));
          this.AppendSql(", default, default, null");
          this.AppendSql(";if ");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.ConstId));
          this.AppendSql(" is null begin return; end;");
          this.AppendSql("insert into ");
          this.AppendSql("@tempIdMap");
          this.AppendSql(" select ");
          this.AppendSql(DalSqlElement.InlineInt(validObject.ConstId - 20000));
          this.AppendSql(",");
          this.AppendSql("@O");
          this.AppendSql(DalSqlElement.InlineInt(-validObject.ConstId));
          this.AppendSql(Environment.NewLine);
        }
      }
    }
  }
}
