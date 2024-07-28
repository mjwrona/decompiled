// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.HistoryComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  internal class HistoryComponent : WorkItemTrackingResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[13]
    {
      (IComponentCreator) new ComponentCreator<HistoryComponent>(1),
      (IComponentCreator) new ComponentCreator<HistoryComponent2>(2),
      (IComponentCreator) new ComponentCreator<HistoryComponent3>(3),
      (IComponentCreator) new ComponentCreator<HistoryComponent4>(4),
      (IComponentCreator) new ComponentCreator<HistoryComponent5>(5),
      (IComponentCreator) new ComponentCreator<HistoryComponent6>(6),
      (IComponentCreator) new ComponentCreator<HistoryComponent7>(7),
      (IComponentCreator) new ComponentCreator<HistoryComponent8>(8),
      (IComponentCreator) new ComponentCreator<HistoryComponent9>(9),
      (IComponentCreator) new ComponentCreator<HistoryComponent10>(10),
      (IComponentCreator) new ComponentCreator<HistoryComponent11>(11),
      (IComponentCreator) new ComponentCreator<HistoryComponent12>(12),
      (IComponentCreator) new ComponentCreator<HistoryComponent13>(13)
    }, "WorkItemWarehouse", "WorkItem");

    public virtual IEnumerable<WorkItemFieldHistoryRecord> GetFieldHistory(long timeStamp)
    {
      this.PrepareStoredProcedure("prc_GetFieldHistory");
      this.BindLong("@timeStamp", timeStamp);
      return (IEnumerable<WorkItemFieldHistoryRecord>) this.ExecuteUnknown<IOrderedEnumerable<WorkItemFieldHistoryRecord>>((System.Func<IDataReader, IOrderedEnumerable<WorkItemFieldHistoryRecord>>) (reader => new HistoryComponent.WorkItemFieldHistoryRecordBinder().BindAll(reader).OrderBy<WorkItemFieldHistoryRecord, long>((System.Func<WorkItemFieldHistoryRecord, long>) (hr => hr.Timestamp))));
    }

    public virtual IEnumerable<WorkItemDestroyHistoryRecord> GetWorkItemDestroyHistory(
      long timeStamp,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemDestroyHistory");
      this.BindLong("@timeStamp", timeStamp);
      this.BindInt("@batchSize", batchSize);
      return this.ExecuteUnknown<IEnumerable<WorkItemDestroyHistoryRecord>>((System.Func<IDataReader, IEnumerable<WorkItemDestroyHistoryRecord>>) (reader => new HistoryComponent.WorkItemDestroyHistoryRecordBinder().BindAll(reader)));
    }

    public virtual IEnumerable<WorkItemLinkHistoryRecord> GetWorkItemLinkHistory(
      long timeStamp,
      int batchSize,
      DateTime? startDate = null)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemLinkHistory");
      this.BindLong("@timeStamp", timeStamp);
      this.BindInt("@batchSize", batchSize);
      if (startDate.HasValue)
        this.BindDateTime("@startDate", startDate.Value);
      return this.ExecuteUnknown<IEnumerable<WorkItemLinkHistoryRecord>>((System.Func<IDataReader, IEnumerable<WorkItemLinkHistoryRecord>>) (reader => new HistoryComponent.WorkItemLinkHistoryRecordBinder().BindAll(reader)));
    }

    public virtual IEnumerable<WorkItemLinkTypeHistoryRecord> GetWorkItemLinkTypeHistory(
      long timeStamp)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemLinkTypeHistory");
      this.BindLong("@timeStamp", timeStamp);
      return this.ExecuteUnknown<IEnumerable<WorkItemLinkTypeHistoryRecord>>((System.Func<IDataReader, IEnumerable<WorkItemLinkTypeHistoryRecord>>) (reader => new HistoryComponent.WorkItemLinkTypeHistoryRecordBinder().BindAll(reader)));
    }

    public virtual IEnumerable<WorkItemTypeCategoryHistoryRecord> GetWorkItemTypeCategoryHistory(
      long timeStamp)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeCategoryHistory");
      this.BindLong("@timeStamp", timeStamp);
      return this.ExecuteUnknown<IEnumerable<WorkItemTypeCategoryHistoryRecord>>((System.Func<IDataReader, IEnumerable<WorkItemTypeCategoryHistoryRecord>>) (reader => new HistoryComponent.WorkItemTypeCategoryHistoryRecordBinder().BindAll(reader)));
    }

    public virtual IEnumerable<WorkItemTypeCategoryMemberHistoryRecord> GetWorkItemTypeCategoryMemberHistory(
      long timeStamp)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeCategoryMemberHistory");
      this.BindLong("@timeStamp", timeStamp);
      return this.ExecuteUnknown<IEnumerable<WorkItemTypeCategoryMemberHistoryRecord>>((System.Func<IDataReader, IEnumerable<WorkItemTypeCategoryMemberHistoryRecord>>) (reader => new HistoryComponent.WorkItemTypeCategoryMemberHistoryRecordBinder().BindAll(reader)));
    }

    public virtual IEnumerable<WorkItemTypeRenameHistoryRecord> GetWorkItemTypeRenameHistory(
      long timeStamp)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeRenameHistory");
      this.BindLong("@timeStamp", timeStamp);
      return this.ExecuteUnknown<IEnumerable<WorkItemTypeRenameHistoryRecord>>((System.Func<IDataReader, IEnumerable<WorkItemTypeRenameHistoryRecord>>) (reader => new HistoryComponent.WorkItemTypeRenameHistoryRecordBinder().BindAll(reader)));
    }

    public virtual IEnumerable<WorkItemIdRevisionPair> GetChangedWorkItems(
      int watermark,
      int batchSize,
      DateTime? startDate = null)
    {
      this.PrepareStoredProcedure("prc_GetChangedWorkItems");
      this.BindInt("@watermark", watermark);
      this.BindInt("@batchSize", batchSize);
      if (startDate.HasValue)
        this.BindDateTime("@startDate", startDate.Value);
      return this.ExecuteUnknown<IEnumerable<WorkItemIdRevisionPair>>((System.Func<IDataReader, IEnumerable<WorkItemIdRevisionPair>>) (reader => new HistoryComponent.WorkItemIdRevisionPairBinder().BindAll(reader)));
    }

    public virtual IEnumerable<WorkItemIdRevisionPair> GetChangedWorkItems(
      int watermark,
      int batchSize,
      Guid? projectId,
      IEnumerable<string> types = null,
      DateTime? startDate = null)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<WorkItemIdRevisionPair> GetChangedWorkItemsPageable(
      WorkItemIdRevisionPair workItemIdRevPair,
      int batchSize,
      Guid? projectId,
      IEnumerable<string> types = null,
      bool includeLatestOnly = false,
      bool includeDiscussionChangesOnly = false,
      bool useLegacyDiscussionChanges = false,
      bool includeDiscussionHistory = false)
    {
      return this.GetChangedWorkItems(workItemIdRevPair.Watermark, batchSize, projectId, types);
    }

    public virtual WorkItemIdRevisionPair GetWorkItemWatermarkForDate(DateTime date) => new WorkItemIdRevisionPair();

    private void PrepareDynamicGetWorkItemResourceLinksByIdRevsProc(
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      bool includeExtentedProperties)
    {
      workItemIdRevPairs = (IEnumerable<WorkItemIdRevisionPair>) workItemIdRevPairs.Distinct<WorkItemIdRevisionPair>().ToArray<WorkItemIdRevisionPair>();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("\r\n            SELECT \r\n               Files.ExtID           AS ResourceId\r\n             , Files.ID              AS SourceId\r\n             , Files.FldID           AS ResourceType\r\n             , Files.AddedDate       AS AuthorizedDate\r\n             , Files.RemovedDate     AS RevisedDate\r\n             , Files.FilePath        AS Location\r\n             , Files.OriginalName    AS Name");
      if (includeExtentedProperties)
        stringBuilder.Append("\r\n             , Files.Comment         AS Comment\r\n             , Files.CreationDate    AS CreatedDate\r\n             , Files.LastWriteDate   AS ModifiedDate\r\n             , Files.Length          AS Length");
      stringBuilder.Append("\r\n            FROM @workItemIdRevPairs Ids\r\n            INNER LOOP JOIN tbl_WorkItemCoreLatest WCore\r\n                ON WCore.PartitionId = @partitionId\r\n                    AND WCore.Id = Ids.Id\r\n                    AND WCore.Rev = Ids.Rev\r\n            INNER LOOP JOIN dbo.WorkItemFiles Files\r\n                ON Files.PartitionId = @partitionId\r\n                   AND Files.Id = Ids.Id\r\n                   AND Files.AddedDate = WCore.AuthorizedDate\r\n            UNION\r\n            SELECT\r\n              Files.ExtID            AS ResourceId\r\n             , Files.ID              AS SourceId\r\n             , Files.FldID           AS ResourceType\r\n             , Files.AddedDate       AS AuthorizedDate\r\n             , Files.RemovedDate     AS RevisedDate\r\n             , Files.FilePath        AS Location\r\n             , Files.OriginalName    AS Name");
      if (includeExtentedProperties)
        stringBuilder.Append("\r\n             , Files.Comment         AS Comment\r\n             , Files.CreationDate    AS CreatedDate\r\n             , Files.LastWriteDate   AS ModifiedDate\r\n             , Files.Length          AS Length");
      stringBuilder.Append("\r\n            FROM @workItemIdRevPairs Ids\r\n            INNER LOOP JOIN tbl_WorkItemCoreWere WCore\r\n                ON WCore.PartitionId = @partitionId\r\n                    AND WCore.Id = Ids.Id\r\n                    AND WCore.Rev = Ids.Rev\r\n            INNER LOOP JOIN dbo.WorkItemFiles Files\r\n                ON Files.PartitionId = @partitionId\r\n                   AND Files.Id = Ids.Id\r\n                   AND Files.AddedDate = WCore.AuthorizedDate\r\n            UNION\r\n            SELECT\r\n              Files.ExtID            AS ResourceId\r\n             , Files.ID              AS SourceId\r\n             , Files.FldID           AS ResourceType\r\n             , Files.AddedDate       AS AuthorizedDate\r\n             , Files.RemovedDate     AS RevisedDate\r\n             , Files.FilePath        AS Location\r\n             , Files.OriginalName    AS Name");
      if (includeExtentedProperties)
        stringBuilder.Append("\r\n             , Files.Comment         AS Comment\r\n             , Files.CreationDate    AS CreatedDate\r\n             , Files.LastWriteDate   AS ModifiedDate\r\n             , Files.Length          AS Length");
      stringBuilder.Append("\r\n            FROM @workItemIdRevPairs Ids\r\n            INNER LOOP JOIN tbl_WorkItemCoreWere WCore\r\n                ON WCore.PartitionId = @partitionId\r\n                    AND WCore.Id = Ids.Id\r\n                    AND WCore.Rev = Ids.Rev\r\n            INNER LOOP JOIN dbo.WorkItemFiles Files\r\n                ON Files.PartitionId = @partitionId\r\n                   AND Files.Id = Ids.Id\r\n                   AND Files.RemovedDate = WCore.RevisedDate\r\n            OPTION(OPTIMIZE FOR(@partitionId UNKNOWN))");
      this.PrepareDynamicProcedure("dynprc_getWorkItemResourceLinksByIdRevs", stringBuilder.ToString());
      this.BindWorkItemIdRevPairs("@workItemIdRevPairs", workItemIdRevPairs);
    }

    private void PrepareDynamicGetWorkItemResourceLinksByIdsProc(
      IEnumerable<int> workItemIds,
      bool includeExtentedProperties)
    {
      workItemIds = (IEnumerable<int>) workItemIds.Distinct<int>().ToArray<int>();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("\r\n            SELECT \r\n              Files.ExtID\t\t\tAS ResourceId\r\n             ,Files.ID\t\t\t\tAS SourceId\r\n             ,Files.FldID\t\t\tAS ResourceType\r\n             ,Files.AddedDate\t\tAS AuthorizedDate\r\n             ,Files.RemovedDate\t\tAS RevisedDate\r\n             ,Files.FilePath\t\tAS Location\r\n             ,Files.OriginalName\tAS Name");
      if (includeExtentedProperties)
        stringBuilder.Append("\r\n             ,Files.Comment\t\t\tAS Comment\r\n             ,Files.CreationDate\tAS CreatedDate\r\n             ,Files.LastWriteDate\tAS ModifiedDate\r\n             ,Files.Length\t\t\tAS Length");
      stringBuilder.Append("\r\n            FROM @workItemIds Ids \r\n            INNER LOOP JOIN dbo.WorkItemFiles Files\r\n                ON Files.PartitionId = @partitionId\r\n                   AND Files.Id = Ids.Val\r\n            OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n            ");
      this.PrepareDynamicProcedure("dynprc_getWorkItemResourceLinksByIds", stringBuilder.ToString());
      this.BindInt32Table("@workItemIds", workItemIds);
    }

    public virtual IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinks(
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs,
      bool includeExtendedProperties)
    {
      this.PrepareDynamicGetWorkItemResourceLinksByIdRevsProc(workItemIdRevPairs, includeExtendedProperties);
      return this.ExecuteUnknown<IReadOnlyCollection<WorkItemResourceLinkInfo>>((System.Func<IDataReader, IReadOnlyCollection<WorkItemResourceLinkInfo>>) (reader => (IReadOnlyCollection<WorkItemResourceLinkInfo>) new HistoryComponent.WorkItemResourceLinkBinder(includeExtendedProperties).BindAll(reader).ToList<WorkItemResourceLinkInfo>()));
    }

    public virtual IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinks(
      IEnumerable<int> workItemIds,
      bool includeExtendedProperties)
    {
      this.PrepareDynamicGetWorkItemResourceLinksByIdsProc(workItemIds, includeExtendedProperties);
      return this.ExecuteUnknown<IReadOnlyCollection<WorkItemResourceLinkInfo>>((System.Func<IDataReader, IReadOnlyCollection<WorkItemResourceLinkInfo>>) (reader => (IReadOnlyCollection<WorkItemResourceLinkInfo>) new HistoryComponent.WorkItemResourceLinkBinder(includeExtendedProperties).BindAll(reader).ToList<WorkItemResourceLinkInfo>()));
    }

    public virtual IReadOnlyCollection<WorkItemResourceLinkInfo> GetWorkItemResourceLinksByResourceIds(
      IEnumerable<KeyValuePair<int, int>> workItemIdResourceIdPairs,
      bool includeExtendedProperties)
    {
      throw new NotImplementedException();
    }

    public virtual int? GetWorkItemMinWatermark(int fieldId) => new int?();

    protected class WorkItemFieldHistoryRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemFieldHistoryRecord>
    {
      private SqlColumnBinder FieldDataType = new SqlColumnBinder(nameof (FieldDataType));
      private SqlColumnBinder FieldId = new SqlColumnBinder(nameof (FieldId));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder ReferenceName = new SqlColumnBinder(nameof (ReferenceName));
      private SqlColumnBinder ReportingReferenceName = new SqlColumnBinder(nameof (ReportingReferenceName));
      private SqlColumnBinder ReportingName = new SqlColumnBinder(nameof (ReportingName));
      private SqlColumnBinder ReportingFormula = new SqlColumnBinder(nameof (ReportingFormula));
      private SqlColumnBinder Timestamp = new SqlColumnBinder(nameof (Timestamp));
      private SqlColumnBinder ReportingType = new SqlColumnBinder(nameof (ReportingType));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));

      public override WorkItemFieldHistoryRecord Bind(IDataReader reader) => new WorkItemFieldHistoryRecord()
      {
        FieldDataType = this.FieldDataType.GetInt32(reader),
        FieldId = this.FieldId.GetInt32(reader),
        IsDeleted = this.IsDeleted.GetBoolean(reader),
        Name = this.Name.GetString(reader, false),
        ReferenceName = this.ReferenceName.GetString(reader, false),
        ReportingFormula = this.ReportingFormula.GetInt32(reader),
        ReportingName = this.ReportingName.GetString(reader, false),
        ReportingReferenceName = this.ReportingReferenceName.GetString(reader, false),
        ReportingType = this.ReportingType.GetInt32(reader),
        Timestamp = this.Timestamp.GetInt64(reader)
      };
    }

    protected class WorkItemDestroyHistoryRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemDestroyHistoryRecord>
    {
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));
      private SqlColumnBinder DestroyedDate = new SqlColumnBinder(nameof (DestroyedDate));
      private SqlColumnBinder Timestamp = new SqlColumnBinder(nameof (Timestamp));

      public override WorkItemDestroyHistoryRecord Bind(IDataReader reader) => new WorkItemDestroyHistoryRecord()
      {
        DestroyedDate = this.DestroyedDate.GetDateTime(reader),
        Timestamp = this.Timestamp.GetInt64(reader),
        WorkItemId = this.WorkItemId.GetInt32(reader)
      };
    }

    protected class WorkItemLinkHistoryRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemLinkHistoryRecord>
    {
      private SqlColumnBinder SourceWorkItemId = new SqlColumnBinder(nameof (SourceWorkItemId));
      private SqlColumnBinder TargetWorkItemId = new SqlColumnBinder(nameof (TargetWorkItemId));
      private SqlColumnBinder ForwardLinkTypeId = new SqlColumnBinder(nameof (ForwardLinkTypeId));
      private SqlColumnBinder ReverseLinkTypeId = new SqlColumnBinder(nameof (ReverseLinkTypeId));
      private SqlColumnBinder CreatedById = new SqlColumnBinder(nameof (CreatedById));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder RemovedById = new SqlColumnBinder(nameof (RemovedById));
      private SqlColumnBinder RemovedDate = new SqlColumnBinder(nameof (RemovedDate));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder Timestamp = new SqlColumnBinder(nameof (Timestamp));

      public override WorkItemLinkHistoryRecord Bind(IDataReader reader) => new WorkItemLinkHistoryRecord()
      {
        Comment = this.Comment.GetString(reader, false),
        CreatedById = this.CreatedById.GetGuid(reader),
        CreatedDate = this.CreatedDate.GetDateTime(reader),
        ForwardLinkTypeId = (int) this.ForwardLinkTypeId.GetInt16(reader),
        RemovedById = this.RemovedById.GetGuid(reader),
        RemovedDate = this.RemovedDate.GetDateTime(reader),
        ReverseLinkTypeId = (int) this.ReverseLinkTypeId.GetInt16(reader),
        SourceWorkItemId = this.SourceWorkItemId.GetInt32(reader),
        TargetWorkItemId = this.TargetWorkItemId.GetInt32(reader),
        Timestamp = this.Timestamp.GetInt64(reader)
      };
    }

    protected class WorkItemLinkTypeHistoryRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemLinkTypeHistoryRecord>
    {
      private SqlColumnBinder ReferenceName = new SqlColumnBinder(nameof (ReferenceName));
      private SqlColumnBinder ForwardName = new SqlColumnBinder(nameof (ForwardName));
      private SqlColumnBinder ForwardId = new SqlColumnBinder(nameof (ForwardId));
      private SqlColumnBinder ReverseName = new SqlColumnBinder(nameof (ReverseName));
      private SqlColumnBinder ReverseId = new SqlColumnBinder(nameof (ReverseId));
      private SqlColumnBinder Rules = new SqlColumnBinder(nameof (Rules));
      private SqlColumnBinder IsDenyDelete = new SqlColumnBinder(nameof (IsDenyDelete));
      private SqlColumnBinder IsDenyEdit = new SqlColumnBinder(nameof (IsDenyEdit));
      private SqlColumnBinder IsDirectional = new SqlColumnBinder(nameof (IsDirectional));
      private SqlColumnBinder IsNonCircular = new SqlColumnBinder(nameof (IsNonCircular));
      private SqlColumnBinder IsSingleTarget = new SqlColumnBinder(nameof (IsSingleTarget));
      private SqlColumnBinder IsTree = new SqlColumnBinder(nameof (IsTree));
      private SqlColumnBinder IsDisabled = new SqlColumnBinder(nameof (IsDisabled));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder Timestamp = new SqlColumnBinder(nameof (Timestamp));

      public override WorkItemLinkTypeHistoryRecord Bind(IDataReader reader) => new WorkItemLinkTypeHistoryRecord()
      {
        ForwardId = (int) this.ForwardId.GetInt16(reader),
        ForwardName = this.ForwardName.GetString(reader, false),
        IsDeleted = this.IsDeleted.GetBoolean(reader),
        IsDenyDelete = this.IsDenyDelete.GetBoolean(reader),
        IsDenyEdit = this.IsDenyEdit.GetBoolean(reader),
        IsDirectional = this.IsDirectional.GetBoolean(reader),
        IsDisabled = this.IsDisabled.GetBoolean(reader),
        IsNonCircular = this.IsNonCircular.GetBoolean(reader),
        IsSingleTarget = this.IsSingleTarget.GetBoolean(reader),
        IsTree = this.IsTree.GetBoolean(reader),
        ReferenceName = this.ReferenceName.GetString(reader, false),
        ReverseId = (int) this.ReverseId.GetInt16(reader),
        ReverseName = this.ReverseName.GetString(reader, false),
        Rules = this.Rules.GetInt32(reader),
        Timestamp = this.Timestamp.GetInt64(reader)
      };
    }

    protected class WorkItemTypeCategoryHistoryRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemTypeCategoryHistoryRecord>
    {
      private SqlColumnBinder ReferenceName = new SqlColumnBinder(nameof (ReferenceName));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder ProjectName = new SqlColumnBinder(nameof (ProjectName));
      private SqlColumnBinder ProjectGuid = new SqlColumnBinder(nameof (ProjectGuid));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder Timestamp = new SqlColumnBinder(nameof (Timestamp));

      public override WorkItemTypeCategoryHistoryRecord Bind(IDataReader reader) => new WorkItemTypeCategoryHistoryRecord()
      {
        IsDeleted = this.IsDeleted.GetBoolean(reader),
        Name = this.Name.GetString(reader, false),
        ProjectGuid = this.ProjectGuid.GetGuid(reader),
        ProjectName = this.ProjectName.GetString(reader, false),
        ReferenceName = this.ReferenceName.GetString(reader, false),
        Timestamp = this.Timestamp.GetInt64(reader)
      };
    }

    protected class WorkItemTypeCategoryMemberHistoryRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemTypeCategoryMemberHistoryRecord>
    {
      private SqlColumnBinder CategoryReferenceName = new SqlColumnBinder(nameof (CategoryReferenceName));
      private SqlColumnBinder ProjectName = new SqlColumnBinder(nameof (ProjectName));
      private SqlColumnBinder ProjectGuid = new SqlColumnBinder(nameof (ProjectGuid));
      private SqlColumnBinder WorkItemTypeName = new SqlColumnBinder(nameof (WorkItemTypeName));
      private SqlColumnBinder IsDeleted = new SqlColumnBinder(nameof (IsDeleted));
      private SqlColumnBinder Timestamp = new SqlColumnBinder(nameof (Timestamp));

      public override WorkItemTypeCategoryMemberHistoryRecord Bind(IDataReader reader) => new WorkItemTypeCategoryMemberHistoryRecord()
      {
        CategoryReferenceName = this.CategoryReferenceName.GetString(reader, false),
        IsDeleted = this.IsDeleted.GetBoolean(reader),
        ProjectGuid = this.ProjectGuid.GetGuid(reader),
        ProjectName = this.ProjectName.GetString(reader, false),
        WorkItemTypeName = this.WorkItemTypeName.GetString(reader, false),
        Timestamp = this.Timestamp.GetInt64(reader)
      };
    }

    protected class WorkItemTypeRenameHistoryRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemTypeRenameHistoryRecord>
    {
      private SqlColumnBinder OldName = new SqlColumnBinder(nameof (OldName));
      private SqlColumnBinder NewName = new SqlColumnBinder(nameof (NewName));
      private SqlColumnBinder ProjectName = new SqlColumnBinder(nameof (ProjectName));
      private SqlColumnBinder ProjectGuid = new SqlColumnBinder(nameof (ProjectGuid));
      private SqlColumnBinder Timestamp = new SqlColumnBinder(nameof (Timestamp));

      public override WorkItemTypeRenameHistoryRecord Bind(IDataReader reader) => new WorkItemTypeRenameHistoryRecord()
      {
        NewName = this.NewName.GetString(reader, false),
        OldName = this.OldName.GetString(reader, false),
        ProjectGuid = this.ProjectGuid.GetGuid(reader),
        ProjectName = this.ProjectName.GetString(reader, false),
        Timestamp = this.Timestamp.GetInt64(reader)
      };
    }

    protected class WorkItemIdRevisionPairBinder : 
      WorkItemTrackingObjectBinder<WorkItemIdRevisionPair>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder Watermark = new SqlColumnBinder(nameof (Watermark));

      public override WorkItemIdRevisionPair Bind(IDataReader reader) => new WorkItemIdRevisionPair()
      {
        Id = this.Id.GetInt32(reader),
        Revision = this.Revision.GetInt32(reader),
        Watermark = this.Watermark.GetInt32(reader)
      };
    }

    protected class WorkItemResourceLinkBinder : 
      WorkItemTrackingObjectBinder<WorkItemResourceLinkInfo>
    {
      private SqlColumnBinder SourceId = new SqlColumnBinder(nameof (SourceId));
      private SqlColumnBinder ResourceType = new SqlColumnBinder(nameof (ResourceType));
      private SqlColumnBinder ResourceId = new SqlColumnBinder(nameof (ResourceId));
      private SqlColumnBinder AuthorizedDate = new SqlColumnBinder(nameof (AuthorizedDate));
      private SqlColumnBinder RevisedDate = new SqlColumnBinder(nameof (RevisedDate));
      private SqlColumnBinder Location = new SqlColumnBinder(nameof (Location));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
      private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
      private SqlColumnBinder Length = new SqlColumnBinder(nameof (Length));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private bool m_readExtendedColumns;

      public WorkItemResourceLinkBinder(bool readExtendedColumns) => this.m_readExtendedColumns = readExtendedColumns;

      public override WorkItemResourceLinkInfo Bind(IDataReader reader)
      {
        WorkItemResourceLinkInfo resourceLinkInfo = new WorkItemResourceLinkInfo()
        {
          SourceId = this.SourceId.GetInt32(reader),
          ResourceType = (ResourceLinkType) this.ResourceType.GetInt32(reader),
          ResourceId = this.ResourceId.GetInt32(reader),
          AuthorizedDate = this.AuthorizedDate.GetDateTime(reader),
          RevisedDate = this.RevisedDate.GetDateTime(reader),
          Location = this.Location.GetString(reader, true),
          Name = this.Name.GetString(reader, true)
        };
        if (this.m_readExtendedColumns)
        {
          resourceLinkInfo.ResourceCreatedDate = this.CreatedDate.GetDateTime(reader);
          resourceLinkInfo.ResourceModifiedDate = this.ModifiedDate.GetDateTime(reader);
          resourceLinkInfo.ResourceSize = this.Length.GetInt32(reader);
          resourceLinkInfo.Comment = this.Comment.GetString(reader, true);
        }
        return resourceLinkInfo;
      }
    }
  }
}
