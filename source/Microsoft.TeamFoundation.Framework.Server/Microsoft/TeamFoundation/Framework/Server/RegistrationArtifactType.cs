// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistrationArtifactType
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Public)]
  public class RegistrationArtifactType
  {
    private string m_Name;
    private OutboundLinkType[] m_ForwardLinkTypes;

    public string Name
    {
      get => this.m_Name;
      set => this.m_Name = value;
    }

    public OutboundLinkType[] OutboundLinkTypes
    {
      get => this.m_ForwardLinkTypes;
      set => this.m_ForwardLinkTypes = value;
    }

    public RegistrationArtifactType DeepClone()
    {
      RegistrationArtifactType registrationArtifactType = new RegistrationArtifactType();
      registrationArtifactType.Name = this.Name;
      OutboundLinkType[] outboundLinkTypes = this.OutboundLinkTypes;
      registrationArtifactType.OutboundLinkTypes = outboundLinkTypes != null ? ((IEnumerable<OutboundLinkType>) outboundLinkTypes).Select<OutboundLinkType, OutboundLinkType>((Func<OutboundLinkType, OutboundLinkType>) (lt => lt.DeepClone())).ToArray<OutboundLinkType>() : (OutboundLinkType[]) null;
      return registrationArtifactType;
    }
  }
}
