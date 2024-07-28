// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemAttachmentComponent2
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
  internal class WorkItemAttachmentComponent2 : WorkItemAttachmentComponent
  {
    public override InternalAttachmentInfo GetAttachment(Guid attachmentId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemAttachmentById");
      this.BindGuid("@attachmentId", attachmentId);
      return this.ExecuteUnknown<InternalAttachmentInfo>((System.Func<IDataReader, InternalAttachmentInfo>) (reader => new WorkItemAttachmentComponent2.InternalAttachmentInfoBinder2().Bind(reader)));
    }

    public override InternalAttachmentInfo GetAttachment(int attachmentExtId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemAttachmentByExtId");
      this.BindInt("@attachmentExtId", attachmentExtId);
      return this.ExecuteUnknown<InternalAttachmentInfo>((System.Func<IDataReader, InternalAttachmentInfo>) (reader => new WorkItemAttachmentComponent2.InternalAttachmentInfoBinder2().Bind(reader)));
    }

    public override IEnumerable<WorkItemAttachmentInfo> GetOrphanAttachments()
    {
      this.PrepareStoredProcedure("prc_GetOrphanWorkItemAttachments");
      return (IEnumerable<WorkItemAttachmentInfo>) new WorkItemAttachmentComponent.WorkItemAttachmentInfoBinder().BindAll(this.ExecuteReader()).ToArray<WorkItemAttachmentInfo>();
    }

    public override void DeleteAttachments(IEnumerable<WorkItemAttachmentInfo> attachments) => this.DeleteAttachments(attachments.Select<WorkItemAttachmentInfo, Guid>((System.Func<WorkItemAttachmentInfo, Guid>) (attachment => attachment.Id)));

    protected override void DeleteAttachments(IEnumerable<Guid> attachmentIds)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkItemAttachments");
      this.BindGuidTable("@attachmentIds", attachmentIds);
      this.ExecuteNonQueryEx();
    }

    protected override void DeleteAttachments(IEnumerable<int> tfsFileIds) => throw new NotImplementedException();

    public override void CreateAttachment(string userSid, Guid attachmentId, int tfsFileId)
    {
      this.PrepareStoredProcedure("prc_CreateWorkItemAttachment");
      this.BindString("@userSid", userSid, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@attachmentId", attachmentId);
      this.BindInt("@tfsFileId", tfsFileId);
      if (this.BindProjectUri)
        this.BindString("@projectUri", (string) null, 500, true, SqlDbType.NVarChar);
      this.ExecuteNonQueryEx();
    }

    protected class InternalAttachmentInfoBinder2 : 
      WorkItemAttachmentComponent.InternalAttachmentInfoBinder
    {
      public InternalAttachmentInfoBinder2()
      {
        this.m_sid = new SqlColumnBinder("UploaderSid");
        this.m_areaId = new SqlColumnBinder("AreaId");
      }
    }
  }
}
