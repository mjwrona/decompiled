// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemAttachmentComponent6
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Attachment;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemAttachmentComponent6 : WorkItemAttachmentComponent5
  {
    public override InternalAttachmentInfo GetAttachment(Guid attachmentId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemAttachmentById");
      this.BindGuid("@attachmentId", attachmentId);
      return this.ExecuteUnknown<InternalAttachmentInfo>((System.Func<IDataReader, InternalAttachmentInfo>) (reader => new WorkItemAttachmentComponent6.InternalAttachmentInfoBinder3().Bind(reader)));
    }

    public override InternalAttachmentInfo GetAttachment(int attachmentExtId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemAttachmentByExtId");
      this.BindInt("@attachmentExtId", attachmentExtId);
      return this.ExecuteUnknown<InternalAttachmentInfo>((System.Func<IDataReader, InternalAttachmentInfo>) (reader => new WorkItemAttachmentComponent6.InternalAttachmentInfoBinder3().Bind(reader)));
    }

    protected class InternalAttachmentInfoBinder3 : 
      WorkItemAttachmentComponent2.InternalAttachmentInfoBinder2
    {
      protected SqlColumnBinder m_isDeleted;

      public InternalAttachmentInfoBinder3() => this.m_isDeleted = new SqlColumnBinder("IsDeleted");

      public override InternalAttachmentInfo Bind(IDataReader reader)
      {
        InternalAttachmentInfo internalAttachmentInfo = base.Bind(reader);
        if (internalAttachmentInfo == null)
          return (InternalAttachmentInfo) null;
        if (reader.NextResult())
          internalAttachmentInfo.IsDeleted = !reader.Read() || this.m_isDeleted.GetBoolean(reader);
        return internalAttachmentInfo;
      }
    }
  }
}
