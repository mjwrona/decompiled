// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalPersonIdElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalPersonIdElement : DalSqlElement
  {
    public virtual void JoinBatch(ElementGroup group, IVssIdentity user)
    {
      this.SetOutputs(0);
      this.SetGroup(group);
      this.SqlBatch.AddParameterNVarChar(user.Descriptor.Identifier, "@userSID");
      if (this.Version >= 42)
        this.SqlBatch.AddParameterUniqueIdentifier(user.Id, "@userVSID");
      this.AppendSql("declare ");
      this.AppendSql("@PersonId");
      this.AppendSql(" as int");
      this.AppendSql(Environment.NewLine);
      this.AppendSql("declare @rebuildOK as int");
      this.AppendSql(Environment.NewLine);
      if (this.Version < 42)
      {
        this.AppendSql("exec @rebuildOK=dbo.");
        this.AppendSql("RebuildCallersViews");
        this.AppendSql(" ");
        this.AppendPartitionIdVariable();
        this.AppendSql("@PersonId");
        this.AppendSql(" output,");
        this.AppendSql("@userSID");
      }
      else
      {
        this.AppendSql("exec @rebuildOK=prc_GetConstIDFromTeamFoundationId");
        this.AppendSql(" ");
        this.AppendSql("@userVSID");
        this.AppendSql(",");
        this.AppendPartitionIdVariable();
        this.AppendSql("@PersonId");
        this.AppendSql(" output");
      }
      this.AppendSql(Environment.NewLine);
      this.AppendSql("if @rebuildOK<>0 return");
      this.AppendSql(Environment.NewLine);
    }
  }
}
