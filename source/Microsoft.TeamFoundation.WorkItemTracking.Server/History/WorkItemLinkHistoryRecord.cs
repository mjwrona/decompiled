// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.WorkItemLinkHistoryRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  public class WorkItemLinkHistoryRecord
  {
    public int SourceWorkItemId { get; set; }

    public int TargetWorkItemId { get; set; }

    public int ForwardLinkTypeId { get; set; }

    public int ReverseLinkTypeId { get; set; }

    public Guid CreatedById { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid RemovedById { get; set; }

    public DateTime RemovedDate { get; set; }

    public string Comment { get; set; }

    public long Timestamp { get; set; }
  }
}
