// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.InternalWorkItemLinkInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  public class InternalWorkItemLinkInfo
  {
    public InternalWorkItemLinkInfo(int sourceId, int targetId, int linkTypeId, bool isLocked)
    {
      this.SourceId = sourceId;
      this.TargetId = targetId;
      this.LinkTypeId = linkTypeId;
      this.IsLocked = isLocked;
    }

    public int SourceId { get; set; }

    public int TargetId { get; set; }

    public int LinkTypeId { get; set; }

    public bool IsLocked { get; set; }
  }
}
