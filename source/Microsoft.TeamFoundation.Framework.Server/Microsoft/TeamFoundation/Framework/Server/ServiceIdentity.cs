// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceIdentity
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServiceIdentity
  {
    private ServiceIdentityInfo m_identityInfo;
    private Microsoft.VisualStudio.Services.Identity.Identity m_identity;

    [XmlElement]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public ServiceIdentityInfo IdentityInfo
    {
      get => this.m_identityInfo;
      set => this.m_identityInfo = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public Microsoft.VisualStudio.Services.Identity.Identity Identity
    {
      get => this.m_identity;
      set => this.m_identity = value;
    }

    public static void ParseName(string name, out string accountName, out string scope)
    {
      int startIndex = name.LastIndexOf('(');
      if (startIndex >= 2)
      {
        accountName = name.Substring(0, startIndex - 1);
        scope = name.Substring(startIndex);
      }
      else
      {
        accountName = name;
        scope = (string) null;
      }
    }
  }
}
