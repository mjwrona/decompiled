// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalGetPersonNameElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalGetPersonNameElement : DalSqlElement
  {
    public virtual void JoinBatch(ElementGroup group)
    {
      this.SetOutputs(1);
      this.SetGroup(group);
      if (this.Version < 42)
      {
        this.AppendSql("select DisplayPart from dbo.Constants where SID = ");
        this.AppendSql("@userSID");
      }
      else
      {
        this.AppendSql("select DisplayPart from dbo.Constants where TeamFoundationId = ");
        this.AppendSql("@userVSID");
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
    }

    public virtual string GetPersonName() => (string) this.GetSingleResult();
  }
}
