// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalApplyIssueChangesElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalApplyIssueChangesElement : DalSqlElement
  {
    public void JoinBatch(ElementGroup group, string objectType)
    {
      this.SetOutputs(1);
      this.SetGroup(group);
      if (this.Version >= 24)
      {
        this.AppendSql("declare @watermark int; exec ");
        this.AppendSql("@status");
        this.AppendSql(" = dbo.prc_iCounterGetNext ");
        this.AppendPartitionIdVariable();
        this.AppendSql(" 'WIT_WorkItemWatermark', 1, @watermark output; if ");
        this.AppendSql("@status");
        this.AppendSql(" <> 0 begin rollback transaction; return; end");
        this.AppendSql(Environment.NewLine);
      }
      this.AppendSql(Environment.NewLine);
      this.AppendSql("exec dbo.[");
      this.AppendSql(objectType);
      this.AppendSql("ApplyChanges");
      this.AppendSql("] ");
      this.AppendPartitionIdVariable();
      this.AppendSql("@PersonId");
      this.AppendSql(",");
      this.AppendSql("@NowUtc");
      this.AppendSql(",");
      this.AppendSql("null");
      if (this.Version >= 18)
      {
        this.AppendSql(", 1, ");
        this.AppendSql("@tempIdMap");
        this.AppendSql(", ");
        this.AppendSql("@personIdMap");
        this.AppendSql(", ");
        this.AppendSql("@customFieldValues");
        if (this.Version >= 22)
          this.AppendSql(", @watermark");
      }
      this.AppendSql(Environment.NewLine);
      if (this.Version < 22)
      {
        this.AppendSql("declare @watermark int; exec ");
        this.AppendSql("@status");
        this.AppendSql(" = dbo.prc_iCounterGetNext ");
        this.AppendPartitionIdVariable();
        this.AppendSql(" 'WIT_WorkItemWatermark', 0, @watermark output; if ");
        this.AppendSql("@status");
        this.AppendSql(" <> 0 begin rollback transaction; return; end");
        this.AppendSql(Environment.NewLine);
        this.AppendSql("set @watermark = @watermark - 1");
        this.AppendSql(Environment.NewLine);
      }
      this.AppendSql("select ");
      this.AppendSql("@PersonId");
      this.AppendSql(" as [");
      this.AppendSql("System.PersonId");
      this.AppendSql("], ");
      this.AppendSql("@NowUtc");
      this.AppendSql(" as [");
      this.AppendSql("System.AuthorizedDate");
      this.AppendSql("], @watermark as [");
      this.AppendSql("System.Watermark");
      this.AppendSql("]");
      this.AppendSql(Environment.NewLine);
    }
  }
}
