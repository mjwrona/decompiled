// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.SecretsScan.WorkItemCommentScanData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.SecretsScan
{
  internal class WorkItemCommentScanData
  {
    public WorkItemCommentScanData(Guid projectId, int workItemId, int revisionId)
    {
      this.ProjectId = projectId;
      this.WorkItemId = workItemId;
      this.RevisionId = revisionId;
    }

    public Guid ProjectId { get; set; }

    public int WorkItemId { get; set; }

    public int RevisionId { get; set; }
  }
}
