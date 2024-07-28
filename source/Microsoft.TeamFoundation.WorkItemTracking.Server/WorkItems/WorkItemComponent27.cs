// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent27
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent27 : WorkItemComponent26
  {
    protected override SqlParameter BindWorkItemLinkUpdates(
      string parameterName,
      IEnumerable<WorkItemLinkUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemLinkUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemLinkUpdateRecord>) new WorkItemComponent27.WorkItemLinkUpdateTable3RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    protected virtual WorkItemComponent.WorkItemRemoteLinkRecordBinder GetWorkItemRemoteLinkRecordBinder() => new WorkItemComponent.WorkItemRemoteLinkRecordBinder(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier));

    public override IEnumerable<WorkItemLinkUpdateResultRecord> GetPendingWorkItemRemoteLink()
    {
      this.PrepareStoredProcedure("prc_GetPendingWorkItemRemoteLinks");
      return (IEnumerable<WorkItemLinkUpdateResultRecord>) this.ExecuteUnknown<List<WorkItemLinkUpdateResultRecord>>((System.Func<IDataReader, List<WorkItemLinkUpdateResultRecord>>) (reader => this.GetWorkItemRemoteLinkRecordBinder().BindAll(reader).ToList<WorkItemLinkUpdateResultRecord>()));
    }

    protected override WorkItemComponent.WorkItemLinkBinder GetWorkItemLinkBinder(
      IdentityDisplayType identityDisplayType = IdentityDisplayType.ComboDisplayName)
    {
      return (WorkItemComponent.WorkItemLinkBinder) new WorkItemComponent.WorkItemLinkBinder4(identityDisplayType);
    }

    protected override void BindUpdateWorkItemsParameter(
      IVssIdentity userIdentity,
      bool bypassRules,
      bool isAdmin,
      int trendInterval,
      bool dualSave,
      WorkItemUpdateDataset updateDataset,
      int workItemLinksLimit,
      int workItemRemoteLinksLimit)
    {
      base.BindUpdateWorkItemsParameter(userIdentity, bypassRules, isAdmin, trendInterval, dualSave, updateDataset, workItemLinksLimit, workItemRemoteLinksLimit);
      this.BindInt("@workItemLinksLimit", workItemLinksLimit);
      this.BindInt("@workItemRemoteLinksLimit", workItemLinksLimit);
    }

    public override void UpdatePendingWorkItemRemoteLink(
      IEnumerable<WorkItemLinkUpdateRecord> updates)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemRemoteLinks");
      this.BindWorkItemLinkUpdates("@workItemRemoteLinkUpdates", updates);
      this.ExecuteNonQuery();
    }

    protected override List<WorkItemLinkInfo> FilterWorkItemLinksWithAsOf(
      List<WorkItemLinkInfo> workItemLinks,
      DateTime? asOf)
    {
      return workItemLinks;
    }

    internal override WorkItemComponent.UpdateWorkItemsResultsReader GetUpdateWorkItemsResultsReader(
      bool bypassRules,
      bool isAdmin,
      WorkItemUpdateDataset updateDataset)
    {
      return (WorkItemComponent.UpdateWorkItemsResultsReader) new WorkItemComponent27.UpdateWorkItemsResultsReader5(bypassRules, isAdmin, updateDataset);
    }

    protected class WorkItemLinkUpdateTable3RecordBinder : 
      WorkItemComponent11.WorkItemLinkUpdateTable2RecordBinder
    {
      protected static readonly SqlMetaData[] s_metadata3 = ((IEnumerable<SqlMetaData>) WorkItemComponent11.WorkItemLinkUpdateTable2RecordBinder.s_metadata2).Concat<SqlMetaData>((IEnumerable<SqlMetaData>) new SqlMetaData[5]
      {
        new SqlMetaData("RemoteHostId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("RemoteProjectId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("RemoteStatus", SqlDbType.TinyInt),
        new SqlMetaData("RemoteStatusMessage", SqlDbType.NVarChar, SqlMetaData.Max),
        new SqlMetaData("RemoteWatermark", SqlDbType.BigInt)
      }).ToArray<SqlMetaData>();

      public override string TypeName => "typ_WorkItemLinkUpdateTable3";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent27.WorkItemLinkUpdateTable3RecordBinder.s_metadata3;

      public WorkItemLinkUpdateTable3RecordBinder(System.Func<Guid, int> dataspaceResolver)
        : base(dataspaceResolver)
      {
      }

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemLinkUpdateRecord update)
      {
        base.SetRecordValues(record, update);
        if (update.IsRemote)
        {
          record.SetInt32(2, update.SourceProjectId != Guid.Empty ? this.m_dataspaceResolver(update.SourceProjectId) : 0);
          record.SetInt32(5, update.TargetProjectId != Guid.Empty ? this.m_dataspaceResolver(update.TargetProjectId) : 0);
        }
        record.SetNullableGuid(11, update.RemoteHostId);
        record.SetNullableGuid(12, update.RemoteProjectId);
        WorkItemTrackingSqlDataRecord record1 = record;
        RemoteStatus? remoteStatus = update.RemoteStatus;
        byte? nullable = remoteStatus.HasValue ? new byte?((byte) remoteStatus.GetValueOrDefault()) : new byte?();
        record1.SetNullableByte(13, nullable);
        record.SetNullableString(14, update.RemoteStatusMessage);
        record.SetNullableInt64(15, update.RemoteWatermark);
      }
    }

    internal class UpdateWorkItemsResultsReader5 : WorkItemComponent22.UpdateWorkItemsResultsReader4
    {
      internal UpdateWorkItemsResultsReader5(
        bool bypassRules,
        bool isAdmin,
        WorkItemUpdateDataset updateDataset)
        : base(bypassRules, isAdmin, updateDataset)
      {
      }

      protected override bool CheckAndReadWorkItemLinkUpdates(IDataReader reader)
      {
        if (this.updateDataset.WorkItemLinkUpdates.Any<WorkItemLinkUpdateRecord>())
        {
          reader.NextResult();
          this.result.LinkUpdateResults = (IEnumerable<WorkItemLinkUpdateResultRecord>) WorkItemTrackingResourceComponent.Bind<WorkItemLinkUpdateResultRecord>(reader, (System.Func<IDataReader, WorkItemLinkUpdateResultRecord>) (r => new WorkItemLinkUpdateResultRecord()
          {
            Order = r.GetInt32(0),
            UpdateType = (LinkUpdateType) r.GetByte(1),
            UpdateTypeExecuted = r.IsDBNull(2) ? new LinkUpdateType?() : new LinkUpdateType?((LinkUpdateType) r.GetByte(2)),
            Status = r.GetInt32(3),
            SourceId = r.GetInt32(4),
            TargetId = r.GetInt32(5),
            DataspaceId = r.GetInt32(6),
            TargetDataspaceId = r.GetInt32(7),
            LinkType = r.GetInt32(8),
            Timestamp = r.GetInt64(10),
            RemoteHostId = r.IsDBNull(11) ? new Guid?() : new Guid?(r.GetGuid(11)),
            RemoteProjectId = r.IsDBNull(12) ? new Guid?() : new Guid?(r.GetGuid(12)),
            RemoteStatus = r.IsDBNull(13) ? new RemoteStatus?() : new RemoteStatus?((RemoteStatus) r.GetByte(13)),
            RemoteStatusMessage = r.IsDBNull(14) ? (string) null : r.GetString(14),
            RemoteWatermark = r.IsDBNull(15) ? new long?() : new long?(r.GetInt64(15))
          })).ToArray<WorkItemLinkUpdateResultRecord>();
          if (this.isMaxAddedLinksCountReturned)
          {
            reader.NextResult();
            if (reader.Read())
              this.result.MaxAddedLinksCount = reader.GetInt32(0);
          }
          if (this.result.LinkUpdateResults.Any<WorkItemLinkUpdateResultRecord>((System.Func<WorkItemLinkUpdateResultRecord, bool>) (wlur => wlur.Status != 0)))
            return false;
        }
        return true;
      }
    }
  }
}
