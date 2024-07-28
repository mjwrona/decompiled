// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VssEvent
  {
    private int m_EventIndex = -1;

    [XmlAttribute("id")]
    public string EventIdValue { get; set; }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("area")]
    public string Area { get; set; }

    [XmlAttribute("areaPath")]
    public string AreaPath { get; set; }

    [XmlAttribute("iterationPath")]
    public string IterationPath { get; set; }

    [XmlElement("Description")]
    public string Description { get; set; }

    [XmlElement("EventIndex")]
    public int EventIndex
    {
      get => this.m_EventIndex;
      set => this.m_EventIndex = value;
    }

    [XmlElement("Alert")]
    public VssAlert Alert { get; set; }

    [XmlElement("MdmEvent")]
    public VssMdmEvent MdmEvent { get; set; }

    [XmlIgnore]
    public int EventId { get; set; }

    [XmlIgnore]
    internal string EventSource { get; set; }
  }
}
