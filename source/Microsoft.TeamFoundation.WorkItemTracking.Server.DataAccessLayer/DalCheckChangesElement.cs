// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalCheckChangesElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalCheckChangesElement : DalSqlElement
  {
    public void JoinBatch(ElementGroup group, string objectType, bool verbose, bool isBulkUpdate)
    {
      if (this.Version <= 1)
        this.JoinBatchCTP2(group, objectType, verbose, isBulkUpdate);
      else
        this.JoinBatchTip(group, objectType, verbose, isBulkUpdate);
    }

    private void JoinBatchTip(
      ElementGroup group,
      string objectType,
      bool verbose,
      bool isBulkUpdate)
    {
      this.m_elementGroup = group;
      if (verbose)
        this.m_outputs = 2;
      else
        this.m_outputs = 1;
      if (this.Version >= 21)
        ++this.m_outputs;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      this.AppendSql("exec dbo.[");
      this.AppendSql(objectType);
      this.AppendSql("AuthorizeChanges");
      this.AppendSql("] ");
      this.AppendPartitionIdVariable();
      this.AppendSql("@PersonId");
      this.AppendSql(",");
      this.AppendSql("@NowUtc");
      this.AppendSql(",");
      this.AppendSql("@fVerbose");
      this.AppendSql(",");
      this.AppendSql("@fRollback");
      this.AppendSql(" output,");
      this.AppendSql("@bypassRules");
      if (this.Version < 18)
        this.AppendSql(",1");
      if (this.Version < 8)
      {
        this.AppendSql(",");
        this.AppendSql("@userSID");
      }
      this.AppendSql(Environment.NewLine);
      if (isBulkUpdate)
      {
        this.SqlBatch.AddParameterBit(!this.m_update.IsValidBatch, "@fATRollback");
        this.AppendSql("set ");
        this.AppendSql("@fRollback");
        this.AppendSql(" = ");
        this.AppendSql("@fRollback");
        this.AppendSql(" | ");
        this.AppendSql("@fATRollback");
        this.AppendSql(Environment.NewLine);
      }
      this.AppendSql("if (");
      this.AppendSql("@fRollback");
      this.AppendSql(" = 1)");
      this.AppendSql(Environment.NewLine);
      this.AppendSql("begin ");
      this.AppendSql(Environment.NewLine);
      this.AppendSql(" exec dbo.");
      this.AppendSql("GetForceRollbackErrorCode");
      this.AppendSql(" ");
      this.AppendPartitionIdVariable();
      this.AppendSql("@PersonId");
      this.AppendSql(",");
      this.AppendSql("@NowUtc");
      this.AppendSql(",");
      this.AppendSql("@ForceRollbackError");
      this.AppendSql(" output");
      this.AppendSql(Environment.NewLine);
      if (isBulkUpdate)
      {
        this.AppendSql(" if (");
        this.AppendSql("@ForceRollbackError");
        this.AppendSql(" <> ");
        this.AppendSql(DalSqlElement.InlineInt(480000));
        this.AppendSql(")");
        this.AppendSql(Environment.NewLine);
        this.AppendSql(" begin; set ");
        this.AppendSql("@ForceRollbackError");
        this.AppendSql(" = ");
        this.AppendSql(DalSqlElement.InlineInt(600139));
        this.AppendSql(" end; ");
        this.AppendSql(Environment.NewLine);
      }
      this.AppendSql("end");
      this.AppendSql(Environment.NewLine);
      this.AppendSql("exec dbo.");
      this.AppendSql("ForceRollback");
      this.AppendSql(" ");
      this.AppendSql("@fRollback");
      this.AppendSql(", ");
      this.AppendSql("@ForceRollbackError");
      this.AppendSql(Environment.NewLine);
      if (this.m_update.NeedToReleaseWatermark)
        this.AppendSql("if @@trancount=0 \r\nbegin  \r\n    exec dbo.prc_ReleaseWorkItemWatermark @partitionId, @watermark \r\n    return \r\nend");
      else
        this.AppendSql("if @@trancount=0 return");
      this.AppendSql(Environment.NewLine);
    }

    private void JoinBatchCTP2(
      ElementGroup group,
      string objectType,
      bool verbose,
      bool isBulkUpdate)
    {
      this.m_elementGroup = group;
      if (verbose)
        this.m_outputs = 2;
      else
        this.m_outputs = 1;
      this.m_index = group.AddElementToGroup(this.m_outputs);
      this.AppendSql("exec dbo.[");
      this.AppendSql(objectType);
      this.AppendSql("AuthorizeChanges");
      this.AppendSql("] ");
      this.AppendSql("@PersonId");
      this.AppendSql(",");
      this.AppendSql("@NowUtc");
      this.AppendSql(",");
      this.AppendSql("@fVerbose");
      this.AppendSql(",");
      this.AppendSql("@fRollback");
      this.AppendSql(" output,");
      this.AppendSql("@bypassRules");
      this.AppendSql(",");
      this.AppendSql("1");
      this.AppendSql(",");
      this.AppendSql("null");
      this.AppendSql(",");
      this.AppendSql("@userSID");
      this.AppendSql(Environment.NewLine);
      this.AppendSql("if (");
      this.AppendSql("@fRollback");
      this.AppendSql(" = 1)");
      this.AppendSql(Environment.NewLine);
      this.AppendSql("begin ");
      this.AppendSql(Environment.NewLine);
      this.AppendSql(" exec dbo.");
      this.AppendSql("GetForceRollbackErrorCode");
      this.AppendSql(" ");
      this.AppendSql("@PersonId");
      this.AppendSql(",");
      this.AppendSql("@NowUtc");
      this.AppendSql(",");
      this.AppendSql("@ForceRollbackError");
      this.AppendSql(" output");
      this.AppendSql(Environment.NewLine);
      if (isBulkUpdate)
      {
        this.AppendSql(" if (");
        this.AppendSql("@ForceRollbackError");
        this.AppendSql(" <> ");
        this.AppendSql(DalSqlElement.InlineInt(480000));
        this.AppendSql(")");
        this.AppendSql(Environment.NewLine);
        this.AppendSql(" begin; set ");
        this.AppendSql("@ForceRollbackError");
        this.AppendSql(" = ");
        this.AppendSql(DalSqlElement.InlineInt(600139));
        this.AppendSql(" end; ");
        this.AppendSql(Environment.NewLine);
      }
      this.AppendSql("end");
      this.AppendSql(Environment.NewLine);
      this.AppendSql("exec dbo.");
      this.AppendSql("ForceRollback");
      this.AppendSql(" ");
      this.AppendSql("@fRollback");
      this.AppendSql(", ");
      this.AppendSql("@ForceRollbackError");
      this.AppendSql(Environment.NewLine);
      this.AppendSql("if @@trancount=0 return");
      this.AppendSql(Environment.NewLine);
    }
  }
}
