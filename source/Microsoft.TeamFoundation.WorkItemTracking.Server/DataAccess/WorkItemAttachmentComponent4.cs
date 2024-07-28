// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemAttachmentComponent4
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemAttachmentComponent4 : WorkItemAttachmentComponent3
  {
    public override void UpdateAttachment(Guid attachmentId, int tfsFileId)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemAttachment");
      this.BindGuid("@attachmentId", attachmentId);
      this.BindInt("@tfsFileId", tfsFileId);
      this.ExecuteNonQueryEx();
    }
  }
}
