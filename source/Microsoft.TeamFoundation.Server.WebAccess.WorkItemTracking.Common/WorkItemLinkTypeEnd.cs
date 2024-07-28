// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemLinkTypeEnd
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class WorkItemLinkTypeEnd
  {
    private WorkItemLinkType m_linkType;
    private string m_name;
    private int m_id;
    private WorkItemLinkTypeEnd m_oppositeEnd;

    internal WorkItemLinkTypeEnd(WorkItemLinkType linkType, string name, int id)
    {
      this.m_linkType = linkType;
      this.m_name = name;
      this.m_id = id;
    }

    public WorkItemLinkType LinkType => this.m_linkType;

    public string Name => this.m_name;

    public string ImmutableName => this.IsForwardLink ? this.m_linkType.ReferenceName + "-Forward" : this.m_linkType.ReferenceName + "-Reverse";

    public int Id => this.m_id;

    public WorkItemLinkTypeEnd OppositeEnd
    {
      get => this.m_oppositeEnd;
      internal set => this.m_oppositeEnd = value;
    }

    public bool IsForwardLink => this.m_linkType.ForwardEnd == this;

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["id"] = (object) this.Id;
      json["immutableName"] = (object) this.ImmutableName;
      json["isForwardLink"] = (object) this.IsForwardLink;
      json["name"] = (object) this.Name;
      return json;
    }
  }
}
