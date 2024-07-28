// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent28
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
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
  internal class WorkItemComponent28 : WorkItemComponent27
  {
    protected override SqlParameter BindWorkItemLinkUpdates(
      string parameterName,
      IEnumerable<WorkItemLinkUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemLinkUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemLinkUpdateRecord>) new WorkItemComponent28.WorkItemLinkUpdateTable4RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    public override (int workItemCount, int revisionCount) BackfillCommentCount(int batchSize)
    {
      this.PrepareStoredProcedure("prc_UpdateCommentCountOnWorkItems", 0);
      this.BindInt("@batchSize", batchSize);
      return this.ExecuteUnknown<(int, int)>((System.Func<IDataReader, (int, int)>) (reader =>
      {
        (int, int) valueTuple = (0, 0);
        if (reader.Read())
          valueTuple = (reader.GetInt32(0), reader.GetInt32(1));
        return valueTuple;
      }));
    }

    public override IEnumerable<int> DestroyWorkItems(
      IVssIdentity userIdentity,
      IEnumerable<int> workItemIds,
      int batchSize = 200)
    {
      this.PrepareStoredProcedure("prc_DestroyWorkItems", 3600);
      this.BindIdentityColumn(userIdentity, "@userSid");
      this.BindInt32Table("@workItemIds", workItemIds);
      this.BindInt("@batchSize", batchSize);
      return (IEnumerable<int>) WorkItemTrackingResourceComponent.Bind<int>(this.ExecuteReader(), (System.Func<IDataReader, int>) (reader => reader.GetInt32(0))).ToArray<int>();
    }

    public override void ProvisionSystemField(
      string refName,
      string localizedFieldName,
      int fieldId,
      int type,
      int reportingType,
      out string duplicateFieldRefName)
    {
      duplicateFieldRefName = (string) null;
      this.PrepareStoredProcedure("prc_ProvisionSystemField");
      this.BindString("@refName", refName, 128, false, SqlDbType.NVarChar);
      this.BindString("@localizedFieldName", localizedFieldName, 128, false, SqlDbType.NVarChar);
      this.BindInt("@fieldId", fieldId);
      this.BindInt("@type", type);
      this.BindInt("@reportingType", reportingType);
      IDataReader reader = this.ExecuteReader();
      if (!reader.Read() || new SqlColumnBinder("fDeleted").GetBoolean(reader))
        return;
      SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("ReferenceName");
      duplicateFieldRefName = sqlColumnBinder.GetString(reader, true);
    }

    protected override WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset> GetWorkItemDataSetBinder(
      bool bindTitle,
      bool bindCountFields,
      IdentityDisplayType identityDisplayType)
    {
      return (WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset>) new WorkItemComponent.WorkItemDatasetBinder6<WorkItemDataset>(bindTitle, bindCountFields, identityDisplayType, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier), this.RequestContext.WitContext().FieldDictionary);
    }

    protected override WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues> GetWorkItemFieldValuesBinder(
      IEnumerable<int> wideTableFields,
      IdentityDisplayType identityDisplayType,
      bool disableProjectionLevelThree)
    {
      return (WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValuesBinder6<WorkItemFieldValues>(wideTableFields, identityDisplayType, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier), disableProjectionLevelThree, this.RequestContext.WitContext().FieldDictionary);
    }

    protected override WorkItemComponent.WorkItemLinkBinder GetWorkItemLinkBinder(
      IdentityDisplayType identityDisplayType = IdentityDisplayType.ComboDisplayName)
    {
      return (WorkItemComponent.WorkItemLinkBinder) new WorkItemComponent.WorkItemLinkBinder5(identityDisplayType);
    }

    protected class WorkItemLinkUpdateTable4RecordBinder : 
      WorkItemComponent27.WorkItemLinkUpdateTable3RecordBinder
    {
      protected static readonly SqlMetaData[] s_metadata4 = ((IEnumerable<SqlMetaData>) WorkItemComponent27.WorkItemLinkUpdateTable3RecordBinder.s_metadata3).Concat<SqlMetaData>((IEnumerable<SqlMetaData>) new SqlMetaData[1]
      {
        new SqlMetaData("TimeStamp", SqlDbType.BigInt)
      }).ToArray<SqlMetaData>();

      public override string TypeName => "typ_WorkItemLinkUpdateTable4";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent28.WorkItemLinkUpdateTable4RecordBinder.s_metadata4;

      public WorkItemLinkUpdateTable4RecordBinder(System.Func<Guid, int> dataspaceResolver)
        : base(dataspaceResolver)
      {
      }

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemLinkUpdateRecord update)
      {
        base.SetRecordValues(record, update);
        record.SetNullableInt64(16, update.TimeStamp);
      }
    }
  }
}
