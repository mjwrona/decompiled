// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceHostLevelModifiedEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServiceHostLevelModifiedEvent
  {
    [XmlElement("CurrentServiceLevel")]
    public string CurrentServiceLevel { get; set; }

    [XmlElement("NewServiceLevel")]
    public string NewServiceLevel { get; set; }

    [XmlElement("DatabaseId")]
    public int DatabaseId { get; set; }

    [XmlIgnore]
    public TeamFoundationHostType HostType { get; set; }

    [XmlElement("HostType")]
    public int HostTypeValue
    {
      get => (int) this.HostType;
      set => this.HostType = (TeamFoundationHostType) value;
    }
  }
}
