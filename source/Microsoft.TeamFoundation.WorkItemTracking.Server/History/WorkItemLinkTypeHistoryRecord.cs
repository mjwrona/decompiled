// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.History.WorkItemLinkTypeHistoryRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.History
{
  public class WorkItemLinkTypeHistoryRecord
  {
    public string ReferenceName { get; set; }

    public string ForwardName { get; set; }

    public int ForwardId { get; set; }

    public string ReverseName { get; set; }

    public int ReverseId { get; set; }

    public int Rules { get; set; }

    public bool IsDenyDelete { get; set; }

    public bool IsDenyEdit { get; set; }

    public bool IsDirectional { get; set; }

    public bool IsNonCircular { get; set; }

    public bool IsSingleTarget { get; set; }

    public bool IsTree { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsDeleted { get; set; }

    public long Timestamp { get; set; }
  }
}
