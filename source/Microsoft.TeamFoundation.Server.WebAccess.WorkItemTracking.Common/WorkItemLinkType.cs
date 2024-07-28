// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemLinkType
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class WorkItemLinkType
  {
    private const int TopologyMask = 28;
    private bool m_isDeleted;

    public WorkItemLinkType(WorkItemLinkTypeRecord linkTypeRecord)
    {
      this.ReferenceName = linkTypeRecord.ReferenceName;
      this.Rules = (uint) linkTypeRecord.Rules;
      this.ForwardEnd = new WorkItemLinkTypeEnd(this, linkTypeRecord.ForwardName, linkTypeRecord.ForwardId);
      if (this.IsDirectional)
      {
        this.ReverseEnd = new WorkItemLinkTypeEnd(this, linkTypeRecord.ReverseName, linkTypeRecord.ReverseId);
        this.ReverseEnd.OppositeEnd = this.ForwardEnd;
        this.ForwardEnd.OppositeEnd = this.ReverseEnd;
      }
      else
      {
        this.ForwardEnd.OppositeEnd = this.ForwardEnd;
        this.ReverseEnd = this.ForwardEnd;
      }
    }

    public string ReferenceName { get; set; }

    private uint Rules { get; set; }

    public WorkItemLinkTypeEnd ForwardEnd { get; private set; }

    public WorkItemLinkTypeEnd ReverseEnd { get; private set; }

    public bool IsDirectional => ((int) this.Rules & 4) == 4;

    public bool IsNonCircular => ((int) this.Rules & 8) == 8;

    public bool IsOneToMany => ((int) this.Rules & 16) == 16;

    public bool IsActive => ((int) this.Rules & 32) == 0 && !this.m_isDeleted;

    public bool CanDelete => ((int) this.Rules & 1) == 0 && !this.m_isDeleted;

    public bool CanEdit => ((int) this.Rules & 2) == 0 && !this.m_isDeleted;

    public WorkItemLinkType.Topology LinkTopology => WorkItemLinkType.GetTopology(this.Rules);

    public bool IsRemote => ((int) this.Rules & 64) == 64;

    internal static WorkItemLinkType.Topology GetTopology(uint topology)
    {
      switch (topology & 28U)
      {
        case 0:
          return WorkItemLinkType.Topology.Network;
        case 4:
          return WorkItemLinkType.Topology.DirectedNetwork;
        case 12:
          return WorkItemLinkType.Topology.Dependency;
        case 28:
          return WorkItemLinkType.Topology.Tree;
        default:
          return WorkItemLinkType.Topology.Unknown;
      }
    }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["referenceName"] = (object) this.ReferenceName;
      json["topology"] = (object) this.LinkTopology.ToString();
      json["canDelete"] = (object) this.CanDelete;
      json["canEdit"] = (object) this.CanEdit;
      json["isActive"] = (object) this.IsActive;
      json["isDirectional"] = (object) this.IsDirectional;
      json["isNonCircular"] = (object) this.IsNonCircular;
      json["isOneToMany"] = (object) this.IsOneToMany;
      json["isRemote"] = (object) this.IsRemote;
      json["forwardEnd"] = (object) this.ForwardEnd.ToJson();
      json["reverseEnd"] = (object) this.ReverseEnd.ToJson();
      return json;
    }

    public enum Topology
    {
      Unknown = -1, // 0xFFFFFFFF
      Network = 0,
      DirectedNetwork = 4,
      Dependency = 12, // 0x0000000C
      Tree = 28, // 0x0000001C
    }
  }
}
