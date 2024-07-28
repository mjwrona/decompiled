// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent19
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent19 : GroupComponent18
  {
    public override IReadOnlyList<GroupAuditRecord> GetGroupAuditRecords(
      long startSequenceIdInclusive,
      long endSequenceIdInclusive)
    {
      this.PrepareStoredProcedure("prc_GetGroupAuditRecords");
      this.BindLong("@startSequenceIdInclusive", startSequenceIdInclusive);
      this.BindLong("@endSequenceIdInclusive", endSequenceIdInclusive);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<GroupAuditRecord>((ObjectBinder<GroupAuditRecord>) new GroupComponent19.GroupAuditRecordColumns());
        List<GroupAuditRecord> groupAuditRecords = new List<GroupAuditRecord>();
        foreach (GroupAuditRecord groupAuditRecord in resultCollection.GetCurrent<GroupAuditRecord>())
          groupAuditRecords.Add(groupAuditRecord);
        return (IReadOnlyList<GroupAuditRecord>) groupAuditRecords;
      }
    }

    protected class GroupAuditRecordColumns : ObjectBinder<GroupAuditRecord>
    {
      private SqlColumnBinder SequenceId = new SqlColumnBinder(nameof (SequenceId));
      private SqlColumnBinder MemberId = new SqlColumnBinder(nameof (MemberId));
      private SqlColumnBinder ContainerId = new SqlColumnBinder(nameof (ContainerId));
      private SqlColumnBinder Active = new SqlColumnBinder(nameof (Active));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));

      protected override GroupAuditRecord Bind() => new GroupAuditRecord(this.SequenceId.GetInt64((IDataReader) this.Reader), this.MemberId.GetGuid((IDataReader) this.Reader, true), this.ContainerId.GetGuid((IDataReader) this.Reader), this.Active.GetBoolean((IDataReader) this.Reader), this.LastUpdated.GetDateTime((IDataReader) this.Reader));
    }
  }
}
