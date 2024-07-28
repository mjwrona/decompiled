// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata.WorkItemLinkTypeEnd
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata
{
  public class WorkItemLinkTypeEnd
  {
    private const string c_forward = "-Forward";
    private const string c_reverse = "-Reverse";

    public int Id { get; set; }

    public string ReferenceName
    {
      get
      {
        string referenceName = this.LinkType.ReferenceName;
        return !this.LinkType.IsDirectional ? referenceName : referenceName + (this.IsForwardEnd ? "-Forward" : "-Reverse");
      }
    }

    public string Name { get; set; }

    public MDWorkItemLinkType LinkType { get; set; }

    public WorkItemLinkTypeEnd OppositeEnd => !this.IsForwardEnd ? this.LinkType.ForwardEnd : this.LinkType.ReverseEnd;

    public bool IsForwardEnd { get; set; }
  }
}
