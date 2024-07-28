// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ClassificationNodeComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class ClassificationNodeComponent2 : ClassificationNodeComponent
  {
    public override List<TreeNodeRecord> GetAllClassificationNodes(bool disableDataspaceRls = false)
    {
      this.DataspaceRlsEnabled = !disableDataspaceRls;
      this.PrepareStoredProcedure("prc_GetAllClassificationNodes");
      return this.ExecuteUnknown<List<TreeNodeRecord>>((System.Func<IDataReader, List<TreeNodeRecord>>) (reader => this.GetClassificationNodeBinder().BindAll(reader).ToList<TreeNodeRecord>()));
    }

    public override List<TreeNodeRecord> GetChangedClassificationNodesByCachestamp(
      long cachestamp,
      out long lastCachestmp,
      bool disableDataspaceRls = false)
    {
      this.DataspaceRlsEnabled = !disableDataspaceRls;
      this.PrepareStoredProcedure("prc_GetChangedClassificationNodesByCachestamp");
      this.BindLong("@cachestamp", cachestamp);
      long internalLastCachestamp = 0;
      List<TreeNodeRecord> nodesByCachestamp = this.ExecuteUnknown<List<TreeNodeRecord>>((System.Func<IDataReader, List<TreeNodeRecord>>) (reader =>
      {
        reader.Read();
        internalLastCachestamp = reader.GetInt64(0);
        reader.NextResult();
        return this.GetClassificationNodeBinder().BindAll(reader).ToList<TreeNodeRecord>();
      }));
      lastCachestmp = internalLastCachestamp;
      return nodesByCachestamp;
    }

    public override List<TreeNodeRecord> GetChangedClassificationNodesBySequenceId(
      int sequenceId,
      out int lastSequenceId)
    {
      this.PrepareStoredProcedure("prc_GetChangedClassificationNodesBySequenceId");
      this.BindInt("@sequenceId", sequenceId);
      int internalLastSequenceId = 0;
      List<TreeNodeRecord> nodesBySequenceId = this.ExecuteUnknown<List<TreeNodeRecord>>((System.Func<IDataReader, List<TreeNodeRecord>>) (reader =>
      {
        reader.Read();
        internalLastSequenceId = reader.GetInt32(0);
        reader.NextResult();
        return this.GetClassificationNodeBinder().BindAll(reader).ToList<TreeNodeRecord>();
      }));
      lastSequenceId = internalLastSequenceId;
      return nodesBySequenceId;
    }

    public override List<TreeNodeRecord> GetDeletedClassificationNodes(
      Guid projectId,
      DateTime since)
    {
      this.PrepareStoredProcedure("prc_GetDeletedClassificationNodes");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindDateTime("@since", since);
      return this.ExecuteUnknown<List<TreeNodeRecord>>((System.Func<IDataReader, List<TreeNodeRecord>>) (reader => this.GetClassificationNodeBinder().BindAll(reader).ToList<TreeNodeRecord>()));
    }

    public override WorkItemClassificationReconciliationState ReconcileWorkItems(
      Guid requestIdentityId)
    {
      this.PrepareStoredProcedure("prc_ReconcileWorkItemClassification");
      this.BindGuid("@changerTfId", requestIdentityId);
      this.BindInt("@timeout", this.GetEffectiveCommandTimeout().Timeout / 2);
      return (WorkItemClassificationReconciliationState) this.ExecuteScalar();
    }

    public override IEnumerable<ClassificationNodeChange> GetClassificationNodeChanges(
      bool pendingReclassification,
      out long cachestamp)
    {
      this.PrepareDynamicProcedure("dynprc_GetClassificationNodeChanges", "\r\nSET NOCOUNT ON\r\nSET XACT_ABORT ON\r\n\r\n-- Get the max cache stamp of processed changes\r\nDECLARE @lastStamp ROWVERSION\r\n\r\nSELECT  @lastStamp = ISNULL(MAX(Cachestamp), 0)\r\nFROM    dbo.tbl_ClassificationNodeChange\r\nWHERE   PartitionId = @partitionId\r\n        AND IsWorkItemReconciled = ~@pendingReclassification\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nSELECT  CONVERT(BIGINT, @lastStamp) AS Cachestamp\r\n\r\nSELECT  DataspaceId, Id, Path\r\nFROM    dbo.tbl_ClassificationNodeChange\r\nWHERE   PartitionId = @partitionId\r\n        AND IsWorkItemReconciled = ~@pendingReclassification\r\n        AND Cachestamp <= @lastStamp\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))");
      return this.BindAndExecuteClassificationNodeChanges(pendingReclassification, out cachestamp);
    }

    public override void DeleteClassificationNodeChanges(long cachestamp)
    {
      this.PrepareStoredProcedure("prc_DeleteClassificationNodeChanges");
      this.BindLong("@cachestamp", cachestamp);
      this.ExecuteNonQuery();
    }

    protected override ClassificationNodeComponent.ClassificationNodeBinder GetClassificationNodeBinder() => (ClassificationNodeComponent.ClassificationNodeBinder) new ClassificationNodeComponent2.ClassificationNodeBinder2(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier));

    protected IEnumerable<ClassificationNodeChange> BindAndExecuteClassificationNodeChanges(
      bool pendingReclassification,
      out long cachestamp)
    {
      this.BindBoolean("@pendingReclassification", pendingReclassification);
      long internalCachestamp = 0;
      List<ClassificationNodeChange> classificationNodeChangeList = this.ExecuteUnknown<List<ClassificationNodeChange>>((System.Func<IDataReader, List<ClassificationNodeChange>>) (reader =>
      {
        reader.Read();
        internalCachestamp = reader.GetInt64(0);
        reader.NextResult();
        return new ClassificationNodeComponent.ClassificationNodeChangeBinder(new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier)).BindAll(reader).ToList<ClassificationNodeChange>();
      }));
      cachestamp = internalCachestamp;
      return (IEnumerable<ClassificationNodeChange>) classificationNodeChangeList;
    }

    protected class ClassificationNodeBinder2 : ClassificationNodeComponent.ClassificationNodeBinder
    {
      protected SqlColumnBinder m_startDate = new SqlColumnBinder("StartDate");
      protected SqlColumnBinder m_finishDate = new SqlColumnBinder("FinishDate");
      protected SqlColumnBinder m_reclassifyId = new SqlColumnBinder("ReclassifyId");
      protected SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
      protected SqlColumnBinder m_changeDate = new SqlColumnBinder("ChangeDate");
      protected SqlColumnBinder m_changedBy = new SqlColumnBinder("ChangedBy");
      protected System.Func<int, Guid> m_projectIdResolver;

      public ClassificationNodeBinder2(System.Func<int, Guid> projectIdResolver)
      {
        this.m_id = new SqlColumnBinder("Id");
        this.m_parentId = new SqlColumnBinder("ParentId");
        this.m_name = new SqlColumnBinder("Name");
        this.m_structureType = new SqlColumnBinder("StructureType");
        this.m_typeId = new SqlColumnBinder("TypeId");
        this.m_identifier = new SqlColumnBinder("Identifier");
        this.m_isDeleted = new SqlColumnBinder("IsDeleted");
        this.m_projectIdResolver = projectIdResolver;
      }

      public override TreeNodeRecord Bind(IDataReader reader)
      {
        TreeNodeRecord treeNodeRecord = base.Bind(reader);
        treeNodeRecord.ProjectId = this.m_projectIdResolver(this.m_dataspaceId.GetInt32(reader));
        if (this.m_startDate.ColumnExists(reader) && !this.m_startDate.IsNull(reader))
          treeNodeRecord.StartDate = new DateTime?(this.m_startDate.GetDateTime(reader));
        if (this.m_finishDate.ColumnExists(reader) && !this.m_finishDate.IsNull(reader))
          treeNodeRecord.FinishDate = new DateTime?(this.m_finishDate.GetDateTime(reader));
        treeNodeRecord.ReclassifyId = this.m_reclassifyId.GetInt32(reader, 0, 0);
        treeNodeRecord.ChangeDate = this.m_changeDate.GetDateTime(reader, DateTime.MinValue);
        treeNodeRecord.ChangedBy = this.m_changedBy.GetString(reader, (string) null);
        return treeNodeRecord;
      }
    }
  }
}
