// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemAttachmentComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Attachment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemAttachmentComponent : WorkItemTrackingResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<WorkItemAttachmentComponent>(0, true),
      (IComponentCreator) new ComponentCreator<WorkItemAttachmentComponent2>(1),
      (IComponentCreator) new ComponentCreator<WorkItemAttachmentComponent3>(2),
      (IComponentCreator) new ComponentCreator<WorkItemAttachmentComponent4>(3),
      (IComponentCreator) new ComponentCreator<WorkItemAttachmentComponent5>(4),
      (IComponentCreator) new ComponentCreator<WorkItemAttachmentComponent6>(5)
    }, "WorkItemAttachment", "WorkItem");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static WorkItemAttachmentComponent()
    {
      WorkItemAttachmentComponent.s_sqlExceptionFactories[600149] = new SqlExceptionFactory(typeof (WorkItemAttachmentAlreadyExistsException));
      WorkItemAttachmentComponent.s_sqlExceptionFactories[600179] = new SqlExceptionFactory(typeof (WorkItemAttachmentNotFoundException));
    }

    protected virtual bool BindProjectUri => true;

    public virtual InternalAttachmentInfo GetAttachment(Guid attachmentId)
    {
      this.PrepareStoredProcedure("FetchTfsFileIdByFilePath");
      this.BindString("@filePath", attachmentId.ToString("D"), 36, false, SqlDbType.NVarChar);
      return this.ExecuteUnknown<InternalAttachmentInfo>((System.Func<IDataReader, InternalAttachmentInfo>) (reader => new WorkItemAttachmentComponent.InternalAttachmentInfoBinder().Bind(reader)));
    }

    public virtual InternalAttachmentInfo GetAttachment(int attachmentExtId)
    {
      this.PrepareStoredProcedure("FetchTfsFileIdByExtId");
      this.BindInt("@extID", attachmentExtId);
      return this.ExecuteUnknown<InternalAttachmentInfo>((System.Func<IDataReader, InternalAttachmentInfo>) (reader => new WorkItemAttachmentComponent.InternalAttachmentInfoBinder().Bind(reader)));
    }

    public virtual IEnumerable<WorkItemAttachmentInfo> GetOrphanAttachments()
    {
      this.PrepareStoredProcedure("GetOrphanAttachmentIds");
      return (IEnumerable<WorkItemAttachmentInfo>) new WorkItemAttachmentComponent.WorkItemAttachmentInfoBinder().BindAll(this.ExecuteReader()).ToArray<WorkItemAttachmentInfo>();
    }

    public virtual IEnumerable<WorkItemAttachmentInfo> GetNoCurrentReferenceAttachments() => throw new NotImplementedException();

    public virtual void DeleteAttachments(IEnumerable<WorkItemAttachmentInfo> attachments) => this.DeleteAttachments(attachments.Select<WorkItemAttachmentInfo, int>((System.Func<WorkItemAttachmentInfo, int>) (attachment => attachment.TfsFileId)));

    protected virtual void DeleteAttachments(IEnumerable<Guid> attachmentIds) => throw new NotImplementedException();

    protected virtual void DeleteAttachments(IEnumerable<int> tfsFileIds)
    {
      this.PrepareStoredProcedure("DeleteOrphanAttachmentsMetadata");
      this.BindInt32Table("@tfsFileIdsTable", tfsFileIds);
      this.ExecuteNonQueryEx();
    }

    public virtual void CreateAttachment(string userSid, Guid attachmentId, int tfsFileId)
    {
      this.PrepareDynamicProcedure("dynprc_CreateAttachment", "\r\n                SET NOCOUNT ON\r\n                SET XACT_ABORT ON\r\n\r\n                DECLARE @tfError NVARCHAR(4000)\r\n                DECLARE @attachmentIdString NVARCHAR(36) = CONVERT(NVARCHAR(36), @attachmentId)\r\n\r\n                BEGIN TRAN\r\n\r\n                DECLARE @exists BIT\r\n                SELECT  @exists = CASE WHEN EXISTS (\r\n                    SELECT  *\r\n                    FROM    dbo.Attachments WITH (UPDLOCK, HOLDLOCK)\r\n                    WHERE   PartitionId = @partitionId\r\n                            AND FileGuid = @attachmentIdString\r\n                ) THEN 1 ELSE 0 END\r\n                OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\n                IF @exists = 1\r\n                BEGIN\r\n                    SET @tfError = dbo.func_GetMessage(600179); RAISERROR(@tfError, 11, 1)\r\n                    ROLLBACK\r\n                    RETURN\r\n                END\r\n\r\n                INSERT INTO dbo.Attachments\r\n                (\r\n                    PartitionId,\r\n                    FileGuid,\r\n                    IdentityName,\r\n                    CreationDate,\r\n                    TfsFileId\r\n                )\r\n                VALUES\r\n                (\r\n                    @partitionId,\r\n                    @attachmentIdString,\r\n                    @userSid,\r\n                    GETUTCDATE(),\r\n                    @tfsFileId\r\n                )\r\n                OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\n                COMMIT");
      this.BindString("@userSid", userSid, 256, false, SqlDbType.NVarChar);
      this.BindString("@attachmentId", attachmentId.ToString("D"), 36, false, SqlDbType.NVarChar);
      this.BindInt("@tfsFileId", tfsFileId);
      this.ExecuteNonQueryEx();
    }

    public virtual void UpdateAttachment(Guid attachmentId, int tfsFileId)
    {
      this.PrepareDynamicProcedure("dynprc_UpdateWorkItemAttachment", "\r\n                SET NOCOUNT ON\r\n                SET XACT_ABORT ON\r\n\r\n                DECLARE @tfError NVARCHAR(4000)\r\n                DECLARE @attachmentIdString NVARCHAR(36) = CONVERT(NVARCHAR(36), @attachmentId)\r\n                \r\n                UPDATE dbo.Attachments \r\n                SET tfsFileId = @tfsFileId\r\n                WHERE FileGuid = @attachmentIdString\r\n                AND PartitionId = @partitionId\r\n                OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\n                IF @@ROWCOUNT = 0\r\n                BEGIN\r\n                    SET @tfError = dbo.func_GetMessage(600148); RAISERROR(@tfError, 11, 1)\r\n                    RETURN\r\n                END\r\n                ");
      this.BindString("@attachmentId", attachmentId.ToString("D"), 36, false, SqlDbType.NVarChar);
      this.BindInt("@tfsFileId", tfsFileId);
      this.ExecuteNonQueryEx();
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) WorkItemAttachmentComponent.s_sqlExceptionFactories;

    protected class WorkItemAttachmentInfoBinder : 
      WorkItemTrackingObjectBinder<WorkItemAttachmentInfo>
    {
      protected SqlColumnBinder m_id = new SqlColumnBinder("AttachmentId");
      protected SqlColumnBinder m_tfsFileId = new SqlColumnBinder("TfsFileId");

      public override WorkItemAttachmentInfo Bind(IDataReader reader)
      {
        Guid guid = Guid.Empty;
        if (this.m_id.ColumnExists(reader))
          guid = this.m_id.GetGuid(reader);
        return new WorkItemAttachmentInfo()
        {
          Id = guid,
          TfsFileId = this.m_tfsFileId.GetInt32(reader)
        };
      }
    }

    protected class InternalAttachmentInfoBinder : 
      WorkItemTrackingObjectBinder<InternalAttachmentInfo>
    {
      protected SqlColumnBinder m_id = new SqlColumnBinder("AttachmentId");
      protected SqlColumnBinder m_tfsFileId = new SqlColumnBinder("TfsFileId");
      protected SqlColumnBinder m_sid;
      protected SqlColumnBinder m_areaId;

      public InternalAttachmentInfoBinder()
      {
        this.m_sid = new SqlColumnBinder("IdentityName");
        this.m_areaId = new SqlColumnBinder("AreaID");
      }

      public override InternalAttachmentInfo Bind(IDataReader reader)
      {
        if (!reader.Read())
          return (InternalAttachmentInfo) null;
        Guid guid = Guid.Empty;
        if (this.m_id.ColumnExists(reader))
          guid = this.m_id.GetGuid(reader);
        InternalAttachmentInfo internalAttachmentInfo1 = new InternalAttachmentInfo();
        internalAttachmentInfo1.Id = guid;
        internalAttachmentInfo1.TfsFileId = this.m_tfsFileId.GetInt32(reader);
        internalAttachmentInfo1.UploaderSid = this.m_sid.GetString(reader, false);
        InternalAttachmentInfo internalAttachmentInfo2 = internalAttachmentInfo1;
        List<int> intList = new List<int>();
        if (reader.NextResult())
        {
          while (reader.Read())
            intList.Add(this.m_areaId.GetInt32(reader));
        }
        internalAttachmentInfo2.PermittedAreaIds = (IEnumerable<int>) intList;
        return internalAttachmentInfo2;
      }
    }
  }
}
