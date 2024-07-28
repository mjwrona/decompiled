// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceIdentityInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServiceIdentityInfo
  {
    private string m_name;
    private string m_password;
    private string m_securityIdentifier;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.Bidirectional)]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.Bidirectional)]
    public string Password
    {
      get => this.m_password;
      set => this.m_password = value;
    }

    [XmlIgnore]
    internal string SecurityIdentifier
    {
      get => this.m_securityIdentifier;
      set => this.m_securityIdentifier = value;
    }
  }
}
