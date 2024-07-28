// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata.MDWorkItemLinkType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class MDWorkItemLinkType
  {
    public int Rules;
    private WorkItemLinkTypeEnd m_reverseEnd;
    private const int c_topologyMask = 28;

    public WorkItemLinkTypeEnd ForwardEnd { get; set; }

    public WorkItemLinkTypeEnd ReverseEnd
    {
      get => this.m_reverseEnd == null ? this.ForwardEnd : this.m_reverseEnd;
      set => this.m_reverseEnd = value;
    }

    public int ForwardId => this.ForwardEnd.Id;

    public int ReverseId => this.ReverseEnd.Id;

    public string ReferenceName { get; set; }

    public string ForwardEndName => this.ForwardEnd.Name;

    public string ReverseEndName => this.ReverseEnd.Name;

    public bool CanEdit => (this.Rules & 2) == 0;

    public bool Enabled => (this.Rules & 32) == 0;

    public bool IsCircular => (this.Rules & 8) == 0;

    public bool IsDirectional => (this.Rules & 4) == 4;

    public bool IsOneToMany => (this.Rules & 16) == 16;

    public bool IsRemote => (this.Rules & 64) == 64;

    public LinkTopology Topology
    {
      get
      {
        if (this.InternalLinkTopologies == 0)
          return LinkTopology.Network;
        if (this.InternalLinkTopologies == 4)
          return LinkTopology.DirectedNetwork;
        return this.InternalLinkTopologies == 12 ? LinkTopology.Dependency : LinkTopology.Tree;
      }
    }

    public int InternalLinkTopologies => this.Rules & 28;
  }
}
