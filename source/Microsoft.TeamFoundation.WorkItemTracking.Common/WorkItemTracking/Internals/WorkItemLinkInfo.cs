// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.WorkItemLinkInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WorkItemLinkInfo : LinkInfo
  {
    public int AddedBy;
    public int RemovedBy;

    public int LinkType { get; set; }

    public int SourceId { get; set; }

    public int TargetId { get; set; }

    public bool IsLocked { get; set; }

    public WorkItemLinkInfo()
    {
      this.LinkType = 0;
      this.SourceId = 0;
      this.TargetId = 0;
      this.AddedBy = 0;
      this.RemovedBy = 0;
      this.IsLocked = false;
    }

    public WorkItemLinkInfo(WorkItemLinkInfo link)
      : base((LinkInfo) link)
    {
      this.LinkType = link.LinkType;
      this.SourceId = link.SourceId;
      this.TargetId = link.TargetId;
      this.AddedBy = link.AddedBy;
      this.RemovedBy = link.RemovedBy;
      this.IsLocked = link.IsLocked;
    }

    public override int GetHashCode() => this.SourceId ^ this.TargetId ^ Math.Abs(this.LinkType);

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      WorkItemLinkInfo other = obj as WorkItemLinkInfo;
      return !(other == (WorkItemLinkInfo) null) && this.EqualsInternal(other);
    }

    public static bool operator ==(
      WorkItemLinkInfo workItemLinkInfo1,
      WorkItemLinkInfo workItemLinkInfo2)
    {
      if ((object) workItemLinkInfo1 == (object) workItemLinkInfo2)
        return true;
      return (object) workItemLinkInfo1 != null && (object) workItemLinkInfo2 != null && workItemLinkInfo1.EqualsInternal(workItemLinkInfo2);
    }

    public static bool operator !=(
      WorkItemLinkInfo workItemLinkInfo1,
      WorkItemLinkInfo workItemLinkInfo2)
    {
      return !(workItemLinkInfo1 == workItemLinkInfo2);
    }

    private bool EqualsInternal(WorkItemLinkInfo other)
    {
      if ((object) other == null)
        return false;
      if (this.SourceId == other.SourceId && this.TargetId == other.TargetId && this.LinkType == other.LinkType)
        return true;
      return this.SourceId == other.TargetId && this.TargetId == other.SourceId && this.LinkType == -other.LinkType;
    }
  }
}
