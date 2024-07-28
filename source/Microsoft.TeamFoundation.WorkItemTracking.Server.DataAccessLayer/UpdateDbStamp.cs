// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.UpdateDbStamp
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class UpdateDbStamp : DalSqlElement
  {
    public void StampDb(IVssIdentity user)
    {
      this.AppendSql("declare ");
      this.AppendSql("@PersonId");
      this.AppendSql(" as int;");
      this.AppendSql(Environment.NewLine);
      this.AppendSql("select ");
      this.AppendSql("@PersonId");
      this.AppendSql("= C.ConstID from dbo.Constants C with (nolock)");
      this.AppendSql(" where ");
      if (this.Version < 42)
      {
        this.AppendSql("C.SID = ");
        this.AppendSql(this.SqlBatch.AddParameterNVarChar(user.Descriptor.Identifier));
        this.AppendSql(" COLLATE DATABASE_DEFAULT");
      }
      else
      {
        this.AppendSql("C.TeamFoundationId = ");
        this.AppendSql(this.SqlBatch.AddParameterUniqueIdentifier(user.Id));
      }
      if (this.IsSchemaPartitioned)
      {
        this.AppendSql(" and ");
        this.AppendSql("PartitionId");
        this.AppendSql("=");
        this.AppendSql("@partitionId");
        this.AppendSql(" OPTION (OPTIMIZE FOR (");
        this.AppendSql("@partitionId");
        this.AppendSql(" UNKNOWN))");
      }
      this.AppendSql(Environment.NewLine);
      this.AppendSql("if not exists(select * from dbo.ConstantDownWard(");
      this.AppendPartitionIdVariable();
      this.AppendSql("0, 0, 0, ");
      this.AppendSql(Convert.ToString(-30, (IFormatProvider) CultureInfo.InvariantCulture));
      this.AppendSql(", null, null)");
      this.AppendSql("          where ConstID = @PersonId)");
      this.AppendSql("    begin EXEC [dbo].[RaiseWITError] " + 600081.ToString() + ", 11,1");
      this.AppendSql("    return end ");
      this.AppendSql(Environment.NewLine);
      this.AppendSql("exec dbo.[");
      this.AppendSql(nameof (StampDb));
      this.AppendSql("] ");
      this.AppendPartitionIdVariable(false);
      this.AppendSql(Environment.NewLine);
      this.SqlBatch.ExecuteBatch();
    }
  }
}
