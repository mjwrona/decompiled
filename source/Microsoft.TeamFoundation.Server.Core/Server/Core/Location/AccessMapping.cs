// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Location.AccessMapping
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.Location
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class AccessMapping
  {
    private string m_moniker;

    public AccessMapping()
    {
    }

    public AccessMapping(
      string moniker,
      string displayName,
      string accessPoint,
      string virtualDirectory)
    {
      this.DisplayName = displayName;
      this.Moniker = moniker;
      this.AccessPoint = accessPoint;
      this.VirtualDirectory = virtualDirectory;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string DisplayName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Moniker
    {
      get => this.m_moniker;
      set => this.m_moniker = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string AccessPoint { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string VirtualDirectory { get; set; }

    internal AccessMapping Clone() => new AccessMapping(this.Moniker, this.DisplayName, this.AccessPoint, this.VirtualDirectory);
  }
}
