// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.InternalWorkItemRelation
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class InternalWorkItemRelation : IWorkItemRelation
  {
    public InternalWorkItemRelation(int sourceId, int targetId, int linkTypeId, bool isLocked)
    {
      if (sourceId == InternalWorkItemRelation.MissingID || targetId == InternalWorkItemRelation.MissingID)
        linkTypeId = InternalWorkItemRelation.MissingID;
      this.SourceID = sourceId;
      this.TargetID = targetId;
      this.LinkTypeID = linkTypeId;
      this.IsLocked = isLocked;
    }

    public int SourceID { get; set; }

    public int TargetID { get; set; }

    public int LinkTypeID { get; set; }

    public bool IsLocked { get; set; }

    public static int MissingID => 0;
  }
}
