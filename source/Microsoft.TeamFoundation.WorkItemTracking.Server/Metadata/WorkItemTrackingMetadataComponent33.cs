// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent33
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent33 : WorkItemTrackingMetadataComponent32
  {
    protected virtual WorkItemTrackingMetadataComponent33.ConstantAuditEntryBinder GetConstantAuditEntryBinder() => new WorkItemTrackingMetadataComponent33.ConstantAuditEntryBinder();

    public override IEnumerable<ConstantAuditEntry> GetDuplicateIdentityConstants()
    {
      this.PrepareStoredProcedure("prc_GetDuplicateIdentityConstants");
      this.BindBoolean("@isHosted", this.RequestContext.ExecutionEnvironment.IsHostedDeployment);
      return this.ExecuteUnknown<IEnumerable<ConstantAuditEntry>>((System.Func<IDataReader, IEnumerable<ConstantAuditEntry>>) (reader => (IEnumerable<ConstantAuditEntry>) this.GetConstantAuditEntryBinder().BindAll(reader).ToList<ConstantAuditEntry>()));
    }

    public override IEnumerable<ConstantRecord> GetConstantRecords(
      IEnumerable<int> constantIds,
      bool includeInactiveConstants = false)
    {
      IEnumerable<int> rows = constantIds.Distinct<int>();
      this.PrepareStoredProcedure("prc_GetConstantRecords");
      this.BindInt32Table("@constantIds", rows);
      this.BindBoolean("@includeInactiveConstants", includeInactiveConstants);
      return (IEnumerable<ConstantRecord>) this.ExecuteUnknown<List<ConstantRecord>>((System.Func<IDataReader, List<ConstantRecord>>) (reader => new WorkItemTrackingMetadataComponent.ConstantRecordBinder().BindAll(reader).ToList<ConstantRecord>()));
    }

    protected class ConstantAuditEntryBinder : WorkItemTrackingObjectBinder<ConstantAuditEntry>
    {
      private SqlColumnBinder TeamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");
      private SqlColumnBinder ConstIdColumn = new SqlColumnBinder("ConstId");
      private SqlColumnBinder DomainPartColumn = new SqlColumnBinder("DomainPart");
      private SqlColumnBinder NamePartColumn = new SqlColumnBinder("NamePart");
      private SqlColumnBinder DisplayPartColumn = new SqlColumnBinder("DisplayPart");
      private SqlColumnBinder ChangerIdColumn = new SqlColumnBinder("ChangerId");
      private SqlColumnBinder AddedDateColumn = new SqlColumnBinder("AddedDate");
      private SqlColumnBinder RemovedDateColumn = new SqlColumnBinder("RemovedDate");
      private SqlColumnBinder SIDColumn = new SqlColumnBinder("SID");

      public override ConstantAuditEntry Bind(IDataReader reader) => new ConstantAuditEntry()
      {
        TeamFoundationId = this.TeamFoundationIdColumn.GetGuid(reader, true),
        ConstId = this.ConstIdColumn.GetInt32(reader),
        DomainPart = this.DomainPartColumn.GetString(reader, false),
        NamePart = this.NamePartColumn.GetString(reader, false),
        DisplayPart = this.DisplayPartColumn.GetString(reader, false),
        ChangerId = this.ChangerIdColumn.GetInt32(reader),
        AddedDate = this.AddedDateColumn.GetDateTime(reader),
        RemovedDate = this.RemovedDateColumn.GetDateTime(reader),
        SID = this.SIDColumn.GetString(reader, true)
      };
    }
  }
}
